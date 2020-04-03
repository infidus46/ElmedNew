using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using Yamed.Control;
using Yamed.Core;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.OmsExp.ExpEditors
{
    public class SLUPACSANK
    {
        public object Row { get; set; }
        public D3_AKT_MEE_TBL AktMee { get; set; }

        public D3_SANK_OMS Sank { get; set; }
        public string S_KOD { get; set; }
        public decimal? S_PROC { get; set; }
        public decimal? S_SHTRAF { get; set; }
        public decimal? S_UM { get; set; }
        public decimal? S_SUMNP { get; set; }
        public decimal? S_SUMP { get; set; }
        public decimal? S_SANK { get; set; }
        public decimal? S_TIP2 { get; set; }
        public string ExpOrder { get; set; }

    }


    /// <summary>
    /// Логика взаимодействия для MeeWindow.xaml
    /// </summary>
    public partial class MeeWindow : UserControl
    {
        private UserControl _panel;
        private bool _isNew;
        D3_AKT_MEE_TBL _meeSank;
        private decimal? _sump;
        private ObservableCollection<DynamicBaseClass> _sankAutos;
        private List<SLUPACSANK> _slpsList;

        private ElmedDataClassesDataContext _dc1;

        private int _stype;

        private int? _sid;
        private object _row;
        private int _re;

        public MeeWindow(int stype, int? sid = null, object row = null, int re = 0)
        {
            InitializeComponent();
            _stype = stype;
            _sid = sid;
            _row = row;
            _re = re;

            _isNew = sid == null;

            //if (_stype == 3)
            {
                ExpertBoxEdit.IsEnabled = true;
                Column_Expert.Visible = true;
                using (ElmedDataClassesDataContext dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
                {
                    var exp = dc.GetTable<ExpertsDB>().ToList();
                    ExpertComboBoxEditSettings.DataContext = exp;
                    ExpertBoxEdit.DataContext = exp;
                }
            }

            var videxp = ((IEnumerable<dynamic>)SprClass.TypeExp2).Where(x => ObjHelper.GetAnonymousValue(x, "EXP_TYPE") == _stype && ObjHelper.GetAnonymousValue(x, "EXP_RE") == _re).ToList();
            VidExpEdit.DataContext = videxp;
            VidExpEditSettings.DataContext = videxp;
        }

        private void SancCreate(SLUPACSANK sl)
        {
            if (sl.Sank == null)
                sl.Sank = new D3_SANK_OMS();
            sl.Sank.D3_ZSLID = (int)ObjHelper.GetAnonymousValue(sl.Row, "ID");
            sl.Sank.D3_SCID = (int)ObjHelper.GetAnonymousValue(sl.Row, "D3_SCID");
            sl.Sank.S_TIP = _re == 0 ? (int?)_stype : null;
            sl.Sank.S_TIP2 = sl.S_TIP2;
            sl.Sank.ExpOrder = sl.ExpOrder;
            sl.Sank.S_OSN = sl.AktMee.COD_PU;
            sl.Sank.S_SUM = sl.AktMee.SUMNP;
            sl.Sank.S_SUM2 = sl.AktMee.SUM_MULCT;
            sl.Sank.S_DATE = sl.AktMee.AKT_DATE;
            sl.Sank.S_COM = (sl.AktMee.ZAKL != null) ? Obrezka(sl.AktMee.ZAKL, 250) : null;
            sl.Sank.S_CODE = Guid.NewGuid().ToString();
            if (ExpertBoxEdit.SelectedItem == null) return;
            try
            {
                sl.Sank.CODE_EXP = ObjHelper.GetAnonymousValue(ExpertBoxEdit.SelectedItem, "KOD").ToString();
            }
            catch
            {

            }
            //sl.Sank.RESANK = _rsank;
        }

        public static string Obrezka(string str, int count)
        {
            if (str != null && str.Length > count)
                return str.Substring(0, count);
            return str;
        }

        private void save1()
        {
            var i = 0;
            foreach (var sl in _slpsList.Where(x => x.AktMee.SANK_AUTO_ID != null).Select(x => x))
            {
                var sl1 = sl;
                using (var dc = new YamedDataClassesDataContext(SprClass.LocalConnectionString))
                {
                    var isSankExist = dc.D3_SANK_OMS.Any(x => x.D3_ZSLID == (int)ObjHelper.GetAnonymousValue(sl1.Row, "ID") && x.S_TIP == _stype && _re == 0);
                    if (isSankExist)
                    {
                        i++;
                        continue;
                    }
                }
                SancCreate(sl);
                using (var dc = new YamedDataClassesDataContext(SprClass.LocalConnectionString))
                {
                    //dc.SANK.InsertOnSubmit(sl.Sank);
                    //dc.SubmitChanges();
                    int sid = Reader2List.ObjectInsertCommand("D3_SANK_OMS", sl.Sank, "ID", SprClass.LocalConnectionString);

                    //dc.Sank_Calc_(dc.SLUCH.Single(x => x.ID == (int)ObjHelper.GetAnonymousValue(sl.Row, "ID")).ID);
                    //dc.Sluch_Calc_(dc.SLUCH.Single(x => x.ID == (int)ObjHelper.GetAnonymousValue(sl.Row, "ID")).ID);
                    sl.AktMee.SANKID = sid;
                    var aktm = dc.D3_AKT_MEE_TBL.OrderByDescending(x => x.ID).FirstOrDefault();
                    if (aktm != null)
                        sl.AktMee.AKT_NUMBER = aktm.AKT_NUMBER + 1;
                    else sl.AktMee.AKT_NUMBER = 1;
                }
                //sl.SLP.CHECK_SL = false;
                using (SqlBulkCopy bulkCopy =
                new SqlBulkCopy(
                    SprClass.LocalConnectionString,
                    SqlBulkCopyOptions.CheckConstraints))
                {
                    bulkCopy.BulkCopyTimeout = 0;
                    bulkCopy.DestinationTableName = "dbo.D3_AKT_MEE_TBL";
                    bulkCopy.WriteToServer(new List<D3_AKT_MEE_TBL>() { sl.AktMee }.AsDataReader());
                }
                //using (var dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
                //{
                //    var sls = dc.SLUCH.Single(x => x.ID == sl.SLP.ID);
                //    sl.SLP.SUMP = sls.SUMP;
                //    sl.SLP.SANK_IT = sls.SANK_IT;
                //    sl.SLP.KSG_OPLATA__Disp = sls.F005.NameWithId;
                //}

            }
            if (i > 0)
                DXMessageBox.Show($"{i} МЭЭ отклоненно как дубли !!!");
        }


        public static decimal? SumpCalc(object row)
        {
            using (var dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
            {
                CalcAmbTARIF t = null;

                if (ObjHelper.GetAnonymousValue(row, "OS_SLUCH_REGION") != null)
                {
                    t =
                        dc.CalcAmbTARIF.FirstOrDefault(
                            x => x.USL_OK == (int)ObjHelper.GetAnonymousValue(row, "USL_OK") && x.OS_SLUCH == (int)ObjHelper.GetAnonymousValue(row, "OS_SLUCH_REGION") &&
                                 ((DateTime)ObjHelper.GetAnonymousValue(row, "DATE_Z_2") >= x.TBEG && (DateTime)ObjHelper.GetAnonymousValue(row, "DATE_Z_2") < x.TEND.AddDays(1)));
                    if (t != null && new int[] { 2, 8, 9, 12, 14, 15, 16, 18, 23 }.Contains((int)ObjHelper.GetAnonymousValue(row, "OS_SLUCH_REGION")))
                    {
                        return t.TARIF2;
                    }

                    return (decimal)0.00;
                }
                else
                {
                    t = dc.CalcAmbTARIF.FirstOrDefault(
                        x => x.USL_OK == (int)ObjHelper.GetAnonymousValue(row, "USL_OK") && x.PROFIL == (int?)ObjHelper.GetAnonymousValue(row, "PROFIL") && x.OS_SLUCH == null &&
                             ((DateTime)ObjHelper.GetAnonymousValue(row, "DATE_Z_2") >= x.TBEG && (DateTime)ObjHelper.GetAnonymousValue(row, "DATE_Z_2") < x.TEND.AddDays(1)));

                    decimal ? ta = 0;
                    if (t != null)
                    {
                        switch ((string)ObjHelper.GetAnonymousValue(row, "P_CEL"))
                        {
                            case "1.1":
                                ta = t.TARIF1;
                                break;
                            case "1.2":
                                ta = t.TARIF2;
                                break;
                            case "1.3":
                                ta = t.TARIF3;
                                break;
                            case "2.0":
                                ta = t.TARIF4;
                                break;
                            case "3.0":
                                ta = t.TARIF5;
                                break;
                            default:
                                ta = t.TARIF1;
                                break;
                        }
                    }
                    //if (sl.IDSP == 9)
                    //    ta =
                    //        Math.Round(
                    //            (decimal)ta *
                    //            Math.Round((decimal)sl.ED_COL * (decimal)0.25, 2, MidpointRounding.AwayFromZero), 2,
                    //            MidpointRounding.AwayFromZero);
                    return ta;
                }
            }
        }


        private void BarButtonItem1_OnItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (_isNew)
            {
                save1();
            }
            else
            {
                foreach (var sl in _slpsList.Where(x => x.AktMee.SANK_AUTO_ID != null))
                {
                    SancCreate(sl);
                }
                //_dc1.SubmitChanges(ConflictMode.ContinueOnConflict);
                Reader2List.CustomExecuteQuery(Reader2List.CustomUpdateCommand<D3_AKT_MEE_TBL>("D3_AKT_MEE_TBL",
                    _slpsList.Where(x => x.AktMee.SANK_AUTO_ID != null).Select(x => x.AktMee).ToList(), "ID"), SprClass.LocalConnectionString);
                Reader2List.CustomExecuteQuery(Reader2List.CustomUpdateCommand<D3_SANK_OMS>("D3_SANK_OMS",
                    _slpsList.Where(x => x.AktMee.SANK_AUTO_ID != null).Select(x => x.Sank).ToList(), "ID"), SprClass.LocalConnectionString);
            }

            var scs = _slpsList.Select(x => ObjHelper.GetAnonymousValue(x.Row, "D3_SCID")).Distinct();

            if (_re == 0)
            {
                foreach (var sc in scs)
                {
                    Reader2List.CustomExecuteQuery($@"EXEC p_oms_calc_sank {sc}; EXEC p_oms_calc_schet {sc};", SprClass.LocalConnectionString);
                }
            }
            else
            {
                foreach (var sc in scs)
                {
                    Reader2List.CustomExecuteQuery($@"EXEC p_oms_calc_sank_ {sc}; EXEC p_oms_calc_schet {sc};", SprClass.LocalConnectionString);
                }
            }
            ((DXWindow) this.Parent).Close();
        }

        private int _rowHandle;

        private void ShablonComboBoxEdit_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (ShablonComboBoxEdit.SelectedItem == null) return;

            var rhs = sluchGridControl.GetSelectedRowHandles();
            foreach (var rh in rhs)
            {
                if (_re == 1 && (int)ObjHelper.GetAnonymousValue(ShablonComboBoxEdit.SelectedItem, "Penalty_1") == -100)
                {
                    var row = (SLUPACSANK) sluchGridControl.GetRow(rh);
                    var result = (IList) Reader2List.CustomAnonymousSelect(
                        $@"Select top 1 ISNULL(ak.SANK_AUTO_ID, sa.MODEL_ID) SANK_AUTO_ID
	FROM D3_SANK_OMS sa
	left join D3_AKT_MEE_TBL ak on sa.ID = ak.SANKID
		where D3_ZSLID = {ObjHelper.GetAnonymousValue(row.Row, "ID")} AND S_TIP = {_stype} order by sa.ID desc",
                        SprClass.LocalConnectionString);
                    var sai = (int?) ObjHelper.GetAnonymousValue(result[0], "SANK_AUTO_ID");

                    if (sai != null)
                    {
                        sluchGridControl.SetCellValue(rh, "AktMee.SANK_AUTO_ID", sai);
                    }
                }
                else
                {
                    sluchGridControl.SetCellValue(rh, "AktMee.SANK_AUTO_ID",
                        ObjHelper.GetAnonymousValue(ShablonComboBoxEdit.SelectedItem, "ID"));
                }
            }
        }

        private void ExpertBoxEdit_OnSelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (ExpertBoxEdit.SelectedItem == null) return;

            var rhs = sluchGridControl.GetSelectedRowHandles();
            foreach (var rh in rhs)
            {
                sluchGridControl.SetCellValue(rh, "AktMee.ExpertID",
                    ObjHelper.GetAnonymousValue(ExpertBoxEdit.SelectedItem, "Id"));
            }

        }


        private void View1_OnCellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (Equals(e.Column, Column_S_SUMNP))
            {
                var slp = (SLUPACSANK)sluchGridControl.SelectedItem;
                slp.AktMee.SUMNP = (decimal)SumnpEdit.EditValue;
                slp.AktMee.SUMP = slp.AktMee.SUMV - slp.AktMee.SUMNP;
                //slp.Sank.S_SUM = slp.AktMee.SUMNP;
                sluchGridControl.DataController.RefreshData();
            }

            //if (Equals(e.Column, Column_CHECK_SL))
            //{
            //    if (_headerCheckEdit == null) _headerCheckEdit = HeaderTemplateFind();
            //    _headerCheckEdit.IsChecked = null;
            //}

            if (Equals(e.Column, Column_SANK_AUTO_ID))
            {
                if (e.Value == null)
                {
                    var slpn = (SLUPACSANK)e.Row;
                    slpn.AktMee.SUMNP = null;
                    slpn.AktMee.SUMP = null;
                    return;
                }

                var auto = _sankAutos.Single(x => (int) x.GetValue("ID") == (int)e.Value);


                var slps = (SLUPACSANK)e.Row;
                //slps.AktMee.F014 = auto.F014;
                slps.AktMee.COD_PU = (string)auto.GetValue("Osn");
                //slps.AktMee.COD_PU_TS = (string)auto.GetValue("S_OSN_TS");

                slps.AktMee.SANK_AUTO_ID = (int?)auto.GetValue("ID");
                slps.AktMee.SUM_PR = (int?)auto.GetValue("Penalty_1");

                var today = DateTime.Today.ToString("yyyyMMdd");
                var p2 = (decimal?)SqlReader.Select(
    $@"SELECT TOP 1 [BASEST]
  FROM [dbo].[Yamed_ExpSpr_Penalty]
  where smocod = (SELECT TOP 1[Parametr]
  FROM [dbo].[Settings] where[name] = 'CodeSMO')
  and [d_begin] <= '{today}' and isnull([d_end], '20991231') >= '{today}'
  order by ID desc",
    SprClass.LocalConnectionString).FirstOrDefault()?.GetValue("BASEST");


                slps.AktMee.SUM_MULCT = (int?)auto.GetValue("Penalty_2") * p2 / 100;
                //slps.AktMee.ZAKL = auto.S_ZAKL;

                decimal ? sump;
                if (_re == 0)
                {
                    if (ObjHelper.GetAnonymousValue(slps.Row, "SUMP") == null || (decimal)ObjHelper.GetAnonymousValue(slps.Row, "SUMP") == 0)
                        sump = slps.AktMee.SUMV;
                    else
                        sump = (decimal)ObjHelper.GetAnonymousValue(slps.Row, "SUMP");

                    slps.AktMee.SUMNP = Math.Round((decimal)sump * (int)auto.GetValue("Penalty_1") / 100, 2, MidpointRounding.AwayFromZero);
                    slps.AktMee.SUMP = (decimal)ObjHelper.GetAnonymousValue(slps.Row, "SUMV") - slps.AktMee.SUMNP;
                }
                else if (_re == 1 && (int) ObjHelper.GetAnonymousValue(ShablonComboBoxEdit.SelectedItem, "Penalty_1") == -100)
                {
                    slps.AktMee.SUMNP = 0;
                    slps.AktMee.SUMP = (decimal) ObjHelper.GetAnonymousValue(slps.Row, "SUMP");
                }
                else
                {
                    sump = slps.AktMee.SUMV;
                    var rsum = sump * (int)auto.GetValue("Penalty_1") / 100;
                    if (slps.S_SANK > rsum)
                        slps.AktMee.SUMNP = -(slps.S_SANK - rsum);
                    else slps.AktMee.SUMNP = rsum - slps.S_SANK;
                    //slps.S_SUMNP = _sump * auto.S_PROCENT / 100 + auto.S_STRAF + auto.S_UMENS;
                    slps.AktMee.SUMP = sump - slps.AktMee.SUMNP;
                }

                sluchGridControl.DataController.RefreshData();
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (SLUPACSANK slps in sluchGridControl.SelectedItems)
            {
                slps.AktMee.ZAKL = (string)PacketZaklEdit.EditValue;
                //slps.Sank.S_COM = (PacketZaklEdit.EditValue != null) ? EconomyWindow.Obrezka((string)PacketZaklEdit.EditValue, 250) : null;
            }
            sluchGridControl.DataController.RefreshData();
        }

        private void PacketAktDateEdit_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            foreach (SLUPACSANK slps in sluchGridControl.SelectedItems)
            {
                slps.AktMee.AKT_DATE = PacketAktDateEdit.DateTime;
            }
            sluchGridControl.DataController.RefreshData();
        }

        private void BaseEdit_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            //if (((CheckEdit)sender).IsChecked == true)
            //{
            //    foreach (var slps in _slpsList)
            //    {
            //        slps.SLP.CHECK_SL = true;
            //    }
            //    sluchGridControl.DataController.RefreshData();
            //}
            //if (((CheckEdit)sender).IsChecked == false)
            //{
            //    foreach (var slps in _slpsList)
            //    {
            //        slps.SLP.CHECK_SL = false;
            //    }
            //    sluchGridControl.DataController.RefreshData();
            //}
        }

        private void View1_OnCellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            (sender as TableView).PostEditor();
        }

        private void MeeWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _sankAutos = SqlReader.Select("Select * from Yamed_ExpSpr_Sank order by Name", SprClass.LocalConnectionString);

            ShablonComboBoxEdit.DataContext = _sankAutos;
            ShablonComboBoxEditSettings.DataContext = _sankAutos;

            _slpsList = new List<SLUPACSANK>();

            if (_isNew)
            {
                using (var dc = new YamedDataClassesDataContext(SprClass.LocalConnectionString))
                {
                    foreach (var row in DxHelper.LoadedRows)
                    {
                        SLUPACSANK slupacsank = new SLUPACSANK();
                        slupacsank.Row = row;
                        _slpsList.Add(slupacsank);

                        var user = dc.Yamed_Users.Single(x => x.ID == SprClass.userId);
                        var im = user.IM.Length > 1 ? user.IM.Remove(1) : user.IM;
                        var ot = user.OT.Length > 1 ? user.OT.Remove(1) : user.OT;

                        slupacsank.AktMee = new D3_AKT_MEE_TBL();
                        slupacsank.AktMee.USERID = user.FAM + " " + im + ". " + ot + ".";
                        slupacsank.AktMee.SLID = (int) ObjHelper.GetAnonymousValue(slupacsank.Row, "ID");
                        if (_re == 1)
                            slupacsank.S_SANK = dc.D3_SANK_OMS.First(x => x.D3_ZSLID == slupacsank.AktMee.SLID && x.S_TIP == _stype).S_SUM;

                        slupacsank.AktMee.CARD_NUM = (string) ObjHelper.GetAnonymousValue(slupacsank.Row, "NHISTORY");
                        slupacsank.AktMee.DOCT = (string) ObjHelper.GetAnonymousValue(slupacsank.Row, "IDDOKT");
                        slupacsank.AktMee.DS1 = (string) ObjHelper.GetAnonymousValue(slupacsank.Row, "DS1");
                        slupacsank.AktMee.DS2 = (string) ObjHelper.GetAnonymousValue(slupacsank.Row, "DS2");
                        //slupacsank.AktMee.PID = (int)ObjHelper.GetAnonymousValue(slupacsank.Row, "D3_PID");

                        slupacsank.AktMee.MONAME = (string) ObjHelper.GetAnonymousValue(slupacsank.Row, "LPU");
                        slupacsank.AktMee.PERIOD1 = (DateTime) ObjHelper.GetAnonymousValue(slupacsank.Row, "DATE_Z_1");
                        slupacsank.AktMee.PERIOD2 = (DateTime) ObjHelper.GetAnonymousValue(slupacsank.Row, "DATE_Z_2");
                        slupacsank.AktMee.POLIS_NUM = (string) ObjHelper.GetAnonymousValue(slupacsank.Row, "NPOLIS");
                        //slupacsank.AktMee.SCHET_NUM = dc.D3_SCHET_OMS.Single(x => x.ID == slupacsank.SLP.SCHET_ID).NSCHET;
                        var sc = (IList)
                            Reader2List.CustomAnonymousSelect(
                                $"Select * from D3_SCHET_OMS where ID = {ObjHelper.GetAnonymousValue(slupacsank.Row, "D3_SCID")}",
                                SprClass.LocalConnectionString);
                        slupacsank.AktMee.SCHET_NUM = (string) ObjHelper.GetAnonymousValue(sc[0], "NSCHET");
                            //sc.NSCHET;

                        var usl = (int?) ObjHelper.GetAnonymousValue(slupacsank.Row, "USL_OK");
                        if ((usl == 3 || usl == 4) && (decimal) ObjHelper.GetAnonymousValue(slupacsank.Row, "SUMV") == 0)
                            slupacsank.AktMee.SUMV = SumpCalc(slupacsank.Row);
                        else slupacsank.AktMee.SUMV = (decimal) ObjHelper.GetAnonymousValue(slupacsank.Row, "SUMV");

                        slupacsank.AktMee.SMONAME =
                            Reader2List.CustomSelect<Entity.Settings>("Select * from Settings",
                                SprClass.LocalConnectionString).SingleOrDefault(x => x.Name == "OrgNaim").Parametr;
                        slupacsank.AktMee.AKT_DATE = SprClass.WorkDate;

                        if (ObjHelper.GetAnonymousValue(slupacsank.Row, "KD_Z") == null)
                        {
                            TimeSpan? kd = (DateTime) ObjHelper.GetAnonymousValue(slupacsank.Row, "DATE_Z_2") -
                                           (DateTime) ObjHelper.GetAnonymousValue(slupacsank.Row, "DATE_Z_1");
                            slupacsank.AktMee.BOL_DLIT = kd.Value.Days == 0 ? kd.Value.Days + 1 : kd.Value.Days;
                        }
                        else slupacsank.AktMee.BOL_DLIT = (int) ObjHelper.GetAnonymousValue(slupacsank.Row, "KD_Z");
                    }
                }
            }
            else
            {
                SLUPACSANK slupacsank = new SLUPACSANK();

                List<D3_SANK_OMS> sank;
                using (SqlConnection con1 = new SqlConnection(SprClass.LocalConnectionString))
                {
                    SqlCommand cmd1 = new SqlCommand(string.Format(@"Select * From D3_SANK_OMS where ID={0}", _sid), con1);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    con1.Open();

                    cmd1.CommandTimeout = 0;
                    SqlDataReader dr1 = cmd1.ExecuteReader();

                    sank = Reader2List.DataReaderMapToList<D3_SANK_OMS>(dr1);


                    dr1.Close();
                    con1.Close();
                }


                List<D3_AKT_MEE_TBL> akt;
                using (SqlConnection con1 = new SqlConnection(SprClass.LocalConnectionString))
                {
                    SqlCommand cmd1 = new SqlCommand(string.Format(@"Select * From D3_AKT_MEE_TBL where SANKID={0}", _sid), con1);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    con1.Open();

                    cmd1.CommandTimeout = 0;
                    SqlDataReader dr1 = cmd1.ExecuteReader();

                    akt = Reader2List.DataReaderMapToList<D3_AKT_MEE_TBL>(dr1);


                    dr1.Close();
                    con1.Close();
                }

                slupacsank.Row = _row;
                slupacsank.AktMee = akt.First();
                slupacsank.Sank = sank.First();
                slupacsank.S_TIP2 = sank.First().S_TIP2;
                slupacsank.ExpOrder = sank.First().ExpOrder;
                _slpsList.Add(slupacsank);
            }


            sluchGridControl.DataContext = _slpsList;//_elKard = elKard;
            //OsnComboBoxEdit.DataContext = SprClass.Otkazs;
            //KsgBoxEdit.DataContext = SprClass.KsgGroups.OrderBy(x => x.ORDERID).ToList();
            //using (ElmedDataClassesDataContext dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
            //{
            //    UserIdTextEdit.DataContext = dc.GetTable<ExpertsDB>().ToList();
            //}
        }

        private void VidExpEdit_OnSelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (VidExpEdit.SelectedItem == null) return;

            var rhs = sluchGridControl.GetSelectedRowHandles();
            foreach (var rh in rhs)
            {
                sluchGridControl.SetCellValue(rh, "S_TIP2",
                    ObjHelper.GetAnonymousValue(VidExpEdit.SelectedItem, "IDVID"));
            }
        }
    }
}
