using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Yamed.Control;
using Yamed.Core;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.OmsExp.SqlEditor
{
    public class MekGuid
    {
        public int ID { get; set; }
        public decimal? SUMV { get; set; }
        public string Com { get; set; }
    }

    public class PolCheckLPU
    {
        public string Fam { get; set; }
        public string Im { get; set; }
        public string Ot { get; set; }
        public DateTime? Dr { get; set; }
        public string Polis { get; set; }
        public DateTime? Date1 { get; set; }
        public DateTime? Date2 { get; set; }
        public string Q { get; set; }
        public int Id { get; set; }
        public decimal Sumv { get; set; }
        public string NHistory { get; set; }
        public int UslOk { get; set; }
    }


    /// <summary>
    /// Логика взаимодействия для AutoMekControl.xaml
    /// </summary>
    public partial class AutoMekControl : UserControl
    {
        private bool _isLpu;
        private object[] _schets;

        public AutoMekControl(object[] schets)
        {
            InitializeComponent();
            _schets = schets;


            //var localWeb =
            //    (IList) Reader2List.CustomAnonymousSelect("Select * from Settings where Name = 'MedicalOrganization'",
            //        SprClass.LocalConnectionString);

            //if (localWeb.Count > 0 && ((string) ObjHelper.GetAnonymousValue(localWeb[0], "Parametr"))?.Length == 6)
            //{
            //    _isLpu = true;
            //    List<MekDb> webList = null;
            //    TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext(); //get UI thread context 
            //    var webTask = Task.Factory.StartNew(() =>
            //    {
            //        try
            //        {
            //            webList = Reader2List.CustomSelect<MekDb>(
            //                $@" SELECT * FROM MEK_ONLINE", SprClass.GlobalElmedOnLineConnectionString);
            //            var profilerBlock = "";//ProfilerBlock();

            //            foreach (var wl in webList)
            //            {
            //                wl.AlgSql = profilerBlock + wl.AlgSql;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            Trace.WriteLine(ex.Message);
            //        }
            //    });
            //    webTask.ContinueWith(x =>
            //    {
            //        AutoMekElement1.MekList.DataContext = webList;
            //    }, uiScheduler);
            //}
            //else
                try
                {
                    //using (var dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
                    {
                        AutoMekElement1.MekList.DataContext = SqlReader.Select("Select * From Yamed_ExpSpr_SqlAlg where ExpType = 1 ", SprClass.LocalConnectionString);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }


            //AutoMekElement1.MekList.SelectedItems.Add(((IList)AutoMekElement1.MekList.DataContext)[0]);
            //AutoMekElement1.MekList.SelectedItems.Add(((IList)AutoMekElement1.MekList.DataContext)[3]);


            AutoMekElement2.TextBlock1.Text = "Реэкспертиза МЭК (полисы):";
            AutoMekElement2.TextBlock2.Text = "Реэкспертиза МЭК:";
            AutoMekElement2.TextBlock3.Text = "Информация о результате реэкспртизы МЭК:";
            AutoMekElement2.PolisCheckListBoxEdit.Items.RemoveAt(3);
            AutoMekElement2.PolisCheckListBoxEdit.Items.RemoveAt(2);
            AutoMekElement2.PolisCheckListBoxEdit.Items.RemoveAt(1);

            using (var dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
            {
                try
                {
                    var rdata = dc.GetTable<RMekDb>().ToList();
                    AutoMekElement2.MekList.DataContext = rdata;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }

        }




        private void AutoMekStart()
        {
            //string srz = Properties.Settings.Default.SRZ;
            var rMek = TabItem2.IsSelected ? true : false;

            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext(); //get UI thread context 
            List<object> mekList;
            mekList = rMek ? AutoMekElement2.MekList.SelectedItems.Select(x => x).ToList() : AutoMekElement1.MekList.SelectedItems.Select(x => x).ToList();

            AutoMekStartButton.IsEnabled = false;
            AutoMekElement1.MekList.IsEnabled = false;
            AutoMekElement1.PolisCheckListBoxEdit.IsEnabled = false;
            AutoMekElement2.MekList.IsEnabled = false;
            AutoMekElement2.PolisCheckListBoxEdit.IsEnabled = false;

            var polisCheck = AutoMekElement1.PolisCheckListBoxEdit.SelectedIndex;
            var attachedCheck = AutoMekElement1.AttachedCheckListBoxEdit.SelectedIndex;

            var mekTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (var sc in _schets)
                    {
                        var rid = ObjHelper.ClassConverter<D3_SCHET_OMS>(sc);

                        if (!rMek)
                        {
                            Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                AutoMekElement1.LogBox.Text += "Проверка " + "#" +
                                                           SprClass.LpuList.SingleOrDefault(
                                                               x => x.mcod == rid.CODE_MO)?.NameWithID +
                                                           "# запущена" + Environment.NewLine + Environment.NewLine;
                            });
                        }
                        else
                        {
                            Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                AutoMekElement2.LogBox.Text += "Проверка " + "#" +
                                                           SprClass.LpuList.SingleOrDefault(
                                                               x => x.mcod == rid.CODE_MO)?.NameWithID +
                                                           "# запущена" + Environment.NewLine + Environment.NewLine;
                            });

                        }


                        switch (polisCheck)
                        {
                            case 0:
                                Dispatcher.BeginInvoke((Action)delegate ()
                                {
                                    AutoMekElement1.LogBox.Text += "Проверка полисов пропущена" + Environment.NewLine;
                                });
                                break;
                            case 1:
                                PolisCheckForTer(rid.ID);
                                Dispatcher.BeginInvoke((Action)delegate ()
                                {
                                    AutoMekElement1.LogBox.Text += "Проверка полисов запущена" + Environment.NewLine;
                                });

                                break;
                            case 2:
                                PolisCheckForTerIgs(rid.ID);
                                Dispatcher.BeginInvoke((Action)delegate ()
                                {
                                    AutoMekElement1.LogBox.Text += "Проверка полисов запущена" + Environment.NewLine;
                                });
                                break;
                            case 3:
                                PolisCheckForTerTfoms(rid.ID);
                                Dispatcher.BeginInvoke((Action)delegate ()
                                {
                                    AutoMekElement1.LogBox.Text += "Проверка полисов запущена" + Environment.NewLine;
                                });
                                break;
                        }

                        if (attachedCheck == 1)
                        {
                            AttachedCheckForTer(rid.ID);
                            Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                AutoMekElement1.LogBox.Text += "Проверка прикрепления запущена" + Environment.NewLine;
                            });
                        }

                        if (!rMek)
                        {
                            foreach (DynamicBaseClass mek in mekList)
                            {
                                List<MekGuid> listGuid = new List<MekGuid>();

                                Dispatcher.BeginInvoke((Action)delegate ()
                                {
                                    AutoMekElement1.LogBox.Text += "Запущена экспертиза - " + (string)mek.GetValue("AlgName") +
                                                               Environment.NewLine;
                                });
                                SqlConnection con =
                                    new SqlConnection(SprClass.LocalConnectionString);
                                SqlCommand cmd = new SqlCommand(((string)mek.GetValue("AlgSql")).Replace("@p1", rid.ID.ToString()), con);
                                //cmd.CommandType = CommandType.StoredProcedure;
                                con.Open();

                                cmd.CommandTimeout = 0;
                                cmd.Prepare();
                                IDataReader dr = cmd.ExecuteReader();
                                while (dr.Read())
                                {
                                    MekGuid mekGuid = new MekGuid();
                                    mekGuid.ID = dr.GetInt32(0);
                                    object sumv = dr.GetValue(1);
                                    mekGuid.SUMV = sumv as decimal? ?? 0;
                                    if (dr.FieldCount == 3)
                                        mekGuid.Com = (string)dr.GetValue(2);
                                    listGuid.Add(mekGuid);
                                }
                                dr.Close();
                                con.Close();

                                //listGuid = dc.ExecuteQuery<MekGuid>(mek.AlgSql.Replace("@p1", schet_id.ToString())
                                //    .Replace("@srz1", srz)).ToList();
                                int count = listGuid.Count;
                                Dispatcher.BeginInvoke((Action)delegate ()
                                {
                                    AutoMekElement1.LogBox.Text += "Найдено - " + count + Environment.NewLine;
                                    if (count > 0 && (string)mek.GetValue("AlgSqlParametr") != "nosank") AutoMekElement1.LogBox.Text += "Идет снятие" + Environment.NewLine;
                                });
                                if (count > 0 && (string)mek.GetValue("AlgSqlParametr") != "nosank")
                                {
                                    //sb = new StringBuilder();
                                    List<D3_SANK_OMS> sankList = new List<D3_SANK_OMS>();
                                    foreach (MekGuid mekGuid in listGuid)
                                    {
                                        D3_SANK_OMS sank = new D3_SANK_OMS();
                                        sank.S_CODE = Guid.NewGuid().ToString();
                                        sank.D3_ZSLID = mekGuid.ID;
                                        sank.S_SUM = mekGuid.SUMV;
                                        sank.S_OSN = (string)mek.GetValue("MekOsn");
                                        sank.S_TIP = 1;
                                        sank.S_TIP2 = 1;
                                        sank.S_IST = 1;
                                        sank.USER_ID = SprClass.userId;
                                        sank.S_DATE = SprClass.WorkDate;
                                        sank.S_COM = !string.IsNullOrEmpty(mekGuid.Com)
                                            ? (string)mek.GetValue("AlgComment") + Environment.NewLine + mekGuid.Com
                                            : (string)mek.GetValue("AlgComment");
                                        sank.S_ZAKL = !string.IsNullOrEmpty(mekGuid.Com)
                                            ? (string)mek.GetValue("AlgComment") + Environment.NewLine + mekGuid.Com
                                            : (string)mek.GetValue("AlgComment");
                                        sank.D3_SCID = rid.ID;
                                        sankList.Add(sank);
                                        //SluchUpdateContructor(mek.AlgComment, mekGuid.ID);
                                    }

                                    Reader2List.BulkInsert(sankList, 100, SprClass.LocalConnectionString);
                                    //if (sb.Length > 0) SluchUpdate(sb.ToString());
                                }
                                if (count > 0 && (string)mek.GetValue("AlgSqlParametr") == "nosank")
                                {
                                    sb = new StringBuilder();
                                    foreach (MekGuid mekGuid in listGuid)
                                    {
                                        SluchUpdateContructor((string)mek.GetValue("AlgComment"), mekGuid.ID);
                                    }
                                    if (sb.Length > 0) SluchUpdate(sb.ToString());
                                }
                                Dispatcher.BeginInvoke((Action)delegate ()
                                {
                                    AutoMekElement1.LogBox.Text += "Экспертиза завершена" + Environment.NewLine +
                                                               Environment.NewLine;
                                });
                                //}
                            }
                        }
                        //else
                        //{
                        //    foreach (RMekDb mek in mekList)
                        //    {
                        //        List<MekGuid> listGuid = new List<MekGuid>();

                        //        Dispatcher.BeginInvoke((Action)delegate ()
                        //        {
                        //            AutoMekElement2.LogBox.Text += "Запущена экспертиза - " + mek.AlgName + Environment.NewLine;
                        //        });
                        //        SqlConnection con = new SqlConnection(SprClass.LocalConnectionString);
                        //        SqlCommand cmd = new SqlCommand(mek.AlgSql.Replace("@p1", rid.ID.ToString()), con);
                        //        //cmd.CommandType = CommandType.StoredProcedure;
                        //        con.Open();

                        //        cmd.CommandTimeout = 0;
                        //        SqlDataReader dr = cmd.ExecuteReader();

                        //        while (dr.Read())
                        //        {
                        //            MekGuid mekGuid = new MekGuid();
                        //            mekGuid.ID = dr.GetInt32(0);
                        //            object sumv = dr.GetValue(1);
                        //            mekGuid.SUMV = sumv as decimal? ?? 0;
                        //            if (dr.FieldCount == 3)
                        //                mekGuid.Com = (string)dr.GetValue(2);
                        //            listGuid.Add(mekGuid);
                        //        }
                        //        dr.Close();
                        //        con.Close();

                        //        //listGuid = dc.ExecuteQuery<MekGuid>(mek.AlgSql.Replace("@p1", schet_id.ToString())
                        //        //    .Replace("@srz1", srz)).ToList();
                        //        int count = listGuid.Count;
                        //        Dispatcher.BeginInvoke((Action)delegate ()
                        //        {
                        //            AutoMekElement2.LogBox.Text += "Найдено - " + count + Environment.NewLine;
                        //            if (count > 0) AutoMekElement2.LogBox.Text += "Идет снятие" + Environment.NewLine;
                        //        });
                        //        if (count > 0)
                        //        {
                        //            //sb = new StringBuilder();
                        //            List<SANK_RE> sankList = new List<SANK_RE>();
                        //            foreach (MekGuid mekGuid in listGuid)
                        //            {
                        //                SANK_RE sank = new SANK_RE();
                        //                sank.SLID = mekGuid.ID;
                        //                sank.S_SUM = mekGuid.SUMV;
                        //                sank.S_OSN = mek.Osn;
                        //                sank.S_TIP = 1;
                        //                sank.S_IST = 1;
                        //                sank.S_DATE = SprClass.WorkDate;
                        //                sank.S_COM = !string.IsNullOrEmpty(mekGuid.Com)
                        //                    ? mek.AlgComment + Environment.NewLine + mekGuid.Com
                        //                    : mek.AlgComment;
                        //                sank.SCHET_ID = rid.ID;
                        //                sankList.Add(sank);
                        //                //SluchReMekUpdateContructor(mek.AlgComment, mekGuid.ID);
                        //            }

                        //            Reader2List.BulkInsert(sankList, 100, SprClass.LocalConnectionString);

                        //            //if (sb.Length > 0) SluchUpdate(sb.ToString());
                        //        }
                        //        Dispatcher.BeginInvoke((Action)delegate ()
                        //        {
                        //            AutoMekElement2.LogBox.Text += "Экспертиза завершена" + Environment.NewLine + Environment.NewLine;
                        //        });
                        //        //}
                        //    }

                        //}

                        if (!rMek && !_isLpu)
                        {
                            Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                AutoMekElement1.LogBox.Text += "Перерасчет сумм счета" + Environment.NewLine;
                            });
                            decimal? ss = 0;

                            Reader2List.CustomExecuteQuery($@"
EXEC p_oms_calc_sank {rid.ID}
EXEC p_oms_calc_schet {rid.ID}
", SprClass.LocalConnectionString);
                            //using (var dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
                            //{
                                //dc.CommandTimeout = 0;
                                //dc.Sank_Calc(rid.ID);
                                //dc.Sluch_Calc(rid.ID);
                                //dc.Mek_Calc(rid.ID);
                                //dc.Schet_Calc(rid.ID);
                                //var sch = dc.D3_SCHET_OMS.Single(x => x.ID == rid.ID);
                                //ss = sch.SANK_MEK;
                            //}



                            Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                AutoMekElement1.LogBox.Text += "Сумма санкций - " + (ss) + Environment.NewLine +
                                                           Environment.NewLine;
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        ErrorGlobalWindow.ShowError(ex.Message);
                    });
                }
            });
            mekTask.ContinueWith(x =>
            {
                AutoMekStartButton.IsEnabled = true;
                AutoMekElement1.MekList.IsEnabled = true;
                AutoMekElement1.PolisCheckListBoxEdit.IsEnabled = true;
                AutoMekElement2.MekList.IsEnabled = true;
                AutoMekElement2.PolisCheckListBoxEdit.IsEnabled = true;
                MessageBox.Show("Проверка завершена");
                //_elReestrWindow.linqInstantFeedbackDataSource.Refresh();
            }, uiScheduler);
        }

        private void PolisCheckForTerTfoms(int reestrId)
        {
            //var qqqw = SprClass.PolChList.Select(x => x);

            //TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext(); //get UI thread context 
            Dispatcher.BeginInvoke((Action)delegate ()
            {
                //LogBox.Clear();
                AutoMekElement1.LogBox.Text += "Загрузка реестра...";
            });
            var ids = new ConcurrentBag<int>();
            //var mekTask = Task.Factory.StartNew(() =>
            //{
            try
            {
                List<PolCheckLPU> lpList = new List<PolCheckLPU>();
                using (SqlConnection con1 = new SqlConnection(SprClass.LocalConnectionString))
                {
                    SqlCommand cmd1 = new SqlCommand(string.Format(@"
                                select pa.fam, pa.im, pa.ot, pa.dr, pa.npolis, pa.SMO, zsl.DATE_Z_1, zsl.DATE_Z_2, zsl.id, zsl.sumv from d3_zsl_oms zsl
                                join d3_pacient_oms pa on zsl.D3_PID = pa.ID
                                where zsl.D3_SCID = {0} and (pa.SMO is null or pa.SMO not like '46%')", reestrId), con1);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    con1.Open();

                    cmd1.CommandTimeout = 0;
                    SqlDataReader dr1 = cmd1.ExecuteReader();
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {

                    });

                    while (dr1.Read())
                    {
                        PolCheckLPU polLpu = new PolCheckLPU();
                        polLpu.Fam = !(dr1.GetValue(0) is DBNull) ? (String)dr1.GetValue(0) : "";
                        polLpu.Im = !(dr1.GetValue(1) is DBNull) ? (String)dr1.GetValue(1) : "";
                        polLpu.Ot = !(dr1.GetValue(2) is DBNull) ? (String)dr1.GetValue(2) : "";
                        polLpu.Dr = (dr1.GetValue(3) is DateTime) ? (DateTime)dr1.GetValue(3) : polLpu.Dr;
                        polLpu.Polis = !(dr1.GetValue(4) is DBNull) ? (String)dr1.GetValue(4) : "";
                        polLpu.Q = !(dr1.GetValue(5) is DBNull) ? (String)dr1.GetValue(5) : polLpu.Q;
                        polLpu.Date1 = (dr1.GetValue(6) is DateTime) ? (DateTime)dr1.GetValue(6) : polLpu.Date1;
                        polLpu.Date2 = (dr1.GetValue(7) is DateTime) ? (DateTime)dr1.GetValue(7) : polLpu.Date2;
                        polLpu.Id = (int)dr1.GetValue(8);
                        polLpu.Sumv = (dr1.GetValue(9) is Decimal) ? (Decimal)dr1.GetValue(9) : polLpu.Sumv;
                        lpList.Add(polLpu);

                        //Dispatcher.BeginInvoke((Action)delegate()
                        //{
                        //    LogBox.Text += "Загрузка SRZ - " + ++i + Environment.NewLine;
                        //});
                    }
                    dr1.Close();
                    con1.Close();
                }

                StringBuilder sb = new StringBuilder();
                foreach (PolCheckLPU lp in lpList)
                {
                    sb.Append("'").Append(lp.Polis).Append("'").Append(",");
                }
                sb.Remove(sb.Length - 1, 1);

                List<D3_SANK_OMS> mList = new List<D3_SANK_OMS>();
                bool isValid = false;
                string coment = "";
                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    //LogBox.Clear();
                    AutoMekElement1.LogBox.Text += "Проверка:" + Environment.NewLine;
                });

                int count = 100;
                int i = 0;
                int c = 0;
                int max = lpList.Count();
                int progressInt = max / count;
                int progressIntLock = progressInt;
                int[] progressInts = new int[count];
                for (int j = 0; j < count; j++)
                {
                    progressInts[j] = progressInt;
                    progressInt += progressIntLock;
                }
                progressInts[count - 1] = max;
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                       new Action(delegate ()
                                       {
                                           progressBar1.EditValue = 0;
                                           progressBar1.Visibility = Visibility;
                                       }));
                int ii = 0;




                foreach (var p in lpList) //, p =>
                {
                    if (i == progressInts[c])
                    {
                        if (c < count - 1) c += 1;
                        Dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(delegate ()
                            {
                                ii++;
                                progressBar1.EditValue = ii;
                            }));
                    }
                    i += 1;

                    List<PolisCheck> polisChecks = new List<PolisCheck>();
                    using (SqlConnection con1 = new SqlConnection(SprClass.SrzConnectionString))
                    {
                        using (SqlCommand cmd1 = new SqlCommand(string.Format(@"
                                    select fd.FAM, fd.IM, fd.OT, fd.DR, 
                                    (CASE
                                        WHEN p.OPDOC = 3 THEN p.ENP
                                        ELSE p.NPOL
                                    END) as POLIS, p.DBEG, p.DEND, p.DSTOP, p.Q, p.OPDOC, p.ENP
                                    from HISTFDR fd
        						    join PEOPLE p on fd.FAM = '{0}' and fd.IM = '{1}' and fd.OT = '{2}' and fd.DR = '{3:yyyyMMdd}' and p.ID = fd.PID",
                        p.Fam.Trim().ToUpper(), p.Im.Trim().ToUpper(), p.Ot.Trim().ToUpper(), p.Dr.Value), con1))
                        {
                            con1.Open();
                            cmd1.CommandTimeout = 0;
                            using (SqlDataReader dr1 = cmd1.ExecuteReader())
                            {
                                int ip = 0;
                                while (dr1.Read())
                                {
                                    PolisCheck polCh = new PolisCheck();
                                    polCh.Fam = !(dr1.GetValue(0) is DBNull) ? (String)dr1.GetValue(0) : "";
                                    polCh.Im = !(dr1.GetValue(1) is DBNull) ? (String)dr1.GetValue(1) : "";
                                    polCh.Ot = !(dr1.GetValue(2) is DBNull) ? (String)dr1.GetValue(2) : "";
                                    polCh.Dr = (dr1.GetValue(3) is DateTime) ? (DateTime)dr1.GetValue(3) : polCh.Dr;
                                    polCh.Polis = !(dr1.GetValue(4) is DBNull) ? (String)dr1.GetValue(4) : null;
                                    polCh.Dbeg = (dr1.GetValue(5) is DateTime) ? (DateTime)dr1.GetValue(5) : polCh.Dbeg;
                                    polCh.Dend = (dr1.GetValue(6) is DateTime) ? (DateTime)dr1.GetValue(6) : polCh.Dend;
                                    polCh.Dstop = (dr1.GetValue(7) is DateTime) ? (DateTime)dr1.GetValue(7) : polCh.Dstop;
                                    polCh.Q = !(dr1.GetValue(8) is DBNull) ? (String)dr1.GetValue(8) : null;
                                    polCh.Opdoc = !(dr1.GetValue(9) is DBNull) ? (int)dr1.GetValue(9) : polCh.Opdoc;
                                    polCh.ENP = !(dr1.GetValue(10) is DBNull) ? (String)dr1.GetValue(10) : null;
                                    polisChecks.Add(polCh);

                                    Dispatcher.BeginInvoke((Action)delegate ()
                                    {
                                        StatusText.Text = ip++.ToString();
                                    });
                                }
                                dr1.Close();
                            }
                            con1.Close();
                        }
                    }

                    //var q = polisChecks.Where(x => x.Polis == p.Polis).ToList();
                    if (polisChecks.Any())
                    {
                        var fq = polisChecks.First();
                        //if (fq.Fam == "АВЕРИНА")
                        //    fq.Fam = "Аверина";
                        if (p.Date2 >= fq.Dbeg && (fq.Dend == null || p.Date2 < fq.Dend.Value.AddDays(1)) &&
                            (fq.Dstop == null || p.Date2 < fq.Dstop.Value.AddDays(1)))
                        {
                            isValid = false;
                            coment = "Есть полис региональной СМО: " + fq.Q;
                        }
                        else
                        {
                            List<PolisCheck> polisChecks1 = new List<PolisCheck>();
                            using (SqlConnection con1 = new SqlConnection(SprClass.SrzConnectionString))
                            {
                                using (SqlCommand cmd1 = new SqlCommand(string.Format(@"
                                            Select fd.FAM, fd.IM, fd.OT, fd.DR, 
                                            (CASE
                                                WHEN p.OPDOC = 3 THEN p.ENP
                                                ELSE p.NPOL
                                            END) as POLIS, p.DBEG, p.DEND, p.DSTOP, p.Q, pol.NPOL, pol.DBEG, pol.DEND, pol.DSTOP, pol.Q, p.OPDOC, pol.POLTP, p.ENP
                                            From HISTFDR fd
                                            join PEOPLE p on fd.FAM = '{0}' and fd.IM = '{1}' and fd.OT = '{2}' and fd.DR = '{3:yyyyMMdd}' and p.ID = fd.PID
                                            JOIN POLIS pol on pol.PID = p.ID",
                                p.Fam.Trim().ToUpper(), p.Im.Trim().ToUpper(), p.Ot.Trim().ToUpper(), p.Dr.Value), con1))
                                {
                                    con1.Open();
                                    cmd1.CommandTimeout = 0;
                                    using (SqlDataReader dr1 = cmd1.ExecuteReader())
                                    {
                                        Dispatcher.BeginInvoke((Action)delegate ()
                                        {

                                        });
                                        while (dr1.Read())
                                        {
                                            PolisCheck polCh = new PolisCheck();
                                            polCh.Fam = !(dr1.GetValue(0) is DBNull) ? (String)dr1.GetValue(0) : "";
                                            polCh.Im = !(dr1.GetValue(1) is DBNull) ? (String)dr1.GetValue(1) : "";
                                            polCh.Ot = !(dr1.GetValue(2) is DBNull) ? (String)dr1.GetValue(2) : "";
                                            polCh.Dr = (dr1.GetValue(3) is DateTime)
                                                ? (DateTime)dr1.GetValue(3)
                                                : polCh.Dr;
                                            polCh.Polis = !(dr1.GetValue(4) is DBNull) ? (String)dr1.GetValue(4) : null;
                                            polCh.Dbeg = (dr1.GetValue(5) is DateTime)
                                                ? (DateTime)dr1.GetValue(5)
                                                : polCh.Dbeg;
                                            polCh.Dend = (dr1.GetValue(6) is DateTime)
                                                ? (DateTime)dr1.GetValue(6)
                                                : polCh.Dend;
                                            polCh.Dstop = (dr1.GetValue(7) is DateTime)
                                                ? (DateTime)dr1.GetValue(7)
                                                : polCh.Dstop;
                                            polCh.Q = !(dr1.GetValue(8) is DBNull) ? (String)dr1.GetValue(8) : null;
                                            polCh.H_Polis = !(dr1.GetValue(9) is DBNull)
                                                ? (String)dr1.GetValue(9)
                                                : null;
                                            polCh.H_Dbeg = (dr1.GetValue(10) is DateTime)
                                                ? (DateTime)dr1.GetValue(10)
                                                : polCh.H_Dbeg;
                                            polCh.H_Dend = (dr1.GetValue(11) is DateTime)
                                                ? (DateTime)dr1.GetValue(11)
                                                : polCh.H_Dend;
                                            polCh.H_Dstop = (dr1.GetValue(12) is DateTime)
                                                ? (DateTime)dr1.GetValue(12)
                                                : polCh.H_Dstop;
                                            polCh.H_Q = !(dr1.GetValue(13) is DBNull) ? (String)dr1.GetValue(13) : null;
                                            polCh.Opdoc = !(dr1.GetValue(14) is DBNull)
                                                ? (int)dr1.GetValue(14)
                                                : polCh.Opdoc;
                                            polCh.Opdoc2 = !(dr1.GetValue(15) is DBNull)
                                                ? (int)dr1.GetValue(15)
                                                : polCh.Opdoc2;
                                            polCh.ENP = !(dr1.GetValue(16) is DBNull) ? (String)dr1.GetValue(16) : null;

                                            polisChecks1.Add(polCh);
                                        }
                                        dr1.Close();
                                    }
                                    con1.Close();
                                }
                            }

                            if (polisChecks1.Any())
                            {
                                foreach (var po in polisChecks1)
                                {
                                    if (p.Date2 >= po.H_Dbeg &&
                                        (po.H_Dend == null || p.Date2 < po.H_Dend.Value.AddDays(1)) &&
                                        (po.H_Dstop == null || p.Date2 < po.H_Dstop.Value.AddDays(1)))
                                    {
                                        isValid = false;
                                        coment = "Есть полис региональной СМО: " + po.Q;
                                        break;
                                    }
                                    isValid = true;
                                }
                            }
                            else
                            {
                                isValid = true;
                            }
                        }
                    }
                    else
                    {
                        isValid = true;
                    }

                    if (!isValid)
                        mList.Add(new D3_SANK_OMS()
                        {
                            D3_ZSLID = p.Id,
                            S_CODE = Guid.NewGuid().ToString(),
                            S_SUM = p.Sumv,
                            S_OSN = "5.2.3.",
                            S_TIP = 1,
                            S_TIP2 = 1,
                            S_IST = 1,
                            USER_ID = SprClass.userId,
                            S_COM = coment,
                            S_ZAKL = coment,
                            S_DATE = DateTime.Today,
                            D3_SCID = reestrId
                        });
                }

                if (mList.Any())
                {
                    Reader2List.BulkInsert(mList, 100, SprClass.LocalConnectionString);
                }

            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    ErrorGlobalWindow.ShowError(ex.Message);
                });
            }
        }

        private void PolisCheckForTer(int reestrId)
        {
            sb2 = new StringBuilder();


            Dispatcher.BeginInvoke((Action)delegate ()
            {
                AutoMekElement1.LogBox.Text += "Загрузка реестра...";
            });
            try
            {
                List<PolCheckLPU> lpList = new List<PolCheckLPU>();
                using (SqlConnection con1 = new SqlConnection(SprClass.LocalConnectionString))
                {
                    SqlCommand cmd1 = new SqlCommand(string.Format(@"
                                select pa.fam, pa.im, pa.ot, pa.dr, pa.npolis, pa.SMO, zsl.DATE_Z_1, zsl.DATE_Z_2, zsl.id, zsl.sumv 
                                from d3_zsl_oms zsl
                                join d3_pacient_oms pa on zsl.D3_PID = pa.ID
                                where zsl.D3_SCID = {0}", reestrId), con1);
                    con1.Open();

                    cmd1.CommandTimeout = 0;
                    SqlDataReader dr1 = cmd1.ExecuteReader();

                    while (dr1.Read())
                    {
                        PolCheckLPU polLpu = new PolCheckLPU();
                        polLpu.Fam = !(dr1.GetValue(0) is DBNull) ? ((String)dr1.GetValue(0)).Trim().ToUpper() : "";
                        polLpu.Im = !(dr1.GetValue(1) is DBNull) ? ((String)dr1.GetValue(1)).Trim().ToUpper() : "";
                        polLpu.Ot = !(dr1.GetValue(2) is DBNull) ? ((String)dr1.GetValue(2)).Trim().ToUpper() : "";
                        polLpu.Dr = (dr1.GetValue(3) is DateTime) ? (DateTime)dr1.GetValue(3) : polLpu.Dr;
                        polLpu.Polis = !(dr1.GetValue(4) is DBNull) ? (String)dr1.GetValue(4) : "";
                        polLpu.Q = !(dr1.GetValue(5) is DBNull) ? (String)dr1.GetValue(5) : polLpu.Q;
                        polLpu.Date1 = (dr1.GetValue(6) is DateTime) ? (DateTime)dr1.GetValue(6) : polLpu.Date1;
                        polLpu.Date2 = (dr1.GetValue(7) is DateTime) ? (DateTime)dr1.GetValue(7) : polLpu.Date2;
                        polLpu.Id = (int)dr1.GetValue(8);
                        polLpu.Sumv = (dr1.GetValue(9) is Decimal) ? (Decimal)dr1.GetValue(9) : polLpu.Sumv;
                        lpList.Add(polLpu);

                    }
                    dr1.Close();
                    con1.Close();
                }

                List<D3_SANK_OMS> mList = new List<D3_SANK_OMS>();

                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    AutoMekElement1.LogBox.Text += "Проверка:" + Environment.NewLine;
                });

                int count = 100;
                int i = 0;
                int c = 0;
                int max = lpList.Count();
                int progressInt = max / count;
                int progressIntLock = progressInt;
                int[] progressInts = new int[count];
                for (int j = 0; j < count; j++)
                {
                    progressInts[j] = progressInt;
                    progressInt += progressIntLock;
                }
                progressInts[count - 1] = max;
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                       new Action(delegate ()
                                       {
                                           progressBar1.EditValue = 0;
                                           progressBar1.Visibility = Visibility;
                                       }));
                int ii = 0;




                foreach (var p in lpList)
                {
                    bool isValid = false;
                    bool isPolisCheck = false;
                    string coment = "";
                    string coment2 = "";
                    string coment3 = "";
                    string polisOsn = "";

                    if (i == progressInts[c])
                    {
                        if (c < count - 1) c += 1;
                        Dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(delegate ()
                            {
                                ii++;
                                progressBar1.EditValue = ii;
                            }));
                    }
                    i += 1;

                    List<PolisCheck> polisChecks = new List<PolisCheck>();
                    using (SqlConnection con1 = new SqlConnection(SprClass.SrzConnectionString))
                    {
                        var cmdStr = @"select top 1 p.FAM, p.IM, p.OT, p.DR, NPOL, p.DBEG, p.DEND, p.DSTOP, p.Q, p.OPDOC, p.ENP, p.ID
				       from PEOPLE p 
				       where ID in (SELECT PID from HISTFDR where";
                        if (p.Fam.Contains("Е") || p.Fam.Contains("Ё"))
                            cmdStr = cmdStr + string.Format(@" FAM LIKE '{0}'", EFix(p.Fam));
                        else
                            cmdStr = cmdStr + string.Format(@" FAM = '{0}'", p.Fam);
                        if (p.Im.Contains("Е") || p.Im.Contains("Ё"))
                            cmdStr = cmdStr + string.Format(@" and IM LIKE '{0}'", EFix(p.Im));
                        else
                            cmdStr = cmdStr + string.Format(@" and IM = '{0}'", p.Im);
                        if (p.Ot.Contains("Е") || p.Ot.Contains("Ё"))
                            cmdStr = cmdStr + string.Format(@" and OT LIKE '{0}'", EFix(p.Ot));
                        else
                            cmdStr = cmdStr + string.Format(@" and OT = '{0}'", p.Ot);
                        cmdStr = cmdStr + string.Format(@" and DR = '{0:yyyyMMdd}') order by dstop", p.Dr.Value);

                        using (SqlCommand cmd1 = new SqlCommand(cmdStr, con1))
                        {
                            con1.Open();
                            cmd1.CommandTimeout = 0;
                            using (SqlDataReader dr1 = cmd1.ExecuteReader())
                            {
                                int ip = 0;
                                while (dr1.Read())
                                {
                                    PolisCheck polCh = new PolisCheck();
                                    polCh.Fam = !(dr1.GetValue(0) is DBNull) ? (String)dr1.GetValue(0) : "";
                                    polCh.Im = !(dr1.GetValue(1) is DBNull) ? (String)dr1.GetValue(1) : "";
                                    polCh.Ot = !(dr1.GetValue(2) is DBNull) ? (String)dr1.GetValue(2) : "";
                                    polCh.Dr = (dr1.GetValue(3) is DateTime) ? (DateTime)dr1.GetValue(3) : polCh.Dr;
                                    polCh.Polis = !(dr1.GetValue(4) is DBNull) ? (String)dr1.GetValue(4) : null;
                                    polCh.Dbeg = (dr1.GetValue(5) is DateTime) ? (DateTime)dr1.GetValue(5) : polCh.Dbeg;
                                    polCh.Dend = (dr1.GetValue(6) is DateTime) ? (DateTime)dr1.GetValue(6) : polCh.Dend;
                                    polCh.Dstop = (dr1.GetValue(7) is DateTime) ? (DateTime)dr1.GetValue(7) : polCh.Dstop;
                                    polCh.Q = !(dr1.GetValue(8) is DBNull) ? (String)dr1.GetValue(8) : null;
                                    polCh.Opdoc = !(dr1.GetValue(9) is DBNull) ? (int)dr1.GetValue(9) : polCh.Opdoc;
                                    polCh.ENP = !(dr1.GetValue(10) is DBNull) ? (String)dr1.GetValue(10) : null;
                                    polCh.Id = (int)dr1.GetValue(11);
                                    polisChecks.Add(polCh);
                                }
                                dr1.Close();
                            }
                            con1.Close();
                        }
                    }

                    if (polisChecks.Any())
                    {
                        var fq = polisChecks.First();
                        if (
                            (
                                p.Date2 >= fq.Dbeg &&
                                (fq.Dend == null || p.Date2 < fq.Dend.Value.AddDays(1))
                                && (fq.Dstop == null || p.Date2 < fq.Dstop.Value.AddDays(1))
                            )
                            ||
                           (
                                p.Date2 >= fq.Dbeg &&
                                (fq.Dend == null || p.Date2 < fq.Dend.Value.AddDays(1))
                                && fq.Dstop != null && fq.Ds == null
                           )
                           )
                        {
                            if (p.Q == fq.Q)
                            {
                                isValid = true;
                            }
                            else
                            {
                                isValid = false;
                                coment = "Страхование в " + ConvertSmo(fq.Q);
                                coment2 = "Страхование в другой СМО";
                                polisOsn = "5.2.1.";
                            }
                        }
                        else
                        {
                            List<PolisCheck> polisChecks1 = new List<PolisCheck>();
                            using (SqlConnection con1 = new SqlConnection(SprClass.SrzConnectionString))
                            {
                                using (SqlCommand cmd1 = new SqlCommand(string.Format(@"
                                            Select pol.NPOL, pol.DBEG, pol.DEND, pol.DSTOP, pol.Q, pol.POLTP
                                            From POLIS pol WHERE pol.PID = {0}", fq.Id), con1))
                                {
                                    con1.Open();
                                    cmd1.CommandTimeout = 0;
                                    using (SqlDataReader dr1 = cmd1.ExecuteReader())
                                    {
                                        //Dispatcher.BeginInvoke((Action)delegate()
                                        //{

                                        //});
                                        while (dr1.Read())
                                        {
                                            PolisCheck polCh = new PolisCheck();
                                            polCh.H_Polis = !(dr1.GetValue(0) is DBNull) ? (String)dr1.GetValue(0) : null;
                                            polCh.H_Dbeg = (dr1.GetValue(1) is DateTime) ? (DateTime)dr1.GetValue(1) : polCh.H_Dbeg;
                                            polCh.H_Dend = (dr1.GetValue(2) is DateTime) ? (DateTime)dr1.GetValue(2) : polCh.H_Dend;
                                            polCh.H_Dstop = (dr1.GetValue(3) is DateTime) ? (DateTime)dr1.GetValue(3) : polCh.H_Dstop;
                                            polCh.H_Q = !(dr1.GetValue(4) is DBNull) ? (String)dr1.GetValue(4) : null;
                                            polCh.Opdoc2 = !(dr1.GetValue(5) is DBNull) ? (int)dr1.GetValue(5) : polCh.Opdoc2;

                                            polisChecks1.Add(polCh);
                                        }
                                        dr1.Close();
                                    }
                                    con1.Close();
                                }
                            }

                            if (polisChecks1.Any())
                            {
                                foreach (var po in polisChecks1)
                                {
                                    isValid = false;
                                    coment = "Нет действующего страхования.";
                                    coment2 = "Нет действующего страхования.";
                                    polisOsn = "5.2.4.";

                                    if (p.Date2 >= po.H_Dbeg &&
                                        (po.H_Dend == null || p.Date2 < po.H_Dend.Value.AddDays(1)) &&
                                        (po.H_Dstop == null || p.Date2 < po.H_Dstop.Value.AddDays(1)))
                                    {
                                        if (p.Q == po.H_Q)
                                        {
                                            isValid = true;
                                            break;
                                        }
                                        isValid = false;
                                        coment = "Страхование в " + ConvertSmo(po.H_Q);
                                        coment2 = "Страхование в другой СМО";
                                        polisOsn = "5.2.1.";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                isValid = false;
                                coment = "Нет действующего страхования.";
                                coment2 = "Нет действующего страхования.";
                                polisOsn = "5.2.4.";
                            }
                        }
                    }
                    else
                    {
                        isValid = false;
                        coment = "Не удалось идентифицировать по ФИО ДР.";
                        coment2 = "Не удалось идентифицировать по ФИО ДР.";
                        polisOsn = "5.2.2.";

                        List<PolisCheck> polisChecks2 = new List<PolisCheck>();
                        using (SqlConnection con1 = new SqlConnection(SprClass.SrzConnectionString))
                        {
                            var cmdStr = string.Format(@"select p.FAM, p.IM, p.OT, p.DR, NPOL, p.DBEG, p.DEND, p.DSTOP, p.Q, p.OPDOC, p.ENP, p.ID
                                            from PEOPLE p where NPOL = '{0}' OR ENP = '{0}'", p.Polis);

                            using (SqlCommand cmd1 = new SqlCommand(cmdStr, con1))
                            {
                                con1.Open();
                                cmd1.CommandTimeout = 0;
                                using (SqlDataReader dr1 = cmd1.ExecuteReader())
                                {
                                    while (dr1.Read())
                                    {
                                        PolisCheck polCh = new PolisCheck();
                                        polCh.Fam = !(dr1.GetValue(0) is DBNull) ? (String)dr1.GetValue(0) : "";
                                        polCh.Im = !(dr1.GetValue(1) is DBNull) ? (String)dr1.GetValue(1) : "";
                                        polCh.Ot = !(dr1.GetValue(2) is DBNull) ? (String)dr1.GetValue(2) : "";
                                        polCh.Dr = (dr1.GetValue(3) is DateTime) ? (DateTime)dr1.GetValue(3) : polCh.Dr;
                                        polCh.Polis = !(dr1.GetValue(4) is DBNull) ? (String)dr1.GetValue(4) : null;
                                        polCh.Dbeg = (dr1.GetValue(5) is DateTime) ? (DateTime)dr1.GetValue(5) : polCh.Dbeg;
                                        polCh.Dend = (dr1.GetValue(6) is DateTime) ? (DateTime)dr1.GetValue(6) : polCh.Dend;
                                        polCh.Dstop = (dr1.GetValue(7) is DateTime) ? (DateTime)dr1.GetValue(7) : polCh.Dstop;
                                        polCh.Q = !(dr1.GetValue(8) is DBNull) ? (String)dr1.GetValue(8) : null;
                                        polCh.Opdoc = !(dr1.GetValue(9) is DBNull) ? (int)dr1.GetValue(9) : polCh.Opdoc;
                                        polCh.ENP = !(dr1.GetValue(10) is DBNull) ? (String)dr1.GetValue(10) : null;
                                        polCh.Id = (int)dr1.GetValue(11);
                                        polisChecks2.Add(polCh);
                                    }
                                    dr1.Close();
                                }
                                con1.Close();
                            }
                        }
                        if (polisChecks2.Any())
                        {
                            var fq2 = polisChecks2.First();
                            if (p.Date2 >= fq2.Dbeg && (fq2.Dend == null || p.Date2 < fq2.Dend.Value.AddDays(1)) &&
                                (fq2.Dstop == null || p.Date2 < fq2.Dstop.Value.AddDays(1)))
                            {
                                if (p.Q == fq2.Q)
                                {
                                    isValid = false;
                                    isPolisCheck = true;
                                    coment3 = string.Format(@"Полис выдан: {0} {1} {2} {3}", fq2.Fam, fq2.Im, fq2.Ot, fq2.Dr.Value.ToShortDateString());
                                }
                                else
                                {
                                    isValid = false;
                                    isPolisCheck = true;
                                    coment3 = string.Format(@"Полис выдан: {0} {1} {2} {3}, СМО: {4}",
                                        fq2.Fam, fq2.Im, fq2.Ot, fq2.Dr.Value.ToShortDateString(), fq2.Q);
                                }
                            }
                            else
                            {
                                isValid = false;
                                isPolisCheck = true;
                                var dBeg = fq2.Dbeg != null ? fq2.Dbeg.Value.ToShortDateString() : "";
                                var dEnd = fq2.Dend != null ? fq2.Dend.Value.ToShortDateString() : "";
                                var dStop = fq2.Dstop != null ? fq2.Dstop.Value.ToShortDateString() : "";
                                coment3 = string.Format(@"Полис выдан: {0} {1} {2} {3}, Действует с: {4} по: {5}, Изъят: {6}",
                                    fq2.Fam, fq2.Im, fq2.Ot, fq2.Dr, dBeg, dEnd, dStop);
                            }
                        }
                    }

                    if (!isValid)
                    {
                        mList.Add(new D3_SANK_OMS()
                        {
                            D3_ZSLID = p.Id,
                            S_CODE = Guid.NewGuid().ToString(),
                            S_SUM = p.Sumv,
                            S_OSN = polisOsn,
                            S_TIP = 1,
                            S_TIP2 = 1,
                            S_IST = 1,
                            USER_ID = SprClass.userId,
                            S_COM = coment,
                            S_ZAKL = coment,
                            S_DATE = DateTime.Today,
                            D3_SCID = reestrId
                        });
                        //SluchUpdateContructor(coment2, p.Id);
                    }
                    if (isPolisCheck)
                    {
                        SluchUpdateContructor2(coment3, p.Id);
                    }
                }

                if (mList.Any())
                {
                    Reader2List.BulkInsert(mList, 100, SprClass.LocalConnectionString);
                }
                //if (sb.Length > 0) SluchUpdate(sb.ToString());
                if (sb2.Length > 0) SluchUpdate(sb2.ToString());
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    ErrorGlobalWindow.ShowError(ex.Message);
                });
            }
        }

        private void AttachedCheckForTer(int reestrId)
        {
            sb2 = new StringBuilder();


            Dispatcher.BeginInvoke((Action)delegate ()
            {
                AutoMekElement1.LogBox.Text += "Загрузка реестра...";
            });
            try
            {
                List<PolCheckLPU> lpList = new List<PolCheckLPU>();
                using (SqlConnection con1 = new SqlConnection(SprClass.LocalConnectionString))
                {
                    SqlCommand cmd1 = new SqlCommand(string.Format(@"
                                select pa.fam, pa.im, pa.ot, pa.dr, pa.npolis, zsl.LPU, zsl.DATE_Z_1, zsl.DATE_Z_2, zsl.id, zsl.sumv 
                                from d3_zsl_oms zsl
                                join d3_pacient_oms pa on zsl.D3_PID = pa.ID
                                where zsl.D3_SCID = {0} and zsl.OS_SLUCH_REGION in (1,9,22,23,47,48,49)", reestrId), con1);
                    con1.Open();

                    cmd1.CommandTimeout = 0;
                    SqlDataReader dr1 = cmd1.ExecuteReader();

                    while (dr1.Read())
                    {
                        PolCheckLPU polLpu = new PolCheckLPU();
                        polLpu.Fam = !(dr1.GetValue(0) is DBNull) ? ((String)dr1.GetValue(0)).Trim().ToUpper() : "";
                        polLpu.Im = !(dr1.GetValue(1) is DBNull) ? ((String)dr1.GetValue(1)).Trim().ToUpper() : "";
                        polLpu.Ot = !(dr1.GetValue(2) is DBNull) ? ((String)dr1.GetValue(2)).Trim().ToUpper() : "";
                        polLpu.Dr = (dr1.GetValue(3) is DateTime) ? (DateTime)dr1.GetValue(3) : polLpu.Dr;
                        polLpu.Polis = !(dr1.GetValue(4) is DBNull) ? (String)dr1.GetValue(4) : "";
                        polLpu.Q = !(dr1.GetValue(5) is DBNull) ? (String)dr1.GetValue(5) : polLpu.Q;
                        polLpu.Date1 = (dr1.GetValue(6) is DateTime) ? (DateTime)dr1.GetValue(6) : polLpu.Date1;
                        polLpu.Date2 = (dr1.GetValue(7) is DateTime) ? (DateTime)dr1.GetValue(7) : polLpu.Date2;
                        polLpu.Id = (int)dr1.GetValue(8);
                        polLpu.Sumv = (dr1.GetValue(9) is Decimal) ? (Decimal)dr1.GetValue(9) : polLpu.Sumv;
                        lpList.Add(polLpu);

                    }
                    dr1.Close();
                    con1.Close();
                }

                List<D3_SANK_OMS> mList = new List<D3_SANK_OMS>();

                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    AutoMekElement1.LogBox.Text += "Проверка:" + Environment.NewLine;
                });

                int count = 100;
                int i = 0;
                int c = 0;
                int max = lpList.Count();
                int progressInt = max / count;
                int progressIntLock = progressInt;
                int[] progressInts = new int[count];
                for (int j = 0; j < count; j++)
                {
                    progressInts[j] = progressInt;
                    progressInt += progressIntLock;
                }
                progressInts[count - 1] = max;
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                       new Action(delegate ()
                                       {
                                           progressBar1.EditValue = 0;
                                           progressBar1.Visibility = Visibility;
                                       }));
                int ii = 0;




                foreach (var p in lpList)
                {
                    bool isValid = false;
                    bool isPolisCheck = false;
                    string coment = "";
                    string coment2 = "";
                    string coment3 = "";
                    string polisOsn = "";
                    dynamic att = null;

                    if (i == progressInts[c])
                    {
                        if (c < count - 1) c += 1;
                        Dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(delegate ()
                            {
                                ii++;
                                progressBar1.EditValue = ii;
                            }));
                    }
                    i += 1;

                    List<PolisCheck> polisChecks = new List<PolisCheck>();
                    using (SqlConnection con1 = new SqlConnection(SprClass.GlobalDocExchangeConnectionString))
                    {


                        var cmdStr = $@"EXEC [dbo].[P_ATTP_DISP_Mek] '{p.Polis}', '{p.Fam}', '{p.Im}', '{p.Ot}', '{p.Dr?.Date.ToString("yyyyMMdd")}', '{p.Date2?.Date.ToString("yyyyMMdd")}', '{p.Date2?.Date.ToString("yyyyMMdd")}'";

                        using (SqlCommand cmd1 = new SqlCommand(cmdStr, con1))
                        {
                            con1.Open();
                            cmd1.CommandTimeout = 0;
                            using (SqlDataReader dr1 = cmd1.ExecuteReader())
                            {
                                int ip = 0;
                                while (dr1.Read())
                                {
                                    if (att == null)
                                        att = new ExpandoObject();
                                    att.ID_MO = !(dr1.GetValue(0) is DBNull) ? (String)dr1.GetValue(0) : "";
                                }
                                dr1.Close();
                            }
                            con1.Close();
                        }
                    }

                    if (att == null)
                    {
                        isValid = false;
                        coment = $"Нет прикрепления к МО-{p.Q} на дату завершения случая";
                        polisOsn = "5.3.1.";
                    }
                    else if (att.ID_MO != p.Q)
                    {
                        isValid = false;
                        coment = "Прикрепление к другой МО-" + att.ID_MO;
                        polisOsn = "5.3.1.";
                    }
                    else
                        isValid = true;



                    if (!isValid)
                    {
                        mList.Add(new D3_SANK_OMS()
                        {
                            D3_ZSLID = p.Id,
                            S_CODE = Guid.NewGuid().ToString(),
                            S_SUM = p.Sumv,
                            S_OSN = polisOsn,
                            S_TIP = 1,
                            S_TIP2 = 1,
                            S_IST = 1,
                            USER_ID = SprClass.userId,
                            S_COM = coment,
                            S_ZAKL = coment,
                            S_DATE = DateTime.Today,
                            D3_SCID = reestrId
                        });
                        //SluchUpdateContructor(coment2, p.Id);
                    }
                }

                if (mList.Any())
                {
                    Reader2List.BulkInsert(mList, 100, SprClass.LocalConnectionString);
                }
                //if (sb.Length > 0) SluchUpdate(sb.ToString());
                if (sb2.Length > 0) SluchUpdate(sb2.ToString());

            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    ErrorGlobalWindow.ShowError(ex.Message);
                });
            }
        }



        private void PolisCheckForTerIgs(int reestrId)
        {
            //var ef = new[] {"Е", "Ё"};

            //sb = new StringBuilder();
            sb2 = new StringBuilder();


            //TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext(); //get UI thread context 
            Dispatcher.BeginInvoke((Action)delegate ()
            {
                //LogBox.Clear();
                AutoMekElement1.LogBox.Text += "Загрузка реестра...";
            });
            //var ids = new ConcurrentBag<int>();
            //var mekTask = Task.Factory.StartNew(() =>
            //{
            try
            {
                List<PolCheckLPU> lpList = new List<PolCheckLPU>();
                using (SqlConnection con1 = new SqlConnection(SprClass.LocalConnectionString))
                {
                    SqlCommand cmd1 = new SqlCommand(string.Format(@"
                                select pa.fam, pa.im, pa.ot, pa.dr, pa.npolis, pa.SMO, zsl.DATE_Z_1, zsl.DATE_Z_2, zsl.id, zsl.sumv from d3_zsl_oms zsl
                                join d3_pacient_oms pa on zsl.D3_PID = pa.ID
                                where zsl.D3_SCID = {0}", reestrId), con1);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    con1.Open();

                    cmd1.CommandTimeout = 0;
                    SqlDataReader dr1 = cmd1.ExecuteReader();
                    //Dispatcher.BeginInvoke((Action)delegate()
                    //{

                    //});

                    while (dr1.Read())
                    {
                        PolCheckLPU polLpu = new PolCheckLPU();
                        polLpu.Fam = !(dr1.GetValue(0) is DBNull) ? ((String)dr1.GetValue(0)).Trim().ToUpper() : "";
                        polLpu.Im = !(dr1.GetValue(1) is DBNull) ? ((String)dr1.GetValue(1)).Trim().ToUpper() : "";
                        polLpu.Ot = !(dr1.GetValue(2) is DBNull) ? ((String)dr1.GetValue(2)).Trim().ToUpper() : "";
                        polLpu.Dr = (dr1.GetValue(3) is DateTime) ? (DateTime)dr1.GetValue(3) : polLpu.Dr;
                        polLpu.Polis = !(dr1.GetValue(4) is DBNull) ? (String)dr1.GetValue(4) : "";
                        polLpu.Q = !(dr1.GetValue(5) is DBNull) ? (String)dr1.GetValue(5) : polLpu.Q;
                        polLpu.Date1 = (dr1.GetValue(6) is DateTime) ? (DateTime)dr1.GetValue(6) : polLpu.Date1;
                        polLpu.Date2 = (dr1.GetValue(7) is DateTime) ? (DateTime)dr1.GetValue(7) : polLpu.Date2;
                        polLpu.Id = (int)dr1.GetValue(8);
                        polLpu.Sumv = (dr1.GetValue(9) is Decimal) ? (Decimal)dr1.GetValue(9) : polLpu.Sumv;
                        lpList.Add(polLpu);

                    }
                    dr1.Close();
                    con1.Close();
                }

                List<D3_SANK_OMS> mList = new List<D3_SANK_OMS>();

                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    //LogBox.Clear();
                    AutoMekElement1.LogBox.Text += "Проверка:" + Environment.NewLine;
                });

                int count = 100;
                int i = 0;
                int c = 0;
                int max = lpList.Count();
                int progressInt = max / count;
                int progressIntLock = progressInt;
                int[] progressInts = new int[count];
                for (int j = 0; j < count; j++)
                {
                    progressInts[j] = progressInt;
                    progressInt += progressIntLock;
                }
                progressInts[count - 1] = max;
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                       new Action(delegate ()
                                       {
                                           progressBar1.EditValue = 0;
                                           progressBar1.Visibility = Visibility;
                                       }));
                int ii = 0;




                foreach (var p in lpList) //, p =>
                {
                    bool isValid = false;
                    bool isPolisCheck = false;
                    string coment = "";
                    string coment2 = "";
                    string coment3 = "";
                    string polisOsn = "";


                    if (i == progressInts[c])
                    {
                        if (c < count - 1) c += 1;
                        Dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(delegate ()
                            {
                                ii++;
                                progressBar1.EditValue = ii;
                            }));
                    }
                    i += 1;

                    //if (p.Fam == "МАХАНЬКОВ")
                    //    p.Fam = "МАХАНЬКОВ";
                    List<PolisCheck> polisChecks = new List<PolisCheck>();
                    using (SqlConnection con1 = new SqlConnection(SprClass.SrzConnectionString))
                    {
                        var cmdStr = @"select p.FAM, p.IM, p.OT, p.DR, NPOL, p.DBEG, p.DEND, p.DSTOP, p.Q, p.OPDOC, p.ENP, p.ID
                                    from PEOPLE p where ID = (SELECT TOP 1 PID 
                                    from HISTFDR where";
                        if (p.Fam.Contains("Е") || p.Fam.Contains("Ё"))
                            cmdStr = cmdStr + string.Format(@" FAM LIKE '{0}'", EFix(p.Fam));
                        else
                            cmdStr = cmdStr + string.Format(@" FAM = '{0}'", p.Fam);
                        if (p.Im.Contains("Е") || p.Im.Contains("Ё"))
                            cmdStr = cmdStr + string.Format(@" and IM LIKE '{0}'", EFix(p.Im));
                        else
                            cmdStr = cmdStr + string.Format(@" and IM = '{0}'", p.Im);
                        if (p.Ot.Contains("Е") || p.Ot.Contains("Ё"))
                            cmdStr = cmdStr + string.Format(@" and OT LIKE '{0}'", EFix(p.Ot));
                        else
                            cmdStr = cmdStr + string.Format(@" and OT = '{0}'", p.Ot);
                        cmdStr = cmdStr + string.Format(@" and DR = '{0}')", p.Dr.Value.ToString("yyyyMMdd"));

                        using (SqlCommand cmd1 = new SqlCommand(cmdStr, con1))
                        {
                            con1.Open();
                            cmd1.CommandTimeout = 0;
                            using (SqlDataReader dr1 = cmd1.ExecuteReader())
                            {
                                int ip = 0;
                                while (dr1.Read())
                                {
                                    PolisCheck polCh = new PolisCheck();
                                    polCh.Fam = !(dr1.GetValue(0) is DBNull) ? (String)dr1.GetValue(0) : "";
                                    polCh.Im = !(dr1.GetValue(1) is DBNull) ? (String)dr1.GetValue(1) : "";
                                    polCh.Ot = !(dr1.GetValue(2) is DBNull) ? (String)dr1.GetValue(2) : "";
                                    polCh.Dr = (dr1.GetValue(3) is DateTime) ? (DateTime)dr1.GetValue(3) : polCh.Dr;
                                    polCh.Polis = !(dr1.GetValue(4) is DBNull) ? (String)dr1.GetValue(4) : null;
                                    polCh.Dbeg = (dr1.GetValue(5) is DateTime) ? (DateTime)dr1.GetValue(5) : polCh.Dbeg;
                                    polCh.Dend = (dr1.GetValue(6) is DateTime) ? (DateTime)dr1.GetValue(6) : polCh.Dend;
                                    polCh.Dstop = (dr1.GetValue(7) is DateTime) ? (DateTime)dr1.GetValue(7) : polCh.Dstop;
                                    polCh.Q = !(dr1.GetValue(8) is DBNull) ? (String)dr1.GetValue(8) : null;
                                    polCh.Opdoc = !(dr1.GetValue(9) is DBNull) ? (int)dr1.GetValue(9) : polCh.Opdoc;
                                    polCh.ENP = !(dr1.GetValue(10) is DBNull) ? (String)dr1.GetValue(10) : null;
                                    polCh.Id = (int)dr1.GetValue(11);
                                    polisChecks.Add(polCh);

                                    //Dispatcher.BeginInvoke((Action)delegate()
                                    //{
                                    //    barStaticItem1.Content = ip++.ToString();
                                    //});
                                }
                                dr1.Close();
                            }
                            con1.Close();
                        }
                    }

                    if (polisChecks.Any())
                    {
                        var fq = polisChecks.First();
                        if ((p.Date2 >= fq.Dbeg && (fq.Dend == null || p.Date2 < fq.Dend.Value.AddDays(1)) &&
                            (fq.Dstop == null || p.Date2 < fq.Dstop.Value.AddDays(1))) || (p.Date2 > fq.Dend && fq.Opdoc == 2))
                        {
                            if (p.Q == fq.Q)
                            {
                                isValid = true;
                            }
                            else
                            {
                                isValid = false;
                                coment = "Страхование в " + ConvertSmo(fq.Q);
                                coment2 = "Страхование в другой СМО";
                                polisOsn = "5.2.1.";
                            }
                        }
                        else
                        {
                            List<PolisCheck> polisChecks1 = new List<PolisCheck>();
                            using (SqlConnection con1 = new SqlConnection(SprClass.SrzConnectionString))
                            {
                                using (SqlCommand cmd1 = new SqlCommand(string.Format(@"
                                            Select pol.NPOL, pol.DBEG, pol.DEND, pol.DSTOP, pol.Q, pol.POLTP
                                            From POLIS pol WHERE pol.PID = {0}", fq.Id), con1))
                                {
                                    con1.Open();
                                    cmd1.CommandTimeout = 0;
                                    using (SqlDataReader dr1 = cmd1.ExecuteReader())
                                    {
                                        //Dispatcher.BeginInvoke((Action)delegate()
                                        //{

                                        //});
                                        while (dr1.Read())
                                        {
                                            PolisCheck polCh = new PolisCheck();
                                            polCh.H_Polis = !(dr1.GetValue(0) is DBNull) ? (String)dr1.GetValue(0) : null;
                                            polCh.H_Dbeg = (dr1.GetValue(1) is DateTime) ? (DateTime)dr1.GetValue(1) : polCh.H_Dbeg;
                                            polCh.H_Dend = (dr1.GetValue(2) is DateTime) ? (DateTime)dr1.GetValue(2) : polCh.H_Dend;
                                            polCh.H_Dstop = (dr1.GetValue(3) is DateTime) ? (DateTime)dr1.GetValue(3) : polCh.H_Dstop;
                                            polCh.H_Q = !(dr1.GetValue(4) is DBNull) ? (String)dr1.GetValue(4) : null;
                                            polCh.Opdoc2 = !(dr1.GetValue(5) is DBNull) ? (int)dr1.GetValue(5) : polCh.Opdoc2;

                                            polisChecks1.Add(polCh);
                                        }
                                        dr1.Close();
                                    }
                                    con1.Close();
                                }
                            }

                            if (polisChecks1.Any())
                            {
                                foreach (var po in polisChecks1)
                                {
                                    isValid = false;
                                    coment = "Нет действующего страхования.";
                                    coment2 = "Нет действующего страхования.";
                                    polisOsn = "5.2.4.";

                                    if (p.Date2 >= po.H_Dbeg &&
                                        (po.H_Dend == null || p.Date2 < po.H_Dend.Value.AddDays(1)) &&
                                        (po.H_Dstop == null || p.Date2 < po.H_Dstop.Value.AddDays(1)))
                                    {
                                        if (p.Q == po.H_Q)
                                        {
                                            isValid = true;
                                            break;
                                        }
                                        isValid = false;
                                        coment = "Страхование в " + ConvertSmo(po.H_Q);
                                        coment2 = "Страхование в другой СМО";
                                        polisOsn = "5.2.1.";

                                        break;
                                    }
                                }
                            }
                            else
                            {
                                isValid = false;
                                coment = "Нет действующего страхования.";
                                coment2 = "Нет действующего страхования.";
                                polisOsn = "5.2.4.";

                            }
                        }
                    }
                    else
                    {
                        isValid = false;
                        coment = "Не удалось идентифицировать по ФИО ДР.";
                        coment2 = "Не удалось идентифицировать по ФИО ДР.";
                        polisOsn = "5.2.2.";


                        List<PolisCheck> polisChecks2 = new List<PolisCheck>();
                        using (SqlConnection con1 = new SqlConnection(SprClass.SrzConnectionString))
                        {
                            var cmdStr = string.Format(@"select p.FAM, p.IM, p.OT, p.DR, NPOL, p.DBEG, p.DEND, p.DSTOP, p.Q, p.OPDOC, p.ENP, p.ID
                                            from PEOPLE p where NPOL = '{0}' OR ENP = '{0}'", p.Polis);

                            using (SqlCommand cmd1 = new SqlCommand(cmdStr, con1))
                            {
                                con1.Open();
                                cmd1.CommandTimeout = 0;
                                using (SqlDataReader dr1 = cmd1.ExecuteReader())
                                {
                                    while (dr1.Read())
                                    {
                                        PolisCheck polCh = new PolisCheck();
                                        polCh.Fam = !(dr1.GetValue(0) is DBNull) ? (String)dr1.GetValue(0) : "";
                                        polCh.Im = !(dr1.GetValue(1) is DBNull) ? (String)dr1.GetValue(1) : "";
                                        polCh.Ot = !(dr1.GetValue(2) is DBNull) ? (String)dr1.GetValue(2) : "";
                                        polCh.Dr = (dr1.GetValue(3) is DateTime) ? (DateTime)dr1.GetValue(3) : polCh.Dr;
                                        polCh.Polis = !(dr1.GetValue(4) is DBNull) ? (String)dr1.GetValue(4) : null;
                                        polCh.Dbeg = (dr1.GetValue(5) is DateTime) ? (DateTime)dr1.GetValue(5) : polCh.Dbeg;
                                        polCh.Dend = (dr1.GetValue(6) is DateTime) ? (DateTime)dr1.GetValue(6) : polCh.Dend;
                                        polCh.Dstop = (dr1.GetValue(7) is DateTime) ? (DateTime)dr1.GetValue(7) : polCh.Dstop;
                                        polCh.Q = !(dr1.GetValue(8) is DBNull) ? (String)dr1.GetValue(8) : null;
                                        polCh.Opdoc = !(dr1.GetValue(9) is DBNull) ? (int)dr1.GetValue(9) : polCh.Opdoc;
                                        polCh.ENP = !(dr1.GetValue(10) is DBNull) ? (String)dr1.GetValue(10) : null;
                                        polCh.Id = (int)dr1.GetValue(11);
                                        polisChecks2.Add(polCh);
                                    }
                                    dr1.Close();
                                }
                                con1.Close();
                            }
                        }
                        if (polisChecks2.Any())
                        {
                            var fq2 = polisChecks2.First();
                            if (p.Date2 >= fq2.Dbeg && (fq2.Dend == null || p.Date2 < fq2.Dend.Value.AddDays(1)) &&
                                (fq2.Dstop == null || p.Date2 < fq2.Dstop.Value.AddDays(1)))
                            {
                                if (p.Q == fq2.Q)
                                {
                                    isValid = false;
                                    isPolisCheck = true;
                                    coment3 = string.Format(@"Полис выдан: {0} {1} {2} {3}", fq2.Fam, fq2.Im, fq2.Ot, fq2.Dr.Value.ToShortDateString());
                                }
                                else
                                {
                                    isValid = false;
                                    isPolisCheck = true;
                                    coment3 = string.Format(@"Полис выдан: {0} {1} {2} {3}, СМО: {4}",
                                        fq2.Fam, fq2.Im, fq2.Ot, fq2.Dr.Value.ToShortDateString(), fq2.Q);
                                }
                            }
                            else
                            {
                                isValid = false;
                                isPolisCheck = true;
                                var dBeg = fq2.Dbeg != null ? fq2.Dbeg.Value.ToShortDateString() : "";
                                var dEnd = fq2.Dend != null ? fq2.Dend.Value.ToShortDateString() : "";
                                var dStop = fq2.Dstop != null ? fq2.Dstop.Value.ToShortDateString() : "";
                                coment3 = string.Format(@"Полис выдан: {0} {1} {2} {3}, Действует с: {4} по: {5}, Изъят: {6}",
                                    fq2.Fam, fq2.Im, fq2.Ot, fq2.Dr, dBeg, dEnd, dStop);
                            }
                        }
                    }

                    if (!isValid)
                    {
                        mList.Add(new D3_SANK_OMS()
                        {
                            D3_ZSLID = p.Id,
                            S_CODE = Guid.NewGuid().ToString(),
                            S_SUM = p.Sumv,
                            S_OSN = polisOsn,
                            S_TIP = 1,
                            S_TIP2 = 1,
                            S_IST = 1,
                            USER_ID = SprClass.userId,
                            S_COM = coment,
                            S_ZAKL = coment,
                            S_DATE = DateTime.Today,
                            D3_SCID = reestrId
                        });
                        //SluchUpdateContructor(coment2, p.Id);
                    }
                    if (isPolisCheck)
                    {
                        SluchUpdateContructor2(coment3, p.Id);
                    }
                }

                if (mList.Any())
                {
                    Reader2List.BulkInsert(mList, 100, SprClass.LocalConnectionString);
                }
                //if (sb.Length > 0) SluchUpdate(sb.ToString());
                if (sb2.Length > 0) SluchUpdate(sb2.ToString());
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    ErrorGlobalWindow.ShowError(ex.Message);
                });
            }
        }

        private StringBuilder sb;
        private StringBuilder sb2;
        private void SluchUpdateContructor(string coment, int id)
        {
            sb.AppendLine(string.Format(@"UPDATE D3_ZSL_OMS SET MEK_COMENT = '{0}' WHERE ID = {1} AND (MEK_COMENT IS NULL OR MEK_COMENT = '')", coment, id));
        }

        private void SluchUpdateContructor2(string coment, int id)
        {
            sb2.AppendLine(string.Format(@"UPDATE D3_ZSL_OMS SET OSP_COMENT = '{0}' WHERE ID = {1}", coment, id));
        }
        static public void SluchUpdate(string command)
        {
            using (SqlConnection con = new SqlConnection(SprClass.LocalConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(command, con))
                {
                    con.Open();
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        static public string EFix(string eFix)
        {
            var ef = eFix.Replace("Е", "@").Replace("Ё", "@");
            return ef.Replace("@", "[ЕЁ]");
        }

        public static string ConvertSmo(string smoCod)
        {
            switch (smoCod)
            {
                case "46001":
                    return "Недействующий полис";
                case "46002":
                    return "ВТБ МС";
                case "46003":
                    return "ИНГОССТРАХ-М";
                case "46004":
                    return "СПАССКИЕ ВОРОТА-М";
                case "46006":
                    return "ИНКО-МЕД";
                default:
                    return smoCod;
            }
        }

        private void AutoMekStartButton_Click(object sender, RoutedEventArgs e)
        {
            AutoMekStart();
        }
    }
}
