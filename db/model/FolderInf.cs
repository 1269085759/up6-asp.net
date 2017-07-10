namespace up6.db.model
{
    /// <summary>
    /// 文件夹信息
    /// </summary>
    public class FolderInf
    {
        public FolderInf()
        {
            this.nameLoc = this.size = this.pathLoc = this.pathSvr = string.Empty;
            this.lenSvr = this.lenLoc = 0;//fix:
            this.uid = this.pidLoc = this.pidSvr = this.idLoc = this.idSvr = 0;
        }

        public string nameLoc;
        /// <summary>
        /// 数字化的长度，以字节为单位。示例：10252412
        /// </summary>
        public long lenLoc;
        /// <summary>
        /// 格式化的长度，示例：10GB
        /// </summary>
        public string size;
        /// <summary>
        /// 已上传大小
        /// </summary>
        public long lenSvr;
        public string perSvr;
        /// <summary>
        /// 客户端父ID，提供给JS使用。
        /// </summary>
        public int pidLoc;
        /// <summary>
        /// 服务端父ID，与数据库对应。
        /// </summary>
        public int pidSvr;
        /// <summary>
        /// 客户端文件夹ID，提供给JS使用。
        /// </summary>
        public int idLoc;
        /// <summary>
        /// 服务端文件夹ID,与数据库对应
        /// </summary>
        public int idSvr;
        /// <summary>
        /// 与xdb_files.id关联
        /// </summary>
        public int idFile;
        /// <summary>
        /// 用户ID
        /// </summary>
        public int uid;
        /// <summary>
        /// 子文件夹总数
        /// </summary>
        public int foldersCount;
        /// <summary>
        /// 子文件数
        /// </summary>
        public int filesCount;
        /// <summary>
        /// 已上传完的文件数
        /// </summary>
        public int filesComplete;
        /// <summary>
        /// 不进行URL编码，由ASPX页面进行统一编码
        /// 文件夹在客户端的路径。D:\\Soft\\Image
        /// </summary>
        public string pathLoc;
        /// <summary>
        /// 需要进行URL编解码，便于客户端不同编码的转码。
        /// 文件夹在服务端路径。E:\\Web
        /// </summary>
        public string pathSvr;
        public int pidRoot = 0;
        public string pathRel = string.Empty;
    }
}