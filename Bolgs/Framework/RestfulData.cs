using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gozyy.Framework
{
    /// <summary>
    /// 用于API数据交换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RestfulData<T>
    {
        public int code = 0;
        public string msg = "";
        public string extra="";
        public List<T> result =new List<T>();

        public RestfulData()
        {
            code = 100;
            msg = "";
            extra = "";
        }
        public RestfulData(int code, string msg)
        {
            this.code = code;
            this.msg = msg;
        }
        public RestfulData(int code, string msg,string extra)
        {
            this.code = code;
            this.msg = msg;
            this.extra = extra;
        }
        /// <summary>
        /// 返回本实例JSON格式
        /// </summary>
        /// <returns></returns>
        public string ToJSON()
        {
            var rst = JsonConvert.SerializeObject(this);
            return rst;
        }
    }
}