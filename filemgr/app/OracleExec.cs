using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using up6.db.database;

namespace up6.filemgr.app
{
    public class OracleExec : SqlExec
    {
        public OracleExec()
        {
            //初始化变量设置器
            this.m_pvSetter = new OracleParValSetter();
            this.m_parSetter = new OracleParamSetter();
            this.m_pc = new OracleParamCreater();
            this.m_cmdRd = new OracleCmdReader();
        }

        public override void exec(string table, string sql, string fields, string where, JObject o)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);
            var field_cdt = this.selFields(where, field_all);

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_pvSetter.setVal(cmd, field_sel, o);
            this.m_pvSetter.setVal(cmd, field_cdt, o);
            db.ExecuteNonQuery(cmd);
        }

        public override JToken exec(string table, string sql, string fields, string newNames = "")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);
            var names = newNames.Split(',');
            if (string.IsNullOrEmpty(newNames)) names = fields.Split(',');

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);

            var r = db.ExecuteReader(cmd);

            JArray a = new JArray();

            while (r.Read())
            {
                var o = new JObject();
                int i = 0;
                foreach (var f in field_sel)
                {
                    var name = names[i];
                    var type = f["type"].ToString().ToLower();
                    o[name] = this.m_cmdRd[type](r, i);
                    ++i;
                }
                a.Add(o);
            }
            r.Close();
            return JToken.FromObject(a);

        }

        public override void exec_batch(string table, string sql, string fields, string where, JToken values)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);
            var field_cdt = this.selFields(where, field_all);

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_pc.create(cmd, field_sel);
            this.m_pc.create(cmd, field_cdt);

            cmd.Connection.Open();
            cmd.Prepare();

            OracleValueSetter pvs = new OracleValueSetter();

            //设置条件
            foreach (var v in values)
            {
                pvs.setVal(cmd, field_sel, v);
                pvs.setVal(cmd, field_cdt, v);
                cmd.ExecuteNonQuery();
            }

            cmd.Connection.Close();
        }

        public override void update(string table, SqlParam[] fields, SqlParam[] where, string predicate = "and")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);
            var field_cdt = this.selFields(where, field_all);

            JObject o = new JObject();
            string sql = string.Format("update {0} set {1} where {2}"
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

            string sql = string.Format("update {0} set {1} where {2}"
                , table
                , this.toSqlSeter(fields)
                , this.toSqlSeter(where));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_pvSetter.setVal(cmd, field_sel, obj);
            this.m_pvSetter.setVal(cmd, field_cdt, obj);
            db.ExecuteNonQuery(cmd);
        }

        public override void update_batch(string table, string fields, SqlParam[] ws, JToken values, string predicate = "and")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);
            var field_cdt = this.selFields(ws, field_all);

            JObject o = new JObject();
            string sql = string.Format("update {0} set {1} where {2}"
                , table
                , this.toSqlSeter(fields)
                , this.toSqlCondition(ws, predicate));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_pc.create(cmd, field_sel);
            this.m_pc.create(cmd, field_cdt);
            cmd.Connection.Open();
            cmd.Prepare();

            OracleValueSetter pvs = new OracleValueSetter();

            //设置条件
            foreach (var v in values)
            {
                pvs.setVal(cmd, field_sel, v);
                pvs.setVal(cmd, field_cdt, v);
                cmd.ExecuteNonQuery();
            }

            cmd.Connection.Close();
        }

        public override void delete(string table, SqlParam[] where, string predicate = "and")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_cdt = this.selFields(where, field_all);

            JObject o = new JObject();
            string sql = string.Format("delete from {0} where {1}"
                , table
                , this.toSqlCondition(where, predicate));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_parSetter.setVal(cmd, field_cdt, where);
            db.ExecuteNonQuery(cmd);
        }

        public override void delete_batch(string table, SqlParam[] ws, JToken values, string predicate = "and")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(ws, field_all);

            JObject o = new JObject();
            string sql = string.Format("delete from {0} where {1}"
                , table
                , this.toSqlCondition(ws, predicate));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_parSetter.setVal(cmd, field_sel, ws);
            cmd.Connection.Open();
            cmd.Prepare();

            OracleValueSetter pvs = new OracleValueSetter();

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
            string sql = string.Format("select count(*) from {0} where {1}"
                , table
                , this.toSqlCondition(where, "and"));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_parSetter.setVal(cmd, field_cdt, where);
            var obj = db.ExecuteScalar(cmd);
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 添加数据，
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fields"></param>
        /// <param name="o">json字段名称必须和fields对应</param>
        public override void insert(string table, string fields, JObject o)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);

            string sql = string.Format("insert into {0} ( {1} ) values( {2} )"
                , table
                , fields
                , this.toSqlParam(fields));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_pvSetter.setVal(cmd, field_sel, o);
            db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 插入数据，返回自增主键ID
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="pars">字段键值对</param>
        public override void insert(string table, SqlParam[] pars)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(pars, field_all);

            string sql = string.Format("insert into {0} ( {1} ) values( {2} )"
                , table
                , this.toSqlFields(pars)
                , this.toSqlParam(pars));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_parSetter.setVal(cmd, field_sel, pars);
            db.ExecuteNonQuery(cmd);
        }

        public override JObject read(string table, string fields, SqlParam[] where)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);
            var field_cdt = this.selFields(where, field_all);

            JObject o = null;
            string sql = string.Format("select {0} from {1} where {2}"
                , this.selFieldNames(field_sel)
                , table
                , this.toSqlCondition(where, "and"));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_parSetter.setVal(cmd, field_cdt, where);

            OracleCmdReader scr = new OracleCmdReader();
            var r = db.ExecuteReader(cmd);
            if (r.Read())
            {
                o = this.m_cmdRd.read(r, field_sel);
            }
            r.Close();
            return o;
        }

        public override JObject read(string table, string fields, string where)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);

            if (!string.IsNullOrEmpty(where)) where = string.Format("where {0}", where);
            JObject o = null;
            string sql = string.Format("select {0} from {1} {2}"
                , this.selFieldNames(field_sel)
                , table
                , where);

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);

            OracleCmdReader scr = new OracleCmdReader();
            var r = db.ExecuteReader(cmd);
            if (r.Read())
            {
                o = this.m_cmdRd.read(r, field_sel);
            }
            r.Close();
            return o;
        }

        public override string toSqlFields(SqlParam[] ps)
        {
            var arr = from t in ps
                      select string.Format("{0}", t.Name);
            var name = string.Join(",", arr.ToArray());
            return name;
        }

        public override string selFieldNames(JToken o, string field = "name")
        {
            var arr = from t in o
                      select string.Format("{0}", t[field]);
            var name = string.Join(",", arr.ToArray());
            return name;
        }

        /// <summary>
        /// 转换成SQL赋值语句
        /// </summary>
        /// <param name="fields">字段列表</param>
        /// <returns></returns>
        public override string toSqlSeter(string fields)
        {
            var lst = fields.Split(',').ToList();
            var arr = from t in lst
                      select string.Format("{0}=:{0}", t);
            return string.Join(",", arr.ToArray());
        }

        /// <summary>
        /// 转换成SQL变量字段
        /// <para>:a,:b,:c</para>
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public override string toSqlParam(SqlParam[] ps)
        {
            var names = from t in ps
                        select ":" + t.Name;
            var name = string.Join(",", names.ToArray());
            return name;
        }

        /// <summary>
        /// 转换在SQL变量
        /// <para>:a,:b,:c</para>
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public override string toSqlParam(string fields)
        {
            var arr = fields.Split(',').ToList();
            var names = from a in arr
                        select string.Format(":{0}", a);
            var name = string.Join(",", names.ToArray());
            return name;
        }

        /// <summary>
        /// 转换成SQL赋值语句
        /// <para>a=:a,b=:b</para>
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="p">谓词</param>
        /// <returns></returns>
        public override string toSqlCondition(SqlParam[] ps, string pre = ",")
        {
            if (ps == null) return "1=1";

            var arr = from t in ps
                      select string.Format("{0}=:{0}", t.Name);
            var name = string.Join(" " + pre + " ", arr.ToArray());
            return name;
        }

        public override JToken select(string table, string fields, SqlParam[] where, string sort = "")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);
            var field_cdt = this.selFields(where, field_all);

            //防止字段名称冲突
            var fns_sql = from f in field_sel
                          select f["name"].ToString();
            fields = string.Join(",", fns_sql.ToArray());

            string sql_where = "";
            if (where.Length > 0) sql_where = string.Format("where {0}", this.toSqlCondition(where, "and"));

            if (!string.IsNullOrEmpty(sort)) sort = string.Format(" order by {0}", sort);
            string sql = string.Format("select {0} from {1} {2} {3}"
                , fields
                , table
                , sql_where
                , sort);

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_parSetter.setVal(cmd, field_cdt, where);
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

        public override JToken select(string table, string fields, string where, string sort = "")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);

            //防止字段名称冲突
            var fns_sql = from f in field_sel
                          select f["name"].ToString();
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

        public override JToken selectUnion(string[] tables, string fields, string where)
        {
            this.m_table = this.table(tables[0]);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);

            List<string> sels = new List<string>();
            SqlBuilder sb = new SqlBuilder();
            foreach (var t in tables)
            {
                sels.Add(sb.select(t, fields, where));
            }
            var sql = string.Join(" union ", sels.ToArray());

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            var r = db.ExecuteReader(cmd);

            JArray arr = new JArray();
            while (r.Read())
            {
                var o = this.m_cmdRd.read(r, field_sel);
                arr.Add(o);
            }
            r.Close();
            return JToken.FromObject(arr);
        }

        public override JToken selectUnion(string[] tables, string fields, SqlParam[] where)
        {
            this.m_table = this.table(tables[0]);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);
            var field_cdt = this.selFields(where, field_all);

            List<string> sels = new List<string>();
            SqlBuilder sb = new SqlBuilder();
            foreach (var t in tables)
            {
                sels.Add(sb.select(t, fields, this.toSqlCondition(where, "and")));
            }
            var sql = string.Join(" union ", sels.ToArray());

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_parSetter.setVal(cmd, field_cdt, where);
            var r = db.ExecuteReader(cmd);

            JArray arr = new JArray();
            while (r.Read())
            {
                var o = this.m_cmdRd.read(r, field_sel);
                arr.Add(o);
            }
            r.Close();
            return JToken.FromObject(arr);
        }
    }
}