using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Text;
using Gozyy.Repositories;
namespace Gozyy.Framework.Utils
{
    public class XTree
    {

        /////----------------------------------------------------
        /// <summary>
        /// 返回树脚本--菜单管理使用
        /// </summary>
        /// <returns></returns>
        public static string GetNodeTree(int nid)
        {
            string treestr = "";
            DataTable dt = SqlHelper.ExecuteDataset("select * from mgr_sitemaps where nid="+nid.ToString()).Tables[0];
            if (dt.Rows.Count <= 0) return "";

            string treeid = "tree" + dt.Rows[0]["nid"].ToString();
            treestr += "var " + treeid + " = new WebFXTree('" + dt.Rows[0]["title"].ToString() + "');";
            treestr += "";
            treestr += treeid + ".setBehavior('explorer');\r\n";
            treestr += treeid + ".icon = '../images/xtree/images/root.gif';\r\n";
            treestr += treeid + ".openIcon = ../images/xtree/images/root.gif';\r\n";
            treestr += NodeTreeUrl(int.Parse(dt.Rows[0]["nid"].ToString()));
            return "<script language='javascript'>" + treestr + " document.write(" + treeid + ");tree" + dt.Rows[0]["nid"].ToString() + ".expandAll();</script>";
        }
        /// <summary>
        /// 迭代节点
        /// </summary>
        /// <param name="noteid"></param>
        /// <returns></returns>
        private static string NodeTreeUrl(int noteid)
        {
            string item = "";
            DataSet ds = SqlHelper.ExecuteDataset("select * from mgr_sitemaps where parentid=" + noteid.ToString()+" order by orders");
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string tid = "";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tid = "tree" + ds.Tables[0].Rows[i]["nid"].ToString();
                    item += " var  " + tid + "= new WebFXTreeItem('" + ds.Tables[0].Rows[i]["title"].ToString()+ "','" + ds.Tables[0].Rows[i]["url"].ToString().Trim() + "');\r\n";
                    item += " tree" + noteid.ToString() + ".add(" + tid + ");\r\n";

                    item += NodeTreeUrl(int.Parse(ds.Tables[0].Rows[i]["nid"].ToString()));


                }
            }
            return item;
        }
        
        /// <summary>
        /// 返回树脚本--授权用(无无效节点[--节点])
        /// </summary>
        /// <returns></returns>
        public static string GetNodeTree()
        {
            string treestr = "";
            DataTable dt = SqlHelper.ExecuteDataset("select * from mgr_sitemaps where parentid=0 order by orders").Tables[0];
            if (dt.Rows.Count <= 0) return "";

            string treeid = "tree" + dt.Rows[0]["nid"].ToString();
            treestr += "var " + treeid + " = new WebFXTree('" + dt.Rows[0]["title"].ToString() + "');";
            treestr += "";
            treestr += treeid + ".setBehavior('explorer');\r\n";
            treestr += treeid + ".icon = '../images/xtree/images/root.gif';\r\n";
            treestr += treeid + ".openIcon = '../images/xtree/images/root.gif';\r\n";
            treestr += NodeTree(int.Parse(dt.Rows[0]["nid"].ToString()));
            return "<script language='javascript'>" + treestr + " document.write(" + treeid + ");</script>";
        }
        /// <summary>
        /// 迭代节点
        /// </summary>
        /// <param name="noteid"></param>
        /// <returns></returns>
        private static string NodeTree(int noteid)
        {
            string item = "";
            DataSet ds = SqlHelper.ExecuteDataset("select * from mgr_sitemaps where parentid=" + noteid.ToString()+" order by orders");
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string tid = "";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tid = "tree" + ds.Tables[0].Rows[i]["nid"].ToString();
                    item += " var  " + tid + "= new WebFXTreeItem('" + ds.Tables[0].Rows[i]["title"].ToString() + "-" + ds.Tables[0].Rows[i]["nid"].ToString() + "','" + ds.Tables[0].Rows[i]["nid"].ToString().Trim() + "');\r\n";
                    item += " tree" + noteid.ToString() + ".add(" + tid + ");\r\n";

                    item += NodeTree(int.Parse(ds.Tables[0].Rows[i]["nid"].ToString()));


                }
            }
            return item;
        }
        /// <summary>
        /// 返回树脚本--授权使用
        /// </summary>
        /// <returns></returns>
        public static string GetRightTree()
        {
            string treestr = "";
            DataTable dt = SqlHelper.ExecuteDataset("select * from mgr_sitemaps where parentid=0 and title != '--' order by orders").Tables[0];
            if (dt.Rows.Count <= 0) return "";

            string treeid = "tree" + dt.Rows[0]["nid"].ToString();
            treestr += "var " + treeid + " = new WebFXTree('" + dt.Rows[0]["title"].ToString() + "');";
            treestr += "";
            treestr += treeid + ".setBehavior('explorer');\r\n";
            treestr += treeid + ".icon = '../../images/xtree/images/root.gif';\r\n";
            treestr += treeid + ".openIcon = '../../images/xtree/images/root.gif';\r\n";
            treestr += RightTree(int.Parse(dt.Rows[0]["nid"].ToString()));
            return "<script language='javascript'>" + treestr + " document.write(" + treeid + ");</script>";
        }
        /// <summary>
        /// 迭代节点
        /// </summary>
        /// <param name="noteid"></param>
        /// <returns></returns>
        private static string RightTree(int noteid)
        {
            string item = "";
            DataSet ds = SqlHelper.ExecuteDataset("select * from mgr_sitemaps where title != '--' and  parentid=" + noteid.ToString());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string tid = "";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tid = "tree" + ds.Tables[0].Rows[i]["nid"].ToString();
                    item += " var  " + tid + "= new WebFXTreeItem('" + ds.Tables[0].Rows[i]["title"].ToString() + "-" + ds.Tables[0].Rows[i]["nid"].ToString() + "','" + ds.Tables[0].Rows[i]["nid"].ToString().Trim() + "');\r\n";
                    item += " tree" + noteid.ToString() + ".add(" + tid + ");\r\n";

                    item += NodeTree(int.Parse(ds.Tables[0].Rows[i]["nid"].ToString()));


                }
            }
            return item;
        }
        /// <summary>
        /// 返回树脚本
        /// </summary>
        /// <returns></returns>
        public static string GetXTree()
        {
            string treestr = "";
            DataTable dt = SqlHelper.ExecuteDataset("select * from mgr_sitemaps where parentid=0").Tables[0];
            if (dt.Rows.Count <= 0) return "";

            string treeid = "tree" + dt.Rows[0]["nid"].ToString();
            treestr += "var " + treeid + " = new WebFXTree('" + dt.Rows[0]["title"].ToString() + "');";
            treestr += "";
            treestr += treeid + ".setBehavior('explorer');\r\n";
            treestr += treeid + ".icon = '../images/xtree/images/root.gif';\r\n";
            treestr += treeid + ".openIcon = '../images/xtree/images/root.gif';\r\n";
            treestr += Tree(int.Parse(dt.Rows[0]["nid"].ToString()));
            return "<script language='javascript'>" + treestr + " document.write(" + treeid + ");</script>";
        }
        /// <summary>
        /// 迭代节点
        /// </summary>
        /// <param name="noteid"></param>
        /// <returns></returns>
        private static string Tree(int noteid)
        {
            string item = "";
            DataSet ds = SqlHelper.ExecuteDataset("select * from mgr_sitemaps where parentid=" + noteid.ToString());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string tid = "";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tid = "tree" + ds.Tables[0].Rows[i]["nid"].ToString();
                    item += " var  " + tid + "= new WebFXTreeItem('" + ds.Tables[0].Rows[i]["title"].ToString() + "'" + (ds.Tables[0].Rows[i]["url"].ToString().Trim() == "" ? "" : ",'" + ds.Tables[0].Rows[i]["url"].ToString() + "'") + ");\r\n";
                    item += " tree" + noteid.ToString() + ".add(" + tid + ");\r\n";

                    item += Tree(int.Parse(ds.Tables[0].Rows[i]["nid"].ToString()));


                }
            }
            return item;
        }
        /// <summary>
        /// 获取节点信息
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public static DataTable GetNote(int note)
        {
            DataTable dt = SqlHelper.ExecuteDataset("select * from mgr_sitemaps where nid=" + note.ToString()).Tables[0];
            return dt;
        }
        /// <summary>
        /// 得到最大编号
        /// </summary>
        /// <returns></returns>
        public static int GetMaxNodeID()
        {
            DataTable dt = SqlHelper.ExecuteDataset("select max(nid) from mgr_sitemaps  ").Tables[0];
            if (dt != null)
            { 
                return int.Parse(dt.Rows[0][0].ToString());
            }
            return -1;
        }
        /// <summary>
        /// 保存节点信息
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="sid"></param>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="order"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static string SaveNote(int parent, int sid, string title, string url, int order, string description)
        {
            DataSet ds = SqlHelper.ExecuteDataset("select * from mgr_sitemaps where parentid=" + parent.ToString());
            if (ds != null && ds.Tables.Count <= 0 && ds.Tables[0].Rows.Count <= 0)
            {
                return "父节点不存在!";
            }
            ds = SqlHelper.ExecuteDataset("select * from mgr_sitemaps where nid=" + sid.ToString());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)       //修改
            {
                string sql = "update  mgr_sitemaps set ";
                sql += "parentid=" + parent.ToString() + ",";
                sql += "title='" + title.ToString() + "',";
                sql += "url='" + url.ToString() + "',";
                sql += "orders=" + order.ToString() + ",";
                sql += "description='" + description.ToString() + "'";
                sql += " where nid=" + sid.ToString();

                return SqlHelper.ExecuteNonQuery(Access.SqlConnectionString, CommandType.Text, sql).ToString();
            }
            else
            {
                string sql = "insert into mgr_sitemaps(nid,parentid,title,url,orders,description)values(" + sid.ToString() + ",";
                sql += parent.ToString() + ",'" + title + "','" + url + "'," + order.ToString() + ",'" + description + "')";
                return SqlHelper.ExecuteNonQuery(Access.SqlConnectionString, CommandType.Text, sql).ToString();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="sid"></param>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="order"></param>
        /// <param name="description"></param>
        /// <param name="images"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static string SaveNote(int parent, int sid, string title, string url, int order, string description, string images, string target)
        {
            DataSet ds = SqlHelper.ExecuteDataset("select * from mgr_sitemaps where parentid=" + parent.ToString());
            if (ds != null && ds.Tables.Count <= 0 && ds.Tables[0].Rows.Count <= 0)
            {
                return "父节点不存在!";
            }
            ds = SqlHelper.ExecuteDataset("select * from mgr_sitemaps where nid=" + sid.ToString());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)       //修改
            {
                string sql = "update  mgr_sitemaps set ";
                sql += "parentid=" + parent.ToString() + ",";
                sql += "title='" + title.ToString() + "',";
                sql += "url='" + url.ToString() + "',";
                sql += "orders=" + order.ToString() + ",";
                sql += "description='" + description.ToString() + "',";
                sql += "target='" + target + "',";
                sql += "images='" + images + "'";
                sql += " where nid=" + sid.ToString();

                var m = SqlHelper.ExecuteNonQuery(Access.SqlConnectionString, CommandType.Text, sql).ToString();
                InitPermission();
                return m;
            }
            else
            {
                string sql = "insert into mgr_sitemaps(nid,parentid,title,url,orders,description,images,target)values(" + sid.ToString() + ",";
                sql += parent.ToString() + ",'" + title + "','" + url + "'," + order.ToString() + ",'" + description + "','" + images + "','" + target + "')";
                var n = SqlHelper.ExecuteNonQuery(Access.SqlConnectionString, CommandType.Text, sql).ToString();
                InitPermission();
                return n;
            }
        }
        /// <summary>
        /// 增加节点
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="sid"></param>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="order"></param>
        /// <param name="description"></param>
        /// <param name="images"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string AddNote(int parent, int sid, string title, string url, int order, string description, string images, string target)
        {
            DataSet ds = SqlHelper.ExecuteDataset("select * from mgr_sitemaps where parentid=" + parent.ToString());
            if (ds != null && ds.Tables.Count <= 0 && ds.Tables[0].Rows.Count <= 0)
            {
                return "父节点不存在!";
            }
            ds = SqlHelper.ExecuteDataset("select * from mgr_sitemaps where nid=" + sid.ToString());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0) 
            {
                return "节点已经存在!";
            }
            else
            {
                string sql = "insert into mgr_sitemaps(nid,parentid,title,url,orders,description,images,target)values(" + sid.ToString() + ",";
                sql += parent.ToString() + ",'" + title + "','" + url + "'," + order.ToString() + ",'" + description + "','" + images + "','" + target + "')";
                var n = SqlHelper.ExecuteNonQuery(Access.SqlConnectionString, CommandType.Text, sql).ToString();
                InitPermission();
                return n;
            }
        }
        /// <summary>
        /// 重新根据角色初始化权限信息
        /// </summary>
        private static void InitPermission()
        {
            return;

            //string sql = " update mgr_users a, mgr_roles b set a.rights = b.rights where a.roles = b.roleid ";
            //SqlHelper.ExecuteNonQuery(Access.SqlConnectionString, CommandType.Text, sql);

            ////初始化超级管理员权限 
            //sql = "select * from mgr_sitemaps";
            //var dt = SqlHelper.ExecuteDataset(Access.SqlConnectionString, CommandType.Text, sql);

            //string nodes = "";
            //if (dt != null)
            //{
            //    for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            //    {
            //        nodes += nodes == "" ? dt.Tables[0].Rows[i]["nid"].ToString() : "," + dt.Tables[0].Rows[i]["nid"].ToString();
            //    }
            //}
            //sql = "update mgr_users set rights ='" + nodes + "' where operatortype=1";
            //SqlHelper.ExecuteNonQuery(Access.SqlConnectionString, CommandType.Text, sql);
        }

        /// <summary>
        /// 返回会员菜单
        /// </summary>
        /// <returns></returns>
        public static string GetUNodeTree()
        {
            string treestr = "";
            DataTable dt = SqlHelper.ExecuteDataset("select * from membermenu where parentid=0 order by orders").Tables[0];
            if (dt.Rows.Count <= 0) return "";

            string treeid = "tree" + dt.Rows[0]["nid"].ToString();
            treestr += "var " + treeid + " = new WebFXTree('" + dt.Rows[0]["title"].ToString() + "');";
            treestr += "";
            treestr += treeid + ".setBehavior('explorer');\r\n";
            treestr += treeid + ".icon = '../images/xtree/images/root.gif';\r\n";
            treestr += treeid + ".openIcon = '../images/xtree/images/root.gif';\r\n";
            treestr += UNodeTree(int.Parse(dt.Rows[0]["nid"].ToString()));
            return "<script language='javascript'>" + treestr + " document.write(" + treeid + ");</script>";
        }
        /// <summary>
        /// 迭代会员菜单节点
        /// </summary>
        /// <param name="noteid"></param>
        /// <returns></returns>
        private static string UNodeTree(int noteid)
        {
            string item = "";
            DataSet ds = SqlHelper.ExecuteDataset("select * from membermenu where parentid=" + noteid.ToString());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string tid = "";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tid = "tree" + ds.Tables[0].Rows[i]["nid"].ToString();
                    item += " var  " + tid + "= new WebFXTreeItem('" + ds.Tables[0].Rows[i]["title"].ToString() + "-" + ds.Tables[0].Rows[i]["nid"].ToString() + "','mmenuadd.aspx?nid=" + ds.Tables[0].Rows[i]["nid"].ToString().Trim() + "');\r\n";
                    item += " tree" + noteid.ToString() + ".add(" + tid + ");\r\n";

                    item += UNodeTree(int.Parse(ds.Tables[0].Rows[i]["nid"].ToString()));


                }
            }
            return item;
        }
        /// <summary>
        /// 返回板块菜单
        /// </summary>
        /// <returns></returns>
        public static string GetCNodeTree()
        {
            string treestr = "";
            DataTable dt = SqlHelper.ExecuteDataset("select * from Columns where parentid=0 order by orders").Tables[0];
            if (dt.Rows.Count <= 0) return "";

            string treeid = "tree" + dt.Rows[0]["nid"].ToString();
            treestr += "var " + treeid + " = new WebFXTree('" + dt.Rows[0]["title"].ToString() + "');";
            treestr += "";
            treestr += treeid + ".setBehavior('explorer');\r\n";
            treestr += treeid + ".icon = '../images/xtree/images/root.gif';\r\n";
            treestr += treeid + ".openIcon = '../images/xtree/images/root.gif';\r\n";
            treestr += CNodeTree(int.Parse(dt.Rows[0]["nid"].ToString()));
            return "<script language='javascript'>" + treestr + " document.write(" + treeid + ");</script>";
        }
        /// <summary>
        /// 迭代板块菜单节点
        /// </summary>
        /// <param name="noteid"></param>
        /// <returns></returns>
        private static string CNodeTree(int noteid)
        {
            string item = "";
            DataSet ds = SqlHelper.ExecuteDataset("select * from Columns where parentid=" + noteid.ToString());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string tid = "";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tid = "tree" + ds.Tables[0].Rows[i]["nid"].ToString();
                    item += " var  " + tid + "= new WebFXTreeItem('" + ds.Tables[0].Rows[i]["title"].ToString() + "-" + ds.Tables[0].Rows[i]["nid"].ToString() + "','columnadd.aspx?nid=" + ds.Tables[0].Rows[i]["nid"].ToString().Trim() + "');\r\n";
                    item += " tree" + noteid.ToString() + ".add(" + tid + ");\r\n";

                    item += CNodeTree(int.Parse(ds.Tables[0].Rows[i]["nid"].ToString()));


                }
            }
            return item;
        }
    }
}
