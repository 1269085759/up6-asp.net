using Newtonsoft.Json.Linq;
using System;
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
            else if (op == "uncomp") this.load_uncomplete();
            else if (op == "uncmp-down") this.load_uncmp_down();
            else if (op == "tree") this.load_tree();
        }

        void load_tree() {
            var pid = Request.QueryString["pid"];
            var swm = new SqlWhereMerge();
            swm.equal("f_fdChild", 0);
            swm.equal("f_fdTask", 1);
            if (!string.IsNullOrEmpty(pid)) swm.equal("f_pid", pid);

            SqlExec se = new SqlExec();
            JArray arr = new JArray();
            var data = se.select("up6_files"
                , "f_id,f_pid,f_pidRoot,f_nameLoc"
                , swm.to_sql()
                ,string.Empty);

            //查子目录
            if (!string.IsNullOrEmpty(pid))
            {
                data = se.select("up6_folders", "f_id,f_pid,f_pidRoot,f_nameLoc", new SqlParam[] { new SqlParam("f_pid", pid) });
            }

            foreach (var f in data)
            {
                var item = new JObject();
                item["id"] = f["f_id"].ToString();
                item["text"] = f["f_nameLoc"].ToString();
                item["parent"] = "#";
                item["nodeSvr"] = f;
                arr.Add(item);
            }
            this.toContent(arr);
        }

        /// <summary>
        /// 加载未完成的文件和目录列表
        /// </summary>
        void load_uncomplete()
        {
            SqlExec se = new SqlExec();
            var files = se.exec("up6_files"
                , "select f_id as id,f_nameLoc as nameLoc,f_pathLoc as pathLoc,f_sizeLoc as sizeLoc,f_lenSvr as lenSvr,f_perSvr as perSvr,f_fdTask as fdTask from up6_files where f_complete=0 and f_fdChild=0 and f_deleted=0"
                , "f_id,f_nameLoc,f_pathLoc,f_sizeLoc,f_lenSvr,f_perSvr,f_fdTask"
                , "id,nameLoc,pathLoc,sizeLoc,lenSvr,perSvr,fdTask");
            PageTool.to_content(files);
        }

        void load_uncmp_down()
        {
            string uid = Request.QueryString["uid"];
            SqlExec se = new SqlExec();
            var files = se.select("down_files"
                , "f_id,f_nameLoc,f_pathLoc,f_perLoc,f_sizeSvr,f_fdTask"
                , new SqlParam[] { new SqlParam("f_uid", int.Parse(uid)) });

            PageTool.to_content(files);
        }

        void mk_folder()
        {
            var data = Request.QueryString["data"];
            data = Server.UrlDecode(data);
            var obj = JObject.Parse(data);
            var pidRoot = obj["f_pidRoot"].ToString().Trim();

            SqlExec se = new SqlExec();

            //根目录
            if (string.IsNullOrEmpty(pidRoot))
            {
                se.insert("up6_files", new SqlParam[] {
            new SqlParam("f_id",Guid.NewGuid().ToString("N"))
            ,new SqlParam("f_pid",obj["f_pid"].ToString())
            ,new SqlParam("f_pidRoot",obj["f_pidRoot"].ToString())
            ,new SqlParam("f_nameLoc",obj["f_nameLoc"].ToString())
            ,new SqlParam("f_complete",true)
            ,new SqlParam("f_fdTask",true)
            });
            }//子目录
            else
            {
                se.insert("up6_folders", new SqlParam[] {
            new SqlParam("f_id",Guid.NewGuid().ToString("N"))
            ,new SqlParam("f_pid",obj["f_pid"].ToString())
            ,new SqlParam("f_pidRoot",obj["f_pidRoot"].ToString())
            ,new SqlParam("f_nameLoc",obj["f_nameLoc"].ToString())
            ,new SqlParam("f_complete",true)
            });
            }

            PageTool.to_content(obj);
        }

        /// <summary>
        /// 生成导航路径
        /// </summary>
        void build_path()
        {
            var data = Request.QueryString["data"];
            data = Server.UrlDecode(data);
            var fd = JObject.Parse(data);

            DbFolder df = new DbFolder();

            this.toContent(df.build_path_by_id(fd));
        }

        void file_rename()
        {
            var data = Request.QueryString["data"];
            data = Server.UrlDecode(data);
            var obj = JObject.Parse(data);

            SqlExec se = new SqlExec();
            se.update("up6_files", "f_nameLoc", "f_id", obj);
            //子文件夹更名
            se.update("up6_folders", "f_nameLoc", "f_id", obj);

            PageTool.to_content(obj);
        }

        void file_del()
        {
            var id = Request.QueryString["id"];

            SqlExec se = new SqlExec();
            //se.update("up6_folders"
            //    , new SqlParam[] { new SqlParam("f_deleted", true) }
            //    , new SqlParam[] {
            //        new SqlParam("f_id",id)
            //        ,new SqlParam("f_pid",id)
            //        ,new SqlParam("f_pidRoot",id)
            //    }
            //    , "or"
            //    );
            se.update("up6_files"
                , new SqlParam[] { new SqlParam("f_deleted", true) }
                , new SqlParam[] {
                    new SqlParam("f_id",id)
                    ,new SqlParam("f_pid",id)
                    ,new SqlParam("f_pidRoot",id)
                }
                , "or"
                );

            PageTool.to_content(new JObject { { "ret", 1 } });
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        void file_del_batch()
        {
            var par = Request.Form["data"];
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

        void load_data()
        {
            SqlExec se = new SqlExec();

            SqlWhereMerge swm = new SqlWhereMerge();
            swm.req_equal("f_pid", "pid", false);
            swm.equal("f_complete", 1);
            swm.equal("f_deleted", 0);

            var pid = Request.QueryString["pid"];
            bool isRoot = string.IsNullOrEmpty(pid);
            if (isRoot) swm.equal("f_fdChild", 0);
            else swm.equal("f_fdChild", 1);

            swm.req_like("f_nameLoc", "key");
            string where = swm.to_sql();

            //文件表
            var files = (JArray)DbBase.page2("up6_files"
                , "f_id"
                , "f_id,f_pid,f_nameLoc,f_sizeLoc,f_lenLoc,f_time,f_pidRoot,f_fdTask,f_pathSvr,f_pathRel"
                , where
                , "f_fdTask desc,f_time desc");

            //根目录不加载 up6_folders 表数据
            JArray folders = new JArray();
            if (!isRoot)
            {
                //目录表
                swm.del("f_fdChild");
                where = swm.to_sql();
                folders = (JArray)DbBase.page2("up6_folders"
                    , "f_id"
                    , "f_id,f_nameLoc,f_pid,f_sizeLoc,f_time,f_pidRoot"
                    , where
                    , "f_time desc");

                foreach (var fd in folders)
                {
                    fd["f_fdTask"] = true;
                    fd["f_fdChild"] = false;
                    fd["f_pathSvr"] = string.Empty;
                    fd["f_pathRel"] = string.Empty;
                }
            }
            foreach (var f in files) folders.Add(f);

            int count = DbBase.count("up6_files", "f_id", where);

            JObject o = new JObject();
            o["count"] = count;
            o["code"] = 0;
            o["msg"] = string.Empty;
            o["data"] = folders;

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