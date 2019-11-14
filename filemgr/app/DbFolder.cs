using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using up6.db.model;

namespace up6.filemgr.app
{
    /// <summary>
    /// 文件夹相关的操作
    /// </summary>
    public class DbFolder
    {
        //根节点
        JObject root = new JObject { { "f_id", "" }, { "f_nameLoc", "根目录" }, { "f_pid", "" }, { "f_pidRoot", "" } };

        public DbFolder()
        {
        }

        /// <summary>
        /// 将目录列表转换成键值表
        /// f_id,folder
        /// f_id,folder
        /// </summary>
        /// <param name="folders"></param>
        /// <returns></returns>
        public Dictionary<string, JToken> toDic(ref JToken folders)
        {
            Dictionary<string, JToken> dt = new Dictionary<string, JToken>();
            foreach (var fd in folders)
            {
                dt[fd["f_id"].ToString()] = fd;
            }
            return dt;
        }

        /// <summary>
        /// 将所有目录转换成关联数组
        /// </summary>
        /// <param name="id">文件夹ID</param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public Dictionary<String, JToken> foldersToDic(string pidRoot)
        {
            //默认加载根目录
            string sql = string.Format("select f_id,f_nameLoc,f_pid,f_pidRoot from up6_folders where f_pidRoot='{0}'", pidRoot);

            SqlExec se = new SqlExec();
            var folders = se.exec("up6_folders", sql, "f_id,f_nameLoc,f_pid,f_pidRoot");
            return this.toDic(ref folders);
        }

        /// <summary>
        /// 按照层级顺序排序
        /// pid,idCur
        /// </summary>
        /// <param name="dt">目录表</param>
        /// <param name="idCur">当前目录ID</param>
        /// <param name="psort">排序结果</param>
        public void sortByPid(ref Dictionary<string, JToken> dt, string idCur, ref List<JToken> psort) {

            string cur = idCur;
            while (true)
            {
                //key不存在
                if (!dt.ContainsKey(cur)) break;

                var d = dt[cur];//查父ID
                psort.Insert(0, d);//将父节点排在前面
                cur = d["f_pid"].ToString().Trim();//取父级ID

                if (cur.Trim() == "0") break;
                if (string.IsNullOrEmpty(cur)) break;
            }
        }

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
        public JToken build_path_by_id(JObject fdCur) {

            var id = fdCur["f_id"].ToString().Trim();//
            var pidRoot = fdCur["f_pidRoot"].ToString().Trim();//

            //根目录
            List<JToken> psort = new List<JToken>();
            if (string.IsNullOrEmpty(id))
            {
                psort.Insert(0, this.root);

                return JToken.FromObject(psort);
            }

            //当前目录是子目录
            SqlExec se = new SqlExec();
            var data = (JArray)se.selectUnion(new string[] { "up6_files", "up6_folders" }, "f_pidRoot"
                , new SqlParam[] { new SqlParam("f_id", id) });
            if (data.Count > 0) pidRoot = data[0]["f_pidRoot"].ToString().Trim();

            //构建目录映射表(id,folder)
            Dictionary<string, JToken> dt = this.foldersToDic(pidRoot);

            //按层级顺序排列目录
            this.sortByPid(ref dt, id, ref psort);

            //是子目录->添加根目录
            if (!string.IsNullOrEmpty(pidRoot))
            {
                var cur = se.read("up6_files", "f_id,f_nameLoc,f_pid,f_pidRoot", new SqlParam[] { new SqlParam("f_id", pidRoot) });
                psort.Insert(0, cur);
            }//是根目录->添加根目录
            else if (!string.IsNullOrEmpty(id) && string.IsNullOrEmpty(pidRoot))
            {
                var root = se.read("up6_files", "f_id,f_nameLoc,f_pid,f_pidRoot", new SqlParam[] { new SqlParam("f_id", id) });
                psort.Insert(0, root);
            }
            psort.Insert(0, this.root);

            return JToken.FromObject(psort);
        }

