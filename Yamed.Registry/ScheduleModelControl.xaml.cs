using System;
using System.Collections;
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
using DevExpress.Xpf.Grid;
using GalaSoft.MvvmLight.Command;
using Yamed.Control;
using Yamed.Control.Editors;
using Yamed.Core;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.Registry
{

    public class ScheduleModel: IDisposable
    {
        public object SelectedRow { get; set; }
        public object Obj { get; set; }
        public string TableName = "Yamed_RegModel";
        private readonly Type _type;

        public ObservableCollection<Core.DynamicBaseClass> ModelSpr { get; set; }
        public DynamicCollection<object> ModelCollection { get; set; }

        public ObservableCollection<Core.DynamicBaseClass> WeekDaySpr { get; set; }
        public ObservableCollection<Core.DynamicBaseClass> RangeSpr { get; set; }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public ScheduleModel()
        {
            
            ModelSpr = new ObservableCollection<Core.DynamicBaseClass>();
            GetModelSpr();

            ModelCollection = new DynamicCollection<object>();
            GetModel();
            _type = ModelCollection.GetDynamicType();

            WeekDaySpr = SqlReader.Select("Select * from Yamed_Spr_WeekDay", SprClass.LocalConnectionString);
            RangeSpr = SqlReader.Select("Select * from Yamed_Spr_Range", SprClass.LocalConnectionString);


            AddCommand = new RelayCommand(() =>
                {
                    Obj = Activator.CreateInstance(_type);
                    var window = new DXWindow
                    {
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        Content = new ScheduleModelEditControl(this),
                        Title = "Редактор модели расписания",
                        Width = 600,
                        Height = 350
                    };
                    window.ShowDialog();
                    GetModel();
                },
                () => true);

            EditCommand = new RelayCommand(() =>
                {
                    Obj = SelectedRow;
                    var window = new DXWindow
                    {
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        Content = new ScheduleModelEditControl(this),
                        Title = "Редактор модели расписания",
                        Width = 600,
                        Height = 350
                    };
                    window.ShowDialog();
                    GetModel();
                },
                () => true);

            DeleteCommand = new RelayCommand(() =>
                {
                    var id = (SelectedRow as Core.DynamicBaseClass)?.GetValue("ID");
                    Reader2List.CustomExecuteQuery($"DELETE FROM {TableName} WHERE ID = {id}", SprClass.LocalConnectionString);
                    GetModel();
                },
                () => true);
        }

        private void GetModel()
        {
            var temp = SqlReader.Select2($"Select * From {TableName} --Order by ModelID", SprClass.LocalConnectionString, _type);
            ModelCollection.Clear();
            foreach (var t in temp)
            {
                ModelCollection.Add(t);
            }

            ModelCollection.SetDynamicType(temp.GetDynamicType());
            temp.Clear();
        }

        public void GetModelSpr()
        {
            ModelSpr.Clear();
            ModelSpr.AddRange(SqlReader.Select("Select * From Yamed_Spr_RegModel", SprClass.LocalConnectionString));
        }

        public void Dispose()
        {
            ModelSpr.Clear();
            ModelCollection.Clear();
            WeekDaySpr.Clear();
            RangeSpr.Clear();
            SelectedRow = null;
            Obj = null;
        }
    }

    /// <summary>
    /// Логика взаимодействия для ScheduleModelControl.xaml
    /// </summary>
    public partial class ScheduleModelControl : UserControl
    {
        private ScheduleModel _scheduleModel;
        public ScheduleModelControl()
        {
            InitializeComponent();

            _scheduleModel = new ScheduleModel();
            ModelGrid.DataContext = _scheduleModel;
        }


        private void ScheduleModelControl_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _scheduleModel.Dispose();
        }
    }
}
