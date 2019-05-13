﻿using Newtonsoft.Json.Linq;
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
            var obj = se.read("up7_folders", "f_id,f_pid,f_pidRoot", new SqlParam[] {
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
            var obj = se.read("up7_folders", "f_id,f_pid,f_pidRoot", new SqlParam[] {
                new SqlParam("f_id",id)
            });

            var list = se.select("up7_folders", "f_id,f_pid", new SqlParam[] {
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

        /// <summary>
        /// 根据pid查询
        /// </summary>
        /// <param name="fdCur"></param>
        public static JToken build_path_by_id(JObject fdCur) {

            var id = fdCur["f_id"].ToString().Trim();//
            var pid = fdCur["f_pid"].ToString().Trim();//
            var pidRoot = fdCur["f_pidRoot"].ToString().Trim();//
            bool isRoot = string.IsNullOrEmpty(pid);//根目录

            //根目录
            List<JToken> psort = new List<JToken>();
            if (string.IsNullOrEmpty(id))
            {
                psort.Insert(0, new JObject { { "f_id", "" }, { "f_nameLoc", "根目录" }, { "f_pid", "" }, { "f_pidRoot", "" } });

                return JToken.FromObject(psort);
            }

            string sql = "select f_id,f_nameLoc,f_pid,f_pidRoot from up7_folders where f_pid='';";
            //子目录
            if (!isRoot)
            {
                //加载所有根目录结构
                sql = string.Format("select f_id,f_nameLoc,f_pid,f_pidRoot from up7_folders where f_pidRoot='{0}'", pidRoot);
            }

            SqlExec se = new SqlExec();

            var folders = se.exec("up7_folders", sql, "f_id,f_nameLoc,f_pid,f_pidRoot");

            //构建目录映射表(id,folder)
            //id,folder
            Dictionary<string, JToken> dt = new Dictionary<string, JToken>();
            foreach (var fd in folders)
            {
                dt[fd["f_id"].ToString()] = fd;
            }

            //按层级顺序排列目录
            string cur = fdCur["f_id"].ToString().Trim();
            while (true)
            {
                //key不存在
                if (!dt.ContainsKey(cur)) break;

                var d = dt[cur];//查父ID
                psort.Insert(0, d);
                cur = d["f_pid"].ToString().Trim();//取父级ID

                if (cur.Trim() == "0") break;
                if (string.IsNullOrEmpty(cur)) break;
            }

            //是子目录->添加根目录
            if (!string.IsNullOrEmpty(pidRoot))
            {
                var root = se.read("up7_files", "f_id,f_nameLoc,f_pid,f_pidRoot", new SqlParam[] { new SqlParam("f_id", pidRoot) });
                psort.Insert(0, root);
            }//是根目录->添加根目录
            else if (!string.IsNullOrEmpty(id) && string.IsNullOrEmpty(pidRoot))
            {
                var root = se.read("up7_files", "f_id,f_nameLoc,f_pid,f_pidRoot", new SqlParam[] { new SqlParam("f_id", id) });
                psort.Insert(0, root);
            }
            psort.Insert(0, (new JObject { { "f_id", "" }, { "f_nameLoc", "根目录" }, { "f_pid", "" }, { "f_pidRoot", "" } }));

            return JToken.FromObject(psort);
        }

        /// <summary>
        /// 获取所有子目录
        /// </summary>
        /// <param name="id"></param>
        public static string[] all_childs(string id)
        {

            SqlExec se = new SqlExec();

            var folders = se.select("up7_folders", "f_id,f_pid", null);

            var dt = new Dictionary<string, HashSet<string>>();
            foreach (var f in folders)
            {
                var fid = f["f_id"].ToString().Trim();
                var fpid = f["f_pid"].ToString().Trim();

                if (fpid == string.Empty) fpid = "0";

                if (dt.ContainsKey(fpid))
                {
                    dt[fpid].Add(fid);
                }
                else {
                    dt[fpid] = new HashSet<string>();
                    dt[fpid].Add(fid);
                }
            }

            if (id.Trim() == "") id = "0";
            var pids = new List<string>();
            pids.Add(id);

            //子目录ID列表
            var childs = new List<string>();
            var pidCur = id;
            while (pids.Count>0)
            {
                //不存在此pid的数据
                if(!dt.ContainsKey(pidCur) ) break;

                childs.AddRange(dt[pidCur]);
                pids.AddRange(dt[pidCur]);//将所有子ID添加成pid
                pids.Remove(pidCur);//移除此PID
                pidCur = dt[pidCur].First();
            }

            return childs.ToArray();
        }
    }
}