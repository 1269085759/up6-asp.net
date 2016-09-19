using Newtonsoft.Json;
using System.Collections.Generic;

namespace up6.demoSql2005.down2.model
{
    public class DnFileInf
    {
        public DnFileInf()
        {
            this.m_fdTask = false;
        }

        public int idSvr { get { return this.m_fid; } set { this.m_fid = value; } }
        public int uid { get { return this.m_uid; } set { this.m_uid = value; } }
        public string mac { get { return this.m_mac; } set { this.m_mac = value; } }
        public string pathLoc { get { return this.m_pathLoc; } set { this.m_pathLoc = value; } }
        public string fileUrl { get { return this.m_pathSvr; } set { this.m_pathSvr = value; } }
        public long lenLoc { get { return this.m_lengthLoc; } set { this.m_lengthLoc = value; } }
        public long lenSvr { get { return this.m_lengthSvr; } set { this.m_lengthSvr = value; } }
        public string sizeSvr { get { return this.m_sizeSvr; } set { this.m_sizeSvr= value; } }
        public string perLoc { get { return this.m_percent; } set { this.m_percent = value; } }
        /// <summary>
        /// 是否已下载完成
        /// </summary>
        public bool complete {get { return this.m_complete; }set { this.m_complete = value; }}
        /// <summary>
        /// 本地文件名称，用来显示用的。
        /// </summary>
        public string nameLoc { get { return this.m_name; } set { this.m_name = value; } }
        public bool fdTask { get { return this.m_fdTask; } set { this.m_fdTask = value; } }
        public int fdID { get { return this.m_fdID; } set { this.m_fdID = value; } }
        public int pidRoot { get { return this.m_pidRoot; } set { this.m_pidRoot = value; } }

        public List<DnFileInf> files { get { return this.m_files; } set { this.m_files = value; } }

        private string m_name=string.Empty;
        private bool m_fdTask=false;//是否是文件夹
        private int m_fdID=0;
        private int m_pidRoot=0;
        private bool m_complete = false;

        [JsonIgnore]
        public int m_fid=0;

        /// <summary>
        /// 用户ID
        /// </summary>
        [JsonIgnore]
        public int m_uid=0;

        /// <summary>
        /// MAC地址
        /// </summary>
        [JsonIgnore]
        public string m_mac = string.Empty;

        /// <summary>
        /// 本地文件路径
        /// </summary>
        [JsonIgnore]
        public string m_pathLoc = string.Empty;

        /// <summary>
        /// 服务器文件路径
        /// </summary>
        [JsonIgnore]
        public string m_pathSvr=string.Empty;

        /// <summary>
        /// 本地文件长度
        /// </summary>
        [JsonIgnore]
        public long m_lengthLoc=0;

        /// <summary>
        /// 服务器文件长度
        /// </summary>
        [JsonIgnore]
        public long m_lengthSvr =0;

        [JsonIgnore]
        public string m_sizeSvr = "0byte";

        /// <summary>
        /// 传输进度
        /// </summary>
        [JsonIgnore]
        public string m_percent="0%";

        [JsonIgnore]
        public List<DnFileInf> m_files;
    }
}