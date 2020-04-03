using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using Yamed.Control;
using Yamed.Control.Editors;
using Yamed.Core;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.Hospital
{
    /// <summary>
    /// Логика взаимодействия для PacientReserveControl.xaml
    /// </summary>
    public partial class ReestrControl1 : UserControl
    {
        private D3_SCHET_OMS _sc;

        public ReestrControl1(D3_SCHET_OMS sc)
        {
            InitializeComponent();

            _sc = sc;
            SchetRegisterGrid1._schetId = _sc;
        }

        public ReestrControl1()
        {
            InitializeComponent();
        }


        private void SluchAdd_OnClick(object sender, RoutedEventArgs e)
        {
            //Menu.Hide();
            //((Button)sender).IsEnabled = false;
            //Decorator.IsSplashScreenShown = true;


            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Случай стационар",
                MyControl = new HospitalEmrSluchTab(null, _sc),
                IsCloseable = "True",
                //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
            });

            //((Button)sender).IsEnabled = true;
        }

        private void Refresh_OnClick(object sender, RoutedEventArgs e)
        {
            SchetRegisterGrid1._linqInstantFeedbackDataSource.Refresh();
        }

        private void SluchEdit_OnClick(object sender, RoutedEventArgs e)
        {
            var tab = SchetRegisterGrid1;
            var sl = Reader2List.CustomSelect<SLUCH>($"Select * From SLUCH Where ID={ObjHelper.GetAnonymousValue(DxHelper.GetSelectedGridRow(tab.gridControl1), "ID")}",
                    SprClass.LocalConnectionString).Single();
            if (sl.OS_SLUCH_REGION == 22)
            {
                //СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                //{
                //    Header = "ДИСПАНСЕРИЗАЦИЯ",
                //    MyControl = new ElKardDispTab(sl, tab._schetId.ID),
                //    IsCloseable = "True"
                //});
            }
            else if (sl.USL_OK == 1 || sl.USL_OK == 2)
            {
                СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                {
                    Header = "СТАТИСТИЧЕСКАЯ КАРТА ВЫБЫВШЕГО ИЗ СТАЦИОНАРА",
                    MyControl = new HospitalEmrSluchTab(sl, tab._schetId),
                    IsCloseable = "True"
                });
            }
            else
            {
                var sprEditWindow = new UniSprEditControl("SLUCH", sl, true, SprClass.LocalConnectionString);
                var window = new DXWindow
                {
                    ShowIcon = false,
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Content = sprEditWindow,
                    Title = "Редактирование"
                };
                window.ShowDialog();
            }
        }

        private void SluchDel_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Удалить случай", "Удаление",
    MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var tab = SchetRegisterGrid1;
                var id = ObjHelper.GetAnonymousValue(DxHelper.GetSelectedGridRow(tab.gridControl1), "ID");
                var uid = Reader2List.CustomSelect<USL>($"Select TOP 1 * FROM USL WHERE SLID={id}", SprClass.LocalConnectionString).SingleOrDefault()?.ID;
                if (uid != null)
                    Reader2List.CustomExecuteQuery($"DELETE USL_ASSIST WHERE UID={uid}", SprClass.LocalConnectionString);
                Reader2List.CustomExecuteQuery($"DELETE USL WHERE SLID={id}", SprClass.LocalConnectionString);
                Reader2List.CustomExecuteQuery($"DELETE SLUCH_DS2 WHERE SLID={id}", SprClass.LocalConnectionString);
                Reader2List.CustomExecuteQuery($"DELETE SLUCH_DS3 WHERE SLID={id}", SprClass.LocalConnectionString);
                Reader2List.CustomExecuteQuery($"DELETE SLUCH WHERE ID={id}", SprClass.LocalConnectionString);
                tab._linqInstantFeedbackDataSource.Refresh();

            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ((Button) sender).IsEnabled = false;
            var tab = SchetRegisterGrid1;
            DxHelper.GetSelectedGridRowsAsync(ref tab.gridControl1);
            bool isLoaded = false;

            //TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext(); //get UI thread context 
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        if (SchetRegisterGrid1.gridControl1.IsAsyncOperationInProgress == false)
                        {
                            isLoaded = true;
                        }
                    });
                    if (isLoaded) break;
                    Thread.Sleep(200);
                }

                for (int index = 0; index < DxHelper.LoadedRows.Count; index++)
                {
                    var row = DxHelper.LoadedRows[index];
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
where tf.slid = {ObjHelper.GetAnonymousValue(row, "ID")}
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
where tf.slid = {ObjHelper.GetAnonymousValue(row, "ID")}
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

                    var traffiList =
                        Reader2List.CustomSelect<USL>(
                            $"Select * from USL where SLID = {ObjHelper.GetAnonymousValue(row, "ID")} and CODE_USL LIKE 'TF%'",
                            SprClass.LocalConnectionString);
                    foreach (var k in (IList)ksg)
                    {
                        var tf =
                            traffiList.SingleOrDefault(x => x.ID == (int)ObjHelper.GetAnonymousValue(k, "ID"));
                        if (tf != null)
                        {
                            tf.KDAY = (int?)ObjHelper.GetAnonymousValue(k, "KDAY");
                            tf.SUMV_USL = (decimal?)ObjHelper.GetAnonymousValue(k, "SUMV");
                            tf.TARIF = (decimal?)ObjHelper.GetAnonymousValue(k, "TARIF");
                            tf.IDKSG = (string)ObjHelper.GetAnonymousValue(k, "IDKSG");
                        }
                    }

                    if (traffiList.Count(x => x.IDKSG == "1565" || x.IDKSG == "1567" || x.IDKSG == "1568") > 1)
                    {
                        string[] dss = new[] {"O14.1", "O34.2", "O36.3", "O36.4", "O42.2"};
                        var tf = traffiList.SingleOrDefault(x => x.IDKSG == "1565");
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

                    if (traffiList.Count(x => x.IDKSG == "st02.001" || x.IDKSG == "st02.003" || x.IDKSG == "st02.004") > 1)
                    {
                        string[] dss = new[] { "O14.1", "O34.2", "O36.3", "O36.4", "O42.2" };
                        var tf = traffiList.SingleOrDefault(x => x.IDKSG == "st02.001");
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

                    //if (traffiList.Count(x => x.IDKSG == 1126 || x.IDKSG == 1129 || x.IDKSG == 700 || x.IDKSG == 703) > 1)
                    //{
                    //    var tf = traffiList.Single(x => x.IDKSG == 1126 || x.IDKSG == 700);
                    //    var day = tf.DATE_OUT - tf.DATE_IN;
                    //    if (day?.Days < 3)
                    //    {
                    //        tf.IDKSG = null;
                    //        tf.SUMV_USL = 0;
                    //        tf.TARIF = 0;
                    //    }

                    //    if (day?.Days >= 3 && day?.Days < 7)
                    //    {
                    //        tf.IDKSG = null;
                    //        tf.SUMV_USL = 0;
                    //        tf.TARIF = 0;
                    //        traffiList.Single(x => x.IDKSG == 1129 || x.IDKSG == 703).DIFF_K = (decimal)1.40;
                    //    }
                    //}

                    foreach (var tr in traffiList)
                    {
                        Reader2List.CustomExecuteQuery(Reader2List.CustomUpdateCommand("USL", tr, "ID"),
                            SprClass.LocalConnectionString);
                    }
                    var i = index;

                }

                Reader2List.CustomExecuteQuery(
$@"
					Update SLUCH 
                    SET TARIF = 0
                        ,SUMV = 0
                        
                    from SLUCH sl 
                    JOIN PACIENT pa on sl.PID = pa.ID
                    where sl.SCHET_ID = { _sc.ID} and sl.USL_OK in (1,2)
", SprClass.LocalConnectionString);



                Reader2List.CustomExecuteQuery(
                    $@"
                        UPDATE sl SET SUMV = slsum, TARIF = sltarif, ED_COL = slcnt
                        FROM SLUCH sl
                        Join (
                        SELECT sl.ID, sum(tf.SUMV_USL) slsum, max(tf.TARIF) sltarif, count(tf.ID) slcnt 
                        FROM SLUCH sl
                        join pacient pa on sl.PID = pa.ID
                        JOIN USL tf on sl.ID = tf.SLID and (tf.SUMV_USL > 0)
                        where sl.SCHET_ID = { _sc.ID} and (sl.VOPL = 1 or sl.VOPL is NULL) and sl.USL_OK in (1,2) and sl.DATE_2 >= '20160101'
                        GROUP BY sl.ID) tt on sl.ID = tt.ID
", SprClass.LocalConnectionString);


            }).ContinueWith(lr =>
            {
                DXMessageBox.Show("Расчет КСГ завершен");
                ((Button)sender).IsEnabled = true;

            }, TaskScheduler.FromCurrentSynchronizationContext());

        }
    }
}
