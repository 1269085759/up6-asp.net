using System;

namespace up6.db
{
    public partial class fd_complete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];
            string uid = Request.QueryString["uid"];
            string cak = Request.QueryString["callback"];
            int ret = 0;

            if ( string.IsNullOrEmpty(id)
                || uid.Length < 1)
            {
            }
            else
            {
                DBFile.fd_complete(id,uid);
                ret = 1;
            }
            Response.Write(cak + "(" + ret + ")");
        }
    }
}