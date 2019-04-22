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
    public partial class index : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string op = Request.QueryString["op"];

            if (op == "data") this.load_data();
            if (op == "rename") this.f_rename();
        }

        void f_rename() {
            var data = Request.QueryString["data"];
            var obj = JObject.Parse(data);

            SqlExec se = new SqlExec();
            se.update("up6_files", "f_nameLoc", "f_id", obj);

            PageTool.to_content(obj);
        }

        void load_data() {
            SqlExec se = new SqlExec();

            SqlWhereMerge swm = new SqlWhereMerge();
            swm.req_equal("f_pid", "pid",false);
            swm.equal("f_complete", 1);
            string where = swm.to_sql();

            //文件表
            var files = (JArray)DbBase.page2("up6_files"
                , "f_id"
                , "f_id,f_nameLoc,f_sizeLoc,f_time,f_fdTask"
                ,where
                ,"f_fdTask desc,f_time desc");

            //目录表
            var folders = (JArray)DbBase.page2("up6_folders"
                , "fd_id"
                , "fd_id,fd_name,timeUpload"
                , where
                , "timeUpload desc");

            //合并表
            foreach (var fd in folders)
            {
                files.Add(new JObject {
                    { "f_id",fd["fd_id"]}
                    ,{ "f_nameLoc",fd["fd_name"]}
                    ,{ "f_sizeLoc",""}
                    ,{ "f_time",fd["timeUpload"]}
                    ,{ "f_fdTask",true}
                });
            }

            int count = DbBase.count("up6_files", "f_id", where);

            JObject o = new JObject();
            o["count"] = count;
            o["code"] = 0;
            o["msg"] = string.Empty;
            o["data"] = files;

            PageTool.to_content(o);
        }
    }
}