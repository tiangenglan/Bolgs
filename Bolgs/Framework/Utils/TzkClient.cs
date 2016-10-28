using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Net.Cache;
using System.Threading;

namespace Gozyy.Framework
{

    public class TzkClient
    {
        const string FILE_PART_HEADER = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                                         "Content-Type: application/octet-stream\r\n\r\n";

        public static string SendByGet(string url, Dictionary<string, string> formDataDict,string secretKey)
        {
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            formDataDict.Add("app_sign", Md5Util.getSignature(formDataDict, secretKey));
            StringBuilder sb = new StringBuilder();
            foreach (var key in formDataDict)
            {
                if (sb.ToString() != "") sb.Append("&");
                sb.AppendFormat("{0}={1}", key.Key, key.Value);
            }
            
            url += "?" + sb.ToString();
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);

            CookieContainer myCookieContainer = GetCookies();
            webRequest.CookieContainer = myCookieContainer;
            webRequest.KeepAlive = false;

            webRequest.Method = "GET";
            //处理响应信息
            using (WebResponse myResp = webRequest.GetResponse())
            {
                Stream ReceiveStream = myResp.GetResponseStream();
                StreamReader readStream = new StreamReader(ReceiveStream, encode);
                Char[] read = new Char[256];
                int count = readStream.Read(read, 0, 256);
                string responseData = null;
                while (count > 0)
                {
                    responseData += new String(read, 0, count);
                    count = readStream.Read(read, 0, 256);
                }
                readStream.Close();
                //cookie操作
                var response = (HttpWebResponse)myResp;
                try
                {
                    for (int i = 0; i < response.Cookies.Count; i++)
                    {
                        if (HttpContext.Current.Response != null && response.Cookies[i].Value != null && response.Cookies[i].Value != "" &&
                            response.Cookies[i].Name != null && response.Cookies[i].Name != "")
                        {
                            System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"), "\r\n get-method: " + i.ToString() + " " + response.Cookies[i].Name + "\r\n");
                            System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"), "\r\n get-method: " + i.ToString() + " " + response.Cookies[i].Value + "\r\n");
                            if (HttpContext.Current.Response.Cookies[response.Cookies[i].Name].Value.Contains(",")) continue;
                            HttpContext.Current.Response.Cookies[response.Cookies[i].Name].Value = response.Cookies[i].Value;
                        }
                        //cookiestr += response.Cookies[i].Name + "=" + response.Cookies[i].Value.ToString()+"\r\n";
                    }
                }
                catch (Exception ee)
                {
                    System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"), "\r\n get-method: cookies set error: " + ee.Message + ee.StackTrace + "\r\n");
                }

                response.Close();
                response = null;
                myResp.Close();
                //返回响应信息
                return responseData;
            }
        }
        /// <summary>
        /// 获取当前Cookies
        /// </summary>
        /// <returns></returns>
        private static CookieContainer GetCookies()
        {
            CookieContainer myCookieContainer = new CookieContainer();
            try
            {
                HttpCookie requestCookie;
                int requestCookiesCount = HttpContext.Current.Request.Cookies.Count;
                //System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"), "\r\n [读取Cookie总数: ]" + requestCookiesCount.ToString() + " " + HttpContext.Current.Request.Url.Host + "\r\n");
                    
                for (int i = 0; i < requestCookiesCount; i++)
                {
                    requestCookie = HttpContext.Current.Request.Cookies[i];
                    //System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"), "\r\n [读取Cookie ]" + requestCookie.Name + "=" + requestCookie.Value + "[" + requestCookie.Path + "][" + requestCookie.Domain + "]\r\n");

                    if (requestCookie.Value.Contains(","))
                    {
                        //System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"), "\r\n [读取逗号Cookie,即将跳过  ]" + requestCookie.Name + "=" + requestCookie.Value + "\r\n");

                        continue;
                    }
                    if (requestCookie.Name=="" || requestCookie.Value == "")
                    {
                        //System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"), "\r\n [读取Cookie为空,即将跳过  ]" + requestCookie.Name + "=" + requestCookie.Value + "\r\n");

                        continue;
                    }
                    System.Net.Cookie clientCookie = new System.Net.Cookie(requestCookie.Name, requestCookie.Value, requestCookie.Path, requestCookie.Domain == null ? HttpContext.Current.Request.Url.Host : requestCookie.Domain);
                    //System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"), "\r\n [======1 " + "\r\n");

                    myCookieContainer.Add(clientCookie);
                    //System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"), "\r\n =====2  ]" + "\r\n");

                }
            }
            catch (Exception e)
            {
                System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"), "\r\n [读取Cookie时挂掉 ]" + e.Message + e.StackTrace + "\r\n");
                //System.IO.File.AppendAllText(@"F:\ssjwww\openapi\logs\error.txt", "\r\n [读取Cookie时挂掉 ]" + e.Message + e.StackTrace + "\r\n");
            }
            //System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"), "[Cookie设置完毕,返回 ]" + "\r\n");
            return myCookieContainer;
        }
        
