using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Yamed.Server;

namespace Yamed.Emr
{
    public static class Converters
    {
        public static string ConvertSmoCod(string smoCod)
        {
            switch (smoCod)
            {
                case "46001":
                    return "71"; // "РОСМЕДСТРАХ-К";
                case "46002":
                    return "74"; // "РОСНО-МС";
                case "46003":
                    return "79"; // "ИНГОССТРАХ-М";
                case "46004":
                    return "73"; // "СПАССКИЕ ВОРОТА-М";
                default:
                    return "98";
            }
        }

        public static string BackConvertSmoCod(string smoCod)
        {
            switch (smoCod)
            {
                case "71":
                    return "46001"; // "РОСМЕДСТРАХ-К";
                case "74":
                    return "46002"; // "РОСНО-МС";
                case "79":
                    return "46003"; // "ИНГОССТРАХ-М";
                case "73":
                    return "46004"; // "СПАССКИЕ ВОРОТА-М";
                case "75":
                    return "46005";
                default:
                    return null;
            }
        }
    }

        public class NomenclatureNameConverter : IValueConverter
    {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value != null)
                    return SprClass.S1664Ns.Single(x => x.Id == (string) value).Name;
                return "НЕ ВЫБРАН КОД УСЛУГИ";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
    }

    public class MyBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return false;

            if (values[0] != null && values[1] != null && values[2] != null &&
                values[3] != null && values[4] != null && values[5] != null) return true;
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MyBool2NullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MyBoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if ((bool) value) return "Да";
            return "Нет";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ButtonVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[1] != null || ((DateTime)values[0]) < DateTime.Now) return Visibility.Hidden;
            else return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ButtonCaptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Резерв";
            return "Снять резерв";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class  ButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DocSerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null)
            {
                switch ((string)value)
                {
                    case "R-ББ":
                        return "[IVXLC1УХЛС]{1,3}-[А-Я]{2}";
                    //case "S1":
                    //case "S":   
                        return @"(\d|\w)?";
                    case "ББ":
                        return @"[А-Я]{2}";
                    case "99":
                        return @"\d{2}";
                    case "99 99":
                        return @"\d{2} \d{2}";   
                }
            }
            return @"(\d|\w)?";;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DocNumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                switch ((string)value)
                {
                    case "999999":
                        return @"\d{6}";
                    case "00000009":
                        return @"\d{1,8}";
                    case "9999999":
                        return @"\d{7}";
                    case "9999990":
                        return @"\d{6,7}";
                    case "0000000009":
                        return @"\d{1,10}";
                }
            }
            return @"(\d)?";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class StatusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == null && values[1] == null) return "Действующий";
            if (values[1] != null) return "Изъят";
            if (values[0] != null && (values[0] as Nullable<DateTime>) < DateTime.Now) return "Просрочен";
            else return "Действующий";
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }


    public class TypeOfPolicy : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((values[0] as Nullable<int>) == 3)
                return values[2];
            else return values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            object[] val = new object[2];
            return val;
        }
    }

    public class SMO : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                switch((string)value)
                {
                    case "46001": return "РОСМЕДСТРАХ-К";
                    case "46002": return "РОСНО-МС";
                    case "46003": return "ИНГОССТРАХ-М";
                    case "46004": return "СПАССКИЕ ВОРОТА-М";
                    case "46005": return "АСК-МЕД";
                    default: return "";
                }
            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if ((int)value == 1)
                    return "МУЖСКОЙ";
                if ((int)value == 2)
                    return "ЖЕНСКИЙ";
            }
            return "НЕ ПОНЯТНО";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MkbConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return value;
            string convert = value as string;
            if (convert.Length > 0)
            {

                return convert.Replace('й', 'q').Replace('ц', 'w').Replace('у', 'e').Replace('к', 'r').Replace('е',
                    't').Replace('н', 'y').Replace('г', 'u').Replace('ш', 'i').Replace('щ', 'o').Replace('з',
                    'p').Replace('ф', 'a').Replace('ы', 's').Replace('в', 'd').Replace('а', 'f').Replace('п', 'g').Replace('р',
                    'h').Replace('о', 'j').Replace('л', 'k').Replace('д', 'l').Replace('я', 'z').Replace('ч', 'x').Replace('с',
                    'c').Replace('м', 'v').Replace('и', 'b').Replace('т', 'n').Replace('ь', 'm').Replace(',', '.').ToUpper();


            }
            else return value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class FontConv : MarkupExtension, IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, System.Type targetType,
                    object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)value == 0 ? Brushes.Red : Brushes.Black;
        }

        public object ConvertBack(object value, System.Type targetType,
                    object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        public override object ProvideValue(System.IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class DiagnosColorConverter : MarkupExtension, IValueConverter
    {

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return new SolidColorBrush(Colors.DarkSlateGray);
            if ((bool)value == false)
                return new SolidColorBrush(Colors.Red);
            if ((bool)value)
                return new SolidColorBrush(Colors.Green);
            return new SolidColorBrush(Colors.LightGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class KsgColorConverter : MarkupExtension, IValueConverter
    {

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((int)value < 177)
                return new SolidColorBrush(Colors.Red);
            if ((int)value > 176)
                return new SolidColorBrush(Colors.Green);
            return new SolidColorBrush(Colors.LightGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class DiagnosShowConverter : MarkupExtension, IValueConverter
    {

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            return SprClass.mkbSearching.Single(x => x.IDDS == (string)value).NameWithID;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class UslugiListConverter : MarkupExtension, IValueConverter
    {

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null) return value;
            return "НОВАЯ УСЛУГА";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Hidden;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //public class VisibilityConverter2 : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value == null) return Visibility.Hidden;
    //        if ((int)value == 0) return Visibility.Hidden;
    //        return Visibility.Visible;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class CollectionToIsCheckedConverter : IMultiValueConverter
    //{
    //    private ViewModel dataContext;

    //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        ICollection collection = values[0] as ICollection;
    //        dataContext = values[1] as ViewModel;

    //        if (collection == null || dataContext == null)
    //            throw new NotSupportedException();

    //        if (collection.Count == 0)
    //            return false;

    //        if (collection.Count == dataContext.List.Count)
    //            return true;

    //        return null;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotSupportedException();
    //    }
    //}

    //public class CollectionToIsThreeStateConverter : IMultiValueConverter
    //{
    //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        ICollection collection = values[0] as ICollection;
    //        ViewModel dataContext = values[1] as ViewModel;

    //        if (collection == null || dataContext == null)
    //            throw new NotSupportedException();

    //        if (collection.Count == 0 || collection.Count == dataContext.List.Count)
    //            return false;

    //        return true;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotSupportedException();
    //    }
    //}

    //public class ViewModel : INotifyPropertyChanged
    //{
    //    public ObservableCollection<TestData> List { get; set; }
    //    public Dictionary<Guid, bool> SelectedValues { get; set; }

    //    public ViewModel()
    //    {
    //        SelectedValues = new Dictionary<Guid, bool>();
    //        List = new ObservableCollection<TestData>();

    //        GenerateData(20);
    //    }

    //    private void GenerateData(int objectCount)
    //    {
    //        for (int i = 0; i < objectCount; i++)
    //            List.Add(new TestData() { Id = Guid.NewGuid(), Number = i });
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;
    //    public virtual void RaisePropertyChanged(string propertyName)
    //    {
    //        if (PropertyChanged != null)
    //            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}


    //public class TestData
    //{
    //    public Guid Id { get; set; }
    //    public int Number { get; set; }
    //}


}
