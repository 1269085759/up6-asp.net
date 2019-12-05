using System;
using up6.db.biz;
using up6.db.database;
using up6.filemgr.app;

namespace up6.db
{
    /// <summary>
    /// 更新文件或文件夹进度，百分比，
    /// </summary>
    public partial class f_process : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id       = this.reqString("id");
            string uid      = this.reqString("uid");
            string offset   = this.reqString("offset");
            string lenSvr   = this.reqString("lenSvr");
            string perSvr   = this.reqString("perSvr");
            string callback = this.reqString("callback");//jsonp参数

            string json = callback + "({\"state\":0})";//返回jsonp格式数据。
            if(    !string.IsNullOrEmpty(id)
                && !string.IsNullOrEmpty(lenSvr)
                && !string.IsNullOrEmpty(perSvr))
            {
                DBFile db = new DBFile();
                db.f_process(int.Parse(uid), id, long.Parse(offset), long.Parse(lenSvr), perSvr);
                up6_biz_event.file_post_process(id);
                json = callback + "({\"state\":1})";//返回jsonp格式数据。
            }
            Response.Write(json);
        }
    }
}