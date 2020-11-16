using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using up6.db.database;

namespace up6.filemgr.app
{
    /*
     *插入数据示例：
     * SqlExec se = new SqlExec();
            se.insert("case", new SqlParam[] {
                new SqlParam("name","test")
            });
     *
     * 读取数据示例
     * SqlExec se = new SqlExec();
            var o = se.read("case", "*", new SqlParam[] {
                new SqlParam("id",2)
            });
        
        更新示例
            SqlExec se = new SqlExec();
            se.update("case"
                , new SqlParam[] {
                    new SqlParam("name","我是谁？")
                }
                , new SqlParam[] {
                new SqlParam("id",2)
            });

        删除示例
            SqlExec se = new SqlExec();
            se.delete("case"
                , new SqlParam[] {
                new SqlParam("id",2)
            });

        统计示例
            SqlExec se = new SqlExec();
             Response.Write( se.count("case",null) );

     */
    /// <summary>
    /// 说明：
    /// 1.需要提供数据库表结构json
    /// </summary>
    public class SqlExec
    {
        protected JToken m_table;

        protected SqlParamCreater m_pc;
        protected SqlParValSetter m_pvSetter;
        protected SqlParamSetter m_parSetter;
        protected SqlCmdReader m_cmdRd;

        public SqlExec()
        {
            //初始化变量设置器
            this.m_pvSetter = new SqlParValSetter();
            this.m_parSetter = new SqlParamSetter();
            this.m_pc = new SqlParamCreater();
            this.m_cmdRd = new SqlCmdReader();
        }

        public JToken table(string tableName) {

            ConfigReader cr = new ConfigReader();
            var o = cr.module(string.Format("database.{0}", tableName));
            return o;
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="fields"></param>
        /// <param name="where"></param>
        /// <param name="o"></param>
        public virtual void exec(string table, string sql, string fields, string where, JObject o)
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

        public object exec(string sql)
        {
            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            return db.ExecuteScalar(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sql"></param>
        /// <param name="fields">字段名称</param>
        /// <param name="newNames">重新命名的字段名称</param>
        /// <returns></returns>
        public virtual JToken exec(string table, string sql, string fields, string newNames = "")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);
            var names = newNames.Split(',');
            if (string.IsNullOrEmpty(newNames))
            {
                var ns = from t in field_sel
                         select t["name"].ToString();
                names = ns.ToArray();
            }

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

        /// <summary>
        /// 批量SQL语句
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sql"></param>
        /// <param name="fields"></param>
        /// <param name="where"></param>
        /// <param name="values">字段值列表</param>
        public virtual void exec_batch(string table, string sql, string fields, string where, JToken values)
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

            SqlValueSetter pvs = new SqlValueSetter();

            //设置条件
            foreach (var v in values)
            {
                pvs.setVal(cmd, field_sel, v);
                pvs.setVal(cmd, field_cdt, v);
                cmd.ExecuteNonQuery();
            }

            cmd.Connection.Close();
        }

        /// <summary>
        /// 添加数据，
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fields"></param>
        /// <param name="o">json字段名称必须和fields对应</param>
        public virtual void insert(string table, string fields, JObject o)
        {
            //加载结构
            this.m_table = this.table(table);
            var identity = this.m_table.SelectToken("fields[?(@.identity==true && @.primary==true)]");
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);

            string sql = string.Format("insert into [{0}] ( {1} ) values( {2} );"
                , table
                , fields
                , this.toSqlParam(fields));
            //有标识主键
            if (identity != null)
            {
                sql += "select @@IDENTITY;";
            }

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_pvSetter.setVal(cmd, field_sel, o);
            var id = db.ExecuteScalar(cmd);
            //return Convert.ToInt32(id);
        }

        /// <summary>
        /// 插入数据，返回自增主键ID
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="pars">字段键值对</param>
        public virtual void insert(string table, SqlParam[] pars)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(pars, field_all);
            var identity = field_all.SelectToken("[?(@.identity==true && @.primary==true)]");

            string sql = string.Format("insert into [{0}] ( {1} ) values( {2} );"
                , table
                , this.toSqlFields(pars)
                , this.toSqlParam(pars));

            //有标识主键
            if (identity != null)
            {
                sql += "select @@IDENTITY;";
            }

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_parSetter.setVal(cmd, field_sel, pars);
            var id = db.ExecuteScalar(cmd);
            //return Convert.ToInt32(id);
        }

