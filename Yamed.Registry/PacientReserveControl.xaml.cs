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
using Yamed.Core;
using Yamed.Registry.ViewModels;
using Yamed.Server;

namespace Yamed.Registry
{
    /// <summary>
    /// Логика взаимодействия для PacientReserveControl.xaml
    /// </summary>
    public partial class PacientReserveControl : UserControl
    {
        private BookingViewModel _bv;
        private DynamicBaseClass _reg;

        private bool _isSaved;

        public PacientReserveControl()
        {
            InitializeComponent();
            _isSaved = false;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _reg.SetValue("Reserve", true);
            Task.Factory.StartNew(() =>
                {
                    //Reader2List.CustomExecuteQuery($@"
                    //       UPDATE [dbo].[YamedRegistry] 
                    //       SET [Reserve] = 1,
                    //       [PacientName] = '{_reg.GetValue("PacientName")}'
                    //       [PacientContact] = '{_reg.GetValue("PacientContact")}'
                    //       [PacientComent] = '{_reg.GetValue("PacientComent")}'
                    //       WHERE ID = {_bv.Id}", SprClass.LocalConnectionString);
                    Reader2List.CustomExecuteQuery(Reader2List.CustomUpdateCommand("YamedRegistry", _reg, "ID"), SprClass.LocalConnectionString);
                })
                .ContinueWith(x =>
                {
                    _bv.Reserve(true);
                    _bv.ReserveName((string)_reg.GetValue("PacientName"), (string)_reg.GetValue("PacientContact"), (string)_reg.GetValue("PacientComent"));
                }, TaskScheduler.FromCurrentSynchronizationContext());
            _isSaved = true;
            (this.Parent as DXWindow)?.Close();

        }

        public void BindReserve(BookingViewModel bv)
        {
            _bv = bv;
            Task.Factory.StartNew(() =>
            {
                _reg = SqlReader.Select2($"SELECT * FROM YamedRegistry WHERE ID = {_bv.Id}", SprClass.LocalConnectionString).Single();

            }).ContinueWith(x =>
            {
                ReserveGrid.DataContext = _reg;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void PacientReserveControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            (this.Parent as DXWindow).Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            if (_isSaved) return;

            Task.Factory.StartNew(() =>
            {
                Reader2List.CustomExecuteQuery($@"
                UPDATE [dbo].[YamedRegistry] SET Reserve = 0
                WHERE ID = {_bv.Id}", SprClass.LocalConnectionString);
            });
            (this.Parent as DXWindow).Closed -= OnClosed;
        }
    }
}
