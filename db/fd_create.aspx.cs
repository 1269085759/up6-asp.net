using System;
using System.Web;
using Newtonsoft.Json;
using up6.db.biz.folder;

namespace up6.db
{
    /// <summary>
    /// 以md5模式存储文件夹，
    /// 
    /// 创建文件夹，并返回创建成功后的文件夹信息。
    /// 1.接收客户端传来的文件夹JSON信息
    /// 2.解析文件夹JSON信息，并根据层级关系创建子文件夹
    /// 3.将文件夹信息保存到数据库
    /// 4.更新文件夹JSON信息并返回到客户端。
    /// 将文件夹JSON保存到文件夹数据表中
    /// 格式：
    /// {
//	"nameLoc": "ftp",
//	"lenLoc": 49175780,
//	"size": "46.8 MB",
//	"lenSvr": 0,
//	"pidLoc": 0,
//	"pidSvr": 0,
//	"idLoc": 0,
//	"idSvr": 0,
//	"idFile": 0,
//	"uid": 0,
//	"foldersCount": 1,
//	"filesCount": 7,
//	"filesComplete": 0,
//	"pathLoc": "C:\\Users\\Administrator\\Desktop\\test\\ftp",
//	"pathSvr": "",
//	"pathRel": "",
//	"pidRoot": 0,
//	"complete": false,
//	"folders": [{
//		"idLoc": 1,
//		"idSvr": 0,
//		"nameLoc": "test2",
//		"pathLoc": "C:\\Users\\Administrator\\Desktop\\test\\ftp\\test2",
//		"pathSvr": "",
//		"pidLoc": 0,
//		"pidSvr": 0
//    }],
//	"files": [{
//		"idLoc": 0,
//		"lenLoc": 9186,
//		"lenSvr": 0,
//		"nameLoc": "ico-capture.jpg",
//		"pathLoc": "C:\\Users\\Administrator\\Desktop\\test\\ftp\\ico-capture.jpg",
//		"pidLoc": 0,
//		"pidSvr": 0,
//		"sizeLoc": "8.97 KB",
//		"md5": "5a2f78459c53169f3ca823093cee40ae"
//	},
//	{
//		"idLoc": 6,
//		"lenLoc": 49102472,
//		"lenSvr": 0,
//		"nameLoc": "Thunder_dl_7.9.6.4502.exe",
//		"pathLoc": "C:\\Users\\Administrator\\Desktop\\test\\ftp\\Thunder_dl_7.9.6.4502.exe",
//		"pidLoc": 0,
//		"pidSvr": 0,
//		"sizeLoc": "46.8 MB",
//		"md5": "523ec7a27a72924d8e23011ec6d851ed"
//	}],
//	"fdName": "ftp",
//	"fid": 0,
//	"name": "open_folders",
//	"perSvr": "0%",
//	"pid": 0,
//	"id": 1
//}
    /// </summary>
    public partial class fd_create : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string folderStr = Request.Form["folder"];
            folderStr = folderStr.Replace("+", "%20");
            folderStr = HttpUtility.UrlDecode(folderStr);

            fd_appender adder = new fd_appender();
            adder.m_root = JsonConvert.DeserializeObject<fd_root>(folderStr);
            adder.save();//保存到数据库

            string json = JsonConvert.SerializeObject(adder.m_root);
            json = HttpUtility.UrlEncode(json);
            json = json.Replace("+", "%20");
            Response.Write(json);
        }
    }
}