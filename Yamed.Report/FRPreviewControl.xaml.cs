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
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;
using FastReport;
using Yamed.Control;
using Yamed.Server;

namespace Yamed.Reports
{
    /// <summary>
    /// Логика взаимодействия для FRPreviewControl.xaml
    /// </summary>
    public partial class FRPreviewControl : UserControl
    {
        public FRPreviewControl(object obj, ReportParams rp)
        {
            FastReport.Preview.PreviewControl prew = new FastReport.Preview.PreviewControl();

            InitializeComponent();

            LoadingDecorator1.IsSplashScreenShown = true;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace((string) obj))
                    {
                        MemoryStream stream = new MemoryStream();
                        StreamWriter writer = new StreamWriter(stream);

                        writer.Write(obj);
                        writer.Flush();
                        stream.Seek(0, SeekOrigin.Begin);

                        var report = Report.FromStream(stream);


                        stream.Close();
                        writer.Close();

                        var frParam = report.Parameters.OfType<FastReport.Data.Parameter>().ToList();
                        foreach (var p in rp.GetType().GetProperties().ToList())
                        {
                            var frp = frParam.SingleOrDefault(x => x.Name == p.Name);
                            if (frp != null) frp.Value = p.GetValue(rp, null);
                        }

                        report.Dictionary.Connections[0].ConnectionString =
                            SprClass.LocalConnectionString;
                        report.Dictionary.Connections[0].CommandTimeout = 0;

                        return report;
                    }

                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    if (ex.InnerException != null)
                    {
                        error += Environment.NewLine;
                        error += ex.InnerException.Message;
                    }
                    Dispatcher.BeginInvoke((Action) delegate()
                    {
                        ErrorGlobalWindow.ShowError(error);
                    });
                }
                finally
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        LoadingDecorator1.IsSplashScreenShown = false;
                    }); 
                }
                return null;

            }).ContinueWith(x =>
            {
                var report = x.Result;
                report.Preview = prew;
                report.Prepare();
                report.ShowPrepared();
                WinFormsHost.Child = prew;

                LoadingDecorator1.IsSplashScreenShown = false;

            }, TaskScheduler.FromCurrentSynchronizationContext());


        }
    }
}
