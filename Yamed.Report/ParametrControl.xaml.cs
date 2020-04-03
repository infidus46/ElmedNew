using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DevExpress.CodeParser;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.XtraReports.UI;
using Yamed.Control;
using Yamed.Core;
using Yamed.Server;
using FastReport;

namespace Yamed.Reports
{
    /// <summary>
    /// Логика взаимодействия для ParametrControl.xaml
    /// </summary>
    public partial class ParametrControl : UserControl
    {
        private readonly int[] _sc;
        private static object _row;
        private readonly int _isExport;
        public bool IsSDates;
        public bool IsPDates;
        public bool IsAllDates;
        public bool IsPayer;
        public bool IsDocType;
        public bool IsAktDates;

        public ParametrControl(int[] sc, object row, int isExport)
        {
            _isExport = isExport;
            _sc = sc;
            _row = row;
            var rtype = (int) ObjHelper.GetAnonymousValue(_row, "RepType");

            IsSDates = new int[] { 101, 102, 103/*, 104*/ }.Contains(rtype);
            IsPDates = new int[] { 110, 120 }.Contains(rtype);
            IsAllDates = new int[] { 111, 112, 113 }.Contains(rtype);
            IsPayer = new int[] { 900, 901 }.Contains(rtype);
            IsDocType = new int[] { 901 }.Contains(rtype);
            IsAktDates = new int[] { 104 }.Contains(rtype);

            if (rtype > 99)
            {
                InitializeComponent();

                if (IsPDates)
                {
                    SDatesLGroup.Visibility = Visibility.Collapsed;
                    PayerLGroup.Visibility = Visibility.Collapsed;
                    DocTypeEdit.Visibility = Visibility.Collapsed;
                    AktDatesLGroup.Visibility = Visibility.Collapsed;
                    SpisokAktLGroup.Visibility = Visibility.Collapsed;
                }

                if (IsSDates)
                {
                    PDatesLGroup.Visibility = Visibility.Collapsed;
                    PayerLGroup.Visibility = Visibility.Collapsed;
                    DocTypeEdit.Visibility = Visibility.Collapsed;
                    AktDatesLGroup.Visibility = Visibility.Collapsed;
                    SpisokAktLGroup.Visibility = Visibility.Collapsed;
                }

                if (IsAktDates)
                {
                    SDatesLGroup.Visibility = Visibility.Collapsed;
                    PDatesLGroup.Visibility = Visibility.Collapsed;
                    PayerLGroup.Visibility = Visibility.Collapsed;
                    DocTypeEdit.Visibility = Visibility.Collapsed;
                    DocTypeLGroup.Visibility = Visibility.Collapsed;
                }

                if (IsPayer)
                {
                    SDatesLGroup.Visibility = Visibility.Collapsed;
                    PDatesLGroup.Visibility = Visibility.Collapsed;
                    AktDatesLGroup.Visibility = Visibility.Collapsed;
                    SpisokAktLGroup.Visibility = Visibility.Collapsed;

                    if (IsDocType)
                    {
                        DocTypeEdit.DataContext = SprClass.OsobSluchDbs;
                    }
                    else
                    {
                        DocTypeLGroup.Visibility = Visibility.Collapsed;
                    }

                    PayerEdit.DataContext = SprClass.Payment;
                    PayerEdit.EditValue = ReportsClass.PaymentId;

                }



                DateSankGenerate();
            }
            else
            {
                ReportCreate();
            }


        }

