using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    /// <summary>
    /// 构建文件夹下载数据
    /// 格式：
    /// [
    ///   {nameLoc,pathSvr,pathRel,lenSvr,sizeSvr}
    ///   {nameLoc,pathSvr,pathRel,lenSvr,sizeSvr}
    /// ]
    /// </summary>
    public class FolderBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">文件夹ID</param>
        /// <returns></returns>
        public JToken build(string id) {
            SqlExec se = new SqlExec();
            var o = se.read("up6_files", "*", new SqlParam[] { new SqlParam("f_id", id) });
            //子目录
            if(o==null)
            {
                o = se.read("up6_folders", "*", new SqlParam[] { new SqlParam("f_id", id) });
            }

            string pathRoot = o["f_pathRel"].ToString();
            var index = pathRoot.Length;

            JArray fs = new JArray();

            //查询文件
            string where = string.Format("CHARINDEX('{0}',f_pathRel)>0 and f_fdTask=0 and f_deleted=0", pathRoot+"/");
            var files = (JArray)se.select("up6_files", "*", where);
            int count = files.Count();//获取数组的长度
            for (int i = 0; i < count; i++)
            {
                var pathRel = files[i]["f_pathRel"].ToString();
                var fo = new JObject {
                    { "f_id",files[i]["f_id"]},
                    { "nameLoc",files[i]["f_nameLoc"]},
                    { "pathSvr",files[i]["f_pathSvr"]},
                    { "pathRel",pathRel.Substring(index)},
                    { "lenSvr",files[i]["f_lenSvr"]},
                    { "sizeSvr",files[i]["f_sizeLoc"]}
                };
                fs.Add(fo);
            }
            return JToken.FromObject(fs);
        }
    }
}