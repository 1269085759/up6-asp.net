using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using up6.db.database;
using up6.db.model;
using up6.db.utils;

namespace up6.db.biz.folder
{
    public class fd_appender
    {
        protected DbHelper db;
        protected DbCommand cmd_add_f = null;
        protected DbCommand cmd_add_fd = null;
        protected PathBuilder pb = new PathBuilderMd5();
        Dictionary<string/*md5*/, FileInf> svr_files = new Dictionary<string, FileInf>();
        public fd_root m_root;//
        private string m_md5s = "0";

        public fd_appender()
        {
            this.db = new DbHelper();
        }

        protected virtual void save_file(FileInf f)
        {
            if (this.cmd_add_f == null)
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
                sb.Append(",@f_perSvr");
                sb.Append(",@f_complete");
                sb.Append(") ;");

                this.cmd_add_f = this.db.GetCommand(sb.ToString());
                this.db.AddString(ref cmd_add_f, "@f_id", string.Empty, 32);
                this.db.AddString(ref cmd_add_f, "@f_pid", string.Empty, 32);
                this.db.AddString(ref cmd_add_f, "@f_pidRoot", string.Empty, 32);
                this.db.AddBool(ref cmd_add_f, "@f_fdTask", false);
                this.db.AddString(ref cmd_add_f, "@f_sizeLoc", string.Empty, 32);
                this.db.AddBool(ref cmd_add_f, "@f_fdChild", true);
                this.db.AddInt(ref cmd_add_f, "@f_uid", 0);
                this.db.AddString(ref cmd_add_f, "@f_nameLoc", string.Empty, 255);
                this.db.AddString(ref cmd_add_f, "@f_nameSvr", string.Empty, 255);
                this.db.AddString(ref cmd_add_f, "@f_pathLoc", string.Empty, 255);
                this.db.AddString(ref cmd_add_f, "@f_pathSvr", string.Empty, 255);
                this.db.AddString(ref cmd_add_f, "@f_pathRel", string.Empty, 255);
                this.db.AddString(ref cmd_add_f, "@f_md5", string.Empty, 40);
                this.db.AddInt64(ref cmd_add_f, "@f_lenLoc", 0);
                this.db.AddString(ref cmd_add_f, "@f_perSvr", string.Empty, 6);
                this.db.AddBool(ref cmd_add_f, "@f_complete", false);
                cmd_add_f.Prepare();
            }
            cmd_add_f.Parameters["@f_id"].Value = f.id;
            cmd_add_f.Parameters["@f_pid"].Value = f.pid;
            cmd_add_f.Parameters["@f_pidRoot"].Value = this.m_root.id;
            cmd_add_f.Parameters["@f_fdTask"].Value = f.fdTask;
            cmd_add_f.Parameters["@f_fdChild"].Value = f.fdChild;
            cmd_add_f.Parameters["@f_sizeLoc"].Value = f.sizeLoc;
            cmd_add_f.Parameters["@f_uid"].Value = f.uid;
            cmd_add_f.Parameters["@f_nameLoc"].Value = f.nameLoc;
            cmd_add_f.Parameters["@f_nameSvr"].Value = f.nameSvr;
            cmd_add_f.Parameters["@f_pathLoc"].Value = f.pathLoc;
            cmd_add_f.Parameters["@f_pathSvr"].Value = f.pathSvr;
            cmd_add_f.Parameters["@f_pathRel"].Value = f.pathRel;
            cmd_add_f.Parameters["@f_md5"].Value = f.md5;
            cmd_add_f.Parameters["@f_lenLoc"].Value = f.lenLoc;
            cmd_add_f.Parameters["@f_perSvr"].Value = f.lenLoc > 0 ? f.perSvr : "0%";
            cmd_add_f.Parameters["@f_complete"].Value = f.lenLoc > 0 ? f.complete : true;
            cmd_add_f.ExecuteNonQuery();
        }

