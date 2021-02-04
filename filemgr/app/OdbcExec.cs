using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using up6.db.database;

namespace up6.filemgr.app
{
    public class OdbcExec: SqlExec
    {
        public override void insert(string table, string fields, JObject o)
        {
            //加载结构
            this.m_table = this.table(table);
            var identity = this.m_table.SelectToken("fields[?(@.identity==true && @.primary==true)]");
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);

            string sql = string.Format("insert into {0} ( {1} ) values( {2} );"
                , table
                , fields
                , this.toSqlParam(fields));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_pvSetter.setVal(cmd, field_sel, o);
            var id = db.ExecuteScalar(cmd);
        }

        public override void insert(string table, SqlParam[] pars)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(pars, field_all);
            var identity = field_all.SelectToken("[?(@.identity==true && @.primary==true)]");

            string sql = string.Format("insert into {0} ( {1} ) values( {2} );"
                , table
                , this.toSqlFields(pars)
                , this.toSqlParam(pars));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_parSetter.setVal(cmd, field_sel, pars);
            var id = db.ExecuteScalar(cmd);
        }

        public override void update(string table, SqlParam[] fields, SqlParam[] where, string predicate = "and")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);
            var field_cdt = this.selFields(where, field_all);

            JObject o = new JObject();
            string sql = string.Format("update \"{0}\" set {1} where {2}"
                , table
                , this.toSqlCondition(fields)
                , this.toSqlCondition(where, predicate));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_parSetter.setVal(cmd, field_sel, fields);
            this.m_parSetter.setVal(cmd, field_cdt, where);
            db.ExecuteNonQuery(cmd);
        }


        public override void update(string table, string fields, string where, JObject obj)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);
            var field_cdt = this.selFields(where, field_all);

            string sql = string.Format("update \"{0}\" set {1} where {2}"
                , table
                , this.toSqlSeter(fields)
                , this.toSqlSeter(where));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_pvSetter.setVal(cmd, field_sel, obj);
            this.m_pvSetter.setVal(cmd, field_cdt, obj);
            db.ExecuteNonQuery(cmd);
        }

        public override string toSqlSeter(string fields)
        {
            var lst = fields.Split(',').ToList();
            var arr = from t in lst
                      select string.Format("{0}=?", t);
            return string.Join(",", arr.ToArray());
        }

        public override void delete(string table, SqlParam[] where, string predicate = "and")
        {
            JObject o = new JObject();
            string sql = string.Format("delete from \"{0}\" where {1}"
                , table
                , where);

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            db.ExecuteNonQuery(cmd);
        }

        public override void delete(string table, string where)
        {
            JObject o = new JObject();
            string sql = string.Format("delete from \"{0}\" where {1}"
                , table
                , where);

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            db.ExecuteNonQuery(cmd);
        }

        public override void delete_batch(string table, SqlParam[] ws, JToken values, string predicate = "and")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(ws, field_all);

            JObject o = new JObject();
            string sql = string.Format("delete from \"{0}\" where {1}"
                , table
                , this.toSqlCondition(ws, predicate));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_parSetter.setVal(cmd, field_sel, ws);
            cmd.Connection.Open();
            cmd.Prepare();

            SqlValueSetter pvs = new SqlValueSetter();

            foreach (var v in values)
            {
                pvs.setVal(cmd, field_sel, v);
                cmd.ExecuteNonQuery();
            }

            cmd.Connection.Close();
        }

        public override int count(string table, SqlParam[] where)
        {
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_cdt = this.selFields(where, field_all);

            JObject o = new JObject();
            string sql = string.Format("select count(*) from \"{0}\" where {1}"
                , table
                , this.toSqlCondition(where, "and"));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_parSetter.setVal(cmd, field_cdt, where);
            var obj = db.ExecuteScalar(cmd);
            return Convert.ToInt32(obj);
        }

        public override string toSqlFields(SqlParam[] ps)
        {
            var arr = from t in ps
                      select string.Format("\"{0}\"", t.Name);
            var name = string.Join(",", arr.ToArray());
            return name;
        }

        /// <summary>
        /// 返回字段列表
        /// </summary>
        /// <param name="o"></param>
        /// <param name="field"></param>
        /// <returns>"f_id","f_uid"</returns>
        public override string selFieldNames(JToken o, string field = "name")
        {
            var arr = from t in o
                      select string.Format("\"{0}\"", t[field]);
            var name = string.Join(",", arr.ToArray());
            return name;
        }

        /// <summary>
        /// 转换成odbc变量，?
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public override string toSqlParam(SqlParam[] ps)
        {
            var names = from t in ps
                        select "?";
            var name = string.Join(",", names.ToArray());
            return name;
        }

        public override string toSqlParam(string fields)
        {
            var arr = fields.Split(',').ToList();
            var names = from a in arr
                        select "?";
            var name = string.Join(",", names.ToArray());
            return name;
        }

        public override string toSqlCondition(SqlParam[] ps, string pre = ",")
        {
            if (ps == null) return "1=1";

            var arr = from t in ps
                      select string.Format("{0}=?", t.Name);

            var name = string.Join(" " + pre + " ", arr.ToArray());
            return name;
        }

        public override JToken select(string table, string fields, string where, string sort = "")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);

            //防止字段名称冲突
            var fns_sql = from f in field_sel
                          select "\"" + f["name"].ToString() + "\"";
            fields = string.Join(",", fns_sql.ToArray());

            if (!string.IsNullOrEmpty(sort)) sort = string.Format(" order by {0}", sort);
            string sql = string.Format("select {0} from {1} where {2} {3}"
                , fields
                , table
                , where
                , sort);

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            var r = db.ExecuteReader(cmd);

            JArray a = new JArray();

            while (r.Read())
            {
                var o = this.m_cmdRd.read(r, field_sel);
                a.Add(o);
            }
            r.Close();
            return JToken.FromObject(a);
        }
    }
}