using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;
using Yamed.Core;
using Yamed.Registry.DragAndDrop;
using Yamed.Registry.ViewModels;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.Registry.Views
{
    /// <summary>
    /// Interaction logic for BookingView.xaml
    /// </summary>
    public partial class BookingView : UserControl
    {
        public BookingView()
        {
            InitializeComponent();
            this.Loaded += (s, e) => AddToDropTargets();
        }

        private void AddToDropTargets()
        {
            DragAndDrop.DragManager.Instance.AddDropTarget(this,
                new DropOptions
                {
                    DragEnter = this.DragEnter,
                    DragOver = this.DragOver,
                    DragLeave = this.DragLeave,
                    Dropped = this.Dropped
                });
        }

        /// Вход в границы элемента
        /// </summary>
        /// <param name="source"></param>
        private new void DragEnter(MyDragDropEventArgs e)
        {
            var vm = (BookingViewModel) this.DataContext;
            //Если пустой элемент
            if (vm.IsPatientExist == false && vm.IsUnavailable == false && e.Data is BookingViewModel &&
                e.Data != this.DataContext)
                border.Opacity = 1; //Показать рамку
            else e.DragArgs.Effects = DragDropEffects.None;
        }

        /// <summary>
        /// Перемещение внутри элемента
        /// </summary>
        /// <param name="source"></param>
        private new void DragOver(MyDragDropEventArgs e)
        {
            var vm = (BookingViewModel) this.DataContext;

            if (
                !(vm.IsPatientExist == false && vm.IsUnavailable == false && e.Data is BookingViewModel &&
                  e.Data != this.DataContext))
                e.DragArgs.Effects = DragDropEffects.None;
        }

        /// <summary>
        /// Выход из границ элемента
        /// </summary>
        /// <param name="source"></param>
        private new void DragLeave(MyDragDropEventArgs e)
        {
            border.Opacity = 0; //Скрыть рамку
        }

        /// <summary>
        /// Отпускание кнопки после перетаскивания элемента
        /// </summary>
        /// <param name="source"></param>
        private void Dropped(MyDragDropEventArgs e)
        {
            var source = e.DragSource;
            var vm = (BookingViewModel) this.DataContext;
            if (vm.IsPatientExist == false && vm.IsUnavailable == false && e.Data is BookingViewModel &&
                e.Data != this.DataContext)
            {
                var dragged = ((BookingViewModel) e.Data);

                var currentName = dragged.PatientFullName;
                Task.Factory.StartNew(() =>
                    {
                        Reader2List.CustomExecuteQuery($@"
                           declare @pid int
                           select @pid = pid from [dbo].[YamedRegistry] WHERE ID = {dragged.Id}

                           UPDATE [dbo].[YamedRegistry] SET [PID] = NULL
                           WHERE ID = {dragged.Id}

                           UPDATE [dbo].[YamedRegistry] SET [PID] = @pid
                           WHERE ID = {vm.Id}", SprClass.LocalConnectionString);

                    })
                    .ContinueWith(x =>
                    {
                        dragged.RemovePatient();
                        vm.SetPatient(currentName);
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void Booking_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //var vm = (BookingViewModel)this.DataContext;
            //var amb = new SluchTemplate();
            //var window = new DXWindow
            //{
            //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
            //    Content = amb,
            //    Title = "Поликлинический случай",
            //    //WindowStyle = WindowStyle.None
            //};
            //window.ShowDialog();
        }


        private void BookingNonExist_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PatientReg();
        }

        private void ReserveItem_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = (BookingViewModel) this.DataContext;

            var isReserved = SqlReader.Select($@"
                           
                           Select Reserve, PID FROM [dbo].[YamedRegistry]
                           WHERE ID = {vm.Id}", SprClass.LocalConnectionString)
                .Select(x => x.GetValue("PID") != null || (bool) x.GetValue("Reserve"))
                .First();
            if (isReserved)
            {
                DXMessageBox.Show(
                    "Ячейка расписания не доступна, пожалуйста обновите сетку расписания для получения актуальных данных",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Task.Factory.StartNew(() =>
            {
                Reader2List.CustomExecuteQuery($@"
                           
                           UPDATE [dbo].[YamedRegistry] SET Reserve = 1
                           WHERE ID = {vm.Id}", SprClass.LocalConnectionString);
            });

            var prc = new PacientReserveControl();
            prc.BindReserve(vm);

            var window = new DXWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = prc,
                Title = "Резервирование ячейки расписания",
                SizeToContent = SizeToContent.WidthAndHeight
                //WindowStyle = WindowStyle.None
            };
            window.ShowDialog();

            //Task.Factory.StartNew(() =>
            //    {
            //        Reader2List.CustomExecuteQuery($@"

            //               UPDATE [dbo].[YamedRegistry] SET [Reserve] = 1
            //               WHERE ID = {vm.Id}", SprClass.LocalConnectionString);
            //    })
            //    .ContinueWith(x =>
            //    {
            //        vm.Reserve(true);
            //    }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void PatientReg()
        {



            var vm = (BookingViewModel) this.DataContext;

            var isReserved = SqlReader.Select($@"
                           
                           Select Reserve, PID FROM [dbo].[YamedRegistry]
                           WHERE ID = {vm.Id}", SprClass.LocalConnectionString)
                .Select(x => x.GetValue("PID") != null || (bool) x.GetValue("Reserve"))
                .First();
            if (isReserved)
            {
                DXMessageBox.Show(
                    "Ячейка расписания не доступна, пожалуйста обновите сетку расписания для получения актуальных данных",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Task.Factory.StartNew(() =>
            {
                Reader2List.CustomExecuteQuery($@"
                           
                           UPDATE [dbo].[YamedRegistry] SET Reserve = 1
                           WHERE ID = {vm.Id}", SprClass.LocalConnectionString);
            });

            var window = new DXWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new PatientRegistration(vm),
                Title = "Регистрация пациента",

                //WindowStyle = WindowStyle.None
            };
            window.ShowDialog();

        }

        private void RegItem_OnClick(object sender, RoutedEventArgs e)
        {
            PatientReg();
        }

        private void DelItem_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = (BookingViewModel) this.DataContext;

            Task.Factory.StartNew(() =>
                {
                    Reader2List.CustomExecuteQuery($@"
                           
                           UPDATE [dbo].[YamedRegistry] SET PID = NULL
                           WHERE ID = {vm.Id}", SprClass.LocalConnectionString);
                })
                .ContinueWith(x =>
                {
                    vm.RemovePatient();
                }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private void EditItem_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = (BookingViewModel) this.DataContext;

            var isReserved = SqlReader.Select($@"
                           
                           Select Reserve, PID FROM [dbo].[YamedRegistry]
                           WHERE ID = {vm.Id}", SprClass.LocalConnectionString)
                .Select(x => x.GetValue("PID") != null || (bool) x.GetValue("Reserve"))
                .First();
            if (isReserved)
            {
                DXMessageBox.Show(
                    "Ячейка расписания не доступна, пожалуйста обновите сетку расписания для получения актуальных данных",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Task.Factory.StartNew(() =>
            {
                Reader2List.CustomExecuteQuery($@"
                           
                           UPDATE [dbo].[YamedRegistry] SET Reserve = 1
                           WHERE ID = {vm.Id}", SprClass.LocalConnectionString);
            });


            var prc = new PacientReserveControl();
            prc.BindReserve(vm);

            var window = new DXWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = prc,
                Title = "Резервирование ячейки расписания",
                SizeToContent = SizeToContent.WidthAndHeight
                //WindowStyle = WindowStyle.None
            };
            window.ShowDialog();

        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = (BookingViewModel) this.DataContext;

            Task.Factory.StartNew(() =>
                {
                    Reader2List.CustomExecuteQuery($@"

                           UPDATE [dbo].[YamedRegistry] SET 
                             [Reserve] = 0,
                             [PacientName] = NULL,
                             [PacientContact] = NULL,
                             [PacientComent] = NULL
                           WHERE ID = {vm.Id}", SprClass.LocalConnectionString);
                })
                .ContinueWith(x =>
                {
                    vm.Reserve(false);
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void TapPrintItem_OnClick(object sender, RoutedEventArgs e)
        {
            PrintHelper.PrintDirect(GetReport());
        }

        private XtraReport GetReport()
        {

            var repRow = SqlReader.Select("Select * from YamedReports where RepName = '_tapTemplate'",
                SprClass.LocalConnectionString)[0];


            XtraReport report = new XtraReport();
            var rl = (string) ObjHelper.GetAnonymousValue(repRow, "Template");
            if (!string.IsNullOrWhiteSpace(rl))
            {
                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);

                writer.Write(rl);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                report.LoadLayout(stream);

                stream.Close();
                writer.Close();
            }
            
            var vm = (BookingViewModel) this.DataContext;

            report.Parameters[0].Value = vm.Id;
            //((SqlDataSource)report.DataSource).ConnectionParameters = new MsSqlConnectionParameters(@"ELMEDSRV\ELMEDSERVER", "elmedicine", "sa", "12345678", MsSqlAuthorizationType.Windows);
            report.CreateDocument();
            return report;
        }

        private void TapPreview_OnClick(object sender, RoutedEventArgs e)
        {
            //// Invoke the Ribbon Print Preview form  
            //// and load the report document into it. 
            //PrintHelper.ShowRibbonPrintPreview(this, report);

            //// Invoke the Ribbon Print Preview form modally. 
            //PrintHelper.ShowRibbonPrintPreviewDialog(Control.WindowUtils.FindOwnerWindow(), GetReport());

            //// Invoke the standard Print Preview form  
            //// and load the report document into it. 
            //PrintHelper.ShowPrintPreview(this, new XtraReport1());

            //// Invoke the standard Print Preview form modally. 
            PrintHelper.ShowPrintPreviewDialog(Control.WindowUtils.FindOwnerWindow(), GetReport());

            //// Invoke the standard Print Preview form modally. 
            //PrintHelper.PrintDirect(GetReport());
        }
    }
}
