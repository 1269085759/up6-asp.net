﻿using System;
using System.Data.Common;
using System.Text;
using up6.db.model;

namespace up6.db
{
    /// <summary>
    /// 数据库访问操作
    /// 更新记录：
    ///		2012-04-10 创建
    ///		2014-03-11 将OleDb对象全部改为使用DbHelper对象，简化代码。
    /// </summary>
    public class DBFile
    {
        /// <summary>
        /// 根据文件ID获取文件信息
        /// </summary>
        /// <param name="f_id"></param>
        /// <returns></returns>
        public bool GetFileInfByFid(string f_id, ref FileInf inf)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();
            sb.Append("select top 1 ");
            sb.Append(" f_uid");
            sb.Append(",f_nameLoc");
            sb.Append(",f_nameSvr");
            sb.Append(",f_pathLoc");
            sb.Append(",f_pathSvr");
            sb.Append(",f_pathRel");
            sb.Append(",f_md5");
            sb.Append(",f_lenLoc");
            sb.Append(",f_sizeLoc");
            sb.Append(",f_pos");
            sb.Append(",f_lenSvr");
            sb.Append(",f_perSvr");
            sb.Append(",f_complete");
            sb.Append(",f_time");
            sb.Append(",f_deleted");
            sb.Append(" from up6_files where f_id=@f_id");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddString(ref cmd, "@f_id", f_id,32);
            DbDataReader r = db.ExecuteReader(cmd);

            if (r.Read())
            {
                inf.id = f_id;
                inf.uid = r.GetInt32(0);
                inf.nameLoc = r.GetString(1);
                inf.nameSvr = r.GetString(2);
                inf.pathLoc = r.GetString(3);
                inf.pathSvr = r.GetString(4);
                inf.pathRel = r.IsDBNull(5) ? string.Empty : r.GetString(5);
                inf.md5 = r.GetString(6);
                inf.lenLoc = r.GetInt64(7);
                inf.sizeLoc = r.GetString(8);
                inf.offset = r.GetInt64(9);
                inf.lenSvr = r.GetInt64(10);
                inf.perSvr = r.GetString(11);
                inf.complete = r.GetBoolean(12);
                inf.time = r.GetDateTime(13);
                inf.deleted = r.GetBoolean(14);
                ret = true;
            }
            r.Close();
            return ret;
        }

        /// <summary>
        /// 根据文件MD5获取文件信息
        /// </summary>
        /// <param name="md5"></param>
        /// <param name="inf"></param>
        /// <returns></returns>
        public bool exist_file(string md5, ref FileInf inf)
        {
            if (string.IsNullOrEmpty(md5)) return false;

            bool ret = false;
            StringBuilder sb = new StringBuilder();
            sb.Append("select top 1 ");
            sb.Append(" f_uid");
            sb.Append(",f_nameLoc");
            sb.Append(",f_nameSvr");
            sb.Append(",f_pathLoc");
            sb.Append(",f_pathSvr");
            sb.Append(",f_pathRel");
            sb.Append(",f_lenLoc");
            sb.Append(",f_sizeLoc");
            sb.Append(",f_pos");
            sb.Append(",f_lenSvr");
            sb.Append(",f_perSvr");
            sb.Append(",f_complete");
            sb.Append(",f_time");
            sb.Append(",f_deleted");
            sb.Append(" from up6_files where f_md5=@f_md5 order by f_perSvr desc");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddString(ref cmd, "@f_md5", md5, 40);
            DbDataReader r = db.ExecuteReader(cmd);
            if (r.Read())
            {
                inf.uid = r.GetInt32(0);
                inf.nameLoc = r.GetString(1);
                inf.nameSvr = r.GetString(2);
                inf.pathLoc = r.GetString(3);
                inf.pathSvr = r.GetString(4);
                inf.pathRel = r.GetString(5);
                inf.md5     = md5;
                inf.lenLoc = r.GetInt64(6);
                inf.sizeLoc = r.GetString(7);
                inf.offset = r.GetInt64(8);
                inf.lenSvr = r.GetInt64(9);
                inf.perSvr = r.GetString(10);
                inf.complete = r.GetBoolean(11);
                inf.time = r.GetDateTime(12);
                inf.deleted = r.GetBoolean(13);
                ret = true;
            }
            r.Close();
            return ret;
        }

