using System;
using System.Web;
using up6.db.biz;
using up6.filemgr.app;

namespace up6.db
{
    public partial class f_list : WebBase
    {
        /// <summary>
        /// 以JSON格式列出所有文件（）
        /// 注意，输出的文件路径会进行UrlEncode编码
        /// 客户端需要进行UrlDecode解码
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            string uid = this.reqString("uid");
            string cbk = this.reqString("callback");//jsonp

            if (!string.IsNullOrEmpty(uid))
            {
                un_builder ub = new un_builder();
                string json = ub.read(uid);
                if (!string.IsNullOrEmpty(json))
                {
                    System.Diagnostics.Debug.WriteLine(json);
                    json = HttpUtility.UrlEncode(json);
                    //UrlEncode会将空格解析成+号
                    json = json.Replace("+", "%20");
                    Response.Write(cbk + "({\"value\":\"" + json + "\"})");
                    return;
                }
            }
            Response.Write(cbk+"({\"value\":null})");
        }
    }
}