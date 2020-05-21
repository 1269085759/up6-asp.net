using System;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using up6.db.biz;
using up6.db.database;
using up6.db.model;
using up6.db.utils;
using up6.filemgr.app;

namespace up6.db
{
    /// <summary>
    /// 以guid模式存储文件夹，
    /// </summary>
    public partial class fd_create : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id       = this.reqString("id");
            string pid      = this.reqString("pid");
            string pidRoot  = this.reqString("pidRoot");
            string uid      = this.reqString("uid");
            string lenLoc   = this.reqString("lenLoc");
            string sizeLoc  = this.reqString("sizeLoc");
            string pathLoc  = this.reqStringDecode("pathLoc");
            string callback = this.reqString("callback");//jsonp参数
            if (string.IsNullOrEmpty(pid)) pid = string.Empty;
            if (string.IsNullOrEmpty(pidRoot)) pidRoot = pid;

            if (string.IsNullOrEmpty(id)
                || string.IsNullOrEmpty(uid)
                || string.IsNullOrEmpty(pathLoc)
                )
            {
                Response.Write(callback + "({\"value\":null})");
                return;
            }

            FileInf fileSvr = new FileInf();
            fileSvr.id      = id;
            fileSvr.pid     = pid;
            fileSvr.pidRoot = pidRoot;
            fileSvr.fdChild = false;
            fileSvr.fdTask  = true;
            fileSvr.uid     = int.Parse(uid);//将当前文件UID设置为当前用户UID
            fileSvr.nameLoc = Path.GetFileName(pathLoc);
            fileSvr.pathLoc = pathLoc;
            fileSvr.lenLoc  = Convert.ToInt64(lenLoc);
            fileSvr.sizeLoc = sizeLoc;
            fileSvr.deleted = false;
            fileSvr.nameSvr = fileSvr.nameLoc;

            //生成存储路径
            PathBuilderUuid pb = new PathBuilderUuid();
            fileSvr.pathSvr    = pb.genFolder(ref fileSvr);
            fileSvr.pathSvr    = fileSvr.pathSvr.Replace("\\", "/");
            if (!Directory.Exists(fileSvr.pathSvr)) Directory.CreateDirectory(fileSvr.pathSvr);

            //添加成根目录
            if (string.IsNullOrEmpty(pid))
            {
                DBConfig cfg = new DBConfig();
                DBFile db = cfg.db();
                db.Add(ref fileSvr);
            }//添加成子目录
            else {
                SqlExec se = new SqlExec();
                se.insert("up6_folders", new SqlParam[] {
                     new SqlParam("f_id",fileSvr.id)
                    ,new SqlParam("f_nameLoc",fileSvr.nameLoc)
                    ,new SqlParam("f_pid",fileSvr.pid)
                    ,new SqlParam("f_pidRoot",fileSvr.pidRoot)
                    ,new SqlParam("f_lenLoc",fileSvr.lenLoc)
                    ,new SqlParam("f_sizeLoc",fileSvr.sizeLoc)
                    ,new SqlParam("f_pathLoc",fileSvr.pathLoc)
                    ,new SqlParam("f_pathSvr",fileSvr.pathSvr)
                    ,new SqlParam("f_uid",fileSvr.uid)
                });
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

            up6_biz_event.folder_create(fileSvr);

            string json = JsonConvert.SerializeObject(fileSvr);
            json = HttpUtility.UrlEncode(json);
            json = json.Replace("+", "%20");
            var jo = new JObject { { "value",json} };
            json = callback + string.Format("({0})",JsonConvert.SerializeObject(jo));
            Response.Write(json);
        }
    }
}