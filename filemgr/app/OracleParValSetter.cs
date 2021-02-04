using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace up6.filemgr.app
{
    public class OracleParValSetter : SqlParValSetter
    {
        public OracleParValSetter()
        {
            this.m_map = new Dictionary<string, setterDelegate>() {
                { "string",(DbCommand cmd,JToken val, JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = ":" + field["name"];
                    p.DbType = DbType.String;
                    p.Size = Convert.ToInt32(field["length"]);
                    p.Value = val[field["name"].ToString()];
                    cmd.Parameters.Add(p);
                } }
                ,{ "int",(DbCommand cmd,JToken val,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = ":" + field["name"];
                    p.DbType = DbType.Int32;
                    p.Value = val[field["name"].ToString()];
                    cmd.Parameters.Add(p);
                } }
                ,{ "datetime",(DbCommand cmd,JToken val,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = ":" + field["name"];
                    p.DbType = DbType.DateTime;
                    p.Value = val[field["name"].ToString()];
                    cmd.Parameters.Add(p);
                } }
                ,{ "long",(DbCommand cmd,JToken val,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = ":" + field["name"];
                    p.DbType = DbType.Int64;
                    p.Value = val[field["name"].ToString()];
                    cmd.Parameters.Add(p);
                } }
                ,{ "smallint",(DbCommand cmd,JToken val,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = ":" + field["name"];
                    p.DbType = DbType.Int16;
                    p.Value = val[field["name"].ToString()];
                    cmd.Parameters.Add(p);
                } }
                ,{ "tinyint",(DbCommand cmd,JToken val,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = ":" + field["name"];
                    p.DbType = DbType.Byte;
                    p.Value = val[field["name"].ToString()];
                    cmd.Parameters.Add(p);
                } }
                ,{ "bool",(DbCommand cmd,JToken val,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = ":" + field["name"];
                    p.DbType = DbType.Boolean;
                    p.Value = val[field["name"].ToString()];
                    cmd.Parameters.Add(p);
                } }
            };
        }
    }
}