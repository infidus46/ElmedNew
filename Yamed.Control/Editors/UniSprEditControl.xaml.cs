using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.LayoutControl;
using Yamed.Core;
using Yamed.Server;


namespace Yamed.Control.Editors
{
    class UniSprSettings
    {
        public Visibility BarVisibility { get; set; }
    }


    /// <summary>
    /// Interaction logic for AttachedUniSprEditWindow.xaml
    /// </summary>
    public partial class UniSprEditControl : UserControl
    {
        private readonly object _obj;
        private readonly bool _is_identity;
        private readonly DBaseInfoClass.PrimaryKeyColumn _pkCol;
        private readonly bool _isEdit;

        private readonly string _tableName;private readonly string _connectionString;
        private readonly List<DBaseInfoClass.ListExtendedProperty> _extendedProperties;
        private readonly List<DBaseInfoClass.ColumnsInformationSchema> _columnsInformationSchema;
        private IDictionary<string, object> _foreignTable;
        public readonly List<DBaseInfoClass.ForeignKeyColumns> _foreignKeyColumn;
        public bool IsConfigMode = false;

        private UniSprControl _uniSprTab;

        UniSprSettings uSprSettings = new UniSprSettings();

        public UniSprEditControl(UniSprControl parrent, object obj, bool isEdit, bool isBarVisible = true)
        {
            InitializeComponent();

            uSprSettings.BarVisibility = isBarVisible ? Visibility.Visible : Visibility.Collapsed;
            this.DataContext = uSprSettings;

            _tableName = parrent.TableName;
            _connectionString = parrent.ConnectionString;
            _obj = obj;
            _isEdit = isEdit;
            _extendedProperties = parrent.ExtendedProperties;
            _columnsInformationSchema = parrent.ColumnsInformationSchema;
            _foreignTable = parrent.ForeignTable;
            _foreignKeyColumn = parrent.ForeignKeyColumn;

            _uniSprTab = parrent;

            _pkCol = DBaseInfo.GetPrimaryKeyInfo(_tableName, _connectionString);

            var info = (IList)DBaseInfo.GetColumnInfo(_tableName, _pkCol.Column_Name, _connectionString);
            _is_identity = (bool) ObjHelper.GetAnonymousValue(info[0], "is_identity");

            SprDataLayoutControl.DataContext = _obj;
        }

         

        public UniSprEditControl(string tableName, object obj, bool isEdit, string connectionString)
        {
            InitializeComponent();

            _tableName = tableName;
            _connectionString = connectionString;
            _obj = obj;
            _isEdit = isEdit;
            _extendedProperties = UniSprControl.GetExtendedProperties(_tableName, _connectionString);
            _columnsInformationSchema = Reader2List.CustomSelect<DBaseInfoClass.ColumnsInformationSchema>($"select * from information_schema.columns where table_name = '{tableName}' and column_name <> 'NameWithID' and column_name not like '%_NOTEDIT'", _connectionString);
            _foreignKeyColumn = UniSprControl.GetForeignKeyColumn(_tableName, _connectionString);

            _foreignTable = new Dictionary<string, object>();


            //Parallel.ForEach(_foreignKeyColumn, (fkc) =>
            //{
            //    var rt = Reader2List.GetAnonymousTable(fkc.ReferenceTableName, _connectionString);
            //    if (_foreignTable.Any(x => x.Key == fkc.ReferenceTableName))
            //        fkc.ReferenceTableName = fkc.ReferenceTableName + "___Guid___" + Guid.NewGuid();
            //    _foreignTable.Add(fkc.ReferenceTableName, rt);

            //});

            //foreach (var fkc in _foreignKeyColumn)
            //{
            //    var rt = Reader2List.GetAnonymousTable(fkc.ReferenceTableName, _connectionString);
            //    if (_foreignTable.Any(x => x.Key == fkc.ReferenceTableName))
            //        fkc.ReferenceTableName = fkc.ReferenceTableName + "___Guid___" + Guid.NewGuid();
            //    _foreignTable.Add(fkc.ReferenceTableName, rt);
            //}
            _pkCol = DBaseInfo.GetPrimaryKeyInfo(_tableName, _connectionString);
            var info = (IList) DBaseInfo.GetColumnInfo(_tableName, _pkCol.Column_Name, _connectionString);
            _is_identity = (bool)ObjHelper.GetAnonymousValue(info[0], "is_identity");

            SprDataLayoutControl.DataContext = _obj;
        }

