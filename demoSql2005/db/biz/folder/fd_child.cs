using System;
using System.Collections.Generic;
using System.Web;

namespace up6.demoSql2005.db.biz.folder
{
    /// <summary>
    /// 子文件夹
    /// </summary>
    public class fd_child : fd_file
    {
        public fd_child() { this.fdTask = true; }
        int fd_files=0;
        int fd_folders=0;
        int fd_filesComplete=0;
    }
}