        /**
         * 支持参数和附件的post请求
         * 
        **/
        public static string SendByPost(string url, Dictionary<string, string> formDataDict, string[] filePathArray, string secretKey)
        {
            System.GC.Collect();
            System.Net.ServicePointManager.DefaultConnectionLimit = 512;
            try
            {
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

                // 边界符  
                var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
                // 最后的结束符  
                var endBoundary = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

                CookieContainer myCookieContainer = GetCookies();
                HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                webRequest.CookieContainer = myCookieContainer;

                webRequest.Timeout = 10000;
                //webRequest.KeepAlive = false;
                webRequest.Method = "POST";
                webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

                var memStream = new MemoryStream();

                //添加文件信息
                addFileInfo(memStream, filePathArray, boundary);

                //生成sign并设置到参数里面
                formDataDict.Add("app_sign", Md5Util.getSignature(formDataDict, secretKey));
                //添加form参数信息
                addFormData(memStream, formDataDict, boundary);

                // 写入最后的结束边界符  
                memStream.Write(endBoundary, 0, endBoundary.Length);

                //webRequest.ContentLength = memStream.Length;  //系统会自动计算
                //这个在Post的时候，一定要加上，如果服务器返回错误，他还会继续再去请求，不会使用之前的错误数据，做返回数据
                webRequest.ServicePoint.Expect100Continue = false;

                HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                webRequest.CachePolicy = noCachePolicy;

                var requestStream = webRequest.GetRequestStream();

                //将post的信息设置到请求流里面
                memStream.Position = 0;
                var tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();

                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();

                string responseData = "";
                //处理响应信息
                //using (WebResponse myResp = webRequest.GetResponse())
                using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
                {
                    //var response = (HttpWebResponse)myResp;
                    if (response.StatusCode == HttpStatusCode.RequestTimeout)
                    {
                        System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"), "\r\n [请求超时]\r\n");
                        Thread.Sleep(1000);
                        return null;
                    }

                    Stream ReceiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(ReceiveStream, encode);
                    Char[] read = new Char[256];
                    int count = readStream.Read(read, 0, 256);

                    while (count > 0)
                    {
                        responseData += new String(read, 0, count);
                        count = readStream.Read(read, 0, 256);
                    }
                    readStream.Close();

                    //cookie操作


                    //string cookiestr = "cookiestr:";    //debug
                    try
                    {
                        //System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"), "\r\n cookies总数:" + response.Cookies.Count.ToString() + "\r\n");
                                
                        for (int i = 0; i < response.Cookies.Count; i++)
                        {
                            if (response.Cookies[i].Name == "ASP.NET_SessionId") continue;
                            if (HttpContext.Current.Response != null && response.Cookies[i].Value != null && response.Cookies[i].Value != "" &&
                                response.Cookies[i].Name != null && response.Cookies[i].Name != "")
                            {
                                //System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"), "\r\n " + i.ToString() + " name:" + response.Cookies[i].Name + "\r\n");

                                HttpContext.Current.Response.Cookies[response.Cookies[i].Name].Value = response.Cookies[i].Value;
                                //System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"), "\r\n " + i.ToString() + " value:" + response.Cookies[i].Value + "\r\n");
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"), "\r\n cookies set error: "+ee.Message+ee.StackTrace+"\r\n");
                    }
                    //debug
                    //System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/" + DateTime.Now.ToString("yyHHmmhhMMss") + ".txt"), cookiestr);
                    //
                    response.Close();
                    //返回响应信息
                }
                webRequest.Abort();
                webRequest = null;
                return responseData;
            }
            catch (Exception e)
            {
                System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("/logs/error.txt"),e.Message+ e.StackTrace+e.Source+e.TargetSite);
                var json = new Json("10001", "访问API时发生错误,EROR:175 "+e.Message+e.TargetSite);
                return json.ToString();
            }
        }

        /**
         * 把文件添加到post请求流里面
         * 
        **/
        private static void addFileInfo(MemoryStream memStream, string[] filePathArray, string boundary)
        {
            //没有文件的情况下，直接返回
            if (filePathArray == null || filePathArray.Length == 0)
            {
                return;
            }

            // 边界符  
            var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");

            //循环处理文件
            for (int i = 0; i < filePathArray.Length; i++)
            {
                FileStream fileStream = new FileStream(filePathArray[i], FileMode.Open, FileAccess.Read);

                FileInfo fileInfo = new FileInfo(filePathArray[i]);
                // 设置文件名路径和文件名称  
                string encodingFilePath = HttpUtility.UrlEncode(fileInfo.FullName, Encoding.UTF8).Replace("+", "%20");
                string encodingFileName = HttpUtility.UrlEncode(fileInfo.Name, Encoding.UTF8).Replace("+", "%20");
                var header = string.Format(FILE_PART_HEADER, encodingFilePath, encodingFileName);
                var headerbytes = Encoding.UTF8.GetBytes(header);   

                //设置每个文件的分界信息
                memStream.Write(beginBoundary, 0, beginBoundary.Length);
                //设置文件的头信息（包括文件名、文件路径）
                memStream.Write(headerbytes, 0, headerbytes.Length);

                //设置文件信息到post请求流里面
                var buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }
            }

        }
        /// <summary>
        /// 发送短信专用
        /// </summary>
        /// <param name="url"></param>
        /// <param name="formDataDict"></param>
        /// <returns></returns>
        public static string SendSMS(string url, string dictstr, Dictionary<string, string> formDataDict)
        {
            if (dictstr.Trim() != "")
            {
                url += HttpContext.Current.Server.UrlEncode(dictstr);
            }
            Encoding encode = System.Text.Encoding.GetEncoding("gb2312");

            // 边界符  
            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            // 最后的结束符  
            var endBoundary = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            
            webRequest.Method = "POST";
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

            var memStream = new MemoryStream();

            //添加form参数信息
            addFormData(memStream, formDataDict, boundary);

            // 写入最后的结束边界符  
            memStream.Write(endBoundary, 0, endBoundary.Length);

            webRequest.ContentLength = memStream.Length;

            var requestStream = webRequest.GetRequestStream();

            //将post的信息设置到请求流里面
            memStream.Position = 0;
            var tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();

            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            requestStream.Close();

            //处理响应信息
            using (WebResponse myResp = webRequest.GetResponse())
            {
                Stream ReceiveStream = myResp.GetResponseStream();
                StreamReader readStream = new StreamReader(ReceiveStream, encode);
                Char[] read = new Char[256];
                int count = readStream.Read(read, 0, 256);
                string responseData = null;
                while (count > 0)
                {
                    responseData += new String(read, 0, count);
                    count = readStream.Read(read, 0, 256);
                }
                //cookie操作
                var response = (HttpWebResponse)myResp;

                readStream.Close();
                myResp.Close();
                //返回响应信息
                return responseData;
            }

        }
        /**
         * 把参数添加到post请求流里面
         * 
        **/
        private static void addFormData(MemoryStream memStream, Dictionary<string, string> formDataDict, string boundary)
        {
            //没有form参数的情况下，直接返回
            if (formDataDict == null || formDataDict.Keys.Count == 0)
            {
                return;
            }

            // 写入字符串的Key  
            var stringKeyHeader = "\r\n--" + boundary +
                                   "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                   "\r\n\r\n{1}";

            //将需要提交的form的参数信息，设置到post请求流里面
            foreach (byte[] formitembytes in from string key in formDataDict.Keys
                                             select string.Format(stringKeyHeader, key, formDataDict[key])
                                                 into formitem
                                                 select Encoding.UTF8.GetBytes(formitem))
            {
                memStream.Write(formitembytes, 0, formitembytes.Length);
            }

        }

    }

}
