using System.Data.Common;
using System.Text;

namespace up6.db.database
{
    public class DBFolder
    {
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
    }
}