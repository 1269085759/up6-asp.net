﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace up6.filemgr.app
{
    /// <summary>
    /// CMD值设置器
    /// <para>自动从字段信息中取名称，根据名称取值</para>
    /// </summary>
    public class SqlParValSetter
    {
        public delegate void setterDelegate(DbCommand cmd, JToken fieldVal, JToken fieldInf);
        protected Dictionary<string, setterDelegate> m_map;

        public setterDelegate this[string index]
        {
            get { return this.m_map[index]; }
        }

        public void setVal(DbCommand cmd,JToken fields,JToken val)
        {
            foreach (var f in fields)
            {
                var type = f["type"].ToString().ToLower();
                this.m_map[type](cmd, val, f);
            }
        }

        public SqlParValSetter()
        {
            this.m_map = new Dictionary<string, setterDelegate>() {
                { "string",(DbCommand cmd,JToken val, JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"];
                    p.DbType = DbType.String;
                    p.Size = Convert.ToInt32(field["length"]);
                    p.Value = val[field["name"].ToString()];
                    cmd.Parameters.Add(p);
                } }
                ,{ "int",(DbCommand cmd,JToken val,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"];
                    p.DbType = DbType.Int32;
                    p.Value = val[field["name"].ToString()];
                    cmd.Parameters.Add(p);
                } }
                ,{ "datetime",(DbCommand cmd,JToken val,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"];
                    p.DbType = DbType.DateTime;
                    p.Value = val[field["name"].ToString()];
                    cmd.Parameters.Add(p);
                } }
                ,{ "long",(DbCommand cmd,JToken val,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"];
                    p.DbType = DbType.Int64;
                    p.Value = val[field["name"].ToString()];
                    cmd.Parameters.Add(p);
                } }
                ,{ "double",(DbCommand cmd,JToken val,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"];
                    p.DbType = DbType.Double;
                    p.Value = val[field["name"].ToString()];
                    cmd.Parameters.Add(p);
                } }
                ,{ "decimal",(DbCommand cmd,JToken val,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"];
                    p.DbType = DbType.Decimal;
                    p.Value = val[field["name"].ToString()];
                    cmd.Parameters.Add(p);
                } }
                ,{ "smallint",(DbCommand cmd,JToken val,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"];
                    p.DbType = DbType.Int16;
                    p.Value = val[field["name"].ToString()];
                    cmd.Parameters.Add(p);
                } }
                ,{ "tinyint",(DbCommand cmd,JToken val,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"];
                    p.DbType = DbType.Byte;
                    p.Value = val[field["name"].ToString()];
                    cmd.Parameters.Add(p);
                } }
                ,{ "bool",(DbCommand cmd,JToken val,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"];
                    p.DbType = DbType.Boolean;
                    p.Value = val[field["name"].ToString()];
                    cmd.Parameters.Add(p);
                } }
            };
        }
    }
}