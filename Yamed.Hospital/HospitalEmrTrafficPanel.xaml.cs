using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed.Hospital
{
    /// <summary>
    /// Логика взаимодействия для HospitalEmrTrafficPanel.xaml
    /// </summary>
    public partial class HospitalEmrTrafficPanel : UserControl
    {
        private USL _usl;
        public HospitalEmrTrafficPanel(USL usl)
        {
            InitializeComponent();
            _usl = usl;
            GridUsl.DataContext = _usl;

            PodrBox.DataContext = SprClass.Podr;
            OtdelBox.DataContext = SprClass.OtdelDbs;
            ProfilBox.DataContext = SprClass.profile;
            DetBox.DataContext = SprClass.detProf;
            DoctorBox.DataContext = SprClass.DoctList;
            SpecBox.DataContext = SprClass.SpecV021List;
            DsBox.DataContext = SprClass.mkbSearching;
            ProfilKBox.DataContext = SprClass.Profil_V020;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ((DXWindow)this.Parent).Close();
        }

        private void mkbBox_GotFocus(object sender, RoutedEventArgs e)
        {
            InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("en-US");
        }

        private void mkbBox_LostFocus(object sender, RoutedEventArgs e)
        {
            InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("ru-RU");
        }
    }
}
