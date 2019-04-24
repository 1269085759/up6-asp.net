using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    /// <summary>
    /// 文件夹相关的操作
    /// </summary>
    public class DbFolder
    {

        /// <summary>
        /// 获取文件夹中所有子项（文件和目录）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string[] all_files(string id)
        {
            SqlExec se = new SqlExec();
            var obj = se.read("up6_folders", "f_id,f_pid,f_pidRoot", new SqlParam[] {
                new SqlParam("f_id",id)
            });

            var list = se.select("up6_files", "f_id", new SqlParam[] {
                new SqlParam("f_pidRoot",obj["f_pidRoot"].ToString())
            });

            var ids = from f in list
                      select f["f_id"].ToString();
            return ids.ToArray();
        }

        /// <summary>
        /// 查询所有文件夹，包含子文件夹
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string[] all_folders(string id)
        {
            SqlExec se = new SqlExec();
            var obj = se.read("up6_folders", "f_id,f_pid,f_pidRoot", new SqlParam[] {
                new SqlParam("f_id",id)
            });

            var list = se.select("up6_folders", "f_id,f_pid", new SqlParam[] {
                new SqlParam("f_pidRoot",obj["f_pidRoot"].ToString())
            });

            var dt = list.Children<JObject>()
                .ToDictionary(x => x["f_pid"].ToString(), x => x["f_id"].ToString().Trim());

            var dt_sel = new Dictionary<string, string>();

            var folders = list.ToArray();
            string pid_cur = id;
            foreach(var i in list)
            {
                if (!dt_sel.ContainsKey(pid_cur)) continue;

                if (i["f_pid"].ToString().Trim() == pid_cur)
                {
                    dt_sel.Add(i["f_id"].ToString(), i["f_pid"].ToString());
                    pid_cur = i["f_id"].ToString();
                }
            }

            var ids = from f in list
                      select f["f_id"].ToString();
            return ids.ToArray();
        }
    }
}