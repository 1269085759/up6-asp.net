using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    /// <summary>
    /// 相对路径构建器。
    /// 使用：
    /// PathRelBuilder prb = new PathRelBuilder();
    /// var fs = prb.build(id.pidRoot)
    /// </summary>
    public class PathRelBuilder
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        private Dictionary<string, JToken> m_folders;
        private JToken m_files;
        /// <summary>
        /// 需要查询的子目录列表
        /// </summary>
        private List<string> m_fdQuerys;
        /// <summary>
        /// 子目录列表
        /// </summary>
        private Dictionary<string, string> m_childs;

        public PathRelBuilder() {
            this.m_fdQuerys = new List<string>();
            this.m_files = null;
            this.m_childs = new Dictionary<string, string>();
        }

        public JToken build(string id, string pidRoot)
        {
            this.query(id, pidRoot);
            this.buildFolder(id);
            return this.buildFile();
        }

        /// <summary>
        /// 查询目录和文件列表
        /// 只加载已完成且未删除的
        /// </summary>
        /// <param name="id"></param>
        void query(string id,string pidRoot)
        {
            //查询目录
            var se = new SqlExec();
            var folders = se.select("up6_folders"
                , "f_id,f_nameLoc,f_pid,f_pidRoot"
                , new SqlParam[] { new SqlParam("f_deleted", 0) }
                , string.Empty);

            //取所有子目录
            this.m_folders = folders.Children<JObject>().ToDictionary(x => x["f_id"].ToString(), x =>JToken.FromObject(x));

            //是根节点
            if (id == pidRoot)
            {
                var root = se.read("up6_files", "f_nameLoc", new SqlParam[] { new SqlParam("f_id", id) });
                this.m_folders.Add(id, new JObject { { "f_id", id }, { "f_pid", string.Empty }, { "f_pidRoot", string.Empty }, { "f_nameLoc", root["f_nameLoc"].ToString() } });
            }

            //查询pidRoot全部文件
            this.m_files = se.select("up6_files"
                , "f_id,f_pid,f_nameLoc,f_pathSvr,f_pathRel,f_lenSvr,f_sizeLoc"
                , new SqlParam[] {
                    new SqlParam("f_pidRoot", pidRoot)
                    ,new SqlParam("f_deleted", false)
                    ,new SqlParam("f_complete", true)
                }
            );
        }

        /// <summary>
        /// 为所有子目录构建路径
        /// </summary>
        /// <param name="id"></param>
        void buildFolder(string id)
        {
            string idCur = id;
            this.m_childs.Add(id, id);
            var fdPrev = m_folders[id];
            fdPrev["f_pathRel"] = string.Empty;
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
                if(!this.m_folders.ContainsKey(c.Key)) this.m_folders.Remove(c.Key);
            }
        }

        /// <summary>
        /// 为所有文件构建路径
        /// 格式：
        /// [
        ///   {nameLoc,pathSvr,pathRel,lenSvr,sizeSvr}
        ///   {nameLoc,pathSvr,pathRel,lenSvr,sizeSvr}
        /// ]
        /// </summary>
        /// <returns></returns>
        JToken buildFile()
        {
            var fs = this.m_files.Children<JObject>().ToDictionary(x => x["f_id"].ToString(), x => x);

            JArray arr = new JArray();
            //更新路径
            foreach (var f in fs)
            {
                if (!this.m_childs.ContainsKey(f.Value["f_pid"].ToString())) continue;

                var parent = this.m_folders[f.Value["f_pid"].ToString()];
                f.Value["f_pathRel"] = parent["f_pathRel"].ToString() + "/" + f.Value["f_nameLoc"].ToString();

                //更名
                f.Value["nameLoc"] = f.Value["f_nameLoc"].ToString();
                f.Value["pathSvr"] = f.Value["f_pathSvr"].ToString();
                f.Value["pathRel"] = f.Value["f_pathRel"].ToString();
                f.Value["lenSvr"] = f.Value["f_lenSvr"].ToString();
                f.Value["sizeSvr"] = f.Value["f_sizeLoc"].ToString();
                arr.Add(f.Value);
            }

            return JToken.FromObject(arr);
        }

        /// <summary>
        /// 更新所有子目录路径
        /// </summary>
        bool updateChilds(string pid,string parentRel)
        {
            var childs = this.m_folders.Where(a => a.Value["f_pid"].ToString() == pid);
            foreach (var c in childs)
            {
                this.m_fdQuerys.Add(c.Value["f_id"].ToString());
                this.m_childs.Add(c.Value["f_id"].ToString(), c.Value["f_id"].ToString());
                c.Value["f_pathRel"] = parentRel + "/" + c.Value["f_nameLoc"].ToString();
                if (string.IsNullOrEmpty(parentRel))
                    c.Value["f_pathRel"] = c.Value["f_nameLoc"].ToString();
            }
            return childs.Count() > 0;
        }
    }
}