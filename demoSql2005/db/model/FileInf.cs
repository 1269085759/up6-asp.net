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
            this.uid = this.pidLoc = this.pidSvr = this.idLoc = this.idSvr = 0;
            this.lenLoc = 0;
        }

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
        /// <summary>
        /// 客户端父ID(文件夹ID)
        /// </summary>
        public int pidLoc;
        /// <summary>
        /// 服务端父ID(文件夹在数据库中的ID)
        /// </summary>
        public int pidSvr;
        /// <summary>
        /// 根级文件夹ID，数据库ID，与xdb_folders.fd_id对应
        /// </summary>
        public int pidRoot;
        /// <summary>
        /// 本地文件ID。
        /// </summary>
        public int idLoc = 0;
        /// <summary>
        /// 文件在服务器中的ID。
        /// </summary>
        public int idSvr = 0;
        /// <summary>
        /// 用户ID
        /// </summary>
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
    }
}