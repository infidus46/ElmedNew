using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors;
using Yamed.Core;
using Yamed.Server;


namespace Yamed.Pattern
{
    /// <summary>
    /// Логика взаимодействия для SettingEditControl.xaml
    /// </summary>
    public partial class SettingEditControl : UserControl
    {
        object _obj;
        public SettingEditControl(object obj)
        {
            InitializeComponent();

            _obj = obj;

            //if (Yamed.Pattern.SqlDbGUI.TablesSchema == null)
            {
                Yamed.Pattern.SqlDbGUI.TablesSchema = Reader2List.CustomAnonymousSelect(@"
                SELECT TABLE_NAME ID, TABLE_SCHEMA + '.' + TABLE_NAME NAME
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = 'dbo' ", SprClass.LocalConnectionString);
            }

            TableParentEdit.DataContext = Yamed.Pattern.SqlDbGUI.TablesSchema;;
            TableTypeEdit.DataContext = Reader2List.CustomAnonymousSelect("select * from SprTableType",
                SprClass.LocalConnectionString);
            Grid1.DataContext = _obj;

            dynamic nullableYes = new ExpandoObject();
            nullableYes.ID = "YES";
            nullableYes.NAME = "Да";
            dynamic nullableNo = new ExpandoObject();
            nullableNo.ID = "NO";
            nullableNo.NAME = "Нет";

            var dataTypes = new object[]
            {
                "varbinary", "binary", "varbinary", "binary", "image", "varchar", "char",
                "nvarchar", "nchar", "nvarchar", "nchar", "text", "ntext", "uniqueidentifier", "rowversion", "bit",
                "tinyint", "smallint", "int", "bigint", "smallmoney", "money", "numeric", "decimal", "real", "float",
                "smalldatetime", "datetime"
            };
            DataTypeBoxEditSettings.Items.AddRange(dataTypes);

            var nullableList = new List<dynamic>() {nullableNo, nullableYes};
            NullableBoxEditSettings.DataContext = nullableList;

            _table = (string)ObjHelper.GetAnonymousValue(_obj, "TableName");
            if (ObjHelper.GetAnonymousValue(_obj, "ID") != null && (int)ObjHelper.GetAnonymousValue(_obj, "ID") != 0)
            {
                Grid2.IsEnabled = true;
                GridControlUpdate();
            }

        }

        private void GridControlUpdate()
        {
            //var list =
            //    Reader2List.CustomAnonymousSelect(
            //        $"select ORDINAL_POSITION, COLUMN_NAME, COLUMN_DEFAULT, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE from information_schema.columns where table_name = '{_table}'",
            //        SprClass.LocalConnectionString);

            _list = (IList)
                Reader2List.CustomAnonymousSelect(
                    $@"select ID, ORDINAL_POSITION, COLUMN_NAME, COLUMN_DEFAULT, IS_NULLABLE, DATA_TYPE, 
                                CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE, SPR_TABLE, 
                                TEMPLATE_TABLE, TEMPL_ID_FIELD, TEMPL_DATA_FIELD 
                                from SettingsTableFields where table_name = '{_table}'",
                    SprClass.LocalConnectionString);

            _type = _list.GetType().GetGenericArguments()[0];
            GridControl1.DataContext = _list;

        }

        Type _type;
        private IList _list;
        private List<dynamic> _delList;
        private string _table;

        private void AddItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var newObj = Activator.CreateInstance(_type);
            _list.Add(newObj);
            GridControl1.RefreshData();
        }

