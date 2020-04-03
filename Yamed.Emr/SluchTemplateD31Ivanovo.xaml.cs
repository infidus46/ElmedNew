using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.XtraEditors.DXErrorProvider;
using Yamed.Control;
using Yamed.Core;
using Yamed.Entity;
using Yamed.Server;
using System.Threading;
using DevExpress.Data;
using DevExpress.Data.Async.Helpers;

using System.Windows.Data;

using System.Windows.Markup;
using System.Windows.Media;
using System.Collections;
using Yamed.OmsExp.ExpEditors;

namespace Yamed.Emr
{

    //public class SL_KSG
    //{
    //    private string ds1;
    //    public D3_SL_OMS sl { get; set; }
    //    public D3_KSG_KPG_OMS ksg { get; set; }
    //    public int ID { get; set; }
    //    public string SL_ID { get; set; }
    //    public DateTime? DATE_1 { get; set; }
    //    public DateTime? DATE_2 { get; set; }
    //    public string DS1 { get; set; }
    //    //{
    //    //    set
    //    //    {

    //    //        ds1 = value;
    //    //    }
    //    //    get { return sl.DS1; }
    //    //}
    //    public decimal? TARIF { get; set; }
    //    public decimal? ED_COL { get; set; }
    //    public decimal? SUM_M { get; set; }
    //}


    /// <summary>
    /// Логика взаимодействия для SluchTemplate.xaml
    /// </summary>
    public partial class SluchTemplateD31Ivanovo : UserControl
    {

        private int rowIndex;
        private GridControl _gc;

        public D3_ZSL_OMS _zsl;
        //public List<D3_SL_OMS> _slList;
        public List<D3_SL_OMS> _slList;
        public List<D3_USL_OMS> _uslList;
        private D3_PACIENT_OMS _pacient;
        public List<D3_SANK_OMS> _sankList;

        private List<D3_NAZ_OMS> _nazList;
        private List<D3_DSS_OMS> _dssList;
        private List<D3_KSG_KPG_OMS> _ksgList;
        private List<D3_SL_KOEF_OMS> _kslpList;
        private List<D3_CRIT_OMS> _critList;

        private List<D3_NAPR_OMS> _naprList;
        private List<D3_CONS_OMS> _consList;
        private List<D3_ONK_SL_OMS> _onkSlList;
        private List<D3_ONK_USL_OMS> _onklUslList;
        private List<D3_B_DIAG_OMS> _diagList;
        private List<D3_B_PROT_OMS> _protList;
        private List<D3_LEK_PR_OMS> _lekList;
               
        private List<D3_NAPR_OMS> _napr_delList;
        private List<D3_CONS_OMS> _cons_delList;
        private List<D3_ONK_SL_OMS> _onkSl_delList;
        private List<D3_ONK_USL_OMS> _onklUsl_delList;
        private List<D3_B_DIAG_OMS> _diag_delList;
        private List<D3_B_PROT_OMS> _prot_delList;
        private List<D3_LEK_PR_OMS> _lek_delList;

        private List<D3_DSS_OMS> _dss_delList;
        private List<D3_NAZ_OMS> _naz_delList;

        private List<D3_SL_KOEF_OMS> _kslp_delList;
        private List<D3_CRIT_OMS> _crit_delList;
        public List<D3_USL_OMS> _usl_delList;


        public SluchTemplateD31Ivanovo(GridControl gc)
        {
            InitializeComponent();

            _gc = gc;

            rowIndex = _gc.GetSelectedRowHandles().Where(x => x >= 0).FirstOrDefault();
            //var row = _gc.GetRow(rowIndex);
            //if (SprClass.Region.ToString() == "46" && SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu)
            //{
            //    SlAddItem.IsEnabled = false;
            //}
            //DevExpress.Xpf.Core.DXGridDataController.DisableThreadingProblemsDetection = true;
            if (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo)
            {
                SlAddItem.IsEnabled = false;
                SlDelItem.IsEnabled = false;
                UslAddItem.IsEnabled = false;
                UslDelItem.IsEnabled = false;
                UslAutoTemplateItem.IsEnabled = false;
                UslEditItem.IsEnabled = false;
                UslTemplateItem.IsEnabled = false;
                Ds2AddItem.IsEnabled = false;
                Ds2DelItem.IsEnabled = false;
                KslpAddItem.IsEnabled = false;
                KslpDelItem.IsEnabled = false;
                CritAddItem.IsEnabled = false;
                CritDelItem.IsEnabled = false;
                NaprAddItem.IsEnabled = false;
                NaprDelItem.IsEnabled = false;
                NaznAddItem.IsEnabled = false;
                NaznDelItem.IsEnabled = false;
                BdiagAddItem.IsEnabled = false;
                BdiagDelItem.IsEnabled = false;
                BprotAddItem.IsEnabled = false;
                BprotDelItem.IsEnabled = false;
                OnkslAddItem.IsEnabled = false;
                OnkslDelItem.IsEnabled = false;
                OnkUslAddItem.IsEnabled = false;
                OnkUslDelItem.IsEnabled = false;
                LekprAddItem.IsEnabled = false;
                LekprDelItem.IsEnabled = false;
                ConsAddItem.IsEnabled = false;
                ConsDelItem.IsEnabled = false;
            }
        }

        public void BindPacient(int pid)
        {

                Task.Factory.StartNew(() => _pacient = PacientData(pid)).ContinueWith(x => PacientGroup.DataContext = _pacient, TaskScheduler.FromCurrentSynchronizationContext());

        }

        public void BindEmptyPacient()
        {
            Task.Factory.StartNew(() => _pacient = PacientData()).ContinueWith(x => PacientGroup.DataContext = _pacient, TaskScheduler.FromCurrentSynchronizationContext());
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                new Action(delegate ()
                {
                    polisBox.Focus();
                }));

        }

        public D3_PACIENT_OMS PacientData()
        {
            return new D3_PACIENT_OMS { NOVOR = "0", ID_PAC = Guid.NewGuid().ToString() };
        }

        public D3_PACIENT_OMS PacientData(int pid)
        {
            
            return Reader2List.CustomSelect<D3_PACIENT_OMS>($"SELECT top 1 * FROM D3_PACIENT_OMS WHERE ID = {pid}", SprClass.LocalConnectionString).Single();
            //Dost = SqlReader.Select($"SELECT * FROM PACIENT_DOST WHERE PID = {pid}", SprClass.LocalConnectionString).ToArray();
            //Dostp = SqlReader.Select($"SELECT * FROM PACIENT_DOSTP WHERE PID = {pid}", SprClass.LocalConnectionString);
            
        }

        //public void BindSluch(int slid, D3_SCHET_OMS sc = null)
        //{
        //    if (_sc == null) _sc = sc;

        //    var i = 0;
        //    var ii = -1;

        //    Task.Factory.StartNew(() =>
        //    {
        //        return Reader2List.CustomSelect<D3_ZSL_OMS>($"Select * from D3_ZSL_OMS where ID = {slid}",
        //            SprClass.LocalConnectionString)[0];
        //    }).ContinueWith((tz) =>
        //    {
        //        _zsl = tz.Result;
        //        ZSlGrid.DataContext = _zsl;

        //        BindPacient(_zsl.D3_PID);

        //        Task.Factory.StartNew(() =>
        //        {
        //            return Reader2List.CustomSelect<D3_SANK_OMS>($"Select * from D3_SANK_OMS where D3_ZSLID = {slid}",
        //                SprClass.LocalConnectionString);
        //        }).ContinueWith((u) =>
        //        {
        //            _sankList = u.Result;
        //            if (_sankList == null) _sankList = new List<D3_SANK_OMS>();
        //            SankGridControl.DataContext = _sankList;
        //            u.Dispose();
        //        }, TaskScheduler.FromCurrentSynchronizationContext());

        //        Task.Factory.StartNew(() =>
        //        {
        //            return Reader2List.CustomSelect<D3_SL_OMS>($"Select * from D3_SL_OMS where D3_ZSLID = {slid}",
        //                SprClass.LocalConnectionString);

        //        }).ContinueWith((s) =>
        //        {
        //            _slList = s.Result;

        //            var slids = ObjHelper.GetIds(_slList.Select(x => x.ID).ToArray());

        //            s.Dispose();

        //            //КСГ
        //            if (_zsl.USL_OK == 1 || _zsl.USL_OK == 2)
        //            {
        //                ii = 7;
        //                Task.Factory.StartNew(() =>
        //                {
        //                    return Reader2List.CustomSelect<D3_KSG_KPG_OMS>($"Select * from D3_KSG_KPG_OMS where D3_SLID in ({slids})",
        //                        SprClass.LocalConnectionString);
        //                }).ContinueWith((ksg) =>
        //                {
        //                    _ksgList = ksg.Result;
        //                    i++;
        //                    ksg.Dispose();
        //                }, TaskScheduler.FromCurrentSynchronizationContext());
        //            }
        //            else ii = 6;

        //            //Услуги
        //            Task.Factory.StartNew(() =>
        //            {
        //                return Reader2List.CustomSelect<D3_USL_OMS>($"Select * from D3_USL_OMS where D3_ZSLID = {slid}",
        //                    SprClass.LocalConnectionString);
        //            }).ContinueWith((u) =>
        //            {
        //                _uslList = u.Result;

        //                UslGrid.DataContext = _uslList;
        //                i++;
        //                u.Dispose();
        //            }, TaskScheduler.FromCurrentSynchronizationContext());

            
        //            //Диагнозы
        //            Task.Factory.StartNew(() =>
        //            {
        //                return Reader2List.CustomSelect<D3_DSS_OMS>($"Select * from D3_DSS_OMS where D3_SLID in ({slids})",
        //                    SprClass.LocalConnectionString);
        //            }).ContinueWith((dss) =>
        //            {
        //                _dssList = dss.Result;
        //                Ds2GridControl.DataContext = _dssList;
        //                i++;

        //                dss.Dispose();
        //            }, TaskScheduler.FromCurrentSynchronizationContext());


        //            //Назначения
        //            Task.Factory.StartNew(() =>
        //            {
        //                return Reader2List.CustomSelect<D3_NAZ_OMS>($"Select * from D3_NAZ_OMS where D3_SLID in ({slids})",
        //                    SprClass.LocalConnectionString);
        //            }).ContinueWith((nz) =>
        //            {
        //                _nazList = nz.Result;
        //                NazGridControl.DataContext = _nazList;
        //                i++;

        //                nz.Dispose();
        //            }, TaskScheduler.FromCurrentSynchronizationContext());

        //            //Направления
        //            Task.Factory.StartNew(() =>
        //            {
        //                return Reader2List.CustomSelect<D3_NAPR_OMS>($"Select * from D3_NAPR_OMS where D3_SLID in ({slids})",
        //                    SprClass.LocalConnectionString);
        //            }).ContinueWith((n) =>
        //            {
        //                _naprList = n.Result;
        //                NaprGridControl.DataContext = _naprList;
        //                i++;

        //                n.Dispose();
        //            }, TaskScheduler.FromCurrentSynchronizationContext());

        //            //Консилиум
        //            Task.Factory.StartNew(() =>
        //            {
        //                return Reader2List.CustomSelect<D3_CONS_OMS>($"Select * from D3_CONS_OMS where D3_SLID in ({slids})",
        //                    SprClass.LocalConnectionString);
        //            }).ContinueWith((c) =>
        //            {
        //                _consList = c.Result;
        //                ConsGridControl.DataContext = _consList;
        //                i++;

