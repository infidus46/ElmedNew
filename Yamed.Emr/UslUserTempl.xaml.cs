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
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.Emr
{
    /// <summary>
    /// Логика взаимодействия для UslUserTempl.xaml
    /// </summary>
    public partial class UslUserTempl : UserControl
    {
        private SluchTemplateD31 _sluchTemplateD31;
        private SluchTemplateD31Ivanovo _sluchTemplateD31Iv;
        public UslUserTempl(SluchTemplateD31 sluchTemplateD31)
        {
            InitializeComponent();

            _sluchTemplateD31 = sluchTemplateD31;

            Task.Factory.StartNew(() =>
            {
                return Reader2List.GetAnonymousTable("Yamed_Spr_UslCategory", SprClass.LocalConnectionString);
            }).ContinueWith(x =>
            {
                UslCategoryEdit.DataContext = x.Result;
            }, TaskScheduler.FromCurrentSynchronizationContext());


        }
        public UslUserTempl(SluchTemplateD31Ivanovo sluchTemplateD31)
        {
            InitializeComponent();

            _sluchTemplateD31Iv = sluchTemplateD31;

            Task.Factory.StartNew(() =>
            {
                return Reader2List.GetAnonymousTable("Yamed_Spr_UslCategory", SprClass.LocalConnectionString);
            }).ContinueWith(x =>
            {
                UslCategoryEdit.DataContext = x.Result;
            }, TaskScheduler.FromCurrentSynchronizationContext());


        }
        public DateTime? datez;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            if (SprClass.Region != "37")
            {
                if (_sluchTemplateD31._uslList == null)
                {
                    _sluchTemplateD31.UslGrid.DataContext = _sluchTemplateD31._uslList = new List<D3_USL_OMS>();
                }
                var lpu = _sluchTemplateD31._zsl.LPU;
                var slgid = ((D3_SL_OMS)_sluchTemplateD31.SlGridControl.SelectedItem).SL_ID;
                
                var ulist = UslUserTemplEdit.SelectedItems;
                foreach (DynamicBaseClass usl in ulist)
                {
                    _sluchTemplateD31._uslList.Add(new D3_USL_OMS
                    {
                        COMENTU = (string)usl.GetValue("Name"),
                        CODE_USL = (string)usl.GetValue("Code"),
                        VID_VME = (string)usl.GetValue("VM_Code"),
                        KOL_USL = (decimal?)usl.GetValue("Kol"),
                        TARIF = (decimal?)usl.GetValue("Tarif"),
                        PROFIL = (int?)usl.GetValue("Profil"),
                        DET = (int?)usl.GetValue("Det"),
                        PRVS = (int?)usl.GetValue("Spec"),
                        LPU = lpu,
                        D3_SLGID = slgid
                    });
                }
            ((DXWindow)this.Parent).Close();
            }
            else if (SprClass.Region == "37")
            {
                if (_sluchTemplateD31Iv._uslList == null)
                {
                    _sluchTemplateD31Iv.UslGrid.DataContext = _sluchTemplateD31Iv._uslList = new List<D3_USL_OMS>();
                }

                var lpu = _sluchTemplateD31Iv._zsl.LPU;
                var slgid = ((D3_SL_OMS)_sluchTemplateD31Iv.SlGridControl.SelectedItem).SL_ID;

                var ulist = UslUserTemplEdit.SelectedItems;
                foreach (DynamicBaseClass usl in ulist)
                {
                    _sluchTemplateD31Iv._uslList.Add(new D3_USL_OMS
                    {
                        COMENTU = (string)usl.GetValue("Name"),
                        CODE_USL = (string)usl.GetValue("Code"),
                        VID_VME = (string)usl.GetValue("VM_Code"),
                        KOL_USL = (decimal?)usl.GetValue("Kol"),
                        TARIF = (decimal?)usl.GetValue("Tarif"),
                        PROFIL = (int?)usl.GetValue("Profil"),
                        DET = (int?)usl.GetValue("Det"),
                        PRVS = (int?)usl.GetValue("Spec"),
                        LPU = lpu,
                        D3_SLGID = slgid
                    });
                }

            ((DXWindow)this.Parent).Close();
            }
        }

        private void UslCategoryEdit_OnSelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            datez = _sluchTemplateD31._zsl.DATE_Z_2;
            var cid = ObjHelper.GetAnonymousValue(UslCategoryEdit.SelectedItem, "ID");
            Task.Factory.StartNew(() =>
            {
                return SqlReader.Select2($"Select * From Yamed_Spr_UslTemplate where CategoryID = {cid} and isnull('{datez}','21000101') between date1 and date2", SprClass.LocalConnectionString);
            }).ContinueWith(x =>
            {
                UslUserTemplEdit.DataContext = x.Result;
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }
    }
}
