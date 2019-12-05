using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Web;

namespace up6.filemgr.app
{
    public class WebBase : System.Web.UI.Page
    {
        /// <summary>
        /// 默认加载data/config/config.json
        /// </summary>
        public JToken m_config;
        public JObject m_site;
        //
        public JToken m_cfgObj;
        public ConfigReader m_webCfg;
        /// <summary>
        /// 页面级的变量
        /// 后台：
        ///     param["id"] = 1;
        /// 定义：
        ///     param.query 查询变量
        /// 前台：
        /// <%= this.param["id"]%>
        /// </summary>
        public JObject param = new JObject();
        /// <summary>
        /// 默认加载/data/config/AppPath.json
        /// </summary>
        public JToken m_path;

        public WebBase()
        {
            this.m_webCfg = new ConfigReader();
            this.m_path = this.m_webCfg.module("path");
            this.m_cfgObj = this.m_webCfg.m_files;
            this.regParamRequest();//注册请求变量
            this.regParamPath();//注册路径变量
        }

        /// <summary>
        /// 获取AppPath.json中的路径
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public JToken path(string p) {
            return this.m_path.SelectToken(p);
        }

        public void regParamPath()
        {
            var o = this.m_webCfg.module("path");
            this.param.Add("path", o);
        }

        public void regParamRequest() {
            JObject query = new JObject();
            foreach (var key in HttpContext.Current.Request.QueryString.Keys)
            {
                var kv = HttpContext.Current.Request.QueryString[key.ToString()];
                JObject obj = new JObject { { key.ToString(), kv } };
                query.Add(key.ToString(), kv);
            }

            this.param.Add("query", query);
            this.param.Add("url", HttpContext.Current.Request.Url.AbsoluteUri);
        }

        /// <summary>
        /// 自动删除首尾空白
        /// </summary>
        /// <returns></returns>
        public JObject request_to_json() {
            JObject query = new JObject();
            foreach (var key in HttpContext.Current.Request.QueryString.Keys)
            {
                var kv = HttpContext.Current.Request.QueryString[key.ToString()];
                if (string.IsNullOrEmpty(kv)) kv = string.Empty;
                kv = kv.Trim();
                query.Add(key.ToString(), kv);
            }
            return query;
        }

        public string reqToString(string name)
        {
            var v = HttpContext.Current.Request.QueryString[name];
            if (string.IsNullOrEmpty(v)) return string.Empty;
            return v;
        }

        public int reqToInt(string name) {
            var v = this.reqToString(name);
            if (string.IsNullOrEmpty(v)) return 0;
            return int.Parse(v);
        }

        /// <summary>
        /// 注册页面级变量
        /// </summary>
        /// <returns></returns>
        public string paramPage()
        {
            string v = string.Format("<script>var page={0};</script>", JsonConvert.SerializeObject(this.param));
            return v;
        }

        public string toInclude(string file)
        {
            bool css = file.ToLower().EndsWith("css");
            if (css)
            {
                return string.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />"
                    , file);
            }
            else
            {
                return string.Format("<script type=\"text/javascript\" src=\"{0}\" charset=\"{1}\"></script>"
                    , file
                    , "utf-8");
            }
        }

        public string require(params object[] ps)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object f in ps)
            {
                //字符串
                if (Type.GetType("System.String") == f.GetType())
                {
                    sb.Append( toInclude(f.ToString()));
                }//json object
                else {
                    var t = JToken.FromObject(f);
                    //数组
                    if (t.Type == JTokenType.Array)
                    {
                        var arr = JArray.FromObject(t);
                        foreach (var a in arr)
                        {
                            sb.Append(toInclude(a.ToString()));
                        }
                    }
                    else {
                        sb.Append(toInclude(t.ToString()));
                    }
                }
            }
            return sb.ToString();
        }

        public string localFile(string file)
        {
            var ps = HttpContext.Current.Server.MapPath(file);
            return File.ReadAllText(ps);
        }

        public string localFile(JToken t)
        {
            StringBuilder sb = new StringBuilder();

            if (t.Type == JTokenType.Array)
            {
                foreach (var jt in t)
                {
                    var data = this.localFile(jt.ToString());
                    sb.Append(data);
                }
            }
            else if (t.Type == JTokenType.String)
            {
                sb.Append(this.localFile(t.ToString()));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 加载模板文件,
        /// </summary>
        /// <param name="file">模板文件相对路径,/data/tmp.html</param>
        /// <returns></returns>
        public string template(string file)
        {
            HtmlTemplater ht = new HtmlTemplater();
            ht.setFile(file);
            return ht.toString();
        }

        public string template(JToken t)
        {
            StringBuilder sb = new StringBuilder();

            HtmlTemplater ht = new HtmlTemplater();
            if (t.Type == JTokenType.Array)
            {
                foreach (var jt in t)
                {
                    ht.setFile(jt.ToString());
                    sb.Append(ht.ToString());
                }
            }
            else if (t.Type == JTokenType.String)
            {
                ht.setFile(t.ToString());
                sb.Append(ht.toString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将JSON作为页面内容输出，
        /// </summary>
        /// <param name="p"></param>
        public void toContent(JToken p)
        {
            Response.Clear();
            Response.Write(JsonConvert.SerializeObject(p));
            Response.End();
        }

        public void toContent(string v)
        {
            Response.Clear();
            Response.Write(v);
            Response.End();
        }
    }
}