        public void SaveTable()
        {
            Reader2List.CustomExecuteQuery(Reader2List.CustomUpdateCommand("SettingsTableFields", _list, "ID"), SprClass.LocalConnectionString);
            
            foreach (var obj in _list)
            {
                var col = ObjHelper.GetAnonymousValue(obj, "COLUMN_NAME");
                var dtype = (string)ObjHelper.GetAnonymousValue(obj, "DATA_TYPE");
                var charLen = (int?)ObjHelper.GetAnonymousValue(obj, "CHARACTER_MAXIMUM_LENGTH");
                var numPrec = (byte?)ObjHelper.GetAnonymousValue(obj, "NUMERIC_PRECISION");
                var numScale = (int?)ObjHelper.GetAnonymousValue(obj, "NUMERIC_SCALE");
                var nullable = (string)ObjHelper.GetAnonymousValue(obj, "IS_NULLABLE");
                var colDefault = (string)ObjHelper.GetAnonymousValue(obj, "COLUMN_DEFAULT");
                colDefault = !string.IsNullOrWhiteSpace(colDefault) ? $"default('{colDefault}')" : null;
                nullable = nullable == "YES" ? "NULL" : nullable == "NO" ? "NOT NULL" : null;
                dtype = dtype == "char" || dtype == "nchar" || dtype == "varchar" || dtype == "nvarchar"
                    ? $"{dtype} ({charLen})"
                    : dtype == "numeric" ? $"{dtype} ({numPrec}, {numScale})" : dtype;

                //if (ObjHelper.GetAnonymousValue(obj, "ORDINAL_POSITION") == null)
                if ((int)ObjHelper.GetAnonymousValue(obj, "ID") == 0)
                    {
                        Reader2List.CustomExecuteQuery(
                        $@"
                    Alter Table {_table}
                    ADD {col} {dtype} {nullable} {colDefault}", SprClass.LocalConnectionString);
                }
                else
                {
                    Reader2List.CustomExecuteQuery(
                        $@"
                    Alter Table {_table}
                    Alter Column {col} {dtype} {nullable} {colDefault}", SprClass.LocalConnectionString);
                }
            }

            if (_delList != null)
            {
                foreach (var obj in _delList)
                {
                    var col = ObjHelper.GetAnonymousValue(obj, "COLUMN_NAME");
                    Reader2List.CustomExecuteQuery(
                            $@"
                    Alter Table {_table}
                    Drop Column {col}", SprClass.LocalConnectionString);
                }
                _delList = null;
            }

            GridControlUpdate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _table = (string)ObjHelper.GetAnonymousValue(_obj, "TableName");

            if (ObjHelper.GetAnonymousValue(_obj, "FieldParent") == null)
            {
                var tableParent = (string)ObjHelper.GetAnonymousValue(_obj, "TableParent");
                Reader2List.ObjectInsertCommand("SettingsTables", _obj, "ID",
                    SprClass.LocalConnectionString);
                Reader2List.CustomExecuteQuery($@"
                    Create Table {_table} (
                    ID int identity Primary Key not null,
                    {tableParent}_ID int not null)", SprClass.LocalConnectionString);

                if (tableParent != null)
                {
                    Reader2List.CustomExecuteQuery($@"
                    ALTER TABLE {_table}  WITH NOCHECK ADD CONSTRAINT [FK_{_table}_{tableParent}] FOREIGN KEY({tableParent}_ID)
                    REFERENCES {tableParent} ([ID])
                    ALTER TABLE {_table} CHECK CONSTRAINT [FK_{_table}_{tableParent}]", SprClass.LocalConnectionString);
                    Reader2List.CustomExecuteQuery($@"
                    CREATE NONCLUSTERED INDEX IX_{tableParent}_ID ON {_table}
                        (
                        	{tableParent}_ID ASC
                        )", SprClass.LocalConnectionString);
                }
            }
            else
            {
                var tableParent = (string)ObjHelper.GetAnonymousValue(_obj, "TableParent");
                var fieldParent = (string)ObjHelper.GetAnonymousValue(_obj, "FieldParent");
                Reader2List.ObjectInsertCommand("SettingsTables", _obj, "ID",
                    SprClass.LocalConnectionString);
                Reader2List.CustomExecuteQuery(
                           $@"
                    Create Table {_table} (
                    ID int Primary Key not null,
                    NAME nvarchar(150) not null)", SprClass.LocalConnectionString);
                Reader2List.CustomExecuteQuery(
                           $@"
                    ALTER TABLE {tableParent}  WITH NOCHECK ADD CONSTRAINT [FK_{tableParent}_{_table}] FOREIGN KEY({fieldParent})
                    REFERENCES {_table} ([ID])
                    ALTER TABLE {tableParent} CHECK CONSTRAINT [FK_{tableParent}_{_table}]", SprClass.LocalConnectionString);
            }

            Grid2.IsEnabled = true;
            GridControlUpdate();
        }

        private void DeleteItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (_delList == null)
                _delList = new List<dynamic>();

            var item = GridControl1.SelectedItem;

            if(ObjHelper.GetAnonymousValue(item, "ORDINAL_POSITION") != null)
                _delList.Add(item);

            _list.Remove(item);
            GridControl1.RefreshData();

        }

        private void TableParentEdit_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var cols =
                Reader2List.CustomAnonymousSelect(
                    $"select COLUMN_NAME ID, COLUMN_NAME + ' (' + DATA_TYPE + ')' NAME from information_schema.columns where table_name = '{e.NewValue}'",
                    SprClass.LocalConnectionString);
            FieldParentEdit.DataContext = cols;
        }

        private void LinkItem_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = GridControl1.SelectedItem;

        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool booleanValue = (bool)value;
            return !booleanValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool booleanValue = (bool)value;
            return !booleanValue;
        }
    }

    public class IsReadOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;

            var val = (string) value;
            if (val == "char" || val == "nchar" || val == "varchar" || val == "nvarchar") return true;

            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
