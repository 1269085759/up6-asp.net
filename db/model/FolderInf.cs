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
            this.uid   = 0;
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
        /// 用户ID
        /// </summary>
        public int uid;
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
        public string pathRel = string.Empty;
    }
}