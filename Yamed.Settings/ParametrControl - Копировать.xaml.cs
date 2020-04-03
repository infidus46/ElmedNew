using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Yamed.Control;
using Yamed.Core;
using Yamed.Server;

namespace Yamed.Settings
{
    /// <summary>
    /// Логика взаимодействия для ParametrControl.xaml
    /// </summary>
    public partial class ParametrControl : UserControl
    {
        private readonly int[] _sc;
        private static object _row;

        public ParametrControl(int[] sc, object row)
        {
            InitializeComponent();

            _sc = sc;
            _row = row;

            DateSankGenerate();
        }

        private void DateSankGenerate()
        {
            int st = 0;
            var type = (int)ObjHelper.GetAnonymousValue(_row, "RepType");
            switch (type)
            {
                case 101:
                    st = 1;
                    break;
                case 102:
                    st = 2;
                    break;
                case 103:
                    st = 3;
                    break;
            }



            var dates = Reader2List.CustomAnonymousSelect($@"
Select S_DATE, convert(nvarchar(10), S_DATE, 104) S_DATE_RUS FROM D3_SANK_OMS
WHERE D3_SCID in ({GetIds(_sc)}) and S_TIP = {st}
group by S_DATE
ORDER BY S_DATE", SprClass.LocalConnectionString);

            DateListBoxEdit.DataContext = dates;
            DateListBoxEdit.SelectedIndex = ((IList)dates).Count - 1;
        }

        string GetStringOfDates(ObservableCollection<object> collection)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in collection.Select(x => ObjHelper.GetAnonymousValue(x, "S_DATE")))
            {
                //sb.Append("'");
                sb.Append(((DateTime) item).ToString("yyyyMMdd"));
                //sb.Append("'");
                sb.Append(",");
            }

            var dates = sb.ToString();
            dates = dates.Remove(dates.Length - 1);
            return dates;
        }

        string GetIds(int[] collection)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in collection)
            {
                //sb.Append("'");
                sb.Append(item.ToString());
                //sb.Append("'");
                sb.Append(",");
            }

            var ids = sb.ToString();
            ids = ids.Remove(ids.Length - 1);
            return ids;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var rl = (string)ObjHelper.GetAnonymousValue(_row, "Template");

            var dates = DateListBoxEdit.SelectedItems.Any() ? GetStringOfDates(DateListBoxEdit.SelectedItems) : null;

            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Отчет",
                MyControl = new PreviewControl(rl, _sc.First(), dates:dates, ids: GetIds(_sc)),
                IsCloseable = "True",
                //TabLocalMenu = new Yamed.Registry.RegistryMenu().MenuElements
            });
        }
    }
}
