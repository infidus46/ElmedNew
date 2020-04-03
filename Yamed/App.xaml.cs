using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using Yamed.Control;

namespace Yamed
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            string error = e.Exception.Message;
            if (e.Exception.InnerException != null)
            {
                error += Environment.NewLine;
                error += e.Exception.InnerException.Message;
            }
            ErrorGlobalWindow.ShowError(error);
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            //ApplicationThemeHelper.ApplicationThemeName = Theme.DeepBlueName;

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement), new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(
                    System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)));
        }
    }
}
