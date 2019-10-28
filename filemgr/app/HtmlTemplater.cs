using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace up6.filemgr.app
{
    /// <summary>
    /// 模板生成器
    /// 用法：
    /// HtmlTemplater ht;
    /// ht.add(变量名称,变量值);//
    /// ht.setHtml();
    /// ht.toString();
    /// 更新时间：
    ///     2019-03-23 优化模板标签替换逻辑，提高效率
    /// </summary>
    public class HtmlTemplater
    {
        private JArray m_params;
        private string m_html;

        public HtmlTemplater() {
            this.m_params = new JArray();

            //添加系统路径变量
            this.add_param_AppPath();
        }

        void add_param_AppPath()
        {
            ConfigReader cr = new ConfigReader();
            this.m_params.Add(cr.m_files);
        }


        public void addParam(JObject o)
        {
            this.m_params.Add(o);
        }

        public void addParam(JToken o)
        {
            if (o == null) return;

            if (o.Type == JTokenType.Array)
            {
                foreach (var j in o)
                {
                    this.m_params.Add(j);
                }
            }
            else {
                this.m_params.Add((JObject)o);
            }
        }

        public void setHtml(string v) { this.m_html = v; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f">相对路径,/data/test.html</param>
        public void setFile(string f) {
            string path = HttpContext.Current.Server.MapPath(f);
            this.m_html = File.ReadAllText(path);
        }

        public string toString()
        {
            //解析所有标签
            this.parse_tags();

            return this.m_html;
        }

        /// <summary>
        /// 提取所有标签，
        /// </summary>
        /// <param name="tags">标签，标签名称。{name},name</param>
        public void extractTag(out Dictionary<string, string> tags)
        {
            int tagBegin = -1;//左括号开始位置
            //提取的标签 {tag}
            tags = new Dictionary<string,string>();

            int charStr = 0;
            int index = 0;
            StringReader sr = new StringReader(this.m_html);
            while ( sr.Peek() != -1)
            {
                charStr = sr.Read();
                if (charStr == '{') tagBegin = index;
                else if (charStr == '}' 
                    && (-1!= tagBegin)
                    )
                {
                    int tagLen = index - tagBegin + 1;
                    string tag = this.m_html.Substring(tagBegin, tagLen);

                    //过滤非标准标签名称,防止重复添加
                    if ( !tags.ContainsKey(tag))
                    {
                        var tagName = tag.Substring(1, tag.Length - 2);
                        tags.Add(tag, tagName);
                    }

                    tagBegin = -1;
                }
                else
                {
                    //跳过非标签字符串
                    char c = (char)charStr;
                    if (!char.IsLetterOrDigit(c) && c != '-' && c!='_')
                    {
                        tagBegin = -1;
                    }
                }
                ++index;
            }
        }

        public void parse_tags()
        {
            //提取标签,(标签值，标签名称)
            Dictionary<string,string> tags;
            this.extractTag(out tags);

            foreach(var o in this.m_params)
            {
                foreach (var t in tags)
                {
                    //去掉{}字符                    
                    var v = o.SelectToken(t.Value);
                    if (null == v) continue;
                    var val = v.ToString();
                    this.m_html = this.m_html.Replace(t.Key, val);
                }
            }
        }
    }
}