        private void DateSankGenerate()
        {
            int st = 0;
            var type = (int) ObjHelper.GetAnonymousValue(_row, "RepType");
            switch (type)
            {
                case 101:
                    st = 1;
                    break;
                case 102:
                    st = 2;
                    break;
                case 103:
                    st = 3;
                    break;
                case 111:
                    st = 1;
                    break;
                case 112:
                    st = 2;
                    break;
                case 113:
                    st = 3;
                    break;
                case 104:
                    st = 4;
                    break;
            }

            object dates = null;
            if (_sc != null)
            {
                if (st == 4)
                {
                    dates = Reader2List.CustomAnonymousSelect($@"
Select DATE_ACT, convert(nvarchar(10), DATE_ACT, 104) DATE_ACT_RUS FROM D3_SANK_OMS
WHERE D3_SCID in ({ObjHelper.GetIds(_sc)}) and S_TIP is NULL
group by DATE_ACT
ORDER BY DATE_ACT", SprClass.LocalConnectionString);
                }
                else
                {
                dates = Reader2List.CustomAnonymousSelect($@"
Select S_DATE, convert(nvarchar(10), S_DATE, 104) S_DATE_RUS FROM D3_SANK_OMS
WHERE D3_SCID in ({ObjHelper.GetIds(_sc)}) and S_TIP = {st}
group by S_DATE
ORDER BY S_DATE", SprClass.LocalConnectionString);
                }
            }
            else
            {
                dates = Reader2List.CustomAnonymousSelect($@"
Select S_DATE, convert(nvarchar(10), S_DATE, 104) S_DATE_RUS FROM D3_SANK_OMS
WHERE S_TIP = {st}
group by S_DATE
ORDER BY S_DATE", SprClass.LocalConnectionString);
            }

            DateListBoxEdit.DataContext = dates;
            DateListBoxEdit.SelectedIndex = ((IList) dates).Count - 1;
            DateActListBoxEdit.DataContext = dates;
        }

        string GetStringOfDates(ObservableCollection<object> collection)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in collection.Select(x => ObjHelper.GetAnonymousValue(x, "S_DATE")))
            {
                //sb.Append("'");
                sb.Append(((DateTime) item).ToString("yyyyMMdd"));
                //sb.Append("'");
                sb.Append(",");
            }

            var dates = sb.ToString();
            dates = dates.Remove(dates.Length - 1);
            return dates;
        }

        string GetStringOfDatesAkt(ObservableCollection<object> collection)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in collection.Select(x => ObjHelper.GetAnonymousValue(x, "DATE_ACT")))
            {
                //sb.Append("'");
                sb.Append(((DateTime)item).ToString("yyyyMMdd"));
                //sb.Append("'");
                sb.Append(",");
            }

            var dates = sb.ToString();
            dates = dates.Remove(dates.Length - 1);
            return dates;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {         
            ReportCreate();
            ((DXWindow)this.Parent).Close();
        }


        ReportParams GetReportParams(int? id = null)
        {
            var rp = new ReportParams();
            if (IsSDates || IsAllDates)
                rp.s_dates = DateListBoxEdit.SelectedItems.Any() ? GetStringOfDates(DateListBoxEdit.SelectedItems) : null;

            if (IsAktDates)
            {
                rp.s_dates = DateActListBoxEdit.SelectedItems.Any() ? GetStringOfDatesAkt(DateActListBoxEdit.SelectedItems) : null;
                rp.num_act = (string)NomerAktEdit.EditValue ?? "";
            }

            if (IsPDates || IsAllDates)
            {
                rp.beg_date = (DateTime) BeginDateEdit.EditValue;
                rp.end_date = (DateTime) EndDateEdit.EditValue;

                rp.M1 = ((DateTime) BeginDateEdit.EditValue).Month;
                rp.M2 = ((DateTime) EndDateEdit.EditValue).Month;
                rp.Y1 = ((DateTime) BeginDateEdit.EditValue).Year;
                rp.Y2 = ((DateTime) EndDateEdit.EditValue).Year;
            }
            if (IsPayer)
            {
                rp.smo = (string) PayerEdit.EditValue;

                rp.dn = (string)DocNumEdit.EditValue;
                rp.dd = (DateTime?)DocDateEdit.EditValue;
                rp.kb = (string)DocKbEdit.EditValue;

            }

            if (IsDocType)
            {
                rp.os = (int?)DocTypeEdit.EditValue;
            }

            rp.ID = id?? _sc?.First();
            rp.IDS = ObjHelper.GetIds(_sc);
            rp.ReqID = id?? _sc?.First();
            return rp;
        }


