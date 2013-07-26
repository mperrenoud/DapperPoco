using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class StringExtensions
    {
        public static string Bracket(this string s)
        {
            if (string.IsNullOrEmpty(s)) { return s; }
            return string.Format("[{0}]", s);
        }

        public static string Parameterize(this string s)
        {
            if (string.IsNullOrEmpty(s)) { return s; }
            return string.Format("@{0}", s);
        }

        public static string Filter(this string s, params string[] fields)
        {
            if (fields == null || fields.Length == 0)
            {
                return s;
            }

            return string.Format("{0} WHERE {1}", s,
                string.Join(" AND ", fields.Select(p => string.Format("{0} = {1}", p.Bracket(), p.Parameterize()))));
        }
    }
}
