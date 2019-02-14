using System;
using up6.db.biz;
using up6.db.database;

namespace up6.db
{
    /// <summary>
    /// 此文件处理单文件上传
    /// </summary>
    public partial class f_complete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string md5 = Request.QueryString["md5"];
            string uid = Request.QueryString["uid"];
            string id = Request.QueryString["id"];
            string cbk = Request.QueryString["callback"];

            //返回值。1表示成功
            int ret = 0;

            if (   string.IsNullOrEmpty(uid)
                || string.IsNullOrEmpty(id))
            {
            }//参数不为空
            else
            {
                DBFile db = new DBFile();
                db.complete(id);
                up6_biz_event.file_post_complete(id);
                ret = 1;
            }
            Response.Write(cbk + "(" + ret + ")");//必须返回jsonp格式数据
        }
    }
}