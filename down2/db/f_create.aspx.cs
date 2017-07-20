using System;
using System.Web;
using Newtonsoft.Json;
using up6.down2.biz;

namespace up6.down2.db
{
    public partial class f_create : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id       = Request.QueryString["id"];
            string uid      = Request.QueryString["uid"];
            string nameLoc  = Request.QueryString["nameLoc"];//客户端使用的是encodeURIComponent编码，
            string pathLoc  = Request.QueryString["pathLoc"];//客户端使用的是encodeURIComponent编码，
            string fileUrl  = Request.QueryString["fileUrl"];
            pathLoc         = HttpUtility.UrlDecode(pathLoc);//utf-8解码
            nameLoc         = HttpUtility.UrlDecode(nameLoc);
            string lenSvr   = Request.QueryString["lenSvr"];
            string sizeSvr  = Request.QueryString["sizeSvr"];
            string cbk      = Request.QueryString["callback"];//应用于jsonp数据

            if (string.IsNullOrEmpty(uid)
                || string.IsNullOrEmpty(pathLoc)
                || string.IsNullOrEmpty(fileUrl)
                || string.IsNullOrEmpty(lenSvr))
            {
                Response.Write(cbk + "({\"value\":null})");
                Response.End();
                return;
            }

            model.DnFileInf inf = new model.DnFileInf();
            inf.id = id;
            inf.uid = int.Parse(uid);
            inf.nameLoc = nameLoc;
            inf.pathLoc = pathLoc;//记录本地存储位置
            inf.fileUrl = fileUrl;
            inf.lenSvr = long.Parse(lenSvr);
            inf.sizeSvr = sizeSvr;
            DnFile db = new DnFile();
            db.Add(ref inf);

            string json = JsonConvert.SerializeObject(inf);
            json = HttpUtility.UrlEncode(json);
            json = json.Replace("+", "%20");
            json = cbk + "({\"value\":\"" + json + "\"})";//返回jsonp格式数据。
            Response.Write(json);
        }
    }
}