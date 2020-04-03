using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Core;
using GalaSoft.MvvmLight.Command;
using Yamed.Control;
using Yamed.Control.Editors;
using Yamed.Server;

namespace Yamed.Registry
{
    public class RegistryMenu
    {
        public ObservableCollection<MenuElement> MenuElements;

        public RegistryMenu()
        {
            MenuElements = new ObservableCollection<MenuElement>()
            {
                new MenuElement
                {
                    Content = "Генератор Расписания",
                    Glyph = new Uri("/Yamed.Icons;component/Icons/Calendar-512.png", UriKind.Relative),
                    Command = new RelayCommand(() =>
                        {
                            var window = new DXWindow
                            {
                                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                                Content = new ScheduleGen(),
                                Title = "Генератор расписания",
                                Width = 800,
                                Height = 550
                            };
                            window.ShowDialog();
                        },
                        () => true)
                },
                new MenuElement
                {
                    Content = "Модель расписания",
                    Glyph = new Uri("/Yamed.Icons;component/Icons/Working_Schedule-512.png", UriKind.Relative),
                    Command = new RelayCommand(() =>
                        {
                            var window = new DXWindow
                            {
                                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                                Content = new ScheduleModelControl(),
                                Title = "Модель расписания",
                                Width = 800,
                                Height = 550
                            };
                            window.ShowDialog();
                        },
                        () => true)
                },
                new MenuElement
                {
                    Content = "Медицинский персонал",
                    Glyph = new Uri("/Yamed.Icons;component/Icons/warranty_company-512.png", UriKind.Relative),
                    Command = new RelayCommand(() =>
                        {
                            var window = new DXWindow
                            {
                                ShowIcon = false,
                                WindowStartupLocation = WindowStartupLocation.Manual,
                                Content =
                                    new UniSprFullControl("Yamed_Spr_MedicalEmployee", SprClass.LocalConnectionString,
                                        false),
                                Title = "Медицинский персонал"
                            };
                            window.ShowDialog();
                            ScheduleControl.GetDoctorSpr();
                        },
                        () => true)
                }
            };

        }

    }
}
