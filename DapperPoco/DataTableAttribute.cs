using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DataTableAttribute : Attribute
    {
        readonly string _tableName;

        public DataTableAttribute(string tableName)
        {
            this._tableName = tableName;
        }

        public string TableName
        {
            get { return _tableName; }
        }
    }
}
