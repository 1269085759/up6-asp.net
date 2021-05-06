using System;
using System.Web;
using up6.db.database;
using up6.down2.biz;
using up6.filemgr.app;

namespace up6.down2.db
{
    /// <summary>
    /// 获取文件夹JSON数据
    /// </summary>
    public partial class fd_data : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id  = Request.QueryString["id"];
            string cbk = Request.QueryString["callback"];
            string json = "({\"value\":null})";

            if (!string.IsNullOrEmpty(id))
            {
                DBConfig cfg = new DBConfig();
                DnFolder df = cfg.downFd();
                string data = HttpUtility.UrlEncode(df.childs(id));
                data = data.Replace("+", "%20");

                json = "({\"value\":\""+ data + "\"})" ;
            }
            Response.Write(cbk + json);
        }
    }
}