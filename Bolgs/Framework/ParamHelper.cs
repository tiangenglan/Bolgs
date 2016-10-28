using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gozyy.Framework
{
    /// <summary>
    /// 参数获取帮助类
    /// </summary>
    public class ParamHelper
    {
        /// <summary>
        /// 过滤字符串
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string Filter(string val)
        {
            val = val.Replace("'", "’");
            val = val.Replace("char(94)", "");
            val = val.Replace("char(124)", "");
            val = val.Replace("IS_SRVROLEMEMBER", "");
            val = val.Replace("And Cast(IS_SRVROLEMEMBER(0x730079007300610064006D0069006E00)", "");
            val = val.Replace("sys_", "");
            val = val.Replace("sp_", "");
            val = val.Replace("exec(", "");
            val = val.Replace("xor", "");
            val = val.Replace("xp_cmdshell", "");
            val = val.Replace("delete", "");
            val = val.Replace("insert", "");
            val = val.Replace("join", "");
            val = val.Replace("sp_oacreate", "");
            val = val.Replace("wscript.shell", "");
            val = val.Replace("xp_regwrite", "");
            val = val.Replace("declare", "");

            val = val.Replace("char(32)", "&nbsp;");
            val = val.Replace("char(9)", "&nbsp;");
            val = val.Replace("char(10)", "<br/>");
            val = val.Replace("char(13)", "");
            val = val.Replace("<", "&lt;");
            val = val.Replace(">", "&gt;");
            val = val.Replace("char(34)", "&quot;");
            val = val.Replace(" ", "&nbsp;");
            val = val.Replace("char(39)", "&#39;");
            return val;
        }
        /// <summary>
        /// 获得QeryString参数STRING
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetQueryString(string key)
        {
            string val = HttpContext.Current.Request.QueryString[key];

            if (val != null && val != "")
            {
                val = Filter(val);
            }

            if (null == val) return string.Empty;
            return val;
        }

        /// <summary>
        /// 获得QeryString参数INT
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetQueryInt(string key, int defValue)
        {
            string val = HttpContext.Current.Request.QueryString[key];
            if (null == val) { return defValue; }
            int.TryParse(val, out defValue);
            return defValue;
        }


        /// <summary>
        /// 获得QeryString参数DATETIEM
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static DateTime GetQueryDateTime(string key, DateTime defValue)
        {
            string val = HttpContext.Current.Request.QueryString[key];
            if (null == val) { return defValue; }
            DateTime.TryParse(val, out defValue);
            return defValue;
        }
        /// <summary>
        /// 获得参数，Query和Form通用 STRING
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            string val = GetQueryString(key);

            if (val != null && val != "")
            {
                val = Filter(val);
            }
            if (string.IsNullOrEmpty(val))
            {
                val = GetFormString(key);
            }
            return val;
        }
        public static string GetNoFilterString(string key)
        {
            string val = GetQueryString(key);

            if (val != null && val != "")
            {
                return  val;
            }
            if (string.IsNullOrEmpty(val))
            {
                val = GetFormString(key);
            }
            return val;
        }
        /// <summary>
        /// 获得参数，Query和Form通用 INT
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetInt(string key, int defValue)
        {
            int val = GetQueryInt(key, defValue);
            if (val == defValue)
            {
                val = GetFormInt(key, defValue);
            }
            return val;
        }
        /// <summary>
        /// 获得Form参数 STRING
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetFormString(string key)
        {
            string val = HttpContext.Current.Request.Form[key];

            if (val != null && val != "")
            {
                val = Filter(val);
            }

            if (null == val) return string.Empty;
            return val;
        }
        /// <summary>
        /// 获得Form参数[不过滤] STRING
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetFormRawString(string key)
        {
            string val = HttpContext.Current.Request.Form[key];

            if (val != null && val != "")
            {
                return val;
            }

            if (null == val) return string.Empty;
            return val;
        }
        /// <summary>
        /// 获得Form参数INT
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static int GetFormInt(string key, int defValue)
        {
            string val = HttpContext.Current.Request.Form[key];
            if (null == val) { return defValue; }
            int.TryParse(val, out defValue);
            return defValue;
        }
    }
}