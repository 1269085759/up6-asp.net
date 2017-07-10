using System.Data.Common;

namespace up6.down2.biz
{
    public class un_file : model.DnFileInf
    {
        public void read(int pidRoot, ref DbDataReader r)
        {
            this.idSvr = r.GetInt32(0);//与up6_files.f_id对应，f_down.aspx用到
            this.nameLoc = (string)r["f_nameLoc"];
            this.pathLoc = (string)r["f_pathLoc"];//
            this.lenLoc = (long)r["f_lenLoc"];
            this.perLoc = (string)r["f_perLoc"];
            this.lenSvr = (long)r["f_lenSvr"];
            this.sizeSvr = (string)r["f_sizeSvr"];
            this.fileUrl = (string)r["f_fileUrl"];
            this.pidRoot = (int)r["f_pidRoot"];
            this.fdTask = (bool)r["f_fdTask"];
            if (this.fdTask) this.pathLoc = (string)r["f_pathLoc"];
        }
    }
}