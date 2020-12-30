using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using up6.db.database;

namespace up6.filemgr.app
{
    public class OdbcDbBase: DbBase
    {
        public override JToken page2(string table, string primaryKey, string fields, string where = "", string sort = "")
        {
            var pageSize = HttpContext.Current.Request.QueryString["limit"];
            var pageIndex = HttpContext.Current.Request.QueryString["page"];
            if (string.IsNullOrEmpty(pageSize)) pageSize = "20";
            if (string.IsNullOrEmpty(pageIndex)) pageIndex = "1";
            return this.page2(table, primaryKey, fields, int.Parse(pageSize), int.Parse(pageIndex), where, sort);
        }

        public override JToken page2(string table, string primaryKey, string fields, int pageSize, int pageIndex, string where = "", string sort = "")
        {
            ConfigReader cr = new ConfigReader();
            var database = cr.module(string.Format("database.{0}", table));
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

            int pageStart = (pageIndex - 1) * (pageSize - 1);
            int pageEnd = (pageIndex - 1) * pageSize + pageSize;
            string sql = string.Format("select {0} from {1} where {2} limit {3},{4}", 
                fields, 
                table, 
                where,
                pageStart,
                pageEnd
                );

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            var r = db.ExecuteReader(cmd);
            JArray a = new JArray();
            SqlCmdReader scr = new SqlCmdReader();
            while (r.Read())
            {
                int index = 0;//从1开始,0是行号
                var o = new JObject();
                foreach (var field in fields_arr)
                {
                    var fd = field_sels[index];
                    var fd_type = fd["type"].ToString().ToLower();

                    o[field] = scr[fd_type](r, index++);
                }
                a.Add(o);
            }
            r.Close();
            return JToken.FromObject(a);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="primaryKey"></param>
        /// <param name="where">name=1</param>
        /// <returns></returns>
        public override int count(string table, string primaryKey, string where = "")
        {
            string sql = string.Format("select count(*) from {0} where {1}", table, where);

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            var obj = db.ExecuteScalar(cmd);
            return Convert.ToInt32(obj);
        }

        public override bool exist(string un)
        {
            DbHelper db = new DbHelper();
            var cmd = db.GetCommand("select id from users where name=?");
            db.AddString(ref cmd, "@name", un, 50);
            var obj = db.ExecuteScalar(cmd);
            return Convert.ToInt32(obj) != 0;
        }

        public override bool exist_email(string email)
        {
            DbHelper db = new DbHelper();
            var cmd = db.GetCommand("select id from users where email=?");
            db.AddString(ref cmd, "@email", email, 50);
            var obj = db.ExecuteScalar(cmd);
            return Convert.ToInt32(obj) != 0;
        }
    }
}