using System.Collections.Generic;

namespace up6.down2.model
{
    public class DnFileInf
    {
        public DnFileInf() 
        {
            this.fdTask = false;
        }

        public int idSvr = 0;
        /// <summary>
        /// 用户ID
        /// </summary>
        public int uid = 0;
        /// <summary>
        /// MAC地址
        /// </summary>
        public string mac = string.Empty;
        /// <summary>
        /// 本地文件路径
        /// </summary>
        public string pathLoc = string.Empty;
        /// <summary>
        /// 服务器文件路径
        /// </summary>
        public string fileUrl = string.Empty;
        /// <summary>
        /// 本地文件长度
        /// </summary>
        public long lenLoc = 0;
        /// <summary>
        /// 服务器文件长度
        /// </summary>
        public long lenSvr = 0;
        public string sizeSvr = "0byte";
        /// <summary>
        /// 传输进度
        /// </summary>
        public string perLoc = "0%";
        /// <summary>
        /// 是否已下载完成
        /// </summary>
        public bool complete = false;
        /// <summary>
        /// 本地文件名称，用来显示用的。
        /// </summary>
        public string nameLoc = string.Empty;
        public bool fdTask = false;//是否是文件夹
        public int fdID = 0;
        public int pidRoot = 0;

        public List<DnFileInf> files;
    }
}