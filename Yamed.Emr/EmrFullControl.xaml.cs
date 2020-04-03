using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.XtraEditors.DXErrorProvider;
using Yamed.Core;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.Emr
{
    /// <summary>
    /// Логика взаимодействия для ClinicEmrPacientControl.xaml
    /// </summary>
    public partial class EmrFullControl : UserControl
    {

        public EmrFullControl()
        {
            InitializeComponent();
            PacientControl.BindEmptyPacient();
        }

        private SluchTemplateD3 _sluchTemplateD3;
        public EmrFullControl(SluchTemplateD3 sluchTemplateD3, int pid = 0)
        {
            InitializeComponent();
            if (pid == 0)
                PacientControl.BindEmptyPacient();
            else PacientControl.BindPacient(pid);
            _sluchTemplateD3 = sluchTemplateD3;

        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
           int pid = PacientControl.SavePacient(_sluchTemplateD3._sc.ID);

            var fam = PacientControl.FamBox.EditValue;
            var im = PacientControl.ImBox.EditValue;
            var ot = PacientControl.OtBox.EditValue;
            var dr = PacientControl.drBox.EditValue;
            var polis = PacientControl.polisBox.EditValue;

            _sluchTemplateD3._zsl.D3_PID = pid;
            _sluchTemplateD3.PacientGroup.Header = $"Пациент: {fam} {im} {ot} {dr:yyyy}";
            _sluchTemplateD3.polisBox.EditValue = polis;

            (this.Parent as DXWindow)?.Close();

        }
    }
}
