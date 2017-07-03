using System;

namespace up6.demoSql2005.db
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
            string guid = Request.QueryString["guid"];
            string guidFD= Request.QueryString["fd-guid"];
            string cbk = Request.QueryString["callback"];

            //返回值。1表示成功
            int ret = 0;

            if (string.IsNullOrEmpty(md5)
                || string.IsNullOrEmpty(uid)
                || string.IsNullOrEmpty(guid))
            {
            }//参数不为空
            else
            {
                DBFile db = new DBFile();
                db.UploadComplete(md5);
                ret = 1;
            }

            //更新文件夹已上传文件数
            if (!string.IsNullOrEmpty(guidFD))
            {
                DBFolder.child_complete(guidFD);
            }
            Response.Write(cbk + "(" + ret + ")");//必须返回jsonp格式数据
        }
    }
}