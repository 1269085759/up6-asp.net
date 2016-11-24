using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using up6.demoSql2005.db;
using up6.demoSql2005.db.biz;

namespace up6.demoSql2005.debug
{
    public partial class check_config : System.Web.UI.Page
    {
        //public string m_conStr = string.Empty;
        //public string m_uploadPath = string.Empty;
        //public string m_conSucc = "失败";
        public string server = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            JObject cfg = new JObject();
            cfg["conStr"] = DbHelper.GetConStr().Replace("\\","/");
            cfg["conState"] = false;

            //输出数据库连接信息
            //this.m_conStr = DbHelper.GetConStr();
            var con = DbHelper.CreateConnection();
            try { con.Open(); con.Close(); cfg["conState"] = true; } catch (Exception ex) { }


            //输出服务器存储路径
            PathMd5Builder pb = new PathMd5Builder();
            string pathSvr = pb.genFile(0, "md5", "QQ2016.exe");
            pathSvr = pathSvr.Replace("\\", "/");
            cfg["uploadPath"] = pathSvr;

            this.server = JsonConvert.SerializeObject(cfg);
        }
    }
}