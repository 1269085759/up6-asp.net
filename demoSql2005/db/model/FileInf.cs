using Newtonsoft.Json;

namespace up6.demoSql2005.db
{
    /// <summary>
    /// 文件信息，与FolderInf配合使用。
    /// </summary>
    public class FileInf
    {
        public FileInf()
        {
            this.nameLoc = this.pathLoc = this.pathSvr = this.sizeLoc = string.Empty;
            this.uid = 0;
            this.lenLoc = 0;
        }

        //文件唯一标识
        public string guid = string.Empty;
        public string pid = string.Empty;
        public string pidRoot = string.Empty;
        /// <summary>
        /// 文件名称。示例：QQ2014.exe
        /// </summary>
        public string nameLoc;
        public string nameSvr;
        /// <summary>
        /// 文件在客户端中的路径。示例：D:\\Soft\\QQ2013.exe
        /// </summary>
        public string pathLoc;
        /// <summary>
        /// 文件在服务器上面的路径。示例：E:\\Web\\Upload\\QQ2013.exe
        /// </summary>
        public string pathSvr;
        /// <summary>
        /// 文件MD5
        /// </summary>
        public string md5 = string.Empty;
        public int uid = 0;
        /// <summary>
        /// 数字化的长度。以字节为单位，示例：1021021
        /// </summary>
        public long lenLoc = 0;
        /// <summary>
        /// 格式化的长度。示例：10G
        /// </summary>
        public string sizeLoc = "0bytes";
        /// <summary>
        /// 已上传大小
        /// </summary>
        public long lenSvr = 0;
        /// <summary>
        /// 文件上传位置。
        /// </summary>
        public long postPos = 0;
        /// <summary>
        /// 上传百分比
        /// </summary>
        public string perSvr = "0%";
        /// <summary>
        /// 相对路径。root\\child\\folderName\\fileName.txt
        /// </summary>
        public string pathRel = string.Empty;
        public bool complete = false;
        public bool m_fdTask = false;
    }
}