using System;
using Yamed.Core;

namespace Yamed.Registry.ViewModels
{
    public class BookingViewModel:ViewModelBase
    {
        public BookingViewModel(int id, bool reserve, string pacientName, string pacientContact, string pacientComent, int hour, int minute)
        {
            this.Id = id;
            this.IsUnavailable = reserve;
            this.PacientName = pacientName;
            this.PacientContact = pacientContact;
            this.PacientComent = pacientComent;
            this.StartTime = DateTime.Today.AddHours(hour).AddMinutes(minute);
            this.PatientFullName = null;
        }

        public BookingViewModel(int id, bool reserve, string pacientName, string pacientContact, string pacientComent, int hour, int minute, string patientName)
            : this(id, reserve, pacientName, pacientContact, pacientComent, hour, minute)
        {
            this.PatientFullName = patientName;
        }

        public int Id { get; private set; }

        public bool IsUnavailable { get; private set; }

        public string PacientName { get; private set; }

        public string PacientContact { get; private set; }

        public string PacientComent { get; private set; }

        public DateTime StartTime { get; private set; }

        public string PatientFullName { get; private set; }

        public bool IsPatientExist
        {
            get { return PatientFullName != null; }
        }


        public void Reserve(bool isReserve)
        {
            this.IsUnavailable = isReserve;
            this.RaisePropertyChanged("IsUnavailable");
        }

        public void ReserveName(string pacientName, string pacientContact, string pacientComent)
        {
            this.PacientName = pacientName;
            this.RaisePropertyChanged("PacientName");

            this.PacientContact = pacientContact;
            this.RaisePropertyChanged("PacientContact");

            this.PacientComent = pacientComent;
            this.RaisePropertyChanged("PacientComent");
        }

        public void SetPatient(string name)
        {
            this.PatientFullName = name;
            this.RaisePropertyChanged("PatientFullName");
            this.RaisePropertyChanged("IsPatientExist");
        }

        public void RemovePatient()
        {
            this.PatientFullName = null;
            this.RaisePropertyChanged("PatientFullName");
            this.RaisePropertyChanged("IsPatientExist");
        }
    }
}
