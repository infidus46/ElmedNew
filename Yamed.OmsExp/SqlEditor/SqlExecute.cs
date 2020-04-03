using System;
using System.Collections;
using System.Windows;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using Yamed.Core;
using Yamed.Server;

namespace Yamed.OmsExp.SqlEditor
{
    class SqlExecute
    {
        public static void GetData(GridControl gridControl, string sqlQuery)
        {
            gridControl.DataContext = Reader2List.CustomAnonymousSelect(sqlQuery,
                SprClass.LocalConnectionString);
        }

        public static void UpdateData(GridControl listBoxEdit)
        {
            listBoxEdit.DataContext = Reader2List.CustomAnonymousSelect(MekQuery, SprClass.LocalConnectionString);
        }

        private const string MekQuery = "Select * From Yamed_ExpSpr_SqlAlg";
        public static void AddData(GridControl listBoxEdit)
        {
            var empty = SqlReader.Select2("Select * From Yamed_ExpSpr_SqlAlg where ID = 0",
                SprClass.LocalConnectionString);
            var obj = Activator.CreateInstance(empty.GetDynamicType());
            ((DynamicBaseClass)obj).SetValue("AlgName", "Новая запись");
            ((DynamicBaseClass)obj).SetValue("AlgGuid", Guid.NewGuid());

            Reader2List.ObjectInsertCommand("Yamed_ExpSpr_SqlAlg", obj, "ID", SprClass.LocalConnectionString);
            UpdateData(listBoxEdit);
            listBoxEdit.SelectItem(((IList)listBoxEdit.DataContext).Count - 1);
            listBoxEdit.View.ScrollIntoView(listBoxEdit.SelectedItem);
        }

        public static void CopyData(GridControl listBoxEdit)
        {
            var item = (DynamicBaseClass)listBoxEdit.SelectedItem;
            item.SetValue("AlgName", (string)item.GetValue("AlgName") + " (копия)");
            item.SetValue("AlgGuid", Guid.NewGuid());
            Reader2List.ObjectInsertCommand("Yamed_ExpSpr_SqlAlg", listBoxEdit.SelectedItem, "ID", SprClass.LocalConnectionString);
            UpdateData(listBoxEdit);
            listBoxEdit.SelectItem(((IList)listBoxEdit.DataContext).Count - 1);
            listBoxEdit.View.ScrollIntoView(listBoxEdit.SelectedItem);
        }

        public static void DeleteData(GridControl listBoxEdit)
        {
            MessageBoxResult result = MessageBox.Show("Удалить алгоритм - " + ((DynamicBaseClass)listBoxEdit.SelectedItem).GetValue("AlgName"), "Удаление алгоритма" + "?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Reader2List.CustomExecuteQuery($"Delete Yamed_ExpSpr_SqlAlg Where Id = {((DynamicBaseClass) listBoxEdit.SelectedItem).GetValue("ID")}",
                    SprClass.LocalConnectionString);
                UpdateData(listBoxEdit);
            }
        }

        public static void SaveAllData(GridControl listBoxEdit)
        {
            MessageBoxResult result = MessageBox.Show("Сохранить изменения в алгоритме?", "Сохранение алгоритма", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Reader2List.CustomExecuteQuery(Reader2List.CustomUpdateCommand("Yamed_ExpSpr_SqlAlg", (IList)listBoxEdit.DataContext, "ID"), SprClass.LocalConnectionString);
                UpdateData(listBoxEdit);
            }
        }

        public static void SaveData(GridControl listBoxEdit)
        {
            Reader2List.CustomExecuteQuery(Reader2List.CustomUpdateCommand("Yamed_ExpSpr_SqlAlg", listBoxEdit.SelectedItem, "ID"), SprClass.LocalConnectionString);
            UpdateData(listBoxEdit);
        }
    }
}
