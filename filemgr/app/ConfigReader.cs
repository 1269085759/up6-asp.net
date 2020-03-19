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
            string file = HttpRuntime.AppDomainAppPath + "/filemgr/data/config/config.json";
            this.m_files = JToken.Parse(File.ReadAllText(file));
        }

        /// <summary>
        /// 自动加载网站中的文件
        /// </summary>
        /// <param name="f">/data/file.txt</param>
        /// <returns></returns>
        public string loclFile(string f)
        {
            f = HttpRuntime.AppDomainAppPath + f;
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
            file = HttpRuntime.AppDomainAppPath + file;
            var o = JToken.Parse( File.ReadAllText(file ));
            return o;
        }

        public string readFile(string name)
        {
            string file = (string)this.m_files.SelectToken(name);
            file = HttpRuntime.AppDomainAppPath + file;
            return File.ReadAllText(file);
        }

        public JToken readJson(string name)
        {
            string file = (string)this.m_files.SelectToken(name);
            file = HttpRuntime.AppDomainAppPath + file;
            var o = JToken.Parse(File.ReadAllText(file));
            return o;
        }

        public string readString(string name)
        {
            string v = (string)this.m_files.SelectToken(name);
            return v;
        }

        public bool readBool(string name)
        {
            var v = (bool)this.m_files.SelectToken(name);
            return v;
        }

        /// <summary>
        /// 读取/data/config/目录下的文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Dictionary<string,object> read(string fileName)
        {
            string ps = HttpRuntime.AppDomainAppPath + "/data/config/"+fileName;
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
            string ps = HttpRuntime.AppDomainAppPath + "/data/config/" + fileName;
            string data = File.ReadAllText(ps);
            var m = JToken.Parse(data);
            return m;
        }
    }
}