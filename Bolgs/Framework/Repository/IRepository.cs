using Gozyy.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gozyy.Repositories
{
    /// <summary>
    /// 数据持久化-可被继承
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IRepository<T>
    {
        /// <summary>
        /// 返回一条记录的实体
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static T Get(Sql sql)
        {
            using (var db = Access.GetDatabase())
            {
                return db.FirstOrDefault<T>(sql);
            }
        }
        /// <summary>
        /// 根据查询参数返回一条记录的实体
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T Get(string sql, params object[] args)
        {
            using (var db = Access.GetDatabase())
            {
                return db.FirstOrDefault<T>(sql, args);
            }
        }
        /// <summary>
        /// 根据主键ID 返回实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T GetById(int id)
        {
            using (var db = Access.GetDatabase())
            {
                return db.SingleOrDefault<T>(id);
            }
        }
        /// <summary>
        /// 增加一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int Insert(T entity)
        {
            using (var db = Access.GetDatabase())
            {
                var i = db.Insert(entity);
                return int.Parse(i.ToString());
            }
        }
        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int Update(T entity)
        {
            using (var db = Access.GetDatabase())
            {
                return db.Update(entity);
            }
        }
        /// <summary>
        /// 更新一条记录根据主键ID
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int Update(T entity, int keyid)
        {
            using (var db = Access.GetDatabase())
            {
                return db.Update(entity, keyid);
            }
        }
        /// <summary>
        /// 根据指定字段更新一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static int Update(T entity, IEnumerable<string> columns)
        {
            using (var db = Access.GetDatabase())
            {
                return db.Update(entity, columns);
            }
        }
        /// <summary>
        /// 根据主键ID更新指定字段
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="columns"></param>
        /// <param name="keyid"></param>
        /// <returns></returns>
        public static int Update(T entity, IEnumerable<string> columns, int keyid)
        {
            using (var db = Access.GetDatabase())
            {
                return db.Update(entity, keyid, columns);
            }
        }
        /// <summary>
        /// 根据实体删除一体记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int Delete(T entity)
        {
            using (var db = Access.GetDatabase())
            {
                return db.Delete(entity);
            }
        }
        /// <summary>
        /// 根据主键ID删除一条记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete(int id)
        {
            using (var db = Access.GetDatabase())
            {
                return db.Delete<T>(id);
            }
        }
        /// <summary>
        /// 根据条件删除记录
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int Delete(Sql sql)
        {
            using (var db = Access.GetDatabase())
            {
                return db.Delete<T>(sql);
            }
        }
        /// <summary>
        /// 根据条件参数删除记录
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int Delete(string sql, params object[] args)
        {
            using (var db = Access.GetDatabase())
            {
                return db.Delete<T>(sql, args);
            }
        }
        /// <summary>
        /// 返回默认所有记录
        /// </summary>
        /// <returns></returns>
        public static List<T> List()
        {
            return List("");
        }
        /// <summary>
        /// 返回查询列表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<T> List(string sql)
        {
            using (var db = Access.GetDatabase())
            {
                return db.Fetch<T>(sql);
            }
        }
        /// <summary>
        /// 根据查询参数返回列表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static List<T> List(string sql, params object[] args)
        {
            using (var db = Access.GetDatabase())
            {
                return db.Fetch<T>(sql, args);
            }
        }
        /// <summary>
        /// 根据查询条件返回指定分页数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static Page<T> List(int page, int pagesize, string sql)
        {
            using (var db = Access.GetDatabase())
            {
                return db.Page<T>(page, pagesize, sql);
            }
        }
        /// <summary>
        /// 根据查询参数返回指定分页数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Page<T> List(int page, int pagesize, string sql, params object[] args)
        {
            using (var db = Access.GetDatabase())
            {
                return db.Page<T>(page, pagesize, sql, args);
            }
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
        public static DataTable GetDataTable(string connstr, string sql, params object[] args)
        {
            DataTable dt = null;
            using (var db = GetDatabase(connstr))
            {
                dt = db.GetDataTable(sql, args);
            }
            return dt;
        }
        /// <summary>
        /// 执行一条语句返回影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int Exec(string sql, params object[] args)
        {
            using (var db = GetDatabase())
            {
                return db.Execute(sql, args);
            }
        }
    }
}
