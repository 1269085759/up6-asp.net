using System;
using up6.db.database;
using up6.down2.biz;

namespace up6.down2.db
{
    public partial class f_del : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string fid = Request.QueryString["id"];
            string uid = Request.QueryString["uid"];
            string cbk = Request.QueryString["callback"];

            if (string.IsNullOrEmpty(uid)
                || string.IsNullOrEmpty(fid))
            {
                Response.Write(cbk + "({\"value\":null})");
                return;
            }

            DBConfig cfg = new DBConfig();
            DnFile db = cfg.dn();
            db.Delete(fid, int.Parse(uid));

            Response.Write(cbk + "({\"value\":1})");
        }
    }
}