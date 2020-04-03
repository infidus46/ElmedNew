using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Mvvm.Native;
using Yamed.Control;
using Yamed.Core;
using Yamed.Server;
using Yamed.Entity;
using System.Data;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Win32;
using System.IO;
using DevExpress.Xpf.Core;

namespace Yamed.Oms
{
    /// <summary>
    /// Логика взаимодействия для ReestrChooseControl.xaml
    /// </summary>
    public partial class ReestrChooseControl : UserControl
    {
        private D3_SCHET_OMS _osc;
        public ReestrChooseControl(D3_SCHET_OMS sc)
        {
            InitializeComponent();
            _osc = sc;
        }
        

        private int _id;
       

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            int warn = 0;
            using (SqlConnection sconn = new SqlConnection(SprClass.LocalConnectionString))
            {
                sconn.Open();
                using (SqlCommand scomm = new SqlCommand("select Parametr from Settings where Name = 'MedicalOrganization' and Parametr = '460000'", sconn))
                {
                    using (SqlDataReader sreader = scomm.ExecuteReader())
                    {
                        while (sreader.Read())
                        {
                            warn = Convert.ToInt32(sreader["Parametr"]);
                        }
                    }
                }
                sconn.Close();
            }
            _id = (int)cbSchets.EditValue;
            int selectCbOne = cbOperation.SelectedIndex;
            if (selectCbOne == 1)
            {
                var ids = ObjHelper.GetIds(DxHelper.LoadedRows.Select(x => ObjHelper.GetAnonymousValue(x, "ID")).OfType<int>().ToArray());

                var connectionString = SprClass.LocalConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                SqlCommand comm = new SqlCommand($@"update d3_pacient_oms set d3_scid={_id} 
where id in (select d3_pid from d3_zsl_oms where id in({ids}))
 update d3_zsl_oms set d3_scid={_id} where id in ({ids})", con);
                con.Open();
                comm.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Перенос записей выполнен успешно.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            if (selectCbOne == 0 & warn != 460000)
            {
                var ids = ObjHelper.GetIds(DxHelper.LoadedRows.Select(x => ObjHelper.GetAnonymousValue(x, "ID")).OfType<int>().ToArray());
                var copyQuery = $@"
IF OBJECT_ID(N'tempdb..#t_pac', N'U') IS NOT NULL 
DROP TABLE #t_pac;

IF OBJECT_ID(N'tempdb..#t_zsl', N'U') IS NOT NULL 
DROP TABLE #t_zsl;

IF OBJECT_ID(N'tempdb..#t_sl', N'U') IS NOT NULL 
DROP TABLE #t_sl;

IF OBJECT_ID(N'tempdb..#t_usl', N'U') IS NOT NULL 
DROP TABLE #t_usl;

IF OBJECT_ID(N'tempdb..#t_naz', N'U') IS NOT NULL 
DROP TABLE #t_naz;

IF OBJECT_ID(N'tempdb..#t_dss', N'U') IS NOT NULL 
DROP TABLE #t_dss;

IF OBJECT_ID(N'tempdb..#t_ksg', N'U') IS NOT NULL 
DROP TABLE #t_ksg;

IF OBJECT_ID(N'tempdb..#t_koeff', N'U') IS NOT NULL 
DROP TABLE #t_koeff;

IF OBJECT_ID(N'tempdb..#t_crit', N'U') IS NOT NULL 
DROP TABLE #t_crit;



declare @cols varchar(max),@cols1 varchar(max), @query varchar(max), @id varchar(max),@cols2 varchar(max),@cols3 varchar(max),@cols4 varchar(max),
@cols5 varchar(max),@cols6 varchar(max),@cols7 varchar(max),@cols8 varchar(max);
set @id='{ids} '

SELECT  @cols = STUFF
   (
        ( 
            SELECT  '], [' + name
            FROM sys.columns
            where object_id = (
                select  object_id from sys.objects
                where name = 'D3_PACIENT_OMS'
            ) 
            and name not in (SELECT  Name  FROM    sys.computed_columns where OBJECT_NAME(object_id)='D3_PACIENT_OMS')
      and name!='rn'
            FOR XML PATH('')
        ), 1, 2, ''
    ) + ']';

  SELECT  @cols1 = STUFF
   (
        ( 
            SELECT  '], [' + name
            FROM sys.columns
            where object_id = (
                select  object_id from sys.objects
                where name = 'D3_ZSL_OMS'
            ) 
            and name not in (SELECT  Name  FROM    sys.computed_columns where OBJECT_NAME(object_id)='D3_ZSL_OMS')
      and name!='rn' and name!='rnp'
            FOR XML PATH('')
        ), 1, 2, ''
    ) + ']';

  SELECT  @cols2 = STUFF
   (
        ( 
            SELECT  '], [' + name
            FROM sys.columns
            where object_id = (
                select  object_id from sys.objects
                where name = 'D3_SL_OMS'
            ) 
            and name not in (SELECT  Name  FROM    sys.computed_columns where OBJECT_NAME(object_id)='D3_SL_OMS')
      and name!='rn' and name!='rnz'
            FOR XML PATH('')
        ), 1, 2, ''
    ) + ']';

SELECT  @cols3 = STUFF
   (
        ( 
            SELECT  '], [' + name
            FROM sys.columns
            where object_id = (
                select  object_id from sys.objects
                where name = 'D3_USL_OMS'
            ) 
            and name not in (SELECT  Name  FROM    sys.computed_columns where OBJECT_NAME(object_id)='D3_USL_OMS')
      and name!='rn' and name!='rns' 
            FOR XML PATH('')
        ), 1, 2, ''
    ) + ']';
  SELECT  @cols4 = STUFF
   (
        ( 
            SELECT  '], [' + name
            FROM sys.columns
            where object_id = (
                select  object_id from sys.objects
                where name = 'D3_NAZ_OMS'
            ) 
            and name not in (SELECT  Name  FROM    sys.computed_columns where OBJECT_NAME(object_id)='D3_NAZ_OMS')
      and name!='rn' and name!='rns' 
            FOR XML PATH('')
        ), 1, 2, ''
    ) + ']';
  SELECT  @cols5 = STUFF
   (
        ( 
            SELECT  '], [' + name
            FROM sys.columns
            where object_id = (
                select  object_id from sys.objects
                where name = 'D3_DSS_OMS'
            ) 
            and name not in (SELECT  Name  FROM    sys.computed_columns where OBJECT_NAME(object_id)='D3_DSS_OMS')
      and name!='rn' and name!='rns' 
            FOR XML PATH('')
        ), 1, 2, ''
    ) + ']';
  SELECT  @cols6 = STUFF
   (
        ( 
            SELECT  '], [' + name
            FROM sys.columns
            where object_id = (
                select  object_id from sys.objects
                where name = 'D3_KSG_KPG_OMS'
            ) 
            and name not in (SELECT  Name  FROM    sys.computed_columns where OBJECT_NAME(object_id)='D3_KSG_KPG_OMS')
      and name!='rn' and name!='rns' 
            FOR XML PATH('')
        ), 1, 2, ''
    ) + ']';  
  SELECT  @cols7 = STUFF
   (
        ( 
            SELECT  '], [' + name
            FROM sys.columns
            where object_id = (
                select  object_id from sys.objects
                where name = 'D3_SL_KOEF_OMS'
            ) 
            and name not in (SELECT  Name  FROM    sys.computed_columns where OBJECT_NAME(object_id)='D3_SL_KOEF_OMS')
      and name!='rn' and name!='rns' 
            FOR XML PATH('')
        ), 1, 2, ''
    ) + ']';
  SELECT  @cols8 = STUFF
   (
        ( 
            SELECT  '], [' + name
            FROM sys.columns
            where object_id = (
                select  object_id from sys.objects
                where name = 'D3_CRIT_OMS'
            ) 
            and name not in (SELECT  Name  FROM    sys.computed_columns where OBJECT_NAME(object_id)='D3_CRIT_OMS')
      and name!='rn' and name!='rns' 
            FOR XML PATH('')
        ), 1, 2, ''
    ) + ']';

  declare @cols10 varchar(max);
  set @cols10=replace(@cols,'[ID],','');
  declare @cols11 varchar(max);
  set @cols11=replace(@cols1,'[ID],','');
  declare @cols12 varchar(max);
  set @cols12=replace(@cols2,'[ID],','');
  declare @cols13 varchar(max);
  set @cols13=replace(@cols3,'[ID],','');
  declare @cols14 varchar(max);
  set @cols14=replace(@cols4,'[ID],','');
  declare @cols15 varchar(max);
  set @cols15=replace(@cols5,'[ID],','');
  declare @cols16 varchar(max);
  set @cols16=replace(@cols6,'[ID],','');
  declare @cols17 varchar(max);
  set @cols17=replace(@cols7,'[ID],','');
  declare @cols18 varchar(max);
  set @cols18=replace(@cols8,'[ID],','');
  

SELECT @query = 'select ROW_NUMBER ( ) OVER (order by id ) as RN,' + @cols + 'into #t_pac from D3_PACIENT_OMS where  id in (select d3_pid from d3_zsl_oms where id in ('+@id+')) 
update #t_pac set ID_PAC=newid(),D3_SCID={_id}

select ROW_NUMBER ( ) OVER (order by id ) as RN,0 as rnp,' + @cols1 + 'into #t_zsl from D3_ZSL_OMS where  id in ('+@id+')
update #t_zsl set #t_zsl.ZSL_ID=NEWID() 
    
    update #t_zsl  set #t_zsl.rnp=#t_pac.RN from #t_pac where #t_zsl.D3_PID=#t_pac.ID
    update #t_zsl set #t_zsl.D3_PGID=#t_pac.ID_PAC,D3_SCID={_id} from #t_pac where #t_zsl.D3_PID=#t_pac.ID

select ROW_NUMBER ( ) OVER (order by id ) as RN,0 as rnz,' + @cols2 + 'into #t_sl from D3_SL_OMS where D3_ZSLID in (select id from #t_zsl)
update #t_sl  set #t_sl.rnz=#t_zsl.RN from #t_zsl where #t_sl.D3_zslID=#t_zsl.ID
    update #t_sl set #t_sl.D3_ZSLGID=#t_zsl.ZSL_ID from #t_zsl where #t_sl.D3_ZSLID=#t_zsl.ID
    update #t_sl set #t_sl.SL_ID=NEWID()


select ROW_NUMBER ( ) OVER (order by id ) as RN,0 as rns,' + @cols3 + 'into #t_usl from D3_USL_OMS where D3_ZSLID in (select id from #t_zsl)

update #t_usl  set #t_usl.rns=#t_sl.RN from #t_sl where #t_usl.D3_slID=#t_sl.ID
    update #t_usl set #t_usl.D3_SLGID=#t_sl.SL_ID from  #t_sl where #t_usl.D3_SLID=#t_sl.ID


select ROW_NUMBER ( ) OVER (order by id ) as RN,0 as rns,' + @cols4 + 'into #t_naz from D3_NAZ_OMS where D3_SLID in (select id from #t_sl)

update #t_naz  set #t_naz.rns=#t_sl.RN from #t_sl where #t_naz.D3_slID=#t_sl.ID
    update #t_naz set #t_naz.D3_SLGID=#t_sl.SL_ID from  #t_sl where #t_naz.D3_SLID=#t_sl.ID


select ROW_NUMBER ( ) OVER (order by id ) as RN,0 as rns,' + @cols5 + 'into #t_dss from D3_DSS_OMS where D3_SLID in (select id from #t_sl)

update #t_dss  set #t_dss.rns=#t_sl.RN from #t_sl where #t_dss.D3_slID=#t_sl.ID
    update #t_dss set #t_dss.D3_SLGID=#t_sl.SL_ID from  #t_sl where #t_dss.D3_SLID=#t_sl.ID


select ROW_NUMBER ( ) OVER (order by id ) as RN,0 as rns,' + @cols6 + 'into #t_ksg from D3_KSG_KPG_OMS where D3_SLID in (select id from #t_sl)

update #t_ksg  set #t_ksg.rns=#t_sl.RN from #t_sl where #t_ksg.D3_slID=#t_sl.ID
    update #t_ksg set #t_ksg.D3_SLGID=#t_sl.SL_ID from  #t_sl where #t_ksg.D3_SLID=#t_sl.ID
    update #t_ksg set #t_ksg.KSG_ID=NEWID()


select ROW_NUMBER ( ) OVER (order by id ) as RN,0 as rns,' + @cols7 + 'into #t_koeff from D3_SL_KOEF_OMS where D3_KSGID in (select id from #t_ksg)

update #t_koeff  set #t_koeff.rns=#t_ksg.RN from #t_ksg where #t_koeff.D3_KSGID=#t_ksg.ID
    update #t_koeff set #t_koeff.D3_KSGGID=#t_ksg.KSG_ID from  #t_ksg where #t_koeff.D3_KSGID=#t_ksg.ID


select ROW_NUMBER ( ) OVER (order by id ) as RN,0 as rns,' + @cols8 + 'into #t_crit from D3_CRIT_OMS where D3_KSGID in (select id from #t_ksg)

update #t_crit  set #t_crit.rns=#t_ksg.RN from #t_ksg where #t_crit.D3_KSGID=#t_ksg.ID
    update #t_crit set #t_crit.D3_KSGGID=#t_ksg.KSG_ID from  #t_ksg where #t_crit.D3_KSGID=#t_ksg.ID




insert into D3_PACIENT_OMS ('+@cols10+') 
select '+@cols10+' from #t_pac

insert into D3_ZSL_OMS ('+@cols11+') 
select '+@cols11+' from #t_zsl
update D3_ZSL_OMS set D3_PID= D3_PACIENT_OMS.id from D3_PACIENT_OMS where D3_ZSL_OMS.D3_PGID=D3_PACIENT_OMS.ID_PAC

insert into D3_SL_OMS ('+@cols12+') 
select '+@cols12+' from #t_sl
update D3_SL_OMS set D3_ZSLID= d3_zsl_oms.ID from D3_ZSL_OMS where D3_SL_OMS.D3_ZSLGID=D3_ZSL_OMS.ZSL_ID

insert into D3_USL_OMS ('+@cols13+') 
select '+@cols13+' from #t_usl
update D3_USL_OMS set D3_ZSLID= d3_sl_oms.D3_ZSLID from D3_SL_OMS where D3_uSL_OMS.D3_SLGID=D3_SL_OMS.SL_ID
    update D3_USL_OMS set D3_SLID= d3_sl_oms.ID from D3_SL_OMS where D3_uSL_OMS.D3_SLGID=D3_SL_OMS.SL_ID
    update D3_USL_OMS set D3_SLGID= d3_sl_oms.SL_ID from D3_SL_OMS where D3_uSL_OMS.D3_SLGID=D3_SL_OMS.SL_ID

insert into D3_NAZ_OMS ('+@cols14+') 
select '+@cols14+' from #t_naz
update D3_NAZ_OMS set D3_SLID= d3_sl_oms.ID from D3_SL_OMS where D3_NAZ_OMS.D3_SLGID=D3_SL_OMS.SL_ID

insert into D3_DSS_OMS ('+@cols15+') 
select '+@cols15+' from #t_dss
update D3_DSS_OMS set D3_SLID= d3_sl_oms.ID from D3_SL_OMS where D3_DSS_OMS.D3_SLGID=D3_SL_OMS.SL_ID

insert into D3_KSG_KPG_OMS ('+@cols16+') 
select '+@cols16+' from #t_ksg
update D3_KSG_KPG_OMS set D3_SLID= d3_sl_oms.ID from D3_SL_OMS where D3_KSG_KPG_OMS.D3_SLGID=D3_SL_OMS.SL_ID

insert into D3_SL_KOEF_OMS ('+@cols17+') 
select '+@cols17+' from #t_koeff
update D3_SL_KOEF_OMS set D3_KSGID= D3_KSG_KPG_OMS.ID from D3_KSG_KPG_OMS where D3_SL_KOEF_OMS.D3_KSGGID=D3_KSG_KPG_OMS.KSG_ID


insert into D3_CRIT_OMS ('+@cols18+') 
select '+@cols18+' from #t_crit
update D3_CRIT_OMS set D3_KSGID= D3_KSG_KPG_OMS.ID from D3_KSG_KPG_OMS where D3_CRIT_OMS.D3_KSGGID=D3_KSG_KPG_OMS.KSG_ID
    
'; 
EXEC (@query) ;
";
                Reader2List.CustomExecuteQuery(copyQuery, SprClass.LocalConnectionString);
                MessageBox.Show("Копирование записей выполнено успешно.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            if (selectCbOne == 2 & warn != 460000)
            {

                var ids =
                        DxHelper.LoadedRows.Select(x => ObjHelper.GetAnonymousValue(x, "ID"))
                            .OfType<int>()
                            .Distinct()
                            .ToArray();
                MessageBoxResult resdel = MessageBox.Show($"Вы действительно хотите переподать {ids.Length} записей?", "Переподача",
MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resdel == MessageBoxResult.Yes)
                {
                    var idstr = ObjHelper.GetIds(ids);
                    var connectionString = SprClass.LocalConnectionString;
                    SqlConnection con = new SqlConnection(connectionString);
                    SqlCommand comm2 = new SqlCommand($@"DECLARE @newsc INT = {_id}
DECLARE @schold INT = {_osc.ID}

if object_id('tempdb..##tabid') is not null  
	drop table ##tabid

select id into ##tabid from D3_ZSL_OMS where D3_SCID=@schold and id in ({idstr})	

exec p_oms_copy_allzsl @newsc,@schold,0 		

	", con);
                    comm2.CommandTimeout = 0;
                    con.Open();

                    comm2.ExecuteNonQuery();
                    con.Close();
                    DXMessageBox.Show($"Успешно отправлено на переподачу {ids.Length} записей");
                }
                else
                {
                    return;
                }
            }
            if (selectCbOne == 3 & warn != 460000)
            {

                var connectionString = SprClass.LocalConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                OpenFileDialog OF = new OpenFileDialog();
                OF.InitialDirectory = @"c:\";
                bool res = OF.ShowDialog().Value;
                string fname = OF.FileName;
                string xml = File.ReadAllText(fname, Encoding.GetEncoding(1251));



                if (res == true)
                {

                    SqlCommand comm1 = new SqlCommand($@"DECLARE @newsc INT = {_id}
DECLARE @schold INT = {_osc.ID}

if object_id('tempdb..##tabid') is not null  
	drop table ##tabid
	
declare @Nxml xml ='{xml}'

select zsl.id	into ##tabid from @Nxml.nodes( '/SANKINFO/SANK/ZSL_ID' ) sl(m)
	 join d3_zsl_oms zsl on zsl.ZSL_ID=sl.m.value('(.)[1]','nvarchar(254)')

exec p_oms_copy_allzsl @newsc,@schold,1 	

", con);
                    comm1.CommandTimeout = 0;
                    con.Open();

                    comm1.ExecuteNonQuery();
                    con.Close();
                    DXMessageBox.Show($"Переподача из файла выполнена");
                }
                else
                {
                    return;
                }

            }
        }


        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("1 Копирование (кроме (онко - C) реестров)");
            list.Add("2 Перенос");
            list.Add("3 Копирование+признак переподачи ");
            list.Add("4 Массовая переподача из файла");
            cbOperation.ItemsSource = list;
            //using (SqlConnection sc = new SqlConnection(SprClass.LocalConnectionString))
            //{
            //    sc.Open();
            //    using (SqlDataAdapter sda = new SqlDataAdapter("select (((('—счет(' + CONVERT([varchar](16), [ID])  + ')/период '+CONVERT([varchar](2),[MONTH],(0)))+'.')+CONVERT([char](6),[YEAR],(0)))+isnull(('('+[COMENTS])+')','')) nameSchet from d3_schet_oms s", sc))
            //    {
            //        using (DataTable dt = new DataTable())
            //        {
            //            sda.Fill(dt);
            //            cbSchets.ItemsSource = dt;
            //        }
            //    }
            //    sc.Close();
            //    cbSchets.SelectedIndex = -1;
            //}
            cbSchets.ItemsSource =
                Reader2List.CustomAnonymousSelect(
                    "select ID, (((('—счет(' + CONVERT([varchar](16), [ID]) + ')/период ' + CONVERT([varchar](2),[MONTH], (0)))+'.')+CONVERT([char](6),[YEAR],(0)))+isnull(('('+[COMENTS])+')','')) nameSchet from d3_schet_oms s",
                    SprClass.LocalConnectionString);
        }
    }
}