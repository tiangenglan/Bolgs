using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Text.RegularExpressions;
namespace Gozyy.Repositories
{
    public class Model : System.Collections.Hashtable
    {
        public Model(string tbname)
            : base(System.StringComparer.Create(System.Globalization.CultureInfo.CurrentCulture, true))
        {
            
            this["_name"] = tbname;
            this["_fields"] = "*";
        }
        public Model(string tbname, string fields)
            : base(System.StringComparer.Create(System.Globalization.CultureInfo.CurrentCulture, true))
        {
            this["_name"] = tbname;
            this["_fields"] = fields;
        }

        /// <summary>
        /// 参数应匹配
        /// </summary>
        /// <param name="where">name like @name and age >= @age</param>
        /// <param name="args">%张%,2</param>
        public void Where(string where, params object[] args)
        {
            this["_where"] = where;
            this["_args"] = args;

            //参数名称
            MatchCollection mc;
            Regex r = new Regex("@[\\w.]+");
            mc = r.Matches(where);
            string[] _params = new string[mc.Count];
            for (int i = 0; i < mc.Count; i++)
            {
                _params[i] = mc[i].Value;
            }
            this["_params"] = _params;
        }

    }
}
