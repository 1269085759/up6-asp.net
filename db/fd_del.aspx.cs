using System;
using up6.db.database;
using up6.filemgr.app;

namespace up6.db
{
    public partial class fd_del : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string fid = this.reqString("id");
            string uid = this.reqString("uid");
            string cbk = this.reqString("callback");
            int ret = 0;

            if (string.IsNullOrEmpty(fid) || 
                string.IsNullOrEmpty(uid)
                )
            {
            }//参数不为空
            else
            {
                DBConfig cfg = new DBConfig();
                cfg.folder().Remove(fid, int.Parse(uid));
                ret = 1;
            }
            this.toContentJson(cbk + "({\"value\":" + ret + "})");//返回jsonp格式数据
        }
    }
}