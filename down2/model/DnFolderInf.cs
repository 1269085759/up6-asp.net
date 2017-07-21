using System.Collections.Generic;

namespace up6.down2.model
{
    public class DnFolderInf : DnFileInf
    {
        public List<DnFileInf> files;
        public DnFolderInf()
        {
            this.fdTask = true;
            this.files = new List<DnFileInf>();
        }
    }
}