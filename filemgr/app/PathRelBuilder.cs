using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    /// <summary>
    /// 相对路径构建器。
    /// </summary>
    public class PathRelBuilder
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        private Dictionary<string, JToken> m_folders;
        /// <summary>
        /// 需要查询的子目录列表
        /// </summary>
        private List<string> m_fdQuerys;
        /// <summary>
        /// 无关目录，非当前目录的子目录
        /// </summary>
        private Dictionary<string,string> m_childs;

        public PathRelBuilder() {
            this.m_fdQuerys = new List<string>();
            this.m_childs = new Dictionary<string, string>();
        }

        /// <summary>
        /// 当前目录ID
        /// </summary>
        /// <param name="id"></param>
        public void build(string id,ref Dictionary<string,JToken> folders)
        {
            var ids = from f in folders
                      select f.Value["f_id"].ToString();
            this.m_childs = ids.ToDictionary(x => x, x => x);

            this.m_folders = folders;
            string idCur = id;
            this.m_childs.Remove(id);
            var fdPrev = m_folders[id];
            fdPrev["f_pathRel"] = fdPrev["f_nameLoc"].ToString();
            while (true)
            {
                //更新所有子目录相对路径
                this.updateChilds(idCur, fdPrev["f_pathRel"].ToString());
                if (this.m_fdQuerys.Count == 0) break;

                idCur = this.m_fdQuerys[0];
                this.m_fdQuerys.RemoveAt(0);

                var fdCur = this.m_folders[idCur];
                var pid = fdCur["f_pid"].ToString();
                var fdParent = this.m_folders[pid];
                fdCur["f_pathRel"] = fdParent["f_pathRel"].ToString() + "/" + fdCur["f_nameLoc"].ToString();
            }

            //清除无关数据
            foreach(var c in this.m_childs)
            {
                folders.Remove(c.Key);
            }
        }

        public JToken buildFiles(ref JToken files)
        {
            var fs = files.Children<JObject>().ToDictionary(x => x["f_id"].ToString(), x => x);

            //删除无关数据
            foreach (var f in fs)
            {
                if(this.m_childs.ContainsKey( f.Value["f_pid"].ToString()))
                {
                    fs.Remove(f.Value["f_id"].ToString());
                }
            }

            JArray arr = new JArray();
            //更新路径
            foreach (var f in fs)
            {
                var parent = this.m_folders[f.Value["f_pid"].ToString()];
                f.Value["f_pathRel"] = parent["f_pathRel"].ToString() + "/" + f.Value["f_nameLoc"].ToString();
                arr.Add(f.Value);
            }

            return JToken.FromObject(arr);
        }

        /// <summary>
        /// 更新子目录相对路径
        /// </summary>
        bool updateChilds(string pid,string parentRel)
        {
            var childs = this.m_folders.Where(a => a.Value["f_pid"].ToString() == pid);
            foreach (var c in childs)
            {
                this.m_fdQuerys.Add(c.Value["f_id"].ToString());
                this.m_childs.Remove(c.Value["f_id"].ToString());
                c.Value["f_pathRel"] = parentRel + "/" + c.Value["f_nameLoc"].ToString();
            }
            return childs.Count() > 0;
        }
    }
}