        private void ReportCreate()
        {
            var rl = (string)ObjHelper.GetAnonymousValue(_row, "Template");
            var rn = (string)ObjHelper.GetAnonymousValue(_row, "RepName");
            var rf = (int)ObjHelper.GetAnonymousValue(_row, "RepFormat");


            if (_isExport == 0)
            {
                if (rf == 1)
                {
                    СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                    {
                        Header = "Отчет",
                        MyControl = new PreviewControl(rl, GetReportParams()),
                        IsCloseable = "True",
                        //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
                    });
                    
                }
                else
                {
                    СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                    {
                        Header = "Отчет",
                        MyControl = new FRPreviewControl(rl, GetReportParams()),
                        IsCloseable = "True",
                        //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
                    });
                    
                }
                
            }
            else
            {
                if (rf == 1)
                {
                    foreach (var sc in _sc)
                    {
                        if (!string.IsNullOrWhiteSpace(rl))
                        {
                            MemoryStream stream = new MemoryStream();
                            StreamWriter writer = new StreamWriter(stream);

                            writer.Write(rl);
                            writer.Flush();
                            stream.Seek(0, SeekOrigin.Begin);

                            var report = XtraReport.FromStream(stream, true);

                            stream.Close();
                            writer.Close();
                            var subReps = report.AllControls<XRSubreport>();

                            foreach (var sr in subReps)
                            {
                                var row = SqlReader.Select($"Select * from YamedReports where RepName = '{sr.Name}'",
                                    SprClass.LocalConnectionString);
                                var t = (string)row[0].GetValue("Template");

                                MemoryStream ms = new MemoryStream();
                                StreamWriter sw = new StreamWriter(ms);

                                sw.Write(t);
                                sw.Flush();
                                ms.Seek(0, SeekOrigin.Begin);

                                var rep = XtraReport.FromStream(ms, true);
                                ((SqlDataSource)rep.DataSource).ConnectionOptions.DbCommandTimeout = 0;

                                sr.ReportSource = rep;

                                ms.Close();
                                sw.Close();
                            }


                            var rp = GetReportParams(sc);
                            var drParam = report.Parameters.OfType<DevExpress.XtraReports.Parameters.Parameter>().ToList();
                            foreach (var p in rp.GetType().GetProperties().ToList())
                            {
                                var drp = drParam.SingleOrDefault(x => x.Name == p.Name);
                                if (drp != null) drp.Value = p.GetValue(rp, null);
                            }

                            //foreach (var p in report.Parameters)
                            //{
                            //    if (p.Name == "ID")
                            //        p.Value = sc;
                            //    if (p.Name == "s_dates")
                            //        p.Value = sdates;
                            //    if (p.Name == "user")
                            //        p.Value = SprClass.userId;
                            //}

                            if (report.DataSource != null)
                                ((SqlDataSource)report.DataSource).ConnectionOptions.DbCommandTimeout = 0;

                            if (_isExport == 1)
                            {
                                var fn =
    SqlReader.Select($"SELECT [ID], [CODE_MO], [YEAR], [MONTH], [NSCHET] FROM [dbo].[D3_SCHET_OMS] WHERE ID={sc}",
    SprClass.LocalConnectionString);

                                report.ExportToPdf($@"D:\out\{(string)fn[0].GetValue("CODE_MO") + "_" + rn + "_" + fn[0].GetValue("YEAR") +
                                               fn[0].GetValue("MONTH") + "_" + fn[0].GetValue("NSCHET") + "_(" + fn[0].GetValue("ID") + "_)"}" + ".pdf");
                                report.ExportToRtf(
                                    $@"D:\out\{(string)fn[0].GetValue("CODE_MO") + "_" + rn + "_" + fn[0].GetValue("YEAR") +
                                               fn[0].GetValue("MONTH") + "_" + fn[0].GetValue("NSCHET") + "_(" + fn[0].GetValue("ID") + "_)"}" + ".rtf");


                            }
                            else
                            {
                                var fn =
    SqlReader.Select($"Select z.id, fam, im, ot from d3_zsl_oms z join d3_pacient_oms pa on z.d3_pid = pa.id  where z.id ={ sc}",
    SprClass.LocalConnectionString);

                                report.ExportToPdf($@"D:\out\{rn + "_" + fn[0].GetValue("fam") + "_" +
                                               fn[0].GetValue("im") + "_" + fn[0].GetValue("ot") + "_(" + fn[0].GetValue("id") + "_)"}" + ".pdf");
                                report.ExportToRtf($@"D:\out\{rn + "_" + fn[0].GetValue("fam") + "_" +
                                               fn[0].GetValue("im") + "_" + fn[0].GetValue("ot") + "_(" + fn[0].GetValue("id") + "_)"}" + ".rtf");

                            }
                        }
                    }
                }
                else
                {
                    foreach (var sc in _sc)
                    {
                        if (!string.IsNullOrWhiteSpace(rl))
                        {
                            MemoryStream stream = new MemoryStream();
                            StreamWriter writer = new StreamWriter(stream);

                            writer.Write(rl);
                            writer.Flush();
                            stream.Seek(0, SeekOrigin.Begin);

                            var report = Report.FromStream(stream);

                            stream.Close();
                            writer.Close();

                            var rp = GetReportParams(sc);
                            var frParam = report.Parameters.OfType<FastReport.Data.Parameter>().ToList();
                            foreach (var p in rp.GetType().GetProperties().ToList())
                            {
                                var frp = frParam.SingleOrDefault(x => x.Name == p.Name);
                                if (frp != null) frp.Value = p.GetValue(rp, null);
                            }

                            report.Dictionary.Connections[0].ConnectionString =
                                SprClass.LocalConnectionString;
                            report.Dictionary.Connections[0].CommandTimeout = 0;

                            report.Prepare();
                            if (_isExport == 1)
                            {
                                var fn =
    SqlReader.Select($"SELECT [ID], [CODE_MO], [YEAR], [MONTH], [NSCHET] FROM [dbo].[D3_SCHET_OMS] WHERE ID={sc}",
    SprClass.LocalConnectionString);

                                using (var exp = new FastReport.Export.Pdf.PDFExport())
                                {
                                    exp.Export(report, $@"D:\out\{(string)fn[0].GetValue("CODE_MO") + "_" + rn + "_" + fn[0].GetValue("YEAR") +
                                                   fn[0].GetValue("MONTH") + "_" + fn[0].GetValue("NSCHET") + "_(" + fn[0].GetValue("ID") + "_)"}" + ".pdf");
                                }

                                using (var exp = new FastReport.Export.RichText.RTFExport())
                                {
                                    exp.Export(report, $@"D:\out\{(string)fn[0].GetValue("CODE_MO") + "_" + rn + "_" + fn[0].GetValue("YEAR") +
                                               fn[0].GetValue("MONTH") + "_" + fn[0].GetValue("NSCHET") + "_(" + fn[0].GetValue("ID") + "_)"}" + ".rtf");
                                }

                            }
                            else
                            {
                                var fn =
    SqlReader.Select($"Select z.id, fam, im, ot from d3_zsl_oms z join d3_pacient_oms pa on z.d3_pid = pa.id  where z.id ={ sc}",
    SprClass.LocalConnectionString);

                                using (var exp = new FastReport.Export.Pdf.PDFExport())
                                {
                                    exp.Export(report, $@"D:\out\{rn + "_" + fn[0].GetValue("fam") + "_" +
                                               fn[0].GetValue("im") + "_" + fn[0].GetValue("ot") + "_(" + fn[0].GetValue("id") + "_)"}" + ".pdf");
                                }

                                using (var exp = new FastReport.Export.RichText.RTFExport())
                                {
                                    exp.Export(report, $@"D:\out\{rn + "_" + fn[0].GetValue("fam") + "_" +
                                               fn[0].GetValue("im") + "_" + fn[0].GetValue("ot") + "_(" + fn[0].GetValue("id") + "_)"}" + ".rtf");
                                }

                            }
                        }
                    }
                }

            }
        }

        private void PayerEdit_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            ReportsClass.PaymentId = (string) PayerEdit.EditValue;
        }
        private string numdates;
        private void getnomerAkt(object sender, RoutedEventArgs e)
        {
            numdates = DateActListBoxEdit.SelectedItems.Any() ? GetStringOfDatesAkt(DateActListBoxEdit.SelectedItems) : null;
            if (numdates != null)
            {
                NomerAktEdit.DataContext = Reader2List.CustomAnonymousSelect($@"
Select NUM_ACT FROM D3_SANK_OMS
WHERE DATE_ACT in ('{numdates}')
group by num_act,date_act
ORDER BY DATE_ACT", SprClass.LocalConnectionString);
            }
        }
    }
}
