using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.UI;

namespace Yamed.Settings
{
    /// <summary>
    /// Логика взаимодействия для ReportPreviewControl.xaml
    /// </summary>
    public partial class PreviewControl : UserControl
    {
        public PreviewControl(object obj, int sc, int req = 0, string dates = null, string ids = null)
        {
            InitializeComponent();

            LoadingDecorator1.IsSplashScreenShown = true;
            Task.Factory.StartNew(() =>
            {
                if (!string.IsNullOrWhiteSpace((string) obj))
                {
                    MemoryStream stream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(stream);

                    writer.Write(obj);
                    writer.Flush();
                    stream.Seek(0, SeekOrigin.Begin);

                    var report = XtraReport.FromStream(stream, true);

                    var c = report.AllControls<XRSubreport>();
                    foreach (var c1 in c)
                    {
                        c1.ReportSource = report;

                    }

                    stream.Close();
                    writer.Close();

                    foreach (var p in report.Parameters)
                    {
                        if (p.Name == "ID")
                            p.Value = sc;
                        if (p.Name == "ReqID")
                            p.Value = req;
                        if (p.Name == "s_dates")
                            p.Value = dates;
                        if (p.Name == "IDS")
                            p.Value = ids;
                    }

                    //report.Parameters[0].Value = sc;
                    //report.Parameters[1].Value = null;
                    //if (report.Parameters.Count == 3)
                    //    report.Parameters[2].Value = req;

                    //((SqlDataSource)report.DataSource).ConnectionParameters = new MsSqlConnectionParameters(@"ELMEDSRV\ELMEDSERVER", "elmedicine", "sa", "12345678", MsSqlAuthorizationType.Windows);
                    //((SqlDataSource) report.DataSource).ConnectionOptions.CommandTimeout = 0;
                    ((SqlDataSource) report.DataSource).ConnectionOptions.DbCommandTimeout = 0;

                    report.CreateDocument();
                    return report;
                }
                return null;

            }).ContinueWith(x =>
            {
                DocumentPreview.DocumentSource = x.Result;
                LoadingDecorator1.IsSplashScreenShown = false;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
