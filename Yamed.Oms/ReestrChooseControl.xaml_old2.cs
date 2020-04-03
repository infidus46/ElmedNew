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
                using (SqlCommand scomm = new SqlCommand("select Parametr from Settings where Name = 'MedicalOrganization' and Parametr = '460028'", sconn))
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
                //updObj.ForEach(x=> x.SetValue("D3_SCID", _id));
                //foreach (var row in updObj)
                //{
                //    var r = row;
                //    ObjHelper.SetAnonymousValue(ref r, _id, "D3_SCID");
                //}

                //var upd = Reader2List.CustomUpdateCommand("D3_ZSL_OMS", updObj, "ID");
                //Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
                MessageBox.Show("Перенос записей выполнен успешно.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            if (selectCbOne == 0 & warn != 460028)
            {
                var ids = ObjHelper.GetIds(DxHelper.LoadedRows.Select(x => ObjHelper.GetAnonymousValue(x, "ID")).OfType<int>().ToArray());
                var copyQuery = "IF OBJECT_ID(N'tempdb..#t_pac', N'U') IS NOT NULL DROP TABLE #t_pac;  IF OBJECT_ID(N'tempdb..#t_zsl', N'U') IS NOT NULL DROP TABLE #t_zsl;  IF OBJECT_ID(N'tempdb..#t_sl', N'U') IS NOT NULL DROP TABLE #t_sl;  IF OBJECT_ID(N'tempdb..#t_usl', N'U') IS NOT NULL DROP TABLE #t_usl;  declare @cols varchar(max),@cols1 varchar(max), @query varchar(max), @id varchar(max),@cols2 varchar(max),@cols3 varchar(max); set @id='" + ids + "' SELECT  @cols = STUFF ( ( SELECT  '], [' + name FROM sys.columns where object_id = ( select  object_id from sys.objects where name = 'D3_PACIENT_OMS' ) and name not in (SELECT  Name  FROM    sys.computed_columns where OBJECT_NAME(object_id)='D3_PACIENT_OMS') and name!='rn' FOR XML PATH('') ), 1, 2, '' ) + ']';  SELECT  @cols1 = STUFF ( ( SELECT  '], [' + name FROM sys.columns where object_id = ( select  object_id from sys.objects where name = 'D3_ZSL_OMS' ) and name not in (SELECT  Name  FROM    sys.computed_columns where OBJECT_NAME(object_id)='D3_ZSL_OMS') and name!='rn' and name!='rnp' FOR XML PATH('') ), 1, 2, '' ) + ']';  SELECT  @cols2 = STUFF ( ( SELECT  '], [' + name FROM sys.columns where object_id = ( select  object_id from sys.objects where name = 'D3_SL_OMS' ) and name not in (SELECT  Name  FROM    sys.computed_columns where OBJECT_NAME(object_id)='D3_SL_OMS') and name!='rn' and name!='rnz' FOR XML PATH('') ), 1, 2, '' ) + ']';  SELECT  @cols3 = STUFF ( ( SELECT  '], [' + name FROM sys.columns where object_id = ( select  object_id from sys.objects where name = 'D3_USL_OMS' ) and name not in (SELECT  Name  FROM    sys.computed_columns where OBJECT_NAME(object_id)='D3_USL_OMS') and name!='rn' and name!='rns' FOR XML PATH('') ), 1, 2, '' ) + ']';  declare @cols10 varchar(max); set @cols10=replace(@cols,'[ID],',''); declare @cols11 varchar(max); set @cols11=replace(@cols1,'[ID],',''); declare @cols12 varchar(max); set @cols12=replace(@cols2,'[ID],',''); declare @cols13 varchar(max); set @cols13=replace(@cols3,'[ID],','');  SELECT @query = 'select ROW_NUMBER ( ) OVER (order by id ) as RN,' + @cols + 'into #t_pac from D3_PACIENT_OMS where  id in (select d3_pid from d3_zsl_oms where id in ('+@id+')) update #t_pac set ID_PAC=newid(),d3_scid=" + _id + "  select ROW_NUMBER ( ) OVER (order by id ) as RN,0 as rnp,' + @cols1 + 'into #t_zsl from D3_ZSL_OMS where  id in ('+@id+') update #t_zsl set #t_zsl.ZSL_ID=NEWID()  update #t_zsl  set #t_zsl.rnp=#t_pac.RN from #t_pac where #t_zsl.D3_PID=#t_pac.ID update #t_zsl set #t_zsl.D3_PGID=#t_pac.ID_PAC,d3_scid=" + _id + " from #t_pac where #t_zsl.D3_PID=#t_pac.ID   select ROW_NUMBER ( ) OVER (order by id ) as RN,0 as rnz,' + @cols2 + 'into #t_sl from D3_SL_OMS where D3_ZSLID in (select id from #t_zsl) update #t_sl  set #t_sl.rnz=#t_zsl.RN from #t_zsl where #t_sl.D3_zslID=#t_zsl.ID update #t_sl set #t_sl.D3_ZSLGID=#t_zsl.ZSL_ID from #t_zsl where #t_sl.D3_ZSLID=#t_zsl.ID update #t_sl set #t_sl.SL_ID=NEWID()   select ROW_NUMBER ( ) OVER (order by id ) as RN,0 as rns,' + @cols3 + 'into #t_usl from D3_USL_OMS where D3_ZSLID in (select id from #t_zsl)  update #t_usl  set #t_usl.rns=#t_sl.RN from #t_sl where #t_usl.D3_slID=#t_sl.ID update #t_usl set #t_usl.D3_SLGID=#t_sl.SL_ID from  #t_sl where #t_usl.D3_SLID=#t_sl.ID    insert into D3_PACIENT_OMS ('+@cols10+') select '+@cols10+' from #t_pac  insert into D3_ZSL_OMS ('+@cols11+') select '+@cols11+' from #t_zsl update D3_ZSL_OMS set D3_PID= D3_PACIENT_OMS.id from D3_PACIENT_OMS where D3_ZSL_OMS.D3_PGID=D3_PACIENT_OMS.ID_PAC  insert into D3_SL_OMS ('+@cols12+') select '+@cols12+' from #t_sl update D3_SL_OMS set D3_ZSLID= d3_zsl_oms.ID from D3_ZSL_OMS where D3_SL_OMS.D3_ZSLGID=D3_ZSL_OMS.ZSL_ID  insert into D3_USL_OMS ('+@cols13+') select '+@cols13+' from #t_usl update D3_USL_OMS set D3_ZSLID= d3_sl_oms.D3_ZSLID from D3_SL_OMS where D3_uSL_OMS.D3_SLGID=D3_SL_OMS.SL_ID update D3_USL_OMS set D3_SLID= d3_sl_oms.ID from D3_SL_OMS where D3_uSL_OMS.D3_SLGID=D3_SL_OMS.SL_ID update D3_USL_OMS set D3_SLGID= d3_sl_oms.SL_ID from D3_SL_OMS where D3_uSL_OMS.D3_SLGID=D3_SL_OMS.SL_ID  '; EXEC (@query);";
                Reader2List.CustomExecuteQuery(copyQuery, SprClass.LocalConnectionString);
                MessageBox.Show("Копирование записей выполнено успешно.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            if (selectCbOne == 2 & warn != 460028)
            {

            }
            if (selectCbOne == 3 & warn != 460028)
            {
                
                var connectionString = SprClass.LocalConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                OpenFileDialog OF = new OpenFileDialog();
                OF.InitialDirectory = @"c:\";
                bool res = OF.ShowDialog().Value;
                string fname = OF.FileName;
                string xml = File.ReadAllText(fname,Encoding.GetEncoding(1251));
                
                

                if (res == true)
                {
                    SqlCommand comm = new SqlCommand($@" 
    declare @SankXml xml
  if object_id ('ztemp_SankXml') is not null drop table ztemp_SankXml
  CREATE TABLE ztemp_SankXml (SankXml xml)
  insert into ztemp_SankXml (SankXml) values('{xml}');

  set @SankXml = (select SankXml from ztemp_sankxml)

 
    if OBJECT_ID('ztemp_Sank532') is not null drop table ztemp_Sank532
  
  select 
    sank.m.value('(./CODE_MO    )[1]','nvarchar(6)') CODE_MO,
    sank.m.value('(./YEAR    )[1]','int') YEAR,
    sank.m.value('(./MONTH    )[1]','int') MONTH,
    sank.m.value('(./SMO    )[1]','nvarchar(5)') SMO,
    sank.m.value('(./NPOLIS    )[1]','nvarchar(16)') NPOLIS,
    sank.m.value('(./FAM    )[1]','nvarchar(40)') FAM,
    sank.m.value('(./IM    )[1]','nvarchar(40)') IM,
    sank.m.value('(./OT    )[1]','nvarchar(40)') OT,
    --sank.m.value('(./DR    )[1]','nvarchar(10)'),
    convert(date,sank.m.value('(./DR    )[1]','nvarchar(10)'),120) DR,
    sank.m.value('(./ZSL_ID    )[1]','uniqueidentifier') ZSL_ID,
    sank.m.value('(./USL_OK    )[1]','int') USL_OK,
    sank.m.value('(./IDSP    )[1]','int') IDSP,
    sank.m.value('(./SUMV    )[1]','numeric(17,2)') SUMV,
    convert(date,sank.m.value('(./DATE_Z_1    )[1]','nvarchar(10)'),120) DATE_Z_1,
    convert(date,sank.m.value('(./DATE_Z_2    )[1]','nvarchar(10)'),120) DATE_Z_2
  into ztemp_Sank532
  from @SankXml.nodes('/SANKINFO/SANK') sank (m)", con);
                    con.Open();
                    comm.ExecuteNonQuery();
                    con.Close();
                    SqlCommand comm1 = new SqlCommand($@"DECLARE @schnew INT = {_id}
DECLARE @schold INT = {_osc.ID}
DECLARE @zid INT, @scom NVARCHAR(500)
DECLARE vendor_cursor CURSOR FOR 
  SELECT  dzo.id, dso.fam
  FROM D3_ZSL_OMS AS dzo
  JOIN ztemp_Sank532 AS dso ON dso.ZSL_ID = dzo.ZSL_ID  
  WHERE dzo.D3_SCID = @schold
  
OPEN vendor_cursor

FETCH NEXT FROM vendor_cursor INTO @zid, @scom

WHILE @@FETCH_STATUS = 0
BEGIN
  PRINT @zid  
  EXEC p_oms_copy_zap @schnew, @schold, @zid
  
  
  FETCH NEXT FROM vendor_cursor INTO @zid, @scom
END;
CLOSE vendor_cursor;
DEALLOCATE vendor_cursor;", con);
                    comm1.CommandTimeout = 0;
                    con.Open();
                    
                    comm1.ExecuteNonQuery();
                    con.Close();
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
            list.Add("1 Копирование");
            list.Add("2 Перенос");
            list.Add("3 Переподача");
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