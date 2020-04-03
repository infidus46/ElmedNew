using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using Yamed.Core;
using Yamed.Server;

namespace Yamed.OmsExp.ExpEditors
{
    /// <summary>
    /// Логика взаимодействия для SankGroupDelete.xaml
    /// </summary>
    public partial class SankGroupDelete : DevExpress.Xpf.Core.DXWindow
    {
        private object _selectedObjects;
        public SankGroupDelete(object selectedObjects)
        {
            InitializeComponent();
            _selectedObjects = selectedObjects;
        }

        private void LogBox_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            LogBox.Focus();
            Dispatcher.BeginInvoke(new Action(() => LogBox.SelectionStart = LogBox.Text.Length));
        }

        private bool _isProcess;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _isProcess = true;
            ((Button) sender).IsEnabled = false;

            var wh = "and S_TIP in (";

            foreach (ListBoxEditItem item in DelListBoxEdit.SelectedItems)
            {
                if (item.Name == "delMekListBoxEditItem")
                    wh += "1,";
                if (item.Name == "delMeeListBoxEditItem")
                    wh += "2,";
                if (item.Name == "delEkmpListBoxEditItem")
                    wh += "3,";
            }
            wh += ")";
            wh = wh.Replace(",)", ")");
            DelListBoxEdit.IsEnabled = false;

            LogBox.Text += Environment.NewLine + "Удаление выбранных санкций...";

            TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext(); //get UI thread context 
            Task.Factory.StartNew(() =>
            {
                Reader2List.CustomExecuteQuery(
                    $"delete D3_SANK_OMS where D3_ZSLID in (select IDS from {_tblName}) {wh}",
                    SprClass.LocalConnectionString);
            }).ContinueWith(a1 =>
            {
                LogBox.Text += " завершено";LogBox.Text += Environment.NewLine + "Расчет санкций...";

                Task.Factory.StartNew(() =>
                {
                    var scList = SqlReader.Select($@"select distinct SCHET_IDS from {_tblName}",
                        SprClass.LocalConnectionString);

                    var scs = scList.Select(x => (int) x.GetValue("SCHET_IDS"));
                    foreach (var sc in scs)
                    {
                        Reader2List.CustomExecuteQuery($@"
EXEC p_oms_calc_sank {sc}
EXEC p_oms_calc_schet {sc}
", SprClass.LocalConnectionString);
                    }

                }).ContinueWith(a2 =>
                {
                    LogBox.Text += " завершено";
                    DXMessageBox.Show("Выбранные санкции успешно удаленны");

                    _isProcess = false;
                    Close();

                }, taskScheduler);
            }, taskScheduler);
        }

        private void SankGroupDelete_OnLoaded(object sender, RoutedEventArgs e)
        {
            LogBox.Text = "Получение данных от сервера...";
            object sanks = new object();
            TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext(); //get UI thread context 
            Task.Factory.StartNew(() =>
            {
                var ids = (from o in ((IEnumerable<dynamic>)_selectedObjects)
                            select new
                            {
                                IDS = ObjHelper.GetAnonymousValue(o, "ID"),
                                SCHET_IDS = ObjHelper.GetAnonymousValue(o, "D3_SCID")
                            }).ToList();
                _tblName = "TempIds_" + Guid.NewGuid().ToString().Replace("-", "_");
                Reader2List.CustomExecuteQuery($"CREATE TABLE [dbo].[{_tblName}]([IDS] [int] NOT NULL, [SCHET_IDS] [int] NOT NULL)", SprClass.LocalConnectionString);
                Reader2List.AnonymousInsertCommand(_tblName, ids, "", SprClass.LocalConnectionString);
                sanks = Reader2List.CustomAnonymousSelect(
                    $"Select * from D3_SANK_OMS where D3_ZSLID in (select IDS from {_tblName})",
                    SprClass.LocalConnectionString);
            }).ContinueWith(a1 =>
            {
                DelListBoxEdit.Items.Add(new ListBoxEditItem {Name = "delMekListBoxEditItem", Content = "Удалить МЭК (" + ((IEnumerable<dynamic>)sanks).Count(x => (int)ObjHelper.GetAnonymousValue(x, "S_TIP") == 1) + ")" });
                DelListBoxEdit.Items.Add(new ListBoxEditItem {Name = "delMeeListBoxEditItem", Content = "Удалить МЭЭ (" + ((IEnumerable<dynamic>)sanks).Count(x => (int)ObjHelper.GetAnonymousValue(x, "S_TIP") == 2) + ")" });
                DelListBoxEdit.Items.Add(new ListBoxEditItem {Name = "delEkmpListBoxEditItem", Content = "Удалить ЭКМП (" + ((IEnumerable<dynamic>)sanks).Count(x => (int)ObjHelper.GetAnonymousValue(x, "S_TIP") == 3) + ")" });
                LogBox.Text += " завершено";
            }, taskScheduler);
        }

        private string _tblName;
        private void SankGroupDelete_OnUnloaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Reader2List.CustomExecuteQuery($"if exists (select * from INFORMATION_SCHEMA.TABLES where table_name ='{_tblName}') drop table {_tblName}", SprClass.LocalConnectionString);
            });
        }

        private void SankGroupDelete_OnClosing(object sender, CancelEventArgs e)
        {
            if (_isProcess)
            {
                DXMessageBox.Show("Идет процесс, закрытие окна невозможно");
                e.Cancel = true;
            }
        } 
    }
}
