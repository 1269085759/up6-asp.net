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
        /// 默认加载
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
            this.param.Add("path", this.m_path);
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

        public JObject req_to_json() {
            JObject query = new JObject();
            foreach (var key in HttpContext.Current.Request.QueryString.Keys)
            {
                var kv = HttpContext.Current.Request.QueryString[key.ToString()];
                JObject obj = new JObject { { key.ToString(), kv } };
                query.Add(key.ToString(), kv);
            }
            return query;
        }

        /// <summary>
        /// 检查head值
        /// </summary>
        /// <returns></returns>
        public bool req_val_null_empty(string names)
        {
            var arr = names.Split(',');
            foreach (var a in arr)
            {
                var kv = Request.QueryString[a.Trim()];
                if (string.IsNullOrEmpty(kv.Trim())) return true;
            }
            return false;
        }

        public JObject head_to_json()
        {
            JObject query = new JObject();
            foreach (var key in HttpContext.Current.Request.Headers)
            {
                var kv = HttpContext.Current.Request.Headers[key.ToString()];
                JObject obj = new JObject { { key.ToString(), kv } };
                query.Add(key.ToString(), kv);
            }
            return query;
        }

        /// <summary>
        /// 检查head值
        /// </summary>
        /// <returns></returns>
        public bool head_val_null_empty(string names)
        {
            var arr = names.Split(',');
            foreach (var a in arr)
            {
                var kv = HttpContext.Current.Request.Headers[a.Trim()];
                if (string.IsNullOrEmpty(kv.Trim())) return true;
            }
            return false;
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
    }
}