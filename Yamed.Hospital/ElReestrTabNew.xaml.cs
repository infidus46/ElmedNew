using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core.ServerMode;
using DevExpress.Xpf.Grid;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.Hospital
{
    /// <summary>
    /// Логика взаимодействия для ElReestr.xaml
    /// </summary>
    public partial class ElReestrTabNew : UserControl
    {
        public readonly LinqInstantFeedbackDataSource _linqInstantFeedbackDataSource;
        private readonly YamedDataClassesDataContext _ElmedDataClassesDataContext;
        //public List<SQLTables.SluPacClass> _zsls;
        public D3_SCHET_OMS _schetId;

        public ElReestrTabNew()
        {
            InitializeComponent();

            SmoEdit.DataContext = SprClass.smo;
            UslOkEdit.DataContext = SprClass.conditionHelp;
            VidPomEdit.DataContext = SprClass.typeHelp;
            NprMoEdit.DataContext = SprClass.LpuList;
            MoEdit.DataContext = SprClass.LpuList;
            ProfilEdit.DataContext = SprClass.profile;
            VidHmpEdit.DataContext = SprClass.VidVmpList;
            MetodHmpEdit.DataContext = SprClass.MetodVmpList;
            KsgEdit.DataContext = SprClass.KsgGroups;
            Ds0Edit.DataContext = SprClass.mkbSearching;
            Ds1Edit.DataContext = SprClass.mkbSearching;
            Ds2Edit.DataContext = SprClass.mkbSearching;
            RsltEdit.DataContext = SprClass.helpResult;
            IshodEdit.DataContext = SprClass.helpExit;
            PrvsEdit.DataContext = SprClass.speciality;
            MspEdit.DataContext = SprClass.specialityNew;
            OsSlRegEdit.DataContext = SprClass.OsobSluchDbs;
            GrZdEdit.DataContext = SprClass.GrZdorovDbs;
            VeteranEdit.DataContext = SprClass.VeteranDbs;
            SchoolEdit.DataContext = SprClass.SchoolStatusDbs;
            WorkStatEdit.DataContext = SprClass.WorkStatDbs;
            IdspEdit.DataContext = SprClass.TypeOplaty;
            OplataEdit.DataContext = SprClass.TypeOplaty;
            MeeTypeEdit.DataContext = SprClass.MeeTypeDbs;
            KsgOplataEdit.DataContext = SprClass.KsgOts;
            UserEdit.DataContext = SprClass.YamedUsers;

            Lpu1Edit.DataContext = SprClass.Podr;
            PodrEdit.DataContext = SprClass.OtdelDbs;
            VozrEdit.DataContext = SprClass.VozrList;

            _ElmedDataClassesDataContext = new YamedDataClassesDataContext()
            {
                ObjectTrackingEnabled = false,
                CommandTimeout = 0,
                Connection = { ConnectionString = SprClass.LocalConnectionString }
            };
            _linqInstantFeedbackDataSource = (LinqInstantFeedbackDataSource)FindResource("LinqInstantFeedbackDataSource");
            //if (_schetId == null)
            //    BindData2();
            //else
            //    BindData(_schetId.ID);
        }

        void BindData(int? schetId)
        {
            IQueryable pQueryable = from sl in _ElmedDataClassesDataContext.SLUCH1
                join pa in _ElmedDataClassesDataContext.PACIENT1 on sl.PID equals pa.ID
                join sc in _ElmedDataClassesDataContext.D3_SCHET_OMS on sl.SCHET_ID equals sc.ID
                where sl.SCHET_ID == schetId && (sl.USL_OK == 1 || sl.USL_OK == 2)
                select new
                {
                    sc.YEAR,
                    sc.MONTH,
                    sl.ID,
                    sl.IDCASE,
                    sl.USL_OK,
                    sl.VIDPOM,
                    sl.NPR_MO,
                    sl.EXTR,
                    sl.LPU,
                    sl.LPU_1,
                    sl.PODR,
                    sl.PROFIL,
                    sl.DET,
                    sl.NHISTORY,
                    sl.DATE_1,
                    sl.DATE_2,
                    sl.DS0,
                    sl.DS1,
                    sl.DS2,
                    sl.CODE_MES1,
                    sl.CODE_MES2,
                    sl.SLUCH_TYPE,
                    sl.RSLT,
                    sl.ISHOD,
                    sl.PRVS,
                    sl.IDDOKT,
                    sl.OS_SLUCH,
                    sl.OS_SLUCH_REGION,
                    sl.IDSP,
                    sl.ED_COL,
                    sl.TARIF,
                    sl.SUMV,
                    sl.OPLATA,
                    sl.SUMP,
                    sl.SANK_IT,
                    sl.COMENTSL,
                    sl.SCHET_ID,
                    sl.GR_ZDOROV,
                    sl.VETERAN,
                    sl.SCHOOL,
                    sl.WORK_STAT,
                    sl.PID,
                    sl.ID_TEMP,
                    sl.PID_TEMP,
                    sl.IDKSG,
                    sl.MEK_COMENT,
                    sl.MEE_COMENT,
                    sl.EKMP_COMENT,
                    sl.REMEK_COM,
                    sl.MEE_TYPE,
                    sl.KSG_OPLATA,
                    sl.KDAY,
                    sl.REQUEST_DATE,
                    sl.USERID,
                    //FIODR = pa.FAM.ToUpper() + " " + pa.IM.ToUpper() + pa.OT + pa.DR.Value.ToString("dd.MM.yyyy"),
                    pa.FAM,
                    pa.IM,
                    pa.OT,
                    pa.W,
                    pa.DR,
                    pa.DOST,
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
                    sl.MSPID,
                    sl.VID_HMP,
                    sl.METOD_HMP,
                    sl.DIFF_K,
                    sl.MEK_COUNT,
                    sl.MEE_COUNT,
                    sl.EKMP_COUNT,
                    sl.RMEK_COUNT,
                    sl.RMEE_COUNT,
                    sl.REKMP_COUNT,
                    sl.KSG_COM,
                    sl.KDAY_COM,
                    sl.DIFF_COM,
                    sl.VOZR,
                    sl.IDSLG
                };

            _linqInstantFeedbackDataSource.QueryableSource = pQueryable;
        }
        void BindData2()
        {
            IQueryable pQueryable = from sl in _ElmedDataClassesDataContext.SLUCH1
                join pa in _ElmedDataClassesDataContext.PACIENT1 on sl.PID equals pa.ID
                join sc in _ElmedDataClassesDataContext.D3_SCHET_OMS on sl.SCHET_ID equals sc.ID
                where (sl.USL_OK == 1 || sl.USL_OK == 2)
                select new
                {
                    sc.YEAR,
                    sc.MONTH,
                    sl.ID,
                    sl.IDCASE,
                    sl.USL_OK,
                    sl.VIDPOM,
                    sl.NPR_MO,
                    sl.EXTR,
                    sl.LPU,
                    sl.LPU_1,
                    sl.PODR,
                    sl.PROFIL,
                    sl.DET,
                    sl.NHISTORY,
                    sl.DATE_1,
                    sl.DATE_2,
                    sl.DS0,
                    sl.DS1,
                    sl.DS2,
                    sl.CODE_MES1,
                    sl.CODE_MES2,
                    sl.SLUCH_TYPE,
                    sl.RSLT,
                    sl.ISHOD,
                    sl.PRVS,
                    sl.IDDOKT,
                    sl.OS_SLUCH,
                    sl.OS_SLUCH_REGION,
                    sl.IDSP,
                    sl.ED_COL,
                    sl.TARIF,
                    sl.SUMV,
                    sl.OPLATA,
                    sl.SUMP,
                    sl.SANK_IT,
                    sl.COMENTSL,
                    sl.SCHET_ID,
                    sl.GR_ZDOROV,
                    sl.VETERAN,
                    sl.SCHOOL,
                    sl.WORK_STAT,
                    sl.PID,
                    sl.ID_TEMP,
                    sl.PID_TEMP,
                    sl.IDKSG,
                    sl.MEE_COMENT,
                    sl.EKMP_COMENT,
                    sl.REMEK_COM,
                    sl.MEE_TYPE,
                    sl.KSG_OPLATA,
                    sl.KDAY,
                    sl.REQUEST_DATE,
                    sl.USERID,
                    //FIODR = pa.FAM.ToUpper() + " " + pa.IM.ToUpper() + pa.OT + pa.DR.Value.ToString("dd.MM.yyyy"),
                    pa.FAM,
                    pa.IM,
                    pa.OT,
                    pa.W,
                    pa.DR,
                    pa.DOST,
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
                    sl.MSPID,
                    sl.VID_HMP,
                    sl.METOD_HMP,
                    sl.DIFF_K,
                    sl.MEK_COUNT,
                    sl.MEE_COUNT,
                    sl.EKMP_COUNT,
                    sl.RMEK_COUNT,
                    sl.RMEE_COUNT,
                    sl.REKMP_COUNT,
                    sl.KSG_COM,
                    sl.KDAY_COM,
                    sl.DIFF_COM,
                    sl.VOZR,
                    sl.IDSLG
                };

            _linqInstantFeedbackDataSource.QueryableSource = pQueryable;
        }


        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
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

            //_zsls = null;

            _ElmedDataClassesDataContext.Dispose();
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            using (var dc = new YamedDataClassesDataContext(SprClass.LocalConnectionString))
            {
                Yamed_Users first = dc.Yamed_Users.Single(x => x.ID == SprClass.userId);
                writer.Write(first.LayRTable);
            }
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            if (stream.Length > 0)
                gridControl1.RestoreLayoutFromStream(stream);
            stream.Close();
            writer.Close();


            if (_schetId == null)
                BindData2();
            else
                BindData(_schetId.ID);
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
    }
}
