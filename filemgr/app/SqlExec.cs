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
        JToken m_table;

        SqlParamCreater m_pc;
        SqlParValSetter m_pvSetter;
        SqlParamSetter m_parSetter;
        SqlCmdReader m_cmdRd;

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
        public void exec(string table, string sql, string fields, string where, JObject o)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.to_param_db(cmd, fields, o, field_all);
            this.to_param_db(cmd, where, o, field_all);
            db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sql"></param>
        /// <param name="fields">字段名称</param>
        /// <param name="newNames">重新命名的字段名称</param>
        /// <returns></returns>
        public JToken exec(string table, string sql, string fields, string newNames = "")
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");
            var field_sel = this.from_fields(fields, field_all);
            var names = newNames.Split(',');
            if (string.IsNullOrEmpty(newNames)) names = fields.Split(',');

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);


            var r = db.ExecuteReader(cmd);

            JArray a = new JArray();

            while (r.Read())
            {
                int index = 0;
                var o = new JObject();
                foreach (var field in field_sel)
                {
                    var fd = field_sel[index];
                    var field_name = field["name"].ToString();
                    if (names.Length > 0) field_name = names[index];
                    var fd_type = fd["type"].ToString().ToLower();
                    o[field_name] = this.m_cmdRd[fd_type](r, index++);
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
        public void exec_batch(string table, string sql, string fields, string where, JToken values)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");

            var field_sels = from f in fields.Split(',')
                             join field in field_all
                             on f.Trim() equals field["name"].ToString()
                             select field;

            var field_where_sels = from f in @where.Split(',')
                                   join field in field_all
                                   on f.Trim() equals field["name"].ToString()
                                   select field;

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.to_param_db(cmd, JToken.FromObject(field_sels));
            this.to_param_db(cmd, JToken.FromObject(field_where_sels));

            cmd.Connection.Open();
            cmd.Prepare();

            SqlValueSetter pvs = new SqlValueSetter();

            //设置条件
            foreach (var v in values)
            {
                //设置变量
                foreach (var w in field_sels)
                {
                    pvs[w["type"].ToString()](cmd, w, v);
                }

                //设置值
                foreach (var w in field_where_sels)
                {
                    pvs[w["type"].ToString()](cmd, w, v);
                }
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
        public int insert(string table, string fields, JObject o)
        {
            //加载结构
            this.m_table = this.table(table);
            var identity = this.m_table.SelectToken("fields[?(@.identity==true && @.primary==true)]");
            var field_all = this.m_table.SelectToken("fields");

            string sql = string.Format("insert into [{0}] ( {1} ) values( {2} );"
                , table
                , fields
                , this.to_param(fields));
            //有标识主键
            if (identity != null)
            {
                sql += "select @@IDENTITY;";
            }

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.to_param_db(cmd, fields, o, field_all);
            var id = db.ExecuteScalar(cmd);
            return Convert.ToInt32(id);
        }

        /// <summary>
        /// 插入数据，返回自增主键ID
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="pars">字段键值对</param>
        public int insert(string table, SqlParam[] pars)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_object = this.m_table.SelectToken("fields");
            var identity = field_object.SelectToken("[?(@.identity==true && @.primary==true)]");

            string sql = string.Format("insert into [{0}] ( {1} ) values( {2} );"
                , table
                , this.to_fields(pars)
                , this.to_param(pars));

            //有标识主键
            if (identity != null)
            {
                sql += "select @@IDENTITY;";
            }

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.to_param_db(cmd, pars, field_object);
            var id = db.ExecuteScalar(cmd);
            return Convert.ToInt32(id);
        }

        public JObject read(string table, string fields, SqlParam[] where)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_object = this.m_table.SelectToken("fields");

            string[] arr = fields.Split(',');

            //所有字段
            if (string.Equals(fields, "*"))
            {
                var fns = from f in field_object select f["name"].ToString();
                arr = fns.ToArray();
            }//指定字段
            else
            {
                var field_list = fields.Split(',').ToList();
                List<string> fdArr = new List<string>(arr);
                var fns = from f in field_list
                          join fo in field_object
                          on f equals fo["name"].ToString()
                          select fo;
                field_object = JToken.FromObject(fns);
            }

            JObject o = null;
            string sql = string.Format("select {0} from [{1}] where {2}"
                , this.to_fields(field_object)
                , table
                , this.to_condition(where, "and"));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.to_param_db(cmd, where);

            SqlCmdReader scr = new SqlCmdReader();
            var r = db.ExecuteReader(cmd);
            if (r.Read())
            {
                o = new JObject();
                int index = 0;
                foreach (var field in arr)
                {
                    var fd = field_object[index];
                    o[field] = scr[fd["type"].ToString()](r, index++);
                }
            }
            r.Close();
            return o;
        }

        public void update(string table, SqlParam[] fields, SqlParam[] where, string predicate = "and")
        {
            //加载结构
            this.m_table = this.table(table);

            JObject o = new JObject();
            string sql = string.Format("update [{0}] set {1} where {2}"
                , table
                , this.to_condition(fields)
                , this.to_condition(where, predicate));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.to_param_db(cmd, fields);
            this.to_param_db(cmd, where);
            db.ExecuteNonQuery(cmd);
        }

        public void update(string table, string fields, string where, JObject obj)
        {
            //加载结构
            this.m_table = this.table(table);
            var field_all = this.m_table.SelectToken("fields");

            string sql = string.Format("update [{0}] set {1} where {2}"
                , table
                , this.to_assignment(fields)
                , this.to_assignment(where));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.to_param_db(cmd, fields, obj, field_all);
            this.to_param_db(cmd, where, obj, field_all);
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
        public void update_batch(string table, string fields, SqlParam[] ws, JToken values, string predicate = "and")
        {
            //加载结构
            this.m_table = this.table(table);
            var table_fields = this.m_table.SelectToken("fields");
            var field_whers = (from w in ws
                               join tf in table_fields
                               on w.Name.Trim() equals tf["name"].ToString()
                               select tf).ToArray();

            var field_sels = (from w in fields.Split(',')
                              join tf in table_fields
                              on w.Trim() equals tf["name"].ToString()
                              select tf).ToArray();

            JObject o = new JObject();
            string sql = string.Format("update {0} set {1} where {2}"
                , table
                , this.to_assignment(fields)
                , this.to_condition(ws, predicate));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.to_param_db(cmd, JToken.FromObject(field_sels));
            this.to_param_db(cmd, ws);
            cmd.Connection.Open();
            cmd.Prepare();

            SqlValueSetter pvs = new SqlValueSetter();

            //设置条件
            foreach (var v in values)
            {
                //设置变量
                int index = 0;
                foreach (var w in ws)
                {
                    pvs[w.Type](cmd, field_whers[index++], v);
                }

                //设置值
                index = 0;
                foreach (var w in field_sels)
                {
                    pvs[w["type"].ToString()](cmd, w, v);
                }
                cmd.ExecuteNonQuery();
            }

            cmd.Connection.Close();
        }

        /// <summary>
        /// 转成赋值语句
        /// </summary>
        /// <param name="fields">字段列表</param>
        /// <returns></returns>
        string to_assignment(string fields)
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
        public void delete(string table, SqlParam[] where, string predicate = "and")
        {
            //加载结构
            this.m_table = this.table(table);

            JObject o = new JObject();
            string sql = string.Format("delete from [{0}] where {1}"
                , table
                , this.to_condition(where, predicate));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.to_param_db(cmd, where);
            db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="table"></param>
        /// <param name="ws">条件</param>
        /// <param name="values">值，[{id:1},{id,2}]</param>
        /// <param name="predicate"></param>
        public void delete_batch(string table, SqlParam[] ws, JToken values, string predicate = "and")
        {

            //加载结构
            this.m_table = this.table(table);
            var table_fields = this.m_table.SelectToken("fields");
            var field_sel = (from w in ws
                             join tf in table_fields
                             on w.Name.Trim() equals tf["name"].ToString()
                             select tf).ToArray();

            JObject o = new JObject();
            string sql = string.Format("delete from [{0}] where {1}"
                , table
                , this.to_condition(ws, predicate));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.to_param_db(cmd, ws);
            cmd.Connection.Open();
            cmd.Prepare();

            SqlValueSetter pvs = new SqlValueSetter();

            foreach (var v in values)
            {
                //设置变量
                int index = 0;
                foreach (var w in ws)
                {
                    pvs[w.Type](cmd, field_sel[index++], v);
                }
                cmd.ExecuteNonQuery();
            }

            cmd.Connection.Close();
        }

        public int count(string table, SqlParam[] where)
        {
            //加载结构
            this.m_table = this.table(table);

            JObject o = new JObject();
            string sql = string.Format("select count(*) from [{0}] where {1}"
                , table
                , this.to_condition(where, "and"));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.to_param_db(cmd, where);
            var obj = db.ExecuteScalar(cmd);
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 转换成字段名称列表
        /// <para>a,b,c,d,e,f,g</para>
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public string to_fields(SqlParam[] ps)
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
        public string to_fields(JToken o, string field = "name")
        {
            var arr = from t in o
                      select string.Format("[{0}]", t[field]);
            var name = string.Join(",", arr.ToArray());
            return name;
        }

        /// <summary>
        /// 拼接成变量
        /// <para>@a,@b,@c</para>
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public string to_param(SqlParam[] ps)
        {
            var names = from t in ps
                        select "@" + t.Name;
            var name = string.Join(",", names.ToArray());
            return name;
        }

        /// <summary>
        /// 拼接成变量
        /// <para>@a,@b,@c</para>
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public string to_param(string fields)
        {
            var arr = fields.Split(',').ToList();
            var names = from a in arr
                        select string.Format("@{0}", a);
            var name = string.Join(",", names.ToArray());
            return name;
        }

        /// <summary>
        /// 拼装成赋值
        /// <para>a=@a,b=@b</para>
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="p">谓词</param>
        /// <returns></returns>
        public string to_condition(SqlParam[] ps, string pre = ",")
        {
            if (ps == null) return "1=1";

            var arr = from t in ps
                      select string.Format("{0}=@{0}", t.Name);
            var name = string.Join(" " + pre + " ", arr.ToArray());
            return name;
        }

        /// <summary>
        /// 转换成数据库变量
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="ps"></param>
        public void to_param_db(DbCommand cmd, SqlParam[] ps, JToken field_all)
        {
            if (null == ps) return;

            var fs = from p in ps
                     join fo in field_all
                     on p.Name equals fo["name"].ToString()
                     select fo;
            var field_arr = fs.ToArray();

            var index = 0;
            foreach (var p in ps)
            {
                var fd = field_arr[index++];
                var fd_type = fd["type"].ToString().ToLower();
                this.m_parSetter[fd_type](cmd, p, fd);
            }
        }

        /// <summary>
        /// 转换成数据库变量
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="ps"></param>
        public void to_param_db(DbCommand cmd, SqlParam[] ps)
        {
            if (null == ps) return;

            var field_objects = this.m_table.SelectToken("fields");
            var res = from p in ps
                      join field in field_objects
                      on p.Name equals field["name"].ToString()
                      select field;

            var field_curs = res.ToArray();

            for (int i = 0; i < ps.Length; i++)
            {
                var fd_type = field_curs[i]["type"].ToString();

                this.m_parSetter[fd_type](cmd, ps[i], field_curs[i]);
            }
        }

        /// <summary>
        /// 为cmd添加字段,按照fields顺序
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="fields">字段列表</param>
        /// <param name="obj">字段的值，name:"",age:""</param>
        /// <param name="field_all">所有字段结构信息</param>
        /// <returns></returns>
        public void to_param_db(DbCommand cmd, string fields, JObject obj, JToken field_all)
        {
            var field_names = fields.Split(',').ToList();
            var field_obj = from f in field_names
                            join fo in field_all
                            on f equals fo["name"].ToString()
                            select fo;

            foreach (var o in field_obj)
            {
                var fd_type = o["type"].ToString().ToLower();

                this.m_pvSetter[fd_type](cmd, obj, o);
            }
        }

        /// <summary>
        /// 为cmd创建变量
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="fields">选择的字段列表</param>
        public void to_param_db(DbCommand cmd, JToken fields)
        {
            foreach (var o in fields)
            {
                var fd_type = o["type"].ToString().ToLower();

                this.m_pc[fd_type](cmd, o);
            }
        }

        /// <summary>
        /// 选择多条数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fields">字段列表a,b,c,d,e或者所有字段：*</param>
        /// <param name="where"></param>
        /// <param name="sort">排序。示例：time desc</param>
        /// <returns></returns>
        public JToken select(string table, string fields, SqlParam[] where, string sort = "")
        {
            //加载结构
            this.m_table = this.table(table);
            var fields_all = this.m_table.SelectToken("fields");

            string[] field_names = fields.Split(',');

            if (string.Equals(fields, "*"))
            {
                var fns = from f in fields_all select f["name"].ToString();
                field_names = fns.ToArray();
            }//指定了字段
            else
            {
                var field_sels = from fn in field_names.ToList()
                                 join item in fields_all
                                 on fn.Trim() equals item["name"].ToString().Trim()
                                 select item;
                fields_all = JToken.FromObject(field_sels.ToArray());
            }

            //防止字段名称冲突
            var fns_sql = from f in fields_all
                          select "[" + f["name"].ToString() + "]";
            fields = string.Join(",", fns_sql.ToArray());

            string sql = string.Format("select {0} from {1} where {2}"
                , fields
                , table
                , this.to_condition(where, "and"));
            //有排序
            if (!string.IsNullOrEmpty(sort))
            {
                sql = string.Format("select {0} from {1} where {2} order by {3}"
                , fields
                , table
                , this.to_condition(where, "and")
                , sort);
            }

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.to_param_db(cmd, where);
            var r = db.ExecuteReader(cmd);

            JArray a = new JArray();

            while (r.Read())
            {
                int index = 0;
                var o = new JObject();
                foreach (var field in field_names)
                {
                    var fd = fields_all[index];
                    var fd_type = fd["type"].ToString().ToLower();
                    o[field] = this.m_cmdRd[fd_type](r, index++);
                }
                a.Add(o);
            }
            r.Close();
            return JToken.FromObject(a);
        }

        /// <summary>
        /// 批量获取字段结构信息
        /// </summary>
        /// <param name="names">字段名称列表</param>
        /// <param name="field_all"></param>
        /// <returns></returns>
        JToken from_fields(string names, JToken field_all)
        {
            var data = from n in names.Split(',')
                       join f in field_all
                       on n.Trim() equals f["name"].ToString()
                       select f;
            return JToken.FromObject(data);
        }
    }
}