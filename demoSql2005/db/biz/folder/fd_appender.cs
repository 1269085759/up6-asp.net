using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;

namespace up6.demoSql2005.db.biz.folder
{
    public class fd_appender
    {
        string[] fd_ids;
        string[] f_ids;
        DbHelper db;
        DbCommand cmd;
        protected PathBuilder pb = new PathMd5Builder();
        protected Dictionary<int/*fd.idLoc*/, int/*fd.idSvr*/> map_pids = new Dictionary<int, int>();
        protected Dictionary<int/*fd.idLoc*/, int/*fd.index*/> map_fd_ids= new Dictionary<int, int>();
        Dictionary<string/*md5*/, fd_file> svr_files = new Dictionary<string, fd_file>();
        public fd_root m_root;//
        private string m_md5s = "0";

        public fd_appender()
        {
            this.db = new DbHelper();
        }

        public virtual void save()
        {
            this.get_md5s();//提取所有文件的MD5
            this.make_ids();
            //增加对空文件夹和0字节文件夹的处理
            if (!string.IsNullOrEmpty(this.m_md5s)) this.get_md5_files();//查询相同MD5值。

            this.set_ids();     //设置文件和文件夹id
            this.update_rel();  //更新结构关系

            //对空文件夹的处理，或者0字节文件夹的处理
            if (this.m_root.lenLoc == 0) this.m_root.complete = true;
            if (this.m_root.files.Count == 0) this.m_root.complete = true;

            //更新文件夹信息
            this.pre_update_fd();
            foreach (fd_child fc in this.m_root.folders)
            {
                this.update_fd(fc);
            }

            //更新根级文件夹信息
            int id_file = this.m_root.idSvr;
            this.m_root.idSvr = this.m_root.fdID;//设为文件夹表的ID
            this.update_fd(this.m_root);
            this.m_root.idSvr = id_file;//设为文件表的ID。
            this.m_root.idFile = id_file;

            //检查相同文件
            this.check_files();

            //批量更新文件
            this.pre_update_files();
            foreach(fd_file f in this.m_root.files)
            {
                this.update_file(f);
            }
            this.update_file(this.m_root);
        }

        protected virtual void get_md5s()
        {
            Dictionary<string, bool> md5s = new Dictionary<string, bool>();
            List<string> md5_arr = new List<string>();
            foreach(fd_file f in this.m_root.files)
            {
                if(!md5s.ContainsKey(f.md5) && !string.IsNullOrEmpty(f.md5))
                {
                    md5s.Add(f.md5, true);
                    md5_arr.Add(f.md5);
                }
            }
            this.m_md5s = string.Join(",", md5_arr.ToArray());
        }

        void make_ids()
        {
            this.cmd = db.GetCommandStored("fd_files_add_batch");
            db.AddInt(ref cmd, "@f_count", this.m_root.files.Count + 1);
            db.AddInt(ref cmd, "@fd_count", this.m_root.folders.Count + 1);
            cmd.Connection.Open();
            var r = cmd.ExecuteReader();
            List<string> id_files = new List<string>();
            List<string> id_fds = new List<string>();
            while(r.Read())
            {
                if (r.GetBoolean(0)) id_files.Add(r.GetInt32(1).ToString());
                else id_fds.Add(r.GetInt32(1).ToString());
            }
            r.Close();
            this.f_ids = id_files.ToArray();
            this.fd_ids = id_fds.ToArray();
        }

        /// <summary>
        /// 设置ID值
        /// 设置文件的父级ID
        /// 设置文件夹的父级ID
        /// </summary>
        void set_ids()
        {
            this.m_root.idSvr = int.Parse(this.f_ids[this.m_root.files.Count]);//取最后一个
            this.m_root.fdID = int.Parse(this.fd_ids[this.m_root.folders.Count]); //取最后一个
            this.map_pids.Add(0, this.m_root.fdID);

            //设置文件夹ID，
            for (int i = 0, l = this.m_root.folders.Count; i < l; ++i)
            {
                this.m_root.folders[i].idSvr = int.Parse(this.fd_ids[i]);
                this.m_root.folders[i].pidRoot = this.m_root.idSvr;
                this.map_pids.Add(this.m_root.folders[i].idLoc, this.m_root.folders[i].idSvr);
                this.map_fd_ids.Add(this.m_root.folders[i].idLoc, i);//添加idLoc,index索引
            }

            for (int i = 0, l = this.m_root.files.Count; i < l; ++i)
            {
                this.m_root.files[i].idSvr = int.Parse(this.f_ids[i]);
                this.m_root.files[i].pidRoot = this.m_root.fdID;
                this.m_root.files[i].fdChild = true;//
            }
        }

