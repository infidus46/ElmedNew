using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ServerMode;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.XtraPrinting.Native;
using Yamed.Control;
using Yamed.Core;
using Yamed.Entity;
using Yamed.OmsExp.ExpEditors;
using Yamed.Server;

namespace Yamed.Oms
{
    /// <summary>
    /// Логика взаимодействия для ElReestr.xaml
    /// </summary>
    public partial class SchetRegisterGrid : UserControl
    {
        public readonly LinqInstantFeedbackDataSource _linqInstantFeedbackDataSource;
        private readonly YamedDataClassesDataContext _ElmedDataClassesDataContext;
        //public List<SQLTables.SluPacClass> _zsls;
        public List<int> Scids;
        public List<int> zslid;
        public SchetRegisterGrid()
        {
            InitializeComponent();

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            using (var dc = new YamedDataClassesDataContext(SprClass.LocalConnectionString))
            {
                Yamed_Users first = dc.Yamed_Users.Single(x => x.ID == SprClass.userId);
                writer.Write(first.LayRTable);
            }
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);


            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                new Action(delegate ()
                {
                    if (stream.Length > 0)
                        gridControl1.RestoreLayoutFromStream(stream);

                    stream.Close();
                    writer.Close();

                    gridControl1.FilterString = "";
                    gridControl1.ClearSorting();
                }));

           
            ZUslOkEdit.DataContext = SprClass.conditionHelp;
            SmoEdit.DataContext = SprClass.smo;
            VidPomEdit.DataContext = SprClass.typeHelp;
            ForPomEdit.DataContext = SprClass.ForPomList;
            MoEdit.DataContext = SprClass.LpuList;
            NprMoEdit.DataContext = SprClass.LpuList;
            Mo_AttEdit.DataContext = SprClass.LpuList;
            RsltEdit.DataContext = SprClass.helpResult;
            RsltdEdit.DataContext = SprClass.V017;
            IshodEdit.DataContext = SprClass.helpExit;
            OsSlRegEdit.DataContext = SprClass.OsobSluchDbs;
            IdspEdit.DataContext = SprClass.tarifUsl;
            VozrEdit.DataContext = SprClass.VozrList;
            VbpEdit.DataContext = SprClass.SprBit;
            KodTEdit.DataContext = SprClass.KodTers;
            KatLgotEdit.DataContext = SprClass.KatLgots;
            WorkStEdit.DataContext = SprClass.WorkStatDbs;
            VetEdit.DataContext = SprClass.VeteranDbs;
            VbrEdit.DataContext = SprClass.SprBit;
            INVEdit.DataContext = SprClass.INV;

            ProfilEdit.DataContext = SprClass.profile;
            VidHmpEdit.DataContext = SprClass.VidVmpList;
            MetodHmpEdit.DataContext = SprClass.MetodVmpList;
            KsgEdit.DataContext = Reader2List.CustomAnonymousSelect($@"select * from V023", SprClass.LocalConnectionString);
            Ds1Edit.DataContext = SprClass.mkbSearching;
            Ds0Edit.DataContext = SprClass.mkbSearching;
            PrvsEdit.DataContext = SprClass.SpecV021List;
            OplataEdit.DataContext = SprClass.Spr79_F005;
            ExpTypeEdit.DataContext = SprClass.MeeTypeDbs;
            UserEdit.DataContext = SprClass.YamedUsers;
            PCelEdit.DataContext = SprClass.SprPCelList;
            DoctEdit.DataContext = SprClass.MedicalEmployeeList;

            Lpu1Edit.DataContext = SprClass.Podr;
            PodrEdit.DataContext = SprClass.OtdelDbs;

            //заполнение полей для Иваново, Андрей insidious
            SOCSTATUS.DataContext = SprClass.rg001;
            KODSP.DataContext = Reader2List.CustomAnonymousSelect($@"Select distinct KOD_LPU,convert(int,rg.KOD_SP) as KOD_SP,convert(nvarchar,rg.KOD_SP)+' '+NSP as NameWithID from rg012 rg left join d3_usl_oms usl on usl.KOD_SP=rg.KOD_SP where KOD_LPU=usl.LPU", SprClass.LocalConnectionString);
            POVOD.DataContext = SprClass.rg003;
            ProfilkEditReg.DataContext = SprClass.rg004;
            GRAF_DN.DataContext = SprClass.SprGrafdn;
            VID_VIZ.DataContext = SprClass.SprVizov;
            VID_BRIG.DataContext = SprClass.SprBrigad;

            SocStatBox.DataContext = SprClass.SocStatsnew;
            ProfilkEdit.DataContext = SprClass.Profil_V020;
            PperEdit.DataContext = SprClass.Per;
            CzabEdit.DataContext = SprClass.V027;
            DnEdit.DataContext = SprClass.DnList;
            ReabEdit.DataContext = SprClass.SprBit;
            DsOnkEdit.DataContext = SprClass.SprBit;

            //if (SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu)
            //    (forLPU).Visibility = Visibility.Visible;

            _ElmedDataClassesDataContext = new YamedDataClassesDataContext()
            {
                ObjectTrackingEnabled = false,
                CommandTimeout = 0,
                Connection = { ConnectionString = SprClass.LocalConnectionString }
            };

