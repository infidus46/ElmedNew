using System;
using System.Collections.Generic;
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
using Yamed.Core;
using Yamed.Server;

namespace Yamed.Ambulatory
{
    /// <summary>
    /// Логика взаимодействия для SluchControl.xaml
    /// </summary>
    public partial class SluchControl : UserControl
    {
        public SluchControl()
        {
            InitializeComponent();
        }

        public void BindSluch(int slid)
        {
            SluchTempl.BindSluch(slid);
        }

        private DynamicBaseClass _reg;
        public void BindEmptySluch(DynamicBaseClass reg)
        {
            _reg = reg;
            SluchTempl.BindEmptySluch((int) _reg.GetValue("PID"));
            SluchTempl.DoctDataFill(_reg);
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                var id = SluchTempl.SaveSluch();
                if (id != null)
                {
                    Reader2List.CustomExecuteQuery($"Update YamedRegistry SET SLID = {id} Where ID = {_reg.GetValue("ID")}", SprClass.LocalConnectionString);
                }
                return id;
            }).ContinueWith(x =>
            {
               _reg?.SetValue("SLID", x.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }
    }
}
