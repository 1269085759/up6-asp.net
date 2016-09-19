using up6.demoSql2005.down2.biz;
using System;

using System.Web;

namespace up6.demoSql2005.down2.db
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

            //string json = DnFile.GetAll(int.Parse(uid));
            un_builder fd = new un_builder();
            string json = fd.read(uid);
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