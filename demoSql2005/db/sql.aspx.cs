using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace up6.demoSql2005.db
{
    public partial class sql : System.Web.UI.Page
    {
        private string m_dbName;	//数据库名称

        public string GetConStr()
        {
            var str = System.Configuration.ConfigurationManager.ConnectionStrings["sql2005"];
            string cs = str.ConnectionString;
            string rs = replace(cs);
            return rs;
        }

        public string replace(string str) 
        {
            string ic = "Initial Catalog";
            int index = str.IndexOf(ic);
            int start = index + ic.Length + 1;
            int end = str.IndexOf(";", start);
            string dbname = str.Substring(start, end - start);
            this.m_dbName = dbname;
            string mst = "master";
            string rs = str.Replace(dbname, mst);
            return rs;
        }

        public void createDataBase() 
        {
            SqlConnection con = new SqlConnection(this.GetConStr());
            SqlCommand cmd = con.CreateCommand();

            string sb = "if db_id('" + m_dbName + "') is null create database " + m_dbName;

            cmd.Connection.Open();
            cmd.CommandText = sb;
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();

            createTable();
        }

        public void createTable() 
        {
            createUpload();
            createDown();
        }

        public void createUpload() 
        {
            string path = Server.MapPath("/demoSql2005/sql");
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                FileInfo[] inf = dir.GetFiles();
                if (inf.Length > 0)
                {
                    foreach (FileInfo finf in inf)
                    {
                        if (finf.Extension.Equals(".sql"))
                        {
                            string fname = finf.Name;
                            fname = fname.Replace(".sql","");

                            DbHelper db = new DbHelper();
                            DbCommand cmd;

                            string dt = "if exists(select object_id from sys.tables where name='" + fname + "') drop table " + fname;
                            cmd = db.GetCommand(dt);
                            db.ExecuteNonQuery(cmd);

                            string dp = "if exists(select name from sysobjects where xtype='P' and name='" + fname + "') drop procedure " + fname;
                            cmd = db.GetCommand(dp);
                            db.ExecuteNonQuery(cmd);

                            StreamReader st = finf.OpenText();
                            string s = st.ReadToEnd();
                            cmd = db.GetCommand(s);
                            db.ExecuteNonQuery(cmd);
                            st.Close();
                        }
                    }
                }
            }
        }

        public void createDown()
        {
            string path = Server.MapPath("/demoSql2005/sql.down");
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                FileInfo[] inf = dir.GetFiles();
                if (inf.Length > 0)
                {
                    foreach (FileInfo finf in inf)
                    {
                        if (finf.Extension.Equals(".sql"))
                        {
                            string fname = finf.Name;
                            fname = fname.Replace(".sql","");

                            DbHelper db = new DbHelper();
                            DbCommand cmd;

                            string dt = "if exists(select object_id from sys.tables where name='" + fname + "') drop table " + fname;
                            cmd = db.GetCommand(dt);
                            db.ExecuteNonQuery(cmd);

                            string dp = "if exists(select name from sysobjects where xtype='P' and name='" + fname + "') drop procedure " + fname;
                            cmd = db.GetCommand(dp);
                            db.ExecuteNonQuery(cmd);

                            StreamReader st = finf.OpenText();
                            string s = st.ReadToEnd();
                            cmd = db.GetCommand(s);
                            db.ExecuteNonQuery(cmd);
                            st.Close();
                        }
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            createDataBase();
            Response.Write("创建成功");
        }
    }
}