using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Gozyy.Repositories;
using System.Data;
namespace Gozyy.Framework
{
    /// <summary>
    /// 顶级抽象类,所有类都有继承此类,并实现 Invoke方法
    /// </summary>
    public abstract class TopApp
    {
        /// <summary>
        /// 虚方法,用来调度方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        //public abstract string Invoke(string method);

        public string Invoke(string method)
        {
            var methodInfo = this.GetType().GetMethod(method, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo == null)
            {
                return Json.GetString("10001", "系统错误,调用的方法不存在");
            }
            if (methodInfo == null)
            {
                return Json.GetString("10001", "系统错误,调用的方法不存在");
            }
            try
            {
                //根据参数匹配调用
                object[] paras = null;
                var param = methodInfo.GetParameters();
                if (param.Length > 0)
                {
                    paras = new object[param.Length];
                    int i = 0;
                    foreach (var o in param)
                    {
                        var pv = GetString(o.Name);
                        if (HttpContext.Current.Request.QueryString[o.Name] == null &&
                            HttpContext.Current.Request.Form[o.Name] == null)
                        {
                            pv = o.DefaultValue.ToString();
                        }
                        switch (o.ParameterType.ToString())
                        {
                            case "System.String":
                                paras[i] = pv;
                                break;
                            case "System.Int32":
                                paras[i] = System.Int32.Parse(pv);
                                break;
                            case "System.Double":
                                paras[i] = System.Double.Parse(pv);
                                break;
                            case "System.Decimal":
                                paras[i] = System.Decimal.Parse(pv);
                                break;
                            default:
                                paras[i] = pv;
                                break;
                        }
                        i++;
                    }
                }
                //返回值
                var result = "";
                switch (methodInfo.ReturnType.Name)
                {
                    case "Int32":
                        result = (string)methodInfo.Invoke(this, paras);
                        break;
                    case "String":
                        result = (string)methodInfo.Invoke(this, paras);    //
                        if (result != "" && result.Substring(0, 1) != "{")  //假如不是JSON格式
                        {
                            result = Json.GetString("100", "获取信息成功", result);
                        }
                        break;
                    case "DataTable":
                        var json = new Json("100", "获取成功");
                        var dt = methodInfo.Invoke(this, paras);
                        if (dt == null)
                        {
                            result = (new Json("103", "没有找到信息")).ToString();
                        }
                        else
                        {
                            json.AddToResult((DataTable)dt);
                            result = json.ToString();
                        }
                        break;
                    case "List`1":
                        var r =methodInfo.Invoke(this, paras);
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        var m = serializer.Serialize(r);
                        result = Json.GetListResult("100", "操作成功",m);
                        break;
                    default:
                        result = Json.GetString("104", "不能识别的返回值格式");
                        break;
                }

                return result;
            }
            catch (System.ArgumentException ea)
            {
                return Json.GetString("10001", "参数错误,请检查后重试", ea.Message);
            }
            catch (Exception e)
            {
                return Json.GetString("10001", "调用API时发生错误", e.Message);
            }
        }
        
