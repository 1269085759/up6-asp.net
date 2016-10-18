using System.Data.Common;

namespace up6.demoSql2005.down2.biz
{
    public class cmp_file : model.DnFileInf
    {
        public cmp_file() { }

        public void read(int pidRoot,ref DbDataReader r)
        {
            this.idSvr = r.GetInt32(0);//与up6_files.f_id对应，f_down.aspx用到
            this.nameLoc = (string)r["f_nameLoc"];
            this.pathLoc = (string)r["f_pathLoc"];//
            this.lenSvr = (long)r["f_lenSvr"];
            this.sizeSvr = (string)r["f_sizeLoc"];
            this.pidRoot = pidRoot;
            this.fdTask = (bool)r["f_fdTask"];
            this.fdID = (int)r["f_fdID"];
            if (this.fdTask) this.pathLoc = (string)r["f_pathLoc"];
        }
    }
}