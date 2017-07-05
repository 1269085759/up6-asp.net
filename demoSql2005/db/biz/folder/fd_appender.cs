using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;

namespace up6.demoSql2005.db.biz.folder
{
    public class fd_appender
    {
        DbHelper db;
        DbCommand cmd;
        protected PathBuilder pb = new PathMd5Builder();
        Dictionary<string/*md5*/, fd_file> svr_files = new Dictionary<string, fd_file>();
        public fd_root m_root;//
        private string m_md5s = "0";

        public fd_appender()
        {
            this.db = new DbHelper();
            this.cmd = this.db.GetCommand("select * from up6_files");
            this.cmd.Connection.Open();
        }

        void saveFiles() {

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

            var cmd = this.db.GetCommand(sb.ToString());
            this.db.AddString(ref cmd, "@f_id", string.Empty, 32);
            this.db.AddString(ref cmd, "@f_pid", string.Empty, 32);
            this.db.AddString(ref cmd, "@f_pidRoot", string.Empty, 32);
            this.db.AddString(ref cmd, "@f_sizeLoc", string.Empty, 32);
            this.db.AddBool(ref cmd, "@f_fdChild", true);
            this.db.AddInt(ref cmd, "@f_uid", 0);
            this.db.AddString(ref cmd, "@f_nameLoc", string.Empty, 255);
            this.db.AddString(ref cmd, "@f_nameSvr", string.Empty, 255);
            this.db.AddString(ref cmd, "@f_pathLoc", string.Empty, 255);
            this.db.AddString(ref cmd, "@f_pathSvr", string.Empty, 255);
            this.db.AddString(ref cmd, "@f_pathRel", string.Empty, 255);
            this.db.AddString(ref cmd, "@f_md5", string.Empty, 40);
            this.db.AddInt64(ref cmd, "@f_lenLoc", 0);
            this.db.AddString(ref cmd, "@f_perSvr", string.Empty, 6);
            this.db.AddBool(ref cmd, "@f_complete", false);
            cmd.Prepare();
            foreach(var f in this.m_root.files)
            {
                if( string.IsNullOrEmpty(f.pathSvr ) )
                f.pathSvr = this.pb.genFile(this.m_root.uid, f.md5, f.nameLoc);
                if( string.IsNullOrEmpty(f.nameSvr) )
                f.nameSvr = f.md5 + Path.GetExtension(f.pathLoc).ToLower();
                cmd.Parameters["@f_id"].Value = f.id;
                cmd.Parameters["@f_pid"].Value = f.pid;
                cmd.Parameters["@f_pidRoot"].Value = this.m_root.id;
                cmd.Parameters["@f_sizeLoc"].Value = f.sizeLoc;
                cmd.Parameters["@f_uid"].Value = f.uid;
                cmd.Parameters["@f_nameLoc"].Value = f.nameLoc;
                cmd.Parameters["@f_nameSvr"].Value = f.nameSvr;
                cmd.Parameters["@f_pathLoc"].Value = f.pathLoc;
                cmd.Parameters["@f_pathSvr"].Value = f.pathSvr;
                cmd.Parameters["@f_pathRel"].Value = f.pathRel;
                cmd.Parameters["@f_md5"].Value = f.md5;
                cmd.Parameters["@f_lenLoc"].Value = f.lenLoc;
                cmd.Parameters["@f_complete"].Value = f.lenLoc>0 ? f.perSvr : "0%";
                cmd.Parameters["@f_complete"].Value = f.lenLoc>0 ? f.complete : true;
                cmd.ExecuteNonQuery();
            }
            var rt = this.m_root;
            //添加根目录
            cmd.Parameters["@f_id"].Value = rt.id;
            cmd.Parameters["@f_pid"].Value = string.Empty;
            cmd.Parameters["@f_pidRoot"].Value = string.Empty;
            cmd.Parameters["@f_fdChild"].Value = false;
            cmd.Parameters["@f_sizeLoc"].Value = rt.sizeLoc;
            cmd.Parameters["@f_uid"].Value = rt.uid;
            cmd.Parameters["@f_nameLoc"].Value = rt.nameLoc;
            cmd.Parameters["@f_nameSvr"].Value = string.Empty;
            cmd.Parameters["@f_pathLoc"].Value = rt.pathLoc;
            cmd.Parameters["@f_pathSvr"].Value = string.Empty;
            cmd.Parameters["@f_pathRel"].Value = string.Empty;
            cmd.Parameters["@f_md5"].Value = rt.md5;
            cmd.Parameters["@f_lenLoc"].Value = rt.lenLoc;
            cmd.ExecuteNonQuery();
        }
        void saveFolders()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into up6_folders(");
            sb.Append(" fd_id");
            sb.Append(",fd_name");
            sb.Append(",fd_pid");
            sb.Append(",fd_uid");
            sb.Append(",fd_pidRoot");

            sb.Append(") values (");

            sb.Append(" @fd_id");
            sb.Append(",@fd_name");
            sb.Append(",@fd_pid");
            sb.Append(",@fd_uid");
            sb.Append(",@fd_pidRoot");
            sb.Append(") ;");

            DbCommand cmd = this.db.GetCommand(sb.ToString());
            this.db.AddString(ref cmd, "@fd_id", string.Empty, 32);
            this.db.AddString(ref cmd, "@fd_name", string.Empty, 50);
            this.db.AddString(ref cmd, "@fd_pid", string.Empty, 32);
            this.db.AddString(ref cmd, "@fd_pidRoot", string.Empty, 32);
            this.db.AddInt(ref cmd, "@fd_uid", 0);
            cmd.Prepare();

            foreach (var f in this.m_root.folders)
            {
                cmd.Parameters["@fd_id"].Value = f.id;
                cmd.Parameters["@fd_name"].Value = f.nameLoc;
                cmd.Parameters["@fd_pid"].Value = f.pid;
                cmd.Parameters["@fd_uid"].Value = f.uid;
                cmd.Parameters["@fd_pidRoot"].Value = this.m_root.id;
                cmd.ExecuteNonQuery();
            }

            //添加根目录
            var rt = this.m_root;
            cmd.Parameters["@fd_id"].Value = rt.id;
            cmd.Parameters["@fd_name"].Value = rt.nameLoc;
            cmd.Parameters["@fd_pid"].Value = string.Empty;
            cmd.Parameters["@fd_uid"].Value = rt.uid;
            cmd.Parameters["@fd_pidRoot"].Value = string.Empty;
            cmd.ExecuteNonQuery();
        }

