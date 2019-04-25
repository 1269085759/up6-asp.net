using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using up6.filemgr.app;

namespace up6.filemgr
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.re_folder_test();
        }

        void re_folder_test()
        {
            PathTool.createDirectory("F:/csharp/apps/up6/upload/2019/04/25/a1ed4bfde038437398eb749e119267d5/RecyclerView-master-master/app/build/tmp/expandedArchives/classes.jar_11pliug4uedhkwo9i99jr8jlm/android/support/v4/media/session/");
        }

        void exec_read_test()
        {
            SqlExec se = new SqlExec();
            var files = se.exec("up6_files"
                , "select f_id,f_nameLoc,f_pathLoc,f_sizeLoc,f_lenSvr,f_perSvr,f_fdTask from up6_files where f_complete=0 and f_fdChild=0"
                , "f_id,f_nameLoc,f_pathLoc,f_sizeLoc,f_lenSvr,f_perSvr,f_fdTask"
                ,"id,nameLoc,pathLoc,sizeLoc,lenSvr,perSvr,fdTask");

            PageTool.to_content(files);
        }

        void exec_batch_test()
        {
            JArray ids = new JArray {
                new JObject{ { "f_id", "38699a00ef9845aeb98687384ec3316c" } }
                ,new JObject{ { "f_id", "b87716eaa7fb4df38c120e464cb398fb" } }
            };
            SqlExec se = new SqlExec();
            se.exec_batch("up6_folders"
                , "update up6_folders set f_deleted=1 where f_id=@f_id"
                , string.Empty
                , "f_id"
                , ids);
        }

        void un_cmp_test()
        {

            SqlExec se = new SqlExec();
            var files = se.select("up6_files", "f_id,f_nameLoc,f_pathLoc,f_sizeLoc",
                new SqlParam[] {
                    new SqlParam("f_fdChild",0)
                    ,new SqlParam("f_complete",false)
                });
            PageTool.to_content(files);
        }
    }
}