using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FastReport;
using FastReport.Data;
using FastReport.Design;
using FastReport.Design.StandardDesigner;
using FastReport.DevComponents.DotNetBar;
using FastReport.Utils;
using Yamed.Core;
using Yamed.Server;

namespace Yamed.Reports
{
    /// <summary>
    /// Логика взаимодействия для FRDesignerControl.xaml
    /// </summary>
    public partial class FRDesignerControl : UserControl
    {
        readonly FastReport.Design.StandardDesigner.DesignerControl _designer = new FastReport.Design.StandardDesigner.DesignerControl();
        private Report _report;
        private object _repRow;

        public FRDesignerControl(object repRow)
        {
            //WireupDesignerEvents();

            InitializeComponent();

            _repRow = repRow;

            //var wc = new WebClient();
            //Image x = Image.FromStream(wc.OpenRead(uri));

            ToolbarBase standardToolbar = _designer.Plugins.Find("StandardToolbar") as ToolbarBase;
            standardToolbar.Items["btnStdNew"].Visible = false;

            var newBtn = new ButtonItem();
            newBtn.Image = ((ButtonItem)standardToolbar.Items["btnStdSaveAll"]).Image;
            newBtn.Tooltip = "Сохранить в файл";
            newBtn.Command = new Command(SaveAsCommand);
            standardToolbar.Items.Insert(2, newBtn);

            standardToolbar.Items["btnStdOpen"].ShowSubItems = false;
            standardToolbar.Items["btnStdSaveAll"].Visible = false;

            _designer.cmdSave.CustomAction += CmdSaveOnCustomAction;
            
            _designer.ShowStatusBar = false;
            _designer.ShowMainMenu = false;


            var rl = (string)ObjHelper.GetAnonymousValue(_repRow, "Template");
            if (!string.IsNullOrWhiteSpace(rl))
            {
                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);

                writer.Write(rl);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                _report = Report.FromStream(stream);

                stream.Close();
                writer.Close();

                if (_report.Dictionary.Connections.Count > 0)
                {
                    _report.Dictionary.Connections[0].ConnectionString = SprClass.LocalConnectionString;
                    _report.Dictionary.Connections[0].CommandTimeout = 0;
                }

            }
            else
            {
                _report = new Report();

                MsSqlDataConnection sqlConnection = new MsSqlDataConnection();
                sqlConnection.ConnectionString = SprClass.LocalConnectionString;
                sqlConnection.Name = "SqlConnection";
                sqlConnection.CommandTimeout = 0;
                
                _report.Dictionary.Connections.Add(sqlConnection);

            }


            _designer.Report = _report;
            _designer.RefreshLayout();
            WinFormsHost.Child = _designer;

            //foreach (FastReport.Data.Parameter p in report.Parameters)
            //{
            //    var name = p.Name;
            //}
        }

        private void CmdSaveOnCustomAction(object sender, EventArgs eventArgs)
        {
            Stream mStream = new MemoryStream();

            _report.Save(mStream);
            mStream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(mStream);

            ObjHelper.SetAnonymousValue(ref _repRow, reader.ReadToEnd(), "Template");
            var upd = Reader2List.CustomUpdateCommand("YamedReports", _repRow, "ID");
            Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);

            mStream.Close();
            reader.Close();
        }

        private void SaveAsCommand(object sender, EventArgs eventArgs)
        {
            _designer.cmdSaveAs.Invoke();
        }

        private void WireupDesignerEvents()
        {
            //Config.DesignerSettings.CustomOpenDialog += new OpenSaveDialogEventHandler(DesignerSettings_CustomOpenDialog);
            ////Config.DesignerSettings.CustomOpenReport += new OpenSaveReportEventHandler(DesignerSettings_CustomOpenReport);
            //Config.DesignerSettings.CustomSaveDialog += new OpenSaveDialogEventHandler(DesignerSettings_CustomSaveDialog);
            ////Config.DesignerSettings.CustomSaveReport += new OpenSaveReportEventHandler(DesignerSettings_CustomSaveReport);
        }

        //private void DesignerSettings_CustomOpenDialog(object sender, OpenSaveDialogEventArgs e)
        //{
        //    using (OpenDialogForm form = new OpenDialogForm())
        //    {
        //        // pass the reports table to display a list of reports
        //        form.ReportsTable = ReportsTable;

        //        // show dialog
        //        e.Cancel = form.ShowDialog() != DialogResult.OK;

        //        // return the selected report in the e.FileName
        //        e.FileName = form.ReportName;
        //    }
        //}

    }
}
