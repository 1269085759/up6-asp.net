using System;
using up6.db.biz;
using up6.db.database;

namespace up6.db
{
    public partial class f_del : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string fid = Request.QueryString["id"];
            string uid = Request.QueryString["uid"];
            string callback = Request.QueryString["callback"];
            int ret = 0;

            if (string.IsNullOrEmpty(fid)
                || string.IsNullOrEmpty(uid))
            {
            }//参数不为空
            else
            {
                DBFile db = new DBFile();
                db.Delete(Convert.ToInt32(uid), fid);
                up6_biz_event.file_del(fid,Convert.ToInt32(uid));
                ret = 1;
            }
            Response.Write(callback + "(" + ret + ")");//返回jsonp格式数据
        }
    }
}