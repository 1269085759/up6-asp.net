using up6.demoSql2005.db;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace up6.demoSql2005.down2.db
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