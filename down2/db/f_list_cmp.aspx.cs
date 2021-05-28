using System;
using System.Web;
using up6.db.database;
using up6.down2.biz;
using up6.filemgr.app;

namespace up6.down2.db
{
    /// <summary>
    /// 列出所有已经上传完的文件和文件夹
    /// 格式：
    /// json:
    ///     [{f1,f2,f3,f4,f5}]
    /// xdb_files
    /// 文件：xdb_files
    /// 文件夹：xdb_files.fd_json
    /// 
    /// </summary>
    public partial class f_list_cmp : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string uid = this.reqStringSafe("uid");
            string cbk = this.reqStringSafe("callback");//jsonp

            if (!string.IsNullOrEmpty(uid))
            {
                DBConfig cfg = new DBConfig();
                DnFile db = cfg.downF();
                string json = db.all_complete(int.Parse(uid));
                if (!string.IsNullOrEmpty(json))
                {
                    System.Diagnostics.Debug.WriteLine(json);
                    json = HttpUtility.UrlEncode(json);
                    //UrlEncode会将空格解析成+号
                    json = json.Replace("+", "%20");
                    this.toContent(cbk + "({\"value\":\"" + json + "\"})", "application/json");
                    return;
                }
            }
            this.toContent(cbk + "({\"value\":null})", "application/json");
        }
    }
}