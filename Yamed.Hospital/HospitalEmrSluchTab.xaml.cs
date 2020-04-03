using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using Yamed.Core;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.Hospital
{
    /// <summary>
    /// Логика взаимодействия для HospitalEmrSluchTab.xaml
    /// </summary>
    public partial class HospitalEmrSluchTab : UserControl
    {
        private SLUCH _zsl;
        private PACIENT _pacient;
        private D3_SCHET_OMS _schet;
        private List<SLUCH_DS2> _ds2List;
        private List<SLUCH_DS3> _ds3List;
        private List<USL_KSLP> _kslpList;
        private List<USL> _traffiList;
        private List<USL> _usl;

        public HospitalEmrSluchTab(SLUCH sluch, D3_SCHET_OMS schet)
        {
            InitializeComponent();

            _schet = schet;
            if (sluch == null)
            {
                _zsl = new SLUCH();
                _pacient = new PACIENT();
                _ds2List = new List<SLUCH_DS2>();
                _ds3List = new List<SLUCH_DS3>();
                _kslpList = new List<USL_KSLP>();

                _traffiList = new List<USL>();
                _usl = new List<USL>();
            }
            else
            {
                _zsl = sluch;
                _pacient = Reader2List.CustomSelect<PACIENT>($"SELECT * FROM PACIENT WHERE ID = {sluch.PID}", SprClass.LocalConnectionString).Single();
                _ds2List = Reader2List.CustomSelect<SLUCH_DS2>($"SELECT * FROM SLUCH_DS2 WHERE SLID = {sluch.ID}", SprClass.LocalConnectionString);
                _ds3List = Reader2List.CustomSelect<SLUCH_DS3>($"SELECT * FROM SLUCH_DS3 WHERE SLID = {sluch.ID}", SprClass.LocalConnectionString);
                _traffiList = Reader2List.CustomSelect<USL>($"SELECT * FROM USL WHERE SLID = {sluch.ID} AND (CODE_USL='TF1.K01.001' OR CODE_USL='TF1.D01.001')", SprClass.LocalConnectionString);
                _usl = Reader2List.CustomSelect<USL>($"SELECT * FROM USL WHERE SLID = {sluch.ID} AND (CODE_USL='VM1.K01.001' OR CODE_USL='VM1.D01.001')", SprClass.LocalConnectionString);
            }
            if (_schet != null)
            {
                _pacient.SCHET_ID = _schet.ID;
                _pacient.ID_PAC = Guid.NewGuid().ToString();
                _zsl.SCHET_ID = _schet.ID;
                _zsl.LPU = _schet.CODE_MO;
            }
            _zsl.DOSTP = false;

            GridPacient.DataContext = _pacient;
            GridSluch.DataContext = _zsl;
            Ds2GridControl.DataContext = _ds2List;
            //Ds3GridControl.DataContext = _ds3List;

            TrafficGridControl.DataContext = _traffiList;
            UslGridControl.DataContext = _usl;
        }

        void SprInit()
        {
            typeHelpBox.DataContext = SprClass.typeHelp;
            conditionHelpBox.DataContext = SprClass.conditionHelp.Where(x => x.id == 1 || x.id == 2);
            KDostBox.DataContext = SprClass.KDostList;
            VOplBox.DataContext = SprClass.VOplList;
            DefGospBox.DataContext = SprClass.DefGospList;
            DoctorBox.DataContext = SprClass.DoctList;
            DoctorPBox.DataContext = SprClass.DoctList;
            Ds2ColumnEdit.DataContext = SprClass.mkbSearching;
            //Ds3ColumnEdit.DataContext = SprClass.mkbSearching;
            KslpCodeColumnEdit.DataContext = SprClass.KslpList;

            PodrColumnEdit.DataContext = SprClass.Podr;
            OtdelColumnEdit.DataContext = SprClass.OtdelDbs;
            //ProfilColumnEdit.DataContext = SprClass.profile;
            DoctorColumnEdit.DataContext = SprClass.DoctList;
            //SpecColumnEdit.DataContext = SprClass.specialityNew;
            DsColumnEdit.DataContext = SprClass.mkbSearching;
            KsgColumnEdit.DataContext = Reader2List.CustomAnonymousSelect($@"select * from V023", SprClass.LocalConnectionString);
            OplataTypeColumnEdit.DataContext = SprClass.KsgOplata;

            UslOtdelColumnEdit.DataContext = SprClass.OtdelDbs;
            UslProfilColumnEdit.DataContext = SprClass.profile;

            UslDoctorColumnEdit.DataContext = SprClass.DoctList;
            UslDsColumnEdit.DataContext = SprClass.mkbSearching;
            UslVidVmeColumnEdit.DataContext = SprClass.SprUsl804;

            //profilBox.DataContext = SprClass.profile;
            //codeSpecialityBox.DataContext = SprClass.mspList;
            //typeSluchBox.DataContext = SprClass.typeSluch;
            //childFrofilBox.DataContext = SprClass.detProf;
            //idspBox.DataContext = SprClass.tarifUsl;
            Ds1Box.DataContext = SprClass.mkbSearching.Where(x => !x.ISDELETE).ToList();
            ds0Box.DataContext = SprClass.mkbSearching.Where(x => !x.ISDELETE).ToList();
            DsNaprBox.DataContext = SprClass.mkbSearching.Where(x => !x.ISDELETE).ToList();
            DspaBox.DataContext = SprClass.mkbSearching.Where(x => !x.ISDELETE).ToList();
            DssBox.DataContext = SprClass.mkbSearching.Where(x => !x.ISDELETE).ToList();
            //UslChildFrofilBox.DataContext = SprClass.detProf;
            //UslCodeBox.DataContext = SprClass.Nomenclatures;
            //UslCodeSpecialityBox.DataContext = SprClass.mspList;
            //UslDoctorTextBox.DataContext = SprClass.doctorBd;
            //UslMkbBox.DataContext = SprClass.mkbSearching.Where(x => x.ISDELETE == false).ToList();
            //UslProfilBox.DataContext = SprClass.profile;


            osobSluchBox.DataContext = SprClass.OsobSluchDbs;
            napravlMedOrgBox.DataContext = SprClass.medOrg;
            napravlBox.DataContext = SprClass.ExtrDbs;
            //codePodrazdlBox.DataContext = SprClass.Podr;
            //ksgTypeBox.DataContext = SprClass.KsgOts.Where(x => x.Id == 1 || x.Id == 2);
            //OperCodeBox.DataContext = SprClass.Nomenclatures;


            GospitBox.DataContext = SprClass.Gospits.Where(x => x.ID == 1 || x.ID == 2).ToList();
            DostavBox.DataContext = SprClass.Dostavs;
            TravmaBox.DataContext = SprClass.TravmaTbls;
            //ksgBox.DataContext = SprClass.KsgGroups;

            HVidBox.DataContext = SprClass.VidVmpList;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new DXWindow
            {
                ShowIcon = false, WindowStartupLocation = WindowStartupLocation.CenterScreen,
                SizeToContent = SizeToContent.WidthAndHeight,
                Content = new HospitalEmrPacientPanel(_pacient)
            };
            window.ShowDialog();
        }

        private void mkbBox_GotFocus(object sender, RoutedEventArgs e)
        {
            InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("en-US");
        }

        private void mkbBox_LostFocus(object sender, RoutedEventArgs e)
        {
            InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("ru-RU");
        }

        private void ButtonInfo_OnClick(object sender, RoutedEventArgs e)
        {
            //var si = KDostBox.SelectedItem;
            //var window = new DXWindow
            //{
            //    ShowIcon = false, WindowStartupLocation = WindowStartupLocation.CenterScreen,
            //    SizeToContent = SizeToContent.WidthAndHeight,
            //    Content = new UniSprTab("SprKDOST", SprClass.LocalConnectionString, ref si),
            //    Title = "SprKDOST"
            //};
            //window.ShowDialog();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveSluch();
        }

        void SaveSluch()
        {
            if (!_traffiList.Any())
            {
                DXMessageBox.Show("Не заполнено движение");
                return;
            }

            if (_pacient.ID == 0)
            {
                _pacient.SCHET_ID = _schet.ID;
                _pacient.ID = Reader2List.ObjectInsertCommand("PACIENT", _pacient, "ID", SprClass.LocalConnectionString);
            }
            else
            {
                Reader2List.CustomExecuteQuery(Reader2List.CustomUpdateCommand("PACIENT", _pacient, "ID"), SprClass.LocalConnectionString);
            }
            _zsl.PID = _pacient.ID;

            //if (_traffiList[0].IDKSG == null)
            //{
            //    if (_zsl.USL_OK == 1)
            //        KsgGroping();
            //    else
            //        KsgGroping2();
            //}

            if (_zsl.ID == 0)
            {
                _zsl.IDSLG = Guid.NewGuid();
                _zsl.PODR = _traffiList.Last()?.PODR;
                _zsl.LPU_1 = _traffiList.Last()?.LPU_1;
                _zsl.PROFIL = _traffiList.Last()?.PROFIL;
                _zsl.DET = _traffiList.Last()?.DET;
                _zsl.MSPID = _traffiList.Last()?.MSPUID;
                _zsl.IDSP = _zsl.USL_OK == 1 ? 33 : 43;

                _zsl.ID = Reader2List.ObjectInsertCommand("SLUCH", _zsl, "ID", SprClass.LocalConnectionString);
            }
            else
            {
                _zsl.PODR = _traffiList.Last()?.PODR;
                _zsl.LPU_1 = _traffiList.Last()?.LPU_1;
                _zsl.PROFIL = _traffiList.Last()?.PROFIL;
                _zsl.DET = _traffiList.Last()?.DET;
                _zsl.MSPID = _traffiList.Last()?.MSPUID;
                _zsl.IDSP = _zsl.USL_OK == 1 ? 33 : 43;

                Reader2List.CustomExecuteQuery(Reader2List.CustomUpdateCommand("SLUCH", _zsl, "ID"), SprClass.LocalConnectionString);
            }

            foreach (var ds2 in _ds2List)
            {
                ds2.SLID = _zsl.ID;
                if (ds2.ID == 0)
                    ds2.ID = Reader2List.ObjectInsertCommand("SLUCH_DS2", ds2, "ID",
                        SprClass.LocalConnectionString);
                else
                    Reader2List.CustomExecuteQuery(Reader2List.CustomUpdateCommand("SLUCH_DS2", ds2, "ID"), SprClass.LocalConnectionString);
            }

            //foreach (var ds3 in _ds3List)
            //{
            //    ds3.SLID = _zsl.ID;
            //    if (ds3.ID == 0)
            //        ds3.ID = Reader2List.ObjectInsertCommand("SLUCH_DS3", ds3, "ID",
            //            SprClass.LocalConnectionString);
            //    else
            //        Reader2List.CustomExecuteQuery(Reader2List.CustomUpdateCommand("SLUCH_DS3", ds3, "ID"), SprClass.LocalConnectionString);
            //}

            foreach (var kslp in _kslpList)
            {
                if (kslp.SLID == 0) kslp.SLID = _traffiList.First().ID;
                if (kslp.ID == 0)
                    kslp.ID = Reader2List.ObjectInsertCommand("USL_KSLP", kslp, "ID",
                        SprClass.LocalConnectionString);
                else
                    Reader2List.CustomExecuteQuery(Reader2List.CustomUpdateCommand("USL_KSLP", kslp, "ID"), SprClass.LocalConnectionString);
            }


            foreach (var tr in _traffiList)
            {
                tr.CODE_USL = _zsl.USL_OK == 1 ? "TF1.K01.001" : "TF1.D01.001";
                if (tr.DIFF_K == null || tr.DIFF_K == 0) tr.DIFF_K = 1;

                tr.SLID = _zsl.ID;
                if (tr.ID == 0)
                    tr.ID = Reader2List.ObjectInsertCommand("USL", tr, "ID",
                        SprClass.LocalConnectionString);
                else
                    Reader2List.CustomExecuteQuery(Reader2List.CustomUpdateCommand("USL", tr, "ID"), SprClass.LocalConnectionString);
            }

            foreach (var u in _usl)
            {
                u.CODE_USL = _zsl.USL_OK == 1 ? "VM1.K01.001" : "VM1.D01.001";

                u.SLID = _zsl.ID;
                if (u.ID == 0)
                    u.ID = Reader2List.ObjectInsertCommand("USL", u, "ID",
                        SprClass.LocalConnectionString);
                else
                    Reader2List.CustomExecuteQuery(Reader2List.CustomUpdateCommand("USL", u, "ID"), SprClass.LocalConnectionString);
                foreach (var assist1 in u.USL_ASSIST)
                {
                    var assist = new USL_ASSIST
                    {
                        UID = u.ID,
                        IDDOCT = assist1.IDDOCT
                    };
                    if (assist.ID == 0)
                        assist.ID = Reader2List.ObjectInsertCommand("USL_ASSIST", assist, "ID",
                            SprClass.LocalConnectionString);
                    else
                        Reader2List.CustomExecuteQuery(Reader2List.CustomUpdateCommand("USL_ASSIST", u, "ID"), SprClass.LocalConnectionString);
                }
            }

        }

        private void Ds3AddItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //_ds3List.Add(new SLUCH_DS3());
            var id = ((USL)TrafficGridControl.SelectedItem).ID;

            _kslpList.Add(new USL_KSLP() {SLID=id});

            Ds3GridControl.RefreshData();
        }

        private void Ds3DelItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var del = (SLUCH_DS3) Ds3GridControl.SelectedItem;

            //if (del.ID == 0)
            //{
            //    _ds3List.Remove(del);
            //}
            //else
            //{
            //    Reader2List.CustomExecuteQuery($"DELETE SLUCH_DS3 WHERE ID = {del.ID}", SprClass.LocalConnectionString);
            //    _ds3List.Remove(del);
            //}
            //Ds3GridControl.RefreshData();

            var del = (USL_KSLP)Ds3GridControl.SelectedItem;

            if (del.ID == 0)
            {
                _kslpList.Remove(del);
            }
            else
            {
                Reader2List.CustomExecuteQuery($"DELETE USL_KSLP WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                _kslpList.Remove(del);
            }
            Ds3GridControl.RefreshData();


        }

        private void Ds2AddItem_OnItemClickem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            _ds2List.Add(new SLUCH_DS2());
            Ds2GridControl.RefreshData();
        }

        private void Ds2DelItem_OnItemClickm_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var del = (SLUCH_DS2) Ds2GridControl.SelectedItem;

            if (del.ID == 0)
            {
                _ds2List.Remove(del);
            }
            else
            {
                Reader2List.CustomExecuteQuery($"DELETE SLUCH_DS2 WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                _ds2List.Remove(del);
            }
            Ds2GridControl.RefreshData();
        }

        private void TrafficAddItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var usl = new USL {SCHET_ID = _schet.ID, KSGOPLATA = 1, LPU = _schet.CODE_MO};
            _traffiList.Add(usl);
            TrafficGridControl.RefreshData();
            var window = new DXWindow
            {
                ShowIcon = false, WindowStartupLocation = WindowStartupLocation.CenterScreen,
                SizeToContent = SizeToContent.Height,
                Content = new HospitalEmrTrafficPanel(usl)
            };
            window.ShowDialog();
        }

        private void TrafficEditItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var usl = (USL)TrafficGridControl.SelectedItem;
            var window = new DXWindow
            {
                ShowIcon = false, WindowStartupLocation = WindowStartupLocation.CenterScreen,
                SizeToContent = SizeToContent.Height,
                Content = new HospitalEmrTrafficPanel(usl)
            };
            window.ShowDialog();
            TrafficGridControl.RefreshData();
        }

        private void TrafficDelItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var del = (USL) TrafficGridControl.SelectedItem;

            if (del.ID == 0)
            {
                _traffiList.Remove(del);
            }
            else
            {
                Reader2List.CustomExecuteQuery($"DELETE USL WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                _traffiList.Remove(del);
            }
            TrafficGridControl.RefreshData();
        }

        private void HospitalEmrSluchTab_OnLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(SprInit), System.Windows.Threading.DispatcherPriority.Render);

            if (_pacient.ID == 0)
            {
                var window = new DXWindow
                {
                    ShowIcon = false, WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    SizeToContent = SizeToContent.Height,
                    Content = new HospitalEmrPacientPanel(_pacient)
                };
                window.ShowDialog();
            }
        }

        private void UslAddItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var usl = new USL { SCHET_ID = _schet.ID, LPU = _schet.CODE_MO};
            _usl.Add(usl);
            UslGridControl.RefreshData();
            var window = new DXWindow
            {
                ShowIcon = false, WindowStartupLocation = WindowStartupLocation.CenterScreen,
                SizeToContent = SizeToContent.Height,
                Content = new HospitalEmrUslugiPanel(usl)
            };
            window.ShowDialog();
        }

        private void UslEditItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var usl = (USL)UslGridControl.SelectedItem;
            var window = new DXWindow
            {
                ShowIcon = false, WindowStartupLocation = WindowStartupLocation.CenterScreen,
                SizeToContent = SizeToContent.Height,
                Content = new HospitalEmrUslugiPanel(usl)
            };
            window.ShowDialog();
            UslGridControl.RefreshData();
        }

        private void UslDelItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var del = (USL)UslGridControl.SelectedItem;

            if (del.ID == 0)
            {
                _usl.Remove(del);
            }
            else
            {
                foreach (var assist in del.USL_ASSIST)
                {
                    Reader2List.CustomExecuteQuery($"DELETE USL_ASSIST WHERE ID = {assist.ID}", SprClass.LocalConnectionString);
                }

                Reader2List.CustomExecuteQuery($"DELETE USL WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                _usl.Remove(del);
            }
            UslGridControl.RefreshData();
        }

        private void SaveCloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveSluch();
            ((DXWindow)this.Parent).Close();
        }

        void SaveNewSluch()
        {
            SaveSluch();
            _zsl = new SLUCH();
            _pacient = new PACIENT();
            _ds2List = new List<SLUCH_DS2>();
            _ds3List = new List<SLUCH_DS3>();
            _kslpList = new List<USL_KSLP>();
            _traffiList = new List<USL>();
            _usl = new List<USL>();

            if (_schet != null)
            {
                _pacient.SCHET_ID = _schet.ID;
                _zsl.SCHET_ID = _schet.ID;
                _zsl.LPU = _schet.CODE_MO;
            }
            _zsl.DOSTP = false;

            GridPacient.DataContext = _pacient;
            GridSluch.DataContext = _zsl;
            Ds2GridControl.DataContext = _ds2List;
            Ds3GridControl.DataContext = _kslpList;
            TrafficGridControl.DataContext = _traffiList;
            UslGridControl.DataContext = _usl;

            if (_pacient.ID == 0)
            {
                var window = new DXWindow
                {
                    ShowIcon = false, WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    SizeToContent = SizeToContent.Height,
                    Content = new HospitalEmrPacientPanel(_pacient)
                };
                window.ShowDialog();
            }

        }
        private void SaveNewButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveNewSluch();
        }

        private void HospitalEmrSluchTab_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F6)
            {
                SaveSluch();
                ((DXWindow)this.Parent).Close();
            }
            if (e.Key == Key.F7)
            {
                SaveNewSluch();
            }
        }

        private void ConditionHelpBox_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            helpResultBox.DataContext = SprClass.helpResult.Where(x=>x.DL_USLOV == (int?)e.NewValue && (x.DATEEND == null || x.DATEEND >= DateTime.Today)).ToList();
            helpExitBox.DataContext = SprClass.helpExit.Where(x=>x.id.ToString().StartsWith(e.NewValue.ToString())).ToList();
        }

        private void HVidBox_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            HMetodBox.DataContext =
                SprClass.MetodVmpList.Where(x => x.HVID == (string) e.NewValue ).ToList();
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Add)
            {
                var usl = new USL { SCHET_ID = _schet.ID, KSGOPLATA = 1, LPU = _schet.CODE_MO};
                _traffiList.Add(usl);
                TrafficGridControl.RefreshData();
                var window = new DXWindow
                {
                    ShowIcon = false, WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    SizeToContent = SizeToContent.Height,
                    Content = new HospitalEmrTrafficPanel(usl)
                };
                window.ShowDialog();
            }
        }

        private void KsgButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveSluch();
            var ksg = Reader2List.CustomAnonymousSelect(
                $@"
--declare @p1 int = 15260


declare @CalcBaseKoefDS table(
	baseKoefDS [numeric](6, 4) NOT NULL,
	[TBEG] [datetime] NOT NULL,
	[TEND] [datetime] NOT NULL)

Insert @CalcBaseKoefDS VALUES (1, '20170101', '20170331')
Insert @CalcBaseKoefDS VALUES (1, '20170401', '20170630')
Insert @CalcBaseKoefDS VALUES (1, '20170701', '20171231')
Insert @CalcBaseKoefDS VALUES (1, '20180101', '20181231')
Insert @CalcBaseKoefDS VALUES (1, '20190101', '20191231')

declare @temp_ksg table(
	[USL_OK] int null,
	[SLID] [int] NULL,
	[ID] [int] NULL,
	[vm_ID] [int] NULL,
	[NKSG] [nvarchar](20) NULL,
	[IDKSG] [nvarchar](20) NOT NULL,
	[USL] [nvarchar](16) NULL,
	[DS1] [nvarchar](10) NULL,
	[DS2] [nvarchar](10) NULL,
	[VOZR] [int] NULL,
	[POL] [int] NULL,
	[DLIT] [int] NULL,
	[tf_idksg] [nvarchar](20) NULL,
	[tf_ds] [nvarchar](10) NULL,
	[sl_ds2] [nvarchar](10) NULL,
	[vm_vid_vme] [nvarchar](16) NULL,
	[tf_kday] [int] NULL,
	[tf_sumv_usl] [numeric](15, 2) NULL,
	[tf_tarif] [numeric](15, 2) NULL,
	[sl_sumv] [numeric](15, 2) NULL,
	[tf_podr] nvarchar(20),
	--[tf_date_in] datetime,
	--[tf_date_out] datetime,
	[KDAY] [int] NULL,
	[SUMV_USL] [numeric](10, 2) NULL,
	[TARIF] [numeric](10, 2) NULL
)

declare @temp_ksg_itog table(
	sur_ksg int default (0),
	rn int,
	[USL_OK] int null,
	[SLID] [int] NULL,
	[ID] [int] NULL,
	[vm_ID] [int] NULL,
	[NKSG] [nvarchar](20) NULL,
	[IDKSG] [nvarchar](20) NOT NULL,
	[USL] [nvarchar](16) NULL,
	[DS1] [nvarchar](10) NULL,
	[DS2] [nvarchar](10) NULL,
	[VOZR] [int] NULL,
	[POL] [int] NULL,
	[DLIT] [int] NULL,
	[tf_idksg] [nvarchar](20) NULL,
	[tf_ds] [nvarchar](10) NULL,
	[sl_ds2] [nvarchar](10) NULL,
	[vm_vid_vme] [nvarchar](16) NULL,
	[tf_kday] [int] NULL,
	[tf_sumv_usl] [numeric](15, 2) NULL,
	[tf_tarif] [numeric](15, 2) NULL,
	[sl_sumv] [numeric](15, 2) NULL,
	[tf_podr] nvarchar(20),
	--[tf_date_in] datetime,
	--[tf_date_out] datetime,
	[KDAY] [int] NULL,
	[SUMV_USL] [numeric](10, 2) NULL,
	[TARIF] [numeric](10, 2) NULL
)

Insert @temp_ksg
select sl.USL_OK, tf.SLID, tf.ID, vm.ID vm_id, ksg.NKSG, t.ID as IDKSG, ksg.USL, ksg.DS1, ksg.DS2, ksg.VOZR as VOZR, ksg.POL, ksg.DLIT,
	tf.idksg tf_idksg, tf.DS tf_ds, ds2.ds sl_ds2,  vm.VID_VME vm_vid_vme, tf.kday tf_kday, tf.sumv_usl tf_sumv_usl, tf.tarif tf_tarif, sl.sumv sl_sumv, tf.podr tf_podr,-- tf.DATE_IN tf_date_in, tf.DATE_OUT tf_date_out, 
						KDAY = 
							(CASE 
							  WHEN sl.USL_OK = 1 THEN (CASE WHEN tf.DATE_IN = tf.DATE_OUT THEN 1 ELSE DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) END)
							  WHEN sl.USL_OK = 2 THEN DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) + 1 -
								(SELECT COUNT(*) FROM dbo.WORK_DAY wd WHERE wd.LPU = tf.LPU and wd.PODR_ID = tf.LPU_1 and H_DATE BETWEEN tf.DATE_IN AND tf.DATE_OUT)
                            END),
						SUMV_USL =
                            CAST(ROUND((CASE 
                            WHEN sl.USL_OK = 1 and t2.[SSL] > 0 THEN 
                            (CASE 
								WHEN (sl.RSLT in (102, 105, 106, 107, 108, 110)) THEN 
                            		(CASE
									    WHEN tf.IDKSG in (1634, 1655, 1656) and DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) < 4
											THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.80, 2)
									    WHEN tf.IDKSG in (1634, 1655, 1656) and DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) >= 4
											THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 1.00, 2)

                            			WHEN DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) < 4
											THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.20, 2)
										ELSE ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.50, 2)
									END)
								WHEN (sl.RSLT not in (102, 105, 106, 107, 108, 110)) THEN 
                            		(CASE 
									    WHEN tf.IDKSG in (1634, 1655, 1656) and DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) < 4
											THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.80, 2)
									    WHEN tf.IDKSG in (1634, 1655, 1656) and DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) >= 4
											THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 1.00, 2)

                            			WHEN DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) < 4
											THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.20, 2)
										ELSE ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 1.00, 2)
									END)
							END)
 
                            WHEN sl.USL_OK = 1 and (t2.[SSL] = -4) THEN 
                            (CASE 
                            WHEN (sl.RSLT in (102, 105, 106, 107, 108, 110)) THEN 
                            	(CASE 
                            		WHEN DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) < 4
										THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.80, 2)
									ELSE ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) ,2) 
								END)
							WHEN (sl.RSLT not in (102, 105, 106, 107, 108, 110)) THEN 
                            	(CASE 
                            		WHEN DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) < 4
										THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.80, 2)
									ELSE ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) ,2) 
								END)
							END)

							WHEN sl.USL_OK = 1 and (t2.[SSL] = 0 OR t2.[SSL] < -4)
							THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) ,2)

                            WHEN sl.USL_OK = 2 and t2.[SSL] > 0 THEN
                            (CASE 
								WHEN (sl.RSLT in (202, 205, 206, 207, 208)) THEN 
								   (CASE 
								   WHEN (DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) + 1 - 
												(SELECT COUNT(*) FROM dbo.WORK_DAY wd
												 WHERE wd.LPU = tf.LPU and LPU=tf.LPU and wd.PODR_ID = tf.LPU_1 and H_DATE BETWEEN tf.DATE_IN AND tf.DATE_OUT)) < 4
											THEN ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00)* 0.20, 2)
											ELSE ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00)* 0.50, 2)
									END)
								WHEN (sl.RSLT not in (202, 205, 206, 207, 208)) THEN 
								   (CASE 
								   WHEN (DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) + 1 - 
												(SELECT COUNT(*) FROM dbo.WORK_DAY wd
												 WHERE wd.LPU = tf.LPU and LPU=tf.LPU and wd.PODR_ID = tf.LPU_1 and H_DATE BETWEEN tf.DATE_IN AND tf.DATE_OUT)) < 4
											THEN ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00)* 0.20, 2)
											ELSE ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00)* 1.00, 2)
									END)
                            END)                        
                           
                            WHEN sl.USL_OK = 2 and t2.[SSL] = -4 THEN
                            (CASE 
								WHEN (sl.RSLT in (202, 205, 206, 207, 208)) THEN 
								   (CASE 
								   WHEN (DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) + 1 - 
												(SELECT COUNT(*) FROM dbo.WORK_DAY wd
												 WHERE wd.LPU = tf.LPU and LPU=tf.LPU and wd.PODR_ID = tf.LPU_1 and H_DATE BETWEEN tf.DATE_IN AND tf.DATE_OUT)) < 4
											THEN ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00)* 0.80, 2)
											ELSE ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00)* 1.00, 2)
									END)
								WHEN (sl.RSLT not in (202, 205, 206, 207, 208)) THEN 
								   (CASE 
								   WHEN (DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) + 1 - 
												(SELECT COUNT(*) FROM dbo.WORK_DAY wd
												 WHERE wd.LPU = tf.LPU and LPU=tf.LPU and wd.PODR_ID = tf.LPU_1 and H_DATE BETWEEN tf.DATE_IN AND tf.DATE_OUT)) < 4
											THEN ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00)* 0.80, 2)
											ELSE ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00)* 1.00, 2)
									END)
                            END)

							WHEN sl.USL_OK = 2 and (t2.[SSL] = 0 OR t2.[SSL] < -4)
								THEN ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00), 2)
                            END),2) AS NUMERIC (10 ,2)),
                        TARIF=
                            CAST((CASE 
                            WHEN sl.USL_OK = 1 THEN t2.TARIF
                            WHEN sl.USL_OK = 2 THEN t2.TARIF
                            END) AS NUMERIC (10 ,2))
