using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yamed.Server;

namespace Yamed.Oms
{
    /*
    class old_calc
    {
        Reader2List.CustomExecuteQuery($@"
                        UPDATE zsl SET SUMV = NULL
						FROM D3_ZSL_OMS zsl 
                        where zsl.D3_SCID = {sc.ID} and zsl.USL_OK in (3,4)
", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
                        UPDATE sl SET SUM_M = NULL, TARIF = NULL
						FROM D3_ZSL_OMS zsl 
                        JOIN D3_SL_OMS sl on sl.D3_ZSLID = zsl.ID
                        where zsl.D3_SCID = {sc.ID} and zsl.USL_OK in (3,4)
", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
                        UPDATE SLUCH SET SUMV = NULL, TARIF = NULL
                        where SCHET_ID = { sc.ID} and USL_OK in (1,2)", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
                        EXEC [dbo].[p_fix] { sc.ID}", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
update zs set idsp = tt.idsp
from [D3_ZSL_OMS] zs
join (
select distinct zs.id,
     case
when zs.USL_OK = 3 and sl.p_cel25 = '1.0' and ltrim(rtrim(p.smo)) not like '46%' then 29
when zs.USL_OK = 3 and sl.p_cel25 = '1.1' and ltrim(rtrim(p.smo)) not like '46%' then 29
when zs.USL_OK = 3 and sl.p_cel25 = '1.2' and ltrim(rtrim(p.smo)) not like '46%' then 29
when zs.USL_OK = 3 and sl.p_cel25 = '1.3' and ltrim(rtrim(p.smo)) not like '46%' then 29
when zs.USL_OK = 3 and sl.p_cel25 = '2.1' and ltrim(rtrim(p.smo)) not like '46%' then 29
when zs.USL_OK = 3 and sl.p_cel25 = '2.2' and ltrim(rtrim(p.smo)) not like '46%' then 30
when zs.USL_OK = 3 and sl.p_cel25 = '2.3' and ltrim(rtrim(p.smo)) not like '46%' then 29
when zs.USL_OK = 3 and sl.p_cel25 = '2.5' and ltrim(rtrim(p.smo)) not like '46%' then 29
when zs.USL_OK = 3 and sl.p_cel25 = '2.6' and ltrim(rtrim(p.smo)) not like '46%' then 29
when zs.USL_OK = 3 and sl.p_cel25 = '3.0' and ltrim(rtrim(p.smo)) not like '46%' then 30
when zs.USL_OK = 3 and sl.p_cel25 = '3.1' and ltrim(rtrim(p.smo)) not like '46%' then 30

when zs.USL_OK = 3 and sl.p_cel25 = '1.0' and ltrim(rtrim(p.smo)) like '46%' then 25
when zs.USL_OK = 3 and sl.p_cel25 = '1.1' and ltrim(rtrim(p.smo)) like '46%' then 29
when zs.USL_OK = 3 and sl.p_cel25 = '1.2' and ltrim(rtrim(p.smo)) like '46%' then 25
when zs.USL_OK = 3 and sl.p_cel25 = '1.3' and ltrim(rtrim(p.smo)) like '46%' then 25
when zs.USL_OK = 3 and sl.p_cel25 = '2.1' and ltrim(rtrim(p.smo)) like '46%' then 25
when zs.USL_OK = 3 and sl.p_cel25 = '2.2' and ltrim(rtrim(p.smo)) like '46%' then 25
when zs.USL_OK = 3 and sl.p_cel25 = '2.3' and ltrim(rtrim(p.smo)) like '46%' then 25
when zs.USL_OK = 3 and sl.p_cel25 = '2.5' and ltrim(rtrim(p.smo)) like '46%' then 25
when zs.USL_OK = 3 and sl.p_cel25 = '2.6' and ltrim(rtrim(p.smo)) like '46%' then 25
when zs.USL_OK = 3 and sl.p_cel25 = '3.0' and ltrim(rtrim(p.smo)) like '46%' then 25
when zs.USL_OK = 3 and sl.p_cel25 = '3.1' and ltrim(rtrim(p.smo)) like '46%' then 25

when zs.USL_OK = 4 and ltrim(rtrim(p.smo)) not like '46%' then 24
when zs.USL_OK = 4 and FOR_POM <> 1 and ltrim(rtrim(p.smo)) like '46%' then 24
when zs.USL_OK = 4 and FOR_POM = 1 and ltrim(rtrim(p.smo)) like '46%' then 36
	 end idsp
FROM [D3_SCHET_OMS] sch
    inner join [D3_PACIENT_OMS] p on sch.id=p.[D3_SCID]
    inner join [D3_ZSL_OMS] zs on p.id=zs.[D3_PID] and zs.usl_ok in (3,4) and (os_sluch_region is null or OS_SLUCH_REGION in (8,12,14,16,18,23,48))
	join D3_SL_OMS sl on zs.ID = sl.d3_zslid
	left join MO_podush pod on zs.lpu = pod.KOD_MO
				   where zs.D3_SCID = {sc.ID} and pod.KOD_MO is not null
) as tt on tt.id = zs.id
", SprClass.LocalConnectionString);



            Reader2List.CustomExecuteQuery($@"
UPDATE D3_ZSL_OMS SET VOZR =
                             (CASE
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 0 THEN 'G00.M00'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 1 THEN 'G00.M01'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 2 THEN 'G00.M02'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 3 THEN 'G00.M03'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 4 THEN 'G00.M04'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 5 THEN 'G00.M05'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 6 THEN 'G00.M06'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 7 THEN 'G00.M07'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 8 THEN 'G00.M08'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 9 THEN 'G00.M09'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 10 THEN 'G00.M10'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 11 THEN 'G00.M11'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 12 THEN 'G00.M12'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 13 THEN 'G00.M12'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 14 THEN 'G00.M12'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 15 THEN 'G01.M03'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 16 THEN 'G01.M03'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 17 THEN 'G01.M03'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 18 THEN 'G01.M06'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 19 THEN 'G01.M06'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 19 THEN 'G01.M06'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 20 THEN 'G01.M06'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 21 THEN 'G01.M06'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 22 THEN 'G01.M06'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 23 THEN 'G01.M06'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 24 THEN 'G02.M00'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 25 THEN 'G02.M00'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 26 THEN 'G02.M00'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 27 THEN 'G02.M00'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 28 THEN 'G02.M00'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 29 THEN 'G02.M00'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 30 THEN 'G02.M00'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 31 THEN 'G02.M00'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 32 THEN 'G02.M00'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 33 THEN 'G02.M00'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 34 THEN 'G02.M00'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 35 THEN 'G02.M00'
								WHEN dbo.GetDateDiff(dr, DATE_Z_2) = 36 THEN 'G03.M00'
								WHEN (year(DATE_Z_2) - year(dr)) = 3 THEN 'G03.M00'
								WHEN (year(DATE_Z_2) - year(dr)) = 4 THEN 'G04.M00'
								WHEN (year(DATE_Z_2) - year(dr)) = 5 THEN 'G05.M00'
								WHEN (year(DATE_Z_2) - year(dr)) = 6 THEN 'G06.M00'
								WHEN (year(DATE_Z_2) - year(dr)) = 7 THEN 'G07.M00'
								WHEN (year(DATE_Z_2) - year(dr)) = 8 THEN 'G08.M00'
								WHEN (year(DATE_Z_2) - year(dr)) = 9 THEN 'G09.M00'
								WHEN (year(DATE_Z_2) - year(dr)) = 10 THEN 'G10.M00'
								WHEN (year(DATE_Z_2) - year(dr)) = 11 THEN 'G11.M00'
								WHEN (year(DATE_Z_2) - year(dr)) = 12 THEN 'G12.M00'
								WHEN (year(DATE_Z_2) - year(dr)) = 13 THEN 'G13.M00'
								WHEN (year(DATE_Z_2) - year(dr)) = 14 THEN 'G14.M00'
								WHEN (year(DATE_Z_2) - year(dr)) = 15 THEN 'G15.M00'
								WHEN (year(DATE_Z_2) - year(dr)) = 16 THEN 'G16.M00'
								WHEN (year(DATE_Z_2) - year(dr)) = 17 THEN 'G17.M00'
								END)														
					from D3_ZSL_OMS zsl 
                    JOIN D3_PACIENT_OMS pa on zsl.D3_PID = pa.ID
                    where zsl.D3_SCID = {sc.ID} and OS_SLUCH_REGION = 11 and VOZR is null
", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
 Update USL SET
					    KDAY = 
							(CASE 
							  WHEN sl.USL_OK = 1 THEN (CASE WHEN u.DATE_IN = u.DATE_OUT THEN 1 ELSE DATEDIFF(DAY, u.DATE_IN, u.DATE_OUT) END)
							  WHEN sl.USL_OK = 2 THEN DATEDIFF(DAY, u.DATE_IN, u.DATE_OUT) + 1 -
								(SELECT COUNT(*) FROM dbo.WORK_DAY wd WHERE wd.LPU = u.LPU and wd.PODR_ID = u.LPU_1 and H_DATE BETWEEN u.DATE_IN AND u.DATE_OUT)
                            END),
						SUMV_USL= 
                            CAST(
                            	ROUND(vmp.TARIF*ISNULL(u.KOL_USL, 1) ,2)
                            AS NUMERIC (15 ,2)),
                        TARIF=
                            CAST(
                            	ROUND(ISNULL(vmp.TARIF, 0) ,2)
                            AS NUMERIC (15 ,2))
                    from USL u
                    join sluch sl on u.SLID = sl.ID
                    JOIN PACIENT pa on sl.PID = pa.ID
                    join V019 v on sl.METOD_HMP = v.IDHM and (sl.DATE_2 >= v.DATEBEG and (sl.DATE_2 < v.DATEEND +1 or v.DATEEND is null))
                    join dbo.CalcVmpTarif vmp ON vmp.IDGR = v.HGR and (sl.DATE_2 >= vmp.TBEG and sl.DATE_2 < vmp.TEND +1) 
                    where sl.SCHET_ID = { sc.ID} and u.CODE_USL Like 'TF%' and sl.USL_OK in (1,2) and sl.VID_HMP is NOT NULL
", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
update usl set
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
									    WHEN tf.IDKSG in ('1634', '1655', '1656', 'st13.003') and DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) < 4
											THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.80, 2)
									    WHEN tf.IDKSG in ('1634', '1655', '1656', 'st13.003') and DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) >= 4
											THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 1.00, 2)

                            			WHEN DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) < 4
											THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.20, 2)
										ELSE ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.50, 2)
									END)
								WHEN (sl.RSLT not in (102, 105, 106, 107, 108, 110)) THEN 
                            		(CASE 
									    WHEN tf.IDKSG in ('1634', '1655', '1656', 'st13.003') and DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) < 4
											THEN ROUND(t2.TARIF*ISNULL(upr.UPR,1.00)*(CASE WHEN tf.IDKSG in (select [IDKSG] from [dbo].[CalNotKus] Where tf.DATE_OUT >= TBEG and tf.DATE_OUT < TEND +1) THEN 1.00 ELSE ISNULL(kf.KUS,1.00) END)*ISNULL(tf.DIFF_K,1.00) * 0.80, 2)
									    WHEN tf.IDKSG in ('1634', '1655', '1656', 'st13.003') and DATEDIFF(DAY, tf.DATE_IN, tf.DATE_OUT) >= 4
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
                        join SLUCH sl on tf.SLID = sl.ID
                        left join SLUCH_DS2 ds2 on sl.ID = ds2.slid
                        join pacient pa on sl.PID = pa.ID
                        join SprKsg t on tf.IDKSG = t.ID
                        join CalcKsgTarif t2 on t.ID = t2.IDKSG  and (tf.DATE_OUT >= t2.DBEG and tf.DATE_OUT < t2.DEND +1)
                        left join KSG_SOD as ksgsod on (tf.DATE_OUT >= ksgsod.TarifDateStart and tf.DATE_OUT < ksgsod.TarifDateEnd +1)
                        left join CalcUprk upr on t.ID = upr.IDKSG and (tf.DATE_OUT >= upr.TBEG and tf.DATE_OUT < upr.TEND +1)
						--left join @CalcBaseKoefDS bkds on (tf.DATE_OUT >= bkds.TBEG and tf.DATE_OUT < bkds.TEND +1)
                        left join CalcMok as kf on kf.KOD_LPU = (SELECT TOP 1 [Parametr] FROM [Settings] WHERE NAME = 'MedicalOrganization') AND (tf.DATE_OUT >= kf.DATESTART and (kf.DATEEND is NULL OR tf.DATE_OUT < kf.DATEEND +1))
                    where sl.SCHET_ID ={ sc.ID} and tf.CODE_USL Like 'TF%' and sl.USL_OK in (1,2) and sl.VID_HMP is NULL 
", SprClass.LocalConnectionString);


            Reader2List.CustomExecuteQuery(
                $@"
                        UPDATE sl SET SUMV = slsum, TARIF = sltarif, ED_COL = slcnt
                        FROM SLUCH sl
                        Join (
                        SELECT sl.ID, sum(tf.SUMV_USL) slsum, max(tf.TARIF) sltarif, count(tf.ID) slcnt 
                        FROM SLUCH sl
                        join PACIENT pa on sl.PID = pa.ID
                        JOIN USL tf on sl.ID = tf.SLID and (tf.SUMV_USL > 0)
                        where sl.SCHET_ID = { sc.ID} and sl.USL_OK in (1,2) and sl.DATE_2 >= '20160101'
                        GROUP BY sl.ID) tt on sl.ID = tt.ID
", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
					Update D3_SL_OMS 
                    SET ED_COL = st.uet_sum
					from D3_SL_OMS sl
					join (Select 
					sum(
					 case
						when FLOOR(DATEDIFF(DAY,DR,DATE_2)/365.25) < 18 Then ISNULL(st.Child * isnull(u.KOL_USL, 1), 0)
						else ISNULL(st.Adult * ISNULL(u.KOL_USL, 1), 0)
					 end) uet_sum, sl.ID
                    from D3_ZSL_OMS zsl 
					JOIN D3_PACIENT_OMS pa on zsl.D3_PID = pa.ID
					Join D3_SL_OMS sl on zsl.ID = sl.D3_ZSLID and zsl.USL_OK = 3
					join D3_USL_OMS u on sl.ID = u.D3_SLID
					join [dbo].[CalcStomatUet] st on u.VID_VME = st.Code 
					GROUP BY sl.ID) st on sl.ID = st.ID
                    ", SprClass.LocalConnectionString);


            Reader2List.CustomExecuteQuery($@"
					Update D3_SL_OMS 
                    SET 
                        TARIF= 
                            (CASE 
								WHEN (sl.P_CEL25 = '1.0' )  THEN t.tarif1
								WHEN (sl.P_CEL25 = '1.1' )  THEN t.tarif2
								WHEN (sl.P_CEL25 = '1.2' )  THEN t.tarif3
								WHEN (sl.P_CEL25 = '1.3' )  THEN t.tarif4
								WHEN (sl.P_CEL25 = '2.1' )  THEN t.tarif5
								WHEN (sl.P_CEL25 = '2.2' )  THEN t.tarif6
								WHEN (sl.P_CEL25 = '2.3' )  THEN t.tarif7
								WHEN (sl.P_CEL25 = '2.5' )  THEN t.tarif8
								WHEN (sl.P_CEL25 = '2.6' )  THEN t.tarif9
								WHEN (sl.P_CEL25 = '3.0' )  THEN t.tarif10
								WHEN (sl.P_CEL25 = '3.1' )  THEN t.tarif11
								ELSE 0.00
                            END),
                        SUM_M=
                            (CASE 
								WHEN (sl.P_CEL25 = '1.0' and sl.PROFIL not in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) * t.tarif1 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '1.1' and sl.PROFIL not in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) * t.tarif2 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '1.2' and sl.PROFIL not in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) * t.tarif3 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '1.3' and sl.PROFIL not in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) * t.tarif4 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '2.1' and sl.PROFIL not in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) * t.tarif5 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '2.2' and sl.PROFIL not in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) * t.tarif6 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '2.3' and sl.PROFIL not in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) * t.tarif7 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '2.5' and sl.PROFIL not in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) * t.tarif8 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '2.6' and sl.PROFIL not in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) * t.tarif9 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '3.0' and sl.PROFIL not in (63,85,86,87,88,89,90,171))  THEN 0.00  --ROUND(isnull(sl.ED_COL, 1.00) * t.tarif10 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '3.1' and sl.PROFIL not in (63,85,86,87,88,89,90,171))  THEN 0.00  --ROUND(isnull(sl.ED_COL, 1.00) * t.tarif11 * ISNULL(kf.KZMP, 1.00), 2)

								WHEN (sl.P_CEL25 = '1.0' and sl.PROFIL in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) / 4 * t.tarif1 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '1.1' and sl.PROFIL in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) / 4 * t.tarif2 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '1.2' and sl.PROFIL in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) / 4 * t.tarif3 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '1.3' and sl.PROFIL in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) / 4 * t.tarif4 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '2.1' and sl.PROFIL in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) / 4 * t.tarif5 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '2.2' and sl.PROFIL in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) / 4 * t.tarif6 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '2.3' and sl.PROFIL in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) / 4 * t.tarif7 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '2.5' and sl.PROFIL in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) / 4 * t.tarif8 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '2.6' and sl.PROFIL in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) / 4 * t.tarif9 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '3.0' and sl.PROFIL in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) / 12 * t.tarif10 * ISNULL(kf.KZMP, 1.00), 2)
								WHEN (sl.P_CEL25 = '3.1' and sl.PROFIL in (63,85,86,87,88,89,90,171))  THEN ROUND(isnull(sl.ED_COL, 1.00) / 12 * t.tarif11 * ISNULL(kf.KZMP, 1.00), 2)

                                ELSE 0.00
                            END)
                    from D3_ZSL_OMS zsl 
					JOIN D3_PACIENT_OMS pa on zsl.D3_PID = pa.ID
					Join D3_SL_OMS sl on zsl.ID = sl.D3_ZSLID and zsl.USL_OK = 3
                    join (Select * From CalcAmbTarif where OS_SLUCH is null and USL_OK = 3) as t on sl.Profil = t.Profil
					and (zsl.DATE_Z_2 >= t.TBEG and zsl.DATE_Z_2 < t.TEND +1) 
                    left join CalcMok as kf on kf.KOD_LPU = zsl.LPU AND (zsl.DATE_Z_2 >= kf.DATESTART and (kf.DATEEND is NULL OR zsl.DATE_Z_2 < kf.DATEEND +1))
                    where zsl.D3_SCID = {sc.ID} and zsl.OS_SLUCH_REGION is null", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
					Update D3_SL_OMS SET
                         ED_COL = ISNULL(ED_COL, 1.00),
                         TARIF = t.tarif1,
                         SUM_M = ROUND(isnull(sl.ED_COL, 1.00) * t.tarif1 * ISNULL(kf.KZMP, 1.00), 2)

                    from D3_ZSL_OMS zsl 
                    JOIN D3_PACIENT_OMS pa on zsl.D3_PID = pa.ID
					join D3_SL_OMS sl on zsl.ID = sl.D3_ZSLID --and zsl.USL_OK =3
                    join CalcAmbTarif t on zsl.OS_SLUCH_REGION = t.OS_SLUCH and (sl.DATE_2 >= t.TBEG and sl.DATE_2 < t.TEND +1) 
                    left join CalcMok as kf on kf.KOD_LPU = zsl.LPU AND (zsl.DATE_Z_2 >= kf.DATESTART and (kf.DATEEND is NULL OR zsl.DATE_Z_2 < kf.DATEEND +1))
                    where zsl.D3_SCID = { sc.ID} and 
					 OS_SLUCH_REGION = 6", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
                        UPDATE D3_ZSL_OMS SET SUMV = slsum
                        FROM D3_ZSL_OMS zsl
                        Join (
                        SELECT zsl.ID, sum(sl.SUM_M) slsum, count(sl.ID) slcnt 
                        FROM D3_SL_OMS sl
						JOIN D3_ZSL_OMS zsl on sl.D3_ZSLID = zsl.ID and zsl.D3_SCID = {sc.ID} and (zsl.OS_SLUCH_REGION is NULL or zsl.OS_SLUCH_REGION = 6)
                        where zsl.USL_OK in (3,4)
                        GROUP BY zsl.ID) tt on zsl.ID = tt.ID
", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
					Update D3_ZSL_OMS SET
                        SUMV=
                            (CASE 
                                WHEN (zsl.FOR_POM = 1 ) THEN ROUND(isnull(sl.ED_COL, 1.00) * t.tarif1 * ISNULL(kf.KZMP, 1.00), 2)
                                WHEN (zsl.FOR_POM = 2 ) THEN ROUND(isnull(sl.ED_COL, 1.00) * t.tarif2 * ISNULL(kf.KZMP, 1.00), 2)
                                ELSE 0.00
                            END)
                    from D3_ZSL_OMS zsl 
					JOIN D3_PACIENT_OMS pa on zsl.D3_PID = pa.ID
					Join D3_SL_OMS sl on zsl.ID = sl.D3_ZSLID and zsl.USL_OK = 4
                    join (Select * From CalcAmbTarif where OS_SLUCH is null and USL_OK = 4) as t on --sl.Profil = t.Profil and
                        (zsl.DATE_Z_2 >= t.TBEG and zsl.DATE_Z_2 < t.TEND +1) 
                    left join CalcMok as kf on kf.KOD_LPU = zsl.LPU AND (zsl.DATE_Z_2 >= kf.DATESTART and (kf.DATEEND is NULL OR zsl.DATE_Z_2 < kf.DATEEND +1))
                    where zsl.D3_SCID = {sc.ID} and zsl.OS_SLUCH_REGION is null

update sl set sum_m = tt1.sumv, tarif = tt1.tarif
from d3_sl_oms sl
join ( select case when RN = 1 then sumv else 0.00 end sumv, sumv tarif, ID from
(
select ROW_NUMBER() OVER (PARTITION BY zsl.ID  ORDER BY sl.date_2 desc) RN,
sl.*, zsl.sumv
from D3_ZSL_OMS zsl
join d3_sl_oms sl on zsl.ID = sl.D3_ZSLID and zsl.USL_OK = 4
join d3_schet_oms sc on zsl.D3_SCID = sc.ID
where zsl.D3_SCID = {sc.ID} and zsl.OS_SLUCH_REGION is null
) tt --where tt.RN = 1
) tt1 on sl.ID = tt1.ID


", SprClass.LocalConnectionString);


            Reader2List.CustomExecuteQuery($@"
					Update D3_ZSL_OMS SET
                        SUMV=
                            (CASE 
                                WHEN sl.P_CEL25 = '3.0' THEN ROUND(isnull(sl.ED_COL, 1.00) * t.tarif10 * ISNULL(kf.KZMP, 1.00), 2)
                                WHEN sl.P_CEL25 = '3.1' THEN ROUND(isnull(sl.ED_COL, 1.00) * t.tarif11 * ISNULL(kf.KZMP, 1.00), 2)
                                ELSE 0.00
                            END)
                    from D3_ZSL_OMS zsl 
					JOIN D3_PACIENT_OMS pa on zsl.D3_PID = pa.ID
					Join D3_SL_OMS sl on zsl.ID = sl.D3_ZSLID and zsl.USL_OK = 3 and sl.P_CEL25 in ('3.0', '3.1') and sl.PROFIL not in (63,85,86,87,88,89,90,171)
                    join (Select * From CalcAmbTarif where OS_SLUCH is null and USL_OK = 3) as t on sl.Profil = t.Profil
					and (zsl.DATE_Z_2 >= t.TBEG and zsl.DATE_Z_2 < t.TEND +1) 
                    left join CalcMok as kf on kf.KOD_LPU = zsl.LPU AND (zsl.DATE_Z_2 >= kf.DATESTART and (kf.DATEEND is NULL OR zsl.DATE_Z_2 < kf.DATEEND +1))
                    where zsl.D3_SCID = {sc.ID} and zsl.OS_SLUCH_REGION is null

update sl set sum_m = tt1.sumv, tarif = tt1.tarif
from d3_sl_oms sl
join ( select case when RN = 1 then sumv else 0.00 end sumv, sumv tarif, ID from
(
select ROW_NUMBER() OVER (PARTITION BY zsl.ID  ORDER BY sl.date_2 desc) RN,
sl.*, zsl.sumv
from D3_ZSL_OMS zsl
join d3_sl_oms sl on zsl.ID = sl.D3_ZSLID and zsl.USL_OK = 3 and sl.P_CEL25 in ('3.0', '3.1') and sl.PROFIL not in (63,85,86,87,88,89,90,171)
join d3_schet_oms sc on zsl.D3_SCID = sc.ID
where zsl.D3_SCID = {sc.ID} and zsl.OS_SLUCH_REGION is null
) tt --where tt.RN = 1
) tt1 on sl.ID = tt1.ID

", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
                Update D3_ZSL_OMS SET 
				SUMV =
                            (CASE
                                WHEN VOZR in ('G00.M00', 'G00.M02', 'G00.M04', 'G00.M05', 'G00.M06','G00.M07', 'G00.M08', 'G00.M09', 'G00.M10', 'G00.M11', 'G01.M03', 'G01.M06') THEN t.tarif1
                                WHEN VOZR in ('G00.M03','G02.M00', 'G04.M00', 'G05.M00', 'G08.M00','G09.M00','G11.M00','G12.M00')  THEN t.tarif2
                                WHEN VOZR in ('G13.M00', 'G14.M00')  THEN t.tarif3
                                --WHEN VOZR in ('G00.M01', 'G00.M12', 'G07.M00') THEN t.tarif4
                                WHEN VOZR in ('G00.M01', 'G07.M00') THEN t.tarif4
                                --WHEN VOZR in ('G10.M00')  THEN t.tarif5
                                WHEN VOZR in ('G10.M00', 'G00.M12')  THEN t.tarif5
                                WHEN VOZR in ('G03.M00')  THEN t.tarif6
                                WHEN VOZR in ('G06.M00')  THEN t.tarif7
                                WHEN VOZR in ('G15.M00', 'G16.M00', 'G17.M00')  THEN t.tarif8
                            END)
                from D3_ZSL_OMS zsl
				JOIN D3_PACIENT_OMS pa on zsl.D3_PID = pa.ID
                join CalcAmbTarif t on zsl.OS_SLUCH_REGION = t.OS_SLUCH and (zsl.DATE_Z_2 >= t.TBEG and zsl.DATE_Z_2 < t.TEND + 1)
                    where zsl.D3_SCID = { sc.ID } and OS_SLUCH_REGION = 11

update sl set sum_m = tt1.sumv, tarif = tt1.tarif
from d3_sl_oms sl
join ( select case when RN = 1 then sumv else 0.00 end sumv, sumv tarif, ID from
(
select ROW_NUMBER() OVER (PARTITION BY zsl.ID  ORDER BY sl.date_2 desc) RN,
sl.*, zsl.sumv
from D3_ZSL_OMS zsl
join d3_sl_oms sl on zsl.ID = sl.D3_ZSLID
join d3_schet_oms sc on zsl.D3_SCID = sc.ID
where zsl.D3_SCID = { sc.ID } and OS_SLUCH_REGION = 11
) tt --where tt.RN = 1
) tt1 on sl.ID = tt1.ID
", SprClass.LocalConnectionString);


            Reader2List.CustomExecuteQuery($@"

declare @t15 table (
id int,
is15 int )

declare @t85 table (
id int, 
is85 int )

declare @tt table (
  id int,
  code_mo nvarchar(6),
  month int,
  W int,
  vozr int,
  OS_SLUCH_REGION int,
  [Real_Kol_Usl] int
  )


insert @t15
select t.id, 1--*,gr.OkazRanee
from
(select zs.id,p.w,
  year(zs.date_z_2)-year(p.dr) [vozr],
  zs.OS_SLUCH_REGION,count(u.id)as kol_vo_ok_ran
FROM [dbo].[D3_SCHET_OMS] sch
    inner join [dbo].[D3_PACIENT_OMS] p on sch.id=p.[D3_SCID]
                                and sch.id =  { sc.ID }  
    inner join [dbo].[D3_ZSL_OMS] zs on p.id=zs.[D3_PID]
                              and zs.OS_SLUCH_REGION=22
    inner join [dbo].[D3_SL_OMS] sl on zs.id=sl.[D3_ZSLID]
    inner join [dbo].[D3_USL_OMS] u on u.D3_SLID=sl.id
                              and u.[NPL]  in (4) --нужно ли????????
                              
group by zs.id  ,p.w,
  year(zs.date_z_2)-year(p.dr),
  zs.OS_SLUCH_REGION  ) t
  left join [dbo].[Kursk_DVN_1etap_Gr] Gr 
    on gr.[RegPrizn]=t.OS_SLUCH_REGION
    and gr.[Pol]=t.W
    AND charindex(cast(t.[vozr] as nvarchar)+',',Gr.Age)<>0   
where t.kol_vo_ok_ran>OkazRanee


insert @tt
select
  k.id,
  k.code_mo,
  k.month,
  K.W,
  K.[vozr],
  K.OS_SLUCH_REGION,
  count (*) [Real_Kol_Usl]
from
(
select
  sch.code_mo,
  sch.month,
  zs.id,
  p.w,
  year(zs.date_z_1) - year(p.dr)  [vozr],
  zs.OS_SLUCH_REGION,
  u.[VID_VME]
  ,DU.[Pol]
    ,DU.[Age]
FROM [dbo].[D3_SCHET_OMS] sch
    inner join [dbo].[D3_PACIENT_OMS] p on sch.id=p.[D3_SCID]
                                and sch.id =  { sc.ID } 
    inner join [dbo].[D3_ZSL_OMS] zs on p.id=zs.[D3_PID]
                              and zs.OS_SLUCH_REGION=22
    inner join [dbo].[D3_SL_OMS] sl on zs.id=sl.[D3_ZSLID]
    inner join [dbo].[D3_USL_OMS] u on u.D3_SLID=sl.id
                              and (u.[NPL] not in (1,2) --нужно ли????????
                                or u.[NPL] is null)
    inner join [dbo].[Kursk_DVN_1etap_Uslugi] du 
                      on du.[Pol]=p.w
                      --AND charindex(cast(dbo.f_GetAge (p.dr,zs.date_z_1) as nvarchar)+',',DU.Age)<>0
                                            AND charindex(cast(year(zs.date_z_2) - year(p.dr) as nvarchar(4))+',',DU.Age)<>0
                      and ltrim(rtrim(du.Code_Usl))=ltrim(rtrim(u.[VID_VME]))
)k
group by
  k.id,
  k.code_mo,
  k.month,
  K.W,
  K.[vozr],
  K.OS_SLUCH_REGION


 
insert @t85
select
  z.id, 1
  --z.*, gr.[StandartKolUsl], GR.IdGr
FROM @tt z LEFT JOIN [dbo].[Kursk_DVN_1etap_Gr] Gr 
    on gr.[RegPrizn]=z.OS_SLUCH_REGION
    and gr.[Pol]=z.W
    AND charindex(cast(z.[vozr] as nvarchar)+',',Gr.Age)<>0 
  inner join [dbo].[D3_ZSL_OMS] zs on z.id=zs.id
where z.[Real_Kol_Usl]<gr.[StandartKolUsl]-gr.OkazRanee

--select
                    Update D3_ZSL_OMS SET 
                    SUMV= 
                            (CASE 
                                WHEN t15.is15 is null and t85.is85 is null and (year(DATE_Z_2) - year(dr) in (21, 24, 27, 30, 33) AND W = 1 ) OR (year(DATE_Z_2) - year(dr) in (21, 24, 27) AND W = 2 ) THEN t.tarif1
                                WHEN t15.is15 is null and t85.is85 is null and (year(DATE_Z_2) - year(dr) in (36, 39, 42, 48, 54, 87, 90, 93, 96, 99) AND W = 1 ) OR (year(DATE_Z_2) - year(dr) in (87, 90, 93, 96, 99) AND W = 2 ) THEN t.tarif2
                                WHEN t15.is15 is null and t85.is85 is null and (year(DATE_Z_2) - year(dr) in (60,66,72,75,78,81,84) AND W = 1 ) OR (year(DATE_Z_2) - year(dr) in (72,75,78,81,84) AND W = 2 )THEN t.tarif3
                                WHEN t15.is15 is null and t85.is85 is null and (year(DATE_Z_2) - year(dr) in (30,33,36) AND W = 2 ) THEN t.tarif4
                                WHEN t15.is15 is null and t85.is85 is null and (year(DATE_Z_2) - year(dr) in (45,57) AND W = 1 ) THEN t.tarif5
                                WHEN t15.is15 is null and t85.is85 is null and (year(DATE_Z_2) - year(dr) in (63,66,69) AND W = 2 ) THEN t.tarif6
                                WHEN t15.is15 is null and t85.is85 is null and (year(DATE_Z_2) - year(dr) in (39,42) AND W = 2 ) THEN t.tarif7
                                WHEN t15.is15 is null and t85.is85 is null and (year(DATE_Z_2) - year(dr) in (63,69) AND W = 1 ) THEN t.tarif8
                                WHEN t15.is15 is null and t85.is85 is null and (year(DATE_Z_2) - year(dr) in (51) AND W = 1 ) THEN t.tarif9
                                WHEN t15.is15 is null and t85.is85 is null and (year(DATE_Z_2) - year(dr) in (45,48,51,54,57) AND W = 2 ) THEN t.tarif10
                                WHEN t15.is15 is null and t85.is85 is null and (year(DATE_Z_2) - year(dr) in (60) AND W = 2 ) THEN t.tarif11

                                WHEN t85.is85 = 1 and (year(DATE_Z_2) - year(dr) in (21, 24, 27, 30, 33) AND W = 1 ) OR (year(DATE_Z_2) - year(dr) in (21, 24, 27) AND W = 2 ) THEN 0
                                WHEN t85.is85 = 1 and (year(DATE_Z_2) - year(dr) in (36, 39, 42, 48, 54, 87, 90, 93, 96, 99) AND W = 1 ) OR (year(DATE_Z_2) - year(dr) in (87, 90, 93, 96, 99) AND W = 2 ) THEN 0
                                WHEN t85.is85 = 1 and (year(DATE_Z_2) - year(dr) in (60,66,72,75,78,81,84) AND W = 1 ) OR (year(DATE_Z_2) - year(dr) in (72,75,78,81,84) AND W = 2 )THEN 0
                                WHEN t85.is85 = 1 and (year(DATE_Z_2) - year(dr) in (30,33,36) AND W = 2 ) THEN 0
                                WHEN t85.is85 = 1 and (year(DATE_Z_2) - year(dr) in (45,57) AND W = 1 ) THEN 0
                                WHEN t85.is85 = 1 and (year(DATE_Z_2) - year(dr) in (63,66,69) AND W = 2 ) THEN 0
                                WHEN t85.is85 = 1 and (year(DATE_Z_2) - year(dr) in (39,42) AND W = 2 ) THEN 0
                                WHEN t85.is85 = 1 and (year(DATE_Z_2) - year(dr) in (63,69) AND W = 1 ) THEN 0
                                WHEN t85.is85 = 1 and (year(DATE_Z_2) - year(dr) in (51) AND W = 1 ) THEN 0
                                WHEN t85.is85 = 1 and (year(DATE_Z_2) - year(dr) in (45,48,51,54,57) AND W = 2 ) THEN 0
                                WHEN t85.is85 = 1 and (year(DATE_Z_2) - year(dr) in (60) AND W = 2 ) THEN 0

WHEN t15.is15 = 1 and (year(DATE_Z_2) - year(dr) in (21, 24, 27, 30, 33) AND W = 1 ) OR (year(DATE_Z_2) - year(dr) in (21, 24, 27) AND W = 2 ) THEN t.tarif12
                                WHEN t15.is15 = 1 and (year(DATE_Z_2) - year(dr) in (36, 39, 42, 48, 54, 87, 90, 93, 96, 99) AND W = 1 ) OR (year(DATE_Z_2) - year(dr) in (87, 90, 93, 96, 99) AND W = 2 ) THEN t.tarif12
                                WHEN t15.is15 = 1 and (year(DATE_Z_2) - year(dr) in (60,66,72,75,78,81,84) AND W = 1 ) OR (year(DATE_Z_2) - year(dr) in (72,75,78,81,84) AND W = 2 )THEN t.tarif12
                                WHEN t15.is15 = 1 and (year(DATE_Z_2) - year(dr) in (30,33,36) AND W = 2 ) THEN t.tarif12
                                WHEN t15.is15 = 1 and (year(DATE_Z_2) - year(dr) in (45,57) AND W = 1 ) THEN t.tarif12
                                WHEN t15.is15 = 1 and (year(DATE_Z_2) - year(dr) in (63,66,69) AND W = 2 ) THEN t.tarif12
                                WHEN t15.is15 = 1 and (year(DATE_Z_2) - year(dr) in (39,42) AND W = 2 ) THEN t.tarif12
                                WHEN t15.is15 = 1 and (year(DATE_Z_2) - year(dr) in (63,69) AND W = 1 ) THEN t.tarif12
                                WHEN t15.is15 = 1 and (year(DATE_Z_2) - year(dr) in (51) AND W = 1 ) THEN t.tarif12
                                WHEN t15.is15 = 1 and (year(DATE_Z_2) - year(dr) in (45,48,51,54,57) AND W = 2 ) THEN t.tarif12
                                WHEN t15.is15 = 1 and (year(DATE_Z_2) - year(dr) in (60) AND W = 2 ) THEN t.tarif12
                            END)

                from D3_ZSL_OMS zsl
        JOIN D3_PACIENT_OMS pa on zsl.D3_PID = pa.ID
                join CalcAmbTarif t on zsl.OS_SLUCH_REGION = t.OS_SLUCH and (zsl.DATE_Z_2 >= t.TBEG and zsl.DATE_Z_2 < t.TEND + 1)
        left join @t15 t15 on zsl.id = t15.id
        left join @t85 t85 on zsl.id = t85.id
                    where zsl.D3_SCID =  { sc.ID }   and 
          OS_SLUCH_REGION = 22

update sl set sum_m = tt1.sumv, tarif = tt1.tarif
from d3_sl_oms sl
join ( select case when RN = 1 then sumv else 0.00 end sumv, sumv tarif, ID from
(
select ROW_NUMBER() OVER (PARTITION BY zsl.ID  ORDER BY sl.date_2 desc) RN,
sl.*, zsl.sumv
from D3_ZSL_OMS zsl
join d3_sl_oms sl on zsl.ID = sl.D3_ZSLID
join d3_schet_oms sc on zsl.D3_SCID = sc.ID
where  zsl.D3_SCID =  { sc.ID }   and 
          OS_SLUCH_REGION = 22
) tt --where tt.RN = 1
) tt1 on sl.ID = tt1.ID
", SprClass.LocalConnectionString);



            Reader2List.CustomExecuteQuery($@"
declare @Kursk_Usl_124N_Gr table(
	[IdGr] [float] NULL,
	[RegPrizn] [float] NULL,
	[Pol] [float] NULL,
	[Age] [nvarchar](255) NULL,
	[StandartKolUsl] [float] NULL,
	[OkazRanee] [float] NULL
)

declare @t15 table (
id int,
is15 int )

declare @t85 table (
id int, 
is85 int )

declare @tt table (
  id int,
  code_mo nvarchar(6),
  month int,
  W int,
  vozr int,
  OS_SLUCH_REGION int,
  [Real_Kol_Usl] int
  )

  insert @Kursk_Usl_124N_Gr
SELECT [ID_List_Group]
      ,[OsSluchReg]
      ,[Pol]
      ,[Age]
		,count (*) cnt, round(count(*) * 0.15, 0)
  FROM [dbo].[Kursk_Usl_124N]
  group by [ID_List_Group], [OsSluchReg], [Pol], [Age]

insert @t15
select t.id, 1--*,gr.OkazRanee
from
(select zs.id,p.w,
  year(zs.date_z_2)-year(p.dr) [vozr],
  zs.OS_SLUCH_REGION,count(u.id)as kol_vo_ok_ran
FROM [dbo].[D3_SCHET_OMS] sch
    inner join [dbo].[D3_PACIENT_OMS] p on sch.id=p.[D3_SCID]
                                and sch.id =  { sc.ID}  
    inner join [dbo].[D3_ZSL_OMS] zs on p.id=zs.[D3_PID]
                              and zs.OS_SLUCH_REGION=47
    inner join [dbo].[D3_SL_OMS] sl on zs.id=sl.[D3_ZSLID]
    inner join [dbo].[D3_USL_OMS] u on u.D3_SLID=sl.id
                              and u.[NPL]  in (4) --нужно ли????????
                              
group by zs.id  ,p.w,
  year(zs.date_z_2)-year(p.dr),
  zs.OS_SLUCH_REGION  ) t
  left join @Kursk_Usl_124N_Gr Gr 
    on gr.[RegPrizn]=t.OS_SLUCH_REGION
    and gr.[Pol]=t.W
    AND charindex(cast(t.[vozr] as nvarchar)+',',Gr.Age)<>0   
where t.kol_vo_ok_ran>OkazRanee


insert @tt
select
  k.id,
  k.code_mo,
  k.month,
  K.W,
  K.[vozr],
  K.OS_SLUCH_REGION,
  count (*) [Real_Kol_Usl]
from
(
select
  sch.code_mo,
  sch.month,
  zs.id,
  p.w,
  year(zs.date_z_1) - year(p.dr)  [vozr],
  zs.OS_SLUCH_REGION,
  u.[VID_VME]
  ,DU.[Pol]
    ,DU.[Age]
FROM [dbo].[D3_SCHET_OMS] sch
    inner join [dbo].[D3_PACIENT_OMS] p on sch.id=p.[D3_SCID]
                                and sch.id =  { sc.ID} 
    inner join [dbo].[D3_ZSL_OMS] zs on p.id=zs.[D3_PID]
                              and zs.OS_SLUCH_REGION=47
    inner join [dbo].[D3_SL_OMS] sl on zs.id=sl.[D3_ZSLID]
    inner join [dbo].[D3_USL_OMS] u on u.D3_SLID=sl.id
                              and (u.[NPL] not in (1,2) --нужно ли????????
                                or u.[NPL] is null)
    inner join [dbo].Kursk_Usl_124N du 
                      on du.[Pol]=p.w
                      --AND charindex(cast(dbo.f_GetAge (p.dr,zs.date_z_1) as nvarchar)+',',DU.Age)<>0
                                            AND charindex(cast(year(zs.date_z_2) - year(p.dr) as nvarchar(4))+',',DU.Age)<>0
                      and ltrim(rtrim(du.Code_Usl))=ltrim(rtrim(u.[VID_VME]))
)k
group by
  k.id,
  k.code_mo,
  k.month,
  K.W,
  K.[vozr],
  K.OS_SLUCH_REGION


 
insert @t85
select
  z.id, 1
  --z.*, gr.[StandartKolUsl], GR.IdGr
FROM @tt z LEFT JOIN @Kursk_Usl_124N_Gr Gr 
    on gr.[RegPrizn]=z.OS_SLUCH_REGION
    and gr.[Pol]=z.W
    AND charindex(cast(z.[vozr] as nvarchar)+',',Gr.Age)<>0 
  inner join [dbo].[D3_ZSL_OMS] zs on z.id=zs.id
where z.[Real_Kol_Usl]<gr.[StandartKolUsl]-gr.OkazRanee

--select
                    Update D3_ZSL_OMS SET 
                    SUMV= 
                            (CASE 
                                WHEN t85.is85 is null and year(DATE_Z_2) - year(dr) in (18,21,24,27,30,33,36,39,41,43,47,49,51,53,57,59,61,63,76,77,78, 79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99) AND W = 1 THEN t.tarif1
                                WHEN  t85.is85 is null and year(DATE_Z_2) - year(dr) in (40,42,44,46,48,52,54,56,58,62,65,66,67,68,69,70,71,72,73,74,75) AND W = 1 THEN t.tarif2
                                WHEN  t85.is85 is null and year(DATE_Z_2) - year(dr) in (55) AND W = 1 THEN t.tarif3
                                WHEN  t85.is85 is null and year(DATE_Z_2) - year(dr) in (50,60,64) AND W = 1 THEN t.tarif4
                                WHEN  t85.is85 is null and year(DATE_Z_2) - year(dr) in (45) AND W = 1 THEN t.tarif5
                                     
                                WHEN  t85.is85 is null and year(DATE_Z_2) - year(dr) in (41,43,47,49,53,55,59,61,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90, 91,92,93,94,95,96,97,98,99) AND W = 2 THEN t.tarif6
                                WHEN  t85.is85 is null and year(DATE_Z_2) - year(dr) in (18,21,24,27,30,33,36,39,51,57,63,65,67,69,71,73,75) AND W = 2 THEN t.tarif7
                                WHEN  t85.is85 is null and year(DATE_Z_2) - year(dr) in (40,44,46,50,52,56,58,62,64,66,68,70,72,74) AND W = 2 THEN t.tarif8
                                WHEN  t85.is85 is null and year(DATE_Z_2) - year(dr) in (42,45,48,54,60) AND W = 2 THEN t.tarif9


                                WHEN t85.is85 = 1 and year(DATE_Z_2) - year(dr) in (18,21,24,27,30,33,36,39,41,43,47,49,51,53,57,59,61,63,76,77,78, 79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99) AND W = 1 THEN 0
                                WHEN t85.is85 = 1 and year(DATE_Z_2) - year(dr) in (40,42,44,46,48,52,54,56,58,62,65,66,67,68,69,70,71,72,73,74,75) AND W = 1 THEN 0
                                WHEN t85.is85 = 1 and year(DATE_Z_2) - year(dr) in (55) AND W = 1 THEN 0
                                WHEN t85.is85 = 1 and year(DATE_Z_2) - year(dr) in (50,60,64) AND W = 1 THEN 0
                                WHEN t85.is85 = 1 and year(DATE_Z_2) - year(dr) in (45) AND W = 1 THEN 0

                                WHEN t85.is85 = 1 and year(DATE_Z_2) - year(dr) in (41,43,47,49,53,55,59,61,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90, 91,92,93,94,95,96,97,98,99) AND W = 2 THEN 0
                                WHEN t85.is85 = 1 and year(DATE_Z_2) - year(dr) in (18,21,24,27,30,33,36,39,51,57,63,65,67,69,71,73,75) AND W = 2 THEN 0
                                WHEN t85.is85 = 1 and year(DATE_Z_2) - year(dr) in (40,44,46,50,52,56,58,62,64,66,68,70,72,74) AND W = 2 THEN 0
                                WHEN t85.is85 = 1 and year(DATE_Z_2) - year(dr) in (42,45,48,54,60) AND W = 2 THEN 0


								--WHEN t15.is15 = 1 and year(DATE_Z_2) - year(dr) in (18,21,24,27,30,33,36,39,41,43,47,49,51,53,57,59,61,63,76,77,78, 79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99) AND W = 1 THEN t.TARIF12
                                --WHEN t15.is15 = 1 and year(DATE_Z_2) - year(dr) in (40,42,44,46,48,52,54,56,58,62,65,66,67,68,69,70,71,72,73,74,75) AND W = 1 THEN t.TARIF12
                                --WHEN t15.is15 = 1 and year(DATE_Z_2) - year(dr) in (55) AND W = 1 THEN t.TARIF12
                                --WHEN t15.is15 = 1 and year(DATE_Z_2) - year(dr) in (50,60,64) AND W = 1 THEN t.TARIF12
                                --WHEN t15.is15 = 1 and year(DATE_Z_2) - year(dr) in (45) AND W = 1 THEN t.TARIF12
                                --
                                --WHEN t15.is15 = 1 and year(DATE_Z_2) - year(dr) in (41,43,47,49,53,55,59,61,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90, 91,92,93,94,95,96,97,98,99) AND W = 2 THEN t.TARIF12
                                --WHEN t15.is15 = 1 and year(DATE_Z_2) - year(dr) in (18,21,24,27,30,33,36,39,51,57,63,65,67,69,71,73,75) AND W = 2 THEN t.TARIF12
                                --WHEN t15.is15 = 1 and year(DATE_Z_2) - year(dr) in (40,44,46,50,52,56,58,62,64,66,68,70,72,74) AND W = 2 THEN t.TARIF12
                                --WHEN t15.is15 = 1 and year(DATE_Z_2) - year(dr) in (42,45,48,54,60) AND W = 2 THEN t.TARIF12

                            END)

                from D3_ZSL_OMS zsl
        JOIN D3_PACIENT_OMS pa on zsl.D3_PID = pa.ID
                join CalcAmbTarif t on zsl.OS_SLUCH_REGION = t.OS_SLUCH and (zsl.DATE_Z_2 >= t.TBEG and zsl.DATE_Z_2 < t.TEND + 1)
        left join @t15 t15 on zsl.id = t15.id
        left join @t85 t85 on zsl.id = t85.id
                    where zsl.D3_SCID =  { sc.ID}   and 
          OS_SLUCH_REGION = 47

update sl set 
--select
sum_m = tt1.sumv, tarif = tt1.tarif
from d3_sl_oms sl
join ( select case when RN = 1 then sumv else 0.00 end sumv, sumv tarif, ID from
(
select ROW_NUMBER() OVER (PARTITION BY zsl.ID  ORDER BY sl.date_2 desc) RN,
sl.*, zsl.sumv
from D3_ZSL_OMS zsl
join d3_sl_oms sl on zsl.ID = sl.D3_ZSLID
join d3_schet_oms sc on zsl.D3_SCID = sc.ID
where  zsl.D3_SCID =  { sc.ID}   and 
          OS_SLUCH_REGION = 47
) tt --where tt.RN = 1
) tt1 on sl.ID = tt1.ID
", SprClass.LocalConnectionString);



            Reader2List.CustomExecuteQuery($@"
                    Update D3_ZSL_OMS SET 
                    SUMV= 
                            (CASE 
                                WHEN year(DATE_Z_2) - year(dr) in (19,20,22,23,25,26,28,29,31,32,34,35,37,38) AND W = 1 THEN t.tarif1
                                WHEN year(DATE_Z_2) - year(dr) in (18,21,24,27,30,33,36,39) AND W = 1 THEN t.tarif2
                                WHEN year(DATE_Z_2) - year(dr) in (41,43,45,47,49,51,53,55,57,59,61,63,65,67,69,71,73,75,77,79,81,83,85,87,89,91,93,95,97,99) AND W = 1 THEN t.tarif3
                                WHEN year(DATE_Z_2) - year(dr) in (40,42,44,46,48,50,52,54,56,58,60,62,64,66,68,70,72,74,76,78,80,82,84,86,88,90,92,94,96,98) AND W = 1 THEN t.tarif4
                                     
                                WHEN year(DATE_Z_2) - year(dr) in (19,20,22,23,25,26,28,29,31,32,34,35,37,38) AND W = 2 THEN t.tarif5
                                WHEN year(DATE_Z_2) - year(dr) in (18,21,24,27,30,33,36,39) AND W = 2 THEN t.tarif6
                                WHEN year(DATE_Z_2) - year(dr) in (41,43,45,47,49,51,53,55,57,59,61,63,65,67,69,71,73,75,77,79,81,83,85,87,89,91,93,95,97,99) AND W = 2 THEN t.tarif7
                                WHEN year(DATE_Z_2) - year(dr) in (40,42,44,46,48,50,52,54,56,58,60,62,64,66,68,70,72,74,76,78,80,82,84,86,88,90,92,94,96,98) AND W = 2 THEN t.tarif8
                            END)

                from D3_ZSL_OMS zsl
        JOIN D3_PACIENT_OMS pa on zsl.D3_PID = pa.ID
                join CalcAmbTarif t on zsl.OS_SLUCH_REGION = t.OS_SLUCH and (zsl.DATE_Z_2 >= t.TBEG and zsl.DATE_Z_2 < t.TEND + 1)
                    where zsl.D3_SCID =  { sc.ID}   and 
          OS_SLUCH_REGION = 49

update sl set 
--select
sum_m = tt1.sumv, tarif = tt1.tarif
from d3_sl_oms sl
join ( select case when RN = 1 then sumv else 0.00 end sumv, sumv tarif, ID from
(
select ROW_NUMBER() OVER (PARTITION BY zsl.ID  ORDER BY sl.date_2 desc) RN,
sl.*, zsl.sumv
from D3_ZSL_OMS zsl
join d3_sl_oms sl on zsl.ID = sl.D3_ZSLID
join d3_schet_oms sc on zsl.D3_SCID = sc.ID
where  zsl.D3_SCID =  { sc.ID}   and 
          OS_SLUCH_REGION = 49
) tt --where tt.RN = 1
) tt1 on sl.ID = tt1.ID
", SprClass.LocalConnectionString);

            Reader2List.CustomExecuteQuery($@"
Update D3_SL_OMS SET ED_COL = 1
from D3_SL_OMS sl
join D3_ZSL_OMS zsl on sl.D3_ZSLID = zsl.ID and zsl.D3_SCID = { sc.ID}
where zsl.OS_SLUCH_REGION in (4,5,7,9,17,21,29,32,33,34,35,36,37,38,40)

					Update D3_ZSL_OMS SET
					--select
                         SUMV = (CASE
                                WHEN ((SMO is null or SMO not like '46%') or OS_SLUCH_REGION in ( 4,5,7,9,17,21,29,30,32,33,34,35,36,37,38,40) or (SELECT Parametr From Settings Where Name = 'SoulNorm') = 0) THEN ROUND(isnull(sl.ED_COL, 1.00) * t.tarif1 * ISNULL(kf.KZMP, 1.00), 2)
                                ELSE ROUND(isnull(sl.ED_COL, 1.00) * t.tarif1 * ISNULL(kf.KZMP, 1.00), 2) --0.00
                            END)
                    from D3_ZSL_OMS zsl 
                    JOIN D3_PACIENT_OMS pa on zsl.D3_PID = pa.ID
					join D3_SL_OMS sl on zsl.ID = sl.D3_ZSLID --and zsl.USL_OK =3
                    join CalcAmbTarif t on zsl.OS_SLUCH_REGION = t.OS_SLUCH and (sl.DATE_2 >= t.TBEG and sl.DATE_2 < t.TEND +1) 
                    left join CalcMok as kf on kf.KOD_LPU = zsl.LPU AND (zsl.DATE_Z_2 >= kf.DATESTART and (kf.DATEEND is NULL OR zsl.DATE_Z_2 < kf.DATEEND +1))
                    where zsl.D3_SCID = { sc.ID} and 
					 OS_SLUCH_REGION not in (11,22,6,47,49)

update sl set sum_m = tt1.sumv, tarif = tt1.tarif
from d3_sl_oms sl
join ( select case when RN = 1 then sumv else 0.00 end sumv, sumv tarif, ID from
(
select ROW_NUMBER() OVER (PARTITION BY zsl.ID  ORDER BY sl.date_2 desc) RN,
sl.*, zsl.sumv
from D3_ZSL_OMS zsl
join d3_sl_oms sl on zsl.ID = sl.D3_ZSLID
join d3_schet_oms sc on zsl.D3_SCID = sc.ID
where  zsl.D3_SCID = { sc.ID} and 
					 OS_SLUCH_REGION not in (11,22,6,47,49,31,39)
) tt --where tt.RN = 1
) tt1 on sl.ID = tt1.ID
", SprClass.LocalConnectionString);


            Reader2List.CustomExecuteQuery($@"
Update D3_SL_OMS SET ED_COL = 1
from D3_SL_OMS sl
join D3_ZSL_OMS zsl on sl.D3_ZSLID = zsl.ID and zsl.D3_SCID = { sc.ID}
where zsl.OS_SLUCH_REGION in (31, 39)

					Update D3_ZSL_OMS SET
					--select
                         SUMV = (CASE
                                WHEN ((SMO is null or SMO not like '46%') or OS_SLUCH_REGION in ( 31, 39) or (SELECT Parametr From Settings Where Name = 'SoulNorm') = 0) THEN ROUND(isnull(sl.ED_COL, 1.00) * t.tarif1 * ISNULL(kf.KZMP, 1.00), 2)
                                ELSE ROUND(isnull(sl.ED_COL, 1.00) * t.tarif1 * ISNULL(kf.KZMP, 1.00), 2) --0.00
                            END)
                    from D3_ZSL_OMS zsl 
                    JOIN D3_PACIENT_OMS pa on zsl.D3_PID = pa.ID
					join D3_SL_OMS sl on zsl.ID = sl.D3_ZSLID --and zsl.USL_OK =3
                    join CalcAmbTarif t on zsl.OS_SLUCH_REGION = t.OS_SLUCH and sl.Profil = t.Profil and (sl.DATE_2 >= t.TBEG and sl.DATE_2 < t.TEND +1) 
                    left join CalcMok as kf on kf.KOD_LPU = zsl.LPU AND (zsl.DATE_Z_2 >= kf.DATESTART and (kf.DATEEND is NULL OR zsl.DATE_Z_2 < kf.DATEEND +1))
                    where zsl.D3_SCID = { sc.ID} and 
					 OS_SLUCH_REGION in (31,39)

update sl set sum_m = tt1.sumv, tarif = tt1.tarif
from d3_sl_oms sl
join ( select case when RN = 1 then sumv else 0.00 end sumv, sumv tarif, ID from
(
select ROW_NUMBER() OVER (PARTITION BY zsl.ID  ORDER BY sl.date_2 desc) RN,
sl.*, zsl.sumv
from D3_ZSL_OMS zsl
join d3_sl_oms sl on zsl.ID = sl.D3_ZSLID
join d3_schet_oms sc on zsl.D3_SCID = sc.ID
where  zsl.D3_SCID = { sc.ID} and 
					 OS_SLUCH_REGION not in (11,22,6,47,49)
) tt --where tt.RN = 1
) tt1 on sl.ID = tt1.ID
", SprClass.LocalConnectionString);

    }
    */
}
