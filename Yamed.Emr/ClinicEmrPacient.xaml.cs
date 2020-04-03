using System;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using Yamed.Control;
using Yamed.Control.Editors;
using Yamed.Core;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.Emr
{
    /// <summary>
    /// Логика взаимодействия для ClinicEmrPacient.xaml
    /// </summary>
    public partial class ClinicEmrPacient : UserControl
    {
        public ClinicEmrPacient()
        {
            InitializeComponent();

            PacientGridControl.view.RowDoubleClick += PccientViewOnRowDoubleClick;
        }

        private void PccientViewOnRowDoubleClick(object sender, RowDoubleClickEventArgs rowDoubleClickEventArgs)
        {
            var row = (D3_PACIENT_OMS)DxHelper.GetSelectedGridRow(PacientGridControl);
            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                SizeToContent = SizeToContent.WidthAndHeight,
                //Content = new EmrPacientControl(row.ID)
            };
            window.ShowDialog();
        }

        private void LookAnalysisItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var obj = AnalysisFormList.SelectedItem;
            var tn = (string)ObjHelper.GetAnonymousValue(obj, "TableName");
            var td = (string)ObjHelper.GetAnonymousValue(obj, "TableDisplayName");

            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = td,
                MyControl = new UniSprControl(tn, SprClass.LocalConnectionString, false),
                IsCloseable = "True"
            });
        }

        private void ClinicEmrPacient_OnUnloaded(object sender, RoutedEventArgs e)
        {
            AnalysisFormList.DataContext = null;
            PacientDocumentList.DataContext = null;

            PacientGridControl.view.RowDoubleClick -= PccientViewOnRowDoubleClick;
        }

        private void PacientGridControl_OnSelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.NewItem as DevExpress.Data.Async.Helpers.ReadonlyThreadSafeProxyForObjectFromAnotherThread;
            if (item == null) return;

            var id = (int)ObjHelper.GetAnonymousValue(item.OriginalRow, "ID");

            Dispatcher.BeginInvoke(new Action(() =>
            {
                SluchGridControl.DataContext = Reader2List.CustomSelect<SLUCH>($"Select * from SLUCH where PID = {id}",
                    SprClass.LocalConnectionString);

            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void SluchGridControl_OnSelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.NewItem;
            if (item == null) return;

            var id = (int)ObjHelper.GetAnonymousValue(item, "ID");

            Dispatcher.BeginInvoke(new Action(() =>
            {
                UslGridControl.DataContext = Reader2List.CustomSelect<USL>($"Select * from USL where SLID = {id}",
                    SprClass.LocalConnectionString);

                AnalysisFormList.DataContext =Reader2List.CustomAnonymousSelect("Select * from settingstables where TableType = 3",
                        SprClass.LocalConnectionString);

                PacientDocumentList.DataContext =
                    Reader2List.CustomAnonymousSelect("Select * from settingstables where Comment='Формы пациента'",
                        SprClass.LocalConnectionString);

            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
