using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.WindowsUI.Navigation;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using Yamed.Control.Editors;
using Yamed.Core;
using Yamed.Server;

namespace Yamed.Pattern
{
    /// <summary>
    /// Логика взаимодействия для SqlDbGUI.xaml
    /// </summary>
    public partial class SqlDbGUI : UserControl
    {
        public static object TablesSchema = null;
        private static object _row;
        private static readonly string _connectionString = SprClass.LocalConnectionString;
        //private static readonly string _connectionString = "Data Source = 91.240.209.114; Initial Catalog = DocExchange; User ID = sa; Password = Gbljh:100";
        public SqlDbGUI()
        {
            
            InitializeComponent();
            DoUpdate();
        }

        private SettingEditControl _settingEditControl;
        private void NewButton_OnClick(object sender, RoutedEventArgs e)
        {
            _isNeedUpdate = true;
            var type = GridControl1.DataContext.GetType().GetGenericArguments()[0];
            var newObj = Activator.CreateInstance(type);
            _row = newObj;
            _settingEditControl = new SettingEditControl(_row);
            NavigationFrame1.Navigate(_settingEditControl);
        }

        private void EditButton_OnClick(object sender, RoutedEventArgs e){
            LoadingDecorator1.IsSplashScreenShown = true;
            _settingEditControl = new SettingEditControl(_row);
            NavigationFrame1.Navigate(_settingEditControl);
            LoadingDecorator1.IsSplashScreenShown = false;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
                Reader2List.CustomExecuteQuery(
                    Reader2List.CustomUpdateCommand("SettingsTables", _row, "ID"), _connectionString);

            _settingEditControl.SaveTable();
        }

        private void NavigationFrame1_OnNavigated(object sender, NavigationEventArgs e)
        {
            if (NavigationFrame1.Content is SettingEditControl)
            {
                NewButton.IsEnabled = false;
                EditButton.IsEnabled = false;
                SaveButton.IsEnabled = true;
                DeleteButton.IsEnabled = false;
                BackButton.IsEnabled = true;
                LayoutButton.IsEnabled = false;
                ReportButton.IsEnabled = false;
            }
            //else if (NavigationFrame1.Content is UniSprEditPanel)
            //{
            //    NewButton.IsEnabled = false;
            //    EditButton.IsEnabled = false;
            //    SaveButton.IsEnabled = true;
            //    DeleteButton.IsEnabled = false;
            //    BackButton.IsEnabled = true;
            //    LayoutButton.IsEnabled = false;
            //    ReportButton.IsEnabled = false;
            //}
            else
            {
                NewButton.IsEnabled = !false;
                EditButton.IsEnabled = !false;
                SaveButton.IsEnabled = !true;
                DeleteButton.IsEnabled = !false;
                BackButton.IsEnabled = !true;
                LayoutButton.IsEnabled = !false;
                ReportButton.IsEnabled = !false;

                if (_isNeedUpdate)
                {
                    DoUpdate();
                }
            }
        }
        private void DoUpdate()
        {
            var list1 = (IList)
                Reader2List.CustomAnonymousSelect(@"Select * from [SettingsTables]",
                    _connectionString);
            GridControl1.DataContext = list1;
            _isNeedUpdate = false;

        }

        private bool _isNeedUpdate;
        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationFrame1.GoBack();
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void LayoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            var obj = _row;
            var tn = (string)ObjHelper.GetAnonymousValue(obj, "TableName");
            var td = (string)ObjHelper.GetAnonymousValue(obj, "TableDisplayName");
            var table =
                Reader2List.CustomAnonymousSelect(
                    $"Select top 0 * from {tn}", _connectionString);
            var type = table.GetType().GetGenericArguments()[0];
            var newObj = Activator.CreateInstance(type);

            //NavigationFrame1.Navigate(new UniSprEditPanel(tn, newObj, false, _connectionString));
            //NavigationFrame1.Source = new UniSprEditPanel(tn, newObj, false, _connectionString);

