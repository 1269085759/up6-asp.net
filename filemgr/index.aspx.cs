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
            else if (op == "search") this.search();
            else if (op == "rename") this.file_rename();
            else if (op == "del") this.file_del();
            else if (op == "del-batch") this.file_del_batch();
            else if (op == "path") this.build_path();
            else if (op == "mk-folder") this.mk_folder();
        }

        void mk_folder() {
            var data = Request.QueryString["data"];
            data = Server.UrlDecode(data);
            var obj = JObject.Parse(data);

            SqlExec se = new SqlExec();

            se.insert("up6_folders", new SqlParam[] {
            new SqlParam("f_id",Guid.NewGuid().ToString("N"))
            ,new SqlParam("f_pid",obj["f_pid"].ToString())
            ,new SqlParam("f_nameLoc",obj["f_nameLoc"].ToString())
            ,new SqlParam("f_complete",true)
            });

            PageTool.to_content(obj);
        }

        /// <summary>
        /// 生成导航路径
        /// </summary>
        void build_path() {

            var data = Request.QueryString["data"];
            data = Server.UrlDecode(data);
            var fd = JObject.Parse(data);

            PageTool.to_content(DbFolder.build_path_by_id(fd));
        }

        void file_rename() {
            var data = Request.QueryString["data"];
            data = Server.UrlDecode(data);
            var obj = JObject.Parse(data);

            SqlExec se = new SqlExec();
            se.update("up6_files", "f_nameLoc", "f_id", obj);
            //子文件夹更名
            se.update("up6_folders", "f_nameLoc", "f_id", obj);

            PageTool.to_content(obj);
        }

        void file_del() {
            var data = Request.QueryString["data"];
            data = Server.UrlDecode(data);
            var f = JObject.Parse(data);

            SqlExec se = new SqlExec();
            se.update("up6_folders"
                ,new SqlParam[] { new SqlParam("f_deleted", true)}
                , new SqlParam[] {
                    new SqlParam("f_id",f["f_id"].ToString())
                    ,new SqlParam("f_pid",f["f_id"].ToString())
                    ,new SqlParam("f_pidRoot",f["f_id"].ToString())
                }
                ,"or"
                );
            se.update("up6_files"
                ,new SqlParam[] { new SqlParam("f_deleted", true)}
                , new SqlParam[] {
                    new SqlParam("f_id",f["f_id"].ToString())
                    ,new SqlParam("f_pid",f["f_id"].ToString())
                    ,new SqlParam("f_pidRoot",f["f_id"].ToString())
                }
                ,"or"
                );

            PageTool.to_content(f);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        void file_del_batch() {
            var par = Request.QueryString["data"];
            par = Server.UrlDecode(par);
            var obj = JToken.Parse(par);

            SqlExec se = new SqlExec();

            //更新文件
            se.exec_batch("up6_files"
                , "update up6_files set f_deleted=1 where f_id=@f_id"
                , string.Empty
                , "f_id"
                , obj);

            //更新文件夹
            se.exec_batch("up6_folders"
                , "update up6_folders set f_deleted=1 where f_id=@f_id"
                , string.Empty
                , "f_id"
                , obj);

            PageTool.to_content(obj);
        }

        void load_data() {
            SqlExec se = new SqlExec();

            SqlWhereMerge swm = new SqlWhereMerge();
            swm.req_equal("f_pid", "pid",false);
            swm.equal("f_complete", 1);
            swm.equal("f_deleted", 0);
            swm.equal("f_fdTask", 0);
            swm.req_like("f_nameLoc", "key");
            string where = swm.to_sql();

            //文件表
            var files = (JArray)DbBase.page2("up6_files"
                , "f_id"
                , "f_id,f_pid,f_nameLoc,f_sizeLoc,f_lenLoc,f_time,f_pidRoot,f_fdTask,f_pathSvr,f_pathRel"
                , where
                ,"f_fdTask desc,f_time desc");

            //搜索时过滤f_pid
            if (!string.IsNullOrEmpty(Request.QueryString["key"]))
            {
                swm.del("f_pid");
                where = swm.to_sql();
            }

            //
            swm.equal("f_fdTask",1);
            where = swm.to_sql();
            var folders = (JArray)DbBase.page2("up6_folders"
                , "f_id"
                , "f_id,f_nameLoc,f_pid,f_pidRoot,f_time,f_pathRel"
                , where
                , "f_time desc");

            //合并表
            foreach (var fd in folders)
            {
                files.Insert(0,new JObject {
                    { "f_id",fd["f_id"]}
                    ,{ "f_pid",fd["f_pid"]}
                    ,{ "f_nameLoc",fd["f_nameLoc"]}
                    ,{ "f_sizeLoc",""}
                    ,{ "f_lenLoc",0}
                    ,{ "f_pathSvr",""}
                    ,{ "f_pathRel",fd["f_pathRel"]}
                    ,{ "f_time",fd["f_time"]}
                    ,{ "f_pidRoot",fd["f_pidRoot"]}
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

        void search()
        {
            SqlSearchComb ssc = new SqlSearchComb();
            ssc.parse();

            SqlWhereMerge swm = new SqlWhereMerge();
            swm.req_equal("f_pid", "pid", false);
            swm.equal("f_complete", 1);
            //swm.equal("f_deleted", 0);
            //swm.req_like("f_nameLoc", "key");

            string where = ssc.union(swm);

            //文件表
            var files = (JArray)DbBase.page2("up6_files"
                , "f_id"
                , "f_id,f_pid,f_nameLoc,f_sizeLoc,f_time,f_pidRoot,f_fdTask"
                , where
                , "f_fdTask desc,f_time desc");

            //搜索时过滤f_pid
            if (!string.IsNullOrEmpty(Request.QueryString["key"]))
            {
                swm.del("f_pid");
                where = swm.to_sql();
            }
            var folders = (JArray)DbBase.page2("up6_folders"
                , "f_id"
                , "f_id,f_nameLoc,f_pid,f_pidRoot,f_time"
                , where
                , "f_time desc");

            //合并表
            foreach (var fd in folders)
            {
                files.Insert(0, new JObject {
                    { "f_id",fd["f_id"]}
                    ,{ "f_pid",fd["f_pid"]}
                    ,{ "f_nameLoc",fd["f_nameLoc"]}
                    ,{ "f_sizeLoc",""}
                    ,{ "f_time",fd["f_time"]}
                    ,{ "f_pidRoot",fd["f_pidRoot"]}
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