        /// <summary>
        /// 取得用户客户端IP(穿过代理服务器取远程用户真实IP地址)
        /// </summary>
        public  string GetClientIP()
        {
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            {
                return System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else
            {
                return System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="cvalue"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public  string SetCookie(string cookie, string cvalue, int days)
        {
            System.Web.HttpContext.Current.Response.Cookies[cookie].Value = Security.DESEncrypt(cvalue);
            System.Web.HttpContext.Current.Response.Cookies[cookie].Expires = DateTime.Now.AddDays(days);
            //HttpContext.Current.Response.Cookies[cookie].Path = "/";
            return cookie;
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="cvalue"></param>
        /// <returns></returns>
        public  string SetCookie(string cookie, string cvalue)
        {
            System.Web.HttpContext.Current.Response.Cookies[cookie].Value = Security.DESEncrypt(cvalue);
            //HttpContext.Current.Response.Cookies[cookie].Path = "/";
            return cookie;
        }
        /// <summary>
        /// 得到Cookie
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public  string GetCookie(string cookie)
        {
            if (System.Web.HttpContext.Current.Request.Cookies[cookie] != null && System.Web.HttpContext.Current.Request.Cookies[cookie].Value!="")
            {
                //return System.Web.HttpContext.Current.Request.Cookies[cookie].Value;
                return Security.DESDecrypt(System.Web.HttpContext.Current.Request.Cookies[cookie].Value);
            }
            return "";
        }
        /// <summary>
        /// 验证必须的参数 
        /// </summary>
        /// <param name="paramstr">param1,param2</param>
        /// <returns></returns>
        public  bool ParamNullValid(string paramstr)
        {
            string[] ps = paramstr.Split(',');
            for (int i = 0; i < ps.Length; i++)
            {
                if (HttpContext.Current.Request.QueryString[ps[i]] == null &&
                    HttpContext.Current.Request.Form[ps[i]] == null)
                {
                    return false;
                }
                //if(HttpContext.Current.Request.QueryString[ps[i]] != null &&
                //if ((HttpContext.Current.Request.QueryString[ps[i]] == null &&
                //    HttpContext.Current.Request.Form[ps[i]] == null)||(
                //    HttpContext.Current.Request.QueryString[ps[i]].ToString()=="") &&
                //    HttpContext.Current.Request.Form[ps[i]].ToString() == ""
                //    )
                //{
                //    //HttpContext.Current.Response.Write(new Json("102", "缺少必须的参数"));
                //    //HttpContext.Current.Response.End();
                //    return false;
                //}
            }
            return true;
        }
        /// <summary>
        /// 获得参数，Query和Form通用 INT
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public  int GetInt(string key, int defValue)
        {
            int val = GetQueryInt(key, defValue);
            if (val == defValue)
            {
                val = GetFormInt(key, defValue);
            }
            return val;
        }
        /// <summary>
        /// 获得参数，Query和Form通用 Decimal
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public  Double GetDouble(string key, Double defValue)
        {
            Double val = GetQueryDouble(key, defValue);
            if (val == defValue)
            {
                val = GetFormDouble(key, defValue);
            }
            return val;
        }
        /// <summary>
        /// 获得Form参数INT
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public  int GetFormInt(string key, int defValue)
        {
            string val = HttpContext.Current.Request.Form[key];
            if (null == val) { return defValue; }
            int rst = 0;
            if (int.TryParse(val, out rst))
            {
                return rst;
            }
            return defValue;
        }
        /// <summary>
        /// 获得Form参数Double
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public  Double GetFormDouble(string key, Double defValue)
        {
            string val = HttpContext.Current.Request.Form[key];
            if (null == val) { return defValue; }
            Double rst = 0.00;
            if (Double.TryParse(val, out rst))
            {
                return rst;
            }
            return defValue;
        }
        /// <summary>
        /// 获得QeryString参数STRING
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public  string GetQueryString(string key)
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
        public  int GetQueryInt(string key, int defValue)
        {
            string val = HttpContext.Current.Request.QueryString[key];
            if (null == val) { return defValue; }
            int rst = 0;
            if (int.TryParse(val, out rst))
            {
                return rst;
            }
            return defValue;
        }
        /// <summary>
        /// 获得QeryString参数Double
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public  Double GetQueryDouble(string key, Double defValue)
        {
            string val = HttpContext.Current.Request.QueryString[key];
            if (null == val) { return defValue; }
            Double rst = 0.00;
            if (Double.TryParse(val, out rst))
            {
                return rst;
            }
            return defValue;
        }

        /// <summary>
        /// 获得参数，Query和Form通用 STRING
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public  string GetString(string key)
        {
            string val = HttpContext.Current.Request.QueryString[key];

            if (val != null && val != "")
            {
                val = Filter(val);
            }
            if (string.IsNullOrEmpty(val))
            {
                val = HttpContext.Current.Request.Form[key];
            }
            if (null == val) return string.Empty;
            return val;
        }
        public  string Filter(string val)
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

            //val = val.Replace("char(32)", "&nbsp;");
            //val = val.Replace("char(9)", "&nbsp;");
            //val = val.Replace("char(10)", "<br/>");
            //val = val.Replace("char(13)", "");
            //val = val.Replace("<", "&lt;");
            //val = val.Replace(">", "&gt;");
            //val = val.Replace("char(34)", "&quot;");
            //val = val.Replace(" ", "&nbsp;");
            //val = val.Replace("char(39)", "&#39;");
            return val;
        }

        /// <summary>
        /// 获得Form参数 STRING
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public  string GetFormString(string key)
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
        public  string GetFormRawString(string key)
        {
            string val = HttpContext.Current.Request.Form[key];

            //if (val != null && val != "")
            //{
            //    val =  val ;
            //}

            if (null == val) return string.Empty;
            return val;
        }
       
        /// <summary>
        /// 获取方法列表
        /// </summary>
        /// <param name="isousoujiu"></param>
        /// <returns></returns>
        public  string[] getMethodList()
        {
            int i, l;
            MethodInfo myMethodInfo;
            MethodInfo[] myArrayMethodInfo = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            l = myArrayMethodInfo.Length;
            string[] methodList = new string[l];
            for (i = 0; i < myArrayMethodInfo.Length; i++)
            {
                myMethodInfo = (MethodInfo)myArrayMethodInfo[i];
                methodList[i] = myMethodInfo.Name;
            }
            return methodList;
        }
        /// <summary>
        /// 只能输入数字："^[0-9]*$"
        /// 只能输入n位的数字："^\d{n}$"
        /// 只能输入至少n位的数字："^\d{n,}$"
        /// 只能输入m~n位的数字："^\d{m,n}$"
        /// </summary>
        /// <param name="regexvalue"></param>
        /// <param name="itemvalue"></param>
        /// <returns></returns>
        public  bool IsRegex(string regexvalue, string itemvalue)
        {
            try
            {
                Regex regex = new System.Text.RegularExpressions.Regex(regexvalue);
                if (regex.IsMatch(itemvalue))
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally { }
        }
    }
}
