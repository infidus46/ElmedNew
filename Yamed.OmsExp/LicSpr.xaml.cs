using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Yamed.Core;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.OmsExp
{
    /// <summary>
    /// Логика взаимодействия для LicSpr.xaml
    /// </summary>
    public partial class LicSpr : UserControl
    {
        public LicSpr()
        {
            InitializeComponent();

            MOBox.DataContext = SprClass.LpuList;
        }

        private void MOBox_OnSelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            LicBox.SelectedIndex = -1;
            using (var dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
            {
                LicBox.DataContext = dc.LIC_NUM_TBL.Where(x => x.LPU == (string) MOBox.EditValue);
            }

            conditionHelpBox.SelectedIndex = -1;
            typeHelpBox.SelectedIndex = -1;
            LicListBoxEdit.SelectedItems.Clear();
            LicListBoxEdit.DataContext = null;
        }

        private void LicBox_OnSelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            conditionHelpBox.SelectedIndex = -1;
            typeHelpBox.SelectedIndex = -1;
            LicListBoxEdit.SelectedItems.Clear();
            LicListBoxEdit.DataContext = null;
            LicMOListBoxEdit.SelectedItems.Clear();
            LicMOListBoxEdit.DataContext = null;

            if (LicBox.SelectedItem != null)
            {
                string[] ss = ((LIC_NUM_TBL)LicBox.SelectedItem).VID_MP.Split(',');
                int[] ii = Array.ConvertAll(ss, Int32.Parse);
                typeHelpBox.DataContext = SprClass.typeHelp.Where(x => ii.Contains(x.Id));

            }

        }

        private void TypeHelpBox_OnSelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            conditionHelpBox.SelectedItem = -1;
            LicListBoxEdit.SelectedItems.Clear();
            LicListBoxEdit.DataContext = null;
            LicMOListBoxEdit.SelectedItems.Clear();
            LicMOListBoxEdit.DataContext = null;

            conditionHelpBox.DataContext = SprClass.conditionHelp;          
        }

        private void ConditionHelpBox_OnSelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (MOBox.EditValue != null && LicBox.EditValue != null && typeHelpBox.EditValue != null
                && conditionHelpBox.EditValue != null)
                LicLoad();
        }

        private void InsertBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (MOBox.EditValue != null && LicBox.EditValue != null && typeHelpBox.EditValue != null
                && conditionHelpBox.EditValue != null && LicListBoxEdit.SelectedItems.Any())
            {
                var licTbls = new List<LIC_TBL>();
                foreach (var item in LicListBoxEdit.SelectedItems.OfType<V002>())
                {
                    var licLpu = new LIC_TBL();
                    licLpu.LPU = (string) MOBox.EditValue;
                    licLpu.LIC_NUM_ID = (int) LicBox.EditValue;
                    licLpu.VID_MP = (int) typeHelpBox.EditValue;
                    licLpu.USL_MP = (int) conditionHelpBox.EditValue;
                    licLpu.PROFIL = item.Id;
                    licTbls.Add(licLpu);
                }
                using (SqlBulkCopy bulkCopy =
                    new SqlBulkCopy(
                        SprClass.LocalConnectionString,
                        SqlBulkCopyOptions.CheckConstraints))
                {
                    bulkCopy.BulkCopyTimeout = 0;
                    bulkCopy.DestinationTableName = "dbo.LIC_TBL";
                    bulkCopy.WriteToServer(licTbls.AsDataReader());
                }
                LicLoad();
            }
            else MessageBox.Show("Поля заполнены не в полном объеме!", "Ошибка");
        }

        void LicLoad()
        {
            var licLpu = new List<LIC_TBL>();
            using (var dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
            {
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<LIC_TBL>(x => x.V002);
                dc.CommandTimeout = 0;
                dc.LoadOptions = options;
                licLpu = dc.LIC_TBL.Where(x => x.LPU == (string)MOBox.EditValue && x.LIC_NUM_ID == (int)LicBox.EditValue
                   && x.VID_MP == (int)typeHelpBox.EditValue && x.USL_MP == (int)conditionHelpBox.EditValue).Select(x => x).ToList();
            }
            LicMOListBoxEdit.DataContext = licLpu;
            LicListBoxEdit.DataContext = SprClass.profile.Where(x => !licLpu.Select(l => l.PROFIL).Contains(x.Id)).Select(x => x);
        }

        private void DelButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (LicMOListBoxEdit.SelectedItem != null)
            {
                using (var dc = new ElmedDataClassesDataContext(SprClass.LocalConnectionString))
                {
                    dc.LIC_TBL.DeleteOnSubmit(dc.LIC_TBL.Single(x=>x.ID == ((LIC_TBL) LicMOListBoxEdit.SelectedItem).ID));
                    dc.SubmitChanges();
                }
                LicLoad();
            }
        }


    }
}
