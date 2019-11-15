using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Web;
using up6.db.database;
using up6.db.model;
using up6.filemgr.app;

namespace up6.db.biz.folder
{
    /// <summary>
    /// 修复pidRoot为空的问题
    /// </summary>
    public class fd_scan
    {
        protected DbHelper db;
        protected DbCommand cmd_add_f = null;
        protected DbCommand cmd_add_fd = null;
        public FileInf root = null;//根节点

        public fd_scan()
        {
            this.db = new DbHelper();
        }
        public void makeCmdF()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into up6_files(");
            sb.Append(" f_id");
            sb.Append(",f_pid");
            sb.Append(",f_pidRoot");
            sb.Append(",f_fdTask");
            sb.Append(",f_fdChild");
            sb.Append(",f_sizeLoc");
            sb.Append(",f_uid");
            sb.Append(",f_nameLoc");
            sb.Append(",f_nameSvr");
            sb.Append(",f_pathLoc");
            sb.Append(",f_pathSvr");
            sb.Append(",f_pathRel");
            sb.Append(",f_md5");
            sb.Append(",f_lenLoc");
            sb.Append(",f_lenSvr");
            sb.Append(",f_perSvr");
            sb.Append(",f_complete");

            sb.Append(") values (");

            sb.Append(" @f_id");
            sb.Append(",@f_pid");
            sb.Append(",@f_pidRoot");
            sb.Append(",@f_fdTask");
            sb.Append(",@f_fdChild");
            sb.Append(",@f_sizeLoc");
            sb.Append(",@f_uid");
            sb.Append(",@f_nameLoc");
            sb.Append(",@f_nameSvr");
            sb.Append(",@f_pathLoc");
            sb.Append(",@f_pathSvr");
            sb.Append(",@f_pathRel");
            sb.Append(",@f_md5");
            sb.Append(",@f_lenLoc");
            sb.Append(",@f_lenSvr");
            sb.Append(",@f_perSvr");
            sb.Append(",@f_complete");
            sb.Append(") ;");

            this.cmd_add_f = this.db.connection.CreateCommand();
            this.cmd_add_f.CommandText = sb.ToString();
            this.cmd_add_f.CommandType = System.Data.CommandType.Text;

            this.db.AddString(ref cmd_add_f, "@f_id", string.Empty, 32);
            this.db.AddString(ref cmd_add_f, "@f_pid", string.Empty, 32);
            this.db.AddString(ref cmd_add_f, "@f_pidRoot", string.Empty, 32);
            this.db.AddBool  (ref cmd_add_f, "@f_fdTask", false);
            this.db.AddString(ref cmd_add_f, "@f_sizeLoc", string.Empty, 32);
            this.db.AddBool  (ref cmd_add_f, "@f_fdChild", true);
            this.db.AddInt   (ref cmd_add_f, "@f_uid", 0);
            this.db.AddString(ref cmd_add_f, "@f_nameLoc", string.Empty, 255);
            this.db.AddString(ref cmd_add_f, "@f_nameSvr", string.Empty, 255);
            this.db.AddString(ref cmd_add_f, "@f_pathLoc", string.Empty, 255);
            this.db.AddString(ref cmd_add_f, "@f_pathSvr", string.Empty, 255);
            this.db.AddString(ref cmd_add_f, "@f_pathRel", string.Empty, 255);
            this.db.AddString(ref cmd_add_f, "@f_md5", string.Empty,40);
            this.db.AddInt64 (ref cmd_add_f, "@f_lenLoc", 0);
            this.db.AddInt64 (ref cmd_add_f, "@f_lenSvr", 0);
            this.db.AddString(ref cmd_add_f, "@f_perSvr", "0%",6);
            this.db.AddBool  (ref cmd_add_f, "@f_complete", false);
            this.cmd_add_f.Prepare();
        }

        public void makeCmdFD() {

            StringBuilder sb = new StringBuilder();
            sb.Append("insert into up6_folders(");
            sb.Append(" f_id");
            sb.Append(",f_pid");
            sb.Append(",f_pidRoot");
            sb.Append(",f_uid");
            sb.Append(",f_nameLoc");
            sb.Append(",f_pathLoc");
            sb.Append(",f_pathSvr");
            sb.Append(",f_pathRel");
            sb.Append(",f_complete");

            sb.Append(") values (");

            sb.Append(" @f_id");
            sb.Append(",@f_pid");
            sb.Append(",@f_pidRoot");
            sb.Append(",@f_uid");
            sb.Append(",@f_name");
            sb.Append(",@f_pathLoc");
            sb.Append(",@f_pathSvr");
            sb.Append(",@f_pathRel");
            sb.Append(",@f_complete");
            sb.Append(") ;");

            this.cmd_add_fd = this.db.connection.CreateCommand();
            this.cmd_add_fd.CommandText = sb.ToString();
            this.cmd_add_fd.CommandType = System.Data.CommandType.Text;

            this.db.AddString(ref cmd_add_fd, "@f_id", string.Empty, 32);
            this.db.AddString(ref cmd_add_fd, "@f_pid", string.Empty, 32);
            this.db.AddString(ref cmd_add_fd, "@f_pidRoot", string.Empty, 32);
            this.db.AddInt   (ref cmd_add_fd, "@f_uid", 0);
            this.db.AddString(ref cmd_add_fd, "@f_name", string.Empty, 255);
            this.db.AddString(ref cmd_add_fd, "@f_pathLoc", string.Empty, 255);
            this.db.AddString(ref cmd_add_fd, "@f_pathSvr", string.Empty, 255);
            this.db.AddString(ref cmd_add_fd, "@f_pathRel", string.Empty, 255);
            this.db.AddBool  (ref cmd_add_fd, "@f_complete", false);
            this.cmd_add_fd.Prepare();
        }

