using System.Linq;
using System.Windows;
using System.Windows.Input;
using Yamed.Entity;
using Yamed.Server;

namespace Yamed
{
    /// <summary>
    /// Логика взаимодействия для Password.xaml
    /// </summary>
    public partial class PasswordWindow : DevExpress.Xpf.Core.DXWindow
    {
        public PasswordWindow()
        {
            InitializeComponent();

            userNameComboBox.DataContext = Reader2List.CustomSelect<Yamed_Users>(@"
SELECT [id]
      ,[UserName]
      ,[Password]
      ,null [LayRTable]
      ,[CODE]
      ,[FAM]
      ,[IM]
      ,[OT]
      ,[DOLJ]
  FROM [dbo].[Yamed_Users]", SprClass.LocalConnectionString);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var user = userNameComboBox.SelectedItem as Yamed_Users;
            if (user != null && user.Password == passwordTextBox.Password)
            {
                    SprClass.userId = user.ID;
                    SprClass.UserFIO = user.FAM + " " + user.IM.Substring(0, 1) + ". " + user.OT.Substring(0, 1) + ".";
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                label.Text = "Неверный пароль";
            }
        }

        private void passwordTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                button1_Click(sender, e);
            }
        }
    }
}
