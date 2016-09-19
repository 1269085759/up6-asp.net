using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;

namespace up6.demoSql2005.down2.model
{
    public class DnFolderInf : DnFileInf
    {
        public DnFolderInf()
        {
            this.fdTask = true;
            this.m_files = new List<DnFileInf>();
        }
    }
}