from usl tf
left join usl vm on tf.SLID = vm.SLID and tf.PODR = vm.PODR and vm.CODE_USL Like 'VM%'
join SLUCH sl on tf.SLID = sl.ID
left join SLUCH_DS2 ds2 on sl.ID = ds2.slid
join pacient pa on sl.PID = pa.ID
join SprKSGDecode ksg on sl.USL_OK = ksg.STYPE and (tf.DATE_OUT >= ksg.DBEG and tf.DATE_OUT < ksg.DEND +1)
and (ksg.DS1 = tf.DS or ksg.DS1 = (left(tf.DS, 1)+'.') or ksg.DS1 is null)
and (ksg.DS2 = ds2.DS or ksg.DS2 is null)
and (ksg.USL = vm.VID_VME or ksg.USL is null)
and (ksg.VOZR =
case 
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) between 0 and 28 then 1
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) between 29 and 90 then 2
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) between 91 and 365 then 3
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) between 0 and 365.25*2 then 4
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) between 0 and 365.25*18 then 5
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) > 365.25*18 then 6
end
or ksg.VOZR =
case 
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) between 0 and 365.25*2 then 4
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) between 0 and 365.25*18 then 5
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) > 365.25*18 then 6
end
or ksg.VOZR =
case 
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) between 0 and 365.25*18 then 5
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) > 365.25*18 then 6
end
or ksg.VOZR is null)
and (ksg.POL = pa.W or ksg.POL is null)
and (ksg.DLIT = 
case when cast(tf.DATE_OUT as date) = cast(tf.DATE_IN as date) or datediff(day, cast(tf.DATE_IN as date), cast(tf.DATE_OUT as date)) <= 3 then 1 else 0 end
or ksg.DLIT is null)
and (ksg.DOP_KR = sl.KSG_DKK or ksg.DOP_KR is null)
join SprKsg t on ksg.NKSG = t.KSGNUM and ksg.STYPE = t.STYPE and ksg.DBEG >= t.DBEG
join CalcKsgTarif t2 on t.ID = t2.IDKSG  and (tf.DATE_OUT >= t2.DBEG and tf.DATE_OUT < t2.DEND +1)
left join KSG_SOD as ksgsod on (tf.DATE_OUT >= ksgsod.TarifDateStart and tf.DATE_OUT < ksgsod.TarifDateEnd +1)
left join CalcUprk upr on t.ID = upr.IDKSG and (tf.DATE_OUT >= upr.TBEG and tf.DATE_OUT < upr.TEND +1)
left join @CalcBaseKoefDS bkds on (tf.DATE_OUT >= bkds.TBEG and tf.DATE_OUT < bkds.TEND +1)
left join CalcMok as kf on kf.KOD_LPU = sl.LPU AND (tf.DATE_OUT >= kf.DATESTART and (kf.DATEEND is NULL OR tf.DATE_OUT < kf.DATEEND +1))
where tf.slid = {_zsl.ID}
  and tf.CODE_USL Like 'TF%' and sl.METOD_HMP is null and tf.DATE_OUT >= '20180301' and tf.DATE_OUT < '20190101'
