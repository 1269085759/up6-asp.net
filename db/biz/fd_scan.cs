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
    /// 扫描文件夹层级结构，并添加到数据库
    /// </summary>
    public class fd_scan
    {
        protected DbHelper db;
        protected DbCommand cmd_cover = null;
        public FileInf root = null;//根节点
        /// <summary>
        /// 文件列表
        /// </summary>
        protected List<FileInf> m_files = new List<FileInf>();
        /// <summary>
        /// 目录列表
        /// </summary>
        protected List<FileInf> m_folders = new List<FileInf>();

        public fd_scan()
        {
            this.db = new DbHelper();
        }

        protected virtual void makeCmdCover()
        {
            string sql = "update up6_files set f_deleted=1 where f_pathRel=@pathRel";

            this.cmd_cover = this.db.connection.CreateCommand();
            this.cmd_cover.CommandText = sql;
            this.cmd_cover.CommandType = System.Data.CommandType.Text;

            this.db.AddString(ref cmd_cover, "@pathRel", string.Empty, 512);
            this.cmd_cover.Prepare();
        }

        protected virtual void cover_file(string pathRel) {
            this.cmd_cover.Parameters["@pathRel"].Value = pathRel;
            this.cmd_cover.ExecuteNonQuery();
        }

        protected void GetAllFiles(FileInf parent, string root)
        {
            DirectoryInfo dir = new DirectoryInfo(parent.pathSvr);
            FileInfo[] allFile = dir.GetFiles();
            //获取文件
            foreach (FileInfo fi in allFile)
            {
                FileInf fl = new FileInf();

                fl.id = Guid.NewGuid().ToString("N");
                fl.pid = parent.id;
                fl.uid = parent.uid;
                fl.pidRoot = this.root.id;
                fl.nameSvr = fi.Name;
                fl.nameLoc = fi.Name;
                fl.pathSvr = fi.FullName;
                fl.pathSvr = fl.pathSvr.Replace("\\", "/");
                fl.pathRel = fl.pathSvr.Remove(0, root.Length + 1);
                fl.pathRel = PathTool.combin(parent.pathRel, fl.nameLoc);
                fl.lenSvr = fi.Length;
                fl.lenLoc = fl.lenSvr;
                fl.sizeLoc = this.BytesToString(fl.lenSvr);
                fl.perSvr = "100%";
                fl.complete = true;
                this.m_files.Add(fl);
                //this.save_file(fl);
            }
            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                FileInf fd = new FileInf();
                fd.id = Guid.NewGuid().ToString("N");
                fd.pid = parent.id;
                fd.uid = parent.uid;
                fd.pidRoot = this.root.id;
                fd.nameSvr = d.Name;
                fd.nameLoc = d.Name;
                fd.pathSvr = d.FullName;
                fd.pathSvr = fd.pathSvr.Replace("\\", "/");
                fd.pathRel = fd.pathSvr.Remove(0, root.Length + 1);
                fd.pathRel = PathTool.combin(parent.pathRel, fd.nameLoc);
                fd.perSvr = "100%";
                fd.complete = true;
                this.m_folders.Add(fd);

                this.GetAllFiles(fd, root);
            }
        }

        protected void getAllFiles(FileInf parent,string root,ref List<string> files)
        {
            DirectoryInfo dir = new DirectoryInfo(parent.pathSvr);
            FileInfo[] allFile = dir.GetFiles();
            foreach (FileInfo fi in allFile)
            {
                FileInf fl = new FileInf();

                //fl.id = Guid.NewGuid().ToString("N");
                //fl.pid = parent.id;
                //fl.pidRoot = this.root.id;
                fl.nameSvr = fi.Name;
                fl.nameLoc = fi.Name;
                fl.pathSvr = fi.FullName;
                fl.pathSvr = fl.pathSvr.Replace("\\", "/");
                fl.pathRel = fl.pathSvr.Remove(0, root.Length + 1);
                fl.pathRel = PathTool.combin(parent.pathRel, fl.nameLoc);
                //fl.lenSvr = fi.Length;
                //fl.lenLoc = fl.lenSvr;
                //fl.sizeLoc = this.BytesToString(fl.lenSvr);
                //fl.perSvr = "100%";
                //fl.complete = true;
                files.Add(fl.pathRel);
            }
            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                FileInf fd = new FileInf();
                //fd.id = Guid.NewGuid().ToString("N");
                //fd.pid = parent.id;
                //fd.pidRoot = this.root.id;
                fd.nameSvr = d.Name;
                fd.nameLoc = d.Name;
                fd.pathSvr = d.FullName;
                fd.pathSvr = fd.pathSvr.Replace("\\", "/");
                fd.pathRel = fd.pathSvr.Remove(0, root.Length + 1);
                fd.pathRel = PathTool.combin(parent.pathRel, fd.nameLoc);
                //fd.perSvr = "100%";
                //fd.complete = true;

                this.getAllFiles(fd, root,ref files);
            }
        }

        /// <summary>
        /// 批量添加文件
        /// </summary>
        /// <param name="con"></param>
        protected virtual void save_files()
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

            var cmd = db.GetCommand(sb.ToString());

            db.AddString(ref cmd, "@f_id", string.Empty, 32);
            db.AddString(ref cmd, "@f_pid", string.Empty, 32);
            db.AddString(ref cmd, "@f_pidRoot", string.Empty, 32);
            db.AddBool(ref cmd, "@f_fdTask", false);
            db.AddString(ref cmd, "@f_sizeLoc", string.Empty, 32);
            db.AddBool(ref cmd, "@f_fdChild", true);
            db.AddInt(ref cmd, "@f_uid", 0);
            db.AddString(ref cmd, "@f_nameLoc", string.Empty, 255);
            db.AddString(ref cmd, "@f_nameSvr", string.Empty, 255);
            db.AddString(ref cmd, "@f_pathLoc", string.Empty, 255);
            db.AddString(ref cmd, "@f_pathSvr", string.Empty, 255);
            db.AddString(ref cmd, "@f_pathRel", string.Empty, 255);
            db.AddString(ref cmd, "@f_md5", string.Empty, 40);
            db.AddInt64(ref cmd, "@f_lenLoc", 0);
            db.AddInt64(ref cmd, "@f_lenSvr", 0);
            db.AddString(ref cmd, "@f_perSvr", "100%", 6);
            db.AddBool(ref cmd, "@f_complete", true);
            cmd.Prepare();

            foreach(var f in this.m_files)
            {
                cmd.Parameters["@f_id"].Value = f.id;
                cmd.Parameters["@f_pid"].Value = f.pid;
                cmd.Parameters["@f_pidRoot"].Value = f.pidRoot;
                cmd.Parameters["@f_sizeLoc"].Value = f.sizeLoc;
                cmd.Parameters["@f_uid"].Value = f.uid;
                cmd.Parameters["@f_nameLoc"].Value = f.nameLoc;
                cmd.Parameters["@f_nameSvr"].Value = f.nameSvr;
                cmd.Parameters["@f_pathLoc"].Value = f.pathLoc;
                cmd.Parameters["@f_pathSvr"].Value = f.pathSvr;
                cmd.Parameters["@f_pathRel"].Value = f.pathRel;
                cmd.Parameters["@f_md5"].Value = f.md5;
                cmd.Parameters["@f_lenLoc"].Value = f.lenLoc;
                cmd.Parameters["@f_lenSvr"].Value = f.lenSvr;
                cmd.ExecuteNonQuery();
            }
            cmd.Dispose();
        }

        /// <summary>
        /// 批量添加目录
        /// </summary>
        /// <param name="con"></param>
        protected virtual void save_folders()
        {
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
            sb.Append(",@f_nameLoc");
            sb.Append(",@f_pathLoc");
            sb.Append(",@f_pathSvr");
            sb.Append(",@f_pathRel");
            sb.Append(",@f_complete");
            sb.Append(") ;");

            var cmd = db.GetCommand(sb.ToString());

            db.AddString(ref cmd, "@f_id", string.Empty, 32);
            db.AddString(ref cmd, "@f_pid", string.Empty, 32);
            db.AddString(ref cmd, "@f_pidRoot", string.Empty, 32);
            db.AddInt(ref cmd, "@f_uid", 0);
            db.AddString(ref cmd, "@f_nameLoc", string.Empty, 255);
            db.AddString(ref cmd, "@f_pathLoc", string.Empty, 255);
            db.AddString(ref cmd, "@f_pathSvr", string.Empty, 255);
            db.AddString(ref cmd, "@f_pathRel", string.Empty, 255);
            db.AddBool(ref cmd, "@f_complete", true);
            cmd.Prepare();

            foreach (var f in this.m_folders)
            {
                cmd.Parameters["@f_id"].Value = f.id;
                cmd.Parameters["@f_pid"].Value = f.pid;
                cmd.Parameters["@f_pidRoot"].Value = f.pidRoot;
                cmd.Parameters["@f_uid"].Value = f.uid;
                cmd.Parameters["@f_nameLoc"].Value = f.nameLoc;
                cmd.Parameters["@f_pathLoc"].Value = f.pathLoc;
                cmd.Parameters["@f_pathSvr"].Value = f.pathSvr;
                cmd.Parameters["@f_pathRel"].Value = f.pathRel;
                cmd.ExecuteNonQuery();
            }
            cmd.Dispose();
        }

        string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        /// <summary>
        /// 覆盖同名文件
        /// </summary>
        /// <param name="inf"></param>
        /// <param name="pathParent"></param>
        public void cover(FileInf inf,string pathParent)
        {
            List<string> files = new List<string>();
            this.getAllFiles(inf, pathParent, ref files);

            this.db.connection.Open();
            this.makeCmdCover();
            foreach(string f in files)
            {
                this.cover_file(f);
            }
            this.db.connection.Close();
        }

        public void scan(FileInf inf, string root)
        {
            //扫描文件和目录
            this.GetAllFiles(inf,root);

            this.db.connection.Open();
            this.save_files();//保存文件列表
            this.save_folders();//保存目录列表
            this.db.connection.Close();

        }
    }
}