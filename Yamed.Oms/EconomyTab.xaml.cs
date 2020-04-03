using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using System.Xml.Linq;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ServerMode;
using DevExpress.Xpf.Grid;
using Microsoft.Win32;
using Yamed.Control;
using Yamed.Control.Editors;
using Yamed.Core;
using Yamed.Entity;
using Yamed.Reports;
using Yamed.Server;

namespace Yamed.Oms
{
    /// <summary>
    /// Логика взаимодействия для EconomyWindow.xaml
    /// </summary>
    public partial class EconomyWindow : UserControl
    {
        public LinqInstantFeedbackDataSource linqInstantFeedbackDataSource;
        public List<D3_SCHET_SMO_OMS> OmsList;

        public static readonly DependencyProperty IsSmoTableVisibleProperty =
            DependencyProperty.Register("IsSmoTableVisible", typeof (Visibility), typeof (EconomyWindow), null);

        public Visibility IsSmoTableVisible
        {
            get
            {
                return (Visibility)GetValue(IsSmoTableVisibleProperty);
            }
            set
            {
                SetValue(IsSmoTableVisibleProperty, value);
            }
        }

        public EconomyWindow()
        {
            InitializeComponent();
            DataContext = this;
            IsSmoTableVisible = SprClass.ProdSett.OrgTypeStatus == OrgType.Lpu ? Visibility.Visible : Visibility.Collapsed;
            if (IsSmoTableVisible == Visibility.Collapsed)
                Grid1.RowDefinitions[2].Height = GridLength.Auto;

            linqInstantFeedbackDataSource = (LinqInstantFeedbackDataSource)FindResource("LinqInstantFeedbackDataSource");
            var edc =  new YamedDataClassesDataContext()
            {
                ObjectTrackingEnabled = false,
                Connection = { ConnectionString = SprClass.LocalConnectionString }
            };

            IQueryable pQueryable = from sc in edc.D3_SCHET_OMS
                                    join f3 in edc.D3_F003 on sc.CODE_MO equals f3.mcod
                                    join sprsc in edc.Yamed_Spr_SchetType on sc.SchetType equals sprsc.ID
                                    select new
                                    {                                        
                                        ID = sc.ID,
                                        CODE_MO = sc.CODE_MO,
                                        NAME_MO = f3.nam_mok,
                                        PLAT = sc.PLAT,
                                        YEAR = sc.YEAR,
                                        MONTH = sc.MONTH,
                                        NSCHET = sc.NSCHET,
                                        DSCHET = sc.DSCHET,
                                        SUMMAV = sc.SUMMAV,
                                        SUMMAP = sc.SUMMAP,
                                        SANK_MEK = sc.SANK_MEK,
                                        SANK_MEE = sc.SANK_MEE,
                                        SANK_EKMP = sc.SANK_EKMP,
                                        COMENTS = sc.COMENTS,
                                        COUNT_SL = sc.SD_Z,
                                        sc.OmsFileName,
                                        DISP = sc.DISP,

                                        sc.SchetType,
                                        SchetTypeName = sprsc.NameWithID, // добавил Андрей insidious
                                                                          //sc.ZapFileName,
                                                                          //sc.PersFileName

                                    };

            linqInstantFeedbackDataSource.QueryableSource = pQueryable;

//            var tfoms = Reader2List.CustomSelect<Settings>("Select * from Settings where Name = 'TFOMS'",
//SprClass.LocalConnectionString).SingleOrDefault()?.Parametr;
            //            var isTfoms = tfoms != null && bool.Parse(tfoms);
            //            var isSmo = tfoms != null && !bool.Parse(tfoms);


        }



        static public void SluchUpdate(string command)
        {
            using (SqlConnection con = new SqlConnection(SprClass.LocalConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(command, con))
                {
                    con.Open();
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }


        private void calendarRowItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            //var schet = ObjHelper.ClassConverter<D3_SCHET_OMS>(DxHelper.GetSelectedGridRow(gridControl));

            //var window = new DXWindow
            //{
            //    ShowIcon = false, WindowStartupLocation = WindowStartupLocation.Manual,
            //    Content = new CalendarHoliday((string)ObjHelper.GetAnonymousValue(schet, "CODE_MO")),
            //    Title = "Выходные дни"
            //};
            //window.ShowDialog();

        }



        private void EconomyWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            linqInstantFeedbackDataSource.Dispose();
        }

