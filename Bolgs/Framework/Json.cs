using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;
namespace Gozyy.Framework
{
    public class Json
    {
        private Dictionary<string, string> _nodes = new System.Collections.Generic.Dictionary<string, string>();
        private List<Json> _jsons = null;
        public Json() { }
        /// <summary>
        /// 直接返回JSON字符串
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string GetString(string code,string msg)
        {
            return (new Json(code,msg)).ToString();
        }
        /// <summary>
        /// 直接返回JSON字符串
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <param name="extra"></param>
        /// <returns></returns>
        public static string GetString(string code, string msg,string extra)
        {
            return (new Json(code, msg,extra)).ToString();
        }
        /// <summary>
        /// 此方式一般是将List转成JSON后调用
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <param name="jsonstr"></param>
        /// <returns></returns>
        public static string GetListResult(string code, string msg, string jsonstr)
        {
            if (jsonstr.Trim() == "") jsonstr = "\"\"";
            else if (jsonstr.Substring(0, 1) != "{" &&
                jsonstr.Substring(0, 1) != "[")
            {
                jsonstr = "\"" + jsonstr + "\"";
            }
            StringBuilder str = new StringBuilder();
            str.AppendLine("{\"code\":" + code + ",\"msg\":\"" + msg + "\",\"result\":" + jsonstr + "}");
            return str.ToString();
        }
        public Json(string code, string msg)
        {
            //_nodes.Add("errcode", code);
            //2014-1-10 陆东冉改动
            _nodes.Add("code", code);
            _nodes.Add("msg", msg);
        }
        public Json(string code, string msg,string extra)
        {
            //_nodes.Add("errcode", code);
            //2014-1-10 陆东冉改动
            _nodes.Add("code", code);
            _nodes.Add("msg", msg);
            _nodes.Add("extra", extra);
        }
        /// <summary>
        /// 添加键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void Add(string key, string val)
        {
            if (!_nodes.ContainsKey(key))
            {
                _nodes.Add(key, "" + val + "");
            }
            else
            {
                _nodes[key] = val;
            }
        }
        /// <summary>
        /// 添加DataTable的键值对:  "key":[{"key":"val"},....]
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dt"></param>
        public void Add(string key, DataTable dt)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i > 0) str.Append(",");
                str.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (j > 0) str.Append(",");
                    str.AppendFormat("\"{0}\":\"{1}\"",dt.Columns[j].ColumnName.ToString().ToLower(), dt.Rows[i][j].ToString());
                }
                str.Append("}");
            }
            if (!_nodes.ContainsKey(key))
            {
                _nodes.Add(key, "["+str.ToString()+"]");
            }
            else
            {
                _nodes.Remove(key);
                _nodes.Add(key, "[" + str.ToString() + "]");
            }
        }
        /// <summary>
        /// 表示此json将需要result元素
        /// </summary>
        public void AddToResult()
        {
            if (_jsons == null) _jsons = new System.Collections.Generic.List<Json>();
        }
        /// <summary>
        /// 向result键中添加键值对: "result":[{"key":"val"........}]
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void AddToResult(string key, string val)
        {
            Json js = new Json();
            if (_jsons == null) _jsons = new System.Collections.Generic.List<Json>();

            if (_jsons.Count > 0)
            {
                js = _jsons[0];
                js.Add(key, val);
            }
            else
            { 
                js.Add(key, val);
                _jsons.Add(js);
            }
            
        }
        /// <summary>
        /// 向result键中添加添加DataTable的键值对: "result":[{"key":[{"key":"val"},....]}]
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void AddToResult(int index,string key, string val)
        {
            Json js = new Json();
            if (_jsons == null) _jsons = new System.Collections.Generic.List<Json>();

            if (_jsons.Count > index)
            {
                js = _jsons[index];
                js.Add(key, val);
            }
            else
            {
                js.Add(key, val);
                _jsons.Add(js);
            }
        }
        public void AddToResult(string key, Json json)
        {
            int index = 0;
            Json js = new Json();
            if (_jsons == null) _jsons = new System.Collections.Generic.List<Json>();

            if (_jsons.Count > index)
            {
                js = _jsons[index];
                js.Add(key, "[" + json.ToString() + "]");
            }
            else
            {
                js.Add(key, "[" + json.ToString() + "]");
                _jsons.Add(js);
            }
        }
        public void AddToResult(int index, string key, Json json)
        {
            Json js = new Json();
            if (_jsons == null) _jsons = new System.Collections.Generic.List<Json>();

            if (_jsons.Count > index)
            {
                js = _jsons[index];
                js.Add(key, "["+json.ToString()+"]");
            }
            else
            {
                js.Add(key, "[" + json.ToString() + "]");
                _jsons.Add(js);
            }
        }
        /// <summary>
        /// 向result键中添加添加DataTable的键值对: "result":[{"key":[{"key":"val"},....]}]
        /// </summary>
        /// <param name="dt"></param>
        public void AddToResult(DataTable dt)
        {
            if (_jsons == null) _jsons = new System.Collections.Generic.List<Json>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var js = new Json();
                for (int j = 0; j < dt.Columns.Count; j++)
                { 
                    js.Add(dt.Columns[j].ColumnName.ToString().ToLower(), dt.Rows[i][j].ToString());
                }
                _jsons.Add(js);
            }
            
        }
        /// <summary>
        /// 向result键中添加添加DataTable的键值对: "result":[{"key":[{"key":"val"},....]}]
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dt"></param>
        public void AddToResult(string key,DataTable dt)
        {
            if (_jsons == null) _jsons = new System.Collections.Generic.List<Json>();

            Json js = new Json();
            if (_jsons.Count > 0)
            {
                js = _jsons[0];
                js.Add(key, dt);
            }
            else
            {
                js.Add(key, dt);
                _jsons.Add(js);
            }
        }
        /// <summary>
        /// 向result键中添加添加DataTable的键值对: "result":[{"key":[{"key":"val"},....]}]
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <param name="dt"></param>
        public void AddToResult(int index,string key, DataTable dt)
        {
            if (_jsons == null) _jsons = new System.Collections.Generic.List<Json>();

            Json js = new Json();
            if (_jsons.Count > index)
            {
                js = _jsons[index];
                js.Add(key, dt);
            }
            else
            {
                js.Add(key, dt);
                _jsons.Add(js);
            }
        }
        /// <summary>
        /// 返回JSON结果
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "{" + GetKeyValString(); 
            StringBuilder str = new StringBuilder();
            if (_jsons != null)
            {
                if (_jsons.Count > 0)
                {
                    if (result != "{") result += ",";
                    result += "\"result\":[";
                    for (int i = 0; i < _jsons.Count; i++)
                    {
                        var js = _jsons[i];
                        if (i > 0) result += ",";
                        result += js.ToString();
                    }
                    result += "]";
                }
                else
                {
                    if (result != "{") result += ",";
                    result += "\"result\":[]";
                }
            }
            result += "}";
            return result;
        }
        /// <summary>
        /// 返回键值对的字符串形式
        /// </summary>
        /// <returns></returns>
        private  string GetKeyValString()
        {
            string result = "";
            StringBuilder str = new StringBuilder();
            if (_nodes != null && _nodes.Count>0)
            {
                foreach (KeyValuePair<string, string> kvp in _nodes)
                {
                    if (str.ToString() != "") str.Append(",");
                    if (kvp.Value != "" && kvp.Value.Substring(0, 1) == "[")
                    {
                        str.Append("\"" + kvp.Key + "\":" + kvp.Value + "");
                    }
                    else
                    {
                        str.Append("\"" + kvp.Key + "\":\"" + kvp.Value + "\"");
                    }
                }
                result = str.ToString();
            }
            return result;
        }
    }
}