        public virtual JObject read(string table, string fields, SqlParam[] where)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);
            var field_cdt = this.selFields(where, field_all);

            JObject o = null;
            string sql = string.Format("select {0} from [{1}] where {2}"
                , this.selFieldNames(field_sel)
                , table
                , this.toSqlCondition(where, "and"));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_parSetter.setVal(cmd, field_cdt, where);

            SqlCmdReader scr = new SqlCmdReader();
            var r = db.ExecuteReader(cmd);
            if (r.Read())
            {
                o = this.m_cmdRd.read(r, field_sel);
            }
            r.Close();
            return o;
        }

        public virtual JObject read(string table, string fields, string where)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);

            if (!string.IsNullOrEmpty(where)) where = string.Format("where {0}", where);
            JObject o = null;
            string sql = string.Format("select {0} from [{1}] {2}"
                , this.selFieldNames(field_sel)
                , table
                , where);

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);

            SqlCmdReader scr = new SqlCmdReader();
            var r = db.ExecuteReader(cmd);
            if (r.Read())
            {
                o = this.m_cmdRd.read(r, field_sel);
            }
            r.Close();
            return o;
        }

        public virtual void update(string table, SqlParam[] fields, SqlParam[] where, string predicate = "and")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);
            var field_cdt = this.selFields(where, field_all);

            JObject o = new JObject();
            string sql = string.Format("update [{0}] set {1} where {2}"
                , table
                , this.toSqlCondition(fields)
                , this.toSqlCondition(where, predicate));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_parSetter.setVal(cmd, field_sel, fields);
            this.m_parSetter.setVal(cmd, field_cdt, where);
            db.ExecuteNonQuery(cmd);
        }

        public virtual void update(string table, string fields, string where, JObject obj)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);
            var field_cdt = this.selFields(where, field_all);

            string sql = string.Format("update [{0}] set {1} where {2}"
                , table
                , this.toSqlSeter(fields)
                , this.toSqlSeter(where));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_pvSetter.setVal(cmd, field_sel, obj);
            this.m_pvSetter.setVal(cmd, field_cdt, obj);
            db.ExecuteNonQuery(cmd);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field">更新的字段列表</param>
        /// <param name="ws">条件</param>
        /// <param name="values">值，[{id:1},{id,2}]</param>
        /// <param name="predicate"></param>
        public virtual void update_batch(string table, string fields, SqlParam[] ws, JToken values, string predicate = "and")
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

            SqlValueSetter pvs = new SqlValueSetter();

            //设置条件
            foreach (var v in values)
            {
                pvs.setVal(cmd, field_sel, v);
                pvs.setVal(cmd, field_cdt, v);
                cmd.ExecuteNonQuery();
            }

            cmd.Connection.Close();
        }

        /// <summary>
        /// 转换成SQL赋值语句
        /// </summary>
        /// <param name="fields">字段列表</param>
        /// <returns></returns>
        public virtual string toSqlSeter(string fields)
        {
            var lst = fields.Split(',').ToList();
            var arr = from t in lst
                      select string.Format("{0}=@{0}", t);
            return string.Join(",", arr.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        /// <param name="predicate">连接词：and,or</param>
        public virtual void delete(string table, SqlParam[] where, string predicate = "and")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_cdt = this.selFields(where, field_all);

            JObject o = new JObject();
            string sql = string.Format("delete from [{0}] where {1}"
                , table
                , this.toSqlCondition(where, predicate));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_parSetter.setVal(cmd, field_cdt, where);            
            db.ExecuteNonQuery(cmd);
        }

        public void delete(string table, string where)
        {
            JObject o = new JObject();
            string sql = string.Format("delete from [{0}] where {1}"
                , table
                , where);

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="table"></param>
        /// <param name="ws">条件</param>
        /// <param name="values">值，[{id:1},{id,2}]</param>
        /// <param name="predicate"></param>
        public virtual void delete_batch(string table, SqlParam[] ws, JToken values, string predicate = "and")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(ws, field_all);

            JObject o = new JObject();
            string sql = string.Format("delete from [{0}] where {1}"
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

        public virtual int count(string table, SqlParam[] where)
        {
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_cdt = this.selFields(where, field_all);

            JObject o = new JObject();
            string sql = string.Format("select count(*) from [{0}] where {1}"
                , table
                , this.toSqlCondition(where, "and"));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.m_parSetter.setVal(cmd, field_cdt, where);
            var obj = db.ExecuteScalar(cmd);
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 转换成SQL字段
        /// <para>a,b,c,d,e,f,g</para>
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public virtual string toSqlFields(SqlParam[] ps)
        {
            var arr = from t in ps
                      select string.Format("[{0}]", t.Name);
            var name = string.Join(",", arr.ToArray());
            return name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="field">JSON中的字段名称</param>
        /// <returns></returns>
        public virtual string selFieldNames(JToken o, string field = "name")
        {
            var arr = from t in o
                      select string.Format("[{0}]", t[field]);
            var name = string.Join(",", arr.ToArray());
            return name;
        }

        /// <summary>
        /// 转换成SQL变量字段
        /// <para>@a,@b,@c</para>
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public virtual string toSqlParam(SqlParam[] ps)
        {
            var names = from t in ps
                        select "@" + t.Name;
            var name = string.Join(",", names.ToArray());
            return name;
        }

        /// <summary>
        /// 转换在SQL变量
        /// <para>@a,@b,@c</para>
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public virtual string toSqlParam(string fields)
        {
            var arr = fields.Split(',').ToList();
            var names = from a in arr
                        select string.Format("@{0}", a);
            var name = string.Join(",", names.ToArray());
            return name;
        }

        /// <summary>
        /// 转换成SQL赋值语句
        /// <para>a=@a,b=@b</para>
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="p">谓词</param>
        /// <returns></returns>
        public virtual string toSqlCondition(SqlParam[] ps, string pre = ",")
        {
            if (ps == null) return "1=1";

            var arr = from t in ps
                      select string.Format("{0}=@{0}", t.Name);
            var name = string.Join(" " + pre + " ", arr.ToArray());
            return name;
        }

        /// <summary>
        /// 选择多条数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fields">字段列表a,b,c,d,e或者所有字段：*</param>
        /// <param name="where"></param>
        /// <param name="sort">排序。示例：time desc</param>
        /// <returns></returns>
        public virtual JToken select(string table, string fields, SqlParam[] where, string sort = "")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);
            var field_cdt = this.selFields(where, field_all);

            //防止字段名称冲突
            var fns_sql = from f in field_sel
                          select "[" + f["name"].ToString() + "]";
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

        /// <summary>
        /// 选择多条数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fields">字段列表a,b,c,d,e或者所有字段：*</param>
        /// <param name="where"></param>
        /// <param name="sort">排序。示例：time desc</param>
        /// <returns></returns>
        public virtual JToken select(string table, string fields, string where, string sort = "")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);

            //防止字段名称冲突
            var fns_sql = from f in field_sel
                          select "[" + f["name"].ToString() + "]";
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

        public virtual JToken selectUnion(string[] tables, string fields, string where)
        {
            this.m_table = this.table(tables[0]);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);

            List<string> sels = new List<string>();
            SqlBuilder sb = new SqlBuilder();
            foreach(var t in tables)
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

        public virtual JToken selectUnion(string[] tables, string fields, SqlParam[] where)
        {
            this.m_table = this.table(tables[0]);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.selFields(fields, field_all);
            var field_cdt = this.selFields(where, field_all);

            List<string> sels = new List<string>();
            SqlBuilder sb = new SqlBuilder();
            foreach (var t in tables)
            {
                sels.Add(sb.select(t, fields, this.toSqlCondition(where,"and")) );
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

        /// <summary>
        /// 批量获取字段结构信息
        /// </summary>
        /// <param name="names">字段名称列表</param>
        /// <param name="field_all"></param>
        /// <returns></returns>
        protected JToken selFields(string names, JToken field_all)
        {
            if (names == "*") return field_all;

            var data = from n in names.Split(',')
                       join f in field_all
                       on n.Trim() equals f["name"].ToString()
                       select f;
            return JToken.FromObject(data);
        }
        protected JToken selFields(SqlParam[] sp, JToken field_all)
        {
            if (sp.Length < 1) return null;
            var data = from n in sp
                       join f in field_all
                       on n.Name equals f["name"].ToString()
                       select f;
            return JToken.FromObject(data);
        }
    }
}