using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using FastReport;
using Yamed.Control;
using Yamed.Core;
using Yamed.Server;

namespace Yamed.Reports
{
    /// <summary>
    /// Логика взаимодействия для StatisticReports.xaml
    /// </summary>
    public partial class StatisticReports : UserControl
    {
        private readonly object[] _scs;
        private readonly int _rt;
        public StatisticReports(object[] scs, int rt = 0)
        {
            InitializeComponent();
            _scs = scs;
            _rt = rt;
            if (_rt == 0)
            {
                if (_scs == null)
                    _rt = 99;
                GridControl1.DataContext =
                    Reader2List.CustomAnonymousSelect($"Select * from YamedReports where Reptype > {_rt} and Reptype < 1000",
                        SprClass.LocalConnectionString);
            }
            else if (_rt >= 1000)
            {
                GridControl1.DataContext =
                    Reader2List.CustomAnonymousSelect(
                        $"Select * from YamedReports where Reptype >= {_rt} and Reptype < {_rt + 1000}",
                        SprClass.LocalConnectionString);
            }
        }


        private static object _row;
        private void GridControl1_OnSelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            _row = ((GridControl)sender).SelectedItem;
        }

        private void CreateDocument_OnClick(object sender, RoutedEventArgs e)
        {
            GetReportParametr(0);
            //else
            //{
            //    //// Invoke the Ribbon Print Preview form  
            //    //// and load the report document into it. 
            //    //PrintHelper.ShowRibbonPrintPreview(this, report);

            //    //// Invoke the Ribbon Print Preview form modally. 
            //    //PrintHelper.ShowRibbonPrintPreviewDialog(Control.WindowUtils.FindOwnerWindow(), GetReport());

            //    //// Invoke the standard Print Preview form  
            //    //// and load the report document into it. 
            //    //PrintHelper.ShowPrintPreview(this, new XtraReport1());

            //    //// Invoke the standard Print Preview form modally. 
            //    PrintHelper.ShowPrintPreviewDialog(Control.WindowUtils.FindOwnerWindow(), GetReport());

            //    //// Invoke the standard Print Preview form modally. 
            //    //PrintHelper.PrintDirect(GetReport());
            //}

        }

        private void ReportExportItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (_rt >= 1000)
            {
                GetReportParametr(2);
            }
            else
                GetReportParametr(1);
        }

        void GetReportParametr(int isExport)
        {
            var ids = _scs?.OfType<int>().ToArray();

            var rtype = (int)ObjHelper.GetAnonymousValue(_row, "RepType");

            var pc = new Reports.ParametrControl(ids, _row, isExport);
            if (rtype > 99)
            {
                var window = new DXWindow
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = pc,
                    Title = "Расширенные параметры",
                    SizeToContent = SizeToContent.WidthAndHeight
                };
                window.ShowDialog();
            }
        }
    }
}
