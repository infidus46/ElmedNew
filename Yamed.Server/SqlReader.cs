using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Yamed.Core;

namespace Yamed.Server
{
    public class SqlReader
    {

        public static ObservableCollection<DynamicBaseClass> Select(string selectCmd, string connectionString)
        {
            ObservableCollection<DynamicBaseClass> objList;
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(selectCmd, connection))
                {
                    command.CommandTimeout = 0;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        var schema = reader.GetSchemaTable();
                        string[] fields = null;
                        Type[] types = null;

                        if (schema == null)
                        {
                            fields = new string[1];
                            types = new Type[1];

                            fields[0] = "СтрокОбработано";
                            types[0] = typeof(int);
                            objList =DataReaderMapToCollection(
                                    ProxyProvider.Instance.NewProxy2(CustomTypeBuilder.CompileResultType(fields, types)),
                                    reader, false);
                        }
                        else
                        {
                            fields = new string[schema.Rows.Count];
                            types = new Type[schema.Rows.Count];
                            int ci = 1;
                            for (int i = 0; i < schema.Rows.Count; i++)
                            {
                                var col = schema.Rows[i].Field<string>("ColumnName");
                                col = string.IsNullOrWhiteSpace(col)
                                    ? "Column" + ci++
                                    : fields.Any(x => x == col) ? col + fields.Count(x => x == col) : col;

                                fields[i] = col;
                                Type type = schema.Rows[i].Field<Type>("DataType");

                                if (schema.Rows[i].Field<bool>("AllowDBNull"))
                                    types[i] = type == typeof(string) || type == typeof(byte[])
                                        ? type
                                        : typeof(Nullable<>).MakeGenericType(type);
                                else
                                    types[i] = type;
                            }
                            
                            objList =
                                DataReaderMapToCollection(
                                    ProxyProvider.Instance.NewProxy2(CustomTypeBuilder.CompileResultType(fields, types)),
                                    reader);

                            fields = null;
                            types = null;
                        }
                    }
                    connection.Close();
                }
            }
            return objList;
        }


        public static ObservableCollection<DynamicBaseClass> DataReaderMapToCollection(object anonymousType, IDataReader dr, bool isRead = true)
        {
            var type = anonymousType.GetType();
            var list = new ObservableCollection<DynamicBaseClass>();

            var obj = (DynamicBaseClass) Activator.CreateInstance(type);
            var propList = obj.GetType().GetProperties().Where(IsAcceptableDbType).ToList();

            while (dr.Read())
            {
                obj = (DynamicBaseClass) Activator.CreateInstance(type);

                for (int i = 0; i < propList.Count; i++)
                {
                    if (!dr.IsDBNull(i))
                    {
                        propList[i].SetValue(obj, dr.GetValue(i), null);
                    }
                }

                list.Add(obj);
            }

            if (isRead == false)
            {
                propList[0].SetValue(obj, dr.RecordsAffected, null);
                list.Add(obj);
            }


            return list;
        }

        public static bool IsAcceptableDbType(PropertyInfo pi)
        {
            return (pi.PropertyType.BaseType ==
                       Type.GetType("System.ValueType") ||
                       pi.PropertyType ==
                       Type.GetType("System.String") ||
                       pi.PropertyType ==
                       Type.GetType("System.Byte[]")
                       );
        }



        public static DynamicCollection<Core.DynamicBaseClass> Select2(string selectCmd, string connectionString, Type stype = null)
        {
            DynamicCollection<Core.DynamicBaseClass> objList;
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(selectCmd, connection))
                {
                    command.CommandTimeout = 0;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        var schema = reader.GetSchemaTable();
                        string[] fields = null;
                        Type[] types = null;

                        if (schema == null)
                        {
                            fields = new string[1];
                            types = new Type[1];

                            fields[0] = "СтрокОбработано";
                            types[0] = typeof(int);
                            objList = DataReaderMapToCollection2(
                                    ProxyProvider.Instance.NewProxy2(CustomTypeBuilder.CompileResultType(fields, types)).GetType(),
                                    reader, false);
                        }
                        else if (stype == null)
                        {
                            fields = new string[schema.Rows.Count];
                            types = new Type[schema.Rows.Count];
                            int ci = 1;
                            for (int i = 0; i < schema.Rows.Count; i++)
                            {
                                var col = schema.Rows[i].Field<string>("ColumnName");
                                col = string.IsNullOrWhiteSpace(col)
                                    ? "Column" + ci++
                                    : fields.Any(x => x == col) ? col + fields.Count(x => x == col) : col;

                                fields[i] = col;
                                Type type = schema.Rows[i].Field<Type>("DataType");

                                if (schema.Rows[i].Field<bool>("AllowDBNull"))
                                    types[i] = type == typeof(string) || type == typeof(byte[])
                                        ? type
                                        : typeof(Nullable<>).MakeGenericType(type);
                                else
                                    types[i] = type;
                            }

                            var baseType = ProxyProvider.Instance.NewProxy2(CustomTypeBuilder.CompileResultType(fields, types)).GetType();
                            objList =
                                DataReaderMapToCollection2(baseType, reader);
                            objList.SetDynamicType(baseType);

                            fields = null;
                            types = null;
                        }
                        else
                        {
                            objList =
                                DataReaderMapToCollection2(stype, reader);
                            objList.SetDynamicType(stype);

                        }

                    }
                    connection.Close();
                }
            }
            return objList;
        }


        public static DynamicCollection<Core.DynamicBaseClass> DataReaderMapToCollection2(Type anonymousType, IDataReader dr, bool isRead = true)
        {
            var type = anonymousType;
            var list = new DynamicCollection<Core.DynamicBaseClass>();

            var obj = Activator.CreateInstance(type);
            var propList = type.GetProperties().Where(IsAcceptableDbType).ToList();

            while (dr.Read())
            {
                obj = Activator.CreateInstance(type);

                for (int i = 0; i < propList.Count; i++)
                {
                    if (!dr.IsDBNull(i))
                    {
                        propList[i].SetValue(obj, dr.GetValue(dr.GetOrdinal(propList[i].Name)), null);
                    }
                }

                list.Add((Core.DynamicBaseClass)obj);
            }

            if (isRead == false)
            {
                propList[0].SetValue(obj, dr.RecordsAffected, null);
                list.Add((Core.DynamicBaseClass)obj);
            }


            return list;
        }
    }
}
