using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ActiveQueryBuilder.Core;
using ActiveQueryBuilder.View.WPF;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.CodeView;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using GalaSoft.MvvmLight.Command;
using MaterialMenu;
using Yamed.Ambulatory;
using Yamed.Control;
using Yamed.Control.Editors;
using Yamed.Core;
using Yamed.Entity;
using Yamed.Oms;
using Yamed.OmsExp;
using Yamed.Pattern;
using Yamed.Registry;
using Yamed.Reports;
using Yamed.Server;


namespace Yamed
{
    /// <summary>
    /// Логика взаимодействия для PrimaryWindow.xaml
    /// </summary>
    public partial class PrimaryWindow : DXTabbedWindow
    {
        //DxTabViewModel _vm = new DxTabViewModel();


        public PrimaryWindow()
        {
            SprClass.LocalConnectionString = Properties.Settings.Default.ConnectionString;
            SprClass.SrzConnectionString = Properties.Settings.Default.SrzConnectionString;

            BaseConnectionDescriptor connection = new MSSQLConnectionDescriptor();
            connection.ConnectionString = SprClass.LocalConnectionString;

            SprClass.Qb = new QueryBuilder {SyntaxProvider = new GenericSyntaxProvider()};
            
            SprClass.Qb.SQLContext.Assign(connection.GetSqlContext());



            //"Data Source=91.240.209.20,1432;Initial Catalog=Elmed;User ID=sa;Password=Hospital6";

            //try
            //{
            //    int scriptNumber = 0;
            //    bool isUpdate = false;
            //    string[] sqlArray = Directory.GetFiles(Environment.CurrentDirectory + @"\Update\", "Script????.sql");
            //    foreach (string s in sqlArray)
            //    {
            //        File.Delete(s);
            //    }

            //    string[] frxArray = Directory.GetFiles(Environment.CurrentDirectory + @"\Update\", "Script????.elm");
            //    foreach (string s in frxArray)
            //    {
            //        var i = s.IndexOf("Script", StringComparison.Ordinal);
            //        string scriptVer = s.Substring(i).Replace("Script", "").Replace(".elm", "");

            //        scriptNumber = Convert.ToInt32(scriptVer);
            //        using (ElmedDataClassesDataContext dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
            //        {
            //            Version v = dc.ExecuteQuery<Version>("SELECT TOP(1) Version as ver FROM DbVersion").Single();
            //            if (v.ver == scriptNumber - 1)
            //            {
            //                dc.CommandTimeout = 0;
            //                string script;
            //                using (
            //                    StreamReader sr = new StreamReader(@"Update\Script" + scriptVer + ".elm",
            //                                                       Encoding.Default))
            //                {
            //                    script = CipherUtility.Decrypt<TripleDESCryptoServiceProvider>(sr.ReadToEnd(),
            //                        "Elmed31948253Crypt", "3dfx");
            //                }

            //                if (!isUpdate)
            //                {
            //                    //using (WaitForm.AsyncWaitDialog.ShowWaitDialog("Идет архивное копирование БД..." + Environment.NewLine + "Это может занять длительное время"))
            //                    {
            //                        dc.ExecuteCommand(string.Format(@"BACKUP DATABASE {0} TO disk='{0}-{1}.bak' WITH init", dc.Connection.Database, "Elmedicine" + "-" + Guid.NewGuid().ToString()));
            //                    }
            //                }
            //                dc.ExecuteCommand(script);
            //                dc.ExecuteCommand("Update DbVersion SET Version = {0}", scriptNumber);
            //                isUpdate = true;
            //            }
            //        }
            //        File.Delete(s);
            //    }
            //    if (isUpdate)
            //        Dispatcher.BeginInvoke((Action)delegate ()
            //        {
            //            ErrorGlobalWindow.ShowError("База данных обновлена до версии - " + scriptNumber);
            //            //                            ErrorGlobalWindow.ShowError("Не закрывайте приложение до завершения автоматического обновления и активации");
            //        });
            //}
            //catch (Exception ex)
            //{
            //    Dispatcher.BeginInvoke((Action)delegate ()
            //    {
            //        ErrorAndClose(ex.Message);
            //    });
            //    return;
            //}


            try
            {
                int scriptNumber = 0;
                bool isUpdate = false;

                string[] frxArray = Directory.GetFiles(Environment.CurrentDirectory + @"\Update\", "Script*.elm");
                foreach (string s in frxArray)
                {
                    var i = s.IndexOf("Script", StringComparison.InvariantCulture);
                    string scriptVer = s.Substring(i).Replace("Script", "").Replace(".elm", "");
                    scriptNumber = Convert.ToInt32(scriptVer);
                    using (ElmedDataClassesDataContext dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
                    {
                        Version v = dc.ExecuteQuery<Version>("SELECT TOP(1) Version as ver FROM DbVersion").Single();
                        if (v.ver == scriptNumber - 1)
                        {
                            dc.CommandTimeout = 0;
                            string script;
                            using (
                                StreamReader sr = new StreamReader(@"Update\Script" + scriptVer + ".elm",
                                                                   Encoding.Default))
                            {
                                script = CipherUtility.Decrypt<TripleDESCryptoServiceProvider>(sr.ReadToEnd(),
                                    "Elmed31948253Crypt", "3dfx");
                            }

                            if (!isUpdate)
                            {
                                //using (WaitForm.AsyncWaitDialog.ShowWaitDialog("Идет архивное копирование БД..." + Environment.NewLine + "Это может занять длительное время"))
                                {
                                    //dc.ExecuteCommand(string.Format(@"BACKUP DATABASE {0} TO disk='{0}-{1}.bak' WITH init", dc.Connection.Database, "Elmedicine" + "-" + Guid.NewGuid().ToString()));
                                }
                            }
                            dc.ExecuteCommand(script);
                            dc.ExecuteCommand("Update DbVersion SET Version = {0}", scriptNumber);
                            isUpdate = true;
                        }
                    }
                    File.Delete(s);
                }
                if (isUpdate)
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        ErrorGlobalWindow.ShowError("База данных обновлена до версии - " + scriptNumber);
                        //                            ErrorGlobalWindow.ShowError("Не закрывайте приложение до завершения автоматического обновления и активации");
                    });
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    ErrorAndClose(ex.Message + Environment.NewLine + ex.InnerException?.Message);
                });
                return;
            }

