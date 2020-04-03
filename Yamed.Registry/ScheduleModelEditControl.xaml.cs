using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using DevExpress.Xpf.CodeView;
using DevExpress.Xpf.Core;
using Yamed.Control.Editors;
using Yamed.Core;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.Registry
{
    /// <summary>
    /// Логика взаимодействия для ScheduleModelEditControl.xaml
    /// </summary>
    public partial class ScheduleModelEditControl : UserControl
    {

        private ScheduleModel _scheduleModel;

        public ScheduleModelEditControl(ScheduleModel scheduleModel)
        {
            InitializeComponent();

            _scheduleModel = scheduleModel;

            DataGrid.DataContext = _scheduleModel;

            if ((int)(_scheduleModel.Obj as DynamicBaseClass).GetValue("ID") > 0)
                DaysEdit.StyleSettings = new DevExpress.Xpf.Editors.ComboBoxStyleSettings();
        }

        private void ButtonInfo_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Content = new UniSprFullControl("Yamed_Spr_RegModel", SprClass.LocalConnectionString, false),
                Title = "Редактирование"
            };
            window.ShowDialog();
            _scheduleModel.GetModelSpr();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var obj = (_scheduleModel.Obj as DynamicBaseClass);
            if ((int)obj.GetValue("ID") == 0)
            {
                foreach (var item in DaysEdit.SelectedItems)
                {
                    obj.SetValue("WeekDayID", (item as DynamicBaseClass).GetValue("ID"));
                    Reader2List.ObjectInsertCommand(_scheduleModel.TableName, _scheduleModel.Obj, "ID", SprClass.LocalConnectionString);
                }
            }
            else
            {
                obj.SetValue("WeekDayID", (DaysEdit.SelectedItems[0] as DynamicBaseClass).GetValue("ID"));
                var upd = Reader2List.CustomUpdateCommand(_scheduleModel.TableName, _scheduleModel.Obj, "ID");
                Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
            }

            (this.Parent as DXWindow)?.Close();
        }
    }
}
