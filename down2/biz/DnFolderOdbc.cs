using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using up6.db.database;
using up6.filemgr.app;

namespace up6.down2.biz
{
    public class DnFolderOdbc: DnFolder
    {
        public override void Clear()
        {
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand("TRUNCATE TABLE down_folders");
            db.ExecuteNonQuery(ref cmd);
        }
    }
}