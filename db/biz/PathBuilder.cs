using System.Web;
using up6.db.model;
using up6.filemgr.app;

namespace up6.db.biz
{
    /// <summary>
    /// 路径生成器基类
    /// 提供文件或文件夹的存储路径
    /// </summary>
    public class PathBuilder
    {
        /// <summary>
        /// 根级存储路径,
        /// </summary>
        /// <returns></returns>
        public string getRoot()
        {
            ConfigReader cr = new ConfigReader();
            var uploadFolder = cr.module("path").SelectToken("upload-folder").ToString();
            uploadFolder = uploadFolder.Replace("{root}", HttpContext.Current.Server.MapPath("/"));

            return uploadFolder;
        }

        public virtual string genFolder(ref FileInf fd)
        {
            return string.Empty;
        }

        public virtual string genFile(int uid, ref FileInf f)
        {
            return string.Empty;
        }
        public virtual string genFile(int uid, string md5, string nameLoc)
        {
            return string.Empty;
        }
    }
}