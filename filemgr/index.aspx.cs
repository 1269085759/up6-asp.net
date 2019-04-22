using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using up6.filemgr.app;

namespace up6.filemgr
{
    public partial class index : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string op = Request.QueryString["op"];

            if (op == "data") this.load_data();
            if (op == "rename") this.f_rename();
            if (op == "path") this.build_path();
        }

        /// <summary>
        /// 生成导航路径
        /// </summary>
        void build_path() {

            var data = Request.QueryString["data"];
            data = Server.UrlDecode(data);
            var fd = JObject.Parse(data);

            if (string.IsNullOrEmpty(fd["f_pid"].ToString().Trim()))
            {
                List<JToken> arr = new List<JToken>();
                if (string.IsNullOrEmpty(fd["f_id"].ToString().Trim()))
                {
                    arr.Add(fd);
                }//其它目录
                else
                {
                    arr.Add(new JObject { { "f_id", "" }, { "f_nameLoc", "根目录" }, { "f_pid", "" }, { "f_pidRoot", "" } });
                    arr.Add(fd);
                }
                PageTool.to_content(JToken.FromObject(arr));
                return;
            }

            SqlExec se = new SqlExec();

            var folders = (JArray)se.select("up6_folders"
                , "f_id,f_nameLoc,f_pid,f_pidRoot"
                , new SqlParam[] {
                    new SqlParam("f_pidRoot",fd["f_pidRoot"].ToString())
                });
            //根目录
            var folderRoot = se.read("up6_files", "f_id,f_nameLoc,f_pid,f_pidRoot"
                , new SqlParam[] {
                    new SqlParam("f_id",fd["f_pidRoot"].ToString())
            });
            folders.Add(folderRoot);            

            //key表
            PageTool.to_content(this.build_path2(folders, fd));
        }

        JToken build_path2(JToken data,JToken fodCur)
        {
            List<JToken> psort = new List<JToken>();
            //id,folder
            Dictionary<string, JToken> dt = new Dictionary<string, JToken>();
            foreach (var fd in data)
            {
                dt[fd["f_id"].ToString()] = fd;
            }

            string cur = fodCur["f_id"].ToString();
            while(true)
            {
                //key不存在
                if (!dt.ContainsKey(cur)) break;

                var d = dt[cur];//查父ID
                psort.Insert(0, d);
                cur = d["f_pid"].ToString();//取父级ID

                if (cur.Trim() == "0") break;
                if (string.IsNullOrEmpty(cur.Trim())) break;
            }

            psort.Insert(0,(new JObject { { "f_id", "" }, { "f_nameLoc", "根目录" }, { "f_pid", "" }, { "f_pidRoot", "" } }) );

            return JToken.FromObject(psort);
        }

        void f_rename() {
            var data = Request.QueryString["data"];
            var obj = JObject.Parse(data);

            SqlExec se = new SqlExec();
            se.update("up6_files", "f_nameLoc", "f_id", obj);

            PageTool.to_content(obj);
        }

        void load_data() {
            SqlExec se = new SqlExec();

            SqlWhereMerge swm = new SqlWhereMerge();
            swm.req_equal("f_pid", "pid",false);
            swm.equal("f_complete", 1);
            swm.req_like("f_nameLoc", "key");
            string where = swm.to_sql();

            //文件表
            var files = (JArray)DbBase.page2("up6_files"
                , "f_id"
                , "f_id,f_pid,f_nameLoc,f_sizeLoc,f_time,f_pidRoot,f_fdTask"
                , where
                ,"f_fdTask desc,f_time desc");

            //目录表
            var folders = (JArray)DbBase.page2("up6_folders"
                , "f_id"
                , "f_id,f_nameLoc,f_pid,f_pidRoot,f_time"
                , where
                , "f_time desc");

            //合并表
            foreach (var fd in folders)
            {
                files.Insert(0,new JObject {
                    { "f_id",fd["f_id"]}
                    ,{ "f_pid",fd["f_pid"]}
                    ,{ "f_nameLoc",fd["f_nameLoc"]}
                    ,{ "f_sizeLoc",""}
                    ,{ "f_time",fd["f_time"]}
                    ,{ "f_pidRoot",fd["f_pidRoot"]}
                    ,{ "f_fdTask",true}
                });
            }

            int count = DbBase.count("up6_files", "f_id", where);

            JObject o = new JObject();
            o["count"] = count;
            o["code"] = 0;
            o["msg"] = string.Empty;
            o["data"] = files;

            PageTool.to_content(o);
        }
    }
}