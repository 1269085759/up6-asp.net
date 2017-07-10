using System.Data.Common;
using up6.db;

namespace up6.down2.db
{
    public class DnFolder
    {
        /// <summary>
        /// 清空数据库
        /// </summary>
        public static void Clear()
        {
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand("delete from down_folders;");
            db.ExecuteNonQuery(ref cmd);
        }
    }
}