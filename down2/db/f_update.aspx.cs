using System;

namespace up6.down2.db
{
    /// <summary>
    /// 更新文件下载进度
    /// </summary>
    public partial class f_update : System.Web.UI.Page
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
                ||  string.IsNullOrEmpty(cbk)
                ||  string.IsNullOrEmpty(lenLoc))
            {
                Response.Write(cbk+"({\"value\":0})");
                Response.End();
                return;
            }

            DnFile db = new DnFile();
            db.process( fid, int.Parse(uid), lenLoc, per);
            
            Response.Write(cbk + "({\"value\":1})");
        }
    }
}