using System;
using up6.db.database;

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
                DBFile db = new DBFile();
                DBFolder.Remove(fid, int.Parse(uid));
                ret = 1;
            }
            Response.Write(cbk + "({\"value\":" + ret + "})");//返回jsonp格式数据
        }
    }
}