        public void SaveItem()
        {
            if (_isEdit == false)
            {
                if (_is_identity)
                {
                   var id = Reader2List.ObjectInsertCommand(_tableName, _obj, _pkCol.Column_Name, _connectionString);
                   _obj.GetType().GetProperty(_pkCol.Column_Name).SetValue(_obj, id, null);
                }
                else
                    Reader2List.ObjectInsertCommand(_tableName, _obj, "not_identity", _connectionString);
            }
            else
            {
                Reader2List.CustomExecuteQuery(
                    Reader2List.CustomUpdateCommand(_tableName, _obj, _pkCol.Column_Name)
                    , _connectionString);
            }

            if (_uniSprTab != null)
                _uniSprTab.DataUpdater(_tableName, _connectionString);

            var parent = this.Parent as DXWindow;
            if (parent != null)
            {
                parent.DialogResult = true;
                parent.Close();
            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveItem();
        }

        private void DataLayoutControl_OnAutoGeneratingItem(object sender, DataLayoutControlAutoGeneratingItemEventArgs e)
        {
            if (_columnsInformationSchema.SingleOrDefault(x => x.COLUMN_NAME == e.PropertyName) == null)
            {
                e.Cancel = true;
                return;
            }
            var cn = _extendedProperties.SingleOrDefault(x => x.objname == e.PropertyName);
            if (cn != null) e.Item.Label = cn.value;

            if (_columnsInformationSchema.Single(x => x.COLUMN_NAME == e.PropertyName).IS_NULLABLE == "NO")
            {
                if (e.PropertyName == _pkCol.Column_Name && _is_identity)
                    ((BaseEdit)e.Item.Content).IsEnabled = false;
                else
                {
                    e.Item.IsRequired = true;
                    ((BaseEdit)e.Item.Content).InvalidValueBehavior = InvalidValueBehavior.AllowLeaveEditor;
                    ((BaseEdit)e.Item.Content).Validate += (o, args) =>
                    {
                        if (args.Value == null)
                        {
                            args.IsValid = false;
                        }
                    };
                }
            }
            
            if (e.Item.Content is DateEdit)
            {((DateEdit)e.Item.Content).NullValueButtonPlacement = EditorPlacement.EditBox;
                var bind1 = new Binding(e.PropertyName);
                bind1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                ((DateEdit)e.Item.Content).SetBinding(BaseEdit.EditValueProperty, bind1);

            }
            var fkc = _foreignKeyColumn.SingleOrDefault(x => x.ColumnName == e.PropertyName);
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

            var cis = Reader2List.CustomSelect<DBaseInfoClass.ColumnsInformationSchema>($"select * from information_schema.columns where table_name = '{rtName}'", _connectionString);
            var dn = cis.SingleOrDefault(x => x.COLUMN_NAME.StartsWith("NameW")) ??
                        cis.SingleOrDefault(x => x.COLUMN_NAME.StartsWith("Name")) ?? cis.Single(x => x.ORDINAL_POSITION == 2);
            //var rt = _foreignTable.Single(x => x.Key == rtName);


            var bind = new Binding(e.PropertyName);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            var control = new ComboBoxEdit();control.ValueMember = fkc.ReferenceColumnName;
            control.DisplayMember = dn.COLUMN_NAME;
            //control.ItemsSource = rt.Value;
            control.SetBinding(BaseEdit.EditValueProperty, bind);
            control.NullValueButtonPlacement = EditorPlacement.EditBox;
            control.IncrementalFiltering = true;
            control.AutoComplete = true;
            control.FilterCondition = FilterCondition.Contains;
            
            e.Item.Content = control;

            if (_columnsInformationSchema.Single(x => x.COLUMN_NAME == e.PropertyName).IS_NULLABLE == "NO")
            {
                e.Item.IsRequired = true;
                ((BaseEdit)e.Item.Content).InvalidValueBehavior = InvalidValueBehavior.AllowLeaveEditor;
                ((BaseEdit) e.Item.Content).Validate += (o, args) =>
                {
                    if (args.Value == null)
                    {
                        args.IsValid = false;
                    }
                };
            }

            var mekTask = Task.Factory.StartNew(() =>
            {
                var tn = fkc.ReferenceTableName.Contains("___Guid___")
                    ? fkc.ReferenceTableName.Remove(fkc.ReferenceTableName.IndexOf("___Guid___", StringComparison.CurrentCulture))
                    : fkc.ReferenceTableName;
                var rt = Reader2List.GetAnonymousTable(tn, _connectionString);
                if (_foreignTable.Any(x => x.Key == fkc.ReferenceTableName))
                    fkc.ReferenceTableName = fkc.ReferenceTableName + "___Guid___" + Guid.NewGuid();
                _foreignTable.Add(fkc.ReferenceTableName, rt);

                return rt;
            });
            mekTask.ContinueWith(x =>
            {
                if (_tableName == "D3_AKT_REGISTR_OMS" && rtName == "F006_NEW")
                {
                    control.ItemsSource = Reader2List.CustomAnonymousSelect($@"select * from f006_new where dateend is null and idvid not in (1,2,3) and exp_re!=1", SprClass.LocalConnectionString);
                }
                else
                {
                    control.ItemsSource = x.Result;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }


        private void SaveLayoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            var ts =
                (IEnumerable<dynamic>)
                    Reader2List.CustomAnonymousSelect($"Select * from SettingsTables where TableName = '{_tableName}'",
                        SprClass.LocalConnectionString);
            if (ts.Any())
            {
                MemoryStream stream = new MemoryStream();
                //Save the layout
                XmlWriter writer = XmlWriter.Create(stream);
                SprDataLayoutControl.WriteToXML(writer);
                writer.Close();

                stream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(stream);
                var l = reader.ReadToEnd();
                var obj = ts.First();

                ObjHelper.SetAnonymousValue(ref obj, l, "TableLayout");

                var upd = Reader2List.CustomUpdateCommand("SettingsTables", obj, "ID");
                Reader2List.CustomExecuteQuery(upd, SprClass.LocalConnectionString);

                stream.Close();
                reader.Close();
            }
            else
            {
                DXMessageBox.Show("Нет настройки для этой формы");
            }
        }

        private void SprDataLayoutControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            SaveButton.IsEnabled = !IsConfigMode;
            LayoutToggleButton.IsChecked = IsConfigMode;

            var ts =
                (IEnumerable<dynamic>)
                    Reader2List.CustomAnonymousSelect($"Select * from SettingsTables where TableName = '{_tableName}'",
                        SprClass.LocalConnectionString);
            if (ts.Any())
            {
                var tl = (string)ObjHelper.GetAnonymousValue(ts.First(), "TableLayout");
                if (!string.IsNullOrWhiteSpace(tl))
                {
                    MemoryStream stream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(tl);
                    writer.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    XmlReader reader = XmlReader.Create(stream);
                    SprDataLayoutControl.ReadFromXML(reader);
                }
            }
        }


        public void SprClear()
        {
            _extendedProperties.Clear();
            _columnsInformationSchema.Clear();
            _foreignTable.Clear();
            _foreignKeyColumn.Clear();

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                GC.WaitForPendingFinalizers();
                GC.Collect();
            });
        }

    }
}