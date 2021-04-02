using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using up6.db.biz.folder;
using up6.db.model;

namespace up6.db.biz
{
    public class fd_scan_oracle : fd_scan
    {
        protected override void makeCmdCover()
        {
            string sql = "update up6_files set f_deleted=1 where f_pathRel=:pathRel";

            this.cmd_cover = this.db.connection.CreateCommand();
            this.cmd_cover.CommandText = sql;
            this.cmd_cover.CommandType = System.Data.CommandType.Text;

            this.db.AddString(ref cmd_cover, ":pathRel", string.Empty, 512);
            this.cmd_cover.Prepare();
        }

        protected override void cover_file(string pathRel)
        {
            this.cmd_cover.Parameters[":pathRel"].Value = pathRel;
            this.cmd_cover.ExecuteNonQuery();
        }

        /// <summary>
        /// 批量添加文件
        /// </summary>
        /// <param name="con"></param>
        protected override void save_files()
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

            sb.Append(" :f_id");
            sb.Append(",:f_pid");
            sb.Append(",:f_pidRoot");
            sb.Append(",:f_fdTask");
            sb.Append(",:f_fdChild");
            sb.Append(",:f_sizeLoc");
            sb.Append(",:f_uid");
            sb.Append(",:f_nameLoc");
            sb.Append(",:f_nameSvr");
            sb.Append(",:f_pathLoc");
            sb.Append(",:f_pathSvr");
            sb.Append(",:f_pathRel");
            sb.Append(",:f_md5");
            sb.Append(",:f_lenLoc");
            sb.Append(",:f_lenSvr");
            sb.Append(",:f_perSvr");
            sb.Append(",:f_complete");
            sb.Append(") ;");

            var cmd = db.connection.CreateCommand();
            cmd.CommandText = sb.ToString();
            cmd.CommandType = System.Data.CommandType.Text;

            db.AddString(ref cmd, ":f_id", string.Empty, 32);
            db.AddString(ref cmd, ":f_pid", string.Empty, 32);
            db.AddString(ref cmd, ":f_pidRoot", string.Empty, 32);
            db.AddBool(ref cmd, ":f_fdTask", false);
            db.AddString(ref cmd, ":f_sizeLoc", string.Empty, 32);
            db.AddBool(ref cmd, ":f_fdChild", true);
            db.AddInt(ref cmd, ":f_uid", 0);
            db.AddString(ref cmd, ":f_nameLoc", string.Empty, 255);
            db.AddString(ref cmd, ":f_nameSvr", string.Empty, 255);
            db.AddString(ref cmd, ":f_pathLoc", string.Empty, 255);
            db.AddString(ref cmd, ":f_pathSvr", string.Empty, 255);
            db.AddString(ref cmd, ":f_pathRel", string.Empty, 255);
            db.AddString(ref cmd, ":f_md5", string.Empty, 40);
            db.AddInt64(ref cmd, ":f_lenLoc", 0);
            db.AddInt64(ref cmd, ":f_lenSvr", 0);
            db.AddString(ref cmd, ":f_perSvr", "100%", 6);
            db.AddBool(ref cmd, ":f_complete", true);
            cmd.Prepare();

            foreach (var f in this.m_files)
            {
                cmd.Parameters[":f_id"].Value = f.id;
                cmd.Parameters[":f_pid"].Value = f.pid;
                cmd.Parameters[":f_pidRoot"].Value = f.pidRoot;
                cmd.Parameters[":f_sizeLoc"].Value = f.sizeLoc;
                cmd.Parameters[":f_uid"].Value = f.uid;
                cmd.Parameters[":f_nameLoc"].Value = f.nameLoc;
                cmd.Parameters[":f_nameSvr"].Value = f.nameSvr;
                cmd.Parameters[":f_pathLoc"].Value = f.pathLoc;
                cmd.Parameters[":f_pathSvr"].Value = f.pathSvr;
                cmd.Parameters[":f_pathRel"].Value = f.pathRel;
                cmd.Parameters[":f_md5"].Value = f.md5;
                cmd.Parameters[":f_lenLoc"].Value = f.lenLoc;
                cmd.Parameters[":f_lenSvr"].Value = f.lenSvr;
                cmd.ExecuteNonQuery();
            }
            cmd.Dispose();
        }

        /// <summary>
        /// 批量添加目录
        /// </summary>
        /// <param name="con"></param>
        protected override void save_folders()
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

            sb.Append(" :f_id");
            sb.Append(",:f_pid");
            sb.Append(",:f_pidRoot");
            sb.Append(",:f_uid");
            sb.Append(",:f_nameLoc");
            sb.Append(",:f_pathLoc");
            sb.Append(",:f_pathSvr");
            sb.Append(",:f_pathRel");
            sb.Append(",:f_complete");
            sb.Append(") ;");

            var cmd = db.connection.CreateCommand();
            cmd.CommandText = sb.ToString();
            cmd.CommandType = System.Data.CommandType.Text;

            db.AddString(ref cmd, ":f_id", string.Empty, 32);
            db.AddString(ref cmd, ":f_pid", string.Empty, 32);
            db.AddString(ref cmd, ":f_pidRoot", string.Empty, 32);
            db.AddInt(ref cmd, ":f_uid", 0);
            db.AddString(ref cmd, ":f_nameLoc", string.Empty, 255);
            db.AddString(ref cmd, ":f_pathLoc", string.Empty, 255);
            db.AddString(ref cmd, ":f_pathSvr", string.Empty, 255);
            db.AddString(ref cmd, ":f_pathRel", string.Empty, 255);
            db.AddBool(ref cmd, ":f_complete", true);
            cmd.Prepare();

            foreach (var f in this.m_files)
            {
                cmd.Parameters[":f_id"].Value = f.id;
                cmd.Parameters[":f_pid"].Value = f.pid;
                cmd.Parameters[":f_pidRoot"].Value = f.pidRoot;
                cmd.Parameters[":f_uid"].Value = f.uid;
                cmd.Parameters[":f_nameLoc"].Value = f.nameLoc;
                cmd.Parameters[":f_pathLoc"].Value = f.pathLoc;
                cmd.Parameters[":f_pathSvr"].Value = f.pathSvr;
                cmd.Parameters[":f_pathRel"].Value = f.pathRel;
                cmd.ExecuteNonQuery();
            }
            cmd.Dispose();
        }

    }
}