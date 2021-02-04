using System;
using up6.db.biz;
using up6.db.database;
using up6.filemgr.app;

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
            var uid = this.reqToInt("uid");
            var id = this.reqString("id");
            var pid = this.reqString("pid");
            var cbk = this.reqString("callback");
            var cover = this.reqToInt("cover");//是否覆盖
            var nameLoc = this.reqStringDecode("nameLoc");//文件名称

            //返回值。1表示成功
            int ret = 0;

            if (string.IsNullOrEmpty(id) )
            {
            }//参数不为空
            else
            {
                DBConfig cfg = new DBConfig();
                DBFile db = cfg.db();
                db.complete(id);

                //覆盖同名文件-更新同名文件状态
                if (cover==1) db.delete(pid, nameLoc, uid,id);

                up6_biz_event.file_post_complete(id);
                ret = 1;
            }
            Response.Write(cbk + "(" + ret + ")");//必须返回jsonp格式数据
        }
    }
}