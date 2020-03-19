using System;
using up6.db.biz;
using up6.db.biz.folder;
using up6.db.database;
using up6.db.model;
using up6.filemgr.app;

namespace up6.db
{
    public partial class fd_complete : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id = this.reqString("id");
            string uid = this.reqString("uid");
            string cak = this.reqString("callback");
            int cover = this.reqToInt("cover");
            int ret = 0;

            if ( string.IsNullOrEmpty(id) || 
                uid.Length < 1)
            {
            }
            else
            {
                DbFolder db = new DbFolder();
                FileInf folder = db.read(id);
                FileInf fdExist = db.read(folder.pathRel, folder.pid,id);                
                if(1==cover && fdExist !=null)
                {
                    folder.id = fdExist.id;
                    folder.pid = fdExist.pid;
                    folder.pidRoot = fdExist.pidRoot;
                }

                //根节点
                FileInf root = new FileInf();
                root.id = folder.pidRoot;
                //当前节点是根节点
                if (string.IsNullOrEmpty(root.id)) root.id = folder.id;
                
                //上传完毕
                DBFile.fd_complete(id, uid);

                //扫描文件夹结构，
                fd_scan sa = new fd_scan();
                sa.root = root;//

                //清理同名子文件
                if (1 == cover && fdExist!=null)
                {
                    //覆盖同名子文件
                    sa.cover(folder, folder.pathSvr);
                }

                //添加文件记录
                sa.scan(folder,folder.pathSvr);

                //更新扫描状态
                DBFile.fd_scan(id, uid);

                up6_biz_event.folder_post_complete(id);

                //删除当前目录
                if (1 == cover && fdExist != null)
                {
                    DBFolder.del(id, int.Parse(uid));
                }

                ret = 1;
            }
            Response.Write(cak + "(" + ret + ")");
        }
    }
}