using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Web;

namespace up6.demoSql2005.db.biz
{
    public class un_file : folder.fd_root
    {
        public void read(int pidRoot,ref DbDataReader r)
        {
            this.idSvr = Convert.ToInt32(r["f_id"]);
            this.nameLoc = r["f_nameLoc"].ToString();
            this.nameSvr = r["f_nameSvr"].ToString();
            this.pidSvr = int.Parse( r["f_pid"].ToString());
            this.fdTask = Convert.ToBoolean(r["f_fdTask"]);
            this.fdChild = Convert.ToBoolean(r["f_fdChild"]);
            this.fdID = int.Parse(r["f_fdID"].ToString());
            this.pathLoc = r["f_pathLoc"].ToString();
            this.pathSvr = r["f_pathSvr"].ToString();
            this.lenLoc = long.Parse( r["f_lenLoc"].ToString());
            this.sizeLoc = r["f_sizeLoc"].ToString();
            this.lenSvr = long.Parse(r["f_lenSvr"].ToString());
            this.perSvr = r["f_perSvr"].ToString();
            this.pos = long.Parse(r["f_pos"].ToString());
            this.complete = Convert.ToBoolean(r["f_complete"]);
            this.md5 = r["f_md5"].ToString();
        }

        public void copy(ref un_file f)
        {
            this.idFile = f.idSvr;
            this.idSvr = f.idSvr;
            this.nameLoc = f.nameLoc;
            this.nameSvr = f.nameSvr;
            this.pidSvr = f.pidSvr;
            this.fdTask = f.fdTask;
            this.fdChild = f.fdChild;
            this.fdID = f.fdID;
            this.pathLoc = f.pathLoc;
            this.pathSvr = f.pathSvr;
            this.lenLoc = f.lenLoc;
            this.sizeLoc = f.sizeLoc;
            this.lenSvr = f.lenSvr;
            this.perSvr = f.perSvr;
            this.pos = f.pos;
            this.complete = f.complete;
            this.md5 = f.md5;
        }
    }
}