﻿using Newtonsoft.Json.Linq;
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
        JToken m_database;
        JToken m_table;

        SqlParValSetter m_pvSetter;
        SqlParamSetter m_parSetter;
        SqlCmdReader m_cmdRd;

        public SqlExec() {
            ConfigReader cr = new ConfigReader();
            this.m_database = cr.module("database");

            //初始化变量设置器
            this.m_pvSetter = new SqlParValSetter();
            this.m_parSetter = new SqlParamSetter();
            this.m_cmdRd = new SqlCmdReader();

        }

        /// <summary>
        /// 添加数据，
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fields"></param>
        /// <param name="o">json字段名称必须和fields对应</param>
        public int insert(string table,string fields,JObject o)
        {
            //加载结构
            this.m_table = this.m_database.SelectToken(table);
            var identity = this.m_table.SelectToken("fields[?(@.identity==true && @.primary==true)]");

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
            this.to_param_db(cmd, fields,o);
            var id = db.ExecuteScalar(cmd);
            return Convert.ToInt32(id);
        }

        /// <summary>
        /// 插入数据，返回自增主键ID
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="pars">字段键值对</param>
        public int insert(string table, SqlParam[] pars) {
            //加载结构
            this.m_table = this.m_database.SelectToken(table);
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
            this.to_param_db(cmd, pars,field_object);
            var id = db.ExecuteScalar(cmd);
            return Convert.ToInt32(id);
        }

        public JObject read(string table,string fields,SqlParam[] where)
        {
            //加载结构
            this.m_table = this.m_database.SelectToken(table);
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
                ,this.to_fields(field_object)
                ,table
                ,this.to_condition(where,"and"));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.to_param_db(cmd, where);

            var r = db.ExecuteReader(cmd);
            if (r.Read())
            {
                o = new JObject();
                int index = 0;
                foreach (var field in arr)
                {
                    var fd = field_object[index];
                    var fd_type = fd["type"].ToString().ToLower();

                    if (string.Equals(fd_type, "string"))
                    {
                        o[field] = r.IsDBNull(index) ? string.Empty : r.GetString(index);
                    }
                    else if (string.Equals(fd_type, "int"))
                    {
                        o[field] = r.IsDBNull(index) ? 0 : r.GetInt32(index);
                    }
                    else if (string.Equals(fd_type, "datetime"))
                    {
                        o[field] = r.IsDBNull(index) ? DateTime.Now : r.GetDateTime(index);
                    }
                    else if (string.Equals(fd_type, "long"))
                    {
                        o[field] = r.IsDBNull(index) ? 0 : r.GetInt64(index);
                    }
                    else if (string.Equals(fd_type, "smallint"))
                    {
                        o[field] = r.IsDBNull(index) ? 0 : r.GetInt16(index);
                    }
                    else if (string.Equals(fd_type, "tinyint"))
                    {
                        o[field] = r.IsDBNull(index) ? 0 : r.GetByte(index);
                    }
                    ++index;//索引后移
                }
            }
            r.Close();
            return o;
        }

        public void update(string table, SqlParam[] fields, SqlParam[] where)
        {
            //加载结构
            this.m_table = this.m_database.SelectToken(table);

            JObject o = new JObject();
            string sql = string.Format("update [{0}] set {1} where {2}"
                ,table
                ,this.to_condition(fields)
                ,this.to_condition(where));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.to_param_db(cmd, fields);
            this.to_param_db(cmd, where);
            db.ExecuteNonQuery(cmd);
        }

        public void update(string table,string fields,string where,JObject obj)
        {
            //加载结构
            this.m_table = this.m_database.SelectToken(table);

            string sql = string.Format("update [{0}] set {1} where {2}"
                , table
                , this.to_assignment(fields)
                , this.to_assignment(where));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.to_param_db(cmd, fields, obj);
            this.to_param_db(cmd, where, obj);
            db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        string to_assignment(string fields)
        {
            var lst = fields.Split(',').ToList();
            var arr = from t in lst
                      select string.Format("{0}=@{0}", t);
            return string.Join(",", arr.ToArray());
        }

        public void delete(string table,SqlParam[] where)
        {
            //加载结构
            this.m_table = this.m_database.SelectToken(table);

            JObject o = new JObject();
            string sql = string.Format("delete from [{0}] where {1}"
                , table
                , this.to_condition(where));

            DbHelper db = new DbHelper();
            var cmd = db.GetCommand(sql);
            this.to_param_db(cmd, where);
            db.ExecuteNonQuery(cmd);
        }

        public int count(string table,SqlParam[] where)
        {
            //加载结构
            this.m_table = this.m_database.SelectToken(table);

            JObject o = new JObject();
            string sql = string.Format("select count(*) from [{0}] where {1}"
                , table
                , this.to_condition(where));

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
        public string to_fields(JToken o, string field="name") {
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
                        select "@"+t.Name;
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
        public string to_condition(SqlParam[] ps,string pre=",")
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
        public void to_param_db(DbCommand cmd, SqlParam[] ps,JToken field_obj)
        {
            if (null == ps) return;

            var fs = from p in ps
                     join fo in field_obj
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
        /// <param name="fields"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public void to_param_db(DbCommand cmd,string fields, JObject obj)
        {
            var field_object = this.m_table.SelectToken("fields");
            var field_names = fields.Split(',').ToList();
            var field_obj = from f in field_names
                            join fo in field_object
                            on f equals fo["name"].ToString()
                            select fo;

            foreach (var o in field_obj)
            {
                var fd_type = o["type"].ToString().ToLower();

                this.m_pvSetter[fd_type](cmd, obj, o);
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
        public JToken select(string table,string fields,SqlParam[] where,string sort="")
        {
            //加载结构
            this.m_table = this.m_database.SelectToken(table);
            var fields_json = this.m_table.SelectToken("fields");

            string[] arr = fields.Split(',');

            if (string.Equals(fields, "*"))
            {
                var fns = from f in fields_json select f["name"].ToString();
                arr = fns.ToArray();
            }//指定了字段
            else
            {
                List<string> fdArr = new List<string>(arr);
                var fns = from f in fields_json
                          where fdArr.Contains(f["name"].ToString())
                          select f;
                fields_json = JToken.FromObject(fns);
            }

            //防止字段名称冲突
            var fns_sql = from f in fields_json
                          select "[" + f["name"].ToString() + "]";
            fields = string.Join(",", fns_sql.ToArray());

            string sql = string.Format("select {0} from {1} where {2}"
                , fields
                , table
                , this.to_condition(where));
            //有排序
            if (!string.IsNullOrEmpty(sort)) {
                sql = string.Format("select {0} from {1} where {2} order by {3}"
                , fields
                , table
                , this.to_condition(where)
                ,sort);
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
                foreach (var field in arr)
                {
                    var fd = fields_json[index];
                    var fd_type = fd["type"].ToString().ToLower();
                    o[field] = this.m_cmdRd[fd_type](r, index++);
                }
                a.Add(o);
            }
            r.Close();
            return JToken.FromObject(a);
        }
    }
}