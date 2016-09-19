using up6.demoSql2005.down2.biz;
using up6.demoSql2005.down2.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace up6.demoSql2005.down2.db
{
    /// <summary>
    /// 创建一个文件夹下载任务。
    /// JSON格式：
    /// {
    //	"m_perSvr": "100%",
    //	"nameLoc": "files-1",
    //	"lenLoc": 12244936,
    //	"size": "11.6MB",
    //	"lenSvr": 12244936,
    //	"perSvr": "100%",
    //	"pidLoc": 0,
    //	"pidSvr": 0,
    //	"idLoc": 0,
    //	"idSvr": 1421,
    //	"idFile": 2524,
    //	"uid": 0,
    //	"foldersCount": 0,
    //	"filesCount": 1,
    //	"filesComplete": 0,
    //	"pathLoc": "C: \\\\Users\\\\Administrator\\\\Desktop\\\\test\\\\files-1",
    //	"pathSvr": "",
    //	"pidRoot": 0,
    //	"pathRel": "",
    //	"files": [{
    //		"nameLoc": "360wangpan_setup.exe",
    //		"pathLoc": "C:\\\\Users\\\\Administrator\\\\Desktop\\\\test\\\\files-1\\\\360wangpan_setup.exe",
    //		"pathSvr": "F:\\\\csharp\\\\HttpUploader6\\\\trunk\\\\v1.3-fd\\\\upload\\\\2016\\\\07\\\\24\\\\a03b6d45916dcd6db43d1660ac789f78.exe",
    //		"md5": "a03b6d45916dcd6db43d1660ac789f78",
    //		"pidLoc": 0,
    //		"pidSvr": 1421,
    //		"pidRoot": 1421,
    //		"idLoc": 0,
    //		"idSvr": 2525,
    //		"uid": 0,
    //		"lenLoc": 12244936,
    //		"sizeLoc": "11.6MB",
    //		"lenSvr": 12244936,
    //		"postPos": 0,
    //		"perSvr": "0%",
    //		"pathRel": null,
    //		"complete": false,
    //		"nameSvr": null
    //    }]
    //}
    /// 
    /// </summary>
    public partial class fd_create : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string uid      = Request.Form["uid"];
            string fdStr    = Request.Form["folder"];//客户端使用的是encodeURIComponent编码，
            if (!string.IsNullOrEmpty(fdStr)) fdStr = fdStr.Replace("%20", "+");
            if (!string.IsNullOrEmpty(fdStr)) fdStr = Server.UrlDecode(fdStr);

            if (string.IsNullOrEmpty(uid)
               || string.IsNullOrEmpty(fdStr))
            {
                Response.Write(0);
                Response.End();
                return;
            }

            DnFolderInf fd = JsonConvert.DeserializeObject<DnFolderInf>(fdStr);
            folder_appender fa = new folder_appender();
            fa.add(ref fd);

            string json = JsonConvert.SerializeObject(fd);
            json = HttpUtility.UrlEncode(json);
            //UrlEncode会将空格解析成+号
            json = json.Replace("+", "%20");
            Response.Write(json);
        }
    }
}