using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using DevExpress.Data;
using DevExpress.Data.Async.Helpers;
using DevExpress.Xpf.Grid;

namespace Yamed.Control
{
    public class DxHelper
    {

        public static object GetSelectedGridRow(GridControl gridControl)
        {
            var handles = gridControl.GetSelectedRowHandles();
            if (handles != null && handles.Any())
            {
                foreach (var handle in handles)
                {
                    if (handle >= 0)
                    {
                        var row = (DevExpress.Data.Async.Helpers.ReadonlyThreadSafeProxyForObjectFromAnotherThread)gridControl.GetRow(handle);
                        return row.OriginalRow;
                    }
                }
            }
            return null;
        }

        public static object[] GetSelectedGridRows(GridControl gridControl)
        {
            bool isAsyncLoader = false;
            var handles = gridControl.GetSelectedRowHandles();
            if (handles == null || !handles.Any()) return null;    
            handles = handles.Where(x => x >= 0).Select(x => x).ToArray();
            var count = handles.Count();
            object[] obj = new object[count];
            if (count > 500) LoadedRows = gridControl.DataController.GetAllFilteredAndSortedRows().OfType<ReadonlyThreadSafeProxyForObjectFromAnotherThread>().Select(x => x.OriginalRow).ToList();
            else
            {
                LoadedRows = new List<object>();
                for (int index = 0; index < count; index++)
                {
                    var handle = handles[index];
                    //obj[index] =
                    //    ((DevExpress.Data.Async.Helpers.ReadonlyThreadSafeProxyForObjectFromAnotherThread)
                    //        gridControl.GetRow(handle)).OriginalRow;
                    var row = gridControl.GetRow(handle);

                    if (row is NotLoadedObject)
                    {
                        if (!isAsyncLoader)
                        {
                            gridControl.AsyncOperationCompleted += GridControlOnAsyncOperationCompleted;
                            isAsyncLoader = true;
                        }
                        gridControl.GetRowAsync(handle).ContinueWith((x) =>
                        {
                            LoadedRows.Add(
                                ((ReadonlyThreadSafeProxyForObjectFromAnotherThread)
                                    x.Result)?.OriginalRow);
                        });
                    }
                    else
                        LoadedRows.Add(
                            ((ReadonlyThreadSafeProxyForObjectFromAnotherThread)
                                row).OriginalRow);
                }

                IsLoaded = !isAsyncLoader;
            }
            return obj = LoadedRows.ToArray();
        }


        public static List<object> LoadedRows;
        public static bool IsLoaded;

        public static void GetSelectedGridRowsAsync(ref GridControl gridControl)
        {
            bool isAsyncLoader = false;

            var handles = gridControl.GetSelectedRowHandles();

            handles = handles.Where(x => x >= 0).Select(x => x).ToArray();

            var count = handles.Length;
            if (count > 500) LoadedRows = gridControl.DataController.GetAllFilteredAndSortedRows().OfType<ReadonlyThreadSafeProxyForObjectFromAnotherThread>().Select(x=>x.OriginalRow).ToList();
            else
            {
                LoadedRows = new List<object>();

                for (int index = 0; index < count; index++)
                {
                    int i = index;
                    var handle = handles[i];
                    var row = gridControl.GetRow(handle);

                    if (row is NotLoadedObject)
                    {
                        if (!isAsyncLoader)
                        {
                            gridControl.AsyncOperationCompleted += GridControlOnAsyncOperationCompleted;
                            isAsyncLoader = true;
                        }
                        gridControl.GetRowAsync(handle).ContinueWith((x) =>
                        {
                            LoadedRows.Add(
                                ((ReadonlyThreadSafeProxyForObjectFromAnotherThread)
                                    x.Result)?.OriginalRow);
                        });
                    }
                    else
                        LoadedRows.Add(
                            ((ReadonlyThreadSafeProxyForObjectFromAnotherThread)
                                row).OriginalRow);
                }

                IsLoaded = !isAsyncLoader;
            }

        }

        private static void GridControlOnAsyncOperationCompleted(object sender, RoutedEventArgs routedEventArgs)
        {
            ((GridControl)sender).AsyncOperationCompleted -= GridControlOnAsyncOperationCompleted;
            IsLoaded = true;
        }
    }
}
