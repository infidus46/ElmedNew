using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using Yamed.Server;

namespace Yamed.OmsExp.ExpEditors
{
    /// <summary>
    /// Логика взаимодействия для AktMeeChoose.xaml
    /// </summary>
    public partial class AktMeeChoose : UserControl
    {
        private int[] _slids;

        public AktMeeChoose(int[] slids)
        {
            InitializeComponent();

            _slids = slids;
            MeeTypEdit.DataContext = SprClass.MeeTypeDbs;
            MeeTypEdit.SelectedItem = SprClass.MeeTypeDbs.Single(x => x.Id == 9);
            ZaprosDateEdit.EditValue = SprClass.WorkDate;
            //Bar1.Visible = false;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            ((DXWindow) this.Parent).Close();
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            StringBuilder cmd = new StringBuilder();

            foreach (int slid in _slids)
            {
                cmd.AppendLine(string.Format(@"UPDATE D3_ZSL_OMS SET EXP_COMENT = '{0}', EXP_TYPE = {1}, EXP_DATE = '{2}', USERID = {3} WHERE ID = {4}",
                    ComentEdit.EditValue, MeeTypEdit.EditValue, SprClass.WorkDate.ToString("yyyyMMdd"), SprClass.userId, slid)); // 
            }
            Reader2List.CustomExecuteQuery(cmd.ToString(), SprClass.LocalConnectionString);

            ((DXWindow)this.Parent).Close();

        }
    }
}
