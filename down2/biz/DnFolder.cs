using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Common;
using up6.db.database;
using up6.down2.model;

namespace up6.down2.biz
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