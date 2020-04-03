using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.Hospital
{
    /// <summary>
    /// Логика взаимодействия для HospitalEmrUslugiPanel.xaml
    /// </summary>
    public partial class HospitalEmrUslugiPanel : UserControl
    {
        private USL _usl;
        private List<USL_ASSIST> _assists;
        public HospitalEmrUslugiPanel(USL usl)
        {
            InitializeComponent();

            _usl = usl;

            if (_usl.ID != 0)
                _assists = Reader2List.CustomSelect<USL_ASSIST> ($"SELECT * FROM USL_ASSIST WHERE UID = {_usl.ID}", SprClass.LocalConnectionString);
            else
            {
                _assists = new List<USL_ASSIST>();
            }

            GridUsl.DataContext = _usl;
            AssistGridControl.DataContext = _assists;

            OtdelBox.DataContext = SprClass.OtdelDbs;
            ProfilBox.DataContext = SprClass.profile;
            DetBox.DataContext = SprClass.detProf;
            DoctorBox.DataContext = SprClass.DoctList;
            SpecBox.DataContext = SprClass.SpecV021List;
            DsBox.DataContext = SprClass.mkbSearching;
            UslOslBox.DataContext = SprClass.OslList;
            AnestBox.DataContext = SprClass.AnestList;
            AssistColumnEdit.DataContext = SprClass.DoctList;
            VidVmeBox.DataContext = SprClass.SprUsl804;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {

            ((DXWindow)this.Parent).Close();
        }

        private void DateInBox_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            _usl.DATE_OUT = _usl.DATE_IN;
        }

        private void mkbBox_GotFocus(object sender, RoutedEventArgs e)
        {
            InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("en-US");
        }

        private void mkbBox_LostFocus(object sender, RoutedEventArgs e)
        {
            InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("ru-RU");
        }

        private void AssistAddItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            _assists.Add(new USL_ASSIST());
            AssistGridControl.RefreshData();
        }

        private void AssistDelItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var del = (USL_ASSIST) AssistGridControl.SelectedItem;

            if (del.ID == 0)
            {
                _assists.Remove(del);
            }
            else
            {
                Reader2List.CustomExecuteQuery($"DELETE USL_ASSIST WHERE ID = {del.ID}", SprClass.LocalConnectionString);
                _assists.Remove(del);
            }
            AssistGridControl.RefreshData();
        }

        private void HospitalEmrUslugiPanel_OnUnloaded(object sender, RoutedEventArgs e)
        {
            foreach (var assist in _assists)
            {
                _usl.USL_ASSIST.Add(assist);
            }
        }
    }
}
