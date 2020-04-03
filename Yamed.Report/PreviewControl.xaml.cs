using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.UI;
using FastReport;
using Yamed.Control;
using Yamed.Server;

namespace Yamed.Reports
{
    /// <summary>
    /// Логика взаимодействия для ReportPreviewControl.xaml
    /// </summary>
    public partial class PreviewControl : UserControl
    {
        public PreviewControl(object obj, ReportParams rp, int req = 0)
        {
            InitializeComponent();

            LoadingDecorator1.IsSplashScreenShown = true;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace((string)obj))
                    {
                        MemoryStream stream = new MemoryStream();
                        StreamWriter writer = new StreamWriter(stream);

                        writer.Write(obj);
                        writer.Flush();
                        stream.Seek(0, SeekOrigin.Begin);

                        var report = XtraReport.FromStream(stream, true);

                        var subReps = report.AllControls<XRSubreport>();
                        foreach (var sr in subReps)
                        {
                            var row = SqlReader.Select($"Select * from YamedReports where RepName = '{sr.Name}'", SprClass.LocalConnectionString);
                            var rl = (string)row[0].GetValue("Template");

                            MemoryStream ms = new MemoryStream();
                            StreamWriter sw = new StreamWriter(ms);

                            sw.Write(rl);
                            sw.Flush();
                            ms.Seek(0, SeekOrigin.Begin);

                            var rep = XtraReport.FromStream(ms, true);
                            ((SqlDataSource)rep.DataSource).ConnectionOptions.DbCommandTimeout = 0;

                            sr.ReportSource = rep;

                            ms.Close();
                            sw.Close();
                        }

                        stream.Close();
                        writer.Close();

                        var drParam = report.Parameters.OfType<DevExpress.XtraReports.Parameters.Parameter>().ToList();

                        if (rp != null)
                        {
                            foreach (var p in rp.GetType().GetProperties().ToList())
                            {
                                var drp = drParam.SingleOrDefault(x => x.Name == p.Name);
                                if (drp != null) drp.Value = p.GetValue(rp, null);
                            }
                        }

                        if (drParam.Any(x => x.Name == "ReqID"))
                        {
                            var drp = drParam.Single(x => x.Name == "ReqID");
                            drp.Value = req;
                        }


                        //foreach (var p in report.Parameters)
                        //{
                        //    if (p.Name == "ID")
                        //        p.Value = sc;
                        //    if (p.Name == "ReqID")
                        //        p.Value = req;
                        //    if (p.Name == "s_dates")
                        //        p.Value = dates;
                        //    if (p.Name == "IDS")
                        //        p.Value = ids;
                        //    if (p.Name == "user")
                        //        p.Value = SprClass.userId;
                        //}

                        //report.Parameters[0].Value = sc;
                        //report.Parameters[1].Value = null;
                        //if (report.Parameters.Count == 3)
                        //    report.Parameters[2].Value = req;

                        //((SqlDataSource)report.DataSource).ConnectionParameters = new MsSqlConnectionParameters(@"ELMEDSRV\ELMEDSERVER", "elmedicine", "sa", "12345678", MsSqlAuthorizationType.Windows);
                        //((SqlDataSource) report.DataSource).ConnectionOptions.CommandTimeout = 0;
                        if (report.DataSource != null)
                            ((SqlDataSource)report.DataSource).ConnectionOptions.DbCommandTimeout = 0;

                        report.CreateDocument();
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
                    Dispatcher.BeginInvoke((Action)delegate ()
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
                DocumentPreview.DocumentSource = x.Result;
                LoadingDecorator1.IsSplashScreenShown = false;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
