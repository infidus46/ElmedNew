using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using Yamed.Server;

namespace Yamed.Control.Editors
{
    /// <summary>
    /// Логика взаимодействия для UniSprFullControl.xaml
    /// </summary>
    public partial class UniSprFullControl : UserControl
    {
        public UniSprFullControl(string tableName, string connectionString, bool isReadOnly)
        {
            InitializeComponent();
            UniSprControl1.GetDbSchema(tableName, connectionString, isReadOnly);
        }


        private void NewItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var type = UniSprControl1.GridControlSpr.DataContext.GetType().GetGenericArguments()[0];
            var obj = Activator.CreateInstance(type);
            var sprEditWindow = new UniSprEditControl(UniSprControl1, obj, false);
            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Content = sprEditWindow,
                Title = "Новая запись"
            };
            window.ShowDialog();
        }

        private void EditItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var sprEditWindow = new UniSprEditControl(UniSprControl1, UniSprControl1.GridControlSpr.SelectedItem, true);
            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Content = sprEditWindow,
                Title = "Редактирование"
            };
            window.ShowDialog();
        }

        private void DeleteItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var pkCol = Reader2List.CustomSelect<DBaseInfoClass.PrimaryKeyColumn>($@"
                SELECT Col.Column_Name from
                    INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab,
                    INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Col
                WHERE
                    Col.Constraint_Name = Tab.Constraint_Name
                    AND Col.Table_Name = Tab.Table_Name
                    AND Constraint_Type = 'PRIMARY KEY'
                    AND Col.Table_Name = '{UniSprControl1.TableName}'",
                UniSprControl1.ConnectionString).Single();

            var row = UniSprControl1.GridControlSpr.SelectedItem;
            var rowId = row.GetType().GetProperty(pkCol.Column_Name).GetValue(row, null);
            Reader2List.CustomExecuteQuery($"DELETE FROM {UniSprControl1.TableName} WHERE {pkCol.Column_Name}={rowId}", UniSprControl1.ConnectionString);
            UniSprControl1.DataUpdater(UniSprControl1.TableName, UniSprControl1.ConnectionString);
        }

        private void UpdateItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            UniSprControl1.DataUpdater(UniSprControl1.TableName, UniSprControl1.ConnectionString);
        }
    }
}
