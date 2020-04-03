using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.CodeView;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;
using GalaSoft.MvvmLight.Command;
using Yamed.Control;
using Yamed.Entity;
using Yamed.Registry.Models;
using Yamed.Registry.ViewModels;
using Yamed.Core;
using Yamed.Server;


namespace Yamed.Registry
{
    public class ScheduleDocument : ViewModelBase
    {
        public ObservableCollection<ScheduleViewModel> ScheduleCollection { get; set; }
        public string SqlQuery { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }
        public string Profil { get; set; }

        private bool _isButtonsVisible;
        public bool IsButtonsVisible
        {
            get { return _isButtonsVisible; }
            set
            {
                _isButtonsVisible = value;
                RaisePropertyChanged("IsButtonsVisible");
            }
        }
    }


    /// <summary>
    /// Логика взаимодействия для ScheduleControl.xaml
    /// </summary>
    public partial class ScheduleControl : UserControl
    {
        readonly TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext(); //get UI thread context 

        //public static ObservableCollection<ScheduleViewModel> ScheduleList;
        public ObservableCollection<ScheduleDocument> DocumentCollection { get; set; }

        public ScheduleControl()
        {
            DoctorSpr = new DynamicCollection<object>();
            DocumentCollection = new ObservableCollection<ScheduleDocument>();
            DocumentCollection.CollectionChanged += DocumentCollectionOnCollectionChanged;

            InitializeComponent();

            ScheduleDocumentGroup.View.NewTabCommand = new RelayCommand(() =>
                {
                    //var dates = DateNavigator1.SelectedDates;
                    var rows = DoctorSpr.OfType<DynamicBaseClass>().Where(x => (bool?) x.GetValue("Checked") == true);
                    ScheduleProcess(DateNavigator1.SelectedDates, rows);
                },
                () => DateNavigator1.SelectedDates.Any());

            //Yamed.Registry.DragAndDrop.DragManager.Instance.AdornerCanvas = this.adornerLayer;

            Task.Factory.StartNew(GetDoctorSpr).ContinueWith((t) =>
            {
                ProfilTreeList.DataContext = DoctorSpr;

                var dates = new List<DateTime> { DateTime.Today };
                foreach (var node in DoctorSpr.OfType<DynamicBaseClass>().Where(g => g.GetValue("ParentID") != null)
                    .GroupBy(g => g.GetValue("ParentID")).Select(g => g.Key))
                {
                    var rows =
                        DoctorSpr.OfType<DynamicBaseClass>()
                            .Where(x => (int?)x.GetValue("ParentID") == (int?)node);
                    ScheduleProcess(dates, rows, true);
                }
            }, uiScheduler);

            ScheduleDocumentGroup.DataContext = DocumentCollection;

            //var doctData = Reader2List.CustomAnonymousSelect("Select * from DoctorBd", SprClass.LocalConnectionString);
            //GridControl1.DataContext = doctData;


        }