            _linqInstantFeedbackDataSource = new LinqInstantFeedbackDataSource()
            {
                AreSourceRowsThreadSafe = false, KeyExpression = "KeyID"
            };
            gridControl1.DataContext = _linqInstantFeedbackDataSource;

        }


        private IQueryable _pQueryable;
        public void BindDataZsl()
        {
            HideSlColumn();
            if (zslid != null)
            {
                _pQueryable = from zsl in _ElmedDataClassesDataContext.D3_ZSL_OMS
                              join pa in _ElmedDataClassesDataContext.D3_PACIENT_OMS on zsl.D3_PID equals pa.ID
                              join sc in _ElmedDataClassesDataContext.D3_SCHET_OMS on zsl.D3_SCID equals sc.ID
                              join sprsc in _ElmedDataClassesDataContext.Yamed_Spr_SchetType on sc.SchetType equals sprsc.ID
                              //join sl in _ElmedDataClassesDataContext.D3_SL_OMS on zsl.ID equals sl.D3_ZSLID
                              where (Scids.Contains(zsl.D3_SCID) || !Scids.Any()) && (zslid.Contains(zsl.ID) || !zslid.Any())
                              select new
                              {
                                  sc.YEAR,
                                  sc.MONTH,
                                  sc.NSCHET,
                                  sc.DSCHET,
                                  SchetType = sprsc.NameWithID,
                                  sc.OmsFileName,

                                  zsl.PR_NOV,
                                  KeyID = zsl.ID,
                                  zsl.D3_SCID,
                                  zsl.ID,
                                  zsl.ZSL_ID,
                                  IDCASE = (Int64?)zsl.IDCASE,
                                  zsl.VIDPOM,
                                  zsl.NPR_MO,
                                  zsl.LPU,
                                  zsl.FOR_POM,
                                  zsl.DATE_Z_1,
                                  zsl.DATE_Z_2,
                                  zsl.RSLT,
                                  zsl.ISHOD,
                                  zsl.OS_SLUCH,
                                  zsl.OS_SLUCH_REGION,
                                  zsl.IDSP,
                                  zsl.SUMV,
                                  zsl.OPLATA,
                                  zsl.SUMP,
                                  zsl.SANK_IT,
                                  zsl.MEK_COMENT,
                                  zsl.OSP_COMENT,
                                  zsl.USL_OK,
                                  Z_P_CEL = zsl.P_CEL,
                                  zsl.MEK_COUNT,
                                  zsl.MEE_COUNT,
                                  zsl.EKMP_COUNT,
                                  zsl.EXP_COMENT,
                                  zsl.EXP_TYPE,
                                  zsl.EXP_DATE,
                                  zsl.ReqID,
                                  zsl.USER_COMENT,
                                  zsl.USERID,
                                  zsl.NPR_DATE,
                                  zsl.KD_Z,
                                  zsl.VB_P,
                                  zsl.RSLT_D,
                                  zsl.VBR,

                                  pa.FAM,
                                  pa.IM,
                                  pa.OT,
                                  pa.MO_ATT,
                                  pa.W,
                                  pa.DR,
                                  pa.FAM_P,
                                  pa.IM_P,
                                  pa.OT_P,
                                  pa.W_P,
                                  pa.DR_P,
                                  pa.MR,
                                  pa.DOCTYPE,
                                  pa.DOCSER,
                                  pa.DOCNUM,
                                  pa.SNILS,
                                  pa.OKATOG,
                                  pa.OKATOP,
                                  pa.COMENTP,
                                  pa.VPOLIS,
                                  pa.SPOLIS,
                                  pa.NPOLIS,
                                  pa.SMO,
                                  pa.SMO_OGRN,
                                  pa.SMO_OK,
                                  pa.SMO_NAM,
                                  pa.NOVOR,
                                  pa.SOC_STAT,
                                  pa.KOD_TER,
                                  pa.KAT_LGOT,
                                  pa.MSE,
                                  pa.INV,
                                  pa.VETERAN,
                                  pa.WORK_STAT,

                                  zsl.VOZR,
                                  pa.AdressP, // поле Адрес регистрации добавил Андрей insidiuos

                                  //для отображения в Иваново
                                  pa.SOCSTATUS
                              };

                _linqInstantFeedbackDataSource.QueryableSource = _pQueryable;
            }
            else
            {
                _pQueryable = from zsl in _ElmedDataClassesDataContext.D3_ZSL_OMS
                              join pa in _ElmedDataClassesDataContext.D3_PACIENT_OMS on zsl.D3_PID equals pa.ID
                              join sc in _ElmedDataClassesDataContext.D3_SCHET_OMS on zsl.D3_SCID equals sc.ID
                              join sprsc in _ElmedDataClassesDataContext.Yamed_Spr_SchetType on sc.SchetType equals sprsc.ID
                              //join sl in _ElmedDataClassesDataContext.D3_SL_OMS on zsl.ID equals sl.D3_ZSLID
                              where (Scids.Contains(zsl.D3_SCID) || !Scids.Any())
                              select new
                              {
                                  sc.YEAR,
                                  sc.MONTH,
                                  sc.NSCHET,
                                  sc.DSCHET,
                                  SchetType = sprsc.NameWithID,
                                  sc.OmsFileName,

                                  zsl.PR_NOV,
                                  KeyID = zsl.ID,
                                  zsl.D3_SCID,
                                  zsl.ID,
                                  zsl.ZSL_ID,
                                  IDCASE = (Int64?)zsl.IDCASE,
                                  zsl.VIDPOM,
                                  zsl.NPR_MO,
                                  zsl.LPU,
                                  zsl.FOR_POM,
                                  zsl.DATE_Z_1,
                                  zsl.DATE_Z_2,
                                  zsl.RSLT,
                                  zsl.ISHOD,
                                  zsl.OS_SLUCH,
                                  zsl.OS_SLUCH_REGION,
                                  zsl.IDSP,
                                  zsl.SUMV,
                                  zsl.OPLATA,
                                  zsl.SUMP,
                                  zsl.SANK_IT,
                                  zsl.MEK_COMENT,
                                  zsl.OSP_COMENT,
                                  zsl.USL_OK,
                                  Z_P_CEL = zsl.P_CEL,
                                  zsl.MEK_COUNT,
                                  zsl.MEE_COUNT,
                                  zsl.EKMP_COUNT,
                                  zsl.EXP_COMENT,
                                  zsl.EXP_TYPE,
                                  zsl.EXP_DATE,
                                  zsl.ReqID,
                                  zsl.USER_COMENT,
                                  zsl.USERID,
                                  zsl.NPR_DATE,
                                  zsl.KD_Z,
                                  zsl.VB_P,
                                  zsl.RSLT_D,
                                  zsl.VBR,

                                  pa.FAM,
                                  pa.IM,
                                  pa.OT,
                                  pa.MO_ATT,
                                  pa.W,
                                  pa.DR,
                                  pa.FAM_P,
                                  pa.IM_P,
                                  pa.OT_P,
                                  pa.W_P,
                                  pa.DR_P,
                                  pa.MR,
                                  pa.DOCTYPE,
                                  pa.DOCSER,
                                  pa.DOCNUM,
                                  pa.SNILS,
                                  pa.OKATOG,
                                  pa.OKATOP,
                                  pa.COMENTP,
                                  pa.VPOLIS,
                                  pa.SPOLIS,
                                  pa.NPOLIS,
                                  pa.SMO,
                                  pa.SMO_OGRN,
                                  pa.SMO_OK,
                                  pa.SMO_NAM,
                                  pa.NOVOR,
                                  pa.SOC_STAT,
                                  pa.KOD_TER,
                                  pa.KAT_LGOT,
                                  pa.MSE,
                                  pa.INV,
                                  pa.VETERAN,
                                  pa.WORK_STAT,

                                  zsl.VOZR,
                                  pa.AdressP, // поле Адрес регистрации добавил Андрей insidiuos

                                  //для отображения в Иваново
                                  pa.SOCSTATUS
                              };

                _linqInstantFeedbackDataSource.QueryableSource = _pQueryable;
            }
        }

        void ShowSlColumn()
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                new Action(delegate()
                {
                    gridControl1.Columns.Where(x => x.Name.StartsWith("Column__SL__")).ToList().ForEach(x =>
                    {
                        if (x.Tag != null)
                            x.Width = (GridColumnWidth)x.Tag;
                    });
                }));
        }

        void HideSlColumn()
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                new Action(delegate ()
                {
                    gridControl1.Columns.Where(x => x.Name.StartsWith("Column__SL__")).ToList().ForEach(x =>
                    {
                        x.Tag = x.Width;
                        x.Width = 0;
                    });
                }));
        }


        public void BindDataSl()
        {
             ShowSlColumn();
            if (zslid != null)
            {
                _pQueryable = from zsl in _ElmedDataClassesDataContext.D3_ZSL_OMS
                              join pa in _ElmedDataClassesDataContext.D3_PACIENT_OMS on zsl.D3_PID equals pa.ID
                              join sc in _ElmedDataClassesDataContext.D3_SCHET_OMS on zsl.D3_SCID equals sc.ID
                              join sl in _ElmedDataClassesDataContext.D3_SL_OMS on zsl.ID equals sl.D3_ZSLID
                              join sprsc in _ElmedDataClassesDataContext.Yamed_Spr_SchetType on sc.SchetType equals sprsc.ID
                              join lksg in _ElmedDataClassesDataContext.D3_KSG_KPG_OMS on sl.ID equals lksg.D3_SLID into tmpksg
                              from ksg in tmpksg.DefaultIfEmpty()
                              where (Scids.Contains(zsl.D3_SCID) || !Scids.Any()) && (zslid.Contains(zsl.ID) || !zslid.Any())
                              select new
                              {
                                  sc.YEAR,
                                  sc.MONTH,
                                  sc.NSCHET,
                                  sc.DSCHET,
                                  zsl.PR_NOV,
                                  SchetType = sprsc.NameWithID,
                                  sc.OmsFileName,

                                  KeyID = sl.ID,
                                  zsl.D3_SCID,
                                  zsl.ID,
                                  zsl.ZSL_ID,
                                  IDCASE = (Int64?)zsl.IDCASE,
                                  zsl.VIDPOM,
                                  zsl.NPR_MO,
                                  zsl.LPU,
                                  zsl.FOR_POM,
                                  zsl.DATE_Z_1,
                                  zsl.DATE_Z_2,
                                  zsl.RSLT,
                                  zsl.ISHOD,
                                  zsl.OS_SLUCH,
                                  zsl.OS_SLUCH_REGION,
                                  zsl.IDSP,
                                  zsl.SUMV,
                                  zsl.OPLATA,
                                  zsl.SUMP,
                                  zsl.SANK_IT,
                                  zsl.MEK_COMENT,
                                  zsl.OSP_COMENT,
                                  zsl.USL_OK,
                                  zsl.MEK_COUNT,
                                  zsl.MEE_COUNT,
                                  zsl.EKMP_COUNT,
                                  zsl.EXP_COMENT,
                                  zsl.EXP_TYPE,
                                  zsl.EXP_DATE,
                                  zsl.ReqID,
                                  zsl.USER_COMENT,
                                  zsl.USERID,
                                  Z_P_CEL = zsl.P_CEL,
                                  zsl.NPR_DATE,
                                  zsl.KD_Z,
                                  zsl.VB_P,
                                  zsl.RSLT_D,
                                  zsl.VBR,
                                  ///////////////////////////////////
                                  sl.VID_HMP,
                                  sl.METOD_HMP,
                                  sl.LPU_1,
                                  sl.PODR,
                                  sl.PROFIL,
                                  sl.DET,
                                  sl.P_CEL25,
                                  sl.TAL_NUM,
                                  sl.TAL_D,
                                  sl.TAL_P,
                                  sl.NHISTORY,
                                  sl.P_PER,
                                  sl.DATE_1,
                                  sl.DATE_2,
                                  sl.KD,
                                  sl.DS0,
                                  sl.DS1,
                                  sl.DS1_PR,
                                  sl.DN,
                                  sl.CODE_MES1,
                                  sl.CODE_MES2,
                                  sl.KSG_DKK,

                                  //для отображения полей Иваново, Андрей insidious
                                  sl.GRAF_DN,
                                  sl.KSKP,
                                  sl.VID_BRIG,
                                  sl.VID_VIZ,
                                  sl.POVOD,
                                  sl.PROFIL_REG,
                                  pa.SOCSTATUS,



                                  //sl.N_KSG,
                                  //sl.KSG_PG,
                                  //sl.SL_K,
                                  //sl.IT_SL,
                                  ksg.N_KSG,
                                  ksg.KSG_PG,
                                  ksg.SL_K,
                                  ksg.IT_SL,

                                  sl.REAB,
                                  sl.PRVS,
                                  sl.VERS_SPEC,
                                  sl.PRVS_VERS,
                                  sl.IDDOKT,
                                  sl.ED_COL,
                                  sl.TARIF,
                                  sl.SUM_M,
                                  sl.COMENTSL,
                                  sl.PROFIL_K,
                                  sl.C_ZAB,
                                  sl.DS_ONK,
                                  ////////////////////////////////

                                  pa.FAM,
                                  pa.IM,
                                  pa.OT,
                                  pa.W,
                                  pa.DR,
                                  pa.FAM_P,
                                  pa.IM_P,
                                  pa.OT_P,
                                  pa.W_P,
                                  pa.DR_P,
                                  pa.MR,
                                  pa.DOCTYPE,
                                  pa.DOCSER,
                                  pa.DOCNUM,
                                  pa.SNILS,
                                  pa.OKATOG,
                                  pa.OKATOP,
                                  pa.COMENTP,
                                  pa.VPOLIS,
                                  pa.SPOLIS,
                                  pa.NPOLIS,
                                  pa.SMO,
                                  pa.SMO_OGRN,
                                  pa.SMO_OK,
                                  pa.SMO_NAM,
                                  pa.NOVOR,
                                  zsl.VOZR,
                                  pa.SOC_STAT,
                                  pa.KOD_TER,
                                  pa.KAT_LGOT,
                                  pa.MSE,
                                  pa.INV,
                                  pa.VETERAN,
                                  pa.WORK_STAT,
                                  pa.AdressP, // поле Адрес регистрации добавил Андрей insidiuos
                              };
                _linqInstantFeedbackDataSource.QueryableSource = _pQueryable;
            }
            else
            {
                _pQueryable = from zsl in _ElmedDataClassesDataContext.D3_ZSL_OMS
                              join pa in _ElmedDataClassesDataContext.D3_PACIENT_OMS on zsl.D3_PID equals pa.ID
                              join sc in _ElmedDataClassesDataContext.D3_SCHET_OMS on zsl.D3_SCID equals sc.ID
                              join sl in _ElmedDataClassesDataContext.D3_SL_OMS on zsl.ID equals sl.D3_ZSLID
                              join sprsc in _ElmedDataClassesDataContext.Yamed_Spr_SchetType on sc.SchetType equals sprsc.ID
                              join lksg in _ElmedDataClassesDataContext.D3_KSG_KPG_OMS on sl.ID equals lksg.D3_SLID into tmpksg
                              from ksg in tmpksg.DefaultIfEmpty()
                              where (Scids.Contains(zsl.D3_SCID) || !Scids.Any())
                              select new
                              {
                                  sc.YEAR,
                                  sc.MONTH,
                                  sc.NSCHET,
                                  sc.DSCHET,
                                  zsl.PR_NOV,
                                  SchetType = sprsc.NameWithID,
                                  sc.OmsFileName,

                                  KeyID = sl.ID,
                                  zsl.D3_SCID,
                                  zsl.ID,
                                  zsl.ZSL_ID,
                                  IDCASE = (Int64?)zsl.IDCASE,
                                  zsl.VIDPOM,
                                  zsl.NPR_MO,
                                  zsl.LPU,
                                  zsl.FOR_POM,
                                  zsl.DATE_Z_1,
                                  zsl.DATE_Z_2,
                                  zsl.RSLT,
                                  zsl.ISHOD,
                                  zsl.OS_SLUCH,
                                  zsl.OS_SLUCH_REGION,
                                  zsl.IDSP,
                                  zsl.SUMV,
                                  zsl.OPLATA,
                                  zsl.SUMP,
                                  zsl.SANK_IT,
                                  zsl.MEK_COMENT,
                                  zsl.OSP_COMENT,
                                  zsl.USL_OK,
                                  zsl.MEK_COUNT,
                                  zsl.MEE_COUNT,
                                  zsl.EKMP_COUNT,
                                  zsl.EXP_COMENT,
                                  zsl.EXP_TYPE,
                                  zsl.EXP_DATE,
                                  zsl.ReqID,
                                  zsl.USER_COMENT,
                                  zsl.USERID,
                                  Z_P_CEL = zsl.P_CEL,
                                  zsl.NPR_DATE,
                                  zsl.KD_Z,
                                  zsl.VB_P,
                                  zsl.RSLT_D,
                                  zsl.VBR,
                                  ///////////////////////////////////
                                  sl.VID_HMP,
                                  sl.METOD_HMP,
                                  sl.LPU_1,
                                  sl.PODR,
                                  sl.PROFIL,
                                  sl.DET,
                                  sl.P_CEL25,
                                  sl.TAL_NUM,
                                  sl.TAL_D,
                                  sl.TAL_P,
                                  sl.NHISTORY,
                                  sl.P_PER,
                                  sl.DATE_1,
                                  sl.DATE_2,
                                  sl.KD,
                                  sl.DS0,
                                  sl.DS1,
                                  sl.DS1_PR,
                                  sl.DN,
                                  sl.CODE_MES1,
                                  sl.CODE_MES2,
                                  sl.KSG_DKK,

                                  //для отображения полей Иваново, Андрей insidious
                                  sl.GRAF_DN,
                                  sl.KSKP,
                                  sl.VID_BRIG,
                                  sl.VID_VIZ,
                                  sl.POVOD,
                                  sl.PROFIL_REG,
                                  pa.SOCSTATUS,



                                  //sl.N_KSG,
                                  //sl.KSG_PG,
                                  //sl.SL_K,
                                  //sl.IT_SL,
                                  ksg.N_KSG,
                                  ksg.KSG_PG,
                                  ksg.SL_K,
                                  ksg.IT_SL,

                                  sl.REAB,
                                  sl.PRVS,
                                  sl.VERS_SPEC,
                                  sl.PRVS_VERS,
                                  sl.IDDOKT,
                                  sl.ED_COL,
                                  sl.TARIF,
                                  sl.SUM_M,
                                  sl.COMENTSL,
                                  sl.PROFIL_K,
                                  sl.C_ZAB,
                                  sl.DS_ONK,
                                  ////////////////////////////////

                                  pa.FAM,
                                  pa.IM,
                                  pa.OT,
                                  pa.W,
                                  pa.DR,
                                  pa.FAM_P,
                                  pa.IM_P,
                                  pa.OT_P,
                                  pa.W_P,
                                  pa.DR_P,
                                  pa.MR,
                                  pa.DOCTYPE,
                                  pa.DOCSER,
                                  pa.DOCNUM,
                                  pa.SNILS,
                                  pa.OKATOG,
                                  pa.OKATOP,
                                  pa.COMENTP,
                                  pa.VPOLIS,
                                  pa.SPOLIS,
                                  pa.NPOLIS,
                                  pa.SMO,
                                  pa.SMO_OGRN,
                                  pa.SMO_OK,
                                  pa.SMO_NAM,
                                  pa.NOVOR,
                                  zsl.VOZR,
                                  pa.SOC_STAT,
                                  pa.KOD_TER,
                                  pa.KAT_LGOT,
                                  pa.MSE,
                                  pa.INV,
                                  pa.VETERAN,
                                  pa.WORK_STAT,
                                  pa.AdressP, // поле Адрес регистрации добавил Андрей insidiuos
                              };
                _linqInstantFeedbackDataSource.QueryableSource = _pQueryable;
            }
        }

        public void BindDataPacient(string fam, string im, string ot, DateTime? dr, string npol = null)
        {
            //ShowSlColumn();
            SlCheckEdit.IsEnabled = false;

            _pQueryable = from zsl in _ElmedDataClassesDataContext.D3_ZSL_OMS
                    join pa in _ElmedDataClassesDataContext.D3_PACIENT_OMS on zsl.D3_PID equals pa.ID
                    join sc in _ElmedDataClassesDataContext.D3_SCHET_OMS on zsl.D3_SCID equals sc.ID
                    join sl in _ElmedDataClassesDataContext.D3_SL_OMS on zsl.ID equals sl.D3_ZSLID
                          join sprsc in _ElmedDataClassesDataContext.Yamed_Spr_SchetType on sc.SchetType equals sprsc.ID
                          where (pa.FAM.StartsWith(fam) || fam == null) && (pa.IM.StartsWith(im) || im == null) && (pa.OT.StartsWith(ot) || ot == null) && (pa.DR == dr || dr ==null) && (pa.NPOLIS == npol || npol == null)
                          select new
                    {
                        sc.YEAR,
                        sc.MONTH,
                              sc.NSCHET,
                              sc.DSCHET,
                              SchetType = sprsc.NameWithID,
                              sc.OmsFileName,
                              zsl.PR_NOV,
                              KeyID = sl.ID,

                        zsl.D3_SCID,
                        zsl.ID,
                        zsl.ZSL_ID,
                              IDCASE = (Int64?)zsl.IDCASE,
                              zsl.VIDPOM,
                        zsl.NPR_MO,
                        zsl.LPU,
                        zsl.FOR_POM,
                        zsl.DATE_Z_1,
                        zsl.DATE_Z_2,
                        zsl.RSLT,
                        zsl.ISHOD,
                        zsl.OS_SLUCH,
                        zsl.OS_SLUCH_REGION,
                        zsl.IDSP,
                        zsl.SUMV,
                        zsl.OPLATA,
                        zsl.SUMP,
                        zsl.SANK_IT,
                        zsl.MEK_COMENT,
                        zsl.OSP_COMENT,
                        zsl.USL_OK,
                        //zsl.P_CEL,
                        zsl.MEK_COUNT,
                        zsl.MEE_COUNT,
                        zsl.EKMP_COUNT,
                        zsl.EXP_COMENT,
                        zsl.EXP_TYPE,
                        zsl.EXP_DATE,
                        zsl.ReqID,
                        zsl.USER_COMENT,
                        zsl.USERID,
                        Z_P_CEL = zsl.P_CEL,
                              zsl.NPR_DATE,
                              zsl.KD_Z,
                              zsl.VB_P,
                              zsl.RSLT_D,
                              zsl.VBR,

                              sl.VID_HMP,
                        sl.METOD_HMP,
                        sl.LPU_1,
                        sl.PODR,
                        sl.PROFIL,
                        sl.DET,
                        sl.P_CEL25,
                        sl.TAL_NUM,
                        sl.TAL_D,
                        sl.TAL_P,
                        sl.NHISTORY,
                        sl.P_PER,
                        sl.DATE_1,
                        sl.DATE_2,
                        sl.KD,
                        sl.DS0,
                        sl.DS1,
                        sl.DS1_PR,
                        sl.DN,
                        sl.CODE_MES1,
                        sl.CODE_MES2,
                        sl.KSG_DKK,
                        sl.N_KSG,
                        sl.KSG_PG,
                        sl.SL_K,
                        sl.IT_SL,
                        sl.REAB,
                        sl.PRVS,
                        sl.VERS_SPEC,
                        sl.PRVS_VERS,
                        sl.IDDOKT,
                        sl.ED_COL,
                        sl.TARIF,
                        sl.SUM_M,
                        sl.COMENTSL,
                              sl.PROFIL_K,
                              sl.C_ZAB,
                              sl.DS_ONK,

                              //для отображения полей Иваново, Андрей insidious
                              sl.GRAF_DN,
                              sl.KSKP,
                              sl.VID_BRIG,
                              sl.VID_VIZ,
                              sl.POVOD,
                              sl.PROFIL_REG,
                              pa.SOCSTATUS,


                              ////////////////////////////////
                              pa.FAM,
                        pa.IM,
                        pa.OT,
                        pa.W,
                        pa.DR,
                        pa.FAM_P,
                        pa.IM_P,
                        pa.OT_P,
                        pa.W_P,
                        pa.DR_P,
                        pa.MR,
                        pa.DOCTYPE,
                        pa.DOCSER,
                        pa.DOCNUM,
                        pa.SNILS,
                        pa.OKATOG,
                        pa.OKATOP,
                        pa.COMENTP,
                        pa.VPOLIS,
                        pa.SPOLIS,
                        pa.NPOLIS,
                        pa.SMO,
                        pa.SMO_OGRN,
                        pa.SMO_OK,
                        pa.SMO_NAM,
                        pa.NOVOR,
                        zsl.VOZR,
                              pa.SOC_STAT,
                              pa.KOD_TER,
                              pa.KAT_LGOT,
                              pa.MSE,
                              pa.INV,
                              pa.VETERAN,
                              pa.WORK_STAT,
                              pa.AdressP, // поле Адрес регистрации добавил Андрей insidiuos
                          };
            _linqInstantFeedbackDataSource.QueryableSource = _pQueryable;

        }

        public void BindDataExpResult(List<int> ids)
        {
            var ui = Guid.NewGuid();
            var qd = DateTime.Now;
            var qlist = new List<D3_EXP_QUERY>();
            foreach(var id in ids)
            {
                D3_EXP_QUERY eq = new D3_EXP_QUERY();
                eq.D3_ZSLID = id;
                eq.QUID = ui;
                eq.QDATE = qd;
                eq.USERID = SprClass.userId;
                qlist.Add(eq);
                
            }
          
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(SprClass.LocalConnectionString, SqlBulkCopyOptions.CheckConstraints))
            {
                bulkCopy.BulkCopyTimeout = 0;
                bulkCopy.DestinationTableName = "dbo.D3_EXP_QUERY";
                bulkCopy.WriteToServer(qlist.AsDataReader());
            }
            qlist.Clear();
            zslid = ids;
            //ids.Clear();
            
            //ShowSlColumn();
            SlCheckEdit.IsEnabled = false;
            
            _pQueryable = from exq in _ElmedDataClassesDataContext.D3_EXP_QUERY
                          join zsl in _ElmedDataClassesDataContext.D3_ZSL_OMS on exq.D3_ZSLID equals zsl.ID
                          join pa in _ElmedDataClassesDataContext.D3_PACIENT_OMS on zsl.D3_PID equals pa.ID
                          join sc in _ElmedDataClassesDataContext.D3_SCHET_OMS on zsl.D3_SCID equals sc.ID
                          join sl in _ElmedDataClassesDataContext.D3_SL_OMS on zsl.ID equals sl.D3_ZSLID
                          join sprsc in _ElmedDataClassesDataContext.Yamed_Spr_SchetType on sc.SchetType equals sprsc.ID
                          where exq.QUID == ui
                          select new
                          {
                              sc.YEAR,
                              sc.MONTH,
                              sc.NSCHET,
                              sc.DSCHET,
                              SchetType = sprsc.NameWithID,
                              sc.OmsFileName,
                              zsl.PR_NOV,

                              KeyID = sl.ID,

                              zsl.D3_SCID,
                              zsl.ID,
                              zsl.ZSL_ID,
                              IDCASE = (Int64?)zsl.IDCASE,
                              zsl.VIDPOM,
                              zsl.NPR_MO,
                              zsl.LPU,
                              zsl.FOR_POM,
                              zsl.DATE_Z_1,
                              zsl.DATE_Z_2,
                              zsl.RSLT,
                              zsl.ISHOD,
                              zsl.OS_SLUCH,
                              zsl.OS_SLUCH_REGION,
                              zsl.IDSP,
                              zsl.SUMV,
                              zsl.OPLATA,
                              zsl.SUMP,
                              zsl.SANK_IT,
                              zsl.MEK_COMENT,
                              zsl.OSP_COMENT,
                              zsl.USL_OK,
                              //zsl.P_CEL,
                              zsl.MEK_COUNT,
                              zsl.MEE_COUNT,
                              zsl.EKMP_COUNT,
                              zsl.EXP_COMENT,
                              zsl.EXP_TYPE,
                              zsl.EXP_DATE,
                              zsl.ReqID,
                              zsl.USER_COMENT,
                              zsl.USERID,
                              Z_P_CEL = zsl.P_CEL,
                              zsl.NPR_DATE,
                              zsl.KD_Z,
                              zsl.VB_P,
                              zsl.RSLT_D,
                              zsl.VBR,
                              sl.VID_HMP,
                              sl.METOD_HMP,
                              sl.LPU_1,
                              sl.PODR,
                              sl.PROFIL,
                              sl.DET,
                              sl.P_CEL25,
                              sl.TAL_NUM,
                              sl.TAL_D,
                              sl.TAL_P,
                              sl.NHISTORY,
                              sl.P_PER,
                              sl.DATE_1,
                              sl.DATE_2,
                              sl.KD,
                              sl.DS0,
                              sl.DS1,
                              sl.DS1_PR,
                              sl.DN,
                              sl.CODE_MES1,
                              sl.CODE_MES2,
                              sl.KSG_DKK,
                              sl.N_KSG,
                              sl.KSG_PG,
                              sl.SL_K,
                              sl.IT_SL,
                              sl.REAB,
                              sl.PRVS,
                              sl.VERS_SPEC,
                              sl.PRVS_VERS,
                              sl.IDDOKT,
                              sl.ED_COL,
                              sl.TARIF,
                              sl.SUM_M,
                              sl.COMENTSL,
                              sl.PROFIL_K,
                              sl.C_ZAB,
                              sl.DS_ONK,



                              //для отображения полей Иваново, Андрей insidious
                              sl.GRAF_DN,
                              sl.KSKP,
                              sl.VID_BRIG,
                              sl.VID_VIZ,
                              sl.POVOD,
                              sl.PROFIL_REG,
                              pa.SOCSTATUS,

                              ////////////////////////////////
                              pa.FAM,
                              pa.IM,
                              pa.OT,
                              pa.W,
                              pa.DR,
                              pa.FAM_P,
                              pa.IM_P,
                              pa.OT_P,
                              pa.W_P,
                              pa.DR_P,
                              pa.MR,
                              pa.DOCTYPE,
                              pa.DOCSER,
                              pa.DOCNUM,
                              pa.SNILS,
                              pa.OKATOG,
                              pa.OKATOP,
                              pa.COMENTP,
                              pa.VPOLIS,
                              pa.SPOLIS,
                              pa.NPOLIS,
                              pa.SMO,
                              pa.SMO_OGRN,
                              pa.SMO_OK,
                              pa.SMO_NAM,
                              pa.NOVOR,
                              pa.SOC_STAT,
                              pa.KOD_TER,
                              pa.KAT_LGOT,
                              pa.MSE,
                              zsl.VOZR,
                              pa.INV,
                              pa.VETERAN,
                              pa.WORK_STAT,
                              pa.AdressP, // поле Адрес регистрации добавил Андрей insidiuos
                          };
            _linqInstantFeedbackDataSource.QueryableSource = _pQueryable;
            var sci = (from exq in _ElmedDataClassesDataContext.D3_EXP_QUERY
                       join zsl in _ElmedDataClassesDataContext.D3_ZSL_OMS on exq.D3_ZSLID equals zsl.ID
                       where exq.QUID == ui
                       select new
                       {
                           zsl.D3_SCID,
                       }).ToList();
            Scids = sci.Select(x => x.D3_SCID).ToList();
        }


        public void BindDataSearch(string lpu, int? m1, int? m2, int? y1, int? y2,
            int? profil, string ds, string pcel, int? uslOk, int? osSl, string st)
        {           
            var id1 = Reader2List.SelectScalar($@"select isnull(min(id),1) from d3_schet_oms where month = {m1 ?? 1} and year={y1 ?? 2099}", SprClass.LocalConnectionString);
            var id2 = Reader2List.SelectScalar($@"select isnull(max(id),100000) from d3_schet_oms where month = {m2 ?? 12} and year={y2 ?? 2099}", SprClass.LocalConnectionString);

            //ShowSlColumn();
            SlCheckEdit.IsEnabled = false;
                _pQueryable = from zsl in _ElmedDataClassesDataContext.D3_ZSL_OMS
                              join pa in _ElmedDataClassesDataContext.D3_PACIENT_OMS on zsl.D3_PID equals pa.ID
                              join sc in _ElmedDataClassesDataContext.D3_SCHET_OMS on zsl.D3_SCID equals sc.ID
                              join sl in _ElmedDataClassesDataContext.D3_SL_OMS on zsl.ID equals sl.D3_ZSLID
                              join sprsc in _ElmedDataClassesDataContext.Yamed_Spr_SchetType on sc.SchetType equals sprsc.ID
                              where (sc.ID >= (int?)id1 && sc.ID <= (int?)id2)  /*((sc.MONTH >= m1_ && sc.MONTH <= m2_) || m1_ == null || m2_ == null) && ((sc.YEAR >= y1_ && sc.YEAR <= y2_) || y1_ == null || y2_ == null)*/
                              && (zsl.LPU == lpu || lpu == null) && (sl.PROFIL == profil || profil == null) && (sl.DS1.StartsWith(ds) || ds == null) && (sl.P_CEL25 == pcel || pcel == null)
                              && (zsl.USL_OK == uslOk || uslOk == null) && (zsl.OS_SLUCH_REGION == osSl || osSl == null) && (sc.SchetType == st || st == null)
                              select new
                              {
                                  sc.YEAR,
                                  sc.MONTH,
                                  sc.NSCHET,
                                  sc.DSCHET,
                                  SchetType = sprsc.NameWithID,
                                  sc.OmsFileName,
                                  zsl.PR_NOV,

                                  KeyID = sl.ID,

                                  zsl.D3_SCID,
                                  zsl.ID,
                                  zsl.ZSL_ID,
                                  IDCASE = (Int64?)zsl.IDCASE,
                                  zsl.VIDPOM,
                                  zsl.NPR_MO,
                                  zsl.LPU,
                                  zsl.FOR_POM,
                                  zsl.DATE_Z_1,
                                  zsl.DATE_Z_2,
                                  zsl.RSLT,
                                  zsl.ISHOD,
                                  zsl.OS_SLUCH,
                                  zsl.OS_SLUCH_REGION,
                                  zsl.IDSP,
                                  zsl.SUMV,
                                  zsl.OPLATA,
                                  zsl.SUMP,
                                  zsl.SANK_IT,
                                  zsl.MEK_COMENT,
                                  zsl.OSP_COMENT,
                                  zsl.USL_OK,
                                  zsl.VBR,
                                  //zsl.P_CEL,
                                  zsl.MEK_COUNT,
                                  zsl.MEE_COUNT,
                                  zsl.EKMP_COUNT,
                                  zsl.EXP_COMENT,
                                  zsl.EXP_TYPE,
                                  zsl.EXP_DATE,
                                  zsl.ReqID,
                                  zsl.USER_COMENT,
                                  zsl.USERID,
                                  Z_P_CEL = zsl.P_CEL,
                                  zsl.NPR_DATE,
                                  zsl.KD_Z,
                                  zsl.VB_P,
                                  zsl.RSLT_D,

                                  sl.VID_HMP,
                                  sl.METOD_HMP,
                                  sl.LPU_1,
                                  sl.PODR,
                                  sl.PROFIL,
                                  sl.DET,
                                  sl.P_CEL25,
                                  sl.TAL_NUM,
                                  sl.TAL_D,
                                  sl.TAL_P,
                                  sl.NHISTORY,
                                  sl.P_PER,
                                  sl.DATE_1,
                                  sl.DATE_2,
                                  sl.KD,
                                  sl.DS0,
                                  sl.DS1,
                                  sl.DS1_PR,
                                  sl.DN,
                                  sl.CODE_MES1,
                                  sl.CODE_MES2,
                                  sl.KSG_DKK,
                                  sl.N_KSG,
                                  sl.KSG_PG,
                                  sl.SL_K,
                                  sl.IT_SL,
                                  sl.REAB,
                                  sl.PRVS,
                                  sl.VERS_SPEC,
                                  sl.PRVS_VERS,
                                  sl.IDDOKT,
                                  sl.ED_COL,
                                  sl.TARIF,
                                  sl.SUM_M,
                                  sl.COMENTSL,
                                  sl.PROFIL_K,
                                  sl.C_ZAB,
                                  sl.DS_ONK,


                                  //для отображения полей Иваново, Андрей insidious
                                  sl.GRAF_DN,
                                  sl.KSKP,
                                  sl.VID_BRIG,
                                  sl.VID_VIZ,
                                  sl.POVOD,
                                  sl.PROFIL_REG,
                                  pa.SOCSTATUS,

                                  ////////////////////////////////
                                  pa.FAM,
                                  pa.IM,
                                  pa.OT,
                                  pa.W,
                                  pa.DR,
                                  pa.FAM_P,
                                  pa.IM_P,
                                  pa.OT_P,
                                  pa.W_P,
                                  pa.DR_P,
                                  pa.MR,
                                  pa.DOCTYPE,
                                  pa.DOCSER,
                                  pa.DOCNUM,
                                  pa.SNILS,
                                  pa.OKATOG,
                                  pa.OKATOP,
                                  pa.COMENTP,
                                  pa.VPOLIS,
                                  pa.SPOLIS,
                                  pa.NPOLIS,
                                  pa.SMO,
                                  pa.SMO_OGRN,
                                  pa.SMO_OK,
                                  pa.SMO_NAM,
                                  pa.NOVOR,
                                  zsl.VOZR,
                                  pa.SOC_STAT,
                                  pa.KOD_TER,
                                  pa.KAT_LGOT,
                                  pa.MSE,
                                  pa.INV,
                                  pa.VETERAN,
                                  pa.WORK_STAT,
                                  pa.AdressP, // поле Адрес регистрации добавил Андрей insidiuos
                              };
                _linqInstantFeedbackDataSource.QueryableSource = _pQueryable;
        }

        public void BindAktExp(int arid)
        {
            HideSlColumn();

            _pQueryable = from zsl in _ElmedDataClassesDataContext.D3_ZSL_OMS
                join pa in _ElmedDataClassesDataContext.D3_PACIENT_OMS on zsl.D3_PID equals pa.ID
                join sc in _ElmedDataClassesDataContext.D3_SCHET_OMS on zsl.D3_SCID equals sc.ID
                join req in _ElmedDataClassesDataContext.D3_REQ_OMS on zsl.ID equals req.D3_ZSLID
                where req.D3_ARID == arid
                select new
                {
                    sc.YEAR,
                    sc.MONTH,
                    sc.NSCHET,
                    sc.DSCHET,
                    sc.OmsFileName,

                    zsl.PR_NOV,
                    KeyID = req.ID,
                    zsl.D3_SCID,
                    zsl.ID,
                    zsl.ZSL_ID,
                    IDCASE = (Int64?) zsl.IDCASE,
                    zsl.VIDPOM,
                    zsl.NPR_MO,
                    zsl.LPU,
                    zsl.FOR_POM,
                    zsl.DATE_Z_1,
                    zsl.DATE_Z_2,
                    zsl.RSLT,
                    zsl.ISHOD,
                    zsl.OS_SLUCH,
                    zsl.OS_SLUCH_REGION,
                    zsl.IDSP,
                    zsl.SUMV,
                    zsl.OPLATA,
                    zsl.SUMP,
                    zsl.SANK_IT,
                    zsl.MEK_COMENT,
                    zsl.OSP_COMENT,
                    zsl.USL_OK,
                    Z_P_CEL = zsl.P_CEL,
                    zsl.MEK_COUNT,
                    zsl.MEE_COUNT,
                    zsl.EKMP_COUNT,
                    zsl.EXP_COMENT,
                    zsl.EXP_TYPE,
                    zsl.EXP_DATE,
                    zsl.ReqID,
                    zsl.USER_COMENT,
                    zsl.USERID,
                    zsl.NPR_DATE,
                    zsl.KD_Z,
                    zsl.VB_P,
                    zsl.RSLT_D,
                    zsl.VBR,

                    pa.FAM,
                    pa.IM,
                    pa.OT,
                    pa.MO_ATT,
                    pa.W,
                    pa.DR,
                    pa.FAM_P,
                    pa.IM_P,
                    pa.OT_P,
                    pa.W_P,
                    pa.DR_P,
                    pa.MR,
                    pa.DOCTYPE,
                    pa.DOCSER,
                    pa.DOCNUM,
                    pa.SNILS,
                    pa.OKATOG,
                    pa.OKATOP,
                    pa.COMENTP,
                    pa.VPOLIS,
                    pa.SPOLIS,
                    pa.NPOLIS,
                    pa.SMO,
                    pa.SMO_OGRN,
                    pa.SMO_OK,
                    pa.SMO_NAM,
                    pa.NOVOR,
                    pa.SOC_STAT,
                    pa.KOD_TER,
                    pa.KAT_LGOT,
                    pa.MSE,
                    pa.INV,
                    pa.VETERAN,
                    pa.WORK_STAT,

                    zsl.VOZR,
                    pa.AdressP, // поле Адрес регистрации добавил Андрей insidiuos

                    //для отображения в Иваново
                    pa.SOCSTATUS
                };

            _linqInstantFeedbackDataSource.QueryableSource = _pQueryable;

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                    new Action(delegate()
                    {
                        if (!(bool) SlCheckEdit.EditValue)
                        {
                            gridControl1.Columns.Where(x => x.Name.StartsWith("Column__SL__")).ToList().ForEach(x =>
                            {
                                if (x.Tag != null)
                                    x.Width = (GridColumnWidth) x.Tag;
                            });
                        }

                        Stream mStream = new MemoryStream();
                        gridControl1.SaveLayoutToStream(mStream);
                        mStream.Seek(0, SeekOrigin.Begin);
                        StreamReader reader = new StreamReader(mStream);

                        using (var dc = new YamedDataClassesDataContext(SprClass.LocalConnectionString))
                        {
                            Yamed_Users first = dc.Yamed_Users.Single(x => x.ID == SprClass.userId);
                            first.LayRTable = reader.ReadToEnd();
                            dc.SubmitChanges();
                        }

                        mStream.Close();
                        reader.Close();
                    }));




            //_zsls = null;

            _ElmedDataClassesDataContext.Dispose();
            _linqInstantFeedbackDataSource.Dispose();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {



        }


        private int _rowHandle;
        private void view_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed) return;
            GridColumn col = ((TableView)sender).GetColumnByMouseEventArgs(e) as GridColumn;
            if (col == null) return;
            int rowHandle = ((TableView)sender).GetRowHandleByMouseEventArgs(e);
            if (rowHandle == _rowHandle) return;

            if (rowHandle >= 0)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    gridControl1.SelectItem(rowHandle);
                }));
            _rowHandle = rowHandle;
            }
        }

        private void View_OnCellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            (sender as TableView).PostEditor();
        }

        private void MtrCheck_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if ((bool?) e.NewValue == true)
            {
                if (string.IsNullOrEmpty(gridControl1.FilterString))
                {
                    gridControl1.FilterString = $"([SMO] Is Null Or [SMO] Not Like '46%')";

                }
                else
                {
                    gridControl1.FilterString += $"And ([SMO] Is Null Or [SMO] Not Like '46%')";

                }
            }
            else
            {
                gridControl1.FilterString =
                    gridControl1.FilterString.
                        Replace($" And ([SMO] Is Null Or [SMO] Not Like '46%')", "").
                        Replace($"([SMO] Is Null Or [SMO] Not Like '46%') And ", "").

                        Replace($"([SMO] Is Null Or [SMO] Not Like '46%')", "").
                        Replace($"[SMO] Is Null Or [SMO] Not Like '46%'", "");

            }
        }

        private void BaseEdit_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                BindDataSl();
            }
            else
            {
                BindDataZsl();
            }

        }

        private void PacientRowItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var row = DxHelper.GetSelectedGridRow(gridControl1);
            var fam = (string)ObjHelper.GetAnonymousValue(row, "FAM");
            var im = (string)ObjHelper.GetAnonymousValue(row, "IM");
            var ot = (string)ObjHelper.GetAnonymousValue(row, "OT");
            var dr = (DateTime)ObjHelper.GetAnonymousValue(row, "DR");

            //string cmd = SprClass.schetQuery +
            //             $" where FAM = '{row.FAM}' and IM = '{row.IM}' and OT= '{row.OT}' and DR = '{row.DR?.Date.ToString("yyyyMMdd")}'";

            var rc = new Oms.SchetRegisterControl();
            rc.SchetRegisterGrid1.BindDataPacient(fam, im, ot, dr);
            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Реестр счета",
                MyControl = rc,
                IsCloseable = "True",
                //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
            });
        }

        public int GetSelectedRowId()
        {
            return (int)ObjHelper.GetAnonymousValue(DxHelper.GetSelectedGridRow(gridControl1), "ID");
        }

        private void UserComentItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            DxHelper.GetSelectedGridRowsAsync(ref gridControl1);
            bool isLoaded = false;
            gridControl1.IsEnabled = false;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        if (gridControl1.IsAsyncOperationInProgress == false)
                        {
                            isLoaded = true;
                        }
                    });
                    if (isLoaded) break;
                    Thread.Sleep(200);
                }

            }).ContinueWith(lr =>
            {
                if (DxHelper.LoadedRows.Count > 0)
                {
                    var window = new DXWindow
                    {
                        ShowIcon = false,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        Content = new ReqControl(1),
                        Title = "Коментарий пользователя",
                        SizeToContent = SizeToContent.Height,
                        Width = 300
                    };
                    window.ShowDialog();
                }
                gridControl1.IsEnabled = true;
                DxHelper.LoadedRows.Clear();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void FlkGroup()
        {
            gridControl1.ClearGrouping();
            gridControl1.GroupBy("EXP_COMENT");

        }

        private void PacientOnkoRowItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            var row = DxHelper.GetSelectedGridRow(gridControl1);
            var fam = (string)ObjHelper.GetAnonymousValue(row, "FAM");
            var im = (string)ObjHelper.GetAnonymousValue(row, "IM");
            var ot = (string)ObjHelper.GetAnonymousValue(row, "OT");
            var dr = (DateTime)ObjHelper.GetAnonymousValue(row, "DR");

            var q = $@"
declare @fam nvarchar(50)='{fam}'
declare @im nvarchar(50)='{im}'
declare @ot nvarchar(50)='{ot}'
declare @dr datetime='{dr.ToString("yyyyMMdd")}'


Select distinct sch.month, SCH.year,sch.code_mo,zs.ID, zs.SUMV, '' com 
FROM [D3_SCHET_OMS] sch                
  inner join D3_PACIENT_OMS p on p.d3_scid=sch.id 
    and ltrim(rtrim(p.fam))=@fam and ltrim(rtrim(p.im))=@im and ltrim(rtrim(p.ot))=@ot and p.dr=@dr
  inner join D3_ZSL_OMS zs on zs.D3_PID=p.id --and zs.usl_ok<>4 and (zs.OS_SLUCH_REGION not in (7,8,9,11,12,17,18,22,23,37,38) or zs.OS_SLUCH_REGION is null)
 join D3_SL_OMS s on s.D3_ZSLID=zs.ID  -- and (s.ds1 not like 'Z%')
 left join   D3_DSS_OMS sd on sd.D3_SLID=s.id and sd.DS_TYPE=2  
 where 
    (
    sch.year>=2019
     and 
     (sch.SchetType like 'C' or (
                    (sch.SchetType like 'D%' or sch.SchetType like 'T%')
                     and                                     
                      (S.DS1 LIKE 'C%' 
                            OR 
                      S.DS1 between 'D00' AND 'D09.z'
                             OR
                       (S.DS1 LIKE 'd70%' AND SD.DS LIKE 'C%') 
                          or
                        ds_onk=1
                       )    
                    
                  )
     )
     )
    or 
    (sch.year<2019 and (S.DS1 LIKE 'C%' OR S.DS1 between 'D00' AND 'D09.z') 
  )                                 
";
            var result = SqlReader.Select(q, SprClass.LocalConnectionString).Select(x=>x.GetValue("ID")).OfType<int>().ToList();
            var rc = new Oms.SchetRegisterControl();
            rc.SchetRegisterGrid1.BindDataExpResult(result);
            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header =  $"Онко-история {fam} {im} {ot}",
                MyControl = rc,
                IsCloseable = "True",
                //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
            });
        }

        private void UslCheckEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                BindDataUsl();
            }
            else
            {
                BindDataZsl();
            }

        }

        public void BindDataUsl()
        {
            ShowSlColumn();
            if (zslid != null)
            {
                _pQueryable = from zsl in _ElmedDataClassesDataContext.D3_ZSL_OMS
                              join pa in _ElmedDataClassesDataContext.D3_PACIENT_OMS on zsl.D3_PID equals pa.ID
                              join sc in _ElmedDataClassesDataContext.D3_SCHET_OMS on zsl.D3_SCID equals sc.ID
                              join sl in _ElmedDataClassesDataContext.D3_SL_OMS on zsl.ID equals sl.D3_ZSLID
                              join usl in _ElmedDataClassesDataContext.D3_USL_OMS on sl.ID equals usl.D3_SLID
                              join sprsc in _ElmedDataClassesDataContext.Yamed_Spr_SchetType on sc.SchetType equals sprsc.ID
                              join lksg in _ElmedDataClassesDataContext.D3_KSG_KPG_OMS on sl.ID equals lksg.D3_SLID into tmpksg
                              from ksg in tmpksg.DefaultIfEmpty()
                                  //join lusl in _ElmedDataClassesDataContext.D3_USL_OMS on sl.ID equals lusl.D3_SLID into tmpusl
                                  //from usl in tmpusl.DefaultIfEmpty()
                              where (Scids.Contains(zsl.D3_SCID) || !Scids.Any()) && (zslid.Contains(zsl.ID) || !zslid.Any())
                              select new
                              {
                                  sc.YEAR,
                                  sc.MONTH,
                                  sc.NSCHET,
                                  sc.DSCHET,
                                  SchetType = sprsc.NameWithID,
                                  sc.OmsFileName,
                                  zsl.PR_NOV,

                                  //KeyID = usl.ID == null ? "_" + sl.ID: "_" + sl.ID + "_" + usl.ID,
                                  KeyID = usl.ID,
                                  zsl.D3_SCID,
                                  zsl.ID,
                                  zsl.ZSL_ID,
                                  IDCASE = (Int64?)zsl.IDCASE,
                                  zsl.VIDPOM,
                                  zsl.NPR_MO,
                                  zsl.LPU,
                                  zsl.FOR_POM,
                                  zsl.DATE_Z_1,
                                  zsl.DATE_Z_2,
                                  zsl.RSLT,
                                  zsl.ISHOD,
                                  zsl.OS_SLUCH,
                                  zsl.OS_SLUCH_REGION,
                                  zsl.IDSP,
                                  zsl.SUMV,
                                  zsl.OPLATA,
                                  zsl.SUMP,
                                  zsl.SANK_IT,
                                  zsl.MEK_COMENT,
                                  zsl.OSP_COMENT,
                                  zsl.USL_OK,
                                  zsl.MEK_COUNT,
                                  zsl.MEE_COUNT,
                                  zsl.EKMP_COUNT,
                                  zsl.EXP_COMENT,
                                  zsl.EXP_TYPE,
                                  zsl.EXP_DATE,
                                  zsl.ReqID,
                                  zsl.USER_COMENT,
                                  zsl.USERID,
                                  Z_P_CEL = zsl.P_CEL,
                                  zsl.NPR_DATE,
                                  zsl.KD_Z,
                                  zsl.VB_P,
                                  zsl.RSLT_D,
                                  zsl.VBR,

                                  ///////////////////////////////////
                                  sl.VID_HMP,
                                  sl.METOD_HMP,
                                  sl.LPU_1,
                                  sl.PODR,
                                  sl.PROFIL,
                                  sl.DET,
                                  sl.P_CEL25,
                                  sl.TAL_NUM,
                                  sl.TAL_D,
                                  sl.TAL_P,
                                  sl.NHISTORY,
                                  sl.P_PER,
                                  sl.DATE_1,
                                  sl.DATE_2,
                                  sl.KD,
                                  sl.DS0,
                                  sl.DS1,
                                  sl.DS1_PR,
                                  sl.DN,
                                  sl.CODE_MES1,
                                  sl.CODE_MES2,
                                  sl.KSG_DKK,
                                  //sl.N_KSG,
                                  //sl.KSG_PG,
                                  //sl.SL_K,
                                  //sl.IT_SL,
                                  ksg.N_KSG,
                                  ksg.KSG_PG,
                                  ksg.SL_K,
                                  ksg.IT_SL,


                                  //для отображения полей Иваново, Андрей insidious
                                  sl.GRAF_DN,
                                  sl.KSKP,
                                  sl.VID_BRIG,
                                  sl.VID_VIZ,
                                  sl.POVOD,
                                  sl.PROFIL_REG,
                                  pa.SOCSTATUS,


                                  sl.REAB,
                                  sl.PRVS,
                                  sl.VERS_SPEC,
                                  sl.PRVS_VERS,
                                  sl.IDDOKT,
                                  sl.ED_COL,
                                  sl.TARIF,
                                  sl.SUM_M,
                                  sl.COMENTSL,
                                  sl.PROFIL_K,
                                  sl.C_ZAB,
                                  sl.DS_ONK,
                                  ////////////////////////////////

                                  usl.VID_VME,
                                  usl.CODE_USL,
                                  TARIF_USL = usl.TARIF,
                                  usl.KOL_USL,
                                  usl.SUMV_USL,
                                  usl.DATE_IN,
                                  usl.DATE_OUT,
                                  usl.KOD_SP,

                                  ////////////////////////////////
                                  pa.FAM,
                                  pa.IM,
                                  pa.OT,
                                  pa.W,
                                  pa.DR,
                                  pa.FAM_P,
                                  pa.IM_P,
                                  pa.OT_P,
                                  pa.W_P,
                                  pa.DR_P,
                                  pa.MR,
                                  pa.DOCTYPE,
                                  pa.DOCSER,
                                  pa.DOCNUM,
                                  pa.SNILS,
                                  pa.OKATOG,
                                  pa.OKATOP,
                                  pa.COMENTP,
                                  pa.VPOLIS,
                                  pa.SPOLIS,
                                  pa.NPOLIS,
                                  pa.SMO,
                                  pa.SMO_OGRN,
                                  pa.SMO_OK,
                                  pa.SMO_NAM,
                                  pa.NOVOR,
                                  zsl.VOZR,
                                  pa.SOC_STAT,
                                  pa.KOD_TER,
                                  pa.KAT_LGOT,
                                  pa.MSE,
                                  pa.INV,
                                  pa.VETERAN,
                                  pa.WORK_STAT,
                                  pa.AdressP, // поле Адрес регистрации добавил Андрей insidiuos
                              };
                _linqInstantFeedbackDataSource.QueryableSource = _pQueryable;
            }
            else
            {
                _pQueryable = from zsl in _ElmedDataClassesDataContext.D3_ZSL_OMS
                              join pa in _ElmedDataClassesDataContext.D3_PACIENT_OMS on zsl.D3_PID equals pa.ID
                              join sc in _ElmedDataClassesDataContext.D3_SCHET_OMS on zsl.D3_SCID equals sc.ID
                              join sl in _ElmedDataClassesDataContext.D3_SL_OMS on zsl.ID equals sl.D3_ZSLID
                              join usl in _ElmedDataClassesDataContext.D3_USL_OMS on sl.ID equals usl.D3_SLID
                              join sprsc in _ElmedDataClassesDataContext.Yamed_Spr_SchetType on sc.SchetType equals sprsc.ID
                              join lksg in _ElmedDataClassesDataContext.D3_KSG_KPG_OMS on sl.ID equals lksg.D3_SLID into tmpksg
                              from ksg in tmpksg.DefaultIfEmpty()
                                  //join lusl in _ElmedDataClassesDataContext.D3_USL_OMS on sl.ID equals lusl.D3_SLID into tmpusl
                                  //from usl in tmpusl.DefaultIfEmpty()
                              where (Scids.Contains(zsl.D3_SCID) || !Scids.Any())
                              select new
                              {
                                  sc.YEAR,
                                  sc.MONTH,
                                  sc.NSCHET,
                                  sc.DSCHET,
                                  SchetType = sprsc.NameWithID,
                                  sc.OmsFileName,
                                  zsl.PR_NOV,

                                  //KeyID = usl.ID == null ? "_" + sl.ID: "_" + sl.ID + "_" + usl.ID,
                                  KeyID = usl.ID,
                                  zsl.D3_SCID,
                                  zsl.ID,
                                  zsl.ZSL_ID,
                                  IDCASE = (Int64?)zsl.IDCASE,
                                  zsl.VIDPOM,
                                  zsl.NPR_MO,
                                  zsl.LPU,
                                  zsl.FOR_POM,
                                  zsl.DATE_Z_1,
                                  zsl.DATE_Z_2,
                                  zsl.RSLT,
                                  zsl.ISHOD,
                                  zsl.OS_SLUCH,
                                  zsl.OS_SLUCH_REGION,
                                  zsl.IDSP,
                                  zsl.SUMV,
                                  zsl.OPLATA,
                                  zsl.SUMP,
                                  zsl.SANK_IT,
                                  zsl.MEK_COMENT,
                                  zsl.OSP_COMENT,
                                  zsl.USL_OK,
                                  zsl.MEK_COUNT,
                                  zsl.MEE_COUNT,
                                  zsl.EKMP_COUNT,
                                  zsl.EXP_COMENT,
                                  zsl.EXP_TYPE,
                                  zsl.EXP_DATE,
                                  zsl.ReqID,
                                  zsl.USER_COMENT,
                                  zsl.USERID,
                                  Z_P_CEL = zsl.P_CEL,
                                  zsl.NPR_DATE,
                                  zsl.KD_Z,
                                  zsl.VB_P,
                                  zsl.RSLT_D,
                                  zsl.VBR,

                                  ///////////////////////////////////
                                  sl.VID_HMP,
                                  sl.METOD_HMP,
                                  sl.LPU_1,
                                  sl.PODR,
                                  sl.PROFIL,
                                  sl.DET,
                                  sl.P_CEL25,
                                  sl.TAL_NUM,
                                  sl.TAL_D,
                                  sl.TAL_P,
                                  sl.NHISTORY,
                                  sl.P_PER,
                                  sl.DATE_1,
                                  sl.DATE_2,
                                  sl.KD,
                                  sl.DS0,
                                  sl.DS1,
                                  sl.DS1_PR,
                                  sl.DN,
                                  sl.CODE_MES1,
                                  sl.CODE_MES2,
                                  sl.KSG_DKK,
                                  //sl.N_KSG,
                                  //sl.KSG_PG,
                                  //sl.SL_K,
                                  //sl.IT_SL,
                                  ksg.N_KSG,
                                  ksg.KSG_PG,
                                  ksg.SL_K,
                                  ksg.IT_SL,


                                  //для отображения полей Иваново, Андрей insidious
                                  sl.GRAF_DN,
                                  sl.KSKP,
                                  sl.VID_BRIG,
                                  sl.VID_VIZ,
                                  sl.POVOD,
                                  sl.PROFIL_REG,
                                  pa.SOCSTATUS,


                                  sl.REAB,
                                  sl.PRVS,
                                  sl.VERS_SPEC,
                                  sl.PRVS_VERS,
                                  sl.IDDOKT,
                                  sl.ED_COL,
                                  sl.TARIF,
                                  sl.SUM_M,
                                  sl.COMENTSL,
                                  sl.PROFIL_K,
                                  sl.C_ZAB,
                                  sl.DS_ONK,
                                  ////////////////////////////////

                                  usl.VID_VME,
                                  usl.CODE_USL,
                                  TARIF_USL = usl.TARIF,
                                  usl.KOL_USL,
                                  usl.SUMV_USL,
                                  usl.DATE_IN,
                                  usl.DATE_OUT,
                                  usl.KOD_SP,
                                  ////////////////////////////////
                                  pa.FAM,
                                  pa.IM,
                                  pa.OT,
                                  pa.W,
                                  pa.DR,
                                  pa.FAM_P,
                                  pa.IM_P,
                                  pa.OT_P,
                                  pa.W_P,
                                  pa.DR_P,
                                  pa.MR,
                                  pa.DOCTYPE,
                                  pa.DOCSER,
                                  pa.DOCNUM,
                                  pa.SNILS,
                                  pa.OKATOG,
                                  pa.OKATOP,
                                  pa.COMENTP,
                                  pa.VPOLIS,
                                  pa.SPOLIS,
                                  pa.NPOLIS,
                                  pa.SMO,
                                  pa.SMO_OGRN,
                                  pa.SMO_OK,
                                  pa.SMO_NAM,
                                  pa.NOVOR,
                                  zsl.VOZR,
                                  pa.SOC_STAT,
                                  pa.KOD_TER,
                                  pa.KAT_LGOT,
                                  pa.MSE,
                                  pa.INV,
                                  pa.VETERAN,
                                  pa.WORK_STAT,
                                  pa.AdressP, // поле Адрес регистрации добавил Андрей insidiuos
                              };
                _linqInstantFeedbackDataSource.QueryableSource = _pQueryable;
            }
        }

        public void BindDataSank()
        {
            HideSlColumn();
            if (zslid != null)
            {
                _pQueryable = from zsl in _ElmedDataClassesDataContext.D3_ZSL_OMS
                              join pa in _ElmedDataClassesDataContext.D3_PACIENT_OMS on zsl.D3_PID equals pa.ID
                              join sc in _ElmedDataClassesDataContext.D3_SCHET_OMS on zsl.D3_SCID equals sc.ID
                              //join sl in _ElmedDataClassesDataContext.D3_SL_OMS on zsl.ID equals sl.D3_ZSLID
                              join sa in _ElmedDataClassesDataContext.D3_SANK_OMS on zsl.ID equals sa.D3_ZSLID
                              join sprsc in _ElmedDataClassesDataContext.Yamed_Spr_SchetType on sc.SchetType equals sprsc.ID
                              //join lksg in _ElmedDataClassesDataContext.D3_KSG_KPG_OMS on sl.ID equals lksg.D3_SLID into tmpksg
                              //from ksg in tmpksg.DefaultIfEmpty()
                              //join lusl in _ElmedDataClassesDataContext.D3_USL_OMS on sl.ID equals lusl.D3_SLID into tmpusl
                              //from usl in tmpusl.DefaultIfEmpty()
                              where (Scids.Contains(zsl.D3_SCID) || !Scids.Any()) && (zslid.Contains(zsl.ID) || !zslid.Any())
                              select new
                              {
                                  sc.YEAR,
                                  sc.MONTH,
                                  sc.NSCHET,
                                  sc.DSCHET,
                                  SchetType = sprsc.NameWithID,
                                  sc.OmsFileName,
                                  zsl.PR_NOV,

                                  //KeyID = usl.ID == null ? "_" + sl.ID: "_" + sl.ID + "_" + usl.ID,
                                  KeyID = sa.ID,
                                  zsl.D3_SCID,
                                  zsl.ID,
                                  zsl.ZSL_ID,
                                  IDCASE = (Int64?)zsl.IDCASE,
                                  zsl.VIDPOM,
                                  zsl.NPR_MO,
                                  zsl.LPU,
                                  zsl.FOR_POM,
                                  zsl.DATE_Z_1,
                                  zsl.DATE_Z_2,
                                  zsl.RSLT,
                                  zsl.ISHOD,
                                  zsl.OS_SLUCH,
                                  zsl.OS_SLUCH_REGION,
                                  zsl.IDSP,
                                  zsl.SUMV,
                                  zsl.OPLATA,
                                  zsl.SUMP,
                                  zsl.SANK_IT,
                                  zsl.MEK_COMENT,
                                  zsl.OSP_COMENT,
                                  zsl.USL_OK,
                                  zsl.MEK_COUNT,
                                  zsl.MEE_COUNT,
                                  zsl.EKMP_COUNT,
                                  zsl.EXP_COMENT,
                                  zsl.EXP_TYPE,
                                  zsl.EXP_DATE,
                                  zsl.ReqID,
                                  zsl.USER_COMENT,
                                  zsl.USERID,
                                  Z_P_CEL = zsl.P_CEL,
                                  zsl.NPR_DATE,
                                  zsl.KD_Z,
                                  zsl.VB_P,
                                  zsl.RSLT_D,
                                  zsl.VBR,

                                  ///////////////////////////////////
                                  //для отображения полей Иваново, Андрей insidious
                                  //sl.GRAF_DN,
                                  //sl.KSKP,
                                  //sl.VID_BRIG,
                                  //sl.VID_VIZ,
                                  //sl.POVOD,
                                  //sl.PROFIL_REG,
                                  pa.SOCSTATUS,
                                  ////////////////////////////////
                                  sa.CODE_EXP,
                                  sa.DATE_ACT,
                                  sa.NUM_ACT,
                                  sa.MODEL_ID,
                                  sa.S_DATE,
                                  sa.S_OSN,
                                  sa.S_SUM,
                                  sa.S_SUM2,
                                  sa.S_TIP,
                                  sa.S_TIP2,


                                  ////////////////////////////////
                                  pa.FAM,
                                  pa.IM,
                                  pa.OT,
                                  pa.W,
                                  pa.DR,
                                  pa.FAM_P,
                                  pa.IM_P,
                                  pa.OT_P,
                                  pa.W_P,
                                  pa.DR_P,
                                  pa.MR,
                                  pa.DOCTYPE,
                                  pa.DOCSER,
                                  pa.DOCNUM,
                                  pa.SNILS,
                                  pa.OKATOG,
                                  pa.OKATOP,
                                  pa.COMENTP,
                                  pa.VPOLIS,
                                  pa.SPOLIS,
                                  pa.NPOLIS,
                                  pa.SMO,
                                  pa.SMO_OGRN,
                                  pa.SMO_OK,
                                  pa.SMO_NAM,
                                  pa.NOVOR,
                                  zsl.VOZR,
                                  pa.SOC_STAT,
                                  pa.KOD_TER,
                                  pa.KAT_LGOT,
                                  pa.MSE,
                                  pa.INV,
                                  pa.VETERAN,
                                  pa.WORK_STAT,
                                  pa.AdressP, // поле Адрес регистрации добавил Андрей insidiuos
                              };
                _linqInstantFeedbackDataSource.QueryableSource = _pQueryable;
            }
            else
            {
                _pQueryable = from zsl in _ElmedDataClassesDataContext.D3_ZSL_OMS
                              join pa in _ElmedDataClassesDataContext.D3_PACIENT_OMS on zsl.D3_PID equals pa.ID
                              join sc in _ElmedDataClassesDataContext.D3_SCHET_OMS on zsl.D3_SCID equals sc.ID
                              //join sl in _ElmedDataClassesDataContext.D3_SL_OMS on zsl.ID equals sl.D3_ZSLID
                              join sa in _ElmedDataClassesDataContext.D3_SANK_OMS on zsl.ID equals sa.D3_ZSLID
                              join sprsc in _ElmedDataClassesDataContext.Yamed_Spr_SchetType on sc.SchetType equals sprsc.ID
                              //join lksg in _ElmedDataClassesDataContext.D3_KSG_KPG_OMS on sl.ID equals lksg.D3_SLID into tmpksg
                              //from ksg in tmpksg.DefaultIfEmpty()
                              //join lusl in _ElmedDataClassesDataContext.D3_USL_OMS on sl.ID equals lusl.D3_SLID into tmpusl
                              //from usl in tmpusl.DefaultIfEmpty()
                              where (Scids.Contains(zsl.D3_SCID) || !Scids.Any())
                              select new
                              {
                                  sc.YEAR,
                                  sc.MONTH,
                                  sc.NSCHET,
                                  sc.DSCHET,
                                  SchetType = sprsc.NameWithID,
                                  sc.OmsFileName,
                                  zsl.PR_NOV,

                                  //KeyID = usl.ID == null ? "_" + sl.ID: "_" + sl.ID + "_" + usl.ID,
                                  KeyID = sa.ID,
                                  zsl.D3_SCID,
                                  zsl.ID,
                                  zsl.ZSL_ID,
                                  IDCASE = (Int64?)zsl.IDCASE,
                                  zsl.VIDPOM,
                                  zsl.NPR_MO,
                                  zsl.LPU,
                                  zsl.FOR_POM,
                                  zsl.DATE_Z_1,
                                  zsl.DATE_Z_2,
                                  zsl.RSLT,
                                  zsl.ISHOD,
                                  zsl.OS_SLUCH,
                                  zsl.OS_SLUCH_REGION,
                                  zsl.IDSP,
                                  zsl.SUMV,
                                  zsl.OPLATA,
                                  zsl.SUMP,
                                  zsl.SANK_IT,
                                  zsl.MEK_COMENT,
                                  zsl.OSP_COMENT,
                                  zsl.USL_OK,
                                  zsl.MEK_COUNT,
                                  zsl.MEE_COUNT,
                                  zsl.EKMP_COUNT,
                                  zsl.EXP_COMENT,
                                  zsl.EXP_TYPE,
                                  zsl.EXP_DATE,
                                  zsl.ReqID,
                                  zsl.USER_COMENT,
                                  zsl.USERID,
                                  Z_P_CEL = zsl.P_CEL,
                                  zsl.NPR_DATE,
                                  zsl.KD_Z,
                                  zsl.VB_P,
                                  zsl.RSLT_D,
                                  zsl.VBR,

                                  ///////////////////////////////////
                                  //для отображения полей Иваново, Андрей insidious
                                  //sl.GRAF_DN,
                                  //sl.KSKP,
                                  //sl.VID_BRIG,
                                  //sl.VID_VIZ,
                                  //sl.POVOD,
                                  //sl.PROFIL_REG,
                                  pa.SOCSTATUS,
                                  ////////////////////////////////
                                  sa.CODE_EXP,
                                  sa.DATE_ACT,
                                  sa.NUM_ACT,
                                  sa.MODEL_ID,
                                  sa.S_DATE,
                                  sa.S_OSN,
                                  sa.S_SUM,
                                  sa.S_SUM2,
                                  sa.S_TIP,
                                  sa.S_TIP2,


                                  ////////////////////////////////
                                  pa.FAM,
                                  pa.IM,
                                  pa.OT,
                                  pa.W,
                                  pa.DR,
                                  pa.FAM_P,
                                  pa.IM_P,
                                  pa.OT_P,
                                  pa.W_P,
                                  pa.DR_P,
                                  pa.MR,
                                  pa.DOCTYPE,
                                  pa.DOCSER,
                                  pa.DOCNUM,
                                  pa.SNILS,
                                  pa.OKATOG,
                                  pa.OKATOP,
                                  pa.COMENTP,
                                  pa.VPOLIS,
                                  pa.SPOLIS,
                                  pa.NPOLIS,
                                  pa.SMO,
                                  pa.SMO_OGRN,
                                  pa.SMO_OK,
                                  pa.SMO_NAM,
                                  pa.NOVOR,
                                  zsl.VOZR,
                                  pa.SOC_STAT,
                                  pa.KOD_TER,
                                  pa.KAT_LGOT,
                                  pa.MSE,
                                  pa.INV,
                                  pa.VETERAN,
                                  pa.WORK_STAT,
                                  pa.AdressP, // поле Адрес регистрации добавил Андрей insidiuos
                              };
                _linqInstantFeedbackDataSource.QueryableSource = _pQueryable;
            }
        }

    }
}

