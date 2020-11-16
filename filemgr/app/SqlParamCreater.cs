using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    /// <summary>
    /// 变量创建器，只创建变量，不赋值
    /// </summary>
    public class SqlParamCreater
    {
        /// <summary>
        /// Command变量创建器
        /// </summary>
        public delegate void dbParamSetDelegate(DbCommand cmd, JToken field);
        protected Dictionary<string, dbParamSetDelegate> m_map;

        public dbParamSetDelegate this[string index]
        {
            get { return this.m_map[index]; }
        }

        public void create(DbCommand cmd,JToken fields)
        {
            foreach (var f in fields)
            {
                var name = f["name"].ToString();
                var type = f["type"].ToString().ToLower();
                this.m_map[type](cmd, f);
            }
        }


        public SqlParamCreater()
        {
            //初始化mcd变量创建映射
            this.m_map = new Dictionary<string, dbParamSetDelegate>() {
                { "string",(DbCommand cmd,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"].ToString();
                    p.DbType = DbType.String;
                    p.Size = Convert.ToInt32(field["length"]);
                    cmd.Parameters.Add(p);
                } }
                ,{ "int",(DbCommand cmd,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"].ToString();
                    p.DbType = DbType.Int32;
                    cmd.Parameters.Add(p);
                } }
                ,{ "datetime",(DbCommand cmd,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"].ToString();
                    p.DbType = DbType.DateTime;
                    cmd.Parameters.Add(p);
                } }
                ,{ "long",(DbCommand cmd,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"].ToString();
                    p.DbType = DbType.Int64;
                    cmd.Parameters.Add(p);
                } }
                ,{ "double",(DbCommand cmd,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"].ToString();
                    p.DbType = DbType.Double;
                    cmd.Parameters.Add(p);
                } }
                ,{ "decimal",(DbCommand cmd,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"].ToString();
                    p.DbType = DbType.Decimal;
                    cmd.Parameters.Add(p);
                } }
                ,{ "smallint",(DbCommand cmd,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"].ToString();
                    p.DbType = DbType.Int16;
                    cmd.Parameters.Add(p);
                } }
                ,{ "tinyint",(DbCommand cmd,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"].ToString();
                    p.DbType = DbType.Byte;
                    cmd.Parameters.Add(p);
                } }
                ,{ "bool",(DbCommand cmd,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = "@" + field["name"].ToString();
                    p.DbType = DbType.Boolean;
                    cmd.Parameters.Add(p);
                } }
            };
        }
    }
}