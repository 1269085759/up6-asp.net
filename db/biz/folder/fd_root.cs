using System.Collections.Generic;
using up6.db.model;

namespace up6.db.biz.folder
{
    /// <summary>
    /// 文件夹
    /// </summary>
    public class fd_root : FileInf
    {
        public List<FileInf> folders;
        public List<FileInf> files;

        public fd_root()
        {
            this.fdTask = true;
        }
    }
}