        protected void save_folder(FileInf f)
        {
            if (this.cmd_add_fd == null)
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

                sb.Append(") values (");

                sb.Append(" @f_id");
                sb.Append(",@f_pid");
                sb.Append(",@f_pidRoot");
                sb.Append(",@f_uid");
                sb.Append(",@f_name");
                sb.Append(",@f_pathLoc");
                sb.Append(",@f_pathSvr");
                sb.Append(",@f_pathRel");
                sb.Append(") ;");

                this.cmd_add_fd = this.db.GetCommand(sb.ToString());
                this.db.AddString(ref cmd_add_fd, "@f_id", string.Empty, 32);
                this.db.AddString(ref cmd_add_fd, "@f_pid", string.Empty, 32);
                this.db.AddString(ref cmd_add_fd, "@f_pidRoot", string.Empty, 32);
                this.db.AddInt(ref cmd_add_fd, "@f_uid", 0);
                this.db.AddString(ref cmd_add_fd, "@f_name", string.Empty, 255);
                this.db.AddString(ref cmd_add_fd, "@f_pathLoc", string.Empty, 255);
                this.db.AddString(ref cmd_add_fd, "@f_pathSvr", string.Empty, 255);
                this.db.AddString(ref cmd_add_fd, "@f_pathRel", string.Empty, 255);
                cmd_add_fd.Prepare();
            }
            cmd_add_fd.Parameters["@f_id"].Value = f.id;
            cmd_add_fd.Parameters["@f_pid"].Value = f.pid;
            cmd_add_fd.Parameters["@f_pidRoot"].Value = this.m_root.id;
            cmd_add_fd.Parameters["@f_uid"].Value = f.uid;
            cmd_add_fd.Parameters["@f_name"].Value = f.nameLoc;
            cmd_add_fd.Parameters["@f_pathLoc"].Value = f.pathLoc;
            cmd_add_fd.Parameters["@f_pathSvr"].Value = f.pathSvr;
            cmd_add_fd.Parameters["@f_pathRel"].Value = f.pathRel;
            cmd_add_fd.ExecuteNonQuery();
        }

        public virtual void save()
        {
            this.db.connection.Open();
            this.get_md5s();//提取所有文件的MD5
            //增加对空文件夹和0字节文件夹的处理
            if (!string.IsNullOrEmpty(this.m_md5s)) this.get_md5_files();//查询相同MD5值。

            //对空文件夹的处理，或者0字节文件夹的处理
            if (this.m_root.lenLoc == 0) this.m_root.complete = true;

            //检查相同文件
            this.check_files();

            this.save_file(this.m_root);
            this.save_folder(this.m_root);

            foreach (FileInf f in this.m_root.files)
            {
                f.pathSvr = this.pb.genFile(this.m_root.uid, f.md5, f.nameLoc);
                f.nameSvr = f.md5 + Path.GetExtension(f.pathLoc).ToLower();
                f.fdChild = true;

                //创建文件
                if (!f.complete && f.lenSvr < 1)
                {
                    FileBlockWriter fr = new FileBlockWriter();
                    fr.make(f.pathSvr, f.lenLoc);
                }

                this.save_file(f);
            }

            foreach(FileInf fd in this.m_root.folders)
            {
                fd.nameSvr = fd.nameLoc;
                this.save_folder(fd);
            }

            this.db.connection.Close();
        }

        protected virtual void get_md5s()
        {
            Dictionary<string, bool> md5s = new Dictionary<string, bool>();
            List<string> md5_arr = new List<string>();
            foreach(var f in this.m_root.files)
            {
                if(!md5s.ContainsKey(f.md5) && !string.IsNullOrEmpty(f.md5))
                {
                    md5s.Add(f.md5, true);
                    md5_arr.Add(f.md5);
                }
            }
            this.m_md5s = string.Join(",", md5_arr.ToArray());
        }

        protected virtual void get_md5_files()
        {
            string sql = "fd_files_check";
            var cmd = this.db.GetCommandStored(sql);

            this.db.AddString(ref cmd, "@md5s", this.m_md5s, int.MaxValue);
            this.db.AddInt(ref cmd, "@md5_len", this.m_root.files[0].md5.Length);
            this.db.AddInt(ref cmd, "@md5s_len",this.m_md5s.Length);
            var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var f = new FileInf();
                f.nameLoc = r["f_nameLoc"].ToString();
                f.nameSvr = r["f_nameSvr"].ToString();
                f.fdTask = Convert.ToBoolean(r["f_fdTask"]);
                f.fdChild = Convert.ToBoolean(r["f_fdChild"]);
                f.pathLoc = r["f_pathLoc"].ToString();
                f.pathSvr = r["f_pathSvr"].ToString();
                f.lenLoc = long.Parse(r["f_lenLoc"].ToString());
                f.sizeLoc = r["f_sizeLoc"].ToString();
                f.lenSvr = long.Parse(r["f_lenSvr"].ToString());
                f.perSvr = r["f_perSvr"].ToString();
                f.offset = long.Parse(r["f_pos"].ToString());
                f.complete = Convert.ToBoolean(r["f_complete"]);
                f.md5 = r["f_md5"].ToString();
                if(!string.IsNullOrEmpty(f.md5))this.svr_files.Add(f.md5, f);
            }
            r.Close();
        }

        /// <summary>
        /// 查找相同MD5的文件
        /// </summary>
        protected virtual void check_files()
        {
            if (this.svr_files.Count < 1) return;
            foreach(var f in this.m_root.files)
            {
                FileInf f_svr;
                if(this.svr_files.TryGetValue(f.md5,out f_svr))
                {
                    this.m_root.lenSvr += f_svr.lenSvr;
                    //f.idSvr = f_svr.idSvr;
                    //f.nameLoc = f_svr.nameLoc;
                    f.nameSvr = f_svr.nameSvr;
                    //f.pidSvr = f_svr.pidSvr;
                    //f.fdTask = f_svr.fdTask;
                    //f.fdChild = f_svr.fdChild;
                    //f.fdID = f_svr.fdID;
                    //f.pathLoc = f_svr.pathLoc;
                    f.pathSvr = f_svr.pathSvr;
                    f.lenLoc = f_svr.lenLoc;
                    f.sizeLoc = f_svr.sizeLoc;
                    f.lenSvr = f_svr.lenSvr;
                    f.perSvr = f_svr.perSvr;
                    f.offset = f_svr.offset;
                    f.complete = f_svr.complete;
                    //f.md5 = f_svr.md5;
                }
            }
        }
    }
}