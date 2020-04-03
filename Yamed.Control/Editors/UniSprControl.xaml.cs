using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid;
using Yamed.Server;

namespace Yamed.Control.Editors
{
    /// <summary>
    /// Interaction logic for AttachedUniSprWindow.xaml
    /// </summary>
    public partial class UniSprControl: UserControl
    {
        public string ConnectionString;
        public string TableName;
        public List<DBaseInfoClass.ForeignKeyColumns> ForeignKeyColumn;
        public List<DBaseInfoClass.ListExtendedProperty> ExtendedProperties;
        public IDictionary<string, object> ForeignTable;
        public List<DBaseInfoClass.ColumnsInformationSchema> ColumnsInformationSchema;
        private object _obj;
        public bool _isReadOnly;

        public UniSprControl(string tableName, string connectionString, bool isReadOnly)
        {
            InitializeComponent();
            GetDbSchema(tableName, connectionString, isReadOnly);

        }

        public UniSprControl()
        {
            InitializeComponent();
        }

        public void GetDbSchema(string tableName, string connectionString, bool isReadOnly)
        {
            TableName = tableName;
            _isReadOnly = isReadOnly;
            ConnectionString = connectionString;

            ForeignKeyColumn = GetForeignKeyColumn(tableName, ConnectionString);
            ExtendedProperties = GetExtendedProperties(tableName, ConnectionString);
            ColumnsInformationSchema = GetColumnsInformationSchema(tableName, connectionString);


            ForeignTable = new Dictionary<string, object>();
            foreach (var fkc in ForeignKeyColumn)
            {
                var rt = Reader2List.GetAnonymousTable(fkc.ReferenceTableName, ConnectionString);
                ForeignTable.Add(fkc.ReferenceTableName, rt);
            }

            DataUpdater(TableName, ConnectionString);
        }

        public static List<DBaseInfoClass.ForeignKeyColumns> GetForeignKeyColumn(string tName, string connectionString)
        {
            List<DBaseInfoClass.ForeignKeyColumns> list = new List<DBaseInfoClass.ForeignKeyColumns>();
            list = Reader2List.CustomSelect<DBaseInfoClass.ForeignKeyColumns>(
                $@"SELECT f.name AS ForeignKey,
                    OBJECT_NAME(f.parent_object_id) AS TableName,
                    COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ColumnName,
                    OBJECT_NAME (f.referenced_object_id)AS ReferenceTableName,
                    COL_NAME(fc.referenced_object_id,
                    fc.referenced_column_id) AS ReferenceColumnName
                    FROM sys.foreign_keys AS f
                    INNER JOIN sys.foreign_key_columns AS fc
                    ON f.OBJECT_ID = fc.constraint_object_id
                    where OBJECT_NAME(f.parent_object_id) = '{tName}' and OBJECT_NAME (f.referenced_object_id) not IN ('PACIENT','SLUCH','USL')", connectionString);
            return list;
        }

        public static List<DBaseInfoClass.ListExtendedProperty> GetExtendedProperties(string tName, string connectionString)
        {
            return Reader2List.CustomSelect<DBaseInfoClass.ListExtendedProperty>(
                $@"SELECT * FROM fn_listextendedproperty(NULL, 'schema', 'dbo', 'table', '{tName}', 'column', default)", connectionString);
        }

        private List<DBaseInfoClass.ColumnsInformationSchema> GetColumnsInformationSchema(string tName, string connectionString)
        {
            return
                Reader2List.CustomSelect<DBaseInfoClass.ColumnsInformationSchema>(
                    $"select * from information_schema.columns where table_name = '{tName}' and column_name <> 'NameWithID' and column_name not like '%_NOTEDIT'", connectionString);
        }

        public void DataUpdater(string tName, string connectionString)
        {
            GridControlSpr.DataContext = Reader2List.GetAnonymousTable(tName, connectionString);
        }


        private void TableView_OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {

            var sprEditWindow = new UniSprEditControl(this, GridControlSpr.SelectedItem, true);
            var window = new DXWindow
            {
                ShowIcon = false,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Content = sprEditWindow,
                Title = "Редактирование"
            };
            window.ShowDialog();

        }

        private void GridControlSpr_OnAutoGeneratingColumn(object sender, AutoGeneratingColumnEventArgs e)
        {
            if (e.Column.FieldName.StartsWith("NameWith"))
            {
                e.Cancel = true;
                return;
            }
            e.Column.ReadOnly = true;
            var cn = ExtendedProperties.SingleOrDefault(x => x.objname == e.Column.FieldName);
            if (cn != null) e.Column.Header = cn.value;

            var fkc = ForeignKeyColumn.SingleOrDefault(x => x.ColumnName == e.Column.FieldName);
            if (fkc == null) return;

            string rtName;
            if (fkc.ReferenceTableName.Contains("___Guid___"))
            {
                rtName = fkc.ReferenceTableName.Remove(fkc.ReferenceTableName.IndexOf("___Guid___", StringComparison.CurrentCulture));
            }
            else
            {
                rtName = fkc.ReferenceTableName;
            }

            var cis = Reader2List.CustomSelect<DBaseInfoClass.ColumnsInformationSchema>($"select * from information_schema.columns where table_name = '{rtName}'", ConnectionString);
            var dn = cis.SingleOrDefault(x => x.COLUMN_NAME.StartsWith("NameWith")) ??
                        cis.SingleOrDefault(x => x.COLUMN_NAME.StartsWith("Name")) ?? cis.SingleOrDefault(x => x.COLUMN_NAME.StartsWith("NAME")) ?? cis.Single(x => x.ORDINAL_POSITION == 2);
            var rt = ForeignTable.Single(x => x.Key == fkc.ReferenceTableName);
            e.Column.EditSettings = new ComboBoxEditSettings
            {
                ValueMember = fkc.ReferenceColumnName,
                DisplayMember = dn.COLUMN_NAME,
                ItemsSource = rt.Value,
                AllowDefaultButton = false
            };

        }
    }
}
