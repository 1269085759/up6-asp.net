using System.Collections.Generic;

namespace up6.down2.model
{
    public class DnFolderInf : DnFileInf
    {
        public DnFolderInf()
        {
            this.fdTask = true;
            this.files = new List<DnFileInf>();
        }
    }
}