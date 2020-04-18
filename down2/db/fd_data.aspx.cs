using System;
using System.Web;
using up6.db.database;
using up6.down2.biz;

namespace up6.down2.db
{
    /// <summary>
    /// 获取文件夹JSON数据
    /// </summary>
    public partial class fd_data : System.Web.UI.Page
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
                string data = HttpUtility.UrlEncode(df.files(id));
                data = data.Replace("+", "%20");

                json = "({\"value\":\""+ data + "\"})" ;
            }
            Response.Write(cbk + json);
        }
    }
}