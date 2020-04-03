using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using Yamed.Control;
using Yamed.Control.Editors;
using Yamed.Core;
using Yamed.Server;

namespace Yamed.Reports
{
    /// <summary>
    /// Логика взаимодействия для StatisticDesigner.xaml
    /// </summary>
    public partial class StatisticDesigner : UserControl
    {
        public StatisticDesigner()
        {
            InitializeComponent();

            UpdateGrid();
        }

        void UpdateGrid()
        {
            GridControl1.DataContext = Reader2List.CustomAnonymousSelect($"Select [ID],[Name],[RepName],[RepType],[Сomment],[RepFormat] from YamedReports",
                SprClass.LocalConnectionString);
        }

        private void DesignerItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var row = GridControl1.SelectedItem;

            if ((int) ObjHelper.GetAnonymousValue(row, "RepFormat") == 1)
            {
                var rt = (IList)Reader2List.CustomAnonymousSelect($"Select * from YamedReports where ID = {ObjHelper.GetAnonymousValue(row, "ID")}",
    SprClass.LocalConnectionString);

                var window = new DXWindow
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = new Reports.ReportControl(rt[0]),
                    Title = "Редактор отчетных форм",
                    SizeToContent = SizeToContent.WidthAndHeight
                };
                window.ShowDialog();}
            else if ((int) ObjHelper.GetAnonymousValue(row, "RepFormat") == 2)
            {
                var rt = (IList)Reader2List.CustomAnonymousSelect($"Select * from YamedReports where ID = {ObjHelper.GetAnonymousValue(row, "ID")}",
                    SprClass.LocalConnectionString);

                СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                {
                    Header = "Дизайнер - " + ObjHelper.GetAnonymousValue(row, "Name"),
                    MyControl = new FRDesignerControl(rt[0]),
                    IsCloseable = "True",
                    //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
                });
            }
            
        }


        private void RefreshItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            UpdateGrid();
        }

        private void AddItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var type = GridControl1.DataContext.GetType().GetGenericArguments()[0];
            var obj = Activator.CreateInstance(type);

            var sprEditWindow = new UniSprEditControl("YamedReports", obj, false, SprClass.LocalConnectionString);
            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Content = sprEditWindow,
                Title = "Новая запись"
            };

            window.ShowDialog();

            UpdateGrid();
        }

        private void EditItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var row = GridControl1.SelectedItem;

            var sprEditWindow = new UniSprEditControl("YamedReports", row, true, SprClass.LocalConnectionString);
            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Content = sprEditWindow,
                Title = "Редактирование"
            };
            window.ShowDialog();

            UpdateGrid();
        }

        private void DelItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var row = GridControl1.SelectedItem;

            MessageBoxResult result =
                MessageBox.Show(
                    "Удалить печатную форму " + ObjHelper.GetAnonymousValue(row, "Name") + "?", "Удаление",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Reader2List.CustomExecuteQuery($@"DELETE FROM YamedReports where ID = {ObjHelper.GetAnonymousValue(row, "ID")}", SprClass.LocalConnectionString);
                UpdateGrid();
            }
        }
    }
}
