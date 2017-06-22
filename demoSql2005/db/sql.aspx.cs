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

            if (!checkDataBase(m_dbName, cmd)) 
            {
                cmd.Connection.Open();
                cmd.CommandText = "CREATE DATABASE " + m_dbName;
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            //con.ChangeDatabase(m_dbName);
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
                            if (checkTable(fname)) 
                            {
                                string dt = "drop table " + fname;
                                cmd = db.GetCommand(dt);
                                db.ExecuteNonQuery(cmd);
                            }

                            if(checkProcedure(fname))
                            {
                                string dp = "drop procedure " + fname;
                                cmd = db.GetCommand(dp);
                                db.ExecuteNonQuery(cmd);
                            }

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
                            if (checkTable(fname)) 
                            {
                                string dt = "drop table " + fname;
                                cmd = db.GetCommand(dt);
                                db.ExecuteNonQuery(cmd);
                            }

                            if(checkProcedure(fname))
                            {
                                string dp = "drop procedure " + fname;
                                cmd = db.GetCommand(dp);
                                db.ExecuteNonQuery(cmd);
                            }

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

        public bool checkDataBase(string name, SqlCommand cmd) 
        {
            bool result = false;
            string sb = "select * from master.dbo.sysdatabases where name='" + name + "'";
            cmd.CommandText = sb;
            cmd.Connection.Open();
            Object obj = cmd.ExecuteScalar();
            cmd.Connection.Close();
            result = obj != null;
            return result;
        }

        public bool checkTable(string name) 
        {
            bool result = false;
            string sb = "select object_id from sys.tables where name='" + name + "'";
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb);
            object obj = db.ExecuteScalar(cmd);
            result = obj != null;
            return result;
        }

        public bool checkProcedure(string name)
        {
            bool result = false;
            string sb = "select name from sysobjects where xtype = 'P' and name = '" + name + "'";
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb);
            object obj = db.ExecuteScalar(cmd);
            result = obj != null;
            return result;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            createDataBase();
            Response.Write("创建成功");
        }
    }
}