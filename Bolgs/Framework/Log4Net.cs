using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Gozyy.Framework
{
    public static  class Log4Net
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Loggering");
        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Debug(string msg)
        {
            msg = "\r\n访问IP:" + GetReqIP() + "\r\n访问路径:" + GetReqUrl() + "\r\n" + msg;
            log.Debug("Debug " + msg);
        }
        /// <summary>
        /// 消息日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Info(string msg)
        {
            msg = "\r\n访问IP:" + GetReqIP() + "\r\n访问路径:" + GetReqUrl() + "\r\n" + msg;
            log.Info("Info " + msg);
        }
        /// <summary>
        /// 消息日志
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        public static void Info(Type t, string msg)
        {
            msg = "\r\n访问IP:" + GetReqIP() + "\r\n访问路径:" + GetReqUrl() + "\r\n" + msg;
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Info("Info " + msg);
        }

        /// <summary>
        /// 告警日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Warn(string msg)
        {
            msg = "\r\n访问IP:" + GetReqIP() + "\r\n访问路径:" + GetReqUrl() + "\r\n" + msg;
            log.Warn("Warn "+ msg);
        }
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string msg)
        {
            msg = "\r\n访问IP:" + GetReqIP() + "\r\n访问路径:" + GetReqUrl() + "\r\n" + msg;
            log.Error(msg);
        }
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        public static void Error(Type t, Exception ex)
        {
            string msg = "\r\n访问IP:" + GetReqIP() + "\r\n访问路径:" + GetReqUrl() + "\r\n";
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Error("Error ", ex);
        }
        /// <summary>
        /// 错误日志+自定义方法
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void Error(Type t,string msg, Exception ex)
        {
            msg = "\r\n访问IP:" + GetReqIP() + "\r\n访问路径:" + GetReqUrl() + "\r\n" + msg;
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Error("Error "+msg, ex);
        }
        /// <summary>
        /// 致命错误
        /// </summary>
        /// <param name="msg"></param>
        public static void Fatal(string msg)
        {
            msg = "\r\n访问IP:" + GetReqIP() + "\r\n访问路径:" + GetReqUrl() + "\r\n"+msg;
            log.Fatal(msg);
        }
        /// <summary>
        /// 致命错误
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        public static void Fatal(Type t, Exception ex)
        {
            string msg =  "\r\n访问IP:" + GetReqIP() + "\r\n访问路径:" + GetReqUrl() + "\r\n";
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Fatal("Fatal "+msg, ex);
        }
        /// <summary>
        /// 致命错误+自定义方法
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void Fatal(Type t, string msg, Exception ex)
        {
            msg = "\r\n访问IP:" + GetReqIP() + "\r\n访问路径:" + GetReqUrl()+"\r\n"+msg;
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Fatal("Fatal " + msg, ex);
        }
        /// <summary>
        /// 取得用户客户端IP(穿过代理服务器取远程用户真实IP地址)
        /// </summary>
        private static  string GetReqIP()
        {
            try
            {
                if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
                else
                {
                    return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                }
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 获取请求的路径
        /// </summary>
        /// <returns></returns>
        private static  string GetReqUrl()
        {
            try
            {
                if (HttpContext.Current != null &&
                    HttpContext.Current.Request != null)
                {
                    string refurl = "";
                    string url = "";
                    if (HttpContext.Current.Request.UrlReferrer != null)
                    {
                        refurl = HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
                    }
                    if (HttpContext.Current.Request.Url.AbsoluteUri != null)
                    {
                        url = HttpContext.Current.Request.Url.AbsoluteUri;
                    }
                    return url +"  "+ refurl;
                    //return HttpContext.Current.Request.Url.AbsoluteUri + "\r\n" + HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
                }
            }
            catch {}
            return "";
        }
    }
}