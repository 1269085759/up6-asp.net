using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
            if (string.IsNullOrEmpty(id))
            {
                Response.Write(cbk + json);
                return;
            }

            //
        }
    }
}