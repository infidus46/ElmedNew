using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using Yamed.Control;
using Yamed.Control.Editors;
using Yamed.Emr;
using Yamed.Entity;
using Yamed.Pattern;
using Yamed.Registry;
using Yamed.Server;

namespace Yamed
{
    /// <summary>
    /// Логика взаимодействия для DesktopControl.xaml
    /// </summary>
    public partial class DesktopControl : UserControl
    {
        public DesktopControl()
        {
            InitializeComponent();

            if (SprClass.Region == "46")
            {
                
                SearchSluchTile.Visibility = Visibility.Visible;
                //PeopleTile.Visibility = Visibility.Visible;
            }
        }

        private void ReestrTile_OnClick(object sender, EventArgs e)
        {
            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Взаиморасчеты",
                //MyControl = new EconomyWindow(),
                IsCloseable = "True"
            });
        }

        private void SearchSluchTile_OnClick(object sender, EventArgs e)
        {
            //СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            //{
            //    Header = "Поиск",
            //    MyControl = new MeeSearch(),
            //    IsCloseable = "True"
            //});

            var window = new DXWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                //Content = new MeeSearch(),
                Title = "Поиск по параметрам",
                SizeToContent = SizeToContent.WidthAndHeight
            };
            window.Show();
        }

        private void StatisticTile_OnClick(object sender, EventArgs e)
        {

            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "SQL выборки",
                //MyControl = new CustomStatistic(),
                IsCloseable = "True"
            });
        }

        private void RegistryTile_OnClick(object sender, EventArgs e)
        {
            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Регистратура",
                MyControl = new ScheduleControl(),
                IsCloseable = "True"
            });
        }

        private void SearchPacientTile_OnClick(object sender, EventArgs e)
        {
            //СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            //{
            //    Header = "Регистратура",
            //    MyControl = new Elmedicine.AttachedPeople.AttachedPrimaryTab(),
            //    IsCloseable = "True"
            //});
        }
        private void SqlBuilderTile_OnClick(object sender, EventArgs e)
        {
            Process.Start(@"C:\Program Files\ActiveDBSoft\Active Query Builder 3 WPF Professional Edition\DemoWPF.exe");
        }

        private void SettingsTile_OnClick(object sender, EventArgs e)
        {
            var window = new DXWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new SqlDbGUI(),
                Title = "Конструктор форм",
                Width = 1000, Height = 600};
            window.Show();
        }

        private void EmrPacientTile_OnClick(object sender, EventArgs e)
        {
            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "ЭМК Пациента",MyControl = new ClinicEmrPacient(),
                IsCloseable = "True"
            });
        }

        private void NsiTile_OnClick(object sender, EventArgs e)
        {
            var window = new DXWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new NsiControl(),
                Title = "НСИ",
                Width = 1000,
                Height = 600
            };
            window.Show();
        }
    }
}
