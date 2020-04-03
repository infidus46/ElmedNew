using System.Windows;

namespace Yamed.Control
{
    /// <summary>
    /// Логика взаимодействия для ErrorGlobalWindow.xaml
    /// </summary>
    public partial class ErrorGlobalWindow : Window
    {
        public ErrorGlobalWindow()
        {
            InitializeComponent();
        }

        public static void ShowError(string error)
        {
            var window = new ErrorGlobalWindow {errorBox = {Text = error}};
            window.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
