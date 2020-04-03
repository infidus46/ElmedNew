namespace Yamed.Server
{
    public class DBaseInfoClass
    {
        public class PrimaryKeyColumn
        {
            public string Column_Name { get; set; }
        }

        public class ListExtendedProperty
        {
            public string objtype { get; set; }
            public string objname { get; set; }
            public string name { get; set; }
            public string value { get; set; }
        }

        public class ColumnsInformationSchema
        {
            public string TABLE_NAME { get; set; }
            public string COLUMN_NAME { get; set; }
            public int ORDINAL_POSITION { get; set; }
            public string COLUMN_DEFAULT { get; set; }
            public string IS_NULLABLE { get; set; }
            public string DATA_TYPE { get; set; }
        }

        public class TablesInformationSchema
        {
            public string TABLE_CATALOG { get; set; }
            public string TABLE_SCHEMA { get; set; }
            public string TABLE_NAME { get; set; }
            public string TABLE_TYPE { get; set; }
        }

        public class ForeignKeyColumns
        {
            public string ForeignKey { get; set; }
            public string TableName { get; set; }
            public string ColumnName { get; set; }
            public string ReferenceTableName { get; set; }
            public string ReferenceColumnName { get; set; }
        }
    }
}
