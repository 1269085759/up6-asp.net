using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Web;
using up6.db.biz;
using up6.db.biz.folder;
using up6.db.database;
using up6.db.model;
using up6.db.utils;

namespace up6.filemgr.app
{
    public partial class up6_svr : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var op = Request.QueryString["op"];
            if (op == "init") this.file_init();
            else if (op == "post") this.file_post();
            else if (op == "proc") this.file_process();
            else if (op == "cmp") this.file_complete();
            else if (op == "fd-init") this.folder_init();
            else if (op == "fd-comp") this.folder_complete();
        }

        void file_init()
        {
            string md5 = Request.QueryString["md5"];
            string id = Request.QueryString["id"];
            string pid = Request.QueryString["pid"];
            string pidRoot = Request.QueryString["pidRoot"];
            string uid = Request.QueryString["uid"];
            string lenLoc = Request.QueryString["lenLoc"];
            string sizeLoc = Request.QueryString["sizeLoc"];
            string callback = Request.QueryString["callback"];//jsonp参数
            //客户端使用的是encodeURIComponent编码，
            string pathLoc = HttpUtility.UrlDecode(Request.QueryString["pathLoc"]);//utf-8解码

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
            fileSvr.pidRoot = pidRoot;
            fileSvr.nameLoc = Path.GetFileName(pathLoc);
            fileSvr.pathLoc = pathLoc;
            fileSvr.lenLoc = Convert.ToInt64(lenLoc);
            fileSvr.sizeLoc = sizeLoc;
            fileSvr.deleted = false;
            fileSvr.md5 = md5;
            fileSvr.nameSvr = fileSvr.nameLoc;

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
            string json = callback + "({\"value\":\"" + jv + "\"})";//返回jsonp格式数据。
            Response.Write(json);
        }

        void file_post()
        {
            string uid = Request.Headers["uid"];
            string f_id = Request.Headers["id"];
            string lenSvr = Request.Headers["lenSvr"];//已传大小
            string lenLoc = Request.Headers["lenLoc"];//本地文件大小
            string blockOffset = Request.Headers["blockOffset"];
            string blockSize = Request.Headers["blockSize"];//当前块大小
            string blockIndex = Request.Headers["blockIndex"];//当前块索引，基于1
            string blockMd5 = Request.Headers["blockMd5"];//块MD5
            string complete = Request.Headers["complete"];//true/false
            string pathSvr = Request.Headers["pathSvr"];//add(2015-03-19):
            pathSvr = HttpUtility.UrlDecode(pathSvr);

            if ( this.head_val_null_empty("lenLoc, uid, id, blockOffset, pathSvr")) return;

            if (Request.Files.Count < 1)
            {
                PageTool.to_content("file is empty");

                return;
            }

            bool verify = false;
            string msg = string.Empty;
            string md5Svr = string.Empty;
            HttpPostedFile file = Request.Files.Get(0);//文件块

            //计算文件块MD5
            if (!string.IsNullOrEmpty(blockMd5))
            {
                md5Svr = Md5Tool.calc(file.InputStream);
            }

            //文件块大小验证
            verify = int.Parse(blockSize) == file.InputStream.Length;
            if (!verify)
            {
                msg = "block size error sizeSvr:" + file.InputStream.Length + " sizeLoc:" + blockSize;
            }

            //块MD5验证
            if (verify && !string.IsNullOrEmpty(blockMd5))
            {
                verify = md5Svr == blockMd5;
                if (!verify) msg = "block md5 error";
            }

            if (verify)
            {
                //2.0保存文件块数据
                FileBlockWriter res = new FileBlockWriter();
                res.make(pathSvr, Convert.ToInt64(lenLoc));
                res.write(pathSvr, Convert.ToInt64(blockOffset), ref file);
                up6_biz_event.file_post_block(f_id, Convert.ToInt32(blockIndex));

                //生成信息
                JObject o = new JObject();
                o["msg"] = "ok";
                o["md5"] = md5Svr;//文件块MD5
                o["offset"] = blockOffset;//偏移
                msg = JsonConvert.SerializeObject(o);
            }
            PageTool.to_content(msg);
        }
        void file_process()
        {
            var obj = this.req_to_json();
            string callback = Request.QueryString["callback"];//jsonp参数

            string json = callback + "({\"state\":0})";//返回jsonp格式数据。
            if (!string.IsNullOrEmpty(obj["id"].ToString())
                && !string.IsNullOrEmpty(obj["lenSvr"].ToString())
                && !string.IsNullOrEmpty(obj["perSvr"].ToString()))
            {
                SqlExec se = new SqlExec();
                se.update("up6_files", "f_pos,f_lenSvr,f_perSvr", "f_id", obj);
                up6_biz_event.file_post_process(obj["id"].ToString());
                json = callback + "({\"state\":1})";//返回jsonp格式数据。
            }
            PageTool.to_content(json);
        }
        void file_complete()
        {
            string md5 = Request.QueryString["md5"];
            string uid = Request.QueryString["uid"];
            string id = Request.QueryString["id"];
            string cbk = Request.QueryString["callback"];

            //返回值。1表示成功
            int ret = 0;

            if (string.IsNullOrEmpty(uid)
                || string.IsNullOrEmpty(id))
            {
            }//参数不为空
            else
            {
                DBFile db = new DBFile();
                db.complete(id);
                up6_biz_event.file_post_complete(id);
                ret = 1;
            }
            PageTool.to_content(cbk + "(" + ret + ")");//必须返回jsonp格式数据
        }

        void folder_init()
        {
            string id = Request.QueryString["id"];
            string pid = Request.QueryString["pid"];
            string pidRoot = Request.QueryString["pidRoot"];
            string uid = Request.QueryString["uid"];
            string lenLoc = Request.QueryString["lenLoc"];
            string sizeLoc = Request.QueryString["sizeLoc"];
            string pathLoc = HttpUtility.UrlDecode(Request.QueryString["pathLoc"]);
            string callback = Request.QueryString["callback"];//jsonp参数

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
            fileSvr.pidRoot = pidRoot;
            fileSvr.fdChild = false;
            fileSvr.fdTask = true;
            fileSvr.uid = int.Parse(uid);//将当前文件UID设置为当前用户UID
            fileSvr.nameLoc = Path.GetFileName(pathLoc);
            fileSvr.pathLoc = pathLoc;
            fileSvr.lenLoc = Convert.ToInt64(lenLoc);
            fileSvr.sizeLoc = sizeLoc;
            fileSvr.deleted = false;
            fileSvr.nameSvr = fileSvr.nameLoc;

            //生成存储路径
            PathBuilderUuid pb = new PathBuilderUuid();
            fileSvr.pathSvr = pb.genFolder(ref fileSvr);
            fileSvr.pathSvr = fileSvr.pathSvr.Replace("\\", "/");
            if (!Directory.Exists(fileSvr.pathSvr)) Directory.CreateDirectory(fileSvr.pathSvr);

            //添加到数据表
            DBFile db = new DBFile();
            db.Add(ref fileSvr);
            up6_biz_event.folder_create(fileSvr);

            string json = JsonConvert.SerializeObject(fileSvr);
            json = HttpUtility.UrlEncode(json);
            json = json.Replace("+", "%20");
            var jo = new JObject { { "value", json } };
            json = callback + string.Format("({0})", JsonConvert.SerializeObject(jo));
            PageTool.to_content(json);
        }
        void folder_complete()
        {
            string id = Request.QueryString["id"];
            string uid = Request.QueryString["uid"];
            string cak = Request.QueryString["callback"];
            int ret = 0;

            if (string.IsNullOrEmpty(id)
                || uid.Length < 1)
            {
            }
            else
            {
                FileInf inf = new FileInf();
                DBFile db = new DBFile();
                db.read(id, ref inf);
                string root = inf.pathSvr;

                //上传完毕
                DBFile.fd_complete(id, uid);

                //扫描文件夹结构，
                fd_scan sa = new fd_scan();
                sa.root = inf;//
                sa.scan(inf, root);

                //更新扫描状态
                DBFile.fd_scan(id, uid);

                up6_biz_event.folder_post_complete(id);

                ret = 1;
            }
            PageTool.to_content(cak + "(" + ret + ")");
        }
    }
}