UNION ALL
select sl.USL_OK, tf.SLID, tf.ID, vm.ID vm_id, ksg.NKSG, t.ID as IDKSG, ksg.USL, ksg.DS1, ksg.DS2, ksg.VOZR as VOZR, ksg.POL, ksg.DLIT,
	tf.idksg tf_idksg, tf.DS tf_ds, ds2.ds sl_ds2,  vm.VID_VME vm_vid_vme, tf.kday tf_kday, tf.sumv_usl tf_sumv_usl, tf.tarif tf_tarif, sl.sumv sl_sumv, tf.podr tf_podr,-- tf.DATE_IN tf_date_in, tf.DATE_OUT tf_date_out, 
						KDAY = 
							(CASE 
							  WHEN sl.USL_OK = 1 THEN (CASE WHEN tf.DATE_IN = tf.DATE_OUT THEN 1 ELSE DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) END)
							  WHEN sl.USL_OK = 2 THEN DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) + 1 -
								(SELECT COUNT(*) FROM dbo.WORK_DAY wd WHERE wd.LPU = tf.LPU and wd.PODR_ID = tf.LPU_1 and H_DATE BETWEEN tf.DATE_IN AND tf.DATE_OUT)
                            END),
						SUMV_USL =
                            CAST(ROUND((CASE 
                            WHEN sl.USL_OK = 1 and t2.[SSL] > 0 THEN 
                            (CASE 
								WHEN (sl.RSLT in (102, 105, 106, 107, 108, 110)) THEN 
                            		(CASE
									    WHEN tf.IDKSG in ('1634', '1655', '1656') and DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) < 4
											THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.80, 2)
									    WHEN tf.IDKSG in ('1634', '1655', '1656') and DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) >= 4
											THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 1.00, 2)

                            			WHEN DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) < 4
											THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.20, 2)
										ELSE ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.50, 2)
									END)
								WHEN (sl.RSLT not in (102, 105, 106, 107, 108, 110)) THEN 
                            		(CASE 
									    WHEN tf.IDKSG in ('1634', '1655', '1656') and DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) < 4
											THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.80, 2)
									    WHEN tf.IDKSG in ('1634', '1655', '1656') and DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) >= 4
											THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 1.00, 2)

                            			WHEN DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) < 4
											THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.20, 2)
										ELSE ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 1.00, 2)
									END)
							END)
 
                            WHEN sl.USL_OK = 1 and (t2.[SSL] = -4) THEN 
                            (CASE 
                            WHEN (sl.RSLT in (102, 105, 106, 107, 108, 110)) THEN 
                            	(CASE 
                            		WHEN DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) < 4
										THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.80, 2)
									ELSE ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) ,2) 
								END)
							WHEN (sl.RSLT not in (102, 105, 106, 107, 108, 110)) THEN 
                            	(CASE 
                            		WHEN DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) < 4
										THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.80, 2)
									ELSE ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) ,2) 
								END)
							END)

							WHEN sl.USL_OK = 1 and (t2.[SSL] = 0 OR t2.[SSL] < -4)
							THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) ,2)

                            WHEN sl.USL_OK = 2 and t2.[SSL] > 0 THEN
                            (CASE 
								WHEN (sl.RSLT in (202, 205, 206, 207, 208)) THEN 
								   (CASE 
								   WHEN (DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) + 1 - 
												(SELECT COUNT(*) FROM dbo.WORK_DAY wd
												 WHERE wd.LPU = tf.LPU and LPU=tf.LPU and wd.PODR_ID = tf.LPU_1 and H_DATE BETWEEN tf.DATE_IN AND tf.DATE_OUT)) < 4
											THEN ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00)* 0.20, 2)
											ELSE ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00)* 0.50, 2)
									END)
								WHEN (sl.RSLT not in (202, 205, 206, 207, 208)) THEN 
								   (CASE 
								   WHEN (DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) + 1 - 
												(SELECT COUNT(*) FROM dbo.WORK_DAY wd
												 WHERE wd.LPU = tf.LPU and LPU=tf.LPU and wd.PODR_ID = tf.LPU_1 and H_DATE BETWEEN tf.DATE_IN AND tf.DATE_OUT)) < 4
											THEN ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00)* 0.20, 2)
											ELSE ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00)* 1.00, 2)
									END)
                            END)                        
                           
                            WHEN sl.USL_OK = 2 and t2.[SSL] = -4 THEN
                            (CASE 
								WHEN (sl.RSLT in (202, 205, 206, 207, 208)) THEN 
								   (CASE 
								   WHEN (DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) + 1 - 
												(SELECT COUNT(*) FROM dbo.WORK_DAY wd
												 WHERE wd.LPU = tf.LPU and LPU=tf.LPU and wd.PODR_ID = tf.LPU_1 and H_DATE BETWEEN tf.DATE_IN AND tf.DATE_OUT)) < 4
											THEN ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00)* 0.80, 2)
											ELSE ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00)* 1.00, 2)
									END)
								WHEN (sl.RSLT not in (202, 205, 206, 207, 208)) THEN 
								   (CASE 
								   WHEN (DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) + 1 - 
												(SELECT COUNT(*) FROM dbo.WORK_DAY wd
												 WHERE wd.LPU = tf.LPU and LPU=tf.LPU and wd.PODR_ID = tf.LPU_1 and H_DATE BETWEEN tf.DATE_IN AND tf.DATE_OUT)) < 4
											THEN ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00)* 0.80, 2)
											ELSE ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00)* 1.00, 2)
									END)
                            END)

							WHEN sl.USL_OK = 2 and (t2.[SSL] = 0 OR t2.[SSL] < -4)
								THEN ROUND(t2.TARIF * ISNULL(tf.DIFF_K,1.00) * ISNULL(upr.UPR, 1.00), 2)
                            END),2) AS NUMERIC (10 ,2)),
                        TARIF=
                            CAST((CASE 
                            WHEN sl.USL_OK = 1 THEN t2.TARIF
                            WHEN sl.USL_OK = 2 THEN t2.TARIF
                            END) AS NUMERIC (10 ,2))