            var uni = new UniSprEditControl(tn, newObj, false, _connectionString);
            uni.IsConfigMode = true;
            var window = new DXWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = uni,
                Title = td
            };
            window.ShowDialog();
            uni = null;
            window = null;
            DoUpdate();
        }

        // Create an MDI (multi-document interface) controller instance.
        XRDesignMdiController mdiController;

        private void ReportButton_OnClick(object sender, RoutedEventArgs e)
        {

           // ExportToHTML();return;



            // Create a design form and get its MDI controller.
            XRDesignRibbonForm form = new XRDesignRibbonForm();


            mdiController = form.DesignMdiController;

            mdiController.DataSourceWizardSettings.SqlWizardSettings.EnableCustomSql = true;


            // Handle the DesignPanelLoaded event of the MDI controller,
            // to override the SaveCommandHandler for every loaded report.
            mdiController.DesignPanelLoaded +=
                new DesignerLoadedEventHandler(mdiController_DesignPanelLoaded);

            // Open an empty report in the form.
            var rep = new XtraReport();
            var rl = (string) ObjHelper.GetAnonymousValue(_row, "ReportLayout");

            if (!string.IsNullOrWhiteSpace(rl))
            {
                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);

                writer.Write(rl);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                rep.LoadLayout(stream);

                stream.Close();
                writer.Close();
            }
            else
            {
                var p = new Parameter
                {
                    Name = "ID",
                    Type = typeof(int)
                };

                rep.Parameters.Add(p);
            }

            mdiController.OpenReport(rep);

            // Show the form.
            form.ShowDialog();
            mdiController.ActiveDesignPanel?.CloseReport();
        }

        private void ExportToHTML()
        {
            // A path to export a report.
            string reportPath = "d:\\Test.html";

            // Create a report instance.
            XtraReport report = new XtraReport();
            var rl = (string)ObjHelper.GetAnonymousValue(_row, "ReportLayout");
            if (!string.IsNullOrWhiteSpace(rl))
            {
                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);

                writer.Write(rl);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                report.LoadLayout(stream);

                stream.Close();
                writer.Close();
            }

            // Get its HTML export options.
            HtmlExportOptions htmlOptions = report.ExportOptions.Html;

            // Set HTML-specific export options.
            htmlOptions.CharacterSet = "UTF-8";
            htmlOptions.TableLayout = false;
            htmlOptions.RemoveSecondarySymbols = false;
            htmlOptions.Title = "Test Title";

            // Set the pages to be exported, and page-by-page options.
            htmlOptions.ExportMode = HtmlExportMode.SingleFilePageByPage;
            htmlOptions.PageRange = "1, 3-5";
            htmlOptions.PageBorderColor = Color.Blue;
            htmlOptions.PageBorderWidth = 3;

            // Export the report to HTML.
            report.ExportToHtml(reportPath);

            // Show the result.
        }

        void mdiController_DesignPanelLoaded(object sender, DesignerLoadedEventArgs e)
        {
            XRDesignPanel panel = (XRDesignPanel)sender;
            panel.AddCommandHandler(new SaveCommandHandler(panel));
        }

        public class SaveCommandHandler : DevExpress.XtraReports.UserDesigner.ICommandHandler
        {
            private XRDesignPanel _panel;

            public SaveCommandHandler(XRDesignPanel panel)
            {
                this._panel = panel;
            }

            public void HandleCommand(DevExpress.XtraReports.UserDesigner.ReportCommand command,
            object[] args)
            {
                // Save the report.
                Save();
            }

            public bool CanHandleCommand(DevExpress.XtraReports.UserDesigner.ReportCommand command,
            ref bool useNextHandler)
            {
                useNextHandler = !(command == ReportCommand.SaveFile ||
                    command == ReportCommand.SaveFileAs);
                return !useNextHandler;
            }

            void Save()
            {
                // Write your custom saving here.
                // ...
                Stream mStream = new MemoryStream();

                // For instance:
                _panel.Report.SaveLayout(mStream);
                mStream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(mStream);

                ObjHelper.SetAnonymousValue(ref _row, reader.ReadToEnd(), "ReportLayout");
                var upd = Reader2List.CustomUpdateCommand("SettingsTables", _row, "ID");
                Reader2List.CustomExecuteQuery(upd, _connectionString);

                mStream.Close();
                reader.Close();

                // Prevent the "Report has been changed" dialog from being shown.
                _panel.ReportState = ReportState.Saved;
            }
        }

        private void GridControl1_OnSelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            _row = ((GridControl) sender).SelectedItem;
        }
    }
}
