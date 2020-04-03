using System;
using System.Windows.Controls;

namespace Yamed.OmsExp.SqlEditor
{
    /// <summary>
    /// Логика взаимодействия для MekElementControl.xaml
    /// </summary>
    public partial class AutoMekElement : UserControl
    {
        public AutoMekElement()
        {
            InitializeComponent();
        }

        private void LogBox_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            LogBox.Focus();
            Dispatcher.BeginInvoke(new Action(() => LogBox.SelectionStart = LogBox.Text.Length));
        }

    }
}
