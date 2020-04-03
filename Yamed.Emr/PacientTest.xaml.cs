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
using DevExpress.Xpf.Core;
using Yamed.Control;
using Yamed.Core;
using Yamed.Entity;

namespace Yamed.Emr
{
    /// <summary>
    /// Логика взаимодействия для PacientTest.xaml
    /// </summary>
    public partial class PacientTest : UserControl
    {
        public PacientTest()
        {
            InitializeComponent();
        }

        private SluchTemplateD3 _sluchTemplateD3;
        public PacientTest(SluchTemplateD3 sluchTemplateD3)
        {
            InitializeComponent();
            _sluchTemplateD3 = sluchTemplateD3;
        }

        private void PatientRegistration_Closed(object sender, EventArgs e)
        {
            //Task.Factory.StartNew(() =>
            //{
            //    Reader2List.CustomExecuteQuery($@"
            //    UPDATE [dbo].[YamedRegistry] SET Reserve = 0
            //    WHERE ID = {_bv}", SprClass.LocalConnectionString);
            //});
            (this.Parent as DXWindow).Closed -= PatientRegistration_Closed;
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            EmrPacientGrid.FilterString = $"([FAM] like '{FamEdit.EditValue}%' and [IM] like '{ImEdit.EditValue}%' and [OT] like '{OtEdit.EditValue}%')";
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new DXWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new EmrFullControl(),
                Title = "Регистрация пациента",
            };
            window.ShowDialog();
            EmrPacientGrid.LinqInstantFeedbackDataSourcePacient.Refresh();
        }

        private void ButtonBase2_OnClick(object sender, RoutedEventArgs e)
        {
            var row = DxHelper.GetSelectedGridRow(EmrPacientGrid);
            var pid = (int)ObjHelper.GetAnonymousValue(row, "ID");

            var window = new DXWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new EmrFullControl(null, pid),
                Title = "Регистрация пациента",
            };
            window.ShowDialog();
            EmrPacientGrid.LinqInstantFeedbackDataSourcePacient.Refresh();
        }

        private void PatientRegistration_OnLoaded(object sender, RoutedEventArgs e)
        {
            (this.Parent as DXWindow).Closed += PatientRegistration_Closed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var row = DxHelper.GetSelectedGridRow(EmrPacientGrid);
            var pid = (int)ObjHelper.GetAnonymousValue(row, "ID");
            var fam = (string)ObjHelper.GetAnonymousValue(row, "FAM");
            var im = (string)ObjHelper.GetAnonymousValue(row, "IM");
            var ot = (string)ObjHelper.GetAnonymousValue(row, "OT");
            var dr = (DateTime)ObjHelper.GetAnonymousValue(row, "DR");
            var polis = (string)ObjHelper.GetAnonymousValue(row, "NPOLIS");

            _sluchTemplateD3._zsl.D3_PID = pid;
            _sluchTemplateD3.PacientGroup.Header = $"Пациент: {fam} {im} {ot} {dr:yyyy}";
            _sluchTemplateD3.polisBox.EditValue = polis;

            (this.Parent as DXWindow).Close();
        }
    }
}