        protected void GetAllFiles(FileInf parent, string root)
        {
            DirectoryInfo dir = new DirectoryInfo(parent.pathSvr);
            FileInfo[] allFile = dir.GetFiles();
            foreach (FileInfo fi in allFile)
            {
                FileInf fl = new FileInf();

                fl.id = Guid.NewGuid().ToString("N");
                fl.pid = parent.id;
                fl.pidRoot = this.root.id;
                fl.nameSvr = fi.Name;
                fl.nameLoc = fi.Name;
                fl.pathSvr = fi.FullName;
                fl.pathSvr = fl.pathSvr.Replace("\\", "/");
                fl.pathRel = fl.pathSvr.Remove(0, root.Length + 1);
                fl.pathRel = PathTool.combin(parent.pathRel, fl.pathRel);
                fl.lenSvr = fi.Length;
                fl.lenLoc = fl.lenSvr;
                fl.perSvr = "100%";
                fl.complete = true;
                this.save_file(fl);
            }
            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                FileInf fd = new FileInf();
                fd.id = Guid.NewGuid().ToString("N");
                fd.pid = parent.id;
                fd.pidRoot = this.root.id;
                fd.nameSvr = d.Name;
                fd.nameLoc = d.Name;
                fd.pathSvr = d.FullName;
                fd.pathSvr = fd.pathSvr.Replace("\\", "/");
                fd.pathRel = fd.pathSvr.Remove(0, root.Length + 1);
                fd.pathRel = PathTool.combin(parent.pathRel, fd.pathRel);
                fd.perSvr = "100%";
                fd.complete = true;
                this.save_folder(fd);

                this.GetAllFiles(fd, root);
            }
        }

        protected void save_file(FileInf f)
        {
            this.cmd_add_f.Parameters["@f_id"].Value = f.id;
            this.cmd_add_f.Parameters["@f_pid"].Value = f.pid;
            this.cmd_add_f.Parameters["@f_pidRoot"].Value = f.pidRoot;
            this.cmd_add_f.Parameters["@f_fdTask"].Value = f.fdTask;
            this.cmd_add_f.Parameters["@f_sizeLoc"].Value = f.sizeLoc;
            this.cmd_add_f.Parameters["@f_fdChild"].Value = true;
            this.cmd_add_f.Parameters["@f_uid"].Value = f.uid;
            this.cmd_add_f.Parameters["@f_nameLoc"].Value = f.nameLoc;
            this.cmd_add_f.Parameters["@f_nameSvr"].Value = f.nameSvr;
            this.cmd_add_f.Parameters["@f_pathLoc"].Value = f.pathLoc;
            this.cmd_add_f.Parameters["@f_pathSvr"].Value = f.pathSvr;
            this.cmd_add_f.Parameters["@f_pathRel"].Value = f.pathRel;
            this.cmd_add_f.Parameters["@f_md5"].Value = f.md5;
            this.cmd_add_f.Parameters["@f_lenLoc"].Value = f.lenLoc;
            this.cmd_add_f.Parameters["@f_lenSvr"].Value = f.lenSvr;
            this.cmd_add_f.Parameters["@f_perSvr"].Value = f.perSvr;
            this.cmd_add_f.Parameters["@f_complete"].Value = f.complete;

            cmd_add_f.ExecuteNonQuery();
        }

        protected void save_folder(FileInf f)
        {
            this.cmd_add_fd.Parameters["@f_id"].Value = f.id;
            this.cmd_add_fd.Parameters["@f_pid"].Value = f.pid;
            this.cmd_add_fd.Parameters["@f_pidRoot"].Value = f.pidRoot;
            this.cmd_add_fd.Parameters["@f_uid"].Value = f.uid;
            this.cmd_add_fd.Parameters["@f_name"].Value = f.nameSvr;
            this.cmd_add_fd.Parameters["@f_pathLoc"].Value = f.pathLoc;
            this.cmd_add_fd.Parameters["@f_pathSvr"].Value = f.pathSvr;
            this.cmd_add_fd.Parameters["@f_pathRel"].Value = f.pathRel;
            this.cmd_add_fd.Parameters["@f_complete"].Value = f.complete;

            cmd_add_fd.ExecuteNonQuery();
        }

        public void scan(FileInf inf, string root)
        {
            this.db.connection.Open();
            this.makeCmdF();
            this.makeCmdFD();

            this.GetAllFiles(inf,root);
            this.db.connection.Close();

            this.cmd_add_f.Dispose();
            this.cmd_add_fd.Dispose();
        }
    }
}