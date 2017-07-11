using System;
using up6.db.database;

namespace up6.db
{
    /// <summary>
    /// 更新文件或文件夹进度，百分比，
    /// </summary>
    public partial class f_process : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id       = Request.QueryString["id"];
            string uid      = Request.QueryString["uid"];
            string offset   = Request.QueryString["offset"];
            string lenSvr   = Request.QueryString["lenSvr"];
            string perSvr   = Request.QueryString["perSvr"];
            string callback = Request.QueryString["callback"];//jsonp参数

            string json = callback + "({\"state\":0})";//返回jsonp格式数据。
            if(    !string.IsNullOrEmpty(id)
                && !string.IsNullOrEmpty(lenSvr)
                && !string.IsNullOrEmpty(perSvr))
            {
                DBFile db = new DBFile();
                db.f_process(int.Parse(uid), id, long.Parse(offset), long.Parse(lenSvr), perSvr);
                json = callback + "({\"state\":1})";//返回jsonp格式数据。
            }
            Response.Write(json);
        }
    }
}