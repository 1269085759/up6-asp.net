using System;
using System.Text;
using System.Data.Common;

namespace up6.demoSql2005.db
{
    public class DBFolder
    {
        /// <summary>
        /// 向数据库添加一条记录
        /// </summary>
        /// <param name="inf"></param>
        /// <returns></returns>
        static public int Add(ref FolderInf inf)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into up6_folders(");
            sb.Append("fd_name");
            sb.Append(",fd_pid");
            sb.Append(",fd_uid");
            sb.Append(",fd_length");
            sb.Append(",fd_size");
            sb.Append(",fd_pathLoc");
            sb.Append(",fd_pathSvr");
            sb.Append(",fd_folders");
            sb.Append(",fd_files");
            sb.Append(",fd_pidRoot");
            sb.Append(",fd_pathRel");

            sb.Append(") values(");
            sb.Append("@fd_name");
            sb.Append(",@pid");
            sb.Append(",@uid");
            sb.Append(",@length");
            sb.Append(",@size");
            sb.Append(",@pathLoc");
            sb.Append(",@pathSvr");
            sb.Append(",@folders");
            sb.Append(",@files");
            sb.Append(",@pidRoot");
            sb.Append(",@pathRel");
            sb.Append(");");
            //
            sb.Append("SELECT @@IDENTITY");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());

            db.AddString(ref cmd, "@fd_name", inf.nameLoc, 50);
            db.AddInt(ref cmd, "@pid", inf.pidSvr);
            db.AddInt(ref cmd, "@uid", inf.uid);
            db.AddInt64(ref cmd, "@length", inf.lenLoc);
            db.AddString(ref cmd, "@size", inf.size, 50);
            db.AddString(ref cmd, "@pathLoc", inf.pathLoc, 255);
            db.AddString(ref cmd, "@pathSvr", inf.pathSvr, 255);
            db.AddInt(ref cmd, "@folders", inf.foldersCount);
            db.AddInt(ref cmd, "@files", inf.filesCount);
            db.AddInt(ref cmd, "@pidRoot", inf.pidRoot);//为下载控件提供支持
            db.AddString(ref cmd, "@pathRel", inf.pathRel,255);//为下载控件提供支持

            //获取新插入的ID
            object fid = db.ExecuteScalar(cmd);
            return Convert.ToInt32(fid);
        }

        static public void Remove(int idFile, int idFd,int uid)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update up6_files set f_deleted=1 where f_id=@idFile and f_uid=@uid;");
            sb.Append("update up6_files set f_deleted=1 where f_pidRoot=@idFolder and f_uid=@uid;");
            sb.Append("update up6_folders set fd_delete=1 where fd_id=@idFolder and fd_uid=@uid;");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddInt(ref cmd, "@idFile", idFile);
            db.AddInt(ref cmd, "@uid", uid);
            db.AddInt(ref cmd, "@idFolder", idFd);
            db.ExecuteNonQuery(cmd);
        }

        static public void Clear()
        {
            string sql = "delete from up6_folders";
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sql);
            db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// (已弃用)获取文件夹已上传大小
        /// 计算所有文件已上传大小。
        /// </summary>
        /// <param name="fidRoot"></param>
        /// <returns></returns>
        static public long GetLenPosted(int fidRoot)
        {
            string sql = "select sum(tb.lenPosted) from (select distinct f_md5,CAST(f_lenSvr AS bigint) as lenPosted from up6_files where f_pidRoot=@f_pidRoot and f_md5 IS NOT NULL) as tb";
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sql);
            db.AddInt(ref cmd, "@f_pidRoot", fidRoot);
            object len = db.ExecuteScalar(cmd);

            return DBNull.Value == len ? 0 : Convert.ToInt64(len);
        }

        /// <summary>
        /// 子文件上传完毕
        /// </summary>
        /// <param name="fd_idSvr"></param>
        static public void child_complete(string guid)
        {
            string sql = "update up6_folders set fd_filesComplete=fd_filesComplete+1 where fd_id=@fd_id";
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sql);
            db.AddString(ref cmd, "@fd_id", guid,32);
            db.ExecuteNonQuery(cmd);
        }
    }
}