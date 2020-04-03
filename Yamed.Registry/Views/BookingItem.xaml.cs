using System;
using System.Windows;
using System.Windows.Controls;
using Yamed.Registry.DragAndDrop;

namespace Yamed.Registry.Views
{
    /// <summary>
    /// Interaction logic for BookingItem.xaml
    /// </summary>
    public partial class BookingItem : UserControl
    {
        public BookingItem()
        {
            InitializeComponent();
            this.Loaded += (s, e) => AddToDragSources();
        }

        private bool isCollapsed;

        public bool IsCollapsed
        {
            get { return isCollapsed; }
            set 
            { 
                isCollapsed = value;
                if (value == true)
                    fio.Visibility = Visibility.Collapsed;
                else fio.Visibility = Visibility.Visible;
            }
        }

        private void AddToDragSources()
        {
            Lazy<FrameworkElement> visual = new Lazy<FrameworkElement>(() => new BookingItem { Height = this.ActualHeight, Width = this.ActualWidth, Opacity = 0.7, DataContext = !this.IsCollapsed ? this.DataContext : null });

            DragManager.Instance.AddDragSource(this,
                new DragOptions
                {
                    Data = this.DataContext,
                    VisualFactory = visual,
                    IsDragOffsetEnabled = true,
                    DragStarted = () => this.Opacity = 0.2,
                    DragEnded = () => this.Opacity = 1,
                    DragCancelled = () => this.Opacity = 1
                });
        }
    }
}
