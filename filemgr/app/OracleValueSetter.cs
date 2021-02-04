using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    public class OracleValueSetter : SqlValueSetter
    {
        public OracleValueSetter()
        {
            //初始化mcd变量创建映射
            this.m_map = new Dictionary<string, dbValueSetDelegate>() {
                { "string",(DbCommand cmd,JToken field, JToken val)=>{
                    var pn = field["name"].ToString();
                    var p = cmd.Parameters[":"+pn];
                    p.Direction = ParameterDirection.Input;
                    p.DbType = DbType.String;
                    p.Size = Convert.ToInt32(field["length"]);
                    p.Value = val[pn];
                } }
                ,{ "int",(DbCommand cmd,JToken field, JToken val)=>{
                    var pn = field["name"].ToString();
                    var p = cmd.Parameters[":"+pn];
                    p.Direction = ParameterDirection.Input;
                    p.DbType = DbType.Int32;
                    p.Value = val[pn];
                } }
                ,{ "datetime",(DbCommand cmd,JToken field, JToken val)=>{
                    var pn = field["name"].ToString();
                    var p = cmd.Parameters[":"+pn];
                    p.Direction = ParameterDirection.Input;
                    p.DbType = DbType.DateTime;
                    p.Value = val[pn];
                } }
                ,{ "long",(DbCommand cmd,JToken field, JToken val)=>{
                    var pn = field["name"].ToString();
                    var p = cmd.Parameters[":"+pn];
                    p.Direction = ParameterDirection.Input;
                    p.DbType = DbType.Int64;
                    p.Value = val[pn];
                } }
                ,{ "smallint",(DbCommand cmd,JToken field, JToken val)=>{
                    var pn = field["name"].ToString();
                    var p = cmd.Parameters[":"+pn];
                    p.Direction = ParameterDirection.Input;
                    p.DbType = DbType.Int16;
                    p.Value = val[pn];
                } }
                ,{ "tinyint",(DbCommand cmd,JToken field, JToken val)=>{
                    var pn = field["name"].ToString();
                    var p = cmd.Parameters[":"+pn];
                    p.Direction = ParameterDirection.Input;
                    p.DbType = DbType.Byte;
                    p.Value = val[pn];
                } }
                ,{ "bool",(DbCommand cmd,JToken field, JToken val)=>{
                    var pn = field["name"].ToString();
                    var p = cmd.Parameters[":"+pn];
                    p.Direction = ParameterDirection.Input;
                    p.DbType = DbType.Boolean;
                    p.Value = val[pn];
                } }
            };
        }
    }
}