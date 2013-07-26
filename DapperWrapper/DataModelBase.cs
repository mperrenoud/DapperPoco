using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
    public abstract class DataModelBase<T> where T : DataModelBase<T>
    {
        public readonly string TableName, PrimaryKey;
        public readonly List<string> DataFields;
        public readonly string Select, Insert, Update, Delete;

        protected readonly string ConnectionString;
        protected readonly PropertyInfo PrimaryKeyInfo;
        protected readonly List<PropertyInfo> DataFieldInfo;

        private static T _instance;
        private static T Instance
        {
            get
            {
                if (_instance == null) { _instance = Activator.CreateInstance<T>(); }
                return _instance;
            }
        }

        public DataModelBase()
            : this(string.Empty)
        {

        }

        public DataModelBase(string connectionString)
        {
            this.ConnectionString = connectionString;

            // set the table name
            var dataTableAttrib = this.GetType().GetCustomAttribute<DataTableAttribute>();
            if (dataTableAttrib == null) { throw new InvalidOperationException("A data model must have a DataTableAttribute defined."); }
            this.TableName = dataTableAttrib.TableName;

            // find the data fields
            this.DataFieldInfo = this.GetType().GetProperties().Where(p => p.GetCustomAttribute<DataFieldAttribute>() != null).ToList();
            this.DataFields = this.DataFieldInfo.Select(df => df.Name).ToList();

            // find the primary key
            var pkList = this.DataFieldInfo.Where(p =>
            {
                var attrib = p.GetCustomAttribute<DataFieldAttribute>();
                if (attrib == null) { return false; }
                return attrib.IsPrimaryKey;
            });
            if (pkList.Count() == 0) { throw new InvalidOperationException("A data model must have a single primary key."); }
            else if (pkList.Count() > 1) { throw new InvalidOperationException("A data model can have only a single primary key."); }
            this.PrimaryKeyInfo = pkList.First();
            this.PrimaryKey = this.PrimaryKeyInfo.Name;

            Select = string.Format("SELECT {0} FROM {1}",
                string.Join(", ", this.DataFields.Select(df => df.Bracket()).ToArray()),
                this.TableName.Bracket());

            Insert = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                this.TableName.Bracket(),
                string.Join(", ", this.DataFields.Except(new[] { this.PrimaryKey }).Select(df => df.Bracket()).ToArray()),
                string.Join(", ", this.DataFields.Except(new[] { this.PrimaryKey }).Select(df => df.Parameterize()).ToArray()));

            Update = string.Format("UPDATE {0} SET {1}",
                this.TableName.Bracket(),
                string.Join(", ", this.DataFields.Except(new[] { this.PrimaryKey }).Select(df => string.Format("{0} = {1}", df.Bracket(), df.Parameterize()))));

            Delete = string.Format("DELETE FROM {0}", this.TableName.Bracket());
        }

        public TValue Field<TValue>(string field)
        {
            var prop = this.GetType().GetProperty(field);
            if (prop == null) { return default(TValue); }
            else if (prop.PropertyType != typeof(TValue)) { return default(TValue); }

            var val = prop.GetValue(this);
            if (val == null) { return default(TValue); }

            return (TValue)val;
        }

        public static IEnumerable<T> Query(string sql = null, object param = null, params string[] filter)
        {
            if (sql == null) { sql = Instance.Select; }
            sql = sql.Filter(filter);

#if DEBUG
            using (SqlCeConnection c = new SqlCeConnection(Instance.ConnectionString))
            {
                c.Open();
                return c.Query<T>(sql, param);
            }
#else
            using (SqlConnection c = new SqlConnection(Instance.ConnectionString))
            {
                c.Open();
                return c.Query<T>(sql, param);
            }
#endif
        }

        public static int Execute(string sql, object param = null, params string[] filter)
        {
            sql = sql.Filter(filter);

#if DEBUG
            using (SqlCeConnection c = new SqlCeConnection(Instance.ConnectionString))
            {
                c.Open();
                return c.Execute(sql, param);
            }
#else
            using (SqlConnection c = new SqlConnection(Instance.ConnectionString))
            {
                c.Open();
                return c.Execute(sql, param);
            }
#endif
        }
    }
}
