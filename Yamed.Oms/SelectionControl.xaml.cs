using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Yamed.Control;
using Yamed.Core;
using Yamed.Server;

namespace Yamed.Oms
{
    /// <summary>
    /// Логика взаимодействия для ReportsWindow.xaml
    /// </summary>
    public partial class SelectionControl : UserControl
    {
        private object[] _rows;
        public SelectionControl(object[] rows)
        {
            InitializeComponent();
            _rows = rows;

            MeeList.DataContext = SqlReader.Select("Select * From Yamed_ExpSpr_SqlAlg where ExpType = 2", SprClass.LocalConnectionString);

        }



        
        private StringBuilder sb;
        private StringBuilder sb2;


        private void LogBox_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            LogBox.Focus();
            Dispatcher.BeginInvoke(new Action(() => LogBox.SelectionStart = LogBox.Text.Length));
        }

        string GetStringOfDates(object[] collection)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int item in collection.Select(x => ObjHelper.GetAnonymousValue(x, "ID")))
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            List<int> ids = new List<int>();

            foreach (DynamicBaseClass mek in MeeList.SelectedItems)
            {
                var result = SqlReader.Select(((string)mek.GetValue("AlgSql")).Replace("@pp1", GetStringOfDates(_rows)).Replace("@p1", GetStringOfDates(_rows)), SprClass.LocalConnectionString);
                ids.AddRange(result.Select(x => (int)x.GetValue("ID")));
            }
            var rc = new SchetRegisterControl();
            rc.SchetRegisterGrid1.BindDataExpResult(ids);
            СommonСomponents.DxTabControlSource.TabElements.Add(new TabElement()
            {
                Header = "Результат выборки",
                MyControl = rc,
                IsCloseable = "True",
            });
        }
    }
}
