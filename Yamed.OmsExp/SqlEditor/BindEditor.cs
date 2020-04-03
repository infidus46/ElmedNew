using System;
using System.ComponentModel;
using System.Windows;
using ActiveQueryBuilder.View.WPF.ExpressionEditor;
using ICSharpCode.AvalonEdit;

namespace Yamed.OmsExp.SqlEditor
{
    public class BindableTextEditor : TextEditor, INotifyPropertyChanged
    {

        /// <summary>
        /// A bindable Text property
        /// </summary>
        public new string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        internal string baseText { get { return base.Text; } set { base.Text = value; } }

        /// <summary>
        /// The bindable text property dependency property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(BindableTextEditor), new PropertyMetadata((obj, args) =>
            {
                BindableTextEditor target = (BindableTextEditor)obj;
                if (target.baseText != (string)args.NewValue)    //avoid undo stack overflow
                    target.baseText = (string)args.NewValue;
            }));

        protected override void OnTextChanged(EventArgs e)
        {
            SetCurrentValue(TextProperty, baseText);
            RaisePropertyChanged("Text");
            base.OnTextChanged(e);
        }

        /// <summary>
        /// Raises a property changed event
        /// </summary>
        /// <param name="property">The name of the property that updates</param>
        public void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
