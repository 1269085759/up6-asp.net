using System.Collections.Generic;
using System.Text;
using up6.db.biz.folder;
using up6.db.database;

namespace up6.db.biz
{
    /// <summary>
    /// 扫描目录，将数据保存到oracle数据库中
    /// </summary>
    public class fd_scan_oracle : fd_scan
    {
        /// <summary>
        /// 覆盖文件
        /// </summary>
        /// <param name="files"></param>
        protected override void cover_files(List<string> files)
        {
            DbHelper db = new DbHelper();
            string sql = "update up6_files set f_deleted=1 where f_pathRel=:pathRel";

            var cmd = db.GetCommand(sql);

            db.AddString(ref cmd, "@pathRel", string.Empty, 512);
            cmd.Connection.Open();
            cmd.Prepare();
            foreach (var f in files)
            {
                cmd.Parameters[":pathRel"].Value = f;
                cmd.ExecuteNonQuery();
            }
            cmd.Connection.Close();
        }

        /// <summary>
        /// 批量添加文件
        /// </summary>
        /// <param name="con"></param>
        protected override void save_files(DbHelper db)
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
        protected override void save_folders(DbHelper db)
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