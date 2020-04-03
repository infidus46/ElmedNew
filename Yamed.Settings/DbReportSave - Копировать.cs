using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.WindowsUI.Navigation;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using Yamed.Core;
using Yamed.Server;

namespace Yamed.Settings
{

    // Create an MDI (multi-document interface) controller instance.

    //private void ReportButton_OnClick()
    //{

    //    // ExportToHTML();return;



    //    // Create a design form and get its MDI controller.
    //    XRDesignRibbonForm form = new XRDesignRibbonForm();


    //    mdiController = form.DesignMdiController;

    //    mdiController.DataSourceWizardSettings.SqlWizardSettings.EnableCustomSql = true;


    //    // Handle the DesignPanelLoaded event of the MDI controller,
    //    // to override the SaveCommandHandler for every loaded report.
    //    mdiController.DesignPanelLoaded +=
    //        new DesignerLoadedEventHandler(mdiController_DesignPanelLoaded);

    //    // Open an empty report in the form.
    //    var rep = new XtraReport();
    //    var rl = (string)ObjHelper.GetAnonymousValue(_row, "ReportLayout");

    //    if (!string.IsNullOrWhiteSpace(rl))
    //    {
    //        MemoryStream stream = new MemoryStream();
    //        StreamWriter writer = new StreamWriter(stream);

    //        writer.Write(rl);
    //        writer.Flush();
    //        stream.Seek(0, SeekOrigin.Begin);

    //        rep.LoadLayout(stream);

    //        stream.Close();
    //        writer.Close();
    //    }
    //    else
    //    {
    //        var p = new Parameter
    //        {
    //            Name = "ID",
    //            Type = typeof(int)
    //        };

    //        rep.Parameters.Add(p);
    //    }

    //    mdiController.OpenReport(rep);

    //    // Show the form.
    //    form.ShowDialog();
    //    mdiController.ActiveDesignPanel?.CloseReport();
    //}

    //private void ExportToHTML()
    //{
    //    // A path to export a report.
    //    string reportPath = "d:\\Test.html";

    //    // Create a report instance.
    //    XtraReport report = new XtraReport();
    //    var rl = (string)ObjHelper.GetAnonymousValue(_row, "ReportLayout");
    //    if (!string.IsNullOrWhiteSpace(rl))
    //    {
    //        MemoryStream stream = new MemoryStream();
    //        StreamWriter writer = new StreamWriter(stream);

    //        writer.Write(rl);
    //        writer.Flush();
    //        stream.Seek(0, SeekOrigin.Begin);

    //        report.LoadLayout(stream);

    //        stream.Close();
    //        writer.Close();
    //    }

    //    // Get its HTML export options.
    //    HtmlExportOptions htmlOptions = report.ExportOptions.Html;

    //    // Set HTML-specific export options.
    //    htmlOptions.CharacterSet = "UTF-8";
    //    htmlOptions.TableLayout = false;
    //    htmlOptions.RemoveSecondarySymbols = false;
    //    htmlOptions.Title = "Test Title";

    //    // Set the pages to be exported, and page-by-page options.
    //    htmlOptions.ExportMode = HtmlExportMode.SingleFilePageByPage;
    //    htmlOptions.PageRange = "1, 3-5";
    //    htmlOptions.PageBorderColor = Color.Blue;
    //    htmlOptions.PageBorderWidth = 3;

    //    // Export the report to HTML.
    //    report.ExportToHtml(reportPath);

    //    // Show the result.
    //}

    //void mdiController_DesignPanelLoaded(object sender, DesignerLoadedEventArgs e)
    //{
    //    XRDesignPanel panel = (XRDesignPanel)sender;
    //    panel.AddCommandHandler(new SaveCommandHandler(panel));
    //}

    public class SaveCommandHandler : DevExpress.XtraReports.UserDesigner.ICommandHandler
    {

        XRDesignMdiController mdiController;

        private XRDesignPanel _panel;
        private object _rep;

        public SaveCommandHandler(XRDesignPanel panel, object rep)
        {
            this._panel = panel;
            this._rep = rep;

        }

        public void HandleCommand(DevExpress.XtraReports.UserDesigner.ReportCommand command,
            object[] args)
        {
            // Save the report.
            Save(_rep);
        }

        public bool CanHandleCommand(DevExpress.XtraReports.UserDesigner.ReportCommand command,
            ref bool useNextHandler)
        {
            useNextHandler = !(command == ReportCommand.SaveFile ||
                               command == ReportCommand.SaveFileAs);
            return !useNextHandler;
        }

        void Save(object row)
        {
            // Write your custom saving here.
            // ...
            Stream mStream = new MemoryStream();

            // For instance:
            _panel.Report.SaveLayout(mStream);
            mStream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(mStream);

            ObjHelper.SetAnonymousValue(ref row, reader.ReadToEnd(), "Template");
            var upd = Reader2List.CustomUpdateCommand("YamedReports", row, "ID");
            Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);

            mStream.Close();
            reader.Close();

            // Prevent the "Report has been changed" dialog from being shown.
            _panel.ReportState = ReportState.Saved;
        }
    }
}