        /// <summary>
        /// 更新层级结构信息
        /// 更新文件夹父级ID
        /// 更新文件父级ID
        /// </summary>
        /// <param name="fd"></param>
        public virtual void update_rel()
        {
            //更新文件夹的层级ID
            foreach(fd_child fd in this.m_root.folders)
            {
                int pidSvr = 0;
                this.map_pids.TryGetValue(fd.pidLoc, out pidSvr);
                fd.pidSvr = pidSvr;
            }

            //更新文件的层级ID
            foreach(fd_file f in this.m_root.files)
            {
                int pidSvr = 0;
                this.map_pids.TryGetValue(f.pidLoc, out pidSvr);
                f.pidSvr = pidSvr;
                //生成服务器文件名称
                f.nameSvr = f.md5 + Path.GetExtension(f.pathLoc).ToLower();
                //生成文件路径
                f.pathSvr = this.pb.genFile(f.uid, f.md5, f.nameLoc);
            }
        }

        void pre_update_fd()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update up6_folders set");
            sb.Append(" fd_name=@fd_name");
            sb.Append(",fd_pid=@fd_pid");
            sb.Append(",fd_uid=@fd_uid");
            sb.Append(",fd_length=@fd_length");
            sb.Append(",fd_size=@fd_size");
            sb.Append(",fd_pathLoc=@fd_pathLoc");
            sb.Append(",fd_pathSvr=@fd_pathSvr");
            sb.Append(",fd_folders=@fd_folders");
            sb.Append(",fd_files=@fd_files");
            sb.Append(",fd_pidRoot=@fd_pidRoot");
            sb.Append(" where fd_id=@fd_id");

            this.cmd.CommandText = sb.ToString();
            this.cmd.CommandType = System.Data.CommandType.Text;

            this.db.AddString(ref this.cmd, "@fd_name", string.Empty, 50);
            this.db.AddInt(ref this.cmd, "@fd_pid", 0);
            this.db.AddInt(ref this.cmd, "@fd_uid", 0);
            this.db.AddInt64(ref this.cmd, "@fd_length", 0);
            this.db.AddString(ref this.cmd, "@fd_size", string.Empty, 50);
            this.db.AddString(ref this.cmd, "@fd_pathLoc", string.Empty, 255);
            this.db.AddString(ref this.cmd, "@fd_pathSvr", string.Empty, 255);
            this.db.AddInt(ref this.cmd, "@fd_folders", 0);
            this.db.AddInt(ref this.cmd, "@fd_files", 0);
            this.db.AddInt(ref this.cmd, "@fd_pidRoot", 0);
            this.db.AddInt(ref this.cmd, "@fd_id", 0);
            this.cmd.Prepare();
        }

        void update_fd(fd_child fd)
        {
            this.cmd.Parameters["@fd_name"].Value = fd.nameLoc;
            this.cmd.Parameters["@fd_pid"].Value = fd.pidSvr;
            this.cmd.Parameters["@fd_uid"].Value = fd.uid;
            this.cmd.Parameters["@fd_length"].Value = fd.lenLoc;
            this.cmd.Parameters["@fd_size"].Value = fd.sizeLoc;
            this.cmd.Parameters["@fd_pathLoc"].Value = fd.pathLoc;
            this.cmd.Parameters["@fd_pathSvr"].Value = fd.pathSvr;
            this.cmd.Parameters["@fd_folders"].Value = fd.foldersCount;
            this.cmd.Parameters["@fd_files"].Value = fd.filesCount;
            this.cmd.Parameters["@fd_pidRoot"].Value = fd.pidRoot;
            this.cmd.Parameters["@fd_id"].Value = fd.idSvr;
            this.cmd.ExecuteNonQuery();
        }

