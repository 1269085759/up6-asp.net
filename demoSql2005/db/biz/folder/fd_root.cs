using System.Collections.Generic;

namespace up6.demoSql2005.db.biz.folder
{
    /// <summary>
    /// 文件夹
    /// </summary>
    public class fd_root : fd_child
    {
        public List<fd_child> folders;
        public List<xdb_files> files;
    }
}