        /// <summary>
        /// 增加一条数据，并返回新增数据的ID
        /// 在ajax_create_fid.aspx中调用
        /// 文件名称，本地路径，远程路径，相对路径都使用原始字符串。
        /// d:\soft\QQ2012.exe
        /// </summary>
        public void Add(ref FileInf model)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into up6_files(");
            sb.Append(" f_id");
            sb.Append(",f_sizeLoc");
            sb.Append(",f_pos");
            sb.Append(",f_lenSvr");
            sb.Append(",f_perSvr");
            sb.Append(",f_complete");
            sb.Append(",f_time");
            sb.Append(",f_deleted");
            sb.Append(",f_fdChild");
            sb.Append(",f_uid");
            sb.Append(",f_nameLoc");
            sb.Append(",f_nameSvr");
            sb.Append(",f_pathLoc");
            sb.Append(",f_pathSvr");
            sb.Append(",f_pathRel");
            sb.Append(",f_md5");
            sb.Append(",f_lenLoc");

            sb.Append(") values (");

            sb.Append(" @f_id");
            sb.Append(",@f_sizeLoc");
            sb.Append(",@f_pos");
            sb.Append(",@f_lenSvr");
            sb.Append(",@f_perSvr");
            sb.Append(",@f_complete");
            sb.Append(",@f_time");
            sb.Append(",@f_deleted");
            sb.Append(",@f_fdChild");
            sb.Append(",@f_uid");
            sb.Append(",@f_nameLoc");
            sb.Append(",@f_nameSvr");
            sb.Append(",@f_pathLoc");
            sb.Append(",@f_pathSvr");
            sb.Append(",@f_pathRel");
            sb.Append(",@f_md5");
            sb.Append(",@f_lenLoc");
            sb.Append(") ;");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());

            db.AddString(ref cmd, "@f_id", model.id, 32);
            db.AddString(ref cmd, "@f_sizeLoc", model.sizeLoc, 10);
            db.AddInt64(ref cmd, "@f_pos", model.offset);
            db.AddInt64(ref cmd, "@f_lenSvr", model.lenSvr);
            db.AddString(ref cmd, "@f_perSvr", model.perSvr, 6);
            db.AddInBool(cmd, "@f_complete",model.complete);
            db.AddDate(ref cmd, "@f_time", model.time);
            db.AddInBool(cmd, "@f_deleted", false);
            db.AddInBool(cmd, "@f_fdChild", model.fdChild);
            db.AddInt(ref cmd, "@f_uid", model.uid);
            db.AddString(ref cmd, "@f_nameLoc", model.nameLoc, 255);
            db.AddString(ref cmd, "@f_nameSvr", model.nameSvr, 255);
            db.AddString(ref cmd, "@f_pathLoc", model.pathLoc, 255);
            db.AddString(ref cmd, "@f_pathSvr", model.pathSvr, 255);
            db.AddString(ref cmd, "@f_pathRel", model.pathRel, 255);
            db.AddString(ref cmd, "@f_md5", model.md5, 40);
            db.AddInt64(ref cmd, "@f_lenLoc", model.lenLoc);

            db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 添加一个文件夹上传任务
        /// 更新记录：
        ///     2016-03-30 返回up6_files.f_id
        /// </summary>
        /// <param name="fd"></param>
        /// <returns></returns>
        static public int Add(ref FolderInf fd)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into up6_files(");
            sb.Append(" f_nameLoc");
            sb.Append(",f_fdTask");
            sb.Append(",f_fdID");
            sb.Append(",f_lenLoc");
            sb.Append(",f_sizeLoc");
            sb.Append(",f_pathLoc");//add(2016-07-27):
            //param
            sb.Append(") values(");
            sb.Append("@f_nameLoc");
            sb.Append(",1");
            sb.Append(",@f_fdID");
            sb.Append(",@fd_lenLoc");
            sb.Append(",@fd_sizeLoc");
            sb.Append(",@f_pathLoc");
            sb.Append(");");
            //获取新插入ID
            sb.Append("SELECT @@IDENTITY");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddString(ref cmd, "@f_nameLoc", fd.nameLoc, 255);
            db.AddInt(ref cmd, "@f_fdID", fd.idSvr);
            db.AddInt64(ref cmd, "@fd_lenLoc", fd.lenLoc);
            db.AddString(ref cmd, "@fd_sizeLoc", fd.size,10);
            db.AddString(ref cmd, "@f_pathLoc", fd.pathLoc,255);
            object f_id = db.ExecuteScalar(cmd);
            return Convert.ToInt32(f_id);
        }

