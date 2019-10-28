using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    public class PageTool
    {
        /// <summary>
        /// 将JSON作为js变量输出
        /// </summary>
        /// <param name="name"></param>
        /// <param name="p"></param>
        public static void to_param(string name, JToken p)
        {
            string v = string.Format("<script>var {0}={1};</script>"
                , name
                , JsonConvert.SerializeObject(p));

            HttpContext.Current.Response.Write(v);
        }

        /// <summary>
        /// 将JSON作为页面内容输出，
        /// </summary>
        /// <param name="p"></param>
        public static void to_content(JToken p)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write(JsonConvert.SerializeObject(p));
            HttpContext.Current.Response.End();
        }

        public static void to_content(string v)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write(v);
            HttpContext.Current.Response.End();
        }

        public static string query_decode(string name)
        {
            return HttpContext.Current.Server.UrlDecode(
                HttpContext.Current.Request.QueryString[name]);
        }
    }
}