        //                c.Dispose();
        //            }, TaskScheduler.FromCurrentSynchronizationContext());

        //            //Случай онко
        //            Task.Factory.StartNew(() =>
        //            {
        //                return Reader2List.CustomSelect<D3_ONK_SL_OMS>($"Select * from D3_ONK_SL_OMS where D3_SLID in ({slids})",
        //                    SprClass.LocalConnectionString);
        //            }).ContinueWith((osl) =>
        //            {
        //                _onkSlList = osl.Result;
        //                //OnkSlGridControl.DataContext = _onkSlList;
        //                i++;

        //                osl.Dispose();

        //                if (_onkSlList.Count > 0)
        //                {
        //                    var onkid = _onkSlList[0].ID;

        //                    //Диагностический блок
        //                    Task.Factory.StartNew(() =>
        //                    {
        //                        return Reader2List.CustomSelect<D3_B_DIAG_OMS>($"Select * from D3_B_DIAG_OMS where D3_ONKSLID = {onkid}",
        //                            SprClass.LocalConnectionString);
        //                    }).ContinueWith((diag) =>
        //                    {
        //                        _diagList = diag.Result;
        //                        BdiagGridControl.DataContext = _diagList;
        //                        diag.Dispose();
        //                    }, TaskScheduler.FromCurrentSynchronizationContext());

        //                    //Противопоказания
        //                    Task.Factory.StartNew(() =>
        //                    {
        //                        return Reader2List.CustomSelect<D3_B_PROT_OMS>($"Select * from D3_B_PROT_OMS where D3_ONKSLID = {onkid}",
        //                            SprClass.LocalConnectionString);
        //                    }).ContinueWith((prot) =>
        //                    {
        //                        _protList = prot.Result;
        //                        BprotGridControl.DataContext = _protList;
        //                        prot.Dispose();
        //                    }, TaskScheduler.FromCurrentSynchronizationContext());

        //                    //Онко услуги
        //                    Task.Factory.StartNew(() =>
        //                    {
        //                        return Reader2List.CustomSelect<D3_ONK_USL_OMS>($"Select * from D3_ONK_USL_OMS where D3_ONKSLID = {onkid}",
        //                            SprClass.LocalConnectionString);
        //                    }).ContinueWith((ousl) =>
        //                    {
        //                        _onklUslList = ousl.Result;
        //                        OnkUslGridControl.DataContext = _onklUslList;
        //                        ousl.Dispose();

        //                        // Лекарства
        //                        if (_onklUslList.Count > 0)
        //                        {
        //                            var ouslids = ObjHelper.GetIds(_onklUslList.Select(x => x.ID).ToArray());
        //                            Task.Factory.StartNew(() =>
        //                            {
        //                                return Reader2List.CustomSelect<D3_LEK_PR_OMS>($"Select * from D3_LEK_PR_OMS where D3_ONKUSLID in ({ouslids})",
        //                                    SprClass.LocalConnectionString);
        //                            }).ContinueWith((lek) =>
        //                            {
        //                                _lekList = lek.Result;
        //                                LekprGridControl.DataContext = _lekList;
        //                                lek.Dispose();
        //                            }, TaskScheduler.FromCurrentSynchronizationContext());

        //                        }
        //                    }, TaskScheduler.FromCurrentSynchronizationContext());
        //                }
        //            }, TaskScheduler.FromCurrentSynchronizationContext());
        //        }, TaskScheduler.FromCurrentSynchronizationContext());

        //        Task.Factory.StartNew(() =>
        //        {
        //            while (true)
        //            {

        //                if (i == ii)
        //                {
        //                    break;
        //                }
        //                //Thread.Sleep(100);
        //            }
        //        }).ContinueWith((b) =>
        //        {
        //            SlGrid.DataContext = _slList;
        //            NextButton.IsEnabled = true;
        //            PrevButton.IsEnabled = true;
        //            b.Dispose();

        //        }, TaskScheduler.FromCurrentSynchronizationContext());

        //        tz.Dispose();
        //    }, TaskScheduler.FromCurrentSynchronizationContext());
        //}

