using System;
using up6.db.biz;
using up6.db.database;

namespace up6.db
{
    /// <summary>
    /// 此文件处理单文件上传
    /// </summary>
    public partial class f_complete : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var md5 = this.reqString("md5");
            var uid = this.reqString("uid");
            var id = this.reqString("id");
            var cbk = this.reqString("callback");

            //返回值。1表示成功
            int ret = 0;

            if (string.IsNullOrEmpty(id) ||
                string.IsNullOrEmpty(uid)
                )
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