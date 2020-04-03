using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Yamed.Core;
using Yamed.Registry.Models;

namespace Yamed.Registry.ViewModels
{
    public class ScheduleViewModel : ViewModelBase
    {
        public string DoctorInfo { get; set; }
        public string DateInfo { get; set; }

        public ScheduleViewModel(IEnumerable<BookingModel> visits, IEnumerable<dynamic> baseSchedule)
        {
            var schedule = baseSchedule.ToList();
            DoctorInfo =
                schedule.Select(x => (string) ObjHelper.GetAnonymousValue(x, "FIO"))
                    .Distinct()
                    .FirstOrDefault();

            DateTime _dateinfo = schedule.Select(x => (DateTime)ObjHelper.GetAnonymousValue(x, "BeginTime")).Distinct().FirstOrDefault();
            DateInfo = _dateinfo.ToString("dd.MM.yyyy");
            var times = (from h in schedule
                        select new
                        {
                            Id = (int)ObjHelper.GetAnonymousValue(h, "ID"),
                            Reserve = (bool)ObjHelper.GetAnonymousValue(h, "Reserve"),
                            PacientName = (string) ObjHelper.GetAnonymousValue(h, "PacientName"),
                            PacientContact = (string)ObjHelper.GetAnonymousValue(h, "PacientContact"),
                            PacientComent = (string)ObjHelper.GetAnonymousValue(h, "PacientComent"),
                            Hour = ((DateTime)ObjHelper.GetAnonymousValue(h, "BeginTime")).Hour,
                            Minute = ((DateTime)ObjHelper.GetAnonymousValue(h, "BeginTime")).Minute
                        }).ToList();

            AllBookings =  (from t in times
                            join v in visits on new { t.Hour, t.Minute } equals new { v.StartTime.Hour, v.StartTime.Minute } into j
                            from item in j.DefaultIfEmpty(null)
                            select new BookingViewModel(t.Id, t.Reserve, t.PacientName, t.PacientContact, t.PacientComent, t.Hour, t.Minute, item?.Name))
                            .ToList();

            // 9 hours starting from 8
            //int hourCount = times.Select(x => x.Hour).Distinct().Count();
            //int startHour = times.Select(x => x.Hour).Distinct().Min();
            //HourBookings = (from h in Enumerable.Range(startHour, hourCount)
            //                select new HourBookingViewModel(h, AllBookings)).ToList();

            HourBookings = (from h in times.Select(x => x.Hour).Distinct()select new HourBookingViewModel(h, AllBookings)).ToList();

            //create commands
            CollapseAll = new RelayCommand(
                () => HourBookings.ForEach(hb => hb.IsExpanded = "False"),
                () => HourBookings.Any(hb => hb.IsExpanded == "True")
            );
            ExpandAll = new RelayCommand(
                () => HourBookings.ForEach(hb => hb.IsExpanded = "True"),
                () => HourBookings.Any(hb => hb.IsExpanded == "False")
            );
        }

        private List<BookingViewModel> AllBookings { get; set; }

        public List<HourBookingViewModel> HourBookings { get; set; }

        public ICommand CollapseAll { get; set; }

        public ICommand ExpandAll { get; set; }
    }
}