from usl tf
left join usl vm on tf.SLID = vm.SLID and tf.PODR = vm.PODR and vm.CODE_USL Like 'VM%'
join SLUCH sl on tf.SLID = sl.ID
left join SLUCH_DS2 ds2 on sl.ID = ds2.slid
join pacient pa on sl.PID = pa.ID
join SprKSGDecode ksg on sl.USL_OK = ksg.STYPE and (tf.DATE_OUT >= ksg.DBEG and tf.DATE_OUT < ksg.DEND +1)
and (ksg.DS1 = tf.DS or ksg.DS1 = (left(tf.DS, 1)+'.') or ksg.DS1 is null)
and (ksg.DS2 = ds2.DS or ksg.DS2 is null)
and (ksg.USL = vm.VID_VME or ksg.USL is null)
and (ksg.VOZR =
case 
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) between 0 and 28 then 1
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) between 29 and 90 then 2
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) between 91 and 365 then 3
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) between 0 and 365.25*2 then 4
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) between 0 and 365.25*18 then 5
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) > 365.25*18 then 6
end
or ksg.VOZR =
case 
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) between 0 and 365.25*2 then 4
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) between 0 and 365.25*18 then 5
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) > 365.25*18 then 6
end
or ksg.VOZR =
case 
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) between 0 and 365.25*18 then 5
	when DATEDIFF(DAY, pa.DR, tf.DATE_IN) > 365.25*18 then 6
