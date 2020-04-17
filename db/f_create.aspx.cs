using System;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using up6.db.biz;
using up6.db.model;
using up6.db.utils;
using up6.db.database;
using up6.filemgr.app;
using Newtonsoft.Json.Linq;

namespace up6.db
{
    /// <summary>
    /// 此文件处理单文件上传逻辑
    /// 此页面需要返回文件的pathSvr路径。并进行urlEncode编码
    /// 更新记录：
    ///     2016-03-23 优化逻辑，分离子文件逻辑
    /// </summary>
    public partial class f_create : WebBase
    {
        void mkpath()
        {
            var id = this.reqString("id");
            var pathLoc = this.reqStringDecode("pathLoc");

            FileInf fileSvr = new FileInf();
            fileSvr.id = id;
            fileSvr.nameLoc = Path.GetFileName(pathLoc);
            fileSvr.nameSvr = fileSvr.nameLoc;
            fileSvr.pathLoc = pathLoc;

            PathBuilderUuid pb = new PathBuilderUuid();
            fileSvr.pathSvr = pb.genFile(id, fileSvr.nameLoc);
            fileSvr.pathSvr = fileSvr.pathSvr.Replace("\\", "/");

            //数据库存在相同文件
            DBFile db = new DBFile();
            FileInf fileExist = new FileInf();
            db.Add(ref fileSvr);
            //触发事件
            up6_biz_event.file_create(fileSvr);
            
            //
            JObject o = new JObject();
            o["pathSvr"] = fileSvr.pathSvr;
            this.toContent(o);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string op          = this.reqString("op");
            string pid          = this.reqString("pid");
            string pidRoot      = this.reqString("pidRoot");
            string md5          = this.reqString("md5");
            string id           = this.reqString("id");
            string uid          = this.reqString("uid");
            string lenLoc       = this.reqString("lenLoc");
            string sizeLoc      = this.reqString("sizeLoc");
            string callback     = this.reqString("callback");//jsonp参数
            //客户端使用的是encodeURIComponent编码，
            string pathLoc      = this.reqStringDecode("pathLoc");//utf-8解码

            if (op == "mkpath") this.mkpath();

            if (string.IsNullOrEmpty(pid)) pid = string.Empty;
            if (string.IsNullOrEmpty(pidRoot)) pidRoot = pid;

            //参数为空
            if (string.IsNullOrEmpty(md5) || 
                string.IsNullOrEmpty(uid) || 
                string.IsNullOrEmpty(sizeLoc)
                )
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
            fileSvr.lenLoc = Convert.ToInt64(lenLoc);
            fileSvr.sizeLoc = sizeLoc;
            fileSvr.deleted = false;
            fileSvr.md5 = md5;
            fileSvr.nameSvr = fileSvr.nameLoc;

            //所有单个文件均以uuid/file方式存储
            PathBuilderUuid pb = new PathBuilderUuid();
            fileSvr.pathSvr = pb.genFile(fileSvr.uid, ref fileSvr);
            fileSvr.pathSvr = fileSvr.pathSvr.Replace("\\","/");

            //数据库存在相同文件
            DBConfig cfg = new DBConfig();
            DBFile db = cfg.db();
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
                fr.make(fileSvr.pathSvr,fileSvr.lenLoc);
            }

            //加密
            ConfigReader cr = new ConfigReader();
            var sec = cr.module("path");
            var encrypt = (bool)sec.SelectToken("$.security.encrypt");
            if (encrypt)
            {
                CryptoTool ct = new CryptoTool();
                fileSvr.pathSvr = ct.encode(fileSvr.pathSvr);
            }

            string jv = JsonConvert.SerializeObject(fileSvr);
            jv = HttpUtility.UrlEncode(jv);
            jv = jv.Replace("+", "%20");
            string json = callback + "({\"value\":\"" + jv + "\",\"ret\":true})";//返回jsonp格式数据。
            Response.Write(json);
        }
    }
}