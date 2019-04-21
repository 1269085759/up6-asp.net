using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace up6.filemgr.app
{
    /// <summary>
    /// 系统配置文件读取
    /// ConfigReader cr = new ConfigReader();
    /// cr.module("名称");//获取json对象
    /// </summary>
    public class ConfigReader
    {
        public JToken m_files;

        /// <summary>
        /// 自动加载/data/config/config.json配置文件
        /// </summary>
        public ConfigReader()
        {
            string file = HttpContext.Current.Server.MapPath("/filemgr/data/config.json");
            this.m_files = JToken.Parse(File.ReadAllText(file));
        }

        /// <summary>
        /// 自动加载网站中的文件
        /// </summary>
        /// <param name="f">/data/file.txt</param>
        /// <returns></returns>
        public string loclFile(string f)
        {
            f = HttpContext.Current.Server.MapPath(f);
            return File.ReadAllText(f);
        }

        /// <summary>
        /// 自动加载模块
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JToken module(string name)
        {
            string file = (string)this.m_files.SelectToken(name);
            file = HttpContext.Current.Server.MapPath(file);
            var o = JToken.Parse( File.ReadAllText(file ));
            return o;
        }

        public JToken to_json(string name)
        {
            string file = (string)this.m_files.SelectToken(name);
            file = HttpContext.Current.Server.MapPath(file);
            var o = JToken.Parse(File.ReadAllText(file));
            return o;
        }

        public string to_string(string name)
        {
            string file = (string)this.m_files.SelectToken(name);
            file = HttpContext.Current.Server.MapPath(file);
            return File.ReadAllText(file);
        }

        public Dictionary<string,object> read()
        {
            string ps = HttpContext.Current.Server.MapPath("/data/config/AppConfig.json");
            string data = File.ReadAllText(ps);
            var arr = JsonConvert.DeserializeObject<Dictionary<string,object>>(data);
            return arr;
        }

        /// <summary>
        /// 读取/data/config/目录下的文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Dictionary<string,object> read(string fileName)
        {
            string ps = HttpContext.Current.Server.MapPath("/data/config/"+fileName);
            string data = File.ReadAllText(ps);
            var m = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
            return m;
        }

        /// <summary>
        /// 读取/data/config目录下的文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public JToken configFile(string fileName)
        {
            string ps = HttpContext.Current.Server.MapPath("/data/config/" + fileName);
            string data = File.ReadAllText(ps);
            var m = JToken.Parse(data);
            return m;
        }
    }
}