end
or ksg.VOZR is null)
and (ksg.POL = pa.W or ksg.POL is null)
and (ksg.DLIT = 
case when cast(tf.DATE_OUT as date) = cast(tf.DATE_IN as date) or datediff(day, cast(tf.DATE_IN as date), cast(tf.DATE_OUT as date)) <= 3 then 1 else 0 end
or ksg.DLIT is null)
and (ksg.DOP_KR = sl.KSG_DKK or ksg.DOP_KR is null)
join SprKsg t on ksg.NKSG = t.ID and ksg.STYPE = t.STYPE and ksg.DBEG >= t.DBEG
join CalcKsgTarif t2 on t.ID = t2.IDKSG  and (tf.DATE_OUT >= t2.DBEG and tf.DATE_OUT < t2.DEND +1)
left join KSG_SOD as ksgsod on (tf.DATE_OUT >= ksgsod.TarifDateStart and tf.DATE_OUT < ksgsod.TarifDateEnd +1)
left join CalcUprk upr on t.ID = upr.IDKSG and (tf.DATE_OUT >= upr.TBEG and tf.DATE_OUT < upr.TEND +1)
left join @CalcBaseKoefDS bkds on (tf.DATE_OUT >= bkds.TBEG and tf.DATE_OUT < bkds.TEND +1)
left join CalcMok as kf on kf.KOD_LPU = sl.LPU AND (tf.DATE_OUT >= kf.DATESTART and (kf.DATEEND is NULL OR tf.DATE_OUT < kf.DATEEND +1))
where tf.slid = {_zsl.ID}
  and tf.CODE_USL Like 'TF%' and sl.METOD_HMP is null and tf.DATE_OUT >= '20190101'

