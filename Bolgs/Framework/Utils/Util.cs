using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Gozyy.Framework.Utils
{
    public static class Util
    {
        /// <summary>
        /// 获取当前主机域名  xxxx
        /// </summary>
        /// <returns></returns>
        public static string GetHost()
        {
            var host = HttpContext.Current.Request.Url.Host;
            var port = HttpContext.Current.Request.Url.Port;
            if (port != 80) return host + ":" + port.ToString();
            return host;
        }
        /// <summary>
        /// 判断是否是微信浏览器访问 
        /// </summary>
        /// <returns></returns>
        public static bool IsMQQBrowser()
        {
            try
            {
                string agent = HttpContext.Current.Request.UserAgent;
                if(agent.ToLower().Contains("mqqbrowser"))
                //if (agent.ToLower().Contains("micromessenger"))
                {
                    return true;
                }
            }
            catch
            { }
            return false;
        }
        /// <summary>
        /// 判断是否是微信浏览器访问
        /// </summary>
        /// <returns></returns>
        public static bool IsMicroMessenger()
        {
            try
            {
                string agent = HttpContext.Current.Request.UserAgent;
                if (agent.ToLower().Contains("micromessenger"))// || agent.ToLower().Contains("mqqbrowser"))
                {
                    return true;
                }
            }
            catch
            { }
            return false;
        }
        /// <summary>
        /// 获取当前主机域名  http://xxxxxx
        /// </summary>
        /// <returns></returns>
        public static string GetHttpHost()
        {
            var host = HttpContext.Current.Request.Url.Host;
            var port = HttpContext.Current.Request.Url.Port;
            if (port != 80) return "http://"+host + ":" + port.ToString();
            return "http://" + host;
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file">上传控件</param>
        /// <param name="path">放置路径，虚拟路径</param>
        /// <returns></returns>
        public static string UpFile(HttpPostedFileBase file, string path)
        {
            return UpFile(file, path, string.Empty);
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file">上传控件 </param>
        /// <param name="path">放置路径，虚拟路径</param>
        /// <param name="name">文件名：为空的话取当前时间命名</param>
        /// <returns></returns>
        public static string UpFile(HttpPostedFileBase file, string path, string name)
        {
            int fileSize = file.ContentLength / 1024; //上传文件的大小
            string filename = file.FileName;        //上传文件名
            string filexe = System.IO.Path.GetExtension(filename).ToLower(); //上传文件的扩展名
            string fname = DateTime.Now.ToString("yyyyMMddHHmmssfff");//Guid.NewGuid().ToString("N")
            string upfilename = (name == string.Empty ? fname + filexe : name + filexe);  //上传后加入到文件下的名称

            string upfilepath = HttpContext.Current.Server.MapPath(path);//上传文件路径
            DirectoryInfo dir = new DirectoryInfo(upfilepath);
            if (!dir.Exists)
            {
                dir.Create();
            }

            //若文件存在，则判断文件是否为只读状态
            if (System.IO.File.Exists(upfilepath + upfilename))
            {
                //获取文件实例
                System.IO.FileInfo file1 = new System.IO.FileInfo(upfilepath + upfilename);
                //文件属性为只读，则更改其属性为正常
                if (file1.Attributes.ToString().IndexOf("ReadOnly") != -1)
                {
                    file1.Attributes = System.IO.FileAttributes.Normal;
                }
            }

            file.SaveAs(upfilepath + upfilename);
            return path + upfilename;

        }
        /// <summary>
        /// 取得用户客户端IP(穿过代理服务器取远程用户真实IP地址)
        /// </summary>
        public static string GetClientIP()
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
    }
}