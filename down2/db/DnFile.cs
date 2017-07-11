using System;
using System.Data.Common;
using System.Text;
using up6.db;
using up6.db.database;

namespace up6.down2.db
{
    public class DnFile
    {
        public int Add(ref model.DnFileInf inf)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into down_files(");
            sql.Append(" f_uid");
            sql.Append(",f_nameLoc");
            sql.Append(",f_pathLoc");
            sql.Append(",f_fileUrl");
            sql.Append(",f_lenSvr");
            sql.Append(",f_sizeSvr");

            sql.Append(") values(");
            sql.Append(" @f_uid");
            sql.Append(",@f_nameLoc");
            sql.Append(",@f_pathLoc");
            sql.Append(",@f_fileUrl");
            sql.Append(",@f_lenSvr");
            sql.Append(",@f_sizeSvr");
            sql.Append(");");
            sql.Append("select @@IDENTITY");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sql.ToString());
            db.AddInt(ref cmd, "@f_uid", inf.uid);
            db.AddString(ref cmd, "@f_nameLoc", inf.nameLoc, 255);
            db.AddString(ref cmd, "@f_pathLoc", inf.pathLoc, 255);
            db.AddString(ref cmd, "@f_fileUrl", inf.fileUrl, 255);
            db.AddInt64(ref cmd, "@f_lenSvr", inf.lenSvr);
            db.AddString(ref cmd, "@f_sizeSvr", inf.sizeSvr,10);
            object id = db.ExecuteScalar(ref cmd);
            inf.idSvr = Convert.ToInt32(id);
            return inf.idSvr;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fid"></param>
        public void Delete(int fid, int uid)
        {
            string sql = "delete from down_files where f_id=@f_id and f_uid=@f_uid";
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sql);
            db.AddInt(ref cmd, "@f_id", fid);
            db.AddInt(ref cmd, "@f_uid", uid);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            //清除子文件
            cmd.CommandText = "delete from down_files where f_pidRoot=@f_id and f_uid=@f_uid;";
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            //db.ExecuteNonQuery(ref cmd);
        }

        public void updateProcess(int fid, int uid, string lenLoc, string perLoc)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update down_files set ");
            sb.Append(" f_lenLoc =@lenLoc");
            sb.Append(",f_perLoc=@f_perLoc");
            sb.Append(" where");
            sb.Append(" f_id =@f_id and f_uid=@f_uid;");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddString(ref cmd, "@lenLoc", lenLoc, 19);
            db.AddString(ref cmd, "@f_perLoc", perLoc, 6);
            db.AddInt(ref cmd, "@f_id", fid);
            db.AddInt(ref cmd, "@f_uid", uid);
            db.ExecuteNonQuery(ref cmd);
        }

        static public void Clear()
        {
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand("delete from down_files;");
            db.ExecuteNonQuery(ref cmd);
        }
    }
}