Insert into @temp_ksg_itog
			Select 0, * From(
			Select ROW_NUMBER () OVER (partition by SLID, ID, vm_ID order by (CASE WHEN USL IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN DS1 IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN DS2 IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN VOZR IS NULL THEN 10 ELSE VOZR END ), (CASE WHEN POL IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN DLIT IS NULL THEN 0 ELSE 1 END ) desc, SUMV_USL desc) rn, *
			from @temp_ksg ) tt1 where rn = 1
			UNION
			Select 0, * From(
			Select ROW_NUMBER () OVER (partition by SLID, ID, vm_ID order by (CASE WHEN USL IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN DS2 IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN DS1 IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN VOZR IS NULL THEN 10 ELSE VOZR END ), (CASE WHEN POL IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN DLIT IS NULL THEN 0 ELSE 1 END ) desc, SUMV_USL desc) rn, *
			from @temp_ksg ) tt2 where rn = 1
			UNION
			Select 0, * From(
			Select ROW_NUMBER () OVER (partition by SLID, ID order by (CASE WHEN DS1 IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN DS2 IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN USL IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN VOZR IS NULL THEN 10 ELSE VOZR END ), (CASE WHEN POL IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN DLIT IS NULL THEN 0 ELSE 1 END ) desc, SUMV_USL desc) rn, *
			from @temp_ksg ) tt3 where rn = 1 
			UNION
			Select 0, * From(
			Select ROW_NUMBER () OVER (partition by SLID, ID order by (CASE WHEN DS1 IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN USL IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN DS2 IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN VOZR IS NULL THEN 10 ELSE VOZR END ), (CASE WHEN POL IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN DLIT IS NULL THEN 0 ELSE 1 END ) desc, SUMV_USL desc) rn, *
			from @temp_ksg ) tt4 where rn = 1 
			UNION
			Select 0, * From(
			Select ROW_NUMBER () OVER (partition by SLID, ID order by (CASE WHEN DS2 IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN DS1 IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN USL IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN VOZR IS NULL THEN 10 ELSE VOZR END ), (CASE WHEN POL IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN DLIT IS NULL THEN 0 ELSE 1 END ) desc, SUMV_USL desc) rn, *
			from @temp_ksg ) tt5 where rn = 1 
			UNION
			Select 0, * From(
			Select ROW_NUMBER () OVER (partition by SLID, ID order by (CASE WHEN DS2 IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN USL IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN DS1 IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN VOZR IS NULL THEN 10 ELSE VOZR END ), (CASE WHEN POL IS NULL THEN 0 ELSE 1 END ) desc, (CASE WHEN DLIT IS NULL THEN 0 ELSE 1 END ) desc, SUMV_USL desc) rn, *
			from @temp_ksg ) tt6 where rn = 1

