using System;
using System.Collections.Generic;
using System.Web;

namespace up6.demoSql2005.db.biz.folder
{
    /// <summary>
    /// 文件夹
    /// </summary>
    public class fd_root : fd_child
    {
        public List<fd_child> folders;
        public List<fd_file> files;
        public int idFile = 0;//文件夹id与up6_files.f_id对应。
    }
}