        public static DynamicCollection<object> DoctorSpr { get; set; }
        public static void GetDoctorSpr()
        {
            var temp = SqlReader.Select2(@"
Select me.ID, FIO Name, ISNULL(-v2.ID, 0) ParentID, Room, CAST (0 as Bit) Checked
                                            from Yamed_Spr_MedicalEmployee me 
                                            Left join V002 v2 on me.PROFIL_ID = v2.ID
                                            where IsInSchedule =1
UNION
Select -ID ID, PRNAME Name, null, null, CAST (0 as Bit) Checked from V002 where ID in (Select PROFIL_ID from Yamed_Spr_MedicalEmployee where IsInSchedule =1)

UNION 
Select 0 ID, 'ОБЩИЕ' Name, null, null, CAST (0 as Bit) Checked where (Select COUNT(*) from Yamed_Spr_MedicalEmployee where IsInSchedule =1 AND PROFIL_ID is null) > 0
", SprClass.LocalConnectionString, DoctorSpr.GetDynamicType());

            DoctorSpr.Clear();
            DoctorSpr.AddRange(temp);
            DoctorSpr.SetDynamicType(temp.GetDynamicType());
            temp.Clear();

        }


        private void DocumentCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                Task.Factory.StartNew(() =>
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        ScheduleDocumentGroup.SelectLast();// = DocumentCollection.Count - 1;
                    });
                });
            }

        }


        private void ScheduleProcess(IEnumerable<DateTime> dates, IEnumerable<DynamicBaseClass> docts, bool ignoreMess = false)
        {
            var localDates = dates.ToList();
            var localDocts = docts.ToList();

            string idrow = null;
            for (int i = 0; i < localDocts.Count; i++)
            {
                idrow = idrow + (int)ObjHelper.GetAnonymousValue(localDocts[i], "ID");
                if (i != localDocts.Count - 1) { idrow = idrow + ","; }
            }

            string days = null;
            for (int i = 0; i < localDates.Count; i++)
            {
                days = days + "'" + localDates[i].ToString("yyyyMMdd") + "'";
                if (i != localDates.Count - 1)
                {
                    days = days + ",";
                }
            }

            string sqlToStartNew = null;
            if (idrow == null)
            {
                sqlToStartNew = $@"Select pa.ID as PACID, pa.FAM, pa.IM, pa.OT, d.FIO, d.id doctID, r.* from YamedRegistry r
                    Left Join Pacient pa on r.PID = pa.ID
                    Join Yamed_Spr_MedicalEmployee d on((r.DID = d.id)) where CAST(r.BeginTime as DATE) IN (" + days + ")";
            }
            else
            {
                sqlToStartNew = $@"Select pa.ID as PACID, pa.FAM, pa.IM, pa.OT, d.FIO, d.id doctID, r.* from YamedRegistry r
                    Left Join Pacient pa on r.PID = pa.ID
                    Join Yamed_Spr_MedicalEmployee d on((r.DID = d.id)) AND (d.id IN(" + idrow + "))" +
                     "where CAST(r.BeginTime as DATE) IN (" + days + ")";
            }


            Task.Factory.StartNew(() =>
                {
                    return SqlReader.Select(sqlToStartNew, SprClass.LocalConnectionString);
                })
                .ContinueWith(x =>
                {
                    if (x.Result.Any())
                    {
                        var sd = new ScheduleDocument();
                        sd.ScheduleCollection = GetSchedule(x.Result);
                        sd.SqlQuery = sqlToStartNew;
                        sd.Date1 = localDates.Min().ToShortDateString();
                        if (localDates.Count > 1)
                            sd.Date2 = localDates.Max().ToShortDateString();
                        if (localDocts.Any())
                        {
                            if (localDocts.Count == 1)
                            {
                                sd.Profil = (string) localDocts[0].GetValue("Name");
                            }
                            else
                            {
                                var k =
                                    localDocts.Where(g => g.GetValue("ParentID") != null)
                                        .GroupBy(g => g.GetValue("ParentID"))
                                        .Select(g => g.Key)
                                        .ToList();
                                if (k.Count > 1)
                                    k.ForEach(g => sd.Profil += "*");
                                else
                                    sd.Profil = (string)
                                        DoctorSpr.OfType<DynamicBaseClass>()
                                            .First(p => (int) p.GetValue("ID") == (int) k[0])
                                            .GetValue("Name");
                                k.Clear();
                            }
                        }
                        else
                        {
                            sd.Profil = "Все профили";
                        }
                        DocumentCollection.Add(sd);
                    }
                    else
                    {
                        if (!ignoreMess)
                        {
                            DXMessageBox.Show("Для выбранных параметров не найдена сетка расписания", "Внимание",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }

                    localDates.Clear();
                    localDocts.Clear();

                }, TaskScheduler.FromCurrentSynchronizationContext());
        } 

        private ObservableCollection<ScheduleViewModel> GetSchedule(ObservableCollection<DynamicBaseClass> collection)
        {
            var schedules = new ObservableCollection<ScheduleViewModel>();
            var groupSchedule = from d in (IEnumerable<dynamic>) collection
                group d by new
                {
                    date = ((DateTime) ObjHelper.GetAnonymousValue(d, "BeginTime")).Date,
                    doctID = ObjHelper.GetAnonymousValue(d, "doctID")
                }
                into newGroup
                select new
                {
                    key = newGroup.Key,
                    list = newGroup.ToList()
                };

            schedules.AddRange(from sch in groupSchedule
                let pacientDatas = (from p in sch.list where ObjHelper.GetAnonymousValue(p, "PACID") != null select p)
                let bookingModels = BookingModelGenerator.GetBookingModels(pacientDatas)
                select new ScheduleViewModel(bookingModels, sch.list));

            #region old variant
            //foreach (var sch in groupSchedule)
            //{
            //    var pacientDatas = from p in sch.list
            //                       where ObjHelper.GetAnonymousValue(p, "PACID") != null
            //                       select p;

            //    var bookingModels = BookingModelGenerator.GetBookingModels(pacientDatas);
            //    var model1 = new ScheduleViewModel(bookingModels, sch.list);
            //    list.Add(model1);
            //}
            //ScheduleItems.DataContext = ScheduleList;
            #endregion old variant

            return schedules;
        }


        private void UpdateDocumentButton_OnClick(object sender, RoutedEventArgs e)
        {
            var sdoc = DocumentCollection[ScheduleDocumentGroup.SelectedIndex];

            Task.Factory.StartNew(() =>
            {
                return SqlReader.Select(sdoc.SqlQuery, SprClass.LocalConnectionString);
            })
                .ContinueWith(x =>
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                        new Action(delegate()
                        {
                            foreach (var sch in sdoc.ScheduleCollection)
                            {
                                foreach (var hb in sch.HourBookings)
                                {
                                    foreach (var b in hb.Bookings)
                                    {
                                        var row = x.Result.Single(r => (int) r.GetValue("ID") == b.Id);

                                        if (b.IsUnavailable != (bool) row.GetValue("Reserve"))
                                        {
                                            b.Reserve((bool) row.GetValue("Reserve"));
                                            b.ReserveName((string)row.GetValue("PacientName"), (string)row.GetValue("PacientContact"), (string)row.GetValue("PacientComent"));
                                        }

                                        if (row.GetValue("PACID") == null)
                                        {
                                            b.RemovePatient();
                                        }
                                        else
                                        {
                                            b.SetPatient((string) row.GetValue("FAM") + " " + row.GetValue("IM") + " " +
                                                         row.GetValue("OT"));
                                        }
                                    }
                                }
                            }
                        }));
                }, uiScheduler);
        }

        private void ScheduleControl_OnUnloaded(object sender, RoutedEventArgs e)
        {
            DocumentCollection.Clear();
        }

        private void DeleteDocumentButton_OnClick(object sender, RoutedEventArgs e)
        {
            DocumentCollection.RemoveAt(ScheduleDocumentGroup.SelectedIndex);
        }

        private void ScheduleDocumentGroup_OnSelectionChanged(object sender, TabControlSelectionChangedEventArgs e)
        {
            foreach (var dc in DocumentCollection.Where(x => x.IsButtonsVisible))
            {
                dc.IsButtonsVisible = false;
            }

            if (e.NewSelectedIndex >= 0)
                DocumentCollection[e.NewSelectedIndex].IsButtonsVisible = true;
        }

        private void ScheduleControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            //var nodes = ProfilTreeView.Nodes;
            //var dates = new List<DateTime> { DateTime.Today };

            //foreach (var node in nodes)
            //{
            //    var rows = DoctorSpr.OfType<DynamicBaseClass>().Where(x => (int?)x.GetValue("ParentID") == (int?)((DynamicBaseClass)node.Content).GetValue("ID"));
            //    ScheduleProcess(dates, rows, true);
            //}
        }
    }
}