        public void BindSluch(int slid, D3_SCHET_OMS sc = null)
        {
            if (_sc == null) _sc = sc;

            var i = 0;
            var ii = -1;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _zsl = Reader2List.CustomSelect<D3_ZSL_OMS>($"Select * from D3_ZSL_OMS where ID = {slid}",
                        SprClass.LocalConnectionString)[0];

                    _sankList = Reader2List.CustomSelect<D3_SANK_OMS>($"Select * from D3_SANK_OMS where D3_ZSLID = {slid}",
        SprClass.LocalConnectionString);

                    _slList = Reader2List.CustomSelect<D3_SL_OMS>($"Select * from D3_SL_OMS where D3_ZSLID = {slid}",
        SprClass.LocalConnectionString);
                    var slids = ObjHelper.GetIds(_slList.Select(x => x.ID).ToArray());

                    if (_zsl.USL_OK == 1 || _zsl.USL_OK == 2)
                    {
                        _ksgList = Reader2List.CustomSelect<D3_KSG_KPG_OMS>($"Select * from D3_KSG_KPG_OMS where D3_SLID in ({slids})",
                                SprClass.LocalConnectionString);
                        var ksgids = ObjHelper.GetIds(_ksgList.Select(x => x.ID).ToArray());

                        _kslpList = Reader2List.CustomSelect<D3_SL_KOEF_OMS>($"Select * from D3_SL_KOEF_OMS where D3_KSGID in ({ksgids})",
                                SprClass.LocalConnectionString);
                        _critList = Reader2List.CustomSelect<D3_CRIT_OMS>($"Select * from D3_CRIT_OMS where D3_KSGID in ({ksgids})",
            SprClass.LocalConnectionString);
                    }

                    //Услуги
                    _uslList = Reader2List.CustomSelect<D3_USL_OMS>($"Select * from D3_USL_OMS where D3_SLID in ({slids})",
        SprClass.LocalConnectionString);

                    //Диагнозы
                    _dssList = Reader2List.CustomSelect<D3_DSS_OMS>($"Select * from D3_DSS_OMS where D3_SLID in ({slids})",
                            SprClass.LocalConnectionString);

                    //Назначения
                    _nazList = Reader2List.CustomSelect<D3_NAZ_OMS>($"Select * from D3_NAZ_OMS where D3_SLID in ({slids})",
                            SprClass.LocalConnectionString);

                    //Направления
                    _naprList = Reader2List.CustomSelect<D3_NAPR_OMS>($"Select * from D3_NAPR_OMS where D3_SLID in ({slids})",
                        SprClass.LocalConnectionString);

                    //Консилиум
                    _consList = Reader2List.CustomSelect<D3_CONS_OMS>($"Select * from D3_CONS_OMS where D3_SLID in ({slids})",
                        SprClass.LocalConnectionString);

                    //Случай онко
                    _onkSlList = Reader2List.CustomSelect<D3_ONK_SL_OMS>($"Select * from D3_ONK_SL_OMS where D3_SLID in ({slids})",
                            SprClass.LocalConnectionString);

                }
                catch (Exception ex)
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        ErrorGlobalWindow.ShowError(ex.Message + Environment.NewLine + ex.InnerException?.Message);
                    });
                }
            }).ContinueWith((tz) =>

        {
            ZSlGrid.DataContext = _zsl;
            BindPacient(_zsl.D3_PID);
            SankGridControl.DataContext = _sankList;
            UslGrid.DataContext = _uslList;
            Ds2GridControl.DataContext = _dssList;
            NazGridControl.DataContext = _nazList;
            NaprGridControl.DataContext = _naprList;
            ConsGridControl.DataContext = _consList;

            if (_onkSlList?.Count > 0)
            {
                var onkid = _onkSlList[0].ID;

                //Диагностический блок
                Task.Factory.StartNew(() =>
            {
                return Reader2List.CustomSelect<D3_B_DIAG_OMS>($"Select * from D3_B_DIAG_OMS where D3_ONKSLID = {onkid}",
                    SprClass.LocalConnectionString);
            }).ContinueWith((diag) =>
            {
                _diagList = diag.Result;
                BdiagGridControl.DataContext = _diagList;
                diag.Dispose();
            }, TaskScheduler.FromCurrentSynchronizationContext());

                //Противопоказания
                Task.Factory.StartNew(() =>
            {
                return Reader2List.CustomSelect<D3_B_PROT_OMS>($"Select * from D3_B_PROT_OMS where D3_ONKSLID = {onkid}",
                    SprClass.LocalConnectionString);
            }).ContinueWith((prot) =>
            {
                _protList = prot.Result;
                BprotGridControl.DataContext = _protList;
                prot.Dispose();
            }, TaskScheduler.FromCurrentSynchronizationContext());

                //Онко услуги
                Task.Factory.StartNew(() =>
            {
                return Reader2List.CustomSelect<D3_ONK_USL_OMS>($"Select * from D3_ONK_USL_OMS where D3_ONKSLID = {onkid}",
                    SprClass.LocalConnectionString);
            }).ContinueWith((ousl) =>
            {
                _onklUslList = ousl.Result;
                OnkUslGridControl.DataContext = _onklUslList;
                ousl.Dispose();

                // Лекарства
                if (_onklUslList.Count > 0)
                {
                    var ouslids = ObjHelper.GetIds(_onklUslList.Select(x => x.ID).ToArray());
                    Task.Factory.StartNew(() =>
                    {
                        return Reader2List.CustomSelect<D3_LEK_PR_OMS>($"Select * from D3_LEK_PR_OMS where D3_ONKUSLID in ({ouslids})",
                            SprClass.LocalConnectionString);
                    }).ContinueWith((lek) =>
                    {
                        _lekList = lek.Result;
                        LekprGridControl.DataContext = _lekList;
                        lek.Dispose();
                    }, TaskScheduler.FromCurrentSynchronizationContext());

                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            }


            SlGrid.DataContext = _slList;
            NextButton.IsEnabled = true;
            PrevButton.IsEnabled = true;

        }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private Task _task;
        public D3_SCHET_OMS _sc;
        public void BindEmptySluch2(D3_SCHET_OMS sc = null)
        {
            if (_sc == null) _sc = sc;
            _zsl = new D3_ZSL_OMS();
            _zsl.D3_SCID = _sc.ID;
            _zsl.LPU = _sc.CODE_MO;
            ZSlGrid.DataContext = _zsl;

            _slList = new List<D3_SL_OMS>();

            _uslList = null;

            _naprList = null;
            _consList = null;
            _onkSlList = null;
            _onklUslList = null;
            _diagList = null;
            _protList = null;
            _lekList = null;

            _dssList = null;
            _nazList = null;
            _ksgList = null;
            _kslpList = null;
            _critList = null;

            _usl_delList = null;

            _dss_delList = null;
            _naz_delList = null;
            _kslp_delList = null;
            _crit_delList = null;

            _napr_delList = null;
            _cons_delList = null;
            _onkSl_delList = null;
            _onklUsl_delList = null;
            _diag_delList = null;
            _prot_delList = null;
            _lek_delList = null;


            _slList.Add(new D3_SL_OMS { SL_ID = Guid.NewGuid().ToString() });
            SlGrid.DataContext = _slList;



            //_task = Task.Factory.StartNew(() =>
            //{
            //    return SqlReader.Select2($"Select * from D3_ZSL_OMS where ID = 0",
            //        SprClass.LocalConnectionString);
            //}).ContinueWith((x) =>
            //{
            //    //_zsl = (DynamicBaseClass)Activator.CreateInstance(x.Result.GetDynamicType());
            //    _zsl.D3_SCID = sc.ID;
            //    SluchGrid.DataContext = _zsl;
            //    x.Dispose();
            //}, TaskScheduler.FromCurrentSynchronizationContext());

            BindEmptyPacient();
        }


        public void DoctDataFill(DynamicBaseClass data)
        {
            //_task.ContinueWith(x =>
            //{
            //    _zsl.SetValue("LPU", data.GetValue("LPU_ID"));
            //    _zsl.SetValue("USL_OK", data.GetValue("USL_OK_ID"));
            //    _zsl.SetValue("LPU_1", data.GetValue("PODR_ID"));
            //    _zsl.SetValue("PORD", data.GetValue("OTD_ID"));
            //    _zsl.SetValue("MSPID", data.GetValue("PRVS_ID"));
            //    _zsl.SetValue("PROFIL", data.GetValue("PROFIL_ID"));
            //    _zsl.SetValue("DET", Convert.ToByte(data.GetValue("DET_ID")));
            //    _zsl.SetValue("VIDPOM", data.GetValue("VID_POM_ID"));
            //    _zsl.SetValue("IDDOKT", data.GetValue("SNILS"));
            //    _zsl.SetValue("IDDOKTO", data.GetValue("DID"));
            //});
        }
        public DateTime? dvmp;
        public int? usl_ok;
        public string fap_lpu;
        public string zsl_lpu;
        void GetSpr()
        {

            // для тестирования заполнения полей Иваново, Андрей insidious


            Socstatus.DataContext = SprClass.rg001; //заполнение поля Socstatus для Иваново
            Povodobr.DataContext = SprClass.rg003; //заполнение поля Povod obr для Иваново
            ProfilkEditreg.DataContext = SprClass.rg004; //заполнение поля profil_reg для Иваново
            Vidviz.DataContext = SprClass.SprVizov; //заполнение поля vid_viz для Иваново
            Vidbrig.DataContext = SprClass.SprBrigad; //заполнение поля vid_brig для Иваново
            Grafdn.DataContext = SprClass.SprGrafdn; // заполнение поля graf_dn для Иваново


            typeUdlBox.DataContext = SprClass.passport;
            //smoOkatoBox.DataContext = SprClass.smoOkato;
            okatoTerBox.DataContext = SprClass.smoOkato;
            okatoTerPribBox.DataContext = SprClass.smoOkato;

            wBox.DataContext = SprClass.sex;
            wpBox.DataContext = SprClass.sex;

            policyTypeBox.DataContext = SprClass.policyType;
            smoBox.DataContext = SprClass.smo;
            //SocStatBox.DataContext = SprClass.SocStatsnew;
            KodTerEdit.DataContext = SprClass.KodTers;
            KatLgotBox.DataContext = SprClass.KatLgots;
            WorkStEdit.DataContext = SprClass.WorkStatDbs;
            InvEdit.DataContext = SprClass.INV;
            //DostEdit.ItemsSource = SprClass.DostList;



            DoctGrid.DataContext = SprClass.MedicalEmployeeList;
            RsltGrid.DataContext = SprClass.helpResult;
            IshodGrid.DataContext = SprClass.helpExit;
            VidPomGrid.DataContext = SprClass.typeHelp;
            LpuEdit.DataContext = SprClass.medOrg;
            UslOkzGrid.DataContext = SprClass.conditionHelp;
            ProfilGrid.DataContext = SprClass.profile;
            //ProfilkEdit.DataContext = SprClass.Profil_V020;
            ProfilColumnEdit.DataContext = SprClass.profile;
            PperEdit.DataContext = SprClass.Per;
            PrvsGrid.DataContext = SprClass.SpecV021List;
            //SluchTypeGrid.DataContext = SprClass.typeSluch;
            PCelTypeGrid.DataContext = SprClass.SprPCelList;
            DetGrid.DataContext = SprClass.SprDetProfilList;
            IdspGrid.DataContext = SprClass.tarifUsl;
            Ds1Edit.DataContext = SprClass.mkbSearching.Where(x => x.ISDELETE == false).ToList();
            //ds.DataContext = SprClass.mkbSearching.Where(x => x.ISDELETE == false).ToList();
            //ds2Box.DataContext = SprClass.mkbSearching.Where(x => x.ISDELETE == false).ToList();
            SluchOsRegionGrid.DataContext = SprClass.OsobSluchDbs;
            NaprMoGrid.DataContext = SprClass.medOrg;
            //NaprGrid.DataContext = SprClass.ExtrDbs;
            OtdelGrid.DataContext = SprClass.OtdelDbs;
            
            ForPomGrid.DataContext = SprClass.ForPomList;
            Ds1PrEdit.DataContext = SprClass.SprBit;
            VozrEdit.DataContext = SprClass.VozrList;
            VetEdit.DataContext = SprClass.VeteranDbs;
            ReabnEdit.DataContext = SprClass.SprBit;
            DnEdit.DataContext = SprClass.DnList;
            OplataEdit.DataContext = SprClass.Spr79_F005;
            VbrBox.DataContext = SprClass.SprBit;
            VbpEdit.DataContext = SprClass.SprBit;
            PrNovEdit.DataContext = SprClass.PrNov;

            DsOnkEdit.DataContext = SprClass.SprBit;
            CzabEdit.DataContext = SprClass.V027;

            UslOtdelColumnEdit.DataContext = SprClass.OtdelDbs;
            UslProfilColumnEdit.DataContext = SprClass.profile;
            UslPrvsColumnEdit.DataContext = SprClass.SpecV021List;
            UslDoctorColumnEdit.DataContext = SprClass.MedicalEmployeeList;
            UslDsColumnEdit.DataContext = SprClass.mkbSearching;
            UslVidVmeColumnEdit.DataContext = SprClass.SprUsl804;
            UslPOtkColumnEdit.DataContext = SprClass.SprBit;
            UslNplEdit.DataContext = SprClass.SprNpl;

            NaprMoColumnEdit.DataContext = SprClass.medOrg;
            NaprvColumnEdit.DataContext = SprClass.V028;
            MetIsslColumnEdit.DataContext = SprClass.V029;
            NaprUslColumnEdit.DataContext = SprClass.SprUsl804;

            PrConsColumnEdit.DataContext = SprClass.N019;

            Ds1tEdit.DataContext = SprClass.N018;
            StadEdit.DataContext = SprClass.N002;
            OnktEdit.DataContext = SprClass.N003;
            OnknEdit.DataContext = SprClass.N004;
            OnkmEdit.DataContext = SprClass.N005;
            MtstzEdit.DataContext = SprClass.SprBit;


            DiagTipColumnEdit.DataContext = SprClass.N00_DiagTip;
            DiagCodeColumnEdit.DataContext = SprClass.N007_010;
            DiagRsltColumnEdit.DataContext = SprClass.N008_011;
            RecRsltColumnEdit.DataContext = SprClass.SprBit;

            ProtColumnEdit.DataContext = SprClass.N001;

            UslTipColumnEdit.DataContext = SprClass.N013;
            HirTipColumnEdit.DataContext = SprClass.N014;
            LekTiplColumnEdit.DataContext = SprClass.N015;
            LekTipvColumnEdit.DataContext = SprClass.N016;
            LuchTipColumnEdit.DataContext = SprClass.N017;



            RegNumColumnEdit.DataContext = SprClass.N020;
            CodeShColumnEdit.DataContext = SprClass.V024;


            
            KslpCodeColumnEdit.DataContext = SprClass.KslpList;
            CritColumn.DataContext = SprClass.V024;
            RsltdBox.DataContext = SprClass.V017;
            NazrColumnEdit.DataContext = SprClass.NAZR;
            NazSpColumnEdit.DataContext = SprClass.SpecV021List;
            NazvColumnEdit.DataContext = SprClass.V029;
            NazUslColumnEdit.DataContext = SprClass.SprUsl804;
            NazNaprMoColumnEdit.DataContext = SprClass.medOrg;
            NazPmpColumnEdit.DataContext = SprClass.profile;
            NazPkColumnEdit.DataContext = SprClass.Profil_V020;

            Ds2ColumnEdit.DataContext = SprClass.mkbSearching;
            Ds2PrColumnEdit.DataContext = SprClass.SprBit;
            Ds2TypeColumnEdit.DataContext = SprClass.DsType;
            PrDs2nColumnEdit.DataContext = SprClass.DnList;
            PodrGrid.DataContext = Reader2List.CustomAnonymousSelect($@"select * from podrdb", SprClass.LocalConnectionString);
            usl_ok = _zsl.USL_OK == null ? 3 : _zsl.USL_OK;
            dvmp = _zsl.DATE_Z_2 == null ? SprClass.WorkDate : _zsl.DATE_Z_2;
            MseEdit.DataContext = SprClass.SprBit;
            HVidBox.DataContext = Reader2List.CustomAnonymousSelect($@"select * from V018 where '{dvmp}' between datebeg and isnull(dateend,'21000101') order by idhvid", SprClass.LocalConnectionString);
            HMetodBox.DataContext = Reader2List.CustomAnonymousSelect($@"select * from V019 where '{dvmp}' between datebeg and isnull(dateend,'21000101') order by idhm", SprClass.LocalConnectionString);
            fap_lpu = _slList[0].LPU_1;
            if ((fap_lpu ?? "3").Length == 8)
            {
                fap.IsChecked = true;
            }
            else
            {
                fap.IsChecked = false;
                PodrGrid.DataContext = Reader2List.CustomAnonymousSelect($@"select * from podrdb where len(id)=3", SprClass.LocalConnectionString);
            }
            zsl_lpu = _zsl.LPU == null ? "370001" : _zsl.LPU;
            
            NksgIvEdit.DataContext = Reader2List.CustomAnonymousSelect($@"select *,N_ST_STR + ' '+ naim as NameWithID from rg010 where (N_ST_STR like 'ds%' or N_ST_STR like 'st%') and kod_lpu='{zsl_lpu}' and '{dvmp}' between dt_beg and isnull(dt_fin,'20530101')", SprClass.LocalConnectionString);
                KODSPColumn.DataContext = Reader2List.CustomAnonymousSelect($@"Select distinct convert(int,KOD_SP) as KOD_SP,convert(nvarchar,KOD_SP)+' '+NSP as NameWithID from rg012 where KOD_LPU='{zsl_lpu}' and '{dvmp}' between dt_beg and isnull(dt_fin,'20530101')", SprClass.LocalConnectionString);
                UslCodeUslColumnIv.DataContext = Reader2List.CustomAnonymousSelect($@"Select KOD_LPU,convert(nvarchar,KODUSL) as KODUSL,convert(nvarchar,KODUSL)+' '+NUSL as NameWithID from rg012 where KOD_LPU='{zsl_lpu}' and '{dvmp}' between dt_beg and isnull(dt_fin,'20530101')", SprClass.LocalConnectionString);
            if (_sankList == null) return;
            if (_sankList.Count == 0) return;

            sankdate = _sankList[0].S_DATE.Value;
            
            sanknameColumnEdit.DataContext = Reader2List.CustomAnonymousSelect($@"select * from f014 where '{sankdate}' between datebeg and isnull(dateend,'21000101')", SprClass.LocalConnectionString);

            VidExpColumnEdit.DataContext = SprClass.TypeExp;
            VidExp2ColumnEdit.DataContext = SprClass.TypeExp2;
        }

        private void HVidBox_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            HMetodBox.DataContext = 
                SprClass.MetodVmpList.Where(x => x.HVID == (string)e.NewValue && (x.DATEEND>=dvmp)).ToList().OrderBy(x => x.IDHM);
        }

        private DateTime sankdate;
        private void mkbBox_GotFocus(object sender, RoutedEventArgs e)
        {
            InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("en-US");
        }

        private void mkbBox_LostFocus(object sender, RoutedEventArgs e)
        {
            InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("ru-RU");
        }

        public int? SaveSluch()
        {
            var pid = _pacient.ID;
            if (pid == 0)
            {
                _pacient.D3_SCID = _sc.ID;
                pid = Reader2List.ObjectInsertCommand("D3_PACIENT_OMS", _pacient, "ID", SprClass.LocalConnectionString);
            }
            else
            {
                var upd = Reader2List.CustomUpdateCommand("D3_PACIENT_OMS", _pacient, "ID");
                Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
            }

            _zsl.D3_PID = pid;
            _zsl.EXP_COMENT = null;
            _zsl.USERID = SprClass.userId;
            _zsl.ZSL_ID = _zsl.ZSL_ID ?? Guid.NewGuid().ToString();
            _zsl.D3_PGID = _pacient.ID_PAC;

            int? zslid = null;
            if (_zsl.ID == 0)
            {
                zslid = Reader2List.ObjectInsertCommand("D3_ZSL_OMS", _zsl, "ID", SprClass.LocalConnectionString);
                _zsl.ID = (int)zslid;
            }
            else
            {
                var upd = Reader2List.CustomUpdateCommand("D3_ZSL_OMS", _zsl, "ID");
                Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
            }

            foreach (var sl in _slList)
            {
                if (sl.ID == 0)
                {
                    sl.D3_ZSLID = _zsl.ID;
                    sl.D3_ZSLGID = _zsl.ZSL_ID;
                    sl.VERS_SPEC = "V021";
                    var slid = Reader2List.ObjectInsertCommand("D3_SL_OMS", sl, "ID", SprClass.LocalConnectionString);
                    sl.ID = (int)slid;
                }
                else
                {
                    var upd = Reader2List.CustomUpdateCommand("D3_SL_OMS", sl, "ID");
                    Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
                }
            }

            if (_zsl.USL_OK == 1 || _zsl.USL_OK == 2)
            {
                if (_ksgList != null)
                    foreach (var ksg in _ksgList)
                    {

                        if (ksg.ID == 0)
                        {
                            ksg.D3_SLID = _slList.Single(x => x.SL_ID == ksg.D3_SLGID).ID;
                            var ksgid = Reader2List.ObjectInsertCommand("D3_KSG_KPG_OMS", ksg, "ID", SprClass.LocalConnectionString);
                            ksg.ID = (int)ksgid;
                        }
                        else
                        {
                            var upd = Reader2List.CustomUpdateCommand("D3_KSG_KPG_OMS", ksg, "ID");
                            Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
                        }
                    }

                if (_kslpList != null)
                    foreach (var kslp in _kslpList)
                    {

                        if (kslp.ID == 0)
                        {
                            kslp.D3_KSGID = _ksgList.Single(x => x.KSG_ID == kslp.D3_KSGGID).ID;
                            var ksgid = Reader2List.ObjectInsertCommand("D3_SL_KOEF_OMS", kslp, "ID", SprClass.LocalConnectionString);
                            kslp.ID = (int)ksgid;
                        }
                        else
                        {
                            var upd = Reader2List.CustomUpdateCommand("D3_SL_KOEF_OMS", kslp, "ID");
                            Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
                        }
                    }

                if (_critList != null)
                    foreach (var crit in _critList)
                    {

                        if (crit.ID == 0)
                        {
                            crit.D3_KSGID = _ksgList.Single(x => x.KSG_ID == crit.D3_KSGGID).ID;
                            var crid = Reader2List.ObjectInsertCommand("D3_CRIT_OMS", crit, "ID", SprClass.LocalConnectionString);
                            crit.ID = (int)crid;
                        }
                        else
                        {
                            var upd = Reader2List.CustomUpdateCommand("D3_CRIT_OMS", crit, "ID");
                            Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
                        }
                    }
            }


            if (_uslList != null)
                foreach (var usl in _uslList)
                {

                    if (usl.ID == 0)
                    {
                        usl.D3_ZSLID = _zsl.ID;
                        usl.VERS_SPEC = "V021";
                        usl.D3_SLID = _slList.Single(x => x.SL_ID == usl.D3_SLGID).ID;
                        var uslid = Reader2List.ObjectInsertCommand("D3_USL_OMS", usl, "ID", SprClass.LocalConnectionString);
                        usl.ID = (int)uslid;
                    }
                    else
                    {
                        var upd = Reader2List.CustomUpdateCommand("D3_USL_OMS", usl, "ID");
                        Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
                    }
                }

            if (_dssList != null)
                foreach (var dss in _dssList)
                {

                    if (dss.ID == 0)
                    {
                        //napr.D3_ZSLID = _zsl.ID;
                        dss.D3_SLID = _slList.Single(x => x.SL_ID == dss.D3_SLGID).ID;
                        var id = Reader2List.ObjectInsertCommand("D3_DSS_OMS", dss, "ID", SprClass.LocalConnectionString);
                        dss.ID = id;
                    }
                    else
                    {
                        var upd = Reader2List.CustomUpdateCommand("D3_DSS_OMS", dss, "ID");
                        Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
                    }
                }

            if (_nazList != null)
                foreach (var naz in _nazList)
                {

                    if (naz.ID == 0)
                    {
                        //napr.D3_ZSLID = _zsl.ID;
                        naz.D3_SLID = _slList.Single(x => x.SL_ID == naz.D3_SLGID).ID;
                        var id = Reader2List.ObjectInsertCommand("D3_NAZ_OMS", naz, "ID", SprClass.LocalConnectionString);
                        naz.ID = id;
                    }
                    else
                    {
                        var upd = Reader2List.CustomUpdateCommand("D3_NAZ_OMS", naz, "ID");
                        Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
                    }
                }

            if (_naprList != null)
                foreach (var napr in _naprList)
                {

                    if (napr.ID == 0)
                    {
                        //napr.D3_ZSLID = _zsl.ID;
                        napr.D3_SLID = _slList.Single(x => x.SL_ID == napr.D3_SLGID).ID;
                        var id = Reader2List.ObjectInsertCommand("D3_NAPR_OMS", napr, "ID", SprClass.LocalConnectionString);
                        napr.ID = id;
                    }
                    else
                    {
                        var upd = Reader2List.CustomUpdateCommand("D3_NAPR_OMS", napr, "ID");
                        Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
                    }
                }

            if (_consList != null)
                foreach (var cons in _consList)
                {

                    if (cons.ID == 0)
                    {
                        //napr.D3_ZSLID = _zsl.ID;
                        cons.D3_SLID = _slList.Single(x => x.SL_ID == cons.D3_SLGID).ID;
                        var id = Reader2List.ObjectInsertCommand("D3_CONS_OMS", cons, "ID", SprClass.LocalConnectionString);
                        cons.ID = id;
                    }
                    else
                    {
                        var upd = Reader2List.CustomUpdateCommand("D3_CONS_OMS", cons, "ID");
                        Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
                    }
                }

            if (_onkSlList != null)
                foreach (var osl in _onkSlList)
                {

                    if (osl.ID == 0)
                    {
                        //napr.D3_ZSLID = _zsl.ID;
                        osl.D3_SLID = _slList.Single(x => x.SL_ID == osl.D3_SLGID).ID;
                        var id = Reader2List.ObjectInsertCommand("D3_ONK_SL_OMS", osl, "ID", SprClass.LocalConnectionString);
                        osl.ID = id;
                    }
                    else
                    {
                        var upd = Reader2List.CustomUpdateCommand("D3_ONK_SL_OMS", osl, "ID");
                        Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
                    }

                    if (_diagList != null)
                        foreach (var diag in _diagList)
                        {

                            if (diag.ID == 0)
                            {
                                //napr.D3_ZSLID = _zsl.ID;
                                diag.D3_ONKSLID = osl.ID;
                                var id = Reader2List.ObjectInsertCommand("D3_B_DIAG_OMS", diag, "ID", SprClass.LocalConnectionString);
                                diag.ID = id;
                            }
                            else
                            {
                                var upd = Reader2List.CustomUpdateCommand("D3_B_DIAG_OMS", diag, "ID");
                                Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
                            }
                        }
                    if (_protList != null)
                        foreach (var prot in _protList)
                        {

                            if (prot.ID == 0)
                            {
                                //napr.D3_ZSLID = _zsl.ID;
                                prot.D3_ONKSLID = osl.ID;
                                var id = Reader2List.ObjectInsertCommand("D3_B_PROT_OMS", prot, "ID", SprClass.LocalConnectionString);
                                prot.ID = id;
                            }
                            else
                            {
                                var upd = Reader2List.CustomUpdateCommand("D3_B_PROT_OMS", prot, "ID");
                                Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
                            }
                        }

                    if (_onklUslList != null)
                        foreach (var ousl in _onklUslList)
                        {

                            if (ousl.ID == 0)
                            {
                                //napr.D3_ZSLID = _zsl.ID;
                                ousl.D3_ONKSLID = osl.ID;
                                var id = Reader2List.ObjectInsertCommand("D3_ONK_USL_OMS", ousl, "ID", SprClass.LocalConnectionString);
                                ousl.ID = id;
                            }
                            else
                            {
                                var upd = Reader2List.CustomUpdateCommand("D3_ONK_USL_OMS", ousl, "ID");
                                Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
                            }


                            if (_lekList != null)
                                foreach (var lek in _lekList)
                                {

                                    if (lek.ID == 0)
                                    {
                                        //napr.D3_ZSLID = _zsl.ID;
                                        lek.D3_ONKUSLID = ousl.ID;
                                        var id = Reader2List.ObjectInsertCommand("D3_LEK_PR_OMS", lek, "ID", SprClass.LocalConnectionString);
                                        lek.ID = id;
                                    }
                                    else
                                    {
                                        var upd = Reader2List.CustomUpdateCommand("D3_LEK_PR_OMS", lek, "ID");
                                        Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
                                    }
                                }
                        }
                }

            //Удаление Назначений
            if (_naz_delList != null)
            {
                foreach (var del in _naz_delList)
                {
                    Reader2List.CustomExecuteQuery($"DELETE D3_NAZ_OMS WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                }
            }

            //Удаление Услуг
            if (_usl_delList != null)
            {
                foreach (var del in _usl_delList)
                {
                    Reader2List.CustomExecuteQuery($"DELETE D3_USL_OMS WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                }
            }

            //Удаление Диагнозов
            if (_dss_delList != null)
            {
                foreach (var del in _dss_delList)
                {
                    Reader2List.CustomExecuteQuery($"DELETE D3_DSS_OMS WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                }
            }

            //Удаление ОНКО
            if (_napr_delList != null)
            {
                foreach (var del in _napr_delList)
                {
                    Reader2List.CustomExecuteQuery($"DELETE D3_NAPR_OMS WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                }
            }

            if (_cons_delList != null)
            {
                foreach (var del in _cons_delList)
                {
                    Reader2List.CustomExecuteQuery($"DELETE D3_CONS_OMS WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                }
            }

            if (_lek_delList != null)
            {
                foreach (var del in _lek_delList)
                {
                    //D3_LEK_PR_OMS
                    Reader2List.CustomExecuteQuery($"DELETE D3_LEK_PR_OMS WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                }
            }

            if (_prot_delList != null)
            {
                foreach (var del in _prot_delList)
                {
                    //D3_B_PROT_OMS
                    Reader2List.CustomExecuteQuery($"DELETE D3_B_PROT_OMS WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                }
            }

            if (_diag_delList != null)
            {
                foreach (var del in _diag_delList)
                {
                    //D3_B_DIAG_OMS
                    Reader2List.CustomExecuteQuery($"DELETE D3_B_DIAG_OMS WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                }
            }

            if (_onklUsl_delList != null)
            {
                foreach (var del in _onklUsl_delList)
                {
                    //D3_ONK_USL_OMS
                    Reader2List.CustomExecuteQuery($"DELETE D3_ONK_USL_OMS WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                }
            }

            if (_onkSl_delList != null)
            {
                foreach (var del in _onkSl_delList)
                {
                    //D3_ONK_SL_OMS
                    Reader2List.CustomExecuteQuery($"DELETE D3_ONK_SL_OMS WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                }
            }

            //Удаление КСЛП
            if (_kslp_delList != null)
            {
                foreach (var del in _kslp_delList)
                {
                    //D3_ONK_SL_OMS
                    Reader2List.CustomExecuteQuery($"DELETE D3_SL_KOEF_OMS WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                }
            }

            //Удаление Критериев
            if (_crit_delList != null)
            {
                foreach (var del in _crit_delList)
                {
                    //D3_ONK_SL_OMS
                    Reader2List.CustomExecuteQuery($"DELETE D3_CRIT_OMS WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                }
            }

            if (SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu && (_zsl.USL_OK == 1 || _zsl.USL_OK == 2))
            {
                    Reader2List.CustomExecuteQuery($@"
                    exec[dbo].[p_oms_calc_kslp_sum] {_zsl.ID}, 0, 'zsl'", SprClass.LocalConnectionString);

                    Reader2List.CustomExecuteQuery($@"
                    exec[dbo].[p_oms_calc_ksg] {_zsl.ID}, 0, 'zsl'", SprClass.LocalConnectionString);

                BindSluch(_zsl.ID);
            }
                return zslid;
        }


        public void SaveSluchAsync()
        {
            Task.Factory.StartNew(() =>
            {
                SaveSluch();
            });
        }

        private void dxEdit_Validate(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {
            //if (sender is ComboBoxEdit)
            //{
            //    ComboBoxEdit ed = (sender as ComboBoxEdit);
            //    object item = ed.GetItemByKeyValue(e.Value);
            //    if (item == null)
            //    {
            //        e.ErrorContent = "Поле обязательно для заполнения";
            //        e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default;
            //        e.IsValid = false;
            //        e.Handled = true;
            //    }
            //}

            //if (sender is TextEdit)
            //{
            //    TextEdit ed = (sender as TextEdit);
            //    if (string.IsNullOrEmpty(ed.Text))
            //    {
            //        e.ErrorContent = "Поле обязательно для заполнения";
            //        e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default;
            //        e.IsValid = false;
            //        e.Handled = true;
            //    }
            //}

            //if (sender is DateEdit)
            //{
            //    DateEdit ed = (sender as DateEdit);
            //    if (ed.EditValue == null)
            //    {
            //        e.ErrorContent = "Поле обязательно для заполнения";
            //        e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default;
            //        e.IsValid = false;
            //        e.Handled = true;
            //    }
            //}
        }

        //PopupListBox lb;
        //private void combo_PopupOpened(object sender, RoutedEventArgs e)
        //{
        //    lb = (PopupListBox)LookUpEditHelper.GetVisualClient((ComboBoxEdit)sender).InnerEditor;
        //    ComboBoxEditItem item = (ComboBoxEditItem)lb.ItemContainerGenerator.ContainerFromIndex(0);
        //    if (item != null)
        //        item.IsSelected = true;
        //    lb.ItemContainerGenerator.ItemsChanged += new System.Windows.Controls.Primitives.ItemsChangedEventHandler(ItemContainerGenerator_ItemsChanged);
        //}
        //void ItemContainerGenerator_ItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        //{
        //    Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        ComboBoxEditItem item = (ComboBoxEditItem)lb.ItemContainerGenerator.ContainerFromIndex(0);
        //        if (item != null)
        //            item.IsSelected = true;
        //    }), System.Windows.Threading.DispatcherPriority.Render);
        //}
        private void SlAddItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            SlEditLock();

            var sl = new D3_SL_OMS { SL_ID = Guid.NewGuid().ToString() };

            _slList.Add(sl);

            SlGridControl.RefreshData();
            SlGridControl.SelectedItem = sl;

            SlEditDefault();
        }

        private void SlDelItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var del = (D3_SL_OMS)SlGridControl.SelectedItem;

            if (del.ID == 0)
            {
                _slList.Remove(del);
                //добавить удаление КСГ
            }
            else
            {
                Reader2List.CustomExecuteQuery($"DELETE D3_SL_OMS WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                Reader2List.CustomExecuteQuery($"DELETE D3_KSG_KPG_OMS WHERE D3_SLID = {del.ID}", SprClass.LocalConnectionString);

                _slList.Remove(del);
            }
            SlGridControl.RefreshData();
        }


        private void PolisEmr_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new DXWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                //Content = new PacientTest(this),
                Title = "Регистрация пациента",

                //WindowStyle = WindowStyle.None
            };
            window.ShowDialog();
        }

        private void PolisSearch_OnClick(object sender, RoutedEventArgs e)
        {
            PolisSrz();
        }

        void PolisSrz()
        {
            try
            {
                var srz = SqlReader.Select(
                    $@"SELECT [FAM],[IM],[OT],[W],[DR],[DS],[Q],[SPOL],[NPOL],[ENP],[OPDOC]  FROM [dbo].[PEOPLE] where npol = '{polisBox.EditValue}' or enp = '{polisBox.EditValue}' order by ID desc",
                    SprClass.GlobalSrzConnectionString);

                if (srz.Any())
                {
                    _pacient.FAM = (string)srz[0].GetValue("FAM");
                    _pacient.IM = (string)srz[0].GetValue("IM");
                    _pacient.OT = (string)srz[0].GetValue("OT");
                    _pacient.W = (int?)srz[0].GetValue("W");
                    _pacient.DR = (DateTime?)srz[0].GetValue("DR");
                    _pacient.SMO = (string)srz[0].GetValue("Q");
                    _pacient.VPOLIS = (int?)srz[0].GetValue("OPDOC");
                    _pacient.NPOLIS =
                        (int?)srz[0].GetValue("OPDOC") == 3
                            ? (string)srz[0].GetValue("ENP")
                            : (string)srz[0].GetValue("NPOL");
                }
                else
                {
                    PacientEmr();
                }
            }
            catch (Exception ex)
            {
                PacientEmr();
            }
        }

        //    void PolisSearch()
        //    {
        //        if (polisBox.EditValue != null && ((string) polisBox.EditValue).Length > 0)
        //        {
        //            var row =
        //SqlReader.Select2(
        //    $"Select * from D3_PACIENT_OMS where NPOLIS = '{polisBox.EditValue}' order by ID desc",
        //    SprClass.LocalConnectionString);
        //            if (row.Count > 0)
        //            {
        //                var pid = (int)ObjHelper.GetAnonymousValue(row[0], "ID");
        //                var fam = (string)ObjHelper.GetAnonymousValue(row[0], "FAM");
        //                var im = (string)ObjHelper.GetAnonymousValue(row[0], "IM");
        //                var ot = (string)ObjHelper.GetAnonymousValue(row[0], "OT");
        //                var dr = (DateTime)ObjHelper.GetAnonymousValue(row[0], "DR");

        //                _zsl.D3_PID = pid;
        //                PacientGroup.Header = $"Пациент: {fam} {im} {ot} {dr:yyyy}";
        //            }
        //            else
        //            {
        //                PacientEmr();
        //            }
        //        }
        //        else
        //        {
        //            PacientEmr();
        //        }
        //    }

        void PacientEmr()
        {
            var pac = SqlReader.Select(
                $@"SELECT top 1 * FROM [dbo].[D3_PACIENT_OMS] where NPOLIS = '{polisBox.EditValue}' order by ID desc",
                SprClass.LocalConnectionString);


            if (pac.Any())
            {
                //_pacient = ObjHelper.ClassConverter<D3_PACIENT_OMS>(pac[0]);
                //_pacient.ID_PAC = Guid.NewGuid().ToString();
                //_pacient.ID = 0;

                _pacient.FAM = (string)pac[0].GetValue("FAM");
                _pacient.IM = (string)pac[0].GetValue("IM");
                _pacient.OT = (string)pac[0].GetValue("OT");
                _pacient.W = (int?)pac[0].GetValue("W");
                _pacient.DR = (DateTime?)pac[0].GetValue("DR");
                _pacient.SMO = (string)pac[0].GetValue("SMO");
                _pacient.VPOLIS = (int?)pac[0].GetValue("VPOLIS");
                _pacient.NPOLIS = (string)pac[0].GetValue("NPOLIS");
            }
            else
            {
                Console.WriteLine("Пациент не найден");
            }
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            SaveSluch();
        }

        private void PolisBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                PolisSrz();

        }

        private void UslAddItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (_uslList == null)
            {
                UslGrid.DataContext = _uslList = new List<D3_USL_OMS>();
            }

            var usl = new D3_USL_OMS { LPU = _zsl.LPU, D3_SLGID = ((D3_SL_OMS)SlGridControl.SelectedItem).SL_ID };
            _uslList.Add(usl);

            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                SizeToContent = SizeToContent.Height,
                Content = new UslTemplateD3(usl)
            };
            window.ShowDialog();
            UslGridControl.RefreshData();
        }

        private void UslEditItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var usl = (D3_USL_OMS)UslGridControl.SelectedItem;
            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                SizeToContent = SizeToContent.Height,
                Content = new UslTemplateD3(usl)
            };
            window.ShowDialog();
            UslGridControl.RefreshData();
        }

        private void UslDelItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var del = (D3_USL_OMS)UslGridControl.SelectedItem;

            if (del.ID == 0)
            {
                _uslList.Remove(del);
            }
            else
            {
                if (_usl_delList == null) _usl_delList = new List<D3_USL_OMS>();
                _usl_delList.Add(del);
                _uslList.Remove(del);
            }

            UslGridControl.RefreshData();
        }

        private void SlGridControl_OnSelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            UslGridControl.FilterString = $"([D3_SLGID] = '{((D3_SL_OMS)SlGridControl.SelectedItem)?.SL_ID}')";
            NaprGridControl.FilterString = $"([D3_SLGID] = '{((D3_SL_OMS)SlGridControl.SelectedItem)?.SL_ID}')";
            ConsGridControl.FilterString = $"([D3_SLGID] = '{((D3_SL_OMS)SlGridControl.SelectedItem)?.SL_ID}')";
            //OnkSlGridControl.FilterString = $"([D3_SLGID] = '{((D3_SL_OMS)SlGridControl.SelectedItem)?.SL_ID}')";
            NazGridControl.FilterString = $"([D3_SLGID] = '{((D3_SL_OMS)SlGridControl.SelectedItem)?.SL_ID}')";
            Ds2GridControl.FilterString = $"([D3_SLGID] = '{((D3_SL_OMS)SlGridControl.SelectedItem)?.SL_ID}')";
            //Ds2GridControl.FilterString = $"([D3_SLGID] = '{((D3_SL_OMS)SlGridControl.SelectedItem)?.SL_ID}')";

            KsgGroup.DataContext = _ksgList?.SingleOrDefault(x => x.D3_SLGID == ((D3_SL_OMS)SlGridControl.SelectedItem)?.SL_ID);
            KslpGridControl.DataContext = _kslpList?.Where(x => x.D3_KSGGID == ((D3_KSG_KPG_OMS)KsgGroup.DataContext)?.KSG_ID);
            CritGridControl.DataContext = _critList?.Where(x => x.D3_KSGGID == ((D3_KSG_KPG_OMS)KsgGroup.DataContext)?.KSG_ID);

            OnkSlGroup.DataContext = _onkSlList?.SingleOrDefault(x => x.D3_SLGID == ((D3_SL_OMS)SlGridControl.SelectedItem)?.SL_ID);
        }

        private void NewZSluch_OnClick(object sender, RoutedEventArgs e)
        {
            NewZsl();
        }

        void NewZsl()
        {
            ZSlEditLock();
            SlEditLock();
            
            BindEmptySluch2();
            //polisBox.EditValue = null;
            //PacientGroup.Header = "Пациент:";

            ZSlEditDefault();
            SlEditDefault();
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                new Action(delegate ()
                {
                    polisBox.Focus();
                }));
        }

        private void UslTemplateItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Width = 800,
                Height = 600,
                //SizeToContent = SizeToContent.Height,
                Content = new UslUserTempl(this)
            };
            window.ShowDialog();
            UslGridControl.RefreshData();
        }

        private D3_ZSL_OMS _zslLock;
        private D3_SL_OMS _slLock;

        void ZSlEditDefault()
        {
            _zsl.USL_OK = _zslLock.USL_OK;
            _zsl.VIDPOM = _zslLock.VIDPOM;
            _zsl.FOR_POM = _zslLock.FOR_POM;
            _zsl.NPR_MO = _zslLock.NPR_MO;
            _zsl.IDSP = _zslLock.IDSP;
            _zsl.OS_SLUCH_REGION = _zslLock.OS_SLUCH_REGION;
            _zsl.VOZR = _zslLock.VOZR;
            _zsl.DATE_Z_1 = _zslLock.DATE_Z_1;
            _zsl.DATE_Z_2 = _zslLock.DATE_Z_2;
            _zsl.RSLT = _zslLock.RSLT;
            _zsl.ISHOD = _zslLock.ISHOD;
            _zsl.VB_P = _zslLock.VB_P;
            _zsl.NPR_DATE = _zslLock.NPR_DATE;
            _zsl.RSLT_D = _zslLock.RSLT_D;
            _zsl.VBR = _zslLock.VBR;
        }
        public bool fapcheck;
        void SlEditDefault()
        {
            fap.IsChecked = fapcheck;
            ((D3_SL_OMS)SlGridControl.SelectedItem).USL_OK = _slLock.USL_OK;
            ((D3_SL_OMS)SlGridControl.SelectedItem).LPU_1 = _slLock.LPU_1;
            ((D3_SL_OMS)SlGridControl.SelectedItem).PODR = _slLock.PODR;
            ((D3_SL_OMS)SlGridControl.SelectedItem).IDDOKT = _slLock.IDDOKT;
            ((D3_SL_OMS)SlGridControl.SelectedItem).PRVS = _slLock.PRVS;
            ((D3_SL_OMS)SlGridControl.SelectedItem).PROFIL = _slLock.PROFIL;
            ((D3_SL_OMS)SlGridControl.SelectedItem).DET = _slLock.DET;
            ((D3_SL_OMS)SlGridControl.SelectedItem).P_CEL25 = _slLock.P_CEL25;
            ((D3_SL_OMS)SlGridControl.SelectedItem).DS1 = _slLock.DS1;
            ((D3_SL_OMS)SlGridControl.SelectedItem).DATE_1 = _slLock.DATE_1;
            ((D3_SL_OMS)SlGridControl.SelectedItem).DATE_2 = _slLock.DATE_2;

            ((D3_SL_OMS)SlGridControl.SelectedItem).C_ZAB = _slLock.C_ZAB;
            ((D3_SL_OMS)SlGridControl.SelectedItem).DN = _slLock.DN;
            ((D3_SL_OMS)SlGridControl.SelectedItem).REAB = _slLock.REAB;
            ((D3_SL_OMS)SlGridControl.SelectedItem).DS_ONK = _slLock.DS_ONK;
            ((D3_SL_OMS)SlGridControl.SelectedItem).DS1_PR = _slLock.DS1_PR;
            ((D3_SL_OMS)SlGridControl.SelectedItem).POVOD = _slLock.POVOD;
            ((D3_SL_OMS)SlGridControl.SelectedItem).P_PER = _slLock.P_PER;
            ((D3_SL_OMS)SlGridControl.SelectedItem).PROFIL_REG = _slLock.PROFIL_REG;
        }

        void ZSlEditLock()
        {
            if (_zslLock == null)
                _zslLock = new D3_ZSL_OMS();

            _zslLock.USL_OK = UslOkzTb.IsChecked == true ? _zsl.USL_OK : null;
            _zslLock.VIDPOM = VidPomTb.IsChecked == true ? _zsl.VIDPOM : null;
            _zslLock.FOR_POM = ForPomTb.IsChecked == true ? _zsl.FOR_POM : null;
            _zslLock.NPR_MO = NaprMoTb.IsChecked == true ? _zsl.NPR_MO : null;
            _zslLock.IDSP = IdspTb.IsChecked == true ? _zsl.IDSP : null;
            _zslLock.OS_SLUCH_REGION = SluchOsRegionTb.IsChecked == true ? _zsl.OS_SLUCH_REGION : null;
            _zslLock.VOZR = VozrTb.IsChecked == true ? _zsl.VOZR : null;
            _zslLock.DATE_Z_1 = DateZ1Tb.IsChecked == true ? _zsl.DATE_Z_1 : null;
            _zslLock.DATE_Z_2 = DateZ2Tb.IsChecked == true ? _zsl.DATE_Z_2 : null;
            _zslLock.RSLT = RsltTb.IsChecked == true ? _zsl.RSLT : null;
            _zslLock.ISHOD = IshodTb.IsChecked == true ? _zsl.ISHOD : null;
            _zslLock.NPR_DATE = NaprDateTb.IsChecked == true ? _zsl.NPR_DATE : null;
            _zslLock.VB_P = VbpTb.IsChecked == true ? _zsl.VB_P : null;
            _zslLock.VBR = MobTb.IsChecked == true ? _zsl.VBR : null;
            _zslLock.RSLT_D = RsTb.IsChecked == true ? _zsl.RSLT_D : null;
            
        }

        void SlEditLock()
        {
            if (_slLock == null)
                _slLock = new D3_SL_OMS();

            _slLock.LPU_1 = PodrTb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).LPU_1 : null;
            _slLock.PODR = OtdelTb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).PODR : null;
            _slLock.IDDOKT = DoctTb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).IDDOKT : null;
            _slLock.PRVS = PrvsTb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).PRVS : null;
            _slLock.PROFIL = ProfilTb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).PROFIL : null;
            _slLock.DET = DetTb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).DET : null;
            _slLock.P_CEL25 = PCelTypeTb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).P_CEL25 : null;
            _slLock.DS1 = Ds1Tb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).DS1 : null;
            _slLock.DATE_1 = Date1Tb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).DATE_1 : null;
            _slLock.DATE_2 = Date2Tb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).DATE_2 : null;
            _slLock.C_ZAB = CzabTb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).C_ZAB : null;
            _slLock.DN = DnTb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).DN : null;
            _slLock.REAB = ReabnTb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).REAB : null;
            _slLock.DS_ONK = DsOnkTb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).DS_ONK : null;
            _slLock.DS1_PR = Ds1PrTb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).DS1_PR : null;
            _slLock.POVOD = povodobrTb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).POVOD : null;
            _slLock.P_PER = PostTb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).P_PER : null;
            //_slLock.PROFIL_K = ProfKTb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).PROFIL_K : null;
            _slLock.PROFIL_REG = ProfKrTb.IsChecked == true ? ((D3_SL_OMS)SlGridControl.SelectedItem).PROFIL_REG : null;

            if (fapTb.IsChecked == true && fap.IsChecked == true)
            {
                fapcheck = true;
            }
            else if (fapTb.IsChecked == true && fap.IsChecked == false)
            {
                fapcheck = false;
            }
        }

        private void EmrPacient_OnClick(object sender, RoutedEventArgs e)
        {
            //var emrPacient = new EmrPacientControl();
            //emrPacient.BindPacient(_zsl.D3_PID);

            //var window = new DXWindow
            //{
            //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
            //    Content = emrPacient,
            //    Title = "Карат пациента",
            //    SizeToContent = SizeToContent.WidthAndHeight

            //    //WindowStyle = WindowStyle.None
            //};
            //window.ShowDialog();
            if (_zsl.D3_PID == 0)
            {
                //DXMessageBox.Show("Пациент еще не выбран");
                //return;

                var window0 = new DXWindow
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    //Content = new EmrFullControl(this),
                    Title = "Регистрация пациента",
                };
                window0.ShowDialog();

            }
            else
            {
                var window = new DXWindow
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    //Content = new EmrFullControl(this, (int)_zsl.D3_PID),
                    Title = "Регистрация пациента",
                };
                window.ShowDialog();

            }

        }

        private void PrvsEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {

        }

        private void Required_OnValidate(object sender, ValidationEventArgs e)
        {
            if (e.Value != null) return;

            e.IsValid = false;
            e.ErrorContent = "Обязательное поле для заполнения";
            e.ErrorType = ErrorType.Critical;
        }

        private void Information_OnValidate(object sender, ValidationEventArgs e)
        {
            if (e.Value != null) return;

            e.IsValid = false;
            e.ErrorContent = "Статистическое поле, заполняется для получения отчетности";
            e.ErrorType = ErrorType.Information;

        }

        private void Conditional_OnValidate(object sender, ValidationEventArgs e)
        {
            if (e.Value != null) return;

            if (((BaseEdit)sender).Name == "OtBox")
            {
                e.IsValid = false;
                e.ErrorContent = "Условное поле, при отсутствии ставится признак достоверности";
                e.ErrorType = ErrorType.Warning;
            }
            else
            {
                e.IsValid = false;
                e.ErrorContent = "Условное поле, заполняется при необходимости";
                e.ErrorType = ErrorType.Information;
            }
        }

        private void SluchTemplateD31Ivanovo_OnInitialized(object sender, EventArgs e)
        {
        }

        private void NaprAddItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (_naprList == null)
            {
                NaprGridControl.DataContext = _naprList = new List<D3_NAPR_OMS>();
            }

            var napr = new D3_NAPR_OMS() { D3_SLGID = ((D3_SL_OMS)SlGridControl.SelectedItem).SL_ID };
            _naprList.Add(napr);

            NaprGridControl.RefreshData();
        }

        private void ConsAddItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_consList == null)
            {
                ConsGridControl.DataContext = _consList = new List<D3_CONS_OMS>();
            }

            var cons = new D3_CONS_OMS() { D3_SLGID = ((D3_SL_OMS)SlGridControl.SelectedItem).SL_ID };
            _consList.Add(cons);

            ConsGridControl.RefreshData();

        }

        private void OnkSlAddItem_Click(object sender, ItemClickEventArgs e)
        {
            if (_onkSlList == null)
            {
                _onkSlList = new List<D3_ONK_SL_OMS>();
            }

            if (_onkSlList.Where(x => x.D3_SLGID == ((D3_SL_OMS)SlGridControl.SelectedItem).SL_ID).Count() > 0)
            {
                DXMessageBox.Show("Не может быть более одной записи случая лечения онкологического заболевания ");
                return;
            }

            var onkSl = new D3_ONK_SL_OMS() { D3_SLGID = ((D3_SL_OMS)SlGridControl.SelectedItem).SL_ID };
            _onkSlList.Add(onkSl);

            OnkSlGroup.DataContext = onkSl;
        }

        private void BdiagAddItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!_onkSlList.Any())
            {
                DXMessageBox.Show("Нет записи случая лечения онкологического заболевания");
                return;
            }

            if (_diagList == null)
            {
                BdiagGridControl.DataContext = _diagList = new List<D3_B_DIAG_OMS>();
            }

            var diag = new D3_B_DIAG_OMS() { D3_ONKSLGID = ((D3_SL_OMS)SlGridControl.SelectedItem).SL_ID };
            _diagList.Add(diag);

            BdiagGridControl.RefreshData();
        }

        private void BprotAddItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!_onkSlList.Any())
            {
                DXMessageBox.Show("Нет записи случая лечения онкологического заболевания");
                return;
            }

            if (_protList == null)
            {
                BprotGridControl.DataContext = _protList = new List<D3_B_PROT_OMS>();
            }

            var prot = new D3_B_PROT_OMS() { D3_ONKSLGID = ((D3_SL_OMS)SlGridControl.SelectedItem).SL_ID };
            _protList.Add(prot);

            BdiagGridControl.RefreshData();

        }

        private void OnkUslAddItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!_onkSlList.Any())
            {
                DXMessageBox.Show("Нет записи случая лечения онкологического заболевания");
                return;
            }

            if (_onklUslList == null)
            {
                OnkUslGridControl.DataContext = _onklUslList = new List<D3_ONK_USL_OMS>();
            }

            var ousl = new D3_ONK_USL_OMS() { D3_ONKSLGID = ((D3_SL_OMS)SlGridControl.SelectedItem).SL_ID };
            _onklUslList.Add(ousl);

            OnkUslGridControl.RefreshData();

        }

        private void LekprAddItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!_onklUslList.Any())
            {
                DXMessageBox.Show("Нет записи услуги при лечении онкологического заболевания");
                return;
            }

            if (_lekList == null)
            {
                LekprGridControl.DataContext = _lekList = new List<D3_LEK_PR_OMS>();
            }

            var lek = new D3_LEK_PR_OMS() { D3_ONKUSLGID = ((D3_ONK_USL_OMS)OnkUslGridControl.SelectedItem).D3_ONKSLGID };
            _lekList.Add(lek);

            LekprGridControl.RefreshData();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(GetSpr), System.Windows.Threading.DispatcherPriority.Render);
        }

        private void NaprDelItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            var del = (D3_NAPR_OMS)NaprGridControl.SelectedItem;

            if (del.ID == 0)
            {
                _naprList.Remove(del);
            }
            else
            {
                if (_napr_delList == null) _napr_delList = new List<D3_NAPR_OMS>();
                _napr_delList.Add(del);
                _naprList.Remove(del);
            }

            NaprGridControl.RefreshData();
        }

        private void ConsDelItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            var del = (D3_CONS_OMS)ConsGridControl.SelectedItem;

            if (del.ID == 0)
            {
                _consList.Remove(del);
            }
            else
            {
                if (_cons_delList == null) _cons_delList = new List<D3_CONS_OMS>();

                _cons_delList.Add(del);
                _consList.Remove(del);
            }

            ConsGridControl.RefreshData();
        }

        private void OnkSlDel_Click(object sender, ItemClickEventArgs e)
        {
            //var del = (D3_ONK_SL_OMS)OnkSlGridControl.SelectedItem;
            var del = _onkSlList?.SingleOrDefault(x => x.D3_SLGID == ((D3_SL_OMS)SlGridControl.SelectedItem)?.SL_ID);

            if (del.ID == 0)
            {
                _onkSlList.Remove(del);
            }
            else
            {
                if (_onkSl_delList == null) _onkSl_delList = new List<D3_ONK_SL_OMS>();

                _onkSl_delList.Add(del);
                _onkSlList.Remove(del);
            }

            OnkSlGroup.DataContext = null;
        }

        private void BdiagDelItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            var del = (D3_B_DIAG_OMS)BdiagGridControl.SelectedItem;

            if (del.ID == 0)
            {
                _diagList.Remove(del);
            }
            else
            {
                if (_diag_delList == null) _diag_delList = new List<D3_B_DIAG_OMS>();

                _diag_delList.Add(del);
                _diagList.Remove(del);
            }

            BdiagGridControl.RefreshData();
        }

        private void BprotDelItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            var del = (D3_B_PROT_OMS)BprotGridControl.SelectedItem;

            if (del.ID == 0)
            {
                _protList.Remove(del);
            }
            else
            {
                if (_prot_delList == null) _prot_delList = new List<D3_B_PROT_OMS>();

                _prot_delList.Add(del);
                _protList.Remove(del);
            }

            BprotGridControl.RefreshData();
        }

        private void OnkUslDelItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            var del = (D3_ONK_USL_OMS)OnkUslGridControl.SelectedItem;

            if (del.ID == 0)
            {
                _onklUslList.Remove(del);
            }
            else
            {
                if (_onklUsl_delList == null) _onklUsl_delList = new List<D3_ONK_USL_OMS>();

                _onklUsl_delList.Add(del);
                _onklUslList.Remove(del);
            }

            OnkUslGridControl.RefreshData();
        }

        private void LekprDelItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            var del = (D3_LEK_PR_OMS)LekprGridControl.SelectedItem;

            if (del.ID == 0)
            {
                _lekList.Remove(del);
            }
            else
            {
                if (_lek_delList == null) _lek_delList = new List<D3_LEK_PR_OMS>();

                _lek_delList.Add(del);
                _lekList.Remove(del);
            }

            LekprGridControl.RefreshData();
        }

        private void PrevButton_OnClick(object sender, RoutedEventArgs e)
        {
            NextButton.IsEnabled = false;
            PrevButton.IsEnabled = false;

            GetZslRowId(--rowIndex);
            GetSpr();
        }

        private void NextButton_OnClick(object sender, RoutedEventArgs e)
        {
            NextButton.IsEnabled = false;
            PrevButton.IsEnabled = false;

            GetZslRowId(++rowIndex);
            GetSpr();
        }

        private object _row;
        private void GetZslRowId(int index)
        {
            var row = _gc.GetRow(index);
            if (row == null) return;

            if (row is NotLoadedObject)
            {
                _gc.GetRowAsync(index).ContinueWith((x) =>
                {
                    _row = ((ReadonlyThreadSafeProxyForObjectFromAnotherThread)x.Result).OriginalRow;
                    BindSluch((int)ObjHelper.GetAnonymousValue(_row, "ID"));
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                _row = ((ReadonlyThreadSafeProxyForObjectFromAnotherThread)row).OriginalRow;
                BindSluch((int)ObjHelper.GetAnonymousValue(_row, "ID"));
            }
        }


        private void SankAddItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var sank = new D3_SANK_OMS
            {
                S_TIP = 1,
                S_TIP2 = 1,
                S_DATE = SprClass.WorkDate,
                S_SUM = _zsl.SUMV,
                D3_ZSLID = _zsl.ID,
                D3_SCID = _zsl.D3_SCID,
                S_CODE = Guid.NewGuid().ToString(),
                D3_ZSLGID = _zsl.ZSL_ID,
                USER_ID = SprClass.userId
            };
            var sumv = _zsl.SUMV;
            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                SizeToContent = SizeToContent.Height,
                Width = 500,
                Content = new SankControl(sank,sumv)
            };
            window.ShowDialog();
            if (sank.ID != 0)
                _sankList.Add(sank);

            SankGridControl.RefreshData();
            ZslUpdate();
            GetSpr();
        }

        private void SankEditItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var sank = (D3_SANK_OMS)SankGridControl.SelectedItem;
            
            if (sank.S_TIP == 1)
            {
                var sumv = _zsl.SUMV;
                if (sank.S_OSN != "5.3.2.")
                {
                    sank.S_SUM = sumv;
                }
                var window = new DXWindow
                {
                    ShowIcon = false,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    SizeToContent = SizeToContent.Height,
                    Width = 500,
                    Content = new SankControl(sank,sumv)
                };
                window.ShowDialog();
                SankGridControl.RefreshData();
                ZslUpdate();
            }
            else
            {
                var st = sank.S_TIP ?? (sank.S_TIP2 >= 20 && sank.S_TIP2 < 30 ? 2 : 3);
                var re = sank.S_TIP == null ? 1 : 0;
                if (sank.USER_ID.ToString() != "" && sank.USER_ID.ToString() != SprClass.userId.ToString() && sank.S_TIP2 != 1)
                {
                    DXMessageBox.Show("Вы не можете редактировать эту санкцию, она проведена другим пользователем");
                    return;
                }
                var row = _gc.GetRow(rowIndex);
                if (row == null) return;

                if (row is NotLoadedObject)
                {
                    _gc.GetRowAsync(rowIndex).ContinueWith((x) =>
                    {
                        _row = ((ReadonlyThreadSafeProxyForObjectFromAnotherThread)x.Result).OriginalRow;
                        var window = new DXWindow
                        {
                            ShowIcon = false,
                            WindowStartupLocation = WindowStartupLocation.CenterScreen,
                            Content = new MedicExpControl(st, sank.ID, _row, re),
                            Title = "Акт МЭЭ",
                            SizeToContent = SizeToContent.Height,
                            Width = 1450
                        };
                        window.ShowDialog();
                        SankGridControl.RefreshData();
                        ZslUpdate();
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                else
                {
                    _row = ((ReadonlyThreadSafeProxyForObjectFromAnotherThread)row).OriginalRow;
                    var window = new DXWindow
                    {
                        ShowIcon = false,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        Content = new MedicExpControl(st, sank.ID, _row, re),
                        Title = "Акт МЭЭ",
                        SizeToContent = SizeToContent.Height,
                        Width = 1450
                    };
                    window.ShowDialog();
                    SankGridControl.RefreshData();
                    ZslUpdate();
                    
                }
            }
            GetSpr();
            //BindSluch((int)ObjHelper.GetAnonymousValue(_row, "ID"));
        }

        private void SankDelItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            
            var result = DXMessageBox.Show("Удалить санкцию?", "Удаление", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
            var s = (D3_SANK_OMS)SankGridControl.SelectedItem;
            if (s.USER_ID.ToString() != "" && s.USER_ID.ToString() != SprClass.userId.ToString() && s.S_TIP2 != 1)
            {
                DXMessageBox.Show("Вы не можете удалить эту санкцию, она проведена другим пользователем");
                return;
            }
            Task.Factory.StartNew(() =>
            {
                Reader2List.CustomExecuteQuery($"DELETE D3_SANK_OMS WHERE ID = {s.ID}", SprClass.LocalConnectionString);
                _sankList.Remove(s);

                Reader2List.CustomExecuteQuery($@"
EXEC p_oms_calc_sank_ {_zsl.D3_SCID}
EXEC p_oms_calc_schet {_zsl.D3_SCID}
", SprClass.LocalConnectionString);

            }).ContinueWith(x =>
            {
                ZslUpdate();
                SankGridControl.RefreshData();

            }, TaskScheduler.FromCurrentSynchronizationContext());
            GetSpr();
        }

        public void ZslUpdate()
        {
            Task.Factory.StartNew(() =>
            {
                return Reader2List.CustomSelect<D3_ZSL_OMS>($"Select * from D3_ZSL_OMS where ID = {_zsl.ID}",
                    SprClass.LocalConnectionString)[0];
            }).ContinueWith((x) =>
            {
                _zsl = x.Result;
                ZSlGrid.DataContext = _zsl;

                x.Dispose();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void LayoutControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
                SaveSluchAsync();
            if (e.Key == Key.F7)
                NewZsl();
        }

        private void GridViewBase_OnCellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu)
            {
                if (Equals(e.Column, KslpCodeColumn) && e.Value != null)
                {
                    var val = ((IEnumerable<dynamic>)SprClass.KslpList).SingleOrDefault(
                        x => (int)ObjHelper.GetAnonymousValue(x, "IDSL") == (int?)e.Value);
                    KslpGridControl.SetCellValue(e.RowHandle, "Z_SL",
        ObjHelper.GetAnonymousValue(val, "ZKOEF"));

                }
            }

        }

        private void GridViewBase_OnCellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            (sender as TableView).PostEditor();
        }

        private void KslpAddItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_kslpList == null)
            {
                KslpGridControl.DataContext = _kslpList = new List<D3_SL_KOEF_OMS>();
            }

            if (_ksgList == null)
            {
                _ksgList = new List<D3_KSG_KPG_OMS>();
            }

            if (KsgGroup.DataContext == null)
            {
                var ksg = new D3_KSG_KPG_OMS
                {
                    KSG_ID = Guid.NewGuid().ToString(),
                    D3_SLGID = ((D3_SL_OMS)SlGridControl.SelectedItem).SL_ID
                };
                _ksgList.Add(ksg);
                KsgGroup.DataContext = ksg;
            }

            var kslp = new D3_SL_KOEF_OMS() { D3_KSGGID = ((D3_KSG_KPG_OMS)KsgGroup.DataContext).KSG_ID };
            _kslpList.Add(kslp);

            KslpGridControl.RefreshData();

        }

        private void KslpDelItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            var del = (D3_SL_KOEF_OMS)KslpGridControl.SelectedItem;

            if (del.ID == 0)
            {
                _kslpList.Remove(del);
            }
            else
            {
                if (_kslp_delList == null) _kslp_delList = new List<D3_SL_KOEF_OMS>();
                _kslp_delList.Add(del);
                _kslpList.Remove(del);
            }

            KslpGridControl.RefreshData();
        }

        private void OnkUslGridControl_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            //LekprGridControl.FilterString = $"([D3_ONKUSLGID] = '{((D3_ONK_USL_OMS)OnkUslGridControl.SelectedItem)?.ONKUSL_ID}')";

        }

        private void NaznAddItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_nazList == null)
            {
                NazGridControl.DataContext = _nazList = new List<D3_NAZ_OMS>();
            }

            var naz = new D3_NAZ_OMS() { D3_SLGID = ((D3_SL_OMS)SlGridControl.SelectedItem).SL_ID };
            _nazList.Add(naz);

            NazGridControl.RefreshData();
        }

        private void NaznDelItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            var del = (D3_NAZ_OMS)NazGridControl.SelectedItem;

            if (del.ID == 0)
            {
                _nazList.Remove(del);
            }
            else
            {
                if (_naz_delList == null) _naz_delList = new List<D3_NAZ_OMS>();
                _naz_delList.Add(del);
                _nazList.Remove(del);
            }

            NazGridControl.RefreshData();
        }

        private void Ds2AddItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_dssList == null)
            {
                Ds2GridControl.DataContext = _dssList = new List<D3_DSS_OMS>();
            }

            var dss = new D3_DSS_OMS() { D3_SLGID = ((D3_SL_OMS)SlGridControl.SelectedItem).SL_ID };
            _dssList.Add(dss);

            Ds2GridControl.RefreshData();

        }

        private void Ds2DelItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            var del = (D3_DSS_OMS)Ds2GridControl.SelectedItem;

            if (del.ID == 0)
            {
                _dssList.Remove(del);
            }
            else
            {
                if (_dss_delList == null) _dss_delList = new List<D3_DSS_OMS>();
                _dss_delList.Add(del);
                _dssList.Remove(del);
            }

            Ds2GridControl.RefreshData();
        }
        private void UslAutoTemplateItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_zsl.DATE_Z_2 == null || _pacient.DR == null || _pacient.W == null || _zsl.OS_SLUCH_REGION == null)
            {
                DXMessageBox.Show("Не заполнены поля для определения стандарта");
                return;
            }
            //var vozr = _zsl.DATE_Z_2?.Year - _pacient.DR?.Year;
            string v = "";
            string s;
            string sg = "0";
            var pol = _pacient.W;
            var os = _zsl.OS_SLUCH_REGION;
            var lpu = _zsl.LPU;
            var datez2 = _zsl.DATE_Z_2;
            var slgid = ((D3_SL_OMS)SlGridControl.SelectedItem).SL_ID;
            if (os == 47 || os == 49 && (_zsl.DATE_Z_2?.Year - _pacient.DR?.Year) >= 18)
            {
                v = (_zsl.DATE_Z_2?.Year - _pacient.DR?.Year).ToString();
            }
            else if (os == 11)
            {
                var mm = Math.Floor((_zsl.DATE_Z_1 - _pacient.DR).Value.Days / 365.25 * 12);
                var mg = Math.Floor((_zsl.DATE_Z_1 - _pacient.DR).Value.Days / 365.25);
                var ms = "";
                if (mg == 0)
                {

                    if (mm % 12 < 10)
                    {
                        s = "0";
                        ms = (mm % 12).ToString();
                    }
                    else
                    {
                        s = "";
                        ms = mm.ToString();
                    }
                }
                else if (mg > 0 && mg < 2)
                {
                    if (mm % 12 < 3)
                    {
                        s = "00";
                    }
                    else if (mm % 12 > 2 && mm % 12 < 6)
                    {
                        s = "03";
                    }
                    else
                    {
                        s = "06";
                    }
                }
                else
                {
                    mg = (double)(_zsl.DATE_Z_1?.Year - _pacient.DR?.Year);
                    s = "00";
                    if (mg > 9)
                    {
                        sg = "";
                    }
                }
                v = "G" + sg + mg + "." + "M" + s + ms;

            }
            else
            {
                return;
            }

            Task.Factory.StartNew(() =>
            {
               
                var autoTempl = SqlReader.Select2($"Select * From Kursk_Usl_124N where '{datez2}' between Dbeg and Dend and OsSluchReg = {os} and Pol = {pol} and Age like '%{v},%'", SprClass.LocalConnectionString);

                if (autoTempl.Count == 0)
                {
                    return null;
                }

                if (_uslList == null)
                {
                    _uslList = new List<D3_USL_OMS>();
                }

                foreach (DynamicBaseClass usl in autoTempl)
                {
                    _uslList.Add(new D3_USL_OMS
                    {
                        COMENTU = (string)usl.GetValue("UsL_Name"),
                        CODE_USL = (string)usl.GetValue("Code_Usl"),
                        VID_VME = (string)usl.GetValue("Code_Usl"),
                        KOL_USL = 1, //(decimal?)usl.GetValue("Kol"),
                        TARIF = 0, //(decimal?)usl.GetValue("Tarif"),
                        PROFIL = (int?)usl.GetValue("Prof"),
                        DET = 0, //(int?)usl.GetValue("Det"),
                        PRVS = (int?)usl.GetValue("Spec"),
                        PRVS_VERS = "V021_"+ (int?)usl.GetValue("Spec"),
                        LPU = lpu,
                        D3_SLGID = slgid
                    });
                }
 //               	[ID_List_Group] [int] NULL,
	//[OsSluchReg] [int] NULL,
	//[Pol] [int] NULL,
	//[Age] [nvarchar](255) NULL,
	//[Code_Usl] [nvarchar](255) NULL,
	//[UsL_Name] [nvarchar](255) NULL,
	//[Obyaz] [int] NULL,
	//[Spec] [int] NULL,
	//[Prof] [int] NULL
                return _uslList;
            }).ContinueWith(x =>
            {
                UslGrid.DataContext = x.Result;
                UslGridControl.RefreshData();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnkSlGroup_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (OnkSlGroup.DataContext == null) OnkSlGroup.IsEnabled = false;
            else OnkSlGroup.IsEnabled = true;

        }

        private void CritAddItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_critList == null)
            {
                CritGridControl.DataContext = _critList = new List<D3_CRIT_OMS>();
            }

            if (_ksgList == null)
            {
                _ksgList = new List<D3_KSG_KPG_OMS>();
            }

            if (KsgGroup.DataContext == null)
            {

                var ksg = new D3_KSG_KPG_OMS
                {
                    KSG_ID = Guid.NewGuid().ToString(),
                    D3_SLGID = ((D3_SL_OMS)SlGridControl.SelectedItem).SL_ID
                };
                _ksgList.Add(ksg);
                KsgGroup.DataContext = ksg;

            }

            var crit = new D3_CRIT_OMS() { D3_KSGGID = ((D3_KSG_KPG_OMS)KsgGroup.DataContext).KSG_ID };
            _critList.Add(crit);

            CritGridControl.RefreshData();
        }

        private void CritDelItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            var del = (D3_CRIT_OMS)CritGridControl.SelectedItem;

            if (del.ID == 0)
            {
                _critList.Remove(del);
            }
            else
            {
                if (_crit_delList == null) _crit_delList = new List<D3_CRIT_OMS>();
                _crit_delList.Add(del);
                _critList.Remove(del);
            }

            CritGridControl.RefreshData();
        }

        private void DateZ2Edit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            dvmp = _zsl.DATE_Z_2 == null ? SprClass.WorkDate : _zsl.DATE_Z_2;       
        }

        private void UslOkzEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            usl_ok = (int?)UslOkzEdit.EditValue;
        }

        private void Fap_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (fap.IsChecked == false)
            {
                PodrGrid.DataContext = Reader2List.CustomAnonymousSelect($@"select * from podrdb where len(id)=3", SprClass.LocalConnectionString);
            }
            else
            {
                PodrGrid.DataContext = Reader2List.CustomAnonymousSelect($@"select * from podrdb where left(id,6)='{_zsl.LPU}'", SprClass.LocalConnectionString);
            } 
        }


    }

    public class RoleVisibility1 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var localWeb1 =
                (IList)Reader2List.CustomAnonymousSelect("Select Parametr from Settings where name ='REGION' and Parametr like '%37%'",
                    SprClass.LocalConnectionString);
            if ((string)value == "LPU" && SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu 
                 ||
                 (string)value == "OMS" && (SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms || SprClass.ProdSett.OrgTypeStatus == OrgType.Smo) 
                 ||
                 (string)value == "SMO" && (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo) 
                 ||
                 (string)value == "37" && (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms || SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu) && localWeb1.Count > 0 //если регион 37 Иваново то показываем
                 ||
                 (string)value == "all" && (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms || SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu) && localWeb1.Count == 0 //если регион не 37, а др. регион СМО,тфомс,lpu показываем
                 ||
                 (string)value == "TFOMS" && (SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms) ) return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



}
