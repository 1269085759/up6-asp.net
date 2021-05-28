using System;
using up6.db.database;
using up6.down2.biz;
using up6.filemgr.app;

namespace up6.down2.db
{
    /// <summary>
    /// 更新文件下载进度
    /// </summary>
    public partial class f_update : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string fid      = Request.QueryString["id"];
            string uid      = Request.QueryString["uid"];
            string lenLoc   = Request.QueryString["lenLoc"];
            string per      = Request.QueryString["perLoc"];
            string cbk      = Request.QueryString["callback"];
            //

            if (    string.IsNullOrEmpty(uid)
                ||  string.IsNullOrEmpty(fid)
                ||  string.IsNullOrEmpty(cbk))
            {
                this.toContent(cbk + "({\"value\":0})", "application/json");
                return;
            }

            DBConfig cfg = new DBConfig();
            DnFile db = cfg.downF();
            db.process( fid, int.Parse(uid), lenLoc, per);
            
            this.toContent(cbk + "({\"value\":1})", "application/json");
        }
    }
}