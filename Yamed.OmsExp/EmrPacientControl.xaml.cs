using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using DevExpress.XtraEditors.DXErrorProvider;
using Yamed.Core;
using Yamed.Server;

namespace Yamed.OmsExp
{
    class PacientData : ViewModelBase
    {
        public DynamicBaseClass Pacient { get; set; }
        //public List<object> Dost { get; set; }

        private List<object> _dost;

        public List<object> Dost
        {
            get { return _dost; }
            set
            {
                _dost = value;
                RaisePropertyChanged("Dost");
            }
        }

        public object Dostp { get; set; }

        public string TableName = "D3_PACIENT_OMS";

        public PacientData()
        {
            Pacient = (DynamicBaseClass)Activator.CreateInstance(SqlReader.Select2($"SELECT * FROM D3_PACIENT_OMS WHERE ID = 0", SprClass.LocalConnectionString).GetDynamicType());
            Pacient.SetValue("NOVOR", "0");

        }

        public PacientData(int pid)
        {
            Pacient = SqlReader.Select($"SELECT * FROM D3_PACIENT_OMS WHERE ID = {pid}", SprClass.LocalConnectionString).Single();
            //Dost = SqlReader.Select($"SELECT * FROM PACIENT_DOST WHERE PID = {pid}", SprClass.LocalConnectionString).ToArray();
            //Dostp = SqlReader.Select($"SELECT * FROM PACIENT_DOSTP WHERE PID = {pid}", SprClass.LocalConnectionString);

        }
    }


    /// <summary>
    /// Логика взаимодействия для ClinicEmrPacientControl.xaml
    /// </summary>
    public partial class EmrPacientControl : UserControl
    {
        private PacientData _pd;

        public EmrPacientControl()
        {
            InitializeComponent();
        }

        public void BindPacient(int pid)
        {
            Task.Factory.StartNew(() => _pd = new PacientData(pid)).ContinueWith(x => PacientGrid.DataContext = _pd.Pacient, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void BindEmptyPacient()
        {
            Task.Factory.StartNew(() => _pd = new PacientData()).ContinueWith(x => PacientGrid.DataContext = _pd.Pacient, TaskScheduler.FromCurrentSynchronizationContext());
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                new Action(delegate()
                {
                    polisBox.Focus();
                }));

        }
        private void EmrPacientControl_OnInitialized(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(SprInit), System.Windows.Threading.DispatcherPriority.Render);
        }

        void SprInit()
        {
            typeUdlBox.DataContext = SprClass.passport;
            //smoOkatoBox.DataContext = SprClass.smoOkato;
            okatoTerBox.DataContext = SprClass.smoOkato;
            okatoTerPribBox.DataContext = SprClass.smoOkato;

            wBox.DataContext = SprClass.sex;
            policyTypeBox.DataContext = SprClass.policyType;
            smoBox.DataContext = SprClass.smo;

            DostEdit.ItemsSource = SprClass.DostList;
        }


        private void Required_OnValidate(object sender, ValidationEventArgs e)
        {
            if (e.Value != null) return;

            e.IsValid = false;
            e.ErrorContent = "Обязательное поле для заполнения";
            e.ErrorType = ErrorType.Critical;
        }

        private void Information_OnValidate(object sender, ValidationEventArgs e)
        {
            if (e.Value != null) return;

            e.IsValid = false;
            e.ErrorContent = "Статистическое поле, заполняется для получения отчетности";
            e.ErrorType = ErrorType.Information;

        }

        //private void DXErrorProvider_GetErrorIcon(GetErrorIconEventArgs e)
        //{
        //    StreamResourceInfo sri = Application.GetResourceStream(new Uri("/Yamed.Icons;component/Icons/PacientCard.png"));
        //    var ico = System.Drawing.Image.FromStream(sri.Stream);

        //    if (e.ErrorType == ErrorType.User8)
        //    {
        //        e.ErrorIcon = ico;
        //    }
        //}

        private void Conditional_OnValidate(object sender, ValidationEventArgs e)
        {
            if (e.Value != null) return;

            if (((BaseEdit) sender).Name == "OtBox")
            {
                e.IsValid = false;
                e.ErrorContent = "Условное поле, при отсутствии ставится признак достоверности";
                e.ErrorType = ErrorType.Warning;
            }
            else
            {
                e.IsValid = false;
                e.ErrorContent = "Условное поле, заполняется при необходимости";
                e.ErrorType = ErrorType.Information;
            }
        }

        public void SavePacient()
        {
            if ((int)_pd.Pacient.GetValue("ID") == 0)
            {
                Reader2List.ObjectInsertCommand(_pd.TableName, _pd.Pacient, "ID", SprClass.LocalConnectionString);
            }
            else
            {
                var upd = Reader2List.CustomUpdateCommand(_pd.TableName, _pd.Pacient, "ID");
                Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SavePacient();
        }
    }
}
