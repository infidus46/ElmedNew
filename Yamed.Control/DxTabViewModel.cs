using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm;

namespace Yamed.Control
{
    public class DxTabViewModel : ViewModelBase
    {
        ObservableCollection<TabElement> _tabElements;

        public DxTabViewModel()
        {
            _tabElements = new ObservableCollection<TabElement>();
        }

        public ObservableCollection<TabElement> TabElements
        {
            get
            {
                return _tabElements;
            }
            set
            {
                _tabElements = value;
                RaisePropertyChanged("TabElements");
            }
        }

    }

    public class TabElement
    {
        public string Header { get; set; }
        public UserControl MyControl { get; set; }
        public string IsCloseable { get; set; }
        public ObservableCollection<MenuElement> TabLocalMenu { get; set; }

        public TabElement()
        {
        }
    }

    public class MenuElement
    {
        public string Content { get; set; }
        public Uri Glyph { get; set; }
        public ICommand Command { get; set; }
    }
}
