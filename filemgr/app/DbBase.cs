using Newtonsoft.Json.Linq;
using System;
using System.Data.Common;
using System.Linq;
using System.Web;
using up6.db.database;

namespace up6.filemgr.app
{
    public class DbBase
    {
        public static DbDataReader all(string table)
        {
            string sql = string.Format("select * from {0}", table);
            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            var r = db.ExecuteReader(cmd);
            return r;
        }

        public static DbDataReader all(string table,string fields)
        {
            string sql = string.Format("select {0} from {1}", table,fields);
            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            var r = db.ExecuteReader(cmd);
            return r;
        }

        public static JToken page2(string table, string primaryKey, string fields, string where = "", string sort = "")
        {
            var pageSize = HttpContext.Current.Request.QueryString["limit"];
            var pageIndex = HttpContext.Current.Request.QueryString["page"];
            if (string.IsNullOrEmpty(pageSize)) pageSize = "20";
            if (string.IsNullOrEmpty(pageIndex)) pageIndex = "1";
            return page2(table, primaryKey, fields, int.Parse(pageSize), int.Parse(pageIndex), where, sort);
        }

        public static JToken page2(string table, string primaryKey, string fields, int pageSize, int pageIndex, string where = "", string sort = "")
        {
            ConfigReader cr = new ConfigReader();
            var database = cr.module(string.Format("database.{0}",table));
            //加载结构
            var table_fields = database.SelectToken("fields");
            var fields_arr = fields.Split(',').ToList();
            var field_sels = (from f in fields_arr
                      join tf in table_fields
                      on f equals tf["name"].ToString()
                      select tf).ToArray();

            //选择所有字段
            if (fields.Trim() == "*")
            {
                field_sels = table_fields.ToArray();
                fields_arr = (from f in field_sels
                          select f["name"].ToString()).ToList();
                fields = string.Join(",", fields_arr.ToArray());
            }

            DbHelper db = new DbHelper();
            var cmd = db.GetCommandStored("spPager");
            db.AddString(ref cmd, "@table", table, 200);
            db.AddString(ref cmd, "@primarykey", primaryKey, 50);
            db.AddInInt32(cmd, "@pagesize", pageSize);
            db.AddInInt32(cmd, "@pageindex", pageIndex);
            db.AddBool(ref cmd, "@docount", false);
            db.AddString(ref cmd, "@where", where, 1000);
            db.AddString(ref cmd, "@sort", sort, 50);
            db.AddString(ref cmd, "@fields", fields, 500);
            var r = db.ExecuteReader(cmd);
            JArray a = new JArray();
            SqlCmdReader scr = new SqlCmdReader();
            while (r.Read())
            {
                int index = 1;//从1开始,0是行号
                var o = new JObject();
                foreach (var field in fields_arr)
                {
                    var fd = field_sels[index-1];
                    var fd_type = fd["type"].ToString().ToLower();

                    o[field] = scr[fd_type](r, index++);
                }
                a.Add(o);
            }
            r.Close();
            return JToken.FromObject(a);
        }

        /// <summary>
        /// 返回成layer.table数据结构
        /// </summary>
        /// <param name="table"></param>
        /// <param name="primaryKey"></param>
        /// <param name="fields"></param>
        /// <param name="where"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static JToken page_to_layer_table(string table, string primaryKey, string fields, string where = "", string sort = "")
        {
            var pageSize = HttpContext.Current.Request.QueryString["limit"];
            var pageIndex = HttpContext.Current.Request.QueryString["page"];
            if (string.IsNullOrEmpty(pageSize)) pageSize = "20";
            if (string.IsNullOrEmpty(pageIndex)) pageIndex = "1";
            var data = page2(table, primaryKey, fields, int.Parse(pageSize), int.Parse(pageIndex), where, sort);
            int count = DbBase.count(table, primaryKey, where);

            JObject o = new JObject();
            o["count"] = count;
            o["code"] = 0;
            o["msg"] = string.Empty;
            o["data"] = data;

            return o;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="primaryKey"></param>
        /// <param name="where">name=1</param>
        /// <returns></returns>
        public static int count(string table ,string primaryKey,string where="")
        {
            DbHelper db = new DbHelper();
            var cmd = db.GetCommandStored("spPager");
            db.AddString(ref cmd, "@table", table, 50);
            db.AddString(ref cmd, "@primarykey", primaryKey, 50);
            db.AddInInt32(cmd, "@pagesize", 0);
            db.AddInInt32(cmd, "@pageindex", 0);
            db.AddBool(ref cmd, "@docount", true);
            db.AddString(ref cmd, "@where", where,1000);
            db.AddString(ref cmd, "@sort", string.Empty,50);
            db.AddString(ref cmd, "@fields", string.Empty,100);
            var obj = db.ExecuteScalar(cmd);
            return Convert.ToInt32(obj);
        }

        public static bool exist(string un)
        {
            DbHelper db = new DbHelper();
            var cmd = db.GetCommand("select id from users where name=@name");
            db.AddString(ref cmd, "@name", un, 50);
            var obj = db.ExecuteScalar(cmd);
            return Convert.ToInt32(obj) != 0;
        }

        public static bool exist_email(string email)
        {
            DbHelper db = new DbHelper();
            var cmd = db.GetCommand("select id from users where email=@email");
            db.AddString(ref cmd, "@email", email, 50);
            var obj = db.ExecuteScalar(cmd);
            return Convert.ToInt32(obj) != 0;
        }
    }
}