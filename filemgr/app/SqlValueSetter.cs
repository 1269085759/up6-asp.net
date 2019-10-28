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
    /// cmd值设置器
    /// </summary>
    public class SqlValueSetter
    {
        /// <summary>
        /// Command变量创建器
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="field">字段类型信息</param>
        /// <param name="val">字段值</param>
        public delegate void dbValueSetDelegate(DbCommand cmd, JToken field, JToken val);
        Dictionary<string, dbValueSetDelegate> m_map;

        public dbValueSetDelegate this[string index]
        {
            get { return this.m_map[index]; }
        }

        public void setVal(DbCommand cmd,JToken fields,JToken val)
        {
            foreach (var f in fields)
            {
                var name = f["name"].ToString();
                var type = f["type"].ToString().ToLower();
                this.m_map[type](cmd, f, val);
            }
        }

        public SqlValueSetter()
        {
            //初始化mcd变量创建映射
            this.m_map = new Dictionary<string, dbValueSetDelegate>() {
                { "string",(DbCommand cmd,JToken field, JToken val)=>{
                    var pn = field["name"].ToString();
                    var p = cmd.Parameters["@"+pn];
                    p.Direction = ParameterDirection.Input;
                    p.DbType = DbType.String;
                    p.Size = Convert.ToInt32(field["length"]);
                    p.Value = val[pn];
                } }
                ,{ "int",(DbCommand cmd,JToken field, JToken val)=>{
                    var pn = field["name"].ToString();
                    var p = cmd.Parameters["@"+pn];
                    p.Direction = ParameterDirection.Input;
                    p.DbType = DbType.Int32;
                    p.Value = val[pn];
                } }
                ,{ "datetime",(DbCommand cmd,JToken field, JToken val)=>{
                    var pn = field["name"].ToString();
                    var p = cmd.Parameters["@"+pn];
                    p.Direction = ParameterDirection.Input;
                    p.DbType = DbType.DateTime;
                    p.Value = val[pn];
                } }
                ,{ "long",(DbCommand cmd,JToken field, JToken val)=>{
                    var pn = field["name"].ToString();
                    var p = cmd.Parameters["@"+pn];
                    p.Direction = ParameterDirection.Input;
                    p.DbType = DbType.Int64;
                    p.Value = val[pn];
                } }
                ,{ "smallint",(DbCommand cmd,JToken field, JToken val)=>{
                    var pn = field["name"].ToString();
                    var p = cmd.Parameters["@"+pn];
                    p.Direction = ParameterDirection.Input;
                    p.DbType = DbType.Int16;
                    p.Value = val[pn];
                } }
                ,{ "tinyint",(DbCommand cmd,JToken field, JToken val)=>{
                    var pn = field["name"].ToString();
                    var p = cmd.Parameters["@"+pn];
                    p.Direction = ParameterDirection.Input;
                    p.DbType = DbType.Byte;
                    p.Value = val[pn];
                } }
                ,{ "bool",(DbCommand cmd,JToken field, JToken val)=>{
                    var pn = field["name"].ToString();
                    var p = cmd.Parameters["@"+pn];
                    p.Direction = ParameterDirection.Input;
                    p.DbType = DbType.Boolean;
                    p.Value = val[pn];
                } }
            };
        }
    }
}