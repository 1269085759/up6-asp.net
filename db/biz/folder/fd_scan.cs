using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Web;
using up6.db.database;
using up6.db.model;

namespace up6.db.biz.folder
{
    public class fd_scan
    {
        protected DbHelper db;
        protected DbCommand cmd_add_f = null;
        protected DbCommand cmd_add_fd = null;

        public fd_scan()
        {
            this.db = new DbHelper();
        }

        protected void GetAllFiles(FileInf inf, string root)
        {
            DirectoryInfo dir = new DirectoryInfo(inf.pathSvr);
            FileInfo[] allFile = dir.GetFiles();
            foreach (FileInfo fi in allFile)
            {
                FileInf fl = new FileInf();

                fl.id = Guid.NewGuid().ToString("N");
                fl.pid = inf.id;
                fl.pidRoot = inf.pidRoot;
                fl.nameSvr = fi.Name;
                fl.pathSvr = fi.FullName;
                fl.pathSvr = fl.pathSvr.Replace("\\", "/");
                fl.pathRel = fl.pathSvr.Remove(0, root.Length + 1);
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
                fd.pid = inf.id;
                fd.pidRoot = inf.pidRoot;
                fd.nameSvr = d.Name;
                fd.pathSvr = d.FullName;
                fd.pathSvr = fd.pathSvr.Replace("\\", "/");
                fd.pathRel = fd.pathSvr.Remove(0, root.Length + 1);
                fd.perSvr = "100%";
                fd.complete = true;
                this.save_folder(fd);

                this.GetAllFiles(fd, root);
            }
        }

        protected void save_file(FileInf f)
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

            
            this.cmd_add_f = this.db.GetCommand(sb.ToString());

            this.db.AddString(ref cmd_add_f, "@f_id", f.id, 32);
            this.db.AddString(ref cmd_add_f, "@f_pid", f.pid, 32);
            this.db.AddString(ref cmd_add_f, "@f_pidRoot", f.pidRoot, 32);
            this.db.AddBool(ref cmd_add_f, "@f_fdTask", f.fdTask);
            this.db.AddString(ref cmd_add_f, "@f_sizeLoc", f.sizeLoc, 32);
            this.db.AddBool(ref cmd_add_f, "@f_fdChild", true);
            this.db.AddInt(ref cmd_add_f, "@f_uid", f.uid);
            this.db.AddString(ref cmd_add_f, "@f_nameLoc", f.nameLoc, 255);
            this.db.AddString(ref cmd_add_f, "@f_nameSvr", f.nameSvr, 255);
            this.db.AddString(ref cmd_add_f, "@f_pathLoc", f.pathLoc, 255);
            this.db.AddString(ref cmd_add_f, "@f_pathSvr", f.pathSvr, 255);
            this.db.AddString(ref cmd_add_f, "@f_pathRel", f.pathRel, 255);
            this.db.AddString(ref cmd_add_f, "@f_md5", f.md5, 40);
            this.db.AddInt64(ref cmd_add_f, "@f_lenLoc", f.lenLoc);
            this.db.AddInt64(ref cmd_add_f, "@f_lenSvr", f.lenSvr);
            this.db.AddString(ref cmd_add_f, "@f_perSvr", f.perSvr, 6);
            this.db.AddBool(ref cmd_add_f, "@f_complete", f.complete);

            cmd_add_f.ExecuteNonQuery();
        }

        protected void save_folder(FileInf f)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into up6_folders(");
            sb.Append(" fd_id");
            sb.Append(",fd_pid");
            sb.Append(",fd_pidRoot");
            sb.Append(",fd_uid");
            sb.Append(",fd_name");
            sb.Append(",fd_pathLoc");
            sb.Append(",fd_pathSvr");
            sb.Append(",fd_pathRel");
            sb.Append(",fd_complete");

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

            this.cmd_add_fd = this.db.GetCommand(sb.ToString());
            this.db.AddString(ref cmd_add_fd, "@f_id", f.id, 32);
            this.db.AddString(ref cmd_add_fd, "@f_pid", f.pid, 32);
            this.db.AddString(ref cmd_add_fd, "@f_pidRoot", f.pidRoot, 32);
            this.db.AddInt(ref cmd_add_fd, "@f_uid", f.uid);
            this.db.AddString(ref cmd_add_fd, "@f_name", f.nameSvr, 255);
            this.db.AddString(ref cmd_add_fd, "@f_pathLoc", f.pathLoc, 255);
            this.db.AddString(ref cmd_add_fd, "@f_pathSvr", f.pathSvr, 255);
            this.db.AddString(ref cmd_add_fd, "@f_pathRel", f.pathRel, 255);
            this.db.AddBool(ref cmd_add_fd, "@f_complete", f.complete);
            cmd_add_fd.ExecuteNonQuery();
        }

        public void scan(FileInf inf, string root)
        {
            this.db.connection.Open();
            this.GetAllFiles(inf,root);
            this.db.connection.Close();
        }

    }
}