        /// <summary>
        /// 构建路径
        /// </summary>
        /// <param name="fd"></param>
        /// <returns></returns>
        public JToken build_path(JObject fdCur)
        {
            //查询文件表目录数据
            SqlExec se = new SqlExec();
            var files = se.select("up6_files", "f_id,f_pid,f_nameLoc", new SqlParam[] { new SqlParam("f_fdTask", true) });
            var folders = se.select("up6_folders", "f_id,f_pid,f_nameLoc",new SqlParam[] { });
            var id = fdCur["f_id"].ToString().Trim();//

            //根目录
            List<JToken> psort = new List<JToken>();
            if (string.IsNullOrEmpty(id))
            {
                psort.Insert(0, this.root);

                return JToken.FromObject(psort);
            }

            //构建目录映射表(id,folder)
            Dictionary<string, JToken> dtFiles = this.toDic(ref files);
            Dictionary<string, JToken> dtFolders = this.toDic(ref folders);
            foreach (var fd in dtFolders)
            {
                if(!dtFiles.ContainsKey(fd.Key))dtFiles.Add(fd.Key, fd.Value);
            }

            //按层级顺序排列目录
            this.sortByPid(ref dtFiles, id, ref psort);

            psort.Insert(0, this.root);

            return JToken.FromObject(psort);
        }

        /// <summary>
        /// 获取所有子目录
        /// </summary>
        /// <param name="id"></param>
        public static string[] all_childs(string id)
        {

            SqlExec se = new SqlExec();

            var folders = se.select("up6_folders", "f_id,f_pid", string.Empty);

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

        public bool exist_same_file(string name,string pid)
        {
            SqlWhereMerge swm = new SqlWhereMerge();
            swm.equal("f_nameLoc", name.Trim());
            swm.equal("f_pid", pid.Trim());
            swm.equal("f_deleted", 0);

            string sql = string.Format("select f_id from up6_files where {0} ", swm.to_sql());

            var se = new SqlExec();
            var arr = (JArray)se.exec("up6_files", sql, "f_id", string.Empty);
            return arr.Count > 0;
        }

        public bool exist_same_folder(string name,string pid)
        {
            SqlWhereMerge swm = new SqlWhereMerge();
            swm.equal("f_nameLoc", name.Trim());
            swm.equal("f_deleted", 0);
            swm.equal("LTRIM (f_pid)", pid.Trim());

            string sql = string.Format("select f_id from up6_files where {0} " +
                                        " union select f_id from up6_folders where {0}", swm.to_sql());

            var se = new SqlExec();
            var fid = (JArray)se.exec("up6_files", sql, "f_id", string.Empty);
            return fid.Count > 0;
        }

        /// <summary>
        /// 获取根目录或子目录信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileInf read(string id) {
            SqlExec se = new SqlExec();
            string sql = string.Format("select f_pid,f_pidRoot,f_pathSvr from up6_files where f_id='{0}' union select f_pid,f_pidRoot,f_pathSvr from up6_folders where f_id='{0}'", id);
            var data = (JArray)se.exec("up6_files", sql, "f_pid,f_pidRoot,f_pathSvr");
            var o = JObject.FromObject(data[0]);

            FileInf file = new FileInf();
            file.id = id;
            file.pid = o["f_pid"].ToString().Trim();
            file.pidRoot = o["f_pidRoot"].ToString().Trim();
            file.pathSvr = o["f_pathSvr"].ToString().Trim();
            return file;
        }

        /// <summary>
        /// 重命名文件检查
        /// </summary>
        /// <param name="newName"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public bool rename_file_check(string newName,string pid)
        {
            SqlExec se = new SqlExec();            
            var res = (JArray)se.select("up6_files"
                , "f_id"
                ,new SqlParam[] {
                    new SqlParam("f_nameLoc",newName)
                    ,new SqlParam("f_pid",pid)
                });
            return res.Count > 0;
        }

        /// <summary>
        /// 重命名目录检查
        /// </summary>
        /// <param name="newName"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public bool rename_folder_check(string newName, string pid)
        {
            SqlExec se = new SqlExec();
            var res = (JArray)se.select("up6_folders"
                , "f_id"
                , new SqlParam[] {
                    new SqlParam("f_nameLoc",newName)
                    ,new SqlParam("f_pid",pid)
                });
            return res.Count > 0;
        }

        public void rename_file(string name,string id) {
            SqlExec se = new SqlExec();
            se.update("up6_files"
                , new SqlParam[] { new SqlParam("f_nameLoc", name) }
                , new SqlParam[] { new SqlParam("f_id", id) });
        }
        public void rename_folder(string name, string id, string pid) {
            SqlExec se = new SqlExec();
            se.update("up6_folders"
                , new SqlParam[] { new SqlParam("f_nameLoc", name) }
                , new SqlParam[] { new SqlParam("f_id", id) });
        }
    }
}