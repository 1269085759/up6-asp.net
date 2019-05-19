using System.Data.Common;
using System.Text;

namespace up6.db.database
{
    public class DBFolder
    {
        static public void Remove(string id, int uid)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update up6_files set f_deleted=1 where f_id=@id and f_uid=@uid;");
            sb.Append("update up6_files set f_deleted=1 where f_pidRoot=@id and f_uid=@uid;");
            sb.Append("update up6_folders set f_deleted=1 where f_id=@id and f_uid=@uid;");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddString(ref cmd, "@id", id,32);
            db.AddInt(ref cmd, "@uid", uid);
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