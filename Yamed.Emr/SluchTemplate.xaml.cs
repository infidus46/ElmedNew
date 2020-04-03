using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Yamed.Core;
using Yamed.Server;

namespace Yamed.Emr
{
    /// <summary>
    /// Логика взаимодействия для SluchTemplate.xaml
    /// </summary>
    public partial class SluchTemplate : UserControl
    {
        private DynamicBaseClass _zsl;

        public SluchTemplate()
        {
            InitializeComponent();

            GetSpr();
        }

        public void BindSluch(int slid)
        {
            Task.Factory.StartNew(() =>
            {
                return SqlReader.Select2($"Select * from SLUCH where ID = {slid}",
                    SprClass.LocalConnectionString)[0];
            }).ContinueWith((x) =>
            {
                _zsl = x.Result;
                SluchGrid.DataContext = _zsl;
                x.Dispose();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private Task _task;
        public void BindEmptySluch(int pid)
        {
            _task = Task.Factory.StartNew(() =>
            {
                return SqlReader.Select2($"Select * from SLUCH where ID = 0",
                    SprClass.LocalConnectionString);
            }).ContinueWith((x) =>
            {
                _zsl = (DynamicBaseClass) Activator.CreateInstance(x.Result.GetDynamicType());
                _zsl.SetValue("PID", pid);
                _zsl.SetValue("IDSLG", Guid.NewGuid());
                SluchGrid.DataContext = _zsl;
                x.Dispose();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void DoctDataFill(DynamicBaseClass data)
        {
            _task.ContinueWith(x =>
            {
                _zsl.SetValue("LPU", data.GetValue("LPU_ID"));
                _zsl.SetValue("USL_OK", data.GetValue("USL_OK_ID"));
                _zsl.SetValue("LPU_1", data.GetValue("PODR_ID"));
                _zsl.SetValue("PORD", data.GetValue("OTD_ID"));
                _zsl.SetValue("MSPID", data.GetValue("PRVS_ID"));
                _zsl.SetValue("PROFIL", data.GetValue("PROFIL_ID"));
                _zsl.SetValue("DET", Convert.ToByte(data.GetValue("DET_ID")));
                _zsl.SetValue("VIDPOM", data.GetValue("VID_POM_ID"));
                _zsl.SetValue("IDDOKT", data.GetValue("SNILS"));
                _zsl.SetValue("IDDOKTO", data.GetValue("DID"));
            });
        }

        void GetSpr()
        {
            DoctGrid.DataContext = SprClass.MedicalEmployeeList;
            RsltGrid.DataContext = SprClass.helpResult;
            IshodGrid.DataContext = SprClass.helpExit;
            VidPomGrid.DataContext = SprClass.typeHelp;
            UslOkGrid.DataContext = SprClass.conditionHelp;
            ProfilGrid.DataContext = SprClass.profile;
            PrvsGrid.DataContext = SprClass.specialityNew;
            SluchTypeGrid.DataContext = SprClass.typeSluch;
            PCelTypeGrid.DataContext = SprClass.SprPCelList;
            DetGrid.DataContext = SprClass.detProf;
            IdspGrid.DataContext = SprClass.tarifUsl;
            Ds1Edit.DataContext = SprClass.mkbSearching.Where(x => x.ISDELETE == false).ToList();
            //ds.DataContext = SprClass.mkbSearching.Where(x => x.ISDELETE == false).ToList();
            //ds2Box.DataContext = SprClass.mkbSearching.Where(x => x.ISDELETE == false).ToList();
            SluchOsRegionGrid.DataContext = SprClass.OsobSluchDbs;
            NaprMoGrid.DataContext = SprClass.medOrg;
            NaprGrid.DataContext = SprClass.ExtrDbs;
            OtdelGrid.DataContext = SprClass.OtdelDbs;
            PodrGrid.DataContext = SprClass.Podr;
            ForPomGrid.DataContext = SprClass.ForPomList;
        }

        private void mkbBox_GotFocus(object sender, RoutedEventArgs e)
        {
            InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("en-US");
        }

        private void mkbBox_LostFocus(object sender, RoutedEventArgs e)
        {
            InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("ru-RU");
        }

        public int? SaveSluch()
        {
            int? id = null;
            if ((int)_zsl.GetValue("ID") == 0)
            {
                id = Reader2List.ObjectInsertCommand("SLUCH", _zsl, "ID", SprClass.LocalConnectionString);
            }
            else
            {
                var upd = Reader2List.CustomUpdateCommand("SLUCH", _zsl, "ID");
                Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);
            }
            return id;
        }

        public void SaveSluchAsync()
        {
            Task.Factory.StartNew(() =>
            {
                SaveSluch();
            });
        }

        private void dxEdit_Validate(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {
            //if (sender is ComboBoxEdit)
            //{
            //    ComboBoxEdit ed = (sender as ComboBoxEdit);
            //    object item = ed.GetItemByKeyValue(e.Value);
            //    if (item == null)
            //    {
            //        e.ErrorContent = "Поле обязательно для заполнения";
            //        e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default;
            //        e.IsValid = false;
            //        e.Handled = true;
            //    }
            //}

            //if (sender is TextEdit)
            //{
            //    TextEdit ed = (sender as TextEdit);
            //    if (string.IsNullOrEmpty(ed.Text))
            //    {
            //        e.ErrorContent = "Поле обязательно для заполнения";
            //        e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default;
            //        e.IsValid = false;
            //        e.Handled = true;
            //    }
            //}

            //if (sender is DateEdit)
            //{
            //    DateEdit ed = (sender as DateEdit);
            //    if (ed.EditValue == null)
            //    {
            //        e.ErrorContent = "Поле обязательно для заполнения";
            //        e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default;
            //        e.IsValid = false;
            //        e.Handled = true;
            //    }
            //}
        }

        //PopupListBox lb;
        //private void combo_PopupOpened(object sender, RoutedEventArgs e)
        //{
        //    lb = (PopupListBox)LookUpEditHelper.GetVisualClient((ComboBoxEdit)sender).InnerEditor;
        //    ComboBoxEditItem item = (ComboBoxEditItem)lb.ItemContainerGenerator.ContainerFromIndex(0);
        //    if (item != null)
        //        item.IsSelected = true;
        //    lb.ItemContainerGenerator.ItemsChanged += new System.Windows.Controls.Primitives.ItemsChangedEventHandler(ItemContainerGenerator_ItemsChanged);
        //}
        //void ItemContainerGenerator_ItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        //{
        //    Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        ComboBoxEditItem item = (ComboBoxEditItem)lb.ItemContainerGenerator.ContainerFromIndex(0);
        //        if (item != null)
        //            item.IsSelected = true;
        //    }), System.Windows.Threading.DispatcherPriority.Render);
        //}
    }
}
