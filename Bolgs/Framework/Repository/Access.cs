using System;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections;
using System.Text;
namespace Gozyy.Repositories
{
    /// <summary>
    /// 访问控制
    /// </summary>
    //[RemotingService]
    public class Access
    {
        private static Access instance = null;
        //public int _counts = 0;
        //public int _pages = 0;
        //public int _pagesize = 20;
        //public int _pageIndex = 1;

        private string dbtype = "sql2k5";
        private string conn = "";
        public Access()
        {
            if (ConfigurationManager.ConnectionStrings["driver"] != null)
            {
                dbtype = ConfigurationManager.ConnectionStrings["driver"].ConnectionString;
            }
        }
        public Access(string connstr)
        {
            if (ConfigurationManager.ConnectionStrings[connstr] != null)
            {
                conn = ConfigurationManager.ConnectionStrings[connstr].ConnectionString;
            }
        }
        public static Access Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Access();
                }
                return instance;
            }
        }
        
        private string _connectionString
        {
            get
            {
                if (conn == "") return ConfigurationManager.ConnectionStrings["connstr"].ConnectionString.Replace("|DataDirectory|", System.AppDomain.CurrentDomain.BaseDirectory);
                else return conn;
            }
        }
        public static string SqlConnectionString
        {
            get
            {
                string _connectionString = ConfigurationManager.ConnectionStrings["connstr"].ConnectionString.Replace("|DataDirectory|", System.AppDomain.CurrentDomain.BaseDirectory);

                //string _connectionString = "Data Source=.;Initial Catalog=master;Persist Security Info=True;User ID=sa;Password=sa";// ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                //try
                //{
                //    string ConStringEncrypt = ConfigurationManager.ConnectionStrings["ConStringEncrypt"].ConnectionString;
                //    if (ConStringEncrypt == "true")
                //    {
                //        _connectionString = DESEncrypt.Decrypt(_connectionString);
                //    }
                //}
                //catch
                //{ 

                //}
                return _connectionString;
            }
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        public int ExecNonQuery(string sql)
        {
            //sql = sql.Replace("--", "").Replace("'", "");
            StringBuilder strSql = new StringBuilder();
            strSql = new StringBuilder();
            strSql.Append(sql);
            var i = SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, strSql.ToString(), null);
            return i;
        }
        /// <summary>
        /// 插入数据返回自动ID
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteInsert(string sql)
        {
            //sql = sql.Replace("--", "").Replace("'", "");
            StringBuilder strSql = new StringBuilder();
            strSql = new StringBuilder();
            strSql.Append(sql);
            //开始插入数据
            var rst = int.Parse(SqlHelper.ExecuteScalar(_connectionString, CommandType.Text, sql.ToString(), null).ToString());
            if (rst == 0)
            {
                return 0;
            }
            else
            {
                return rst;
            }
        }
        /// <summary>
        /// 执行SQL并返回int值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int ExecNonQuery(string sql, params object[] args)
        {
            //参数
            MatchCollection mc;
            Regex r = new Regex("@[\\w.]+");
            mc = r.Matches(sql);//sql.Substring(sql.IndexOf("where")));
            System.Data.SqlClient.SqlParameter[] prms = null;
            if (mc.Count > 0)
            {
                prms = new System.Data.SqlClient.SqlParameter[mc.Count];
                for (int i = 0; i < mc.Count; i++)
                {
                    prms[i] = new System.Data.SqlClient.SqlParameter(mc[i].Value, args[i]);
                }
                foreach (var m in prms)
                {
                    if (m.Value == null) m.Value = DBNull.Value;
                }
            }
            //----------------------------------------------------------------------------------------------------
            if (sql == null || sql.Trim() == "") return -1;
            StringBuilder strSql = new StringBuilder();
            strSql = new StringBuilder();
            strSql.Append(sql);
            var rst = SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, strSql.ToString(), prms);
            return rst;
        }


        /// <summary>
        /// 增加一条记录-根据哈希表hashtable;如果增加成功,返回主键ID
        /// </summary>
        public int New(System.Collections.Hashtable ht)
        {
            if (ht["_name"] == null) return -1;

            StringBuilder sql = new StringBuilder();
            var param1 = "";
            var param2 = "";
            var pcount = 0;
            foreach (DictionaryEntry de in ht)
            {
                if (de.Key.ToString().Substring(0, 1) == "_") continue;

                if (param1.Trim() != "") param1 += ",[" + de.Key.ToString() + "]";
                else param1 = "[" + de.Key.ToString() + "]";
                if (param2.Trim() != "") param2 += ",@_" + de.Key.ToString();
                else param2 = "@_" + de.Key.ToString();

                pcount++;
            }
            sql.AppendFormat("insert into " + ht["_name"].ToString() + "({0})values({1});select @@IDENTITY", param1, param2);

            int rst = 0;
            switch (dbtype)
            {
                default:
                    System.Data.SqlClient.SqlParameter[] prms = SqlParam(ht);

                    foreach (var m in prms)
                    {
                        if (m.Value == null) m.Value = DBNull.Value;
                    }
                    //开始插入数据
                    rst = int.Parse(SqlHelper.ExecuteScalar(_connectionString, CommandType.Text, sql.ToString(), prms).ToString());
                    if (rst == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return rst;
                    }
            }
        }
        /// <summary>
        /// 删除一条记录 根据主键
        /// </summary>
        public int Del(System.Collections.Hashtable ht)
        {
            if (ht["_name"] == null) return -1;
            if (ht["_where"] == null) return -1;
            if (ht["_args"] == null) return -1;
            if (ht["_params"] == null) return -1;

            StringBuilder strSql = new StringBuilder();
            strSql.Append(" delete " + ht["_name"].ToString() + "   where " + ht["_where"].ToString());
            //参数设置
            string[] _params = (string[])ht["_params"];
            object[] _args = (object[])ht["_args"];
            System.Data.SqlClient.SqlParameter[] prms = new System.Data.SqlClient.SqlParameter[_params.Length];
            for (int i = 0; i < _params.Length; i++)
            {
                prms[i] = new System.Data.SqlClient.SqlParameter(_params[i], _args[i]);
            }
            foreach (var m in prms)
            {
                if (m.Value == null) m.Value = DBNull.Value;
            }

            int rst = SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, strSql.ToString(), prms);
            if (rst == 0)
            {
                return 0;
            }
            else
            {
                return rst;
            }
        }

        /// <summary>
        /// 获取查询参数对象 SqlParameter
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        private System.Data.SqlClient.SqlParameter[] SqlParam(System.Collections.Hashtable ht)
        {
            var sql = "select * from " + ht["_name"].ToString() + " where 1=2";
            DataTable dt = null;
            dt = SqlHelper.ExecuteDataset(_connectionString, CommandType.Text, sql, null).Tables[0];
            if (dt == null) return null;
            //where语句的参数
            int wi = 0;
            string[] _params = null;
            object[] _args = null;
            if (ht["_args"] != null)
            {
                _params = (string[])ht["_params"];
                _args = (object[])ht["_args"];
                wi = _args.Length;
            }
            //根据列类型生成相应参数
            ArrayList htparams = new ArrayList();

            var fname = "";
            foreach (DictionaryEntry de in ht)
            {
                if (de.Key.ToString().Substring(0, 1) == "_") continue;

                fname = de.Key.ToString().ToLower();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (fname == dt.Columns[i].ColumnName.ToString().ToLower())
                    {
                        System.Data.IDataParameter iparam = new System.Data.SqlClient.SqlParameter();
                        iparam.ParameterName = "@_" + dt.Columns[i].ColumnName;
                        iparam.DbType = GetDbType(dt.Columns[i].DataType);
                        iparam.Value = de.Value;
                        htparams.Add(iparam);
                    }
                }
            }
            System.Data.SqlClient.SqlParameter[] prms = new System.Data.SqlClient.SqlParameter[htparams.Count + wi];
            for (int i = 0; i < htparams.Count; i++)
            {
                prms[i] = (System.Data.SqlClient.SqlParameter)htparams[i];
            }
            //where条件参数
            for (int j = 0; j < wi; j++)
            {
                prms[htparams.Count + j] = new System.Data.SqlClient.SqlParameter(_params[j], _args[j]);
            }
            return prms;
        }
        private System.Data.SqlClient.SqlParameter[] SqlWhereParam(System.Collections.Hashtable ht)
        {
            string[] _params = null;
            object[] _args = null;
            if (ht["_args"] != null)
            {
                _params = (string[])ht["_params"];
                _args = (object[])ht["_args"];
            }
            else
            {
                return null;
            }
            //根据列类型生成相应参数
            ArrayList htparams = new ArrayList();

            System.Data.SqlClient.SqlParameter[] prms = new System.Data.SqlClient.SqlParameter[_params.Length];
            //where条件参数
            for (int j = 0; j < _params.Length; j++)
            {
                prms[htparams.Count + j] = new System.Data.SqlClient.SqlParameter(_params[j], _args[j]);
            }
            return prms;
        }
        /// <summary>
        /// 修改一条记录-根据哈希表hashtable
        /// </summary>
        public int Update(System.Collections.Hashtable ht)
        {
            if (ht["_name"] == null) return -1;
            if (ht["_where"] == null) return -1;

            StringBuilder sql = new StringBuilder();
            var param1 = "";
            var pcount = 0;
            foreach (DictionaryEntry de in ht)
            {
                if (de.Key.ToString().Substring(0, 1) == "_") continue;

                if (param1.Trim() != "") param1 += ",[" + de.Key.ToString() + "]=@_" + de.Key.ToString();
                else param1 = de.Key.ToString() + "=@_" + de.Key.ToString();

                pcount++;
            }
            sql.AppendFormat("update " + ht["_name"].ToString() + " set {0} where {1}", param1, ht["_where"].ToString());

            int rst = 0;
            switch (dbtype)
            {
                default:
                    System.Data.SqlClient.SqlParameter[] prms = SqlParam(ht);

                    foreach (var m in prms)
                    {
                        if (m.Value == null) m.Value = DBNull.Value;
                    }
                    //开始更新数据
                    rst = SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, sql.ToString(), prms);
                    if (rst == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return rst;
                    }
            }
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetData(string sql)
        {
            if (sql == null || sql.Trim() == "") return null;
            StringBuilder strSql = new StringBuilder();
            strSql = new StringBuilder();
            strSql.Append(sql);
            var rst = SqlHelper.ExecuteDataset(_connectionString, CommandType.Text, strSql.ToString(), null);
            if (rst == null) { return null; }
            return rst.Tables[0];
        }
        /// <summary>
        /// 获取第一行第一列的值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string GetValue(string sql, params object[] args)
        {
            //参数
            MatchCollection mc;
            Regex r = new Regex("@[\\w.]+");
            mc = r.Matches(sql);//sql.Substring(sql.IndexOf("where")));
            System.Data.SqlClient.SqlParameter[] prms = null;
            if (mc.Count > 0)
            {
                prms = new System.Data.SqlClient.SqlParameter[mc.Count];
                for (int i = 0; i < mc.Count; i++)
                {
                    prms[i] = new System.Data.SqlClient.SqlParameter(mc[i].Value, args[i]);
                }
                foreach (var m in prms)
                {
                    if (m.Value == null) m.Value = DBNull.Value;
                }
            }
            //----------------------------------------------------------------------------------------------------
            if (sql == null || sql.Trim() == "") return null;
            StringBuilder strSql = new StringBuilder();
            strSql = new StringBuilder();
            strSql.Append(sql);
            var rst = SqlHelper.ExecuteDataset(_connectionString, CommandType.Text, strSql.ToString(), prms);
            if (rst == null) { return null; }
            if (rst.Tables[0].Rows.Count > 0)
            {
                return rst.Tables[0].Rows[0][0].ToString();
            }
            return "";
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DataTable GetData(string sql, params object[] args)
        {
            //参数
            MatchCollection mc;
            Regex r = new Regex("@[\\w.]+");
            mc = r.Matches(sql);//sql.Substring(sql.IndexOf("where")));
            System.Data.SqlClient.SqlParameter[] prms = null;
            if (mc.Count > 0)
            {
                prms = new System.Data.SqlClient.SqlParameter[mc.Count];
                for (int i = 0; i < mc.Count; i++)
                {
                    prms[i] = new System.Data.SqlClient.SqlParameter(mc[i].Value, args[i]);
                }
                foreach (var m in prms)
                {
                    if (m.Value == null) m.Value = DBNull.Value;
                }
            }
            //----------------------------------------------------------------------------------------------------
            if (sql == null || sql.Trim() == "") return null;
            StringBuilder strSql = new StringBuilder();
            strSql = new StringBuilder();
            strSql.Append(sql);
            var rst = SqlHelper.ExecuteDataset(_connectionString, CommandType.Text, strSql.ToString(), prms);
            if (rst == null) { return null; }
            return rst.Tables[0];
        }
        /// <summary>
        /// 获取数据Model
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Hashtable GetModel(string sql, params object[] args)
        {
            //参数
            MatchCollection mc;
            Regex r = new Regex("@[\\w.]+");
            mc = r.Matches(sql);//sql.Substring(sql.IndexOf("where")));
            System.Data.SqlClient.SqlParameter[] prms = null;
            if (mc.Count > 0)
            {
                prms = new System.Data.SqlClient.SqlParameter[mc.Count];
                for (int i = 0; i < mc.Count; i++)
                {
                    prms[i] = new System.Data.SqlClient.SqlParameter(mc[i].Value, args[i]);
                }
                foreach (var m in prms)
                {
                    if (m.Value == null) m.Value = DBNull.Value;
                }
            }
            //----------------------------------------------------------------------------------------------------
            if (sql == null || sql.Trim() == "") return null;
            StringBuilder strSql = new StringBuilder();
            strSql = new StringBuilder();
            strSql.Append(sql);
            var rst = SqlHelper.ExecuteDataset(_connectionString, CommandType.Text, strSql.ToString(), prms);
            if (rst == null) { return null; }
            if (rst.Tables.Count < 1 || rst.Tables[0].Rows.Count < 1) return null;
            Hashtable model = new Hashtable(System.StringComparer.Create(System.Globalization.CultureInfo.CurrentCulture, true));
            var row = rst.Tables[0].Rows[0];
            for (int i = 0; i < rst.Tables[0].Columns.Count; i++)
            {
                model[rst.Tables[0].Columns[i].ColumnName] = rst.Tables[0].Rows[0][i].ToString();
            }
            return model;
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataTable GetData(System.Collections.Hashtable ht)
        {
            if (ht["_name"] == null) return null;

            var fields = "*";
            if (ht["_fields"] != null && ht["_fields"].ToString() != "")
            {
                fields = ht["_fields"].ToString();
            }

            StringBuilder strSql = new StringBuilder();

            strSql.Append("select  " + fields + "  ");
            strSql.Append(" FROM " + ht["_name"].ToString() + " ");
            if (ht["_where"] != null && ht["_where"].ToString().Trim() != "")
            {
                strSql.Append(" where " + ht["_where"].ToString());
            }

            string[] _params = null;
            object[] _args = null;
            System.Data.SqlClient.SqlParameter[] prms = null;
            if (ht["_params"] != null)
            {
                //参数设置
                _params = (string[])ht["_params"];
                _args = (object[])ht["_args"];
                prms = new System.Data.SqlClient.SqlParameter[_params.Length];
                for (int i = 0; i < _params.Length; i++)
                {
                    prms[i] = new System.Data.SqlClient.SqlParameter(_params[i], _args[i]);
                }
                foreach (var m in prms)
                {
                    if (m.Value == null) m.Value = DBNull.Value;
                }
            }

            DataSet rst = null;
            switch (dbtype)
            {
                default:
                    rst = SqlHelper.ExecuteDataset(_connectionString, CommandType.Text, strSql.ToString(), prms);
                    break;
            }

            if (rst == null) return null;
            ht["_counts"] = rst.Tables[0].Rows.Count;
            return rst.Tables[0];
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataTable GetData(System.Collections.Hashtable ht, int top)
        {
            if (ht["_name"] == null) return null;

            var fields = "*";
            if (ht["_fields"] != null && ht["_fields"].ToString() != "")
            {
                fields = ht["_fields"].ToString();
            }

            StringBuilder strSql = new StringBuilder();

            strSql.Append("select top " + top.ToString() + " " + fields + "  ");
            strSql.Append(" FROM " + ht["_name"].ToString() + " ");
            if (ht["_where"] != null && ht["_where"].ToString().Trim() != "")
            {
                strSql.Append(" where " + ht["_where"].ToString());
            }

            string[] _params = null;
            object[] _args = null;
            System.Data.SqlClient.SqlParameter[] prms = null;
            if (ht["_params"] != null)
            {
                //参数设置
                _params = (string[])ht["_params"];
                _args = (object[])ht["_args"];
                prms = new System.Data.SqlClient.SqlParameter[_params.Length];
                for (int i = 0; i < _params.Length; i++)
                {
                    prms[i] = new System.Data.SqlClient.SqlParameter(_params[i], _args[i]);
                }
                foreach (var m in prms)
                {
                    if (m.Value == null) m.Value = DBNull.Value;
                }
            }

            DataSet rst = null;
            switch (dbtype)
            {
                default:
                    rst = SqlHelper.ExecuteDataset(_connectionString, CommandType.Text, strSql.ToString(), prms);
                    break;
            }

            if (rst == null) return null;
            ht["_counts"] = rst.Tables[0].Rows.Count;
            return rst.Tables[0];
        }
        //#region 需要调用存储过程的分页方法---过时
        ///// <summary>
        ///// 获得数据列表--分页
        ///// 参数: 
        ///// 1 fields    [选填] 字段 如: name,age
        ///// 2 order     [必填] 排序字段 必填,多个使用","分割 如: id,name desc,age
        ///// 3 where     [必填] 条件语句   age=18
        ///// 4 pageindex [必填] 页索引
        ///// 5 pagesize  [选填] 页大小  
        ///// 6 counts  返回参数  总记录数
        ///// 7 pages   返回参数  总页数
        ///// </summary>
        ///// <param name="ht"></param>
        ///// <returns></returns>
        //[DataTableType("Weekly.Model")]
        //public DataTable GetPaging(System.Collections.Hashtable ht)
        //{
        //    if (ht["_name"] == null) return null;
        //    var pagesize = 20;
        //    var pageindex = 0;
        //    var counts = 0;
        //    var pages = 0;
        //    var fields = "*";
        //    if (ht["fields"] != null && ht["fields"].ToString() != "")
        //    {
        //        fields = ht["fields"].ToString();
        //    }
        //    string _name = ht["_name"].ToString();

        //    if (ht["pageindex"] != null && ht["pageindex"].ToString() != "")
        //        pageindex = int.Parse(ht["pageindex"].ToString());

        //    if (ht["pagesize"] != null && ht["pagesize"].ToString() != "")
        //        pagesize = int.Parse(ht["pagesize"].ToString());

        //    string _where = ht["where"].ToString();

        //    var model = new Model("PR_Paging");
        //    model["TableName"] = _name;
        //    model["ReFieldsStr"] = fields;
        //    model["OrderString"] = ht["order"] == null ? "id":ht["order"];
        //    model["WhereString"] = _where;
        //    model["PageSize"] = pagesize;
        //    model["PageIndex"] = pageindex;
        //    model["TotalRecord"] = 0;
        //    var dt =Proc(model);
        //    if (dt != null)
        //    {
        //        counts = int.Parse(model["TotalRecord"].ToString()); 
        //        pages = counts / pagesize; 
        //        if (counts % pagesize > 0) pages++;
        //        ht["_counts"] = counts;
        //        ht["_pages"] = pages;
        //    }
        //    else
        //    {
        //        pages = 0; 
        //        counts = 0;
        //        ht["_counts"] = counts;
        //        ht["_pages"] = pages;
        //    }
        //    return dt;

        //}
        //#endregion

        /// <summary>
        /// 分页查询方法
        /// </summary>
        /// <param name="vtname">表或视图名称</param>
        /// <param name="fields">字段</param>
        /// <param name="order">排序</param>
        /// <param name="pagesize">页大小</param>
        /// <param name="pageindex">页索引</param>
        /// <param name="counts">总记录数</param>
        /// <param name="pages">总页数</param>
        /// <param name="where">where条件: name=@name</param>
        /// <param name="args">where参数</param>
        /// <returns></returns>
        public DataTable GetPaging(string vtname, string fields, string order, int pagesize, int pageindex, out int counts, out int pages, string where, params object[] args)
        {
            counts = 0;
            //参数
            MatchCollection mc;
            Regex r = new Regex("@[\\w.]+");
            mc = r.Matches(where);
            System.Data.SqlClient.SqlParameter[] prms = null;
            if (mc.Count > 0)
            {
                prms = new System.Data.SqlClient.SqlParameter[mc.Count];
                for (int i = 0; i < mc.Count; i++)
                {
                    prms[i] = new System.Data.SqlClient.SqlParameter(mc[i].Value, args[i]);
                }
                foreach (var m in prms)
                {
                    if (m.Value == null) m.Value = DBNull.Value;
                }
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select count(*) as counts ");
            if (where.Trim() != "") strSql.Append(" from " + vtname + "  where " + where);
            else strSql.Append(" from " + vtname);

            var ds = SqlHelper.ExecuteDataset(_connectionString, CommandType.Text, strSql.ToString(), prms);
            if (ds == null || ds.Tables.Count < 1)
            {
                counts = 0;
                pages = 0;
                return null;
            }
            else
            {
                counts = int.Parse(ds.Tables[0].Rows[0]["counts"].ToString());
                pages = counts / pagesize;
                if (counts % pagesize > 0) pages++;
            }
            //查询结果
            strSql = new StringBuilder();
            strSql.Append(" select " + fields + " from ( ");
            strSql.Append(" select row_number() over (order by " + order + ") as rowId," + fields);
            if (where.Trim() != "") strSql.Append(" from " + vtname + "   where " + where + ") as t ");
            else strSql.Append(" from " + vtname + " ) as t ");
            strSql.Append(" where rowId between " + (pagesize * (pageindex - 1) + 1).ToString() + " and " + (pagesize * pageindex).ToString());
            ds = SqlHelper.ExecuteDataset(_connectionString, CommandType.Text, strSql.ToString(), prms);
            if (ds == null || ds.Tables.Count < 1) return null;
            return ds.Tables[0];
        }
        /// <summary>
        /// 执行存储过程-根据哈希表hashtable  [只适用于SQL Server数据库]
        /// </summary>
        public DataTable Proc(System.Collections.Hashtable ht)
        {
            if (ht["_name"] == null) return null;

            StringBuilder sql = new StringBuilder();
            var param1 = "";
            var pcount = 0;
            foreach (DictionaryEntry de in ht)
            {
                if (de.Key.ToString().Substring(0, 1) == "_") continue;
                pcount++;
            }
            sql.AppendFormat("" + ht["_name"].ToString() + "");
            System.Data.SqlClient.SqlParameter[] parameters = new System.Data.SqlClient.SqlParameter[pcount];
            //获取字段信息  
            var fieldtype = "";
            var fieldlen = 0;

            DataTable dt = GetStruct(ht["_name"].ToString(), "p", "sql2k5");
            if (dt.Rows.Count > 0)
            {
                int k = -1;
                int flag = 0;
                var fname = "";
                foreach (DictionaryEntry de in ht)
                {
                    if (de.Key.ToString().Substring(0, 1) == "_") continue;

                    k += 1;
                    param1 = de.Key.ToString();
                    fname = de.Key.ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (param1 == dt.Rows[i]["fieldname"].ToString().Replace("@", ""))
                        {
                            flag++;
                            fieldtype = dt.Rows[i]["fieldtype"].ToString();
                            fieldlen = int.Parse(dt.Rows[i]["fieldlen"].ToString());

                            parameters[k] = GetSqlParameter(param1, fieldtype, fieldlen);
                            parameters[k].Value = de.Value;
                            //输出参数
                            if (dt.Rows[i]["outparam"].ToString() == "1")
                            {
                                parameters[k].Direction = ParameterDirection.Output;
                            }
                        }
                    }
                }
                if (pcount != flag) return null;
            }
            else
            {
                return null;
            }
            foreach (var m in parameters)
            {
                if (m.Value == null) m.Value = DBNull.Value;
            }
            //开始执行过程
            var rst = SqlHelper.ExecuteDataset(_connectionString, CommandType.StoredProcedure, sql.ToString(), parameters);
            if (rst != null)
            {
                foreach (var p in parameters)
                {
                    if (p.Direction == ParameterDirection.Output)
                    {
                        ht[p.ParameterName.Replace("@", "")] = p.Value;
                    }
                }
                if (rst.Tables.Count == 0) return null;
                return rst.Tables[0];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 调用存储过程-返回一个查询
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public DataTable ProcData(System.Collections.Hashtable ht)
        {
            if (ht["_name"] == null) return null;

            StringBuilder sql = new StringBuilder();
            var param1 = "";
            var pcount = 0;
            foreach (DictionaryEntry de in ht)
            {
                if (de.Key.ToString().Substring(0, 1) == "_") continue;
                pcount++;
            }
            sql.AppendFormat("" + ht["_name"].ToString() + "");
            System.Data.SqlClient.SqlParameter[] parameters = new System.Data.SqlClient.SqlParameter[pcount];
            //获取字段信息  
            var fieldtype = "";
            var fieldlen = 0;

            DataTable dt = GetStruct(ht["_name"].ToString(), "p", "sql2k5");
            if (dt.Rows.Count > 0)
            {
                int k = -1;
                int flag = 0;
                var fname = "";
                foreach (DictionaryEntry de in ht)
                {
                    if (de.Key.ToString().Substring(0, 1) == "_") continue;

                    k += 1;
                    param1 = de.Key.ToString();
                    fname = de.Key.ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (param1 == dt.Rows[i]["fieldname"].ToString().Replace("@", ""))
                        {
                            flag++;
                            fieldtype = dt.Rows[i]["fieldtype"].ToString();
                            fieldlen = int.Parse(dt.Rows[i]["fieldlen"].ToString());

                            parameters[k] = GetSqlParameter(param1, fieldtype, fieldlen);
                            parameters[k].Value = de.Value;
                            //输出参数
                            if (dt.Rows[i]["outparam"].ToString() == "1")
                            {
                                parameters[k].Direction = ParameterDirection.Output;
                            }
                        }
                    }
                }
                if (pcount != flag) return null;
            }
            else
            {
                return null;
            }
            foreach (var m in parameters)
            {
                if (m.Value == null) m.Value = DBNull.Value;
            }
            //开始执行过程
            var rst = SqlHelper.ExecuteDataset(_connectionString, CommandType.StoredProcedure, sql.ToString(), parameters);
            if (rst != null)
            {
                foreach (var p in parameters)
                {
                    if (p.Direction == ParameterDirection.Output)
                    {
                        ht[p.ParameterName.Replace("@", "")] = p.Value;
                    }
                }
                if (rst.Tables.Count == 0) return null;
                return rst.Tables[0];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 调用存储过程-返回值为整数  -1代表错误
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public int ProcInt(System.Collections.Hashtable ht)
        {
            if (ht["_name"] == null) return -1;

            StringBuilder sql = new StringBuilder();
            var param1 = "";
            var pcount = 0;
            foreach (DictionaryEntry de in ht)
            {
                if (de.Key.ToString().Substring(0, 1) == "_") continue;
                pcount++;
            }
            sql.AppendFormat("" + ht["_name"].ToString() + "");
            System.Data.SqlClient.SqlParameter[] parameters = new System.Data.SqlClient.SqlParameter[pcount];
            //获取字段信息  
            var fieldtype = "";
            var fieldlen = 0;

            DataTable dt = GetStruct(ht["_name"].ToString(), "p", "sql2k5");
            if (dt.Rows.Count > 0)
            {
                int k = -1;
                int flag = 0;
                var fname = "";
                foreach (DictionaryEntry de in ht)
                {
                    if (de.Key.ToString().Substring(0, 1) == "_") continue;

                    k += 1;
                    param1 = de.Key.ToString();
                    fname = de.Key.ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (param1 == dt.Rows[i]["fieldname"].ToString().Replace("@", ""))
                        {
                            flag++;
                            fieldtype = dt.Rows[i]["fieldtype"].ToString();
                            fieldlen = int.Parse(dt.Rows[i]["fieldlen"].ToString());

                            parameters[k] = GetSqlParameter(param1, fieldtype, fieldlen);
                            parameters[k].Value = de.Value;
                            //输出参数
                            if (dt.Rows[i]["outparam"].ToString() == "1")
                            {
                                parameters[k].Direction = ParameterDirection.Output;
                            }
                        }
                    }
                }
                if (pcount != flag) return -1;
            }
            else
            {
                return -1;
            }
            foreach (var m in parameters)
            {
                if (m.Value == null) m.Value = DBNull.Value;
            }
            //开始执行过程
            var rst = SqlHelper.ExecuteNonQuery(_connectionString, CommandType.StoredProcedure, sql.ToString(), parameters);
            foreach (var p in parameters)
            {
                if (p.Direction == ParameterDirection.Output)
                {
                    ht[p.ParameterName.Replace("@", "")] = p.Value;
                }
            }
            return rst;
        }
        /// <summary>
        /// 返回一个参数
        /// </summary>
        /// <param name="fieldtype"></param>
        /// <param name="fieldlen"></param>
        /// <returns></returns>
        private System.Data.SqlClient.SqlParameter GetSqlParameter(string fname, string fieldtype, int fieldlen)
        {
            System.Data.SqlClient.SqlParameter param = null;
            switch (fieldtype)
            {
                case "bigint":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.BigInt);
                    break;
                case "binary":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.Binary);
                    break;
                case "bit":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.Bit);
                    break;
                case "char":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.Char);
                    break;
                case "datetime":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.DateTime);
                    break;
                case "decimal":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.Decimal);
                    break;
                case "float":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.Float);
                    break;
                case "image":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.Image);
                    break;
                case "int":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.Int);
                    break;
                case "money":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.Money);
                    break;
                case "nchar":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.NChar);
                    break;
                case "ntext":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.NText, fieldlen);
                    break;
                case "numeric":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.Decimal);
                    break;
                case "nvarchar":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.NVarChar, fieldlen);
                    break;
                case "real":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.Real);
                    break;
                case "SmallDateTime":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.SmallDateTime);
                    break;
                case "smallint":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.SmallInt);
                    break;
                case "smallmoney":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.SmallMoney);
                    break;
                case "text":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.Text);
                    break;
                case "tinyint":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.TinyInt);
                    break;
                case "uniqueidentifier":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.UniqueIdentifier);
                    break;
                case "varbinary":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.VarBinary);
                    break;
                case "varchar":
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.VarChar, fieldlen);
                    break;
                default:
                    param = new System.Data.SqlClient.SqlParameter("@" + fname, SqlDbType.VarChar, fieldlen);
                    break;
            }
            return param;
        }
        /// <summary>
        /// 参数类型与数据库类型的转换
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private System.Data.DbType GetDbType(Type type)
        {
            DbType result = DbType.String;
            if (type.Equals(typeof(int)) || type.IsEnum)
                result = DbType.Int32;
            else if (type.Equals(typeof(long)))
                result = DbType.Int32;
            else if (type.Equals(typeof(double)) || type.Equals(typeof(Double)))
                result = DbType.Decimal;
            else if (type.Equals(typeof(DateTime)))
                result = DbType.DateTime;
            else if (type.Equals(typeof(bool)))
                result = DbType.Boolean;
            else if (type.Equals(typeof(string)))
                result = DbType.String;
            else if (type.Equals(typeof(decimal)))
                result = DbType.Decimal;
            else if (type.Equals(typeof(byte[])))
                result = DbType.Binary;
            else if (type.Equals(typeof(Guid)))
                result = DbType.Guid;

            return result;

        }
        /// <summary>
        /// 获取sqlserver中对象的结构信息
        /// </summary>
        /// <param name="name">表,视图,存储过程名称</param>
        /// <param name="type">表 u,视图 v,存储过程 p</param>
        /// <param name="sqlver">sql2k,sql2k5</param>
        /// <returns></returns>
        private DataTable GetStruct(string name, string type, string sqlver)
        {
            StringBuilder sql = new StringBuilder();
            if (sqlver == "sql2k")
            {
                sql.Append(" SELECT ");
                sql.Append(" sysobjects.name AS [tablename],  ");
                sql.Append(" sysproperties.[value] AS [tablenote],  ");
                sql.Append(" syscolumns.name AS [fieldname],  ");
                sql.Append(" properties.[value] AS [fieldnote],  ");
                sql.Append(" systypes.name AS [fieldtype],  ");
                sql.Append(" syscolumns.length AS [fieldlen],  ");
                sql.Append(" syscolumns.isoutparam AS [outparam],  ");
                sql.Append(" ISNULL(COLUMNPROPERTY(syscolumns.id, syscolumns.name, 'Scale'), 0) AS [scale],  ");
                sql.Append(" CASE syscolumns.isnullable WHEN '1' THEN 'Y' ELSE 'N' END AS [nullable],  ");
                sql.Append(" CASE WHEN syscomments.text IS NULL THEN '' ELSE syscomments.text END AS [defvalue],  ");
                sql.Append(" CASE WHEN COLUMNPROPERTY(syscolumns.id, syscolumns.name, 'IsIdentity') = 1 THEN 1 ELSE 0 END AS [identity],  ");
                sql.Append(" CASE WHEN EXISTS (SELECT 1 FROM sysobjects WHERE xtype = 'PK' AND name IN  ");
                sql.Append(" (SELECT name  ");
                sql.Append(" FROM sysindexes  ");
                sql.Append(" WHERE indid IN  ");
                sql.Append(" (SELECT indid  ");
                sql.Append(" FROM sysindexkeys  ");
                sql.Append(" WHERE id = syscolumns.id AND colid = syscolumns.colid)))  ");
                sql.Append(" THEN 1 ELSE 0 END AS [iskey]  ");
                sql.Append(" FROM syscolumns INNER JOIN  ");
                sql.Append(" sysobjects ON sysobjects.id = syscolumns.id INNER JOIN  ");
                sql.Append(" systypes ON syscolumns.xtype = systypes.xtype LEFT OUTER JOIN  ");
                sql.Append(" sysproperties properties ON syscolumns.id = properties.id AND  ");
                sql.Append(" syscolumns.colid = properties.smallid LEFT OUTER JOIN  ");
                sql.Append(" sysproperties ON sysobjects.id = sysproperties.id AND  ");
                sql.Append(" sysproperties.smallid = 0 LEFT OUTER JOIN  ");
                sql.Append(" syscomments ON syscolumns.cdefault = syscomments.id  ");
                sql.Append(" WHERE (sysobjects.xtype = '" + type + "')  ");
                sql.Append(" AND sysobjects.NAME='" + name + "' ");
                sql.Append(" ORDER BY [tablename],[syscolumns.colorder]  ");
            }
            else
            {
                sql.Append(" SELECT  ");
                sql.Append(" Sysobjects.name AS [tablename], ");
                sql.Append(" '' as [tablenote], ");
                sql.Append(" syscolumns.name AS [fieldname],  ");
                sql.Append(" sys.extended_properties.[value] AS [fieldnote],  ");
                sql.Append(" systypes.name AS [fieldtype],  ");
                sql.Append(" syscolumns.length AS [fieldlen],  ");
                sql.Append(" syscolumns.isoutparam AS [outparam],  ");

                sql.Append(" CASE syscolumns.isnullable WHEN '1' THEN 'Y' ELSE 'N' END AS [nullable],  ");
                sql.Append(" syscomments.text AS [defvalue],  ");
                sql.Append(" ISNULL(COLUMNPROPERTY(syscolumns.id, syscolumns.name, 'Scale'), 0) AS [scale],  ");
                sql.Append(" COLUMNPROPERTY(syscolumns.id, syscolumns.name, 'IsIdentity') AS [identity] ,  ");
                sql.Append(" CASE WHEN EXISTS (SELECT 1 FROM sysobjects WHERE xtype = 'PK' AND name IN  ");
                sql.Append(" (SELECT name  ");
                sql.Append(" FROM sysindexes  ");
                sql.Append(" WHERE indid IN  ");
                sql.Append(" (SELECT indid  ");
                sql.Append(" FROM sysindexkeys  ");
                sql.Append(" WHERE id = syscolumns.id AND colid = syscolumns.colid)))  ");
                sql.Append(" THEN 1 ELSE 0 END AS [iskey]  ");
                sql.Append(" FROM syscolumns  ");
                sql.Append(" INNER JOIN systypes  ");
                sql.Append(" ON syscolumns.xtype = systypes.xtype  ");
                sql.Append(" LEFT JOIN sysobjects ON syscolumns.id = sysobjects.id  ");
                sql.Append(" LEFT OUTER JOIN sys.extended_properties ON  ");
                sql.Append(" ( sys.extended_properties.minor_id = syscolumns.colid  ");
                sql.Append(" AND sys.extended_properties.major_id = syscolumns.id)  ");
                sql.Append(" LEFT OUTER JOIN syscomments ON syscolumns.cdefault = syscomments.id  ");
                sql.Append(" WHERE (systypes.name <> 'sysname')  ");
                sql.Append(" AND syscolumns.id IN (SELECT id FROM SYSOBJECTS WHERE xtype = '" + type + "' AND NAME = '" + name + "') ");
                //sql.Append(" ORDER BY [tablename],[syscolumns.colorder]  ");

            }
            DataSet ds = SqlHelper.ExecuteDataset(_connectionString, CommandType.Text, sql.ToString(), null);
            if (ds.Tables[0].Rows.Count > 0) return ds.Tables[0];
            return null;
        }
        /// <summary>  
        /// 分页条目
        /// </summary>   
        /// <returns></returns> 
        public string PagePilot(int page, int pages, int counts)
        {
            int currentPage = page;       //当前页  
            var totalPages = pages;       //总页数  
            var output = new StringBuilder();


            if (currentPage <= 0) currentPage = 1;
            if (totalPages > 1)
            {
                if (currentPage != 1)
                {
                    //处理首页连接
                    output.AppendFormat("{0} ", "<a href='javascript:void(0);' onclick=\"$('#page').val('1');return query();\">首页</a>");
                }
                if (currentPage > 1)
                {
                    //处理上一页的连接
                    output.Append("<a href='javascript:void(0);' onclick=\"$('#page').val('" + (currentPage - 1) + "');return query();\">上一页</a>");
                }
                else
                {
                    output.Append("上一页");
                }
                output.Append(" ");
                int currint = 5;
                for (int i = 0; i <= 10; i++)
                {
                    //一共最多显示10个页码，前面5个，后面5个  
                    if ((currentPage + i - currint) >= 1 && (currentPage + i - currint) <= totalPages)
                        if (currint == i)
                        {
                            //当前页处理  
                            output.Append(string.Format("[{0}]", currentPage));
                        }
                        else
                        {
                            //一般页处理 
                            output.Append("<a href='javascript:void(0);'onclick=\"$('#page').val('" + (currentPage + i - currint).ToString() + "');return query();\">" + (currentPage + i - currint).ToString() + "</a>");
                        }
                    output.Append(" ");
                }
                if (currentPage < totalPages)
                {
                    //处理下一页的链接 
                    output.Append("<a href='javascript:void(0);'onclick=\"$('#page').val('" + (currentPage + 1) + "');return query();\">下一页</a>");
                }
                else
                {
                    output.Append("下一页");
                }
                output.Append(" ");
                if (currentPage != totalPages)
                {
                    output.Append("<a href='javascript:void(0);' onclick=\"$('#page').val('" + totalPages + "');return query();\">末页</a>");
                }
                output.Append(" ");
            }
            output.AppendFormat("[{0} / {1}] 总记录数:{2}", currentPage, totalPages, counts);//这个统计加不加都行 
            return output.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Repositories.Database GetDatabase()
        {
            return new Repositories.Database("connstr");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connstr"></param>
        /// <returns></returns>
        public static Repositories.Database GetDatabase(string connstr)
        {
            return new Repositories.Database(connstr);
        }
        /// <summary>
        /// 返回一个查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, params object[] args)
        {
            DataTable dt = null;
            using (var db = GetDatabase())
            {
                dt = db.GetDataTable(sql, args);
            }
            return dt;
        }
        /// <summary>
        /// 返回指定数据库连接字符串的查询
        /// </summary>
        /// <param name="connstr"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string connstr,string sql, params object[] args)
        {
            DataTable dt = null;
            using (var db = GetDatabase(connstr))
            {
                dt = db.GetDataTable(sql, args);
            }
            return dt;
        }
    }
}