        public virtual void save()
        {
            this.get_md5s();//提取所有文件的MD5
            //增加对空文件夹和0字节文件夹的处理
            if (!string.IsNullOrEmpty(this.m_md5s)) this.get_md5_files();//查询相同MD5值。

            //this.update_rel();  //更新结构关系

            //对空文件夹的处理，或者0字节文件夹的处理
            if (this.m_root.lenLoc == 0) this.m_root.complete = true;

            //检查相同文件
            this.check_files();

            this.saveFiles();
            this.saveFolders();
            this.cmd.Connection.Close();
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

        /// <summary>
        /// 更新层级结构信息
        /// 更新文件夹父级ID
        /// 更新文件父级ID
        /// </summary>
        /// <param name="fd"></param>
        public virtual void update_rel()
        {
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


        void update_file(fd_file f)
        {
            if (!f.fdTask)
            { 
                FileBlockWriter fr = new FileBlockWriter();
                fr.make(f.pathSvr, f.lenLoc);
            }

            this.cmd.Parameters["@f_pidRoot"].Value = f.pidRoot;
            this.cmd.Parameters["@f_fdTask"].Value = f.fdTask;
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
            this.cmd.Parameters["@f_guid"].Value = f.id;
            this.cmd.ExecuteNonQuery();
        }
    }
}