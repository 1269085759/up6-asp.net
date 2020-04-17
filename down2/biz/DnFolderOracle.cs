using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data.Common;
using up6.db.database;
using up6.down2.model;
using up6.filemgr.app;

namespace up6.down2.biz
{
    public class DnFolderOracle : DnFolder
    {
        public override void Clear()
        {
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand("TRUNCATE TABLE down_folders");
            db.ExecuteNonQuery(ref cmd);
        }

        /// <summary>
        /// 获取指定目录的所有子文件
        /// </summary>
        /// <param name="id">文件夹id</param>
        /// <returns></returns>
        public override string files(string id)
        {
            DBConfig cfg = new DBConfig();
            SqlExec se = cfg.ec();
            var fd = se.read("up6_folders", "f_pidRoot", new SqlParam[] { new SqlParam("f_id", id) });
            string pidRoot = string.Empty;
            //子目录表中不存在，表示当前目录是根目录
            if (fd == null) pidRoot = id;
            else
            {
                //子目录表中存在，表示当前目录是子目录
                pidRoot = fd["f_pidRoot"].ToString().Trim();
            }

            return this.filesChild(id, pidRoot);
        }
    }
}