        static public void Clear()
        {
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand("delete from up6_files;");
            db.ExecuteNonQuery(cmd);
            cmd.CommandText = "delete from up6_folders;";
            db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f_uid"></param>
        /// <param name="f_id">文件夹ID</param>
        static public void fd_complete(string f_id,string uid)
        {
            string sql = "update up6_files set f_perSvr='100%',f_lenSvr=f_lenLoc,f_complete=1 where f_id=@f_id and f_uid=@uid;";
            sql += "update up6_folders set fd_complete=1 where fd_id=@f_id and fd_uid=@uid;";
            sql += "update up6_files set f_perSvr='100%',f_lenSvr=f_lenLoc,f_complete=1 where f_pidRoot=@f_id;";

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sql);
            db.AddString(ref cmd, "@f_id", f_id,32);
            db.AddInt(ref cmd, "@uid", int.Parse(uid));
            db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 更新上传进度
        /// </summary>
        ///<param name="f_uid">用户ID</param>
        ///<param name="f_id">文件ID</param>
        ///<param name="f_pos">文件位置，大小可能超过2G，所以需要使用long保存</param>
        ///<param name="f_lenSvr">已上传长度，文件大小可能超过2G，所以需要使用long保存</param>
        ///<param name="f_perSvr">已上传百分比</param>
        public bool f_process(int f_uid, string f_id, long offset, long f_lenSvr, string f_perSvr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update up6_files");
            sb.Append(" set");
            sb.Append(" f_pos =@f_pos");
            sb.Append(",f_lenSvr=@f_lenSvr");
            sb.Append(",f_perSvr=@f_perSvr");
            sb.Append(" where f_uid =@f_uid and f_id=@f_id");
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString() );

            db.AddInt64(ref cmd, "@f_pos", offset);
            db.AddInt64(ref cmd, "@f_lenSvr", f_lenSvr);
            db.AddString(ref cmd, "@f_perSvr", f_perSvr, 6);
            db.AddInt(ref cmd, "@f_uid", f_uid);
            db.AddString(ref cmd, "@f_id", f_id,32);

            db.ExecuteNonQuery(cmd);
            return true;
        }

        /// <summary>
        /// 更新文件夹-文件进度
        /// </summary>
        /// <param name="f_uid"></param>
        /// <param name="f_id"></param>
        /// <param name="f_pos"></param>
        /// <param name="lenSvr"></param>
        /// <param name="perSvr"></param>
        /// <param name="fd_idSvr"></param>
        /// <param name="fd_lenSvr"></param>
        /// <returns></returns>
        public bool fd_fileProcess(int uid, int f_id, long f_pos, long lenSvr, string perSvr, int fd_idSvr, long fd_lenSvr,string perSvrFD,bool f_complete)
        {
            //string sql = "update up6_files set f_pos=@f_pos,f_lenSvr=@f_lenSvr,f_perSvr=@f_perSvr where uid=@uid and f_id=@f_id";
            string sql = "fd_fileProcess";
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommandStored(sql);

            db.AddInt64(ref cmd, "@f_pos", f_pos);
            db.AddInt(ref cmd, "@uid", uid);
            db.AddInt(ref cmd, "@idSvr", f_id);
            db.AddInt64(ref cmd, "@lenSvr", lenSvr);
            db.AddString(ref cmd, "@perSvr", perSvr, 6);
            db.AddBool(ref cmd, "@complete", f_complete);
            db.AddInt(ref cmd, "@fd_idSvr", fd_idSvr);
            db.AddInt64(ref cmd, "@fd_lenSvr", fd_lenSvr);
            db.AddString(ref cmd,"@fd_perSvr", perSvrFD,6);

            db.ExecuteNonQuery(cmd);
            return true;
        }

        /// <summary>
        /// 上传完成。将所有相同MD5文件进度都设为100%
        /// </summary>
        public void UploadComplete(string md5)
        {
            string sql = "update up6_files set f_lenSvr=f_lenLoc,f_perSvr='100%',f_complete=1 where f_md5=@f_md5";
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sql);

            db.AddString(ref cmd, "@f_md5", md5, 40);
            db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 删除一条数据，并不真正删除，只更新删除标识。
        /// </summary>
        /// <param name="f_uid"></param>
        /// <param name="f_id"></param>
        public void Delete(int f_uid, string f_id)
        {
            string sql = "update up6_files set f_deleted=1 where f_uid=@f_uid and f_id=@f_id";
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sql);

            db.AddInt(ref cmd, "@f_uid", f_uid);
            db.AddString(ref cmd, "@f_id", f_id,32);
            db.ExecuteNonQuery(cmd);
        }
    }
}