DECLARE sur_cursor CURSOR FOR   
			select slid from @temp_ksg_itog
			group by SLID having count(*) > 1
declare @slid int  

OPEN sur_cursor  
  
FETCH NEXT FROM sur_cursor   
INTO @slid 
  
WHILE @@FETCH_STATUS = 0  
BEGIN  


 IF (select count(*) from @temp_ksg_itog where slid =@slid and 'st02.010' in (select IDKSG from @temp_ksg_itog where slid =@slid) and 'st02.008' in (select IDKSG from @temp_ksg_itog where slid =@slid)) > 0
	UPDATE @temp_ksg_itog SET sur_ksg = 1 where slid =@slid and IDKSG = 'st02.010'
 IF (select count(*) from @temp_ksg_itog where slid =@slid and 'st02.011' in (select IDKSG from @temp_ksg_itog where slid =@slid) and 'st02.008' in (select IDKSG from @temp_ksg_itog where slid =@slid)) > 0
	UPDATE @temp_ksg_itog SET sur_ksg = 1 where slid =@slid and IDKSG = 'st02.011'
 IF (select count(*) from @temp_ksg_itog where slid =@slid and 'st02.010' in (select IDKSG from @temp_ksg_itog where slid =@slid) and 'st02.009' in (select IDKSG from @temp_ksg_itog where slid =@slid)) > 0
	UPDATE @temp_ksg_itog SET sur_ksg = 1 where slid =@slid and IDKSG = 'st02.010'
 IF (select count(*) from @temp_ksg_itog where slid =@slid and 'st14.001' in (select IDKSG from @temp_ksg_itog where slid =@slid) and 'st04.002' in (select IDKSG from @temp_ksg_itog where slid =@slid)) > 0
	UPDATE @temp_ksg_itog SET sur_ksg = 1 where slid =@slid and IDKSG = 'st14.001'
 IF (select count(*) from @temp_ksg_itog where slid =@slid and 'st14.002' in (select IDKSG from @temp_ksg_itog where slid =@slid) and 'st04.002' in (select IDKSG from @temp_ksg_itog where slid =@slid)) > 0
	UPDATE @temp_ksg_itog SET sur_ksg = 1 where slid =@slid and IDKSG = 'st14.002'
 IF (select count(*) from @temp_ksg_itog where slid =@slid and 'st21.001' in (select IDKSG from @temp_ksg_itog where slid =@slid) and 'st21.007' in (select IDKSG from @temp_ksg_itog where slid =@slid)) > 0
	UPDATE @temp_ksg_itog SET sur_ksg = 1 where slid =@slid and IDKSG = 'st21.001'
 IF (select count(*) from @temp_ksg_itog where slid =@slid and 'st34.002' in (select IDKSG from @temp_ksg_itog where slid =@slid) and 'st34.001' in (select IDKSG from @temp_ksg_itog where slid =@slid)) > 0
	UPDATE @temp_ksg_itog SET sur_ksg = 1 where slid =@slid and IDKSG = 'st34.002'
 IF (select count(*) from @temp_ksg_itog where slid =@slid and 'st34.002' in (select IDKSG from @temp_ksg_itog where slid =@slid) and 'st26.001' in (select IDKSG from @temp_ksg_itog where slid =@slid)) > 0
	UPDATE @temp_ksg_itog SET sur_ksg = 1 where slid =@slid and IDKSG = 'st34.002'
 IF (select count(*) from @temp_ksg_itog where slid =@slid and 'st30.006' in (select IDKSG from @temp_ksg_itog where slid =@slid) and 'st30.003' in (select IDKSG from @temp_ksg_itog where slid =@slid)) > 0
	UPDATE @temp_ksg_itog SET sur_ksg = 1 where slid =@slid and IDKSG = 'st30.006'
 IF (select count(*) from @temp_ksg_itog where slid =@slid and 'st09.001' in (select IDKSG from @temp_ksg_itog where slid =@slid) and 'st30.005' in (select IDKSG from @temp_ksg_itog where slid =@slid)) > 0
	UPDATE @temp_ksg_itog SET sur_ksg = 1 where slid =@slid and IDKSG = 'st09.001'
 IF (select count(*) from @temp_ksg_itog where slid =@slid and 'st31.002' in (select IDKSG from @temp_ksg_itog where slid =@slid) and 'st31.017' in (select IDKSG from @temp_ksg_itog where slid =@slid)) > 0
	UPDATE @temp_ksg_itog SET sur_ksg = 1 where slid =@slid and IDKSG = 'st31.002'

    FETCH NEXT FROM sur_cursor   
    INTO @slid 
