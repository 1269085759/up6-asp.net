using System;
using up6.db.database;
using up6.down2.biz;
using up6.filemgr.app;

namespace up6.down2.db
{
    public partial class f_del : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string fid = Request.QueryString["id"];
            string uid = Request.QueryString["uid"];
            string cbk = Request.QueryString["callback"];

            if (string.IsNullOrEmpty(uid)
                || string.IsNullOrEmpty(fid))
            {
                this.toContentJson(cbk + "({\"value\":null})");
                return;
            }

            DBConfig cfg = new DBConfig();
            DnFile db = cfg.downF();
            db.Delete(fid, int.Parse(uid));

            this.toContentJson(cbk + "({\"value\":1})");
        }
    }
}