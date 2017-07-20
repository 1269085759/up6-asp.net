using System;
using System.Web;
using up6.down2.biz;

namespace up6.down2.db
{
    /// <summary>
    /// 列出未完成的文件和文件夹下载任务。
    /// 格式：json
    ///     [f1,f2,f3,f4]
    /// f1为xdb_files对象
    /// </summary>
    public partial class f_list : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string uid = Request.QueryString["uid"];
            string cbk = Request.QueryString["callback"];

            if (string.IsNullOrEmpty(uid))
            {
                Response.Write(cbk+"({\"value\":null})");
                Response.End();
                return;
            }

            DnFile db = new DnFile();
            string json = db.all_uncmp(int.Parse(uid));
            if (!string.IsNullOrEmpty(json))
            {
                json = HttpUtility.UrlEncode(json);
                json = json.Replace("+", "%20");
                json = cbk + "({\"value\":\"" + json + "\"})";
            }
            else { json = cbk + "({\"value\":null})"; };

            Response.Write(json);
        }
    }
}