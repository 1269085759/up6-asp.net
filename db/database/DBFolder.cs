using System.Data.Common;
using System.Text;
using up6.filemgr.app;

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

        static public void del(string id,int uid)
        {
            SqlExec se = new SqlExec();
            se.update("up6_files",
                new SqlParam[] {
                    new SqlParam("f_deleted",true)
                },
                new SqlParam[] {
                    new SqlParam("f_id", id),
                    new SqlParam("f_uid",uid)
                });
            se.update("up6_folders",
                new SqlParam[] {
                    new SqlParam("f_deleted",true)
                },
                new SqlParam[] 
                {
                    new SqlParam("f_id", id) ,
                    new SqlParam("f_uid",uid)
                });
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