        protected virtual void get_md5_files()
        {
            string sql = "fd_files_check";

            this.cmd.Parameters.Clear();
            this.cmd.CommandText = sql;
            this.db.AddString(ref cmd, "@md5s", this.m_md5s, int.MaxValue);
            this.db.AddInt(ref cmd, "@md5_len", this.m_root.files[0].md5.Length);
            this.db.AddInt(ref cmd, "@md5s_len",this.m_md5s.Length);
            var r = this.cmd.ExecuteReader();
            while (r.Read())
            {
                fd_file f = new fd_file();
                f.idSvr = Convert.ToInt32(r["f_id"]);
                f.nameLoc = r["f_nameLoc"].ToString();
                f.nameSvr = r["f_nameSvr"].ToString();
                f.pidSvr = int.Parse(r["f_pid"].ToString());
                f.fdTask = Convert.ToBoolean(r["f_fdTask"]);
                f.fdChild = Convert.ToBoolean(r["f_fdChild"]);
                f.fdID = int.Parse(r["f_fdID"].ToString());
                f.pathLoc = r["f_pathLoc"].ToString();
                f.pathSvr = r["f_pathSvr"].ToString();
                f.lenLoc = long.Parse(r["f_lenLoc"].ToString());
                f.sizeLoc = r["f_sizeLoc"].ToString();
                f.lenSvr = long.Parse(r["f_lenSvr"].ToString());
                f.perSvr = r["f_perSvr"].ToString();
                f.pos = long.Parse(r["f_pos"].ToString());
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
                fd_file f_svr;
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
                    f.pos = f_svr.pos;
                    f.complete = f_svr.complete;
                    //f.md5 = f_svr.md5;
                }
            }
        }

        void pre_update_files()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update up6_files set");
            sb.Append(" f_pid=@f_pid");
            sb.Append(",f_pidRoot=@f_pidRoot");
            sb.Append(",f_fdTask=@f_fdTask");
            sb.Append(",f_fdID=@f_fdID");
            sb.Append(",f_fdChild=@f_fdChild");
            sb.Append(",f_uid=@f_uid");
            sb.Append(",f_nameLoc=@f_nameLoc");
            sb.Append(",f_nameSvr=@f_nameSvr");
            sb.Append(",f_pathLoc=@f_pathLoc");
            sb.Append(",f_pathSvr=@f_pathSvr");
            sb.Append(",f_pathRel=@f_pathRel");
            sb.Append(",f_md5=@f_md5");
            sb.Append(",f_lenLoc=@f_lenLoc");
            sb.Append(",f_sizeLoc=@f_sizeLoc");
            sb.Append(",f_pos=@f_pos");
            sb.Append(",f_lenSvr=@f_lenSvr");
            sb.Append(",f_perSvr=@f_perSvr");
            sb.Append(",f_complete=@f_complete");
            sb.Append(" where f_id=@f_id");

            this.cmd.Parameters.Clear();
            this.cmd.CommandText = sb.ToString();
            this.cmd.CommandType = System.Data.CommandType.Text;

            this.db.AddInt(ref this.cmd, "@f_pid", 0);
            this.db.AddInt(ref this.cmd, "@f_pidRoot", 0);
            this.db.AddBool(ref this.cmd, "@f_fdTask", true);
            this.db.AddInt(ref this.cmd, "@f_fdID", 0);
            this.db.AddBool(ref this.cmd, "@f_fdChild", true);
            this.db.AddInt(ref this.cmd, "@f_uid", 0);
            this.db.AddString(ref this.cmd, "@f_nameLoc", string.Empty, 255);
            this.db.AddString(ref this.cmd, "@f_nameSvr", string.Empty,255);
            this.db.AddString(ref this.cmd, "@f_pathLoc", string.Empty,255);
            this.db.AddString(ref this.cmd, "@f_pathSvr", string.Empty,255);
            this.db.AddString(ref this.cmd, "@f_pathRel", string.Empty,255);
            this.db.AddString(ref this.cmd, "@f_md5", string.Empty,40);
            this.db.AddInt64(ref this.cmd, "@f_lenLoc", 0);
            this.db.AddString(ref this.cmd, "@f_sizeLoc", string.Empty,10);
            this.db.AddInt64(ref this.cmd, "@f_pos", 0);
            this.db.AddInt64(ref this.cmd, "@f_lenSvr", 0);
            this.db.AddString(ref this.cmd, "@f_perSvr", string.Empty,6);
            this.db.AddBool(ref this.cmd, "@f_complete",false);
            this.db.AddInt(ref this.cmd, "@f_id", 0);
            this.cmd.Prepare();
        }

        void update_file(fd_file f)
        {
            if (!f.fdTask)
            { 
                FileBlockWriter fr = new FileBlockWriter();
                fr.make(f.pathSvr, f.lenLoc);
            }

            this.cmd.Parameters["@f_pid"].Value = f.pidSvr;
            this.cmd.Parameters["@f_pidRoot"].Value = f.pidRoot;
            this.cmd.Parameters["@f_fdTask"].Value = f.fdTask;
            this.cmd.Parameters["@f_fdID"].Value = f.fdID;
            this.cmd.Parameters["@f_fdChild"].Value = f.fdChild;
            this.cmd.Parameters["@f_uid"].Value = f.uid;
            this.cmd.Parameters["@f_nameLoc"].Value = f.nameLoc;
            this.cmd.Parameters["@f_nameSvr"].Value = f.nameSvr;
            this.cmd.Parameters["@f_pathLoc"].Value = f.pathLoc;
            this.cmd.Parameters["@f_pathSvr"].Value = f.pathSvr;
            this.cmd.Parameters["@f_pathRel"].Value = f.pathRel;
            this.cmd.Parameters["@f_md5"].Value = f.md5;
            this.cmd.Parameters["@f_lenLoc"].Value = f.lenLoc;
            this.cmd.Parameters["@f_sizeLoc"].Value = f.sizeLoc;
            this.cmd.Parameters["@f_pos"].Value = f.pos;
            this.cmd.Parameters["@f_lenSvr"].Value = f.lenSvr;
            this.cmd.Parameters["@f_perSvr"].Value = f.lenLoc > 0 ? f.perSvr : "100%";
            //fix(2016-09-21):0字节文件直接显示100%
            this.cmd.Parameters["@f_complete"].Value = f.lenLoc > 0 ? f.complete : true;
            this.cmd.Parameters["@f_id"].Value = f.idSvr;
            this.cmd.ExecuteNonQuery();
        }
    }
}