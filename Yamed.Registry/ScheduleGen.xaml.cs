using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using DevExpress.Xpf.Core.ServerMode;
using Yamed.Core;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.Registry
{

    /// <summary>
    /// Логика взаимодействия для ScheduleGen.xaml
    /// </summary>
    public partial class ScheduleGen : UserControl
    {
        private readonly object _data;
        private readonly DynamicCollection<Core.DynamicBaseClass> _model;

        public ScheduleGen()
        {
            InitializeComponent();


            _data = Reader2List.CustomAnonymousSelect(@"Select * from Yamed_Spr_MedicalEmployee where IsInSchedule=1", SprClass.LocalConnectionString);
            _model = SqlReader.Select2(@"Select * from Yamed_Spr_RegModel", SprClass.LocalConnectionString);

            DoctorComboBoxEdit.DataContext = _data;
            ModelEdit.DataContext = _model;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Date1Edit.EditValue == null || Date2Edit.EditValue == null)
            {
                DXMessageBox.Show("Не выбраны даты (дата)", "Внимание");
                return;
            }

            if (DoctorComboBoxEdit.SelectedIndex < 0)
            {
                DXMessageBox.Show("Не выбран сотрудник", "Внимание");
                return;
            }

            if (Time1Edit.EditValue == null || Time2Edit.EditValue == null || Time3Edit.EditValue == null)
            {
                DXMessageBox.Show("Не заполнены интервалы", "Внимание");
                return;
            }

            if (Convert.ToInt32(((string)Time1Edit.EditValue).Replace(":", "")) >= Convert.ToInt32(((string)Time2Edit.EditValue).Replace(":", "")))
            {
                DXMessageBox.Show("Не верно задан интервал", "Внимание");
                return;
            }
            
            var d1 = (DateTime)Date1Edit.EditValue;
            var d2 = (DateTime)Date2Edit.EditValue;
            int si = 0;

            while (d1 <= d2)
            {
                var bhm = ((string)Time1Edit.EditValue).Split(':');
                int beginHour = Convert.ToInt32(bhm[0]);
                int beginMinut = Convert.ToInt32(bhm[1]);

                var ehm = ((string)Time2Edit.EditValue).Split(':');
                int endHour = Convert.ToInt32(ehm[0]);
                int endMinut = Convert.ToInt32(ehm[1]);

                int num = ((endHour * 60) + endMinut) - ((beginHour * 60) + beginMinut);
                int interval = Convert.ToInt32(Time3Edit.EditValue);
                int kol = num / interval;

                DateTime time = d1.Date.AddHours(beginHour).AddMinutes(beginMinut);
                DateTime time2 = time.AddMinutes(interval);
                var intervalList = new List<YamedRegistry>();


                var doctId = (int)DoctorComboBoxEdit.EditValue;
                for (int i = 0; i < kol; i++)
                {
                    if (d1.Date.AddHours(endHour).AddMinutes(endMinut) == time) continue;
                    var scheduleRecord = new YamedRegistry();
                    scheduleRecord.BeginTime = time;
                    scheduleRecord.DID = doctId;
                    time = time2;
                    time2 = time2.AddMinutes(interval);
                    intervalList.Add(scheduleRecord);
                }

                var cnt =
                    Reader2List.CustomAnonymousSelect($@"
                            Select count(*) cnt from YamedRegistry
                            where cast(BeginTime as DATE) = '{d1:yyyyMMdd}' and DID = {doctId}",
                        SprClass.LocalConnectionString);
                if ((int)ObjHelper.GetAnonymousValue(((IList)cnt)[0], "cnt") == 0)
                {
                    Reader2List.CustomInsertCommand("YamedRegistry", intervalList, "ID", SprClass.LocalConnectionString);
                    si++;
                }


                d1 = d1.AddDays(1);
            }
            DXMessageBox.Show($"Расписание создано успешно на {si} дат");
        }

        private void ScheduleGen_Closed(object sender, EventArgs e)
        {
            ((IList) _data).Clear();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ((DXWindow)Parent).Closed += ScheduleGen_Closed;
        }

        private void CreateModelButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ModelEdit.EditValue == null || DoctorComboBoxEdit.EditValue == null) return;
            var mid = ModelEdit.EditValue;
            var modelCollection = SqlReader.Select2($"Select * from Yamed_RegModel where modelid = {mid}",
                SprClass.LocalConnectionString);

            var d1 = (DateTime)Date1Edit.EditValue;
            var d2 = (DateTime)Date2Edit.EditValue;
            int si = 0;

            while (d1 <= d2)
            {
                foreach (var model in modelCollection)
                {
                    DayOfWeek dow = GetWeekDay((int)model.GetValue("WeekDayID"));
                    if (dow != d1.DayOfWeek) continue;

                    var beginTime = (TimeSpan)model.GetValue("BeginTime");
                    var endTime = (TimeSpan)model.GetValue("EndTime");
                    var interval = (int)model.GetValue("Interval");

                    int beginHour = beginTime.Hours;
                    int beginMinut = beginTime.Minutes;

                    int endHour = endTime.Hours;
                    int endMinut = endTime.Minutes;

                    int num = ((endHour * 60) + endMinut) - ((beginHour * 60) + beginMinut);
                    int kol = num / interval;

                    DateTime time = d1.Date.AddHours(beginHour).AddMinutes(beginMinut);
                    DateTime time2 = time.AddMinutes(interval);
                    var intervalList = new List<YamedRegistry>();


                    var doctId = (int)DoctorComboBoxEdit.EditValue;
                    for (int i = 0; i < kol; i++)
                    {
                        if (d1.Date.AddHours(endHour).AddMinutes(endMinut) == time) continue;
                        var scheduleRecord = new YamedRegistry();
                        scheduleRecord.BeginTime = time;
                        scheduleRecord.DID = doctId;
                        time = time2;
                        time2 = time2.AddMinutes(interval);
                        intervalList.Add(scheduleRecord);
                    }

                    var cnt =
                        Reader2List.CustomAnonymousSelect($@"
                            Select count(*) cnt from YamedRegistry
                            where cast(BeginTime as DATE) = '{d1:yyyyMMdd}' and DID = {doctId}",
                            SprClass.LocalConnectionString);
                    if ((int)ObjHelper.GetAnonymousValue(((IList)cnt)[0], "cnt") == 0)
                    {
                        Reader2List.CustomInsertCommand("YamedRegistry", intervalList, "ID",
                            SprClass.LocalConnectionString);
                        si++;
                    }
                }
                d1 = d1.AddDays(1);
            }
            DXMessageBox.Show($"Расписание создано успешно на {si} дат");
        }

        private DayOfWeek GetWeekDay(int day)
        {
            switch (day)
            {
                case 1: return DayOfWeek.Monday;
                case 2: return  DayOfWeek.Tuesday;
                case 3: return DayOfWeek.Wednesday;
                case 4: return DayOfWeek.Thursday;
                case 5: return DayOfWeek.Friday;
                case 6: return DayOfWeek.Saturday;
                case 7: return DayOfWeek.Sunday;
                default:
                    return 0;
            }
        }


    }
}
