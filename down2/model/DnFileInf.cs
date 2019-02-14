using System.Collections.Generic;
using up6.db.model;

namespace up6.down2.model
{
    public class DnFileInf : FileInf
    {
        public DnFileInf() 
        {
            this.fdTask = false;
        }
        //与up6_files..f_id关联
        public string f_id= string.Empty;
        /// <summary>
        /// MAC地址
        /// </summary>
        public string mac = string.Empty;
        /// 服务器文件路径
        /// </summary>
        public string fileUrl = string.Empty;
        public string sizeSvr = "0byte";
        /// <summary>
        /// 传输进度
        /// </summary>
        public string perLoc = "0%";
    }
}