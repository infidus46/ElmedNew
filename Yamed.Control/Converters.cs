//using System;
//using System.Globalization;
//using System.Windows;
//using System.Windows.Data;
//using System.Windows.Markup;
//using System.Windows.Media;

//namespace Elmedicine.Controls
//{
//    public class MyBoolConverter : IMultiValueConverter
//    {
//        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (values == null) return false;

//            if (values[0] != null && values[1] != null && values[2] != null &&
//                values[3] != null && values[4] != null && values[5] != null) return true;
//            return false;
//        }

//        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class MyBool2NullConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (value == null) return Visibility.Collapsed;
//            return Visibility.Visible;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class MyBoolToStringConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (value == null) return null;
//            if ((bool) value) return "Да";
//            return "Нет";
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }
//    public class DiagnosShowConverter : MarkupExtension, IValueConverter
//    {

//        public override object ProvideValue(IServiceProvider serviceProvider)
//        {
//            return this;
//        }

//        #region IValueConverter Members

//        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            if (value == null) return null;
//            return value;  //SprClass.mkbSearching.Single(x => x.IDDS == (string)value).NameWithID;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion
//    }
//}
