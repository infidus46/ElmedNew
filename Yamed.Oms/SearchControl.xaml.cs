using System;
using System.Windows.Controls;
using DevExpress.Xpf.Bars;
using Yamed.Control;
using Yamed.Server;
using System.Linq;
using Yamed.Core;

namespace Yamed.Oms
{
    /// <summary>
    /// Логика взаимодействия для SearchControl.xaml
    /// </summary>
    public partial class SearchControl : UserControl
    {
        public SearchControl()
        {
            InitializeComponent();

            LpuComboBoxEdit.DataContext = SprClass.LpuList;
            ProfilComboBoxEdit.DataContext = SprClass.profile;
            PCelEdit.DataContext = SprClass.SprPCelList;
            UslOkEdit.DataContext = SprClass.conditionHelp;
            OsSluchEdit.DataContext = SprClass.OsobSluchDbs;
            TypeSchetEdit.DataContext = SprClass.SchetType;
            DsComboBoxEdit.DataContext = SprClass.mkbSearching.Where(x => x.ISDELETE == false).ToList();

            var years = new object[] {2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024, 2025};
            var months = new object[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12};
            StartYearComboBoxEdit.Items.AddRange(years);
            EndYearComboBoxEdit.Items.AddRange(years);
            StartMonthComboBoxEdit.Items.AddRange(months);
            EndMonthComboBoxEdit.Items.AddRange(months);
        }

        private void SearchItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var rc = new SchetRegisterControl();
                if (TabItem1.IsSelected)
                {
                    rc.SchetRegisterGrid1.BindDataSearch((string)LpuComboBoxEdit.EditValue, (int?)StartMonthComboBoxEdit.EditValue, (int?)EndMonthComboBoxEdit.EditValue,
                        (int?)StartYearComboBoxEdit.EditValue, (int?)EndYearComboBoxEdit.EditValue, (int?)ProfilComboBoxEdit.EditValue, (string)DsComboBoxEdit.EditValue,
                        (string)PCelEdit.EditValue, (int?)UslOkEdit.EditValue, (int?)OsSluchEdit.EditValue, (string)TypeSchetEdit.EditValue);
                }

                if (TabItem2.IsSelected)
                {
                    rc.SchetRegisterGrid1.BindDataPacient((string)FamBoxEdit.EditValue, (string)ImBoxEdit.EditValue, (string)OtBoxEdit.EditValue, (DateTime?)DrBoxEdit.EditValue, (string)PolisBoxEdit.EditValue);
                }

                СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
                {
                    Header = "Реестр счета",
                    MyControl = rc,
                    IsCloseable = "True",
                    //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
                });
        }
    }
}
