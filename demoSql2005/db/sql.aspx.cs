using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace up6.demoSql2005.db
{
    public partial class sql : System.Web.UI.Page
    {
        public string m_dbName;	//数据库名称
        public string m_dbUser;
        public string m_dbPass;

        /*public string GetConStr()
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

        public string createDataBase() 
        {
            SqlConnection con = new SqlConnection(this.GetConStr());
            SqlCommand cmd = con.CreateCommand();

            string sb = "if db_id('" + m_dbName + "') is null create database " + m_dbName;

            cmd.Connection.Open();
            cmd.CommandText = sb;
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();

            createTable();
            return "";
        }*/

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

        public void showConfig()
        {
            var str = System.Configuration.ConfigurationManager.ConnectionStrings["sql2005"];
            string cs = str.ConnectionString;
            readConfig(cs);
        }

        public void readConfig(string str) 
        {
            string ic = "Initial Catalog";
            string uid = "User Id";
            string password = "Password";
            List<string> lst = new List<string>();
            lst.Add(ic);
            lst.Add(uid);
            lst.Add(password);

            foreach (string s in lst)
            {
                int index = str.IndexOf(s);
                int start = index + s.Length + 1;

                int end = str.IndexOf(";", start);
                string name = str.Substring(start, end - start);

                if (string.Equals(s, ic))
                {
                    this.m_dbName = name;
                }
                else if (string.Equals(s, uid))
                {
                    this.m_dbUser = name;
                }
                else
                {
                    this.m_dbPass = name;
                }
            }
        }
        
        public void updateConfig(string id,string pwd,string dbname) 
        {
            XmlDocument doc = new XmlDocument();
            string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Web.config";

            doc.Load(strFileName);

            bool result = false;
            XmlNodeList nodes = doc.GetElementsByTagName("add");
            for (int i = 0; i < nodes.Count; i++)
            {   
                XmlAttribute name = nodes[i].Attributes["name"];
                if (name != null)
                {
                    if (name.Value == "sql2005")
                    {
                        name = nodes[i].Attributes["connectionString"];
                        string str = name.Value;
                        name.Value = replaceAll(str,id,pwd,dbname);
                        
                        if(!string.Equals(str,name.Value))
                        {
                            result = true;
                        }
                        break;
                    }
                }
            }
            if (result)
            {
                doc.Save(strFileName);
            }
        }

        public string replaceAll(string str,string id,string pwd,string dbname)
        {
            string ic = "Initial Catalog";
            string uid = "User Id";
            string password = "Password";
            List<string> lst = new List<string>();
            lst.Add(ic);
            lst.Add(uid);
            lst.Add(password);

            foreach(string s in lst)
            {
                int index = str.IndexOf(s);
                int start = index + s.Length + 1;

                int end = str.IndexOf(";", start);
                string name = str.Substring(start, end - start);

                if(string.Equals(s,ic))
                {
                    str = str.Replace(name, dbname);
                }
                else if (string.Equals(s, uid))
                {
                    str = str.Replace(name, id);
                }
                else 
                {
                    str = str.Replace(name, pwd);
                }
            }
            return str;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            showConfig();

            string id = Request.Form["uid"];
            string pwd = Request.Form["pwd"];
            string dbname = Request.Form["dbname"];

            if (id != null)
            {
                this.m_dbUser = id;
            }
            if (pwd != null)
            {
                this.m_dbPass = pwd;
            }
            if (dbname != null)
            {
                this.m_dbName = dbname;
            }
            updateConfig(this.m_dbUser,this.m_dbPass,this.m_dbName);

            if (id != null || pwd != null || dbname != null)
            {
                createTable();
                Response.Write("创建成功");
            }
            //createDataBase();
            //Response.Write("创建成功");
        }
    }
}