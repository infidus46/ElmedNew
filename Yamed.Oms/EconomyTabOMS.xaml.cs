using System;
using System.Collections;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ServerMode;
using DevExpress.Xpf.Grid;
using Ionic.Zip;
using Microsoft.Win32;
using Yamed.Control;
using Yamed.Control.Editors;
using Yamed.Core;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.Oms
{
    /// <summary>
    /// Логика взаимодействия для EconomyWindow.xaml
    /// </summary>
    public partial class EconomyTabOMS : UserControl
    {
        public LinqInstantFeedbackDataSource linqInstantFeedbackDataSource;

        public static readonly DependencyProperty IsSmoTableVisibleProperty =
            DependencyProperty.Register("IsSmoTableVisible", typeof (Visibility), typeof (EconomyWindow), null);

        public Visibility IsSmoTableVisible
        {
            get
            {
                return (Visibility)GetValue(IsSmoTableVisibleProperty);
            }
            set
            {
                SetValue((DependencyProperty) IsSmoTableVisibleProperty, value);
            }
        }

        public EconomyTabOMS()
        {
            InitializeComponent();
            DataContext = this;
            //IsSmoTableVisible = SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu ? Visibility.Visible : Visibility.Collapsed;
            //if (IsSmoTableVisible == Visibility.Collapsed)
            //    Grid1.RowDefinitions[2].Height = GridLength.Auto;
            if (SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)
            {
                ScExportEis.IsVisible = true;
            }
            linqInstantFeedbackDataSource = (LinqInstantFeedbackDataSource)FindResource("LinqInstantFeedbackDataSource");
            var edc =  new YamedDataClassesDataContext()
            {
                ObjectTrackingEnabled = false,
                Connection = { ConnectionString = SprClass.LocalConnectionString }
            };

            IQueryable pQueryable = from sc in edc.D3_SCHET_OMS
                                    join f3 in edc.D3_F003 on sc.CODE_MO equals f3.mcod
                                    join sprsc in edc.Yamed_Spr_SchetType on sc.SchetType equals sprsc.ID
                                    select new
                                    {                                        
                                        ID = sc.ID,
                                        CODE_MO = sc.CODE_MO,
                                        NAME_MO = f3.nam_mok,
                                        NAME_MO_ID = f3.NameWithID,
                                        PLAT = sc.PLAT,
                                        YEAR = sc.YEAR,
                                        MONTH = sc.MONTH,
                                        NSCHET = sc.NSCHET,
                                        DSCHET = sc.DSCHET,
                                        SUMMAV = sc.SUMMAV,
                                        SUMMAP = sc.SUMMAP,
                                        SANK_MEK = sc.SANK_MEK,
                                        SANK_MEE = sc.SANK_MEE,
                                        SANK_EKMP = sc.SANK_EKMP,
                                        COMENTS = sc.COMENTS,
                                        sc.DISP,
                                        sc.SD_Z,
                                        sc.OmsFileName,
                                        sc.ZapFileName,
                                        sc.PersFileName,
                                        sc.SchetType,
                                        SchetTypeName = sprsc.NameWithID, // добавил Андрей insidious
                                        sc.Status 
                                    };

            linqInstantFeedbackDataSource.QueryableSource = pQueryable;

//            var tfoms = Reader2List.CustomSelect<Settings>("Select * from Settings where Name = 'TFOMS'",
//SprClass.LocalConnectionString).SingleOrDefault()?.Parametr;
            //            var isTfoms = tfoms != null && bool.Parse(tfoms);
            //            var isSmo = tfoms != null && !bool.Parse(tfoms);


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


        private void calendarRowItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            //var schet = ObjHelper.ClassConverter<D3_SCHET_OMS>(DxHelper.GetSelectedGridRow(gridControl));

            //var window = new DXWindow
            //{
            //    ShowIcon = false, WindowStartupLocation = WindowStartupLocation.Manual,
            //    Content = new CalendarHoliday((string)ObjHelper.GetAnonymousValue(schet, "CODE_MO")),
            //    Title = "Выходные дни"
            //};
            //window.ShowDialog();

        }



        private void EconomyWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            linqInstantFeedbackDataSource.Dispose();
        }

        private void EconomyWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                new Action(delegate()
                {
                    gridControl.ExpandGroupRow(-1);
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                        new Action(delegate()
                        {
                            gridControl.ExpandGroupRow(-2);
                        }));
                }));
            //PlatColumnEdit.DataContext = SprClass.Payment;
        }



        private void GridControl_OnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            //var row = DxHelper.GetSelectedGridRow(gridControl);
            //if (row == null) return;

            //var id = ObjHelper.GetAnonymousValue(row, "ID");
            //OmsList =
            //    Reader2List.CustomSelect<SCHET_OMS>(
            //        $@"Select * From SCHET_OMS WHERE SCHET_ID = {id}"
            //        , SprClass.LocalConnectionString);
            //gridControl2.DataContext = OmsList;
        }


        private void RefreshItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            linqInstantFeedbackDataSource.Refresh();
        }

        private void AddItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = (EconomyWindow)((TabElement)СommonСomponents.DxTabObject).MyControl;
            var schet = new D3_SCHET_OMS();

            var sprEditWindow = new UniSprEditControl("D3_SCHET_OMS", schet, false, SprClass.LocalConnectionString);
            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Content = sprEditWindow,
                Title = "Новая запись"
            };

            window.ShowDialog();
            linqInstantFeedbackDataSource.Refresh();
        }

        private void EditItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = (EconomyWindow)((TabElement)СommonСomponents.DxTabObject).MyControl;
            var row = DxHelper.GetSelectedGridRow(gridControl);
            if (row == null) return;

            var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(row);

            var sprEditWindow = new UniSprEditControl("D3_SCHET_OMS", sc, true, SprClass.LocalConnectionString);
            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Content = sprEditWindow,
                Title = "Редактирование"
            };

            window.ShowDialog();
            linqInstantFeedbackDataSource.Refresh();
        }

        private void DelItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = (EconomyWindow)((TabElement)СommonСomponents.DxTabObject).MyControl;
            var row = ObjHelper.ClassConverter<D3_SCHET_OMS>(DxHelper.GetSelectedGridRow(gridControl));
            if (row == null) return;

            MessageBoxResult result = MessageBox.Show("Удалить счет за период " + row.MONTH + "." + row.YEAR + "\n" + SprClass.LpuList.Single(x => x.mcod == row.CODE_MO).NameWithID + "?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                //barButtonItem4.IsEnabled = false;
                TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext(); //get UI thread context 


               // обработчик удаления счетов со статусами, Андрей insidious
                if (string.Equals(row.Status, "отработан", StringComparison.CurrentCultureIgnoreCase) || string.Equals(row.Status, "ошибочен", StringComparison.CurrentCultureIgnoreCase))
                {
                    ErrorGlobalWindow.ShowError("Невозможно удалить счет с данным статусом");
                }
                else
                {
                    bool isDel = false;
                    LoadingDecorator1.IsSplashScreenShown = true;
                    var delSchet = Task.Factory.StartNew(() =>
                    {
                        {
                            try
                            {
                                isDel = true;
                                Reader2List.CustomExecuteQuery($@"DELETE FROM D3_SCHET_OMS where ID = {row.ID} ", SprClass.LocalConnectionString);
                            }
                            catch (Exception ex)
                            {
                                isDel = false;
                                Dispatcher.BeginInvoke((Action)delegate ()
                                {
                                    ErrorGlobalWindow.ShowError(ex.Message);
                                });
                            }
                        }
                    });
                    delSchet.ContinueWith(x =>
                    {

                        if (isDel)
                        {
                            LoadingDecorator1.IsSplashScreenShown = false;

                            linqInstantFeedbackDataSource.Refresh();
                            ErrorGlobalWindow.ShowError("Счет удален");
                        }
                    //barButtonItem4.IsEnabled = true;
                    //}
                }, uiScheduler);
                }
            }

            //var id = PublicVoids.GetAnonymousValue(row, "ID");
            //Reader2List.CustomExecuteQuery($"DELETE FROM {"D3_SCHET_OMS"} WHERE {"ID"}={id}", SprClass.LocalConnectionString);
            //tab.linqInstantFeedbackDataSource.Refresh();
        }

        private void ScImportItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "OMS File |*.oms;*.zip";


            var result = openFileDialog.ShowDialog();

            var rfile = openFileDialog.FileName;
            var omszfn = openFileDialog.SafeFileName;

            string zapfn = "";
            string persfn = "";
            var zapms = new MemoryStream();
            var persms = new MemoryStream();

            string x_fn = "";
            string t_fn = "";
            var x_ms = new MemoryStream();
            var t_ms = new MemoryStream();

            if (result != true) return;

            //var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(DxHelper.GetSelectedGridRow(gridControl));
            ((BarButtonItem)sender).IsEnabled = false;
            LoadingDecorator1.IsSplashScreenShown = true;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (ZipFile zip = ZipFile.Read(rfile))
                    {
                        if (zip.Count == 2)
                        {
                            foreach (ZipEntry zipEntry in zip)
                            {
                                if (zipEntry.FileName.StartsWith("HM") || zipEntry.FileName.StartsWith("CM") || zipEntry.FileName.StartsWith("D") ||
                                    zipEntry.FileName.StartsWith("T"))
                                {
                                    zapfn = zipEntry.FileName;
                                    zipEntry.Extract(zapms);
                                    zapms.Position = 0;
                                }

                                if (zipEntry.FileName.StartsWith("L"))
                                {
                                    persfn = zipEntry.FileName;
                                    zipEntry.Extract(persms);
                                    persms.Position = 0;
                                }
                            }

                        }

                        else
                        {
                            foreach (ZipEntry zipEntry in zip)
                            {
                                if (zipEntry.FileName.StartsWith("HM"))
                                {
                                    zapfn = zipEntry.FileName;
                                    zipEntry.Extract(zapms);
                                    zapms.Position = 0;
                                }

                                if (zipEntry.FileName.StartsWith("L"))
                                {
                                    persfn = zipEntry.FileName;
                                    zipEntry.Extract(persms);
                                    persms.Position = 0;
                                }

                                if (zipEntry.FileName.StartsWith("XM"))
                                {
                                    x_fn = zipEntry.FileName;
                                    zipEntry.Extract(x_ms);
                                    x_ms.Position = 0;
                                }

                                if (zipEntry.FileName.StartsWith("TM"))
                                {
                                    t_fn = zipEntry.FileName;
                                    zipEntry.Extract(t_ms);
                                    t_ms.Position = 0;
                                }
                            }
                            {

                            }
                        }
                    }
                    //"0x" + BitConverter.ToString(arraytoinsert).Replace("-", "")
                }
                catch (Exception ex)
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        DXMessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException?.Message);
                    });
                }


                var zapsr = new StreamReader(zapms, Encoding.Default);
                string zapxml = zapsr.ReadToEnd();
                zapms.Dispose();
                zapms.Close();

                var perssr = new StreamReader(persms, Encoding.Default);
                string persxml = perssr.ReadToEnd();
                persms.Dispose();
                persms.Close();

                var x_sr = new StreamReader(x_ms, Encoding.Default);
                string x_xml = x_sr.ReadToEnd();
                x_sr.Dispose();
                x_sr.Close();

                var t_sr = new StreamReader(t_ms, Encoding.Default);
                string t_xml = t_sr.ReadToEnd();
                t_sr.Dispose();
                t_sr.Close();
                if (string.IsNullOrEmpty(zapxml) || string.IsNullOrEmpty(persxml))
                {
                    //Reader2List.CustomExecuteQuery($@"Update DOX_SCHET SET DOX_STATUS=12 ", SprClass.LocalConnectionString);
                }
                else
                {
                    try
                    {
                        //                        var q = $@"

                        //                        BEGIN TRANSACTION;  
                        //                        BEGIN TRY

                        //                            Declare @sc int
                        //                            INSERT INTO D3_SCHET_OMS (ZAPXMLFILE, PERSXMLFILE, ZAPFILENAME, PERSFILENAME) VALUES (CAST('{zapxml.Replace("'", "")}' AS XML), CAST('{persxml.Replace("'", "")}' AS XML), '{zapfn}', '{persfn}')
                        //                            Select @sc = SCOPE_IDENTITY()

                        //                            EXEC p_oms_load_schet @sc
                        //                            EXEC p_oms_load_pacient @sc
                        //                            EXEC p_oms_load_zsl @sc
                        //                            EXEC p_oms_load_sl @sc
                        //                            EXEC p_oms_load_dss @sc
                        //                            EXEC p_oms_load_usl @sc

                        //update d3_zsl_oms set usl_ok = sl.usl_ok, p_cel = sl.p_cel
                        //from d3_zsl_oms zsl
                        //join d3_sl_oms sl on zsl.ID = sl.D3_ZSLID
                        //where zsl.USL_OK is null or zsl.P_CEL is null

                        //                        END TRY
                        //                        BEGIN CATCH
                        //                            IF @@TRANCOUNT > 0  
                        //                                ROLLBACK TRANSACTION;  
                        //                            --PRINT 'In catch block.';
                        //                        DECLARE
                        //                           @ErMessage NVARCHAR(2048),
                        //                           @ErSeverity INT,
                        //                           @ErState INT

                        //                         SELECT
                        //                           @ErMessage = ERROR_MESSAGE(),
                        //                           @ErSeverity = ERROR_SEVERITY(),
                        //                           @ErState = ERROR_STATE()

                        //                         RAISERROR (@ErMessage,
                        //                                     @ErSeverity,
                        //                                     @ErState )
                        //                        END CATCH;

                        //                        IF @@TRANCOUNT > 0  
                        //                            COMMIT TRANSACTION;  
                        //                        ";
                        var q = $@"
                            EXEC [dbo].[p_oms_load_all_newformat] '{zapxml.Replace("'", "")}', '{persxml.Replace("'", "")}', '{zapfn}', '{persfn}'
                        ";


                        zapxml = null;
                        persxml = null;
                        Reader2List.CustomExecuteQuery(q, SprClass.LocalConnectionString);
                        q = null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            DXMessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException?.Message);
                        });
                        //Reader2List.CustomExecuteQuery($@"Update DOX_SCHET SET DOX_STATUS=13 WHERE ID = {id}", _connectionString);
                        //return;
                    }
                }
                zapsr.Dispose();
                zapsr.Close();
                perssr.Dispose();
                perssr.Close();


                //Console.WriteLine("Распакован " + id);

                GC.WaitForPendingFinalizers();
                GC.Collect();

            }).ContinueWith(x =>
            {
                linqInstantFeedbackDataSource.Refresh();

                DXMessageBox.Show("Загрузка успешно завершена");

                ((BarButtonItem)sender).IsEnabled = true;
                LoadingDecorator1.IsSplashScreenShown = false;


            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void DelMekRowMenu_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var row = ObjHelper.ClassConverter<D3_SCHET_OMS>(DxHelper.GetSelectedGridRow(gridControl));
            if (row == null) return;

            MessageBoxResult result = MessageBox.Show("Удалить МЭКи за период " + row.MONTH + "." + row.YEAR + "\n" + SprClass.LpuList.Single(x => x.mcod == row.CODE_MO).NameWithID + "?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                bool isDel = false;
                LoadingDecorator1.IsSplashScreenShown = true;

                var delSank = Task.Factory.StartNew(() =>
                {
                        try
                        {
                            Reader2List.CustomExecuteQuery($@"
DELETE D3_SANK_OMS where D3_SCID = {row.ID}
EXEC p_oms_calc_sank {row.ID}
EXEC p_oms_calc_schet {row.ID}
", SprClass.LocalConnectionString);
                            
                        }
                        catch (Exception ex)
                        {
                            isDel = false;
                            Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                ErrorGlobalWindow.ShowError(ex.Message);
                            });
                        }
                    isDel = true;
                });
                delSank.ContinueWith(x =>
                {

                    if (isDel)
                    {
                        LoadingDecorator1.IsSplashScreenShown = false;

                        linqInstantFeedbackDataSource.Refresh();
                        ErrorGlobalWindow.ShowError("МЭКи в счете удалены.");
                    }
                    //barButtonItem4.IsEnabled = true;
                    //}
                }, TaskScheduler.FromCurrentSynchronizationContext());


            }
        }

        private int _gr = 1;
        private void GrItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (_gr == 1)
            {
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                    new Action(delegate()
                    {
                        gridControl.ClearGrouping();

                        gridControl.GroupBy("NAME_MO_ID");
                    }));
                _gr = 2;
            } else if (_gr == 2)
            {
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                    new Action(delegate()
                    {
                        gridControl.ClearGrouping();

                        gridControl.GroupBy("YEAR");
                        gridControl.GroupBy("MONTH");
                    }));
                _gr = 1;
            }
        }

        private void ScExportItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //MtrExport();
            var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(DxHelper.GetSelectedGridRow(gridControl));

            var qxml = SqlReader.Select($@"
            exec p_oms_export_30K {ObjHelper.GetAnonymousValue(sc, "ID")}"
, SprClass.LocalConnectionString);



            string result1 = "<?xml version=\"1.0\" encoding=\"windows-1251\"?>" + (string)qxml[0].GetValue("HM");
            string result2 = "<?xml version=\"1.0\" encoding=\"windows-1251\"?>" + (string)qxml[0].GetValue("LM");
            using (ZipFile zip = new ZipFile(Encoding.GetEncoding("windows-1251")))
            {
                zip.AddEntry((string)qxml[0].GetValue("hf_name") + ".xml", result1);
                zip.AddEntry((string)qxml[0].GetValue("lf_name") + ".xml", result2);

                //if (isLoop)
                //{
                //    if (!Directory.Exists(Environment.CurrentDirectory + @"\Out"))
                //        Directory.CreateDirectory(Environment.CurrentDirectory + @"\Out");

                //    var zipFile = Environment.CurrentDirectory + @"\Out\" + (string)qxml[0].GetValue("hf_name") + ".oms";
                //    zip.Save(zipFile);
                //}
                //else
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();

                        saveFileDialog.Filter = "OMS File (*.oms)|*.oms";
                        saveFileDialog.FileName = (string)qxml[0].GetValue("hf_name") + ".oms";

                        bool? result = saveFileDialog.ShowDialog();
                        if (result == true)
                            zip.Save(saveFileDialog.FileName);
                    });
                }

            }


        }

        private void MtrExport()
        {
            var schets = DxHelper.GetSelectedGridRows(gridControl);
            if (schets.Length > 1)
                foreach (var sc1 in schets)
                {
                    var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(sc1);
                    XmlStreemMtr(sc, true, true);
                }
            else
            {
                var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(schets[0]);
                XmlStreemMtr(sc, true);
            }

        }

        public void XmlStreemMtr(object schet, bool isTfomsSchet, bool isLoop = false, int loopCount = 1)
        {

            //LoadingDecorator1.IsSplashScreenShown = true;

            int i = 0;
            var schetp = schet;
            string plat = string.IsNullOrEmpty((string)ObjHelper.GetAnonymousValue(schetp, "PLAT")) ? "46" : (string)ObjHelper.GetAnonymousValue(schetp, "PLAT");
            string m = Convert.ToString(ObjHelper.GetAnonymousValue(schetp, "MONTH"));
            string mm = (int)ObjHelper.GetAnonymousValue(schetp, "MONTH") < 10 ? "0" + m : m;
            string sf = plat.Length > 2 ? "S" : "T";

            string filename1 = "";
            string filename2 = "";

            if (SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu)
            {
                filename1 = "HM" + ObjHelper.GetAnonymousValue(schetp, "CODE_MO") + sf + plat + "_" +
                            ObjHelper.GetAnonymousValue(schetp, "YEAR").ToString().Substring(2) +
                            mm + "1";
                filename2 = "LM" + ObjHelper.GetAnonymousValue(schetp, "CODE_MO") + sf + plat + "_" +
                            ObjHelper.GetAnonymousValue(schetp, "YEAR").ToString().Substring(2) + mm + "1";
            }
            else if (ObjHelper.GetAnonymousValue(schetp, "PLAT") != null && ((string)ObjHelper.GetAnonymousValue(schetp, "PLAT")).StartsWith("04"))
            {

                var com1 = (string)ObjHelper.GetAnonymousValue(schetp, "COMENTS");
                var com2 = com1.Substring(com1.IndexOf("M") + 1);
                var codemoreg = com2.Remove(com2.IndexOf("S"));

                string codef = "";
                if (((string)ObjHelper.GetAnonymousValue(schetp, "COMENTS")).StartsWith("D"))
                {
                    codef = ((string)ObjHelper.GetAnonymousValue(schetp, "COMENTS")).Substring(0, 2);

                }
                else
                    codef = ((string)ObjHelper.GetAnonymousValue(schetp, "COMENTS")).Substring(0, 1);

                filename1 = codef + "S" + ((string)ObjHelper.GetAnonymousValue(schetp, "PLAT")).Substring(3) + "T" +
                            ((string)ObjHelper.GetAnonymousValue(schetp, "PLAT")).Substring(0, 2) + "_" +
                            ObjHelper.GetAnonymousValue(schetp, "YEAR").ToString().Substring(2) +
                            mm + loopCount + "_" + codemoreg;
                filename2 = "L" + codef.Remove(0, 1) + "S" + "" + ((string)ObjHelper.GetAnonymousValue(schetp, "PLAT")).Substring(3) + "T" +
                            ((string)ObjHelper.GetAnonymousValue(schetp, "PLAT")).Substring(0, 2) + "_" +
                            ObjHelper.GetAnonymousValue(schetp, "YEAR").ToString().Substring(2) +
                            mm + loopCount + "_" + codemoreg;
            }
            else
            {
                filename1 = ((string)ObjHelper.GetAnonymousValue(schetp, "ZapFileName"))?.Replace(".xml", "");
                filename2 = ((string)ObjHelper.GetAnonymousValue(schetp, "PersFileName"))?.Replace(".xml", "");
                if (filename1 == null)
                    filename1 = "HM" + ObjHelper.GetAnonymousValue(schetp, "CODE_MO") + "T" + "46" + "_" +
                                ObjHelper.GetAnonymousValue(schetp, "YEAR").ToString().Substring(2) +
                                mm + "1";
                if (filename2 == null)
                    filename2 = "LM" + ObjHelper.GetAnonymousValue(schetp, "CODE_MO") + "T" + "46" + "_" +
                                ObjHelper.GetAnonymousValue(schetp, "YEAR").ToString().Substring(2) + mm + "1";

            }
            string filename3 = "XM" + ObjHelper.GetAnonymousValue(schetp, "CODE_MO") + sf + plat + "_" + ObjHelper.GetAnonymousValue(schetp, "YEAR").ToString().Substring(2) +
                    mm + "1";

            var pacientList = Reader2List.CustomSelect<D3_PACIENT_OMS>(
            (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu) && isTfomsSchet ? $@"
            Select distinct pa.* From SLUCH sl
            Join Pacient pa on sl.pid = pa.id
            where sl.schet_id = {ObjHelper.GetAnonymousValue(schet, "ID")} and (sl.vopl is null or sl.vopl = 1)
            order by pa.FAM, pa.IM, pa.OT, pa.DR" :
            SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms && isTfomsSchet ? $@"
			Select distinct pa.* From D3_ZSL_MTR zsl
            Join d3_Pacient_oms pa on zsl.D3_PID = pa.id
            where zsl.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")}
            order by pa.FAM, pa.IM, pa.OT, pa.DR" :
            $@"
            Select distinct pa.* From SLUCH sl
            Join Pacient pa on sl.pid = pa.id
            where sl.SCHET_OMS_ID = {ObjHelper.GetAnonymousValue(schet, "CODE")} and (sl.vopl is null or sl.vopl = 1)
            order by pa.FAM, pa.IM, pa.OT, pa.DR", SprClass.LocalConnectionString);

            var sluchList = Reader2List.CustomSelect<SLUCH>(
            (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu) && isTfomsSchet ? $@"
            Select distinct sl.* From SLUCH sl
            where sl.schet_id = {ObjHelper.GetAnonymousValue(schet, "ID")} and (sl.vopl is null or sl.vopl = 1)" :
            SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms && isTfomsSchet ? $@"
SELECT [ID]
      ,[IDCASE]
      ,[Z_USL_OK] [USL_OK]
      ,[VIDPOM]
      ,[NPR_MO]
      ,null [EXTR]
      ,[LPU]
      ,null [LPU_1]
      ,null [PODR]
      ,(Select top 1 [PROFIL] from d3_sl_oms where d3_zslid = zsl.id order by date_2 desc) [PROFIL]
      ,cast((Select top 1 [DET] from d3_sl_oms where d3_zslid = zsl.id order by date_2 desc) as tinyint) [DET]
      ,(Select top 1 [NHISTORY] from d3_sl_oms where d3_zslid = zsl.id order by date_2 desc) [NHISTORY]
      ,[DATE_Z_1] [DATE_1]
      ,[DATE_Z_2] [DATE_2]
      ,null [DS0]
      ,(Select top 1 [DS1] from d3_sl_oms where d3_zslid = zsl.id order by date_2 desc)[DS1]
      ,null [DS2]
      ,null [CODE_MES1]
      ,null [CODE_MES2]
      ,null [SLUCH_TYPE]
      ,[RSLT]
      ,[ISHOD]
      ,(Select top 1 [PRVS] from d3_sl_oms where d3_zslid = zsl.id order by date_2 desc)[PRVS]
      ,(Select top 1 [IDDOKT] from d3_sl_oms where d3_zslid = zsl.id order by date_2 desc)[IDDOKT]
      ,convert(nvarchar(20), [OS_SLUCH]) [OS_SLUCH]
      ,[OS_SLUCH_REGION]
      ,[IDSP]
      ,1.00 ED_COL
      ,[SUMV] [TARIF]
      ,[SUMV]
      ,cast ([OPLATA] as numeric (1,0)) [OPLATA]
      ,[SUMP]
      ,[SANK_IT]
      ,null [COMENTSL]
      ,d3_scid [SCHET_ID]
      ,null [GR_ZDOROV]
      ,null [VETERAN]
      ,null [SCHOOL]
      ,null [WORK_STAT]
      ,d3_pid [PID]
      ,null [ID_TEMP]
      ,null [PID_TEMP]
      ,null [IDKSG]
      ,null [MEK_COMENT]
      ,null [MEE_COMENT]
      ,null [EKMP_COMENT]
      ,null [MEE_TYPE]
      ,null [KSG_OPLATA]
      ,null [KDAY]
      ,null [REQUEST_DATE]
      ,null [USERID]
      ,(Select top 1 [PRVS] from d3_sl_oms where d3_zslid = zsl.id order by date_2 desc) [MSPID]
      ,null [LPUID]
      ,null [DIFF_K]
      ,null [VID_HMP]
      ,null [METOD_HMP]
      ,null [IDSLG]
      ,null [ISIDSLG]
      ,null [REMEK_COM]
      ,null [MEK_COUNT]
      ,null [MEE_COUNT]
      ,null [EKMP_COUNT]
      ,null [RMEK_COUNT]
      ,null [RMEE_COUNT]
      ,null [REKMP_COUNT]
      ,null [GOSP]
      ,null [VR_DOST]
      ,null [TRAVMA]
      ,null [NNAPR]
      ,null [DNAPR]
      ,null [KDOST]
      ,null [NDOST]
      ,null [VOPL]
      ,null [NNAR]
      ,null [DSN]
      ,null [DOSTP]
      ,null [DSKZ]
      ,null [PRS]
      ,null [DSS]
      ,null [DSPA]
      ,null [IDDOKTP]
      ,null [IDDOKTO]
      ,null [DEFGOSP]
      ,null [SCHET_OMS_ID]
      ,null [KSG_COM]
      ,null [KDAY_COM]
      ,null [DIFF_COM]
      ,null [RSLT_D]
      ,null [VOZR]
      ,null [TER_ID]
	  ,[FOR_POM]
	  ,null [KSG_DKK]

  FROM [dbo].D3_ZSL_MTR zsl
  where D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")}" :
            $@"
            Select distinct sl.* From SLUCH sl
            where sl.SCHET_OMS_ID = {ObjHelper.GetAnonymousValue(schet, "CODE")} and (sl.vopl is null or sl.vopl = 1)", SprClass.LocalConnectionString).OrderBy(x => x.PID);

            var ds2List = Reader2List.CustomSelect<SLUCH_DS2>(
            (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu) && isTfomsSchet ? $@"
            Select distinct ds2.* From SLUCH sl
            Join SLUCH_DS2 ds2 on sl.ID = ds2.SLID
            where sl.schet_id = {ObjHelper.GetAnonymousValue(schet, "ID")} and (sl.vopl is null or sl.vopl = 1)" :
            SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms && isTfomsSchet ? $@"
            Select distinct ds2.* From SLUCH sl
            Join SLUCH_DS2 ds2 on sl.ID = ds2.SLID
            Join Pacient pa on sl.pid = pa.id and (pa.SMO is null or pa.SMO not like '46%')
            where sl.schet_id = 0" :
            $@"
            Select distinct ds2.* From SLUCH sl
            Join SLUCH_DS2 ds2 on sl.ID = ds2.SLID
            where sl.SCHET_OMS_ID = {ObjHelper.GetAnonymousValue(schet, "CODE")} and (sl.vopl is null or sl.vopl = 1)", SprClass.LocalConnectionString).OrderBy(x => x.ID);

            var ds3List = Reader2List.CustomSelect<SLUCH_DS2>(
            (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu) && isTfomsSchet ? $@"
            Select distinct ds3.* From SLUCH sl
            Join SLUCH_DS3 ds3 on sl.ID = ds3.SLID
            where sl.schet_id = {ObjHelper.GetAnonymousValue(schet, "ID")} and (sl.vopl is null or sl.vopl = 1)" :
            SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms && isTfomsSchet ? $@"
            Select distinct ds3.* From SLUCH sl
            Join SLUCH_DS3 ds3 on sl.ID = ds3.SLID
            Join Pacient pa on sl.pid = pa.id and (pa.SMO is null or pa.SMO not like '46%')
            where sl.schet_id = 0" :
            $@"
            Select distinct ds3.* From SLUCH sl
            Join SLUCH_DS3 ds3 on sl.ID = ds3.SLID
            where sl.SCHET_OMS_ID = {ObjHelper.GetAnonymousValue(schet, "CODE")} and (sl.vopl is null or sl.vopl = 1)", SprClass.LocalConnectionString).OrderBy(x => x.ID);

            var uslList = Reader2List.CustomSelect<USL>(
            (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu) && isTfomsSchet ? $@"
            Select distinct u.* From SLUCH sl
            Join usl u on sl.id = u.slid
            where sl.schet_id = {ObjHelper.GetAnonymousValue(schet, "ID")} and (sl.vopl is null or sl.vopl = 1)" :
            SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms && isTfomsSchet ? $@"
            Select distinct u.* From SLUCH sl
            Join Pacient pa on sl.pid = pa.id and (pa.SMO is null or pa.SMO not like '46%')
            Join usl u on sl.id = u.slid
            where sl.schet_id = 0" :
            $@"
            Select distinct u.* From SLUCH sl
            Join usl u on sl.id = u.slid
            where sl.SCHET_OMS_ID = {ObjHelper.GetAnonymousValue(schet, "CODE")} and (sl.vopl is null or sl.vopl = 1)", SprClass.LocalConnectionString);

            var sankList = Reader2List.CustomSelect<D3_SANK_OMS>(
            (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu) && isTfomsSchet ? $@"
            Select distinct sa.* From SLUCH sl
            Join sank sa on sl.id = sa.slid
            where sl.schet_id = {ObjHelper.GetAnonymousValue(schet, "ID")} and (sl.vopl is null or sl.vopl = 1)" :
            SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms && isTfomsSchet ? $@"
            Select distinct sa.*
			From D3_ZSL_MTR sl
            Join D3_SANK_OMS sa on sl.id = sa.d3_zslid
            where sl.D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")}" :
            $@"
            Select distinct sa.* From SLUCH sl
            Join sank sa on sl.id = sa.slid
            where sl.SCHET_OMS_ID = {ObjHelper.GetAnonymousValue(schet, "CODE")} and (sl.vopl is null or sl.vopl = 1)", SprClass.LocalConnectionString);

            decimal? sumInTer = null;
            if (SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)
            {
                var sumInTerQuery = Reader2List.CustomAnonymousSelect(
                        $@"
                Select sum(SUMV) as SUMV from D3_ZSL_MTR
            where D3_SCID = {ObjHelper.GetAnonymousValue(schet, "ID")}", SprClass.LocalConnectionString);
                sumInTer = (decimal?)ObjHelper.GetAnonymousValue(((IList)sumInTerQuery)[0], "SUMV");
            }

            //decimal? sum3 = 0;
            //if (plat.StartsWith("57"))
            //{
            //    var _sum3 =
            //        Reader2List.CustomAnonymousSelect(
            //            $@"
            //    Select isnull(sum(SUMV), 0) as SUMV from sluch
            //    where schet_id ={ObjHelper.GetAnonymousValue(schet, isTfomsSchet ? "ID" : "CODE")}
            //    and celp in (2.1,2.2,2.3,2.4,2.5,2.6,2.7,3.2,3.3,3.4,3.5,3.6,3.7,6.1,6.2,6.3)",
            //            SprClass.LocalConnectionString);
            //    sum3 =
            //        (decimal?)
            //            ObjHelper.GetAnonymousValue(((IList) _sum3)[0], "SUMV");
            //}

            var isWriter3 = false;

            int nzap1 = 1;
            int nzap3 = 1;
            int idcase1 = 1;
            int idcase3 = 1;
            XmlWriterSettings writerSettings
                = new XmlWriterSettings()
                {
                    Encoding = Encoding.GetEncoding("windows-1251"),
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = Environment.NewLine,
                    ConformanceLevel = ConformanceLevel.Document
                };
            using (ZipFile zip = new ZipFile())
            {
                var stream1 = new MemoryStream();
                var stream2 = new MemoryStream();
                var stream3 = new MemoryStream();

                using (XmlWriter writer1 = XmlWriter.Create(stream1, writerSettings))
                {
                    using (XmlWriter writer2 = XmlWriter.Create(stream2, writerSettings))
                    {
                        using (XmlWriter writer3 = XmlWriter.Create(stream3, writerSettings))
                        {
                            // writer1
                            writer1.WriteStartElement("ZL_LIST");
                            writer1.WriteStartElement("ZGLV");
                            writer1.WriteElementString("VERSION", "2.1");
                            writer1.WriteElementString("DATA", DateTime.Now.Date.ToString("yyyy-MM-dd"));
                            writer1.WriteElementString("FILENAME", filename1);
                            writer1.WriteEndElement();
                            writer1.WriteStartElement("D3_SCHET_OMS");
                            if (ObjHelper.GetAnonymousValue(schet, "CODE") != null) writer1.WriteElementString("CODE", Convert.ToString(ObjHelper.GetAnonymousValue(schet, "CODE")));
                            else writer1.WriteElementString("CODE", ObjHelper.GetAnonymousValue(schet, "ID").ToString());
                            if (ObjHelper.GetAnonymousValue(schet, "CODE_MO") != null) writer1.WriteElementString("CODE_MO", (string)ObjHelper.GetAnonymousValue(schet, "CODE_MO"));
                            if (ObjHelper.GetAnonymousValue(schet, "YEAR") != null) writer1.WriteElementString("YEAR", Convert.ToString(ObjHelper.GetAnonymousValue(schet, "YEAR")));
                            if (ObjHelper.GetAnonymousValue(schet, "MONTH") != null) writer1.WriteElementString("MONTH", Convert.ToString(ObjHelper.GetAnonymousValue(schet, "MONTH")));
                            if (ObjHelper.GetAnonymousValue(schet, "NSCHET") != null) writer1.WriteElementString("NSCHET", (string)ObjHelper.GetAnonymousValue(schet, "NSCHET"));
                            if (ObjHelper.GetAnonymousValue(schet, "DSCHET") != null) writer1.WriteElementString("DSCHET", ((DateTime)ObjHelper.GetAnonymousValue(schet, "DSCHET")).Date.ToString("yyyy-MM-dd"));
                            if (ObjHelper.GetAnonymousValue(schet, "PLAT") != null) writer1.WriteElementString("PLAT", (string)ObjHelper.GetAnonymousValue(schet, "PLAT"));

                            if ((SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu && isTfomsSchet) || SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)
                            {
                                writer1.WriteElementString("SUMMAV", Convert.ToString(sumInTer).Replace(",", "."));
                            }
                            else
                            {
                                if (ObjHelper.GetAnonymousValue(schet, "SUMMAV") != null) writer1.WriteElementString("SUMMAV", Convert.ToString((decimal?)ObjHelper.GetAnonymousValue(schet, "SUMMAV")).Replace(",", "."));
                                else writer1.WriteElementString("SUMMAV", "0.00");
                            }
                            if (ObjHelper.GetAnonymousValue(schet, "COMENTS") != null) writer1.WriteElementString("COMENTS", (string)ObjHelper.GetAnonymousValue(schet, "COMENTS"));
                            if (ObjHelper.GetAnonymousValue(schet, "SUMMAP") != null && (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)) writer1.WriteElementString("SUMMAP", Convert.ToString(ObjHelper.GetAnonymousValue(schet, "SUMMAP")).Replace(",", "."));
                            if (ObjHelper.GetAnonymousValue(schet, "SANK_MEK") != null && (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)) writer1.WriteElementString("SANK_MEK", Convert.ToString(ObjHelper.GetAnonymousValue(schet, "SANK_MEK")).Replace(",", "."));
                            if (ObjHelper.GetAnonymousValue(schet, "SANK_MEE") != null && (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)) writer1.WriteElementString("SANK_MEE", Convert.ToString(ObjHelper.GetAnonymousValue(schet, "SANK_MEE")).Replace(",", "."));
                            if (ObjHelper.GetAnonymousValue(schet, "SANK_EKMP") != null && (SprClass.ProdSett.OrgTypeStatus == OrgType.Smo || SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms)) writer1.WriteElementString("SANK_EKMP", Convert.ToString(ObjHelper.GetAnonymousValue(schet, "SANK_EKMP")).Replace(",", "."));
                            if ((string)ObjHelper.GetAnonymousValue(schet, "COMENTS") != null)
                            {
                                var com = (string)ObjHelper.GetAnonymousValue(schet, "COMENTS");
                                if (com.StartsWith("DP"))
                                    writer1.WriteElementString("DISP", "ДВ1");
                                if (com.StartsWith("DV"))
                                    writer1.WriteElementString("DISP", "ДВ2");
                                if (com.StartsWith("DO"))
                                    writer1.WriteElementString("DISP", "ОПВ");
                                if (com.StartsWith("DS"))
                                    writer1.WriteElementString("DISP", "ДС1");
                                if (com.StartsWith("DU"))
                                    writer1.WriteElementString("DISP", "ДС2");
                                if (com.StartsWith("DF"))
                                    writer1.WriteElementString("DISP", "ОН1");
                                if (com.StartsWith("DD"))
                                    writer1.WriteElementString("DISP", "ОН2");
                                if (com.StartsWith("DR"))
                                    writer1.WriteElementString("DISP", "ОН3");
                            }

                            writer1.WriteEndElement();
                            //
                            // writer2
                            writer2.WriteStartElement("PERS_LIST");
                            writer2.WriteStartElement("ZGLV");
                            writer2.WriteElementString("VERSION", "2.1");
                            writer2.WriteElementString("DATA", DateTime.Now.Date.ToString("yyyy-MM-dd"));
                            writer2.WriteElementString("FILENAME", filename2);
                            writer2.WriteElementString("FILENAME1", filename1);
                            writer2.WriteEndElement();
                            //

                            foreach (var zap in pacientList)
                            {
                                var pacWriter3 = false;
                                var pacWriter1 = false;

                                //writer2
                                writer2.WriteStartElement("PERS");
                                if (zap.ID_PAC != null) writer2.WriteElementString("ID_PAC", zap.ID_PAC);
                                if (zap.FAM != null) writer2.WriteElementString("FAM", (zap.FAM));
                                if (zap.IM != null) writer2.WriteElementString("IM", zap.IM);
                                if (zap.OT != null) writer2.WriteElementString("OT", zap.OT);
                                if (zap.W != null) writer2.WriteElementString("W", Convert.ToString(zap.W));
                                if (zap.DR != null) writer2.WriteElementString("DR", zap.DR.Value.ToString("yyyy-MM-dd"));
                                //if (zap.DOST != null) writer2.WriteElementString("DOST", zap.DOST.ToString());
                                if (zap.FAM_P != null) writer2.WriteElementString("FAM_P", zap.FAM_P);
                                if (zap.IM_P != null) writer2.WriteElementString("IM_P", zap.IM_P);
                                if (zap.OT_P != null) writer2.WriteElementString("OT_P", zap.OT_P);
                                if (zap.W_P != null) writer2.WriteElementString("W_P", zap.W_P.ToString());
                                if (zap.DR_P != null) writer2.WriteElementString("DR_P", zap.DR_P.Value.ToString("yyyy-MM-dd"));
                                if (zap.MR != null) writer2.WriteElementString("MR", zap.MR);
                                if (zap.DOCTYPE != null) writer2.WriteElementString("DOCTYPE", zap.DOCTYPE.ToString());
                                if (zap.DOCSER != null) writer2.WriteElementString("DOCSER", (zap.DOCSER));
                                if (zap.DOCNUM != null) writer2.WriteElementString("DOCNUM", (zap.DOCNUM));
                                if (zap.SNILS != null) writer2.WriteElementString("SNILS", zap.SNILS);
                                if (zap.OKATOG != null) writer2.WriteElementString("OKATOG", zap.OKATOG);
                                if (zap.OKATOP != null) writer2.WriteElementString("OKATOP", zap.OKATOP);
                                if (zap.COMENTP != null) writer2.WriteElementString("COMENTP", zap.COMENTP);
                                writer2.WriteEndElement();
                                //
                                foreach (var slu in sluchList.Where(x => x.PID == zap.ID))
                                {
                                    if (pacWriter1 == false)
                                    {
                                        // writer1
                                        writer1.WriteStartElement("ZAP");
                                        writer1.WriteElementString("N_ZAP", Convert.ToString(nzap1++));
                                        writer1.WriteElementString("PR_NOV", "0");
                                        writer1.WriteStartElement("PACIENT");
                                        if (zap.ID_PAC != null) writer1.WriteElementString("ID_PAC", zap.ID_PAC);
                                        if (zap.VPOLIS != null) writer1.WriteElementString("VPOLIS", Convert.ToString(zap.VPOLIS));
                                        if (zap.SPOLIS != null) writer1.WriteElementString("SPOLIS", zap.SPOLIS);
                                        if (zap.NPOLIS != null) writer1.WriteElementString("NPOLIS", zap.NPOLIS);
                                        if (zap.SMO != null) writer1.WriteElementString("SMO", zap.SMO);
                                        if (zap.SMO_OGRN != null && zap.SMO == null) writer1.WriteElementString("SMO_OGRN", zap.SMO_OGRN);
                                        if (zap.SMO_OK != null && zap.SMO == null) writer1.WriteElementString("SMO_OK", zap.SMO_OK);
                                        if (zap.SMO_NAM != null && zap.SMO == null) writer1.WriteElementString("SMO_NAM", zap.SMO_NAM);
                                        if (zap.NOVOR != null) writer1.WriteElementString("NOVOR", zap.NOVOR);
                                        else writer1.WriteElementString("NOVOR", "0");
                                        writer1.WriteEndElement();
                                        pacWriter1 = true;
                                    }

                                    writer1.WriteStartElement("SLUCH");
                                    writer1.WriteElementString("IDCASE", idcase1++.ToString());
                                    if (plat == null || plat.StartsWith("46")) writer1.WriteElementString("ID", slu.ID.ToString());
                                    if (plat == null || plat.StartsWith("46")) writer1.WriteElementString("IDSLG", slu.IDSLG.ToString());
                                    if (slu.USL_OK != null) writer1.WriteElementString("USL_OK", slu.USL_OK.ToString());
                                    if (slu.VIDPOM != null) writer1.WriteElementString("VIDPOM", slu.VIDPOM.ToString());
                                    if (slu.FOR_POM != null) writer1.WriteElementString("FOR_POM", slu.FOR_POM.ToString());
                                    if (slu.VID_HMP != null) writer1.WriteElementString("VID_HMP", slu.VID_HMP.ToString());
                                    if (slu.METOD_HMP != null) writer1.WriteElementString("METOD_HMP", slu.METOD_HMP.ToString());
                                    if (slu.NPR_MO != null) writer1.WriteElementString("NPR_MO", slu.NPR_MO);
                                    if (slu.EXTR != null) writer1.WriteElementString("EXTR", slu.EXTR.ToString());
                                    if (slu.LPU != null) writer1.WriteElementString("LPU", slu.LPU);
                                    if (slu.LPU_1 != null) writer1.WriteElementString("LPU_1", slu.LPU_1);
                                    if (slu.PODR != null) writer1.WriteElementString("PODR", slu.PODR.ToString());
                                    if (slu.PROFIL != null) writer1.WriteElementString("PROFIL", slu.PROFIL.ToString());
                                    if (slu.DET != null) writer1.WriteElementString("DET", slu.DET.ToString());
                                    if (!string.IsNullOrWhiteSpace(slu.NHISTORY)) writer1.WriteElementString("NHISTORY", slu.NHISTORY);
                                    else writer1.WriteElementString("NHISTORY", slu.ID.ToString());
                                    if (slu.DATE_1 != null) writer1.WriteElementString("DATE_1", slu.DATE_1.Value.ToString("yyyy-MM-dd"));
                                    if (slu.DATE_2 != null) writer1.WriteElementString("DATE_2", slu.DATE_2.Value.ToString("yyyy-MM-dd"));
                                    if (slu.DS0 != null) writer1.WriteElementString("DS0", slu.DS0);
                                    if (slu.DS1 != null) writer1.WriteElementString("DS1", (slu.DS1));
                                    foreach (var ds2 in ds2List.Where(x => x.SLID == slu.ID))
                                    {
                                        if (ds2.DS != null) writer1.WriteElementString("DS2", ds2.DS);
                                    }
                                    foreach (var ds3 in ds3List.Where(x => x.SLID == slu.ID))
                                    {
                                        if (ds3.DS != null) writer1.WriteElementString("DS3", ds3.DS);
                                    }
                                    if (slu.KSG_OPLATA != null && plat.StartsWith("57")) writer1.WriteElementString("IDTKSG", slu.KSG_OPLATA.ToString());
                                    if (slu.IDKSG != null && plat.StartsWith("57")) writer1.WriteElementString("CODE_KSG", slu.IDKSG.ToString());
                                    if (slu.CODE_MES1 != null) writer1.WriteElementString("CODE_MES1", slu.CODE_MES1);
                                    if (slu.CODE_MES2 != null) writer1.WriteElementString("CODE_MES2", slu.CODE_MES2);
                                    if (slu.SLUCH_TYPE != null && plat.StartsWith("46")) writer1.WriteElementString("SLUCH_TYPE", slu.SLUCH_TYPE.ToString());
                                    if (slu.RSLT != null) writer1.WriteElementString("RSLT", slu.RSLT.ToString());
                                    if (slu.ISHOD != null) writer1.WriteElementString("ISHOD", slu.ISHOD.ToString());
                                    if (slu.MSPID != null)
                                    {
                                        writer1.WriteElementString("PRVS", slu.MSPID.ToString());
                                        //if (slu.PRVD != null && plat.StartsWith("57")) writer1.WriteElementString("PRVD", slu.PRVD.ToString());
                                        writer1.WriteElementString("VERS_SPEC", "V015");
                                    }
                                    else
                                    {
                                        if (slu.PRVS != null && plat.StartsWith("57")) writer1.WriteElementString("PRVS", slu.PRVS.ToString());

                                    }
                                    //if (slu.CELP != null && plat.StartsWith("57")) writer1.WriteElementString("CEL_P", slu.CELP.ToString().Replace(",", "."));
                                    if (slu.IDDOKT != null) writer1.WriteElementString("IDDOKT", slu.IDDOKT);
                                    if (slu.OS_SLUCH != null) writer1.WriteElementString("OS_SLUCH", slu.OS_SLUCH.ToString());
                                    if (slu.OS_SLUCH_REGION != null && plat.StartsWith("46")) writer1.WriteElementString("OS_SLUCH_REGION", slu.OS_SLUCH_REGION.ToString());
                                    if (slu.VETERAN != null && plat.StartsWith("46")) writer1.WriteElementString("VETERAN", slu.VETERAN.ToString());
                                    if (slu.GR_ZDOROV != null && plat.StartsWith("46")) writer1.WriteElementString("GR_ZDOROV", slu.GR_ZDOROV.ToString());
                                    if (slu.SCHOOL != null && plat.StartsWith("46")) writer1.WriteElementString("SCHOOL", slu.SCHOOL.ToString());
                                    if (slu.WORK_STAT != null && plat.StartsWith("46")) writer1.WriteElementString("WORK_STAT", slu.WORK_STAT.ToString());
                                    if (slu.IDSP != null) writer1.WriteElementString("IDSP", slu.IDSP.ToString());
                                    if (slu.IDKSG != null && plat.StartsWith("46")) writer1.WriteElementString("IDKSG", slu.IDKSG.ToString());
                                    if (slu.KSG_OPLATA != null && plat.StartsWith("46")) writer1.WriteElementString("KSG_OPLATA", slu.KSG_OPLATA.ToString());
                                    if (slu.KDAY != null && plat.StartsWith("46")) writer1.WriteElementString("KDAY", slu.KDAY.ToString());
                                    if (slu.ED_COL != null) writer1.WriteElementString("ED_COL", slu.ED_COL.ToString().Replace(",", "."));
                                    if (slu.TARIF != null) writer1.WriteElementString("TARIF", slu.TARIF.ToString().Replace(",", "."));
                                    //if (slu.PK1 != null && plat.StartsWith("57")) writer1.WriteElementString("PK1", slu.PK1.ToString().Replace(",", "."));
                                    //if (slu.PK2 != null && plat.StartsWith("57")) writer1.WriteElementString("PK2", slu.PK2.ToString().Replace(",", "."));
                                    //if (slu.PK3 != null && plat.StartsWith("57")) writer1.WriteElementString("PK3", slu.PK3.ToString().Replace(",", "."));
                                    if (slu.SUMV != null) writer1.WriteElementString("SUMV", slu.SUMV.ToString().Replace(",", "."));
                                    if (slu.OPLATA != null) writer1.WriteElementString("OPLATA", slu.OPLATA.ToString());
                                    if (slu.SUMP != null) writer1.WriteElementString("SUMP", slu.SUMP.ToString().Replace(",", "."));
                                    if (slu.SANK_IT != null) writer1.WriteElementString("SANK_IT", slu.SANK_IT.ToString().Replace(",", "."));
                                    if (slu.COMENTSL != null) writer1.WriteElementString("COMENTSL", slu.COMENTSL);

                                    foreach (var sank in sankList.Where(x => x.D3_ZSLID == slu.ID))
                                    {
                                        writer1.WriteStartElement("SANK");
                                        writer1.WriteElementString("S_CODE", Guid.NewGuid().ToString());
                                        if (sank.S_SUM != null) writer1.WriteElementString("S_SUM", sank.S_SUM.ToString().Replace(",", "."));
                                        if (sank.S_TIP != null) writer1.WriteElementString("S_TIP", sank.S_TIP.ToString());
                                        if (sank.S_OSN != null) writer1.WriteElementString("S_OSN", sank.S_OSN);
                                        if (sank.S_DATE != null) writer1.WriteElementString("S_DATE", sank.S_DATE.Value.ToString("yyyy-MM-dd"));
                                        if (sank.S_COM != null) writer1.WriteElementString("S_COM", sank.S_COM);
                                        if (sank.S_IST != null) writer1.WriteElementString("S_IST", sank.S_IST.ToString());
                                        writer1.WriteEndElement();
                                    }
                                    int idserv = 1;
                                    foreach (var usl in uslList.Where(x => x.SLID == slu.ID))
                                    {
                                        writer1.WriteStartElement("USL");
                                        writer1.WriteElementString("IDSERV", idserv++.ToString());
                                        if (usl.LPU != null) writer1.WriteElementString("LPU", usl.LPU);
                                        if (usl.LPU_1 != null || usl.LPU_1 != usl.LPU) writer1.WriteElementString("LPU_1", usl.LPU_1); //
                                        if (usl.PODR != null) writer1.WriteElementString("PODR", usl.PODR.ToString()); //
                                        if (usl.PROFIL != null) writer1.WriteElementString("PROFIL", usl.PROFIL.ToString());
                                        if (usl.VID_VME != null) writer1.WriteElementString("VID_VME", usl.VID_VME);
                                        if (usl.DET != null) writer1.WriteElementString("DET", usl.DET.ToString());
                                        if (usl.DATE_IN != null) writer1.WriteElementString("DATE_IN", usl.DATE_IN.Value.ToString("yyyy-MM-dd"));
                                        if (usl.DATE_OUT != null) writer1.WriteElementString("DATE_OUT", usl.DATE_OUT.Value.ToString("yyyy-MM-dd"));
                                        if (usl.DS != null) writer1.WriteElementString("DS", usl.DS);
                                        if (usl.IDKSG != null && plat.StartsWith("46")) writer1.WriteElementString("IDKSG", usl.IDKSG.ToString().Replace(",", "."));
                                        if (usl.KSGOPLATA != null && plat.StartsWith("46")) writer1.WriteElementString("KSGOPLATA", usl.KSGOPLATA.ToString().Replace(",", "."));
                                        if (usl.DIFF_K != null && plat.StartsWith("46")) writer1.WriteElementString("DIFF_K", usl.DIFF_K.ToString().Replace(",", "."));
                                        if (usl.KDAY != null && plat.StartsWith("46")) writer1.WriteElementString("KDAY", usl.KDAY.ToString());
                                        if (usl.CODE_USL != null) writer1.WriteElementString("CODE_USL", usl.CODE_USL);
                                        if (usl.KOL_USL != null) writer1.WriteElementString("KOL_USL", usl.KOL_USL.ToString().Replace(",", "."));
                                        if (usl.TARIF != null) writer1.WriteElementString("TARIF", usl.TARIF.ToString().Replace(",", "."));
                                        if (usl.SUMV_USL != null) writer1.WriteElementString("SUMV_USL", usl.SUMV_USL.ToString().Replace(",", "."));
                                        if (usl.MSPUID != null) writer1.WriteElementString("PRVS", usl.MSPUID.ToString());
                                        //if (slu.PRVD != null && plat.StartsWith("57")) writer1.WriteElementString("PRVD", slu.PRVD.ToString());
                                        if (usl.CODE_MD != null) writer1.WriteElementString("CODE_MD", usl.CODE_MD);
                                        else if (slu.IDDOKT != null) writer1.WriteElementString("CODE_MD", slu.IDDOKT);
                                        else writer1.WriteElementString("CODE_MD", "0");
                                        if (usl.USL_PRR != null && plat.StartsWith("46")) writer1.WriteElementString("USL_PRR", Convert.ToByte(usl.USL_PRR).ToString());
                                        if (usl.USL_OTK != null && plat.StartsWith("46")) writer1.WriteElementString("USL_OTKAZ", Convert.ToByte(usl.USL_OTK).ToString());
                                        if (usl.COMENTU != null) writer1.WriteElementString("COMENTU", usl.COMENTU); //
                                        writer1.WriteEndElement();
                                    }
                                    writer1.WriteEndElement();
                                    if (isWriter3)
                                    {
                                        writer3.WriteEndElement();
                                    }
                                }
                                if (pacWriter1 == true)
                                    writer1.WriteEndElement();
                                if (isWriter3)
                                {
                                    writer3.WriteEndElement();
                                }

                            }
                            if (isWriter3)
                            {
                                writer3.WriteEndElement();
                                writer3.Flush();
                            }
                            writer3.Close();
                        }
                        writer2.WriteEndElement();
                        writer2.Flush();
                        writer2.Close();
                    }
                    writer1.WriteEndElement();
                    writer1.Flush();
                    writer1.Close();
                }

                string result1 = Encoding.Default.GetString(stream1.ToArray());
                string result2 = Encoding.Default.GetString(stream2.ToArray());
                string result3 = Encoding.Default.GetString(stream3.ToArray());

                zip.AddEntry(filename1, result1);
                zip.AddEntry(filename2, result2);
                if (isWriter3)
                    zip.AddEntry(filename3 + ".xml", result3);
                if (isLoop)
                {
                    if (!Directory.Exists(Environment.CurrentDirectory + @"\Out"))
                        Directory.CreateDirectory(Environment.CurrentDirectory + @"\Out");

                    var zipFile = Environment.CurrentDirectory + @"\Out\" + filename1 + ".oms";
                    zip.Save(zipFile);
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        if (plat.StartsWith("57"))
                        {
                            var num = isWriter3 ? "3" : "2";
                            saveFileDialog.Filter = "ZIP File (*.zip)|*.zip";
                            saveFileDialog.FileName = filename1.Insert(1, num) + ".zip";
                        }
                        else
                        {
                            saveFileDialog.Filter = "OMS File (*.oms)|*.oms";
                            saveFileDialog.FileName = filename1 + ".oms";
                        }

                        bool? result = saveFileDialog.ShowDialog();
                        if (result == true)
                            zip.Save(saveFileDialog.FileName);
                    });
                }
            }
            //LoadingDecorator1.IsSplashScreenShown = false;
        }

        private void ScImport2Item_OnItemClick(object sender, ItemClickEventArgs e)
        {
//            OpenFileDialog openFileDialog = new OpenFileDialog();
//            openFileDialog.Filter = "OMS File |*.oms;*.zip";


//            var result = openFileDialog.ShowDialog();

//            var rfile = openFileDialog.FileName;
//            var omszfn = openFileDialog.SafeFileName;

//            if (result != true) return;

//            //var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(DxHelper.GetSelectedGridRow(gridControl));
//            ((BarButtonItem)sender).IsEnabled = false;
//            LoadingDecorator1.IsSplashScreenShown = true;
//            Task.Factory.StartNew(() =>
//            {
//                try
//                {
//                    using (ZipFile zip = ZipFile.Read(rfile))
//                    {
//                        foreach (ZipEntry zipEntry in zip)
//                        {
//                            var ms = new MemoryStream();

//                            var fd = zipEntry.CreationTime;
//                            var fn = zipEntry.FileName;
//                            zipEntry.Extract(ms);
//                            ms.Position = 0;

//                            var sr = new StreamReader(ms, Encoding.Default);
//                            string xml = sr.ReadToEnd();
//                            ms.Dispose();
//                            ms.Close();
//                            sr.Dispose();sr.Close();

//                            var q = $@"

//                        BEGIN TRANSACTION;  
//                        BEGIN TRY

//                            Declare @sc int
//                            INSERT INTO [dbo].[D3_SCHET_OMS_FILES] ([FileXML],[FileName],[FileDate],[D3_SCID]) VALUES (CAST('{xml.Replace("'", "")}' AS XML), '{fn}', '{fd.ToString("yyyyMMdd HH:mm")}',
//                                                                                                                                                                                CAST('{x_xml.Replace("'", "")}' AS XML), CAST('{t_xml.Replace("'", "")}' AS XML), '{x_fn}', '{t_fn}')
//                        END TRY
//                        BEGIN CATCH
//                            IF @@TRANCOUNT > 0  
//                                ROLLBACK TRANSACTION;  
//                            --PRINT 'In catch block.';
//                        DECLARE
//                           @ErMessage NVARCHAR(2048),
//                           @ErSeverity INT,
//                           @ErState INT

//                         SELECT
//                           @ErMessage = ERROR_MESSAGE(),
//                           @ErSeverity = ERROR_SEVERITY(),
//                           @ErState = ERROR_STATE()

//                         RAISERROR (@ErMessage,
//                                     @ErSeverity,
//                                     @ErState )
//                        END CATCH;

//                        IF @@TRANCOUNT > 0  
//                            COMMIT TRANSACTION;  
//                        ";

//                            zapxml = null;
//                            persxml = null;
//                            Reader2List.CustomExecuteQuery(q, SprClass.LocalConnectionString);
//                            q = null;

//                        }
//                    }
//                    //"0x" + BitConverter.ToString(arraytoinsert).Replace("-", "")
//                }
//                catch (Exception ex)
//                {
//                    Dispatcher.BeginInvoke((Action)delegate ()
//                    {
//                        DXMessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException?.Message);
//                    });
//                }


//                var zapsr = new StreamReader(zapms, Encoding.Default);
//                string zapxml = zapsr.ReadToEnd();
//                zapms.Dispose();
//                zapms.Close();

//                var perssr = new StreamReader(persms, Encoding.Default);
//                string persxml = perssr.ReadToEnd();
//                persms.Dispose();
//                persms.Close();

//                var x_sr = new StreamReader(x_ms, Encoding.Default);
//                string x_xml = x_sr.ReadToEnd();
//                x_sr.Dispose();
//                x_sr.Close();

//                var t_sr = new StreamReader(t_ms, Encoding.Default);
//                string t_xml = t_sr.ReadToEnd();
//                t_sr.Dispose();
//                t_sr.Close();
//                if (string.IsNullOrEmpty(zapxml) || string.IsNullOrEmpty(persxml))
//                {
//                    //Reader2List.CustomExecuteQuery($@"Update DOX_SCHET SET DOX_STATUS=12 ", SprClass.LocalConnectionString);
//                }
//                else
//                {
//                    try
//                    {
//                        var q = $@"

//                        BEGIN TRANSACTION;  
//                        BEGIN TRY

//                            Declare @sc int
//                            INSERT INTO D3_SCHET_OMS (ZAPXMLFILE, PERSXMLFILE, ZAPFILENAME, PERSFILENAME, X_XmlFile, T_XmlFile, X_FileName, T_FileName) VALUES (CAST('{zapxml.Replace("'", "")}' AS XML), CAST('{persxml.Replace("'", "")}' AS XML), '{zapfn}', '{persfn}',
//                                                                                                                                                                                CAST('{x_xml.Replace("'", "")}' AS XML), CAST('{t_xml.Replace("'", "")}' AS XML), '{x_fn}', '{t_fn}')
//--                            Select @sc = SCOPE_IDENTITY()
//--
//--                            EXEC p_oms_load_schet @sc
//--                            EXEC p_oms_load_pacient @sc
//--                            EXEC p_oms_load_zsl @sc
//--                            EXEC p_oms_load_sl @sc
//--                            EXEC p_oms_load_dss @sc
//--                            EXEC p_oms_load_usl @sc

//--update d3_zsl_oms set usl_ok = sl.usl_ok, p_cel = sl.p_cel
//--from d3_zsl_oms zsl
//--join d3_sl_oms sl on zsl.ID = sl.D3_ZSLID
//--where zsl.USL_OK is null or zsl.P_CEL is null

//                        END TRY
//                        BEGIN CATCH
//                            IF @@TRANCOUNT > 0  
//                                ROLLBACK TRANSACTION;  
//                            --PRINT 'In catch block.';
//                        DECLARE
//                           @ErMessage NVARCHAR(2048),
//                           @ErSeverity INT,
//                           @ErState INT

//                         SELECT
//                           @ErMessage = ERROR_MESSAGE(),
//                           @ErSeverity = ERROR_SEVERITY(),
//                           @ErState = ERROR_STATE()

//                         RAISERROR (@ErMessage,
//                                     @ErSeverity,
//                                     @ErState )
//                        END CATCH;

//                        IF @@TRANCOUNT > 0  
//                            COMMIT TRANSACTION;  
//                        ";

//                        zapxml = null;
//                        persxml = null;
//                        Reader2List.CustomExecuteQuery(q, SprClass.LocalConnectionString);
//                        q = null;
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine(ex);
//                        Dispatcher.BeginInvoke((Action)delegate ()
//                        {
//                            DXMessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException?.Message);
//                        });
//                        //Reader2List.CustomExecuteQuery($@"Update DOX_SCHET SET DOX_STATUS=13 WHERE ID = {id}", _connectionString);
//                        //return;
//                    }
//                }
//                zapsr.Dispose();
//                zapsr.Close();
//                perssr.Dispose();
//                perssr.Close();


//                //Console.WriteLine("Распакован " + id);

//                GC.WaitForPendingFinalizers();
//                GC.Collect();

//            }).ContinueWith(x =>
//            {
//                linqInstantFeedbackDataSource.Refresh();

//                DXMessageBox.Show("Загрузка успешно завершена");

//                ((BarButtonItem)sender).IsEnabled = true;
//                LoadingDecorator1.IsSplashScreenShown = false;


//            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ScImport3Item_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var row = DxHelper.GetSelectedGridRow(gridControl);
            var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(row);

            try
            {
                var q = $@"

                        BEGIN TRANSACTION;  
                        BEGIN TRY


                            EXEC p_oms_load_schet {sc.ID}


--update d3_zsl_oms set usl_ok = sl.usl_ok, p_cel = sl.p_cel
--from d3_zsl_oms zsl
--join d3_sl_oms sl on zsl.ID = sl.D3_ZSLID
--where zsl.USL_OK is null or zsl.P_CEL is null

                        END TRY
                        BEGIN CATCH
                            IF @@TRANCOUNT > 0  
                                ROLLBACK TRANSACTION;  
                            --PRINT 'In catch block.';
                        DECLARE
                           @ErMessage NVARCHAR(2048),
                           @ErSeverity INT,
                           @ErState INT

                         SELECT
                           @ErMessage = ERROR_MESSAGE(),
                           @ErSeverity = ERROR_SEVERITY(),
                           @ErState = ERROR_STATE()

                         RAISERROR (@ErMessage,
                                     @ErSeverity,
                                     @ErState )
                        END CATCH;

                        IF @@TRANCOUNT > 0  
                            COMMIT TRANSACTION;  
                        ";
                Reader2List.CustomExecuteQuery(q, SprClass.LocalConnectionString);
                q = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    DXMessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException?.Message);
                });
                //Reader2List.CustomExecuteQuery($@"Update DOX_SCHET SET DOX_STATUS=13 WHERE ID = {id}", _connectionString);
                //return;
            }
        }

        private void ScExport3Item_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var schets = DxHelper.GetSelectedGridRows(gridControl);
            if (schets.Length > 1)
                foreach (var sc1 in schets)
                {
                    var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(sc1);
                    XmlStreemMtr3(sc, true, true);
                }
            else
            {
                var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(schets[0]);
                XmlStreemMtr3(sc, true);
            }

        }

        public void XmlStreemMtr3(object schet, bool isTfomsSchet, bool isLoop = false, int loopCount = 1)
        {

            var qxml = SqlReader.Select($@"
            exec p_oms_export_mtr_30K {ObjHelper.GetAnonymousValue(schet, "ID")}"
            , SprClass.LocalConnectionString);



            string result1 = "<?xml version=\"1.0\" encoding=\"windows-1251\"?>" + (string) qxml[0].GetValue("HM");
            string result2 = "<?xml version=\"1.0\" encoding=\"windows-1251\"?>" + (string) qxml[0].GetValue("LM");
            using (ZipFile zip = new ZipFile(Encoding.GetEncoding("windows-1251")))
            {
                zip.AddEntry((string)qxml[0].GetValue("hf_name")+".xml", result1);
                zip.AddEntry((string)qxml[0].GetValue("lf_name")+".xml", result2);

                if (isLoop)
                {
                    if (!Directory.Exists(Environment.CurrentDirectory + @"\Out"))
                        Directory.CreateDirectory(Environment.CurrentDirectory + @"\Out");

                    var zipFile = Environment.CurrentDirectory + @"\Out\" + (string)qxml[0].GetValue("hf_name") + ".oms";
                    zip.Save(zipFile);
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();

                        saveFileDialog.Filter = "OMS File (*.oms)|*.oms";
                            saveFileDialog.FileName = (string)qxml[0].GetValue("hf_name") + ".oms";

                        bool? result = saveFileDialog.ShowDialog();
                        if (result == true)
                            zip.Save(saveFileDialog.FileName);
                    });
                }

            }
            //LoadingDecorator1.IsSplashScreenShown = false;
        }
        private void ScExportEis_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var per = new Period();
            var window = new DXWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = per,
                Title = "Выгрузка в ЕИССОИ",
                SizeToContent = SizeToContent.WidthAndHeight
            };
            window.ShowDialog();
        }

    }

}