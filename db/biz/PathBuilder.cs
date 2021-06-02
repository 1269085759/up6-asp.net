using System.IO;
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

        /// <summary>
        /// 相对路径转换成绝对路径
        /// /2021/05/28/guid/nameLoc => d:/upload/2021/05/28/guid/nameLoc
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string relToAbs(string path)
        {
            string root = this.getRoot();
            root = root.Replace("\\", "/");
            path = path.Replace("\\", "/");
            if (path.StartsWith("/"))
            {
                path = PathTool.combin(root, path);
            }
            return path;
        }

        /// <summary>
        /// 将路径转换成相对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string absToRel(string path)
        {
            string root = this.getRoot().Replace("\\","/");
            path = path.Replace("\\", "/");
            path = path.Replace(root, string.Empty);
            return path;
        }
    }
}