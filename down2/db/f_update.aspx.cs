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
            string fid      = Request.QueryString["idSvr"];
            string uid      = Request.QueryString["uid"];
            string lenLoc   = Request.QueryString["lenLoc"];
            string per      = Request.QueryString["perLoc"];
            string cbk      = Request.QueryString["callback"];
            //
            string file_id  = Request.QueryString["file_id"];
            string file_lenLoc = Request.QueryString["file_lenLoc"];
            string file_per = Request.QueryString["file_per"];

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
            if(int.Parse(fid)>0)db.updateProcess(int.Parse(fid), int.Parse(uid), lenLoc, per);
            //更新子文件
            if (!string.IsNullOrEmpty(file_id) && !string.IsNullOrEmpty(file_lenLoc))
            {
                db.updateProcess(int.Parse(file_id), int.Parse(uid), file_lenLoc, file_per);
            }
            Response.Write(cbk + "({\"value\":1})");
        }
    }
}