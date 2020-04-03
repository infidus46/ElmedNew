using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Yamed.Core
{
    public enum OrgType { Tfoms, Smo, Lpu, Test }

    public class ObjHelper
    {
        public static T ClassConverter<T>(object obj_orig)
        {
            T obj = default(T);
            
            obj = Activator.CreateInstance<T>();
            var propList = obj.GetType().GetProperties().ToList();
            foreach (var o in propList)
            {
                var val = GetAnonymousValue(obj_orig, o.Name); if (val != null)
                    o.SetValue(obj, val, null);
            }

            return obj;
        }

        public static object GetAnonymousValue(object obj, string field)
        {
            return obj.GetType().GetProperty(field)?.GetValue(obj, null);
        }

        public static void SetAnonymousValue(ref object obj, object value, string field)
        {
            obj.GetType().GetProperty(field).SetValue(obj, value, null);
        }

        public static string GetIds(int[] collection)
        {
            if (collection == null || collection.Count() == 0) return "0";

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


    }

    public class DynamicBaseClass : IDisposable
    {

        public object GetValue (string field)
        {
            return this.GetType().GetProperty(field)?.GetValue(this, null);
        }

        public void SetValue(string field, object value)
        {
            this.GetType().GetProperty(field)?.SetValue(this, value, null);
        }

        public DynamicBaseClass()
        {

        }

        //#region INotifyPropertyChanged Members
        //public event PropertyChangedEventHandler PropertyChanged;
        //protected virtual void RaisePropertyChanged(string propertyName)
        //{
        //    var handler = PropertyChanged;
        //    handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
        //#endregion

        #region IDisposable Members

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            this.OnDispose();
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {
        }
        #endregion // IDisposable Members
    }

    public class DynamicCollection<T> : ObservableCollection<T>
    {
        public DynamicCollection() : base()
        {
            // СДЕЛАТЬ ОБНОВЛЕНИЕ ИЗ БАЗЫ

            //ХРАНИТЬ ЗАПРОС
        }

        private Type DynamicType { get; set; }

        public void SetDynamicType(Type type)
        {
            DynamicType = type;
        }

        public Type GetDynamicType()
        {
            return DynamicType;
        }
    }
}
