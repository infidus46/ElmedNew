using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Grid;
using Yamed.Core;
using Yamed.Server;

namespace Yamed.Control.Editors
{
    /// <summary>
    /// Логика взаимодействия для NsiControl.xaml
    /// </summary>
    public partial class NsiControl : UserControl
    {

        private static object _row;
        public static string tmp_table;
        private UniSprControl unitab;
        private UniSprEditControl sprEditWindow;

        enum BarOperations //опероации
        {
            New = 0,
            Edit,
            Save,
            Delete,
            Back
        }


        public NsiControl()
        {
            InitializeComponent();

            SprLoad();
        }


        #region buttons

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationFrame2.GoBack();

            AppBarCustom(BarOperations.Back);//настройка кнопок AppBar-а
        }

        private void NewButton_OnClick(object sender, RoutedEventArgs e)
        {

            var type = unitab.GridControlSpr.DataContext.GetType().GetGenericArguments()[0];
            var obj = Activator.CreateInstance(type);
            sprEditWindow = new UniSprEditControl(unitab, obj, false);
            NavigationFrame2.Navigate(sprEditWindow);
            AppBarCustom(BarOperations.New);//настройка кнопок AppBar-а

        }

        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {

            sprEditWindow = new UniSprEditControl(unitab, unitab.GridControlSpr.SelectedItem, true);

            NavigationFrame2.Navigate(sprEditWindow);
            AppBarCustom(BarOperations.Edit);//настройка кнопок AppBar-а


        }



        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                sprEditWindow.SaveItem();
            }
            catch
            { MessageBox.Show("Ошибка в заполнении полей."); }
            AppBarCustom(BarOperations.Save);//настройка кнопок AppBar-а
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var pkCol = Reader2List.CustomSelect<DBaseInfoClass.PrimaryKeyColumn>($@"
                SELECT Col.Column_Name from
                    INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab,
                    INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Col
                WHERE
                    Col.Constraint_Name = Tab.Constraint_Name
                    AND Col.Table_Name = Tab.Table_Name
                    AND Constraint_Type = 'PRIMARY KEY'
                    AND Col.Table_Name = '{unitab.TableName}'",
               unitab.ConnectionString).Single();

            var row = unitab.GridControlSpr.SelectedItem;
            var rowId = row.GetType().GetProperty(pkCol.Column_Name).GetValue(row, null);
            Reader2List.CustomExecuteQuery($"DELETE FROM {unitab.TableName} WHERE {pkCol.Column_Name}={rowId}", unitab.ConnectionString);
            unitab.DataUpdater(unitab.TableName, unitab.ConnectionString);
        }
        #endregion buttons



        private void SprControl_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            _row = ((GridControl)sender).SelectedItem;
            tmp_table = (string)ObjHelper.GetAnonymousValue(_row, "ИД");

            unitab = new UniSprControl(tmp_table, SprClass.LocalConnectionString, false);

            try
            {
                NavigationFrame2.Navigate(unitab);
                AppBarCustom(BarOperations.Back);

            }
            catch
            {
                MessageBox.Show("Ошибка.");
            }
        }


        private void SprLoad()
        {
            var list1 = (IList)Reader2List.CustomAnonymousSelect(@"Select TableName ИД, TableDisplayName + ' (' + TableName + ')' Наименование, Comment Коментарий  from [SettingsTables] where TableType = 2",
                  SprClass.LocalConnectionString);


            SprControl.DataContext = list1;

        }






        private void AppBarCustom(BarOperations bo)
        {
            switch (bo)
            {
                case BarOperations.New:

                    BackButton.IsEnabled = true;
                    NewButton.IsEnabled = false;
                    EditButton.IsEnabled = false;
                    SaveButton.IsEnabled = true;
                    DeleteButton.IsEnabled = false;
                    break;
                case BarOperations.Edit:
                    BackButton.IsEnabled = true;
                    NewButton.IsEnabled = false;
                    EditButton.IsEnabled = false;
                    SaveButton.IsEnabled = true;
                    DeleteButton.IsEnabled = false;
                    break;
                case BarOperations.Save:
                    BackButton.IsEnabled = true;
                    NewButton.IsEnabled = false;
                    EditButton.IsEnabled = false;
                    SaveButton.IsEnabled = true;
                    DeleteButton.IsEnabled = false;
                    break;
                case BarOperations.Back:
                    BackButton.IsEnabled = true;
                    NewButton.IsEnabled = true;
                    EditButton.IsEnabled = true;
                    SaveButton.IsEnabled = true;
                    DeleteButton.IsEnabled = true;
                    break;

            }
        }


    }
}
