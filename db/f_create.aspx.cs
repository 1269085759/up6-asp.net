﻿using System;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using up6.db.biz;
using up6.db.model;
using up6.db.utils;
using up6.db.database;

namespace up6.db
{
    /// <summary>
    /// 此文件处理单文件上传逻辑
    /// 此页面需要返回文件的pathSvr路径。并进行urlEncode编码
    /// 更新记录：
    ///     2016-03-23 优化逻辑，分享子文件逻辑
    /// </summary>
    public partial class f_create : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string md5          = Request.QueryString["md5"];
            string id           = Request.QueryString["id"];
            string uid          = Request.QueryString["uid"];
            string lenLoc       = Request.QueryString["lenLoc"];
            string sizeLoc      = Request.QueryString["sizeLoc"];
            string callback     = Request.QueryString["callback"];//jsonp参数
            //客户端使用的是encodeURIComponent编码，
            string pathLoc      = HttpUtility.UrlDecode(Request.QueryString["pathLoc"]);//utf-8解码

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
            fileSvr.nameLoc = Path.GetFileName(pathLoc);
            fileSvr.pathLoc = pathLoc;
            fileSvr.lenLoc = Convert.ToInt64(lenLoc);
            fileSvr.sizeLoc = sizeLoc;
            fileSvr.deleted = false;
            fileSvr.md5 = md5;
            fileSvr.nameSvr = md5 + Path.GetExtension(pathLoc).ToLower();
            
            //所有单个文件均以md5方式存储
            PathBuilderMd5 pb = new PathBuilderMd5();
            fileSvr.pathSvr = pb.genFile(fileSvr.uid, ref fileSvr);

            //数据库存在相同文件
            DBFile db = new DBFile();
            FileInf fileExist = new FileInf();
            if (db.exist_file(md5, ref fileExist))
            {
                fileSvr.pathSvr = fileExist.pathSvr;
                fileSvr.perSvr = fileExist.perSvr;
                fileSvr.lenSvr = fileExist.lenSvr;
                fileSvr.complete = fileExist.complete;
                db.Add(ref fileSvr);
            }//数据库不存在相同文件
            else
            {
                db.Add(ref fileSvr);

                //2.0创建器。仅创建一个空白文件
                FileBlockWriter fr = new FileBlockWriter();
                fr.make(fileSvr.pathSvr,fileSvr.lenLoc);
            }
            string jv = JsonConvert.SerializeObject(fileSvr);
            jv = HttpUtility.UrlEncode(jv);
            jv = jv.Replace("+", "%20");
            string json = callback + "({\"value\":\"" + jv + "\"})";//返回jsonp格式数据。
            Response.Write(json);
        }
    }
}