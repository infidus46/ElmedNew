using System.Collections;
using System.Linq;
using Yamed.Entity;

namespace Yamed.Server
{
    public class DBaseInfo
    {
        public static object GetColumnInfo(string table, string pk, string connectionString)
        {
            var info = (IList)
                Reader2List.CustomAnonymousSelect(
                    $@"
                select o.name t_name, c.name c_name, c.is_identity
                from sys.objects o
                inner
                join sys.columns c on o.object_id = c.object_id
                where o.name = '{table}' and c.name = '{pk}'", connectionString);
            return info;
        }

        public static DBaseInfoClass.PrimaryKeyColumn GetPrimaryKeyInfo(string table, string connectionString)
        {
            var pkCol =
                Reader2List.CustomSelect<DBaseInfoClass.PrimaryKeyColumn>(
                    $@"
                SELECT Col.Column_Name from
                    INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab,
                    INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Col
                WHERE
                    Col.Constraint_Name = Tab.Constraint_Name
                    AND Col.Table_Name = Tab.Table_Name
                    AND Constraint_Type = 'PRIMARY KEY'
                    AND Col.Table_Name = '{table}'", connectionString).Single();
            return pkCol;
        }
    }
}
