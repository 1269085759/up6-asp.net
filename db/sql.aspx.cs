﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using up6.db.database;

namespace up6.db
{
    public partial class sql : System.Web.UI.Page
    {
        public string m_dbName;	//数据库名称
        public string m_dbUser;
        public string m_dbPass;

        public void createTable() 
        {
            clear();
            createUpload();
            createDown();
        }

        public void clear() 
        {
            string[] sql_clear = { 
                                     "if object_id('fd_files_check') is not null drop procedure fd_files_check"
                                    ,"if object_id('fd_add_batch') is not null drop procedure fd_add_batch"
                                    ,"if object_id('up6_files') is not null drop table up6_files"
                                    ,"if object_id('up6_folders') is not null drop table up6_folders"
                                    ,"if object_id('down_files') is not null drop table down_files"
                                    ,"if object_id('down_folders') is not null drop table down_folders",
                                 };
            
            DbHelper db = new DbHelper();
            DbCommand cmd;

            foreach(string str in sql_clear)
            {
                cmd = db.GetCommand(str);
                db.ExecuteNonQuery(cmd);
            }
        }

        public void createUpload() 
        {
            string path = Server.MapPath("/sql");
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
                            StreamReader st = finf.OpenText();
                            string str = st.ReadToEnd();

                            DbHelper db = new DbHelper();
                            DbCommand cmd = db.GetCommand(str);
                            db.ExecuteNonQuery(cmd);
                            st.Close();
                        }
                    }
                }
            }
        }

        public void createDown()
        {
            string path = Server.MapPath("/sql.down");
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
                            StreamReader st = finf.OpenText();
                            string s = st.ReadToEnd();

                            DbHelper db = new DbHelper();
                            DbCommand cmd = db.GetCommand(s);
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

        protected void Page_Load(object sender, EventArgs e)
        {
            showConfig();

            createTable();
            Response.Write("创建成功");
        }
    }
}