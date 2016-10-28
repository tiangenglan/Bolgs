using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gozyy.Framework
{
    public static class Cookie
    {
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="cvalue"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public static string SetCookie(string cookie, string cvalue, int days)
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
        public static string SetCookie(string cookie, string cvalue)
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
        public static string GetCookie(string cookie)
        {
            if (System.Web.HttpContext.Current.Request.Cookies[cookie] != null)
            {
                return Security.DESDecrypt(System.Web.HttpContext.Current.Request.Cookies[cookie].Value);
            }
            return "";
        }
    }
}