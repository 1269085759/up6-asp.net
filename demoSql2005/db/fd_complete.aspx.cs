using System;

namespace up6.demoSql2005.db
{
    public partial class fd_complete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id_file = Request.QueryString["id_file"];
            string id_fd = Request.QueryString["id_folder"];
            string uid = Request.QueryString["uid"];
            string cak = Request.QueryString["callback"];
            int ret = 0;

            if ( string.IsNullOrEmpty(id_fd)
                || uid.Length < 1)
            {
            }
            else
            {
                DBFile.fd_complete(id_file,id_fd,uid);
                ret = 1;
            }
            Response.Write(cak + "(" + ret + ")");
        }
    }
}