END   
CLOSE sur_cursor;  
DEALLOCATE sur_cursor;  


-- выбор КСГ ДС по услуге
UPDATE @temp_ksg_itog SET sur_ksg = 1 where usl_ok = 2 and USL is not null

Select --*
 rn_itog, ID, IDKSG, KDAY, SUMV_USL SUMV, TARIF
 --SLID, sl_sumv, ISNULL(CONVERT(nvarchar(250), SUMV_USL), '') com
 From (
	Select ROW_NUMBER () OVER (partition by SLID, ID order by sur_ksg desc, SUMV_USL desc, (case when USL is NULL THEN 0 ELSE 1 end) desc)  rn_itog, * FROM @temp_ksg_itog
) itog where rn_itog =1
--rn_itog =1 and (( IDKSG <> tf_idksg or SUMV_USL <> tf_sumv_usl) and not (tf_idksg in (700,702,703,1126,1128,1129) and tf_sumv_usl = 0)) 
--slid = 6806463
" + Class1.ProfilerBlock(), SprClass.LocalConnectionString);

            foreach (var k in (IList)ksg)
            {
                var tf = _traffiList.SingleOrDefault(x => x.ID == (int)ObjHelper.GetAnonymousValue(k, "ID"));
                if (tf != null)
                {
                    tf.KDAY = (int?) ObjHelper.GetAnonymousValue(k, "KDAY");
                    tf.SUMV_USL = (decimal?)ObjHelper.GetAnonymousValue(k, "SUMV");
                    tf.TARIF = (decimal?)ObjHelper.GetAnonymousValue(k, "TARIF");
                    tf.IDKSG = (string) ObjHelper.GetAnonymousValue(k, "IDKSG");
                }
            }

            if (_traffiList.Count(x => x.IDKSG == "1565" || x.IDKSG == "1567" || x.IDKSG == "1568" || x.IDKSG == "st02.001" || x.IDKSG == "st02.003" || x.IDKSG == "st02.004") > 1)
            {
                string[] dss = new[] { "O14.1", "O34.2", "O36.3", "O36.4", "O42.2" };
                var tf = _traffiList.SingleOrDefault(x => x.IDKSG == "1565" || x.IDKSG == "st02.001");
                if (tf != null)
                {
                    var day = tf.DATE_OUT - tf.DATE_IN;
                    if (day?.Days > 1 && dss.Contains(tf.DS) || day?.Days > 5)
                    {

                    }
                    else
                    {
                        tf.IDKSG = null;
                        tf.SUMV_USL = 0;
                        tf.TARIF = 0;
                    }
                }
            }

            //if (_traffiList.Count(x => x.IDKSG == "st02.001" || x.IDKSG == "st02.003" || x.IDKSG == "st02.004") > 1)
            //{
            //    string[] dss = new[] { "O14.1", "O34.2", "O36.3", "O36.4", "O42.2" };
            //    var tf = _traffiList.SingleOrDefault(x => x.IDKSG == "st02.001");
            //    if (tf != null)
            //    {
            //        var day = tf.DATE_OUT - tf.DATE_IN;
            //        if (day?.Days > 1 && dss.Contains(tf.DS) || day?.Days > 5)
            //        {

            //        }
            //        else
            //        {
            //            tf.IDKSG = null;
            //            tf.SUMV_USL = 0;
            //            tf.TARIF = 0;
            //        }
            //    }
            //}

            SaveSluch();
        }

        private void TrafficGridControl_OnSelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            var id = ((USL) TrafficGridControl.SelectedItem).ID;
            _kslpList = Reader2List.CustomSelect<USL_KSLP>($"SELECT * FROM USL_KSLP WHERE SLID = {id}", SprClass.LocalConnectionString);
            Ds3GridControl.DataContext = _kslpList;
        }

        private void GridViewBase_OnCellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (Equals(e.Column, Ds3Column))
            {
                var val = ((IEnumerable<dynamic>) SprClass.KslpList).Single(
                    x => (decimal) ObjHelper.GetAnonymousValue(x, "IDSL") == (decimal) e.Value);
                Ds3GridControl.SetCellValue(e.RowHandle, "Z_SL",
    ObjHelper.GetAnonymousValue(val, "ZKOEF"));

            }

        }

        private void GridViewBase_OnCellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            (sender as TableView).PostEditor();
        }
    }
}
