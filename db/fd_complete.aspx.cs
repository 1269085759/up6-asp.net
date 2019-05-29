using System;
using up6.db.biz;
using up6.db.biz.folder;
using up6.db.database;
using up6.db.model;
using up6.filemgr.app;

namespace up6.db
{
    public partial class fd_complete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];
            string uid = Request.QueryString["uid"];
            string cak = Request.QueryString["callback"];
            int ret = 0;

            if ( string.IsNullOrEmpty(id)
                || uid.Length < 1)
            {
            }
            else
            {
                DbFolder db = new DbFolder();
                FileInf inf = db.read(id);
                string root = inf.pathSvr;
                
                //上传完毕
                DBFile.fd_complete(id, uid);

                //扫描文件夹结构，
                fd_scan sa = new fd_scan();
                sa.root = inf;//
                sa.scan(inf,root);

                //更新扫描状态
                DBFile.fd_scan(id, uid);

                up6_biz_event.folder_post_complete(id);

                ret = 1;
            }
            Response.Write(cak + "(" + ret + ")");
        }
    }
}