            PasswordWindow password = new PasswordWindow();
            if (password.ShowDialog() == false)
            {
                ErrorAndClose(null);
                Thread.Sleep(200);
                return;
            }

            InitializeComponent();

            //Yamed.Registry.DragAndDrop.DragManager.Instance.AdornerCanvas = this.adornerLayer;


            СommonСomponents.DxTabControlSource = new DxTabViewModel();
            PrimaryGrid.DataContext = СommonСomponents.DxTabControlSource;

            СommonСomponents.DxTabControlSource.TabElements.CollectionChanged += TabElements_CollectionChanged;

            //new WebUI.VirtualUI().Start();


            //Yamed.Entity.MyTypeBuilder.CreateNewObject();

            //Password password = new Password();
            //if (password.ShowDialog() == false)
            //{
            //    ErrorAndClose(null);
            //}
            //else
            {
                //СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                //{
                //    Header = "Рабочий стол",
                //    MyControl = new DesktopControl(),
                //    IsCloseable = "False",
                //});
                //СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                //{
                //    Header = "Регистратура",
                //    MyControl = new ScheduleControl(),
                //    IsCloseable = "True"
                //});


            }
            ModulesAppBar.DataContext = new ModulesCollection();

        }

        public class Version
        {
            public int ver { get; set; }
        }

        private void TabElements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                new Action(delegate ()
                {
                    DxTabControl1.SelectLast();
                }));

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Decorator.IsSplashScreenShown = false;
            }
        }

        private void ErrorAndClose(string error)
        {
            if (error != null)
                ErrorGlobalWindow.ShowError(error);

            ThreadStart ts = delegate ()
            {
                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    Application.Current.Shutdown();
                });
            };
            var t = new Thread(ts);
            t.Start();
        }

        private void BeckupElmed_OnClick(object sender, RoutedEventArgs e)
        {
            //using (var dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
            //{
            //    using (WaitForm.AsyncWaitDialog.ShowWaitDialog("Идет архивное копирование БД..." + Environment.NewLine + "Это может занять длительное время"))
            //    {
            //        dc.CommandTimeout = 0;
            //        dc.ExecuteCommand(string.Format(@"BACKUP DATABASE {0} TO disk='{0}-{1}.bak' WITH init", dc.Connection.Database, "Elmedicine" + "-" + Guid.NewGuid().ToString()));
            //    }
            //}
        }



        private void PrimaryWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            //dateStatusItem.EditValue = DateTime.Today;
            //biColumn.Content = "Пользователь: " + SprClass.UserFIO;

            Decorator.IsSplashScreenShown = true;

            TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext(); //get UI thread context 
            var mekTask = Task.Factory.StartNew(() =>
            {
//                try
//                {
//                    int scriptNumber = 0;
//                    bool isUpdate = false;
//                    string[] sqlArray = Directory.GetFiles(Environment.CurrentDirectory + @"\Update\", "Script???.sql");
//                    foreach (string s in sqlArray)
//                    {
//                        File.Delete(s);
//                    }

//                    string[] frxArray = Directory.GetFiles(Environment.CurrentDirectory + @"\Update\", "Script???.elm");
//                    foreach (string s in frxArray)
//                    {
//                        string scriptVer = s.Substring(s.Length - 13).Replace("Script", "").Replace(".elm", "");
//                        scriptNumber = Convert.ToInt32(scriptVer);
//                        using (ElmedDataClassesDataContext dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
//                        {
//                            Version v = dc.ExecuteQuery<Version>("SELECT TOP(1) Version as ver FROM DbVersion").Single();
//                            if (v.ver == scriptNumber - 1)
//                            {
//                                dc.CommandTimeout = 0;
//                                string script;
//                                using (
//                                    StreamReader sr = new StreamReader(@"Update\Script" + scriptVer + ".elm",
//                                                                       Encoding.Default))
//                                {
//                                    script = CipherUtility.Decrypt<TripleDESCryptoServiceProvider>(sr.ReadToEnd(),
//                                        "Elmed31948253Crypt", "3dfx");
//                                }

//                                if (!isUpdate)
//                                {
//                                    //using (WaitForm.AsyncWaitDialog.ShowWaitDialog("Идет архивное копирование БД..." + Environment.NewLine + "Это может занять длительное время"))
//                                    {
//                                        dc.ExecuteCommand(string.Format(@"BACKUP DATABASE {0} TO disk='{0}-{1}.bak' WITH init", dc.Connection.Database, "Elmedicine" + "-" + Guid.NewGuid().ToString()));
//                                    }
//                                }
//                                dc.ExecuteCommand(script);
//                                dc.ExecuteCommand("Update DbVersion SET Version = {0}", scriptNumber);
//                                isUpdate = true;
//                            }
//                        }
//                        File.Delete(s);
//                    }
//                    if (isUpdate)
//                        Dispatcher.BeginInvoke((Action)delegate ()
//                        {
//                            ErrorGlobalWindow.ShowError("База данных обновлена до версии - " + scriptNumber);
////                            ErrorGlobalWindow.ShowError("Не закрывайте приложение до завершения автоматического обновления и активации");
//                        });
//                }
//                catch (Exception ex)
//                {
//                    Dispatcher.BeginInvoke((Action)delegate ()
//                    {
//                        ErrorAndClose(ex.Message);
//                    });
//                    return;
//                }
            });
            mekTask.ContinueWith(x =>
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        SprClass.SprLoad();
                    }
                    catch (Exception ex)
                    {
                        ErrorAndClose(ex.Message);
                        Thread.Sleep(200);
                    }
                })
                .ContinueWith(x1 =>
                {
                    foreach (var child in ((StackPanel)Menu.Menu.Content).Children)
                    {
                        if (!(child is Button)) continue;

                        var b = (Button)child;
                        if ((string)b.Tag == "OMS" &&
                            (SprClass.ProdSett.OrgTypeStatus == OrgType.Tfoms || SprClass.ProdSett.OrgTypeStatus == OrgType.Smo))
                            ((Button)child).Visibility = Visibility.Visible;

                        if ((string)b.Tag == "LPU" && SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu)
                            ((Button)child).Visibility = Visibility.Visible;
                    }

                    Decorator.IsSplashScreenShown = false;
                }, taskScheduler);
            }, taskScheduler);






        }
        private void SqlScriptRun_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = ((TabElement)СommonСomponents.DxTabObject).MyControl;
            //if (tab is MedicalEconomicControl.SqlEditorControl)
            //{
            //    var tab1 = (MedicalEconomicControl.SqlEditorControl)tab;
            //    MedicalEconomicControl.SqlExecute.GetData(tab1.GridControl1, tab1.sqlEditor.Text);
            //}
            //else if (tab is CustomStatistic)
            //{
            //    var tab1 = (CustomStatistic)tab;
            //    Statistic.SqlExecute.GetData(tab1.GridControl1, tab1.sqlEditor.Text);
            //}
            //else
            //{
            //    var tab1 = (MedicalExpertQuery)tab;
            //    MedicalExperts.SqlExecute.GetData(tab1.GridControl1, tab1.sqlEditor.Text);

            //}

        }


        private void RefreshTbl_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = ((TabElement)СommonСomponents.DxTabObject).MyControl;
            //if (tab is MedicalEconomicControl.SqlEditorControl)
            //{
            //    var tab1 = (MedicalEconomicControl.SqlEditorControl)tab;
            //    MedicalEconomicControl.SqlExecute.UpdateData(tab1.MekList);
            //}
            //else if (tab is CustomStatistic)
            //{
            //    var tab1 = (CustomStatistic)tab;
            //    Statistic.SqlExecute.UpdateData(tab1.StatList);
            //}
            //else if (tab is MedicalExpertQuery)
            //{
            //    var tab1 = (MedicalExpertQuery)tab;
            //    MedicalExperts.SqlExecute.UpdateData(tab1.MeeQueryList1);
            //}

        }

        private void AddItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = ((TabElement)СommonСomponents.DxTabObject).MyControl;

            //if (tab is MedicalEconomicControl.SqlEditorControl)
            //{
            //    var tab1 = (MedicalEconomicControl.SqlEditorControl)tab;
            //    MedicalEconomicControl.SqlExecute.AddData(tab1.MekList);
            //}
            //else if (tab is CustomStatistic)
            //{
            //    var tab1 = (CustomStatistic)tab;
            //    Statistic.SqlExecute.AddData(tab1.StatList);
            //}
            //else if (tab is MedicalExpertQuery)
            //{
            //    var tab1 = (MedicalExpertQuery)tab;
            //    MedicalExperts.SqlExecute.AddData(tab1.MeeQueryList1);
            //}
        }

        private void CopyItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = ((TabElement)СommonСomponents.DxTabObject).MyControl;
            //if (tab is MedicalEconomicControl.SqlEditorControl)
            //{
            //    var tab1 = (MedicalEconomicControl.SqlEditorControl)tab;
            //    MedicalEconomicControl.SqlExecute.CopyData(tab1.MekList);
            //}
            //else if (tab is CustomStatistic)
            //{
            //    var tab1 = (CustomStatistic)tab;
            //    Statistic.SqlExecute.CopyData(tab1.StatList);
            //}
            //else if (tab is MedicalExpertQuery)
            //{
            //    var tab1 = (MedicalExpertQuery)tab;
            //    MedicalExperts.SqlExecute.CopyData(tab1.MeeQueryList1);
            //}
        }


        private void DelItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = ((TabElement)СommonСomponents.DxTabObject).MyControl;
            //if (tab is MedicalEconomicControl.SqlEditorControl)
            //{
            //    var tab1 = (MedicalEconomicControl.SqlEditorControl)tab;
            //    MedicalEconomicControl.SqlExecute.DeleteData(tab1.MekList);
            //}
            //else if (tab is CustomStatistic)
            //{
            //    var tab1 = (CustomStatistic)tab;
            //    Statistic.SqlExecute.DeleteData(tab1.StatList);
            //}
            //else if (tab is MedicalExpertQuery)
            //{
            //    var tab1 = (MedicalExpertQuery)tab;
            //    MedicalExperts.SqlExecute.DeleteData(tab1.MeeQueryList1);
            //}
        }

        private void SaveItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = ((TabElement)СommonСomponents.DxTabObject).MyControl;
            //if (tab is MedicalEconomicControl.SqlEditorControl)
            //{
            //    var tab1 = (MedicalEconomicControl.SqlEditorControl)tab;
            //    MedicalEconomicControl.SqlExecute.SaveData(tab1.MekList);
            //}
            //else if (tab is CustomStatistic)
            //{
            //    var tab1 = (CustomStatistic)tab;
            //    Statistic.SqlExecute.SaveData(tab1.StatList);
            //}
            //else if (tab is MedicalExpertQuery)
            //{
            //    var tab1 = (MedicalExpertQuery)tab;
            //    MedicalExperts.SqlExecute.SaveData(tab1.MeeQueryList1);
            //}
        }

        private void SaveAllItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = ((TabElement)СommonСomponents.DxTabObject).MyControl;
            //if (tab is MedicalEconomicControl.SqlEditorControl)
            //{
            //    var tab1 = (MedicalEconomicControl.SqlEditorControl)tab;
            //    MedicalEconomicControl.SqlExecute.SaveAllData(tab1.MekList);
            //}
            //else if (tab is CustomStatistic)
            //{
            //    var tab1 = (CustomStatistic)tab;
            //    Statistic.SqlExecute.SaveAllData(tab1.StatList);
            //}
            //else if (tab is MedicalExpertQuery)
            //{
            //    var tab1 = (MedicalExpertQuery)tab;
            //    MedicalExperts.SqlExecute.SaveAllData(tab1.MeeQueryList1);
            //}
        }

        private void ExportXlsxItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = ((TabElement)СommonСomponents.DxTabObject).MyControl;
            //if (tab is MedicalEconomicControl.SqlEditorControl)
            //{
            //    var tab1 = (MedicalEconomicControl.SqlEditorControl)tab;
            //    SaveFileDialog saveFileDialog = new SaveFileDialog();
            //    saveFileDialog.Filter = "Excel File (*.xlsx)|*.xlsx";

            //    bool? result = saveFileDialog.ShowDialog();
            //    if (result == true)
            //    {
            //        tab1.TableView1.ExportToXlsx(saveFileDialog.FileName);
            //        Process.Start(saveFileDialog.FileName);
            //    }
            //}
            //else if (tab is MedicalExperts.MedicalExpertQuery)
            //{
            //    var tab1 = (MedicalExpertQuery)tab;
            //    SaveFileDialog saveFileDialog = new SaveFileDialog();
            //    saveFileDialog.Filter = "Excel File (*.xlsx)|*.xlsx";

            //    bool? result = saveFileDialog.ShowDialog();
            //    if (result == true)
            //    {
            //        tab1.TableView1.ExportToXlsx(saveFileDialog.FileName);
            //        Process.Start(saveFileDialog.FileName);
            //    }
            //}
            //else
            //{
            //    var tab1 = (CustomStatistic)tab;
            //    SaveFileDialog saveFileDialog = new SaveFileDialog();
            //    saveFileDialog.Filter = "Excel File (*.xlsx)|*.xlsx";

            //    bool? result = saveFileDialog.ShowDialog();
            //    if (result == true)
            //    {
            //        tab1.TableView1.ExportToXlsx(saveFileDialog.FileName);
            //        Process.Start(saveFileDialog.FileName);
            //    }
            //}
        }

        private void HospitalItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = (ElReestrTabNew)((TabElement)СommonСomponents.DxTabObject).MyControl;
            //СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            //{
            //    Header = "СТАТИСТИЧЕСКАЯ КАРТА ВЫБЫВШЕГО ИЗ СТАЦИОНАРА",
            //    MyControl = new HospitalEmrSluchTab(null, tab._schetId),
            //    IsCloseable = "True"
            //});
        }



        private void EditSluchItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = (ElReestrTabNew)((TabElement)СommonСomponents.DxTabObject).MyControl;
            //var sl = Reader2List.CustomSelect<SLUCH>($"Select * From SLUCH Where ID={ObjHelper.GetAnonymousValue(PublicVoids.GetSelectedGridRow(tab.gridControl1), "ID")}",
            //        SprClass.LocalConnectionString).Single();
            //if (sl.OS_SLUCH_REGION == 22)
            //{
            //    СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            //    {
            //        Header = "ДИСПАНСЕРИЗАЦИЯ",
            //        MyControl = new ElKardDispTab(sl, tab._schetId.ID),
            //        IsCloseable = "True"
            //    });
            //}
            //else if (sl.USL_OK == 1 || sl.USL_OK == 2)
            //{
            //    СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            //    {
            //        Header = "СТАТИСТИЧЕСКАЯ КАРТА ВЫБЫВШЕГО ИЗ СТАЦИОНАРА",
            //        MyControl = new HospitalEmrSluchTab(sl, tab._schetId),
            //        IsCloseable = "True"
            //    });
            //}
            //else
            //{
            //    var sprEditWindow = new UniSprEditPanel("SLUCH", sl, true, SprClass.LocalConnectionString);
            //    var window = new DXWindow
            //    {
            //        ShowIcon = false,
            //        WindowStartupLocation = WindowStartupLocation.Manual,
            //        Content = sprEditWindow,
            //        Title = "Редактирование"
            //    };
            //    window.ShowDialog();
            //}
        }


        private void SluchRefreshTbl_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = (ElReestrTabNew)((TabElement)СommonСomponents.DxTabObject).MyControl;
            //tab._linqInstantFeedbackDataSource.Refresh();
        }

        private void SluchDelItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
    //        MessageBoxResult result = MessageBox.Show("Удалить случай", "Удаление",
    //MessageBoxButton.YesNo, MessageBoxImage.Question);
    //        if (result == MessageBoxResult.Yes)
    //        {
    //            var tab = (ElReestrTabNew)((TabElement)СommonСomponents.DxTabObject).MyControl;
    //            var id = ObjHelper.GetAnonymousValue(PublicVoids.GetSelectedGridRow(tab.gridControl1), "ID");
    //            var uid = Reader2List.CustomSelect<USL>($"Select TOP 1 * FROM USL WHERE SLID={id}", SprClass.LocalConnectionString).SingleOrDefault()?.ID;
    //            if (uid != null)
    //                Reader2List.CustomExecuteQuery($"DELETE USL_ASSIST WHERE UID={uid}", SprClass.LocalConnectionString);
    //            Reader2List.CustomExecuteQuery($"DELETE USL WHERE SLID={id}", SprClass.LocalConnectionString);
    //            Reader2List.CustomExecuteQuery($"DELETE SLUCH_DS2 WHERE SLID={id}", SprClass.LocalConnectionString);
    //            Reader2List.CustomExecuteQuery($"DELETE SLUCH_DS3 WHERE SLID={id}", SprClass.LocalConnectionString);
    //            Reader2List.CustomExecuteQuery($"DELETE SLUCH WHERE ID={id}", SprClass.LocalConnectionString);
    //            tab._linqInstantFeedbackDataSource.Refresh();

    //        }
        }

        private void DateStatusItem_OnEditValueChanged(object sender, RoutedEventArgs e)
        {
            //SprClass.WorkDate = (DateTime)dateStatusItem.EditValue;
        }

        private void ExcelSluchItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = (ElReestrTabNew)((TabElement)СommonСomponents.DxTabObject).MyControl;

            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.Filter = "xml File (*.xls)|*.xls";

            //bool? result = saveFileDialog.ShowDialog();
            //if (result == true)
            //    tab.view.ExportToXls(saveFileDialog.FileName);

        }


        private void DxTabControl1_OnSelectionChanged(object sender, TabControlSelectionChangedEventArgs e)
        {
            //СommonСomponents.DxTabObject = e.NewSelectedItem;
            //var control = ((TabElement)e.NewSelectedItem).MyControl;

            //if (control is DesktopControl)
            //{
            //    GetRibbonPages(new[] { "Файл", "Справочники" });
            //    RibbonControl1.IsMinimized = false;
            //}
            //else if (control is UniSprTab)
            //{
            //    if (((UniSprTab)control)._isReadOnly)
            //    {
            //        SpravochnicPage1.SprAddButtonItem.IsEnabled = false;
            //        SpravochnicPage1.SprEditButtonItem.IsEnabled = false;
            //        SpravochnicPage1.SprDelButtonItem.IsEnabled = false;
            //        SpravochnicPage1.SprRefreshButtonItem.IsEnabled = false;
            //    }
            //    else
            //    {
            //        SpravochnicPage1.SprAddButtonItem.IsEnabled = true;
            //        SpravochnicPage1.SprEditButtonItem.IsEnabled = true;
            //        SpravochnicPage1.SprDelButtonItem.IsEnabled = true;
            //        SpravochnicPage1.SprRefreshButtonItem.IsEnabled = true;
            //    }
            //    GetRibbonPages(new[] { "Справочники" });
            //    RibbonControl1.IsMinimized = false;
            //}
            //else if (control is EconomyWindow)
            //{
            //    GetRibbonPages(new[] { "Счет/период", "Файл", "Экспертиза", "Справочники" });
            //    RibbonControl1.IsMinimized = false;
            //    SpravochnicPage1.SprAddButtonItem.IsEnabled = false;
            //    SpravochnicPage1.SprEditButtonItem.IsEnabled = false;
            //    SpravochnicPage1.SprDelButtonItem.IsEnabled = false;
            //    SpravochnicPage1.SprRefreshButtonItem.IsEnabled = false;
            //}
            //else if (control is MedicalEconomicControl.SqlEditorControl || control is CustomStatistic || control is MedicalExpertQuery)
            //{
            //    GetRibbonPages(new[] { "Редактор" });
            //    RibbonControl1.IsMinimized = false;
            //}
            //else if (control is ElReestrTabNew)
            //{
            //    GetRibbonPages(new[] { "Случаи счета/периода" });
            //    RibbonControl1.IsMinimized = false;
            //}
            //else if (control is Yamed.Registry.ScheduleControl)
            //{
            //    GetRibbonPages(new[] { "Регистратура", "Справочники" });
            //    RibbonControl1.IsMinimized = false;
            //}
            //else if (control is Yamed.Clinic.ClinicEmrPacient)
            //{
            //    GetRibbonPages(new[] { "Электронная медицинская карта", "Справочники" });
            //    RibbonControl1.IsMinimized = false;
            //}
            //else if (control is HospitalEmrSluchTab)
            //{
            //    RibbonControl1.IsMinimized = true;
            //}
            //else
            //{
            //    GetRibbonPages(new[] { "Файл", "Справочники" });
            //    SpravochnicPage1.SprAddButtonItem.IsEnabled = false;
            //    SpravochnicPage1.SprEditButtonItem.IsEnabled = false;
            //    SpravochnicPage1.SprDelButtonItem.IsEnabled = false;
            //    SpravochnicPage1.SprRefreshButtonItem.IsEnabled = false;
            //}

        }

        private void BarItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var window = new DXWindow
            {
                ShowIcon = false,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new ScheduleGen(),
                Title = "Генератор расписания",
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStyle = WindowStyle.SingleBorderWindow
            };
            window.Loaded += (o, args) =>
            {
                //var button1 = (Button)DevExpress.Xpf.Core.Native.LayoutHelper.FindElementByName(this, DXWindow.ButtonParts.PART_CloseButton.ToString());
                //button1.IsHitTestVisible = false;
                //button1.Visibility = Visibility.Collapsed;

                //var button2 = (Button)DevExpress.Xpf.Core.Native.LayoutHelper.FindElementByName(this, DXWindow.ButtonParts.PART_Minimize.ToString());
                //button2.IsHitTestVisible = false;
                //button2.Opacity = 0;
            };
            window.ShowDialog();
        }

        private void UIElement_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ModulesAppBar.IsOpen = !ModulesAppBar.IsOpen;
        }

        private void ModulesAppBar_OnMouseLeave(object sender, MouseEventArgs e)
        {
            ModulesAppBar.IsOpen = false;
        }

        private void ModulesAppBar_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ModulesAppBar.IsOpen = true;
        }

        public class ModulesCollection
        {
            public ModulesCollection()
            {
                ScheduleControlRun = new RelayCommand(
                    () =>
                    {
                        СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                        {
                            Header = "Регистратура",
                            MyControl = new ScheduleControl(),
                            IsCloseable = "True",
                            TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
                        });
                    },
                    () => true);
            }

            public ICommand ScheduleControlRun { get; set; }
        }

        private void MainMenuOpenItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //ModulesAppBar.IsOpen = !ModulesAppBar.IsOpen;

            Menu.Toggle();
        }

        private void Registry_OnClick(object sender, RoutedEventArgs e)
        {
            Menu.Hide();
            ((Button) sender).IsEnabled = false;
            Decorator.IsSplashScreenShown = true;

            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Регистратура",
                MyControl = new ScheduleControl(),
                IsCloseable = "True",
                TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
            });
            ((Button)sender).IsEnabled = true;

        }

        private void WorkSpace_OnClick(object sender, RoutedEventArgs e)
        {
            Menu.Hide();
            ((Button)sender).IsEnabled = false;
            Decorator.IsSplashScreenShown = true;

            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Рабочее место врача",
                MyControl = new WorkSpaceControl(),
                IsCloseable = "True",
                //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
            });

            ((Button)sender).IsEnabled = true;
        }

        private void Settings_OnClick(object sender, RoutedEventArgs e)
        {
            Menu.Hide();
            ((Button)sender).IsEnabled = false;
            Decorator.IsSplashScreenShown = true;

            //СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            //{
            //    Header = "Настройки",
            //    MyControl = new StatisticReports(10, "1"),
            //    IsCloseable = "True",
            //    //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
            //});

            ((Button)sender).IsEnabled = true;
        }

        private void Pattern_OnClick(object sender, RoutedEventArgs e)
        {
            Menu.Hide();
            ((Button)sender).IsEnabled = false;
            Decorator.IsSplashScreenShown = true;

            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Конструктор форм",
                MyControl = new SqlDbGUI(),
                IsCloseable = "True",
                //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
            });

            ((Button)sender).IsEnabled = true;

        }

        private void Reestr_OnClick(object sender, RoutedEventArgs e)
        {
            Menu.Hide();
            ((Button)sender).IsEnabled = false;
            Decorator.IsSplashScreenShown = true;

            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Реестры",
                MyControl = new EconomyControl(),
                IsCloseable = "True",
                TabLocalMenu = new Yamed.Oms.OmsMenu().MenuElements
            });

            ((Button)sender).IsEnabled = true;
        }

        private void OmsExpButton_OnClick(object sender, RoutedEventArgs e)
        {
            Menu.Hide();
            ((Button)sender).IsEnabled = false;
            Decorator.IsSplashScreenShown = true;

            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Реестр счетов",
                MyControl = new ExpControl(),
                IsCloseable = "True",
                TabLocalMenu = new OmsMenu().MenuElements
            });

            ((Button)sender).IsEnabled = true;
        }


        private void OmsExpAktRegistr_OnClick(object sender, RoutedEventArgs e)
        {
            Menu.Hide();
            ((Button)sender).IsEnabled = false;
            Decorator.IsSplashScreenShown = true;

            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Реестр актов",
                MyControl = new AktRegisterGrid(),
                IsCloseable = "True",
                //TabLocalMenu = new OmsMenu().MenuElements
            });

            ((Button)sender).IsEnabled = true;
        }


        //private void OmsExpAktRegistr_OnClick(object sender, RoutedEventArgs e)
        //{
        //    Menu.Hide();
        //    ((Button)sender).IsEnabled = false;
        //    Decorator.IsSplashScreenShown = true;

        //    СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
        //    {
        //        Header = "Реестр экспертиз",
        //        MyControl = new ExpControl(),
        //        IsCloseable = "True",
        //        TabLocalMenu = new OmsMenu().MenuElements
        //    });

        //    ((Button)sender).IsEnabled = true;
        //}
        private void Nsi_OnClick(object sender, RoutedEventArgs e)
        {
            Menu.Hide();
            ((Button)sender).IsEnabled = false;
            Decorator.IsSplashScreenShown = true;

            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "НСИ",
                MyControl = new NsiControl(),
                IsCloseable = "True",
                //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
            });

            ((Button)sender).IsEnabled = true;
        }

        private void PrimaryWindow_OnClosed(object sender, EventArgs e)
        {
            ThreadStart ts = delegate ()
            {
                Thread.Sleep(200);
                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    Application.Current?.Shutdown();
                });
            };
            var t = new Thread(ts);
            t.Start();
        }


        private void WorkDateEdit_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            SprClass.WorkDate = (DateTime) e.NewValue;
        }

        private void WorkDateEdit_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (((DateEdit) sender).EditValue == null)
                ((DateEdit) sender).EditValue = DateTime.Today;
        }

        private void UserTextEdit_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (((TextEdit)sender).EditValue == null)
                ((TextEdit)sender).EditValue = SprClass.UserFIO;

        }

        private void BarSplitButtonItem_BeforeCommandExecute(object sender, ItemClickEventArgs e)
        {
            СommonСomponents.DxTabControlSource.TabElements.Clear();
        }

        private void Reports_OnClick(object sender, RoutedEventArgs e)
        {

            Menu.Hide();
            ((Button) sender).IsEnabled = false;
            Decorator.IsSplashScreenShown = true;

            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Реестры",
                MyControl = new StatisticDesigner(),
                IsCloseable = "True",
                TabLocalMenu = new OmsMenu().MenuElements
            });

            ((Button) sender).IsEnabled = true;

        }

//<<<<<<< HEAD
//=======
        private void Update_OnClick(object sender, RoutedEventArgs e)
        {
            if (SprClass.Region == "37")
            {
                System.Diagnostics.Process.Start(Path.Combine(Environment.CurrentDirectory, "Updater Imed.exe"));
            }
            else
            {
                System.Diagnostics.Process.Start(Path.Combine(Environment.CurrentDirectory, "Imed Updater.exe"));
            }
        }
//>>>>>>> 9b0437a00be34a44e896169b1aee22c0972a7d15
    }
}