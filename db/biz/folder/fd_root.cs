using System.Collections.Generic;
using up6.db.model;

namespace up6.db.biz.folder
{
    /// <summary>
    /// 文件夹
    /// </summary>
    public class fd_root : fd_child
    {
        public List<fd_child> folders;
        public List<FileInf> files;
    }
}