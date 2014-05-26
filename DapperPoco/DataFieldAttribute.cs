using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DataFieldAttribute : Attribute
    {
        public DataFieldAttribute()
        {

        }

        public bool IsPrimaryKey { get; set; }
    }
}
