using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data.Common;
using up6.db.database;
using up6.down2.model;
using up6.filemgr.app;

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

        /// <summary>
        /// 获取指定目录的所有子文件
        /// </summary>
        /// <param name="id">文件夹id</param>
        /// <returns></returns>
        public string files(string id)
        {
            var se = new SqlExec();
            var fd = se.read("up6_folders", "f_pidRoot", new SqlParam[] { new SqlParam("f_id", id) });
            string pidRoot = string.Empty;
            //子目录表中不存在，表示当前目录是根目录
            if (fd == null) pidRoot = id;
            else
            {
                //子目录表中存在，表示当前目录是子目录
                pidRoot = fd["f_pidRoot"].ToString().Trim();
            }
            
            return this.filesChild(id,pidRoot);
        }

        /// <summary>
        /// 取子目录所有文件
        /// </summary>
        /// <returns></returns>
        public string filesChild(string id,string pidRoot)
        {
            //构建子目录路径
            PathRelBuilder prb = new PathRelBuilder();
            var fs = prb.build(id,pidRoot);
            

            return JsonConvert.SerializeObject(fs);
        }
    }
}