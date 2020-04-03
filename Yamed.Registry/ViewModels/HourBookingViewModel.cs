using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using DevExpress.Xpf.CodeView;
using Yamed.Core;

namespace Yamed.Registry.ViewModels
{

    public class HourBookingViewModel : ViewModelBase
    {
        private string _isExpanded;
        private int _viewHeight;
        private int _pacientCnt;
        private SolidColorBrush _bookingColor;

        public HourBookingViewModel(int startHour, List<BookingViewModel> allBookings)
        {
            ViewHeight = 150;
            _isExpanded = "True";

            StartHour = startHour;
            StartTime = DateTime.Today.AddHours(startHour);

            Bookings = new ObservableCollection<BookingViewModel>();
            Bookings.CollectionChanged += Bookings_CollectionChanged;

            Bookings.AddRange(allBookings.Where(b => b.StartTime.Hour == startHour));

            AllCnt = Bookings.Count;
            PacientCnt = AllCnt - (from b in Bookings
                                   where b.IsPatientExist
                                   select b).Count();


            if (PacientCnt == 0)
                BookingColor = new SolidColorBrush(Colors.Red);
            else BookingColor = new SolidColorBrush(Colors.Black);

        }

        private void Bookings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (BookingViewModel item in e.OldItems)
                {
                    //Removed items
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (BookingViewModel item in e.NewItems)
                {
                    //Added items
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsPatientExist") return;
            PacientCnt = AllCnt - (from b in Bookings
                          where b.IsPatientExist
                          select b).Count();
            if (PacientCnt == 0)
                BookingColor = new SolidColorBrush(Colors.Red);
            else BookingColor = new SolidColorBrush(Colors.Green);

        }

        public int PacientCnt
        {
            get { return _pacientCnt; }
            set
            {
                _pacientCnt = value;
                RaisePropertyChanged("PacientCnt");
            }
        }

        public SolidColorBrush BookingColor
        {
            get { return _bookingColor; }
            set
            {
                _bookingColor = value;
                RaisePropertyChanged("BookingColor");
            }
        }

        public int AllCnt { get; set; }

        public int StartHour { get; set; }

        public DateTime StartTime { get; set; }

        public ObservableCollection<BookingViewModel> Bookings { get; set; }   

        public string IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                RaisePropertyChanged("IsExpanded");
            }
        }
        public int ViewHeight
        {
            get { return _viewHeight; }
            set
            {
                _viewHeight = value;
                RaisePropertyChanged("ViewHeight");
            }
        }
    }
}
