using System;
using System.Web;
using Newtonsoft.Json;
using up6.db.database;
using up6.down2.biz;
using up6.filemgr.app;

namespace up6.down2.db
{
    public partial class f_create : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id       = Request.QueryString["id"];
            string uid      = Request.QueryString["uid"];
            string fdTask   = Request.QueryString["fdTask"];
            string nameLoc  = Request.QueryString["nameLoc"];//客户端使用的是encodeURIComponent编码，
            string pathLoc  = Request.QueryString["pathLoc"];//客户端使用的是encodeURIComponent编码，
            string lenSvr   = Request.QueryString["lenSvr"];
            string sizeSvr  = Request.QueryString["sizeSvr"];
            string cbk      = Request.QueryString["callback"];//应用于jsonp数据
            pathLoc         = HttpUtility.UrlDecode(pathLoc);//utf-8解码
            nameLoc         = HttpUtility.UrlDecode(nameLoc);
            sizeSvr         = HttpUtility.UrlDecode(sizeSvr);

            if (string.IsNullOrEmpty(uid)
                || string.IsNullOrEmpty(pathLoc)
                || string.IsNullOrEmpty(lenSvr))
            {
                this.toContentJson(cbk + "({\"value\":null})");
                return;
            }

            model.DnFileInf inf = new model.DnFileInf();
            inf.id = id;
            inf.uid = int.Parse(uid);
            inf.nameLoc = nameLoc;
            inf.pathLoc = pathLoc;//记录本地存储位置
            inf.lenSvr = long.Parse(lenSvr);
            inf.sizeSvr = sizeSvr;
            inf.fdTask = fdTask == "1";
            DBConfig cfg = new DBConfig();
            DnFile db = cfg.downF();
            db.Add(ref inf);

            string json = JsonConvert.SerializeObject(inf);
            json = HttpUtility.UrlEncode(json);
            json = json.Replace("+", "%20");
            json = cbk + "({\"value\":\"" + json + "\"})";//返回jsonp格式数据。
            this.toContentJson(json);
        }
    }
}