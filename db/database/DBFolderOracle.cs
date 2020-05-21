using System.Data.Common;
using System.Text;
using up6.filemgr.app;

namespace up6.db.database
{
    public class DBFolderOracle : DBFolder
    {
        public override void Remove(string id, int uid)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("begin ");
            sb.Append("update up6_files set f_deleted=1 where f_id=:f_id and f_uid=:f_uid;");
            sb.Append("update up6_files set f_deleted=1 where f_pidRoot=:f_id and f_uid=:f_uid;");
            sb.Append("update up6_folders set f_deleted=1 where f_id=:f_id and f_uid=:f_uid;");
            sb.Append(" end;");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddString(ref cmd, ":f_id", id, 32);
            db.AddInt(ref cmd, ":f_uid", uid);
            db.ExecuteNonQuery(cmd);
        }

        public override void del(string id, int uid)
        {
            DBConfig cfg = new DBConfig();
            SqlExec se = cfg.se();
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

        public override void Clear()
        {
            string sql = "truncate table up6_folders";
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sql);
            db.ExecuteNonQuery(cmd);
        }
    }
}