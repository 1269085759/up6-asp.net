using System;
using System.Collections.Generic;
using System.Web;

namespace up6.demoSql2005.db.biz.folder
{
    /// <summary>
    /// 文件
    /// </summary>
    public class fd_file
    {
        public int idLoc = 0;
        public int idSvr = 0;//与up6_files.f_id对应
        public string nameLoc = string.Empty;
        public string nameSvr = string.Empty;
        public string pathLoc = string.Empty;
        public string pathSvr = string.Empty;
        public string pathRel = string.Empty;
        public string md5 = string.Empty;
        public int pidLoc = 0;
        public int pidSvr = 0;
        public int pidRoot = 0;//
        public int fdID = 0;//与up6_folders.fd_id对应，提供给文件夹使用。
        public bool fdChild;//是否是一个子文件
        public long lenLoc = 0;
        public string sizeLoc = "0";//sizeLoc
        public long pos;//上传位置
        public long lenSvr = 0;
        public string perSvr = "0%";
        public int uid = 0;
        public int filesCount = 0;
        public int foldersCount = 0;
        public bool complete = false;
        public bool fdTask = false;
    }
}