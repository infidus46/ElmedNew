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
using DevExpress.Xpf.WindowsUI.Navigation;
using Yamed.Core;
using Yamed.Server;
using NavigationEventArgs = DevExpress.Xpf.WindowsUI.Navigation.NavigationEventArgs;

namespace Yamed.Ambulatory
{
    /// <summary>
    /// Логика взаимодействия для WorkSpaceControl.xaml
    /// </summary>
    public partial class WorkSpaceControl : UserControl
    {
        public WorkSpaceControl()
        {
            InitializeComponent();

            DoctorEdit.DataContext = Reader2List.CustomAnonymousSelect(@"Select * from Yamed_Spr_MedicalEmployee where IsInSchedule=1", SprClass.LocalConnectionString);

        }

        private void tempButton_Click(object sender, RoutedEventArgs e)
        {
            var date = ((DateTime?) WorkDateEdit.EditValue)?.ToString("yyyyMMdd") ?? DateTime.Today.ToString("yyyyMMdd");
            var doct = DoctorEdit.EditValue;

            var sqlToStartNew = $@"
                    Select pa.FAM, pa.IM, pa.OT, FLOOR(DATEDIFF(DAY, DR, BeginTime)/365.25) DR_V, r.*
					,[LPU_ID], [USL_OK_ID], [PODR_ID], [OTD_ID], [PRVS_ID], [PROFIL_ID], [DET_ID], [VID_POM_ID], d.[SNILS]
                    from YamedRegistry r
                    Join D3_PACIENT_OMS pa on r.PID = pa.ID
                    Join Yamed_Spr_MedicalEmployee d on r.DID = d.id AND d.id = {doct}
                    where CAST(r.BeginTime as DATE) IN ( '{date}' )";

            PacientBookingEdit.DataContext = SqlReader.Select(sqlToStartNew, SprClass.LocalConnectionString);
        }

        private void NavigationFrame_OnNavigated(object sender, NavigationEventArgs e)
        {
            if (e.Source as string == "SluchControl")
            {
                ControlPanelGrid.IsEnabled = false;
                var item = (DynamicBaseClass)PacientBookingEdit.SelectedItem;
                if (item.GetValue("SLID") == null)
                {
                    ((SluchControl) e.Content).BindEmptySluch(item);
                }
                else
                    ((SluchControl) e.Content).BindSluch((int) item.GetValue("SLID"));
            }
            else
            {
                ControlPanelGrid.IsEnabled = true;
            }
        }
    }
}