        private void EconomyWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                new Action(delegate()
                {
                    gridControl.ExpandGroupRow(-1);
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                        new Action(delegate()
                        {
                            gridControl.ExpandGroupRow(-2);
                        }));
                }));
            PlatColumnEdit.DataContext = SprClass.Payment;
        }


        private void ThrowLoad_OnItemClick(object sender, ItemClickEventArgs e)
        {
            string inDir;
            string outDir;
            var schet = new D3_SCHET_OMS();
            using (var dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
            {
                inDir = dc.Settings.Single(x => x.Name == "InDirectory").Parametr + @"\";
                outDir = dc.Settings.Single(x => x.Name == "OutDirectory").Parametr + @"\";
            }

            string[] omsArray = Directory.GetFiles(inDir, "*.oms");

            int count = 0;
            int scount = omsArray.Count();
            MessageBoxResult mbResult =
                MessageBox.Show(
                    "Запустить импорт " + omsArray.Count() + " файлов?",
                    "Импорт счетов СМО",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext(); //get UI thread context 
            if (mbResult == MessageBoxResult.Yes)
            {
                var mekTask = Task.Factory.StartNew(() =>
                {
                        foreach (string oms in omsArray)
                        {
                        }
                });
            }


        }

        private void OmsAddItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var oms = new D3_SCHET_SMO_OMS();
            var row = DxHelper.GetSelectedGridRow(gridControl);
            oms.SCHET_ID = (int)ObjHelper.GetAnonymousValue(row, "ID");
            oms.YEAR = (int)ObjHelper.GetAnonymousValue(row, "YEAR");
            oms.MONTH = (int)ObjHelper.GetAnonymousValue(row, "MONTH");
            oms.CODE_MO = (string)ObjHelper.GetAnonymousValue(row, "CODE_MO");

            OmsList.Add(oms);
            //var type = typeof (SCHET_OMS);
            //var obj = Activator.CreateInstance(type);
            var sprEditWindow = new UniSprEditControl("D3_SCHET_SMO_OMS", oms, false, SprClass.LocalConnectionString);
            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Content = sprEditWindow,
                Title = "Новая запись"
            };
            var result = window.ShowDialog();
            if (result != true)
            {
                OmsList.Remove(oms);
            }
            OmsList = Reader2List.CustomSelect<D3_SCHET_SMO_OMS>(
                $@"Select * From D3_SCHET_SMO_OMS WHERE SCHET_ID = {ObjHelper.GetAnonymousValue(row, "ID")}"
                , SprClass.LocalConnectionString);
            gridControl2.DataContext = OmsList;
        }

        private void GridControl_OnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            var row = DxHelper.GetSelectedGridRow(gridControl);
            if (row == null) return;

            var id = ObjHelper.GetAnonymousValue(row, "ID");
            OmsList =
                Reader2List.CustomSelect<D3_SCHET_SMO_OMS>(
                    $@"Select * From D3_SCHET_SMO_OMS WHERE SCHET_ID = {id}"
                    , SprClass.LocalConnectionString);
            gridControl2.DataContext = OmsList;
        }

        private void OmsEditItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var row = gridControl2.SelectedItem;
            if (row == null) return;
            var sprEditWindow = new UniSprEditControl("D3_SCHET_SMO_OMS", row, true, SprClass.LocalConnectionString);
            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Content = sprEditWindow,
                Title = "Редактирование"
            };
            window.ShowDialog();
            gridControl2.RefreshData();
        }

        private void OmsDelItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var row = (D3_SCHET_SMO_OMS)gridControl2.SelectedItem;
            if (row == null) return;

            Reader2List.CustomExecuteQuery($"DELETE FROM D3_SCHET_SMO_OMS WHERE {"CODE"}={row.CODE}", SprClass.LocalConnectionString);
            OmsList.Remove(row);
            gridControl2.RefreshData();
        }

        private void RefreshItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            linqInstantFeedbackDataSource.Refresh();
        }

        private void AddItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = (EconomyWindow)((TabElement)СommonСomponents.DxTabObject).MyControl;
            var schet = new D3_SCHET_OMS();

            var sprEditWindow = new UniSprEditControl("D3_SCHET_OMS", schet, false, SprClass.LocalConnectionString);
            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Content = sprEditWindow,
                Title = "Новая запись"
            };

            window.ShowDialog();
            linqInstantFeedbackDataSource.Refresh();
        }

        private void EditItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = (EconomyWindow)((TabElement)СommonСomponents.DxTabObject).MyControl;
            var row = DxHelper.GetSelectedGridRow(gridControl);
            if (row == null) return;

            var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(row);

            var sprEditWindow = new UniSprEditControl("D3_SCHET_OMS", sc, true, SprClass.LocalConnectionString);
            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Content = sprEditWindow,
                Title = "Редактирование"
            };

            window.ShowDialog();
            linqInstantFeedbackDataSource.Refresh();
        }

        private void DelItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //var tab = (EconomyWindow)((TabElement)СommonСomponents.DxTabObject).MyControl;
            var row = ObjHelper.ClassConverter<D3_SCHET_OMS>(DxHelper.GetSelectedGridRow(gridControl));
            if (row == null) return;

            MessageBoxResult result = MessageBox.Show("Удалить счет за период " + row.MONTH + "." + row.YEAR + "\n" + SprClass.LpuList.Single(x => x.mcod == row.CODE_MO).NameWithID + "?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                //barButtonItem4.IsEnabled = false;
                TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext(); //get UI thread context 


                bool isDel = false;
                var delSchet = Task.Factory.StartNew(() =>
                {
                    {
                        try
                        {
                            isDel = true;
                            Reader2List.CustomExecuteQuery($@"DELETE FROM D3_SCHET_OMS where ID = {row.ID} ", SprClass.LocalConnectionString);
                        }
                        catch (Exception ex)
                        {
                            isDel = false;
                            Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                ErrorGlobalWindow.ShowError(ex.Message);
                            });
                        }
                    }
                });
                delSchet.ContinueWith(x =>
                {

                    if (isDel)
                    {
                        linqInstantFeedbackDataSource.Refresh();
                        ErrorGlobalWindow.ShowError("Счет удален");
                    }
                    //barButtonItem4.IsEnabled = true;
                    //}
                }, uiScheduler);
            }

            //var id = PublicVoids.GetAnonymousValue(row, "ID");
            //Reader2List.CustomExecuteQuery($"DELETE FROM {"D3_SCHET_OMS"} WHERE {"ID"}={id}", SprClass.LocalConnectionString);
            //tab.linqInstantFeedbackDataSource.Refresh();
        }

        private void DocumentItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var sc = ObjHelper.ClassConverter<D3_SCHET_OMS>(DxHelper.GetSelectedGridRow(gridControl));
            var row = (D3_SCHET_SMO_OMS)gridControl2.SelectedItem;

            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Печатные формы",
                MyControl = new StatisticReports(new object[] { sc.ID }),
                IsCloseable = "True",
            });
        }
    }

    public class MonthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "Ошибка: Пустой код";
            switch ((int)value)
            {
                case 1:
                    return "Январь";
                case 2:
                    return "Февраль";
                case 3:
                    return "Март";
                case 4:
                    return "Апрель";
                case 5:
                    return "Май";
                case 6:
                    return "Июнь";
                case 7: return "Июль";
                case 8:
                    return "Август";
                case 9:
                    return "Сентябрь";
                case 10:
                    return "Октябрь";
                case 11:
                    return "Ноябрь";
                case 12:
                    return "Декабрь";
            }
            return "Ошибка: Неверный код";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return 0;
        }
    }
}