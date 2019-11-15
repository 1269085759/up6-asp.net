﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Web;
using up6.db.biz;
using up6.db.database;
using up6.db.model;
using up6.db.utils;
using up6.down2.biz;
using up6.filemgr.app;

namespace up6.filemgr
{
    public partial class vue : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string op = Request.QueryString["op"];

            if (op == "data") this.load_data(false);
            else if (op == "search") this.search();
            else if (op == "rename") this.file_rename();
            else if (op == "del") this.file_del();
            else if (op == "del-batch") this.file_del_batch();
            else if (op == "path") this.build_path();
            else if (op == "mk-folder") this.mk_folder();
            else if (op == "uncomp") this.load_uncomplete();
            else if (op == "uncmp-down") this.load_uncmp_down();
            else if (op == "tree") this.load_tree();
            else if (op == "f_create") this.f_create();
            else if (op == "fd_create") this.fd_create();
            else if (op == "fd_data") this.fd_data();
            else this.load_data(true);
        }


        void fd_create()
        {
            string id = Request.QueryString["id"];
            string pid = Request.QueryString["pid"];
            string uid = Request.QueryString["uid"];
            string lenLoc = Request.QueryString["lenLoc"];
            string sizeLoc = Request.QueryString["sizeLoc"];
            string pathLoc = HttpUtility.UrlDecode(Request.QueryString["pathLoc"]);
            string pathRel = this.reqToString("pathRel");
            string callback = Request.QueryString["callback"];//jsonp参数
            if (string.IsNullOrEmpty(pid)) pid = string.Empty;
            pid = pid.Trim();

            if (string.IsNullOrEmpty(id)
                || string.IsNullOrEmpty(uid)
                || string.IsNullOrEmpty(pathLoc)
                )
            {
                Response.Write(callback + "({\"value\":null})");
                return;
            }

            FileInf fileSvr = new FileInf();
            fileSvr.id = id;
            fileSvr.pid = pid;
            fileSvr.pidRoot = "";
            fileSvr.fdChild = false;
            fileSvr.fdTask = true;
            fileSvr.uid = int.Parse(uid);//将当前文件UID设置为当前用户UID
            fileSvr.nameLoc = Path.GetFileName(pathLoc);
            fileSvr.pathLoc = pathLoc;
            fileSvr.pathRel = PathTool.combin(pathRel, fileSvr.nameLoc);
            fileSvr.lenLoc = Convert.ToInt64(lenLoc);
            fileSvr.sizeLoc = sizeLoc;
            fileSvr.deleted = false;
            fileSvr.nameSvr = fileSvr.nameLoc;

            //检查同名目录
            DbFolder df = new DbFolder();
            if (df.exist_same_folder(fileSvr.nameLoc, pid))
            {
                var o = new JObject { { "value", null }, { "ret", false }, { "code", "102" } };
                var js = callback + string.Format("({0})", JsonConvert.SerializeObject(o));
                this.toContent(js);
                return;
            }

            //生成存储路径
            PathBuilderUuid pb = new PathBuilderUuid();
            fileSvr.pathSvr = pb.genFolder(ref fileSvr);
            fileSvr.pathSvr = fileSvr.pathSvr.Replace("\\", "/");
            if (!Directory.Exists(fileSvr.pathSvr)) Directory.CreateDirectory(fileSvr.pathSvr);

            //添加成根目录
            if (string.IsNullOrEmpty(pid))
            {
                DBFile db = new DBFile();
                db.Add(ref fileSvr);
            }//添加成子目录
            else
            {
                SqlExec se = new SqlExec();
                se.insert("up6_folders", new SqlParam[] {
                     new SqlParam("f_id",fileSvr.id)
                    ,new SqlParam("f_nameLoc",fileSvr.nameLoc)
                    ,new SqlParam("f_pid",fileSvr.pid)
                    ,new SqlParam("f_pidRoot","")
                    ,new SqlParam("f_lenLoc",fileSvr.lenLoc)
                    ,new SqlParam("f_sizeLoc",fileSvr.sizeLoc)
                    ,new SqlParam("f_pathLoc",fileSvr.pathLoc)
                    ,new SqlParam("f_pathSvr",fileSvr.pathSvr)
                    ,new SqlParam("f_pathRel",fileSvr.pathRel)
                    ,new SqlParam("f_uid",fileSvr.uid)
                });
            }

            up6_biz_event.folder_create(fileSvr);

            string json = JsonConvert.SerializeObject(fileSvr);
            json = HttpUtility.UrlEncode(json);
            json = json.Replace("+", "%20");
            var jo = new JObject { { "value", json }, { "ret", true } };
            json = callback + string.Format("({0})", JsonConvert.SerializeObject(jo));
            this.toContent(json);
        }

        /// <summary>
        /// 获取文件夹结构（JSON）
        /// 格式：
        /// [
        ///   {nameLoc,pathSvr,pathRel,lenSvr,sizeSvr}
        ///   {nameLoc,pathSvr,pathRel,lenSvr,sizeSvr}
        /// ]
        /// </summary>
        void fd_data() {

            string id = Request.QueryString["id"];
            string cbk = Request.QueryString["callback"];
            string json = "({\"value\":null})";

            if (!string.IsNullOrEmpty(id))
            {
                FolderBuilder fb = new FolderBuilder();
                var data = JsonConvert.SerializeObject(fb.build(id));
                data = this.Server.UrlEncode(data);
                data = data.Replace("+", "%20");

                json = "({\"value\":\"" + data + "\"})";
            }
            this.toContent(cbk + json);
        }

        void f_create()
        {

            string pid = Request.QueryString["pid"];
            string pidRoot = Request.QueryString["pidRoot"];
            string md5 = Request.QueryString["md5"];
            string id = Request.QueryString["id"];
            string uid = Request.QueryString["uid"];
            string lenLoc = Request.QueryString["lenLoc"];
            string sizeLoc = Request.QueryString["sizeLoc"];
            string callback = Request.QueryString["callback"];//jsonp参数
            //客户端使用的是encodeURIComponent编码，
            string pathLoc = HttpUtility.UrlDecode(Request.QueryString["pathLoc"]);//utf-8解码
            string pathRel = this.reqToString("pathRel");

            if (string.IsNullOrEmpty(pid)) pid = string.Empty;
            if (string.IsNullOrEmpty(pidRoot)) pidRoot = pid;

            //参数为空
            if (string.IsNullOrEmpty(md5)
                || string.IsNullOrEmpty(uid)
                || string.IsNullOrEmpty(sizeLoc))
            {
                Response.Write(callback + "({\"value\":null})");
                return;
            }

            FileInf fileSvr = new FileInf();
            fileSvr.fdChild = false;
            fileSvr.uid = int.Parse(uid);//将当前文件UID设置为当前用户UID
            fileSvr.id = id;
            fileSvr.pid = pid;
            fileSvr.fdChild = !string.IsNullOrEmpty(pid);
            fileSvr.pidRoot = pidRoot;
            fileSvr.nameLoc = Path.GetFileName(pathLoc);
            fileSvr.pathLoc = pathLoc;
            fileSvr.pathRel = PathTool.combin(pathRel, fileSvr.nameLoc);
            fileSvr.lenLoc = Convert.ToInt64(lenLoc);
            fileSvr.sizeLoc = sizeLoc;
            fileSvr.deleted = false;
            fileSvr.md5 = md5;
            fileSvr.nameSvr = fileSvr.nameLoc;

            //同名文件检测
            DbFolder df = new DbFolder();
            if (df.exist_same_file(fileSvr.nameLoc, pid))
            {
                var data = callback + "({'value':'','ret':false,'code':'101'})";
                this.toContent(data);
                return;
            }

            //所有单个文件均以uuid/file方式存储
            PathBuilderUuid pb = new PathBuilderUuid();
            fileSvr.pathSvr = pb.genFile(fileSvr.uid, ref fileSvr);
            fileSvr.pathSvr = fileSvr.pathSvr.Replace("\\", "/");

            //数据库存在相同文件
            DBFile db = new DBFile();
            FileInf fileExist = new FileInf();
            if (db.exist_file(md5, ref fileExist))
            {
                fileSvr.nameSvr = fileExist.nameSvr;
                fileSvr.pathSvr = fileExist.pathSvr;
                fileSvr.perSvr = fileExist.perSvr;
                fileSvr.lenSvr = fileExist.lenSvr;
                fileSvr.complete = fileExist.complete;
                db.Add(ref fileSvr);

                //触发事件
                up6_biz_event.file_create_same(fileSvr);
            }//数据库不存在相同文件
            else
            {
                db.Add(ref fileSvr);
                //触发事件
                up6_biz_event.file_create(fileSvr);

                //2.0创建器。仅创建一个空白文件
                FileBlockWriter fr = new FileBlockWriter();
                fr.make(fileSvr.pathSvr, fileSvr.lenLoc);
            }
            string jv = JsonConvert.SerializeObject(fileSvr);
            jv = HttpUtility.UrlEncode(jv);
            jv = jv.Replace("+", "%20");
            string json = callback + "({\"value\":\"" + jv + "\",\"ret\":true})";//返回jsonp格式数据。
            this.toContent(json);
        }

        void load_tree()
        {
            var pid = Request.QueryString["pid"];
            var swm = new SqlWhereMerge();
            swm.equal("f_fdChild", 0);
            swm.equal("f_fdTask", 1);
            swm.equal("f_deleted", 0);
            if (!string.IsNullOrEmpty(pid)) swm.equal("f_pid", pid);

            SqlExec se = new SqlExec();
            JArray arr = new JArray();
            var data = se.select("up6_files"
                , "f_id,f_pid,f_pidRoot,f_nameLoc"
                , swm.to_sql()
                , string.Empty);

            //查子目录
            if (!string.IsNullOrEmpty(pid))
            {
                data = se.select("up6_folders"
                    , "f_id,f_pid,f_pidRoot,f_nameLoc"
                    , new SqlParam[] {
                        new SqlParam("f_pid", pid)
                        ,new SqlParam("f_deleted", false)
                    });
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
                , "select f_id ,f_nameLoc ,f_pathLoc ,f_sizeLoc ,f_lenSvr ,f_perSvr ,f_fdTask ,f_md5 from up6_files where f_complete=0 and f_deleted=0"
                , "f_id,f_nameLoc,f_pathLoc,f_sizeLoc,f_lenSvr,f_perSvr,f_fdTask,f_md5"
                , "id,nameLoc,pathLoc,sizeLoc,lenSvr,perSvr,fdTask,md5");
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
            var name = obj["f_nameLoc"].ToString().Trim();
            var pid = obj["f_pid"].ToString().Trim();
            var pidRoot = obj["f_pidRoot"].ToString().Trim();
            obj["f_nameLoc"] = name;
            obj["f_pid"] = pid;
            obj["f_pidRoot"] = pidRoot;
            obj["f_pathRel"] = PathTool.combin(obj["f_pathRel"].ToString(), name);

            DbFolder df = new DbFolder();
            if (df.exist_same_folder(name, pid))
            {
                var ret = new JObject { { "ret", false }, { "msg", "已存在同名目录" } };
                this.toContent(ret);
                return;
            }

            SqlExec se = new SqlExec();

            //根目录
            if (string.IsNullOrEmpty(pid))
            {
                obj["f_id"] = Guid.NewGuid().ToString("N");

                se.insert("up6_files", new SqlParam[] {
                    new SqlParam("f_id",obj["f_id"].ToString())
                    ,new SqlParam("f_pid",obj["f_pid"].ToString())
                    ,new SqlParam("f_pidRoot",obj["f_pidRoot"].ToString())
                    ,new SqlParam("f_nameLoc",obj["f_nameLoc"].ToString())
                    ,new SqlParam("f_complete",true)
                    ,new SqlParam("f_fdTask",true)
                    ,new SqlParam("f_pathRel",obj["f_pathRel"].ToString())
                });
            }//子目录
            else
            {
                obj["f_id"] = Guid.NewGuid().ToString("N");
                se.insert("up6_folders"
                    , new SqlParam[] {
                    new SqlParam("f_id",obj["f_id"].ToString())
                    ,new SqlParam("f_pid",obj["f_pid"].ToString())
                    ,new SqlParam("f_pidRoot",obj["f_pidRoot"].ToString())
                    ,new SqlParam("f_nameLoc",obj["f_nameLoc"].ToString())
                    ,new SqlParam("f_complete",true)
                    ,new SqlParam("f_pathRel",obj["f_pathRel"].ToString())
                    });
            }

            obj["ret"] = true;
            this.toContent(obj);
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

            this.toContent(df.build_path(fd));
        }

        void file_rename()
        {
            var data = Request.QueryString["data"];
            data = Server.UrlDecode(data);
            var o = JObject.Parse(data);

            var db = new DbFolder();
            var fdTask = Convert.ToBoolean(o["f_fdTask"].ToString());
            var pid = o["f_pid"].ToString().Trim();
            var id = o["f_id"].ToString().Trim();
            var nameNew = o["f_nameLoc"].ToString().Trim();

            bool exist = false;
            if (!fdTask || string.IsNullOrEmpty(pid)) exist = db.rename_file_check(nameNew, pid);
            else exist = db.rename_folder_check(nameNew, pid);

            //存在同名项
            if (exist)
            {
                var res = new JObject { { "state", false }, { "msg", "存在同名项" } };
                this.toContent(res);
                return;
            }

            //是文件或根目录
            if (!fdTask || string.IsNullOrEmpty(pid)) db.rename_file(nameNew, id);
            else db.rename_folder(nameNew, id, pid);

            var ret = new JObject { { "state", true } };
            this.toContent(ret);
            return;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
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

        /// <summary>
        /// 获取目录和文件列表
        /// </summary>
        /// <param name="toParam">注册到变量？</param>
        void load_data(bool toParam)
        {
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
                    , "f_id,f_nameLoc,f_pid,f_sizeLoc,f_time,f_pidRoot,f_pathRel"
                    , where
                    , "f_time desc");

                foreach (var fd in folders)
                {
                    fd["f_fdTask"] = true;
                    fd["f_fdChild"] = false;
                    fd["f_pathSvr"] = string.Empty;
                }
            }
            foreach (var f in files) folders.Add(f);

            int count = DbBase.count("up6_files", "f_id", where);

            JObject o = new JObject();
            o["count"] = count;
            o["code"] = 0;
            o["msg"] = string.Empty;
            o["data"] = folders;

            if (toParam) this.param.Add("items",o);
            else this.toContent(o);
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