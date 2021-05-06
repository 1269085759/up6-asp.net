using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using up6.db.database;
using up6.filemgr.app;

namespace up6.down2.biz
{
    public class DnFolder
    {
        /// <summary>
        /// 清空数据库
        /// </summary>
        public virtual void Clear()
        {
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand("delete from down_folders");
            db.ExecuteNonQuery(ref cmd);
        }

        /// <summary>
        /// 取根目录下所有子文件
        /// 注意：传进来的必须是根目录ID
        /// </summary>
        /// <param name="pidRoot"></param>
        /// <returns></returns>
        public string childs(string pidRoot)
        {
            var se = new SqlExec();
            var fs = (JArray)se.select("up6_files", "f_id,f_nameLoc,f_pathSvr,f_pathRel,f_lenSvr",
                new SqlParam[] { new SqlParam("f_pidRoot", pidRoot) }
                );

            var childs = new JArray();
            foreach(var o in fs)
            {
                childs.Add(new JObject {
                    { "f_id", o["f_id"]},
                    { "nameLoc", o["f_nameLoc"]},
                    { "pathSvr", o["f_pathSvr"]},
                    { "pathRel", o["f_pathRel"]},
                    { "lenSvr", o["f_lenSvr"]}
                });
            }
            return JsonConvert.SerializeObject(childs);
        }
    }
}