using DevExpress.Xpf.Core;
using System;
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

namespace Yamed.Oms
{
    /// <summary>
    /// Логика взаимодействия для Period_unload.xaml
    /// </summary>
    public partial class Period_unload : UserControl
    {
        
        public Period_unload()
        {
            InitializeComponent();
        }
        
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OmsMenu.y1_ = BeginDateEdit.DateTime.Year.ToString();
            OmsMenu.m1_ = BeginDateEdit.DateTime.Month.ToString();
            OmsMenu.y2_ = EndDateEdit.DateTime.Year.ToString();
            OmsMenu.m2_ = EndDateEdit.DateTime.Month.ToString();
            ((DXWindow)this.Parent).Close();

        }
    }
}
