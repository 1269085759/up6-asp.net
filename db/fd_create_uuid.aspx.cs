using System;
using System.Web;
using Newtonsoft.Json;
using up6.db.biz.folder;
using up6.db.biz;

namespace up6.db
{
    /// <summary>
    /// 以uuid模式存储文件夹，自动生成文件存储路径，自动生成文件夹存储路径
    /// 和客户端文件夹结构完全保持一致。
    /// </summary>
    public partial class fd_create_uuid : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string folderStr = Request.Form["folder"];
            folderStr = folderStr.Replace("+", "%20");
            folderStr = HttpUtility.UrlDecode(folderStr);

            fd_appender adder = new fd_uuid_appender();
            adder.m_root = JsonConvert.DeserializeObject<fd_root>(folderStr);
            adder.save();//保存到数据库
            //触发事件
            up6_biz_event.folder_create(adder.m_root);

            string json = JsonConvert.SerializeObject(adder.m_root);
            json = HttpUtility.UrlEncode(json);
            json = json.Replace("+", "%20");
            Response.Write(json);
        }
    }
}