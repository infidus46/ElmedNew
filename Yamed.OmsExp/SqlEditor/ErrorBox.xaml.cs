using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace Yamed.OmsExp.SqlEditor
{
    public partial class ErrorBox
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(ErrorBox), new PropertyMetadata(default(string)));

        public string Message
        {
            get { return (string) GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public ErrorBox()
        {
            InitializeComponent();

            Visibility = Visibility.Collapsed;

            var property = DependencyPropertyDescriptor.FromProperty(MessageProperty, typeof(ErrorBox));
            property.AddValueChanged(this, MessagePropertyChanged);
        }

        private void MessagePropertyChanged(object sender, EventArgs e)
        {
            TextBlockErrorPrompt.Text = Message;

            Visibility = string.IsNullOrEmpty(Message) ? Visibility.Collapsed : Visibility.Visible;

            if(string.IsNullOrEmpty(Message)) return;

            var timer = new Timer(CallBackPopup, null, 3000, Timeout.Infinite);
        }

        private void CallBackPopup(object state)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                if (GridError.Visibility == Visibility.Collapsed) return;

                TextBlockErrorPrompt.Text = string.Empty;
                Visibility = Visibility.Collapsed;
            });
        }
    }
}
