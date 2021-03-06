﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    public class OracleParamSetter : SqlParamSetter
    {
        public OracleParamSetter()
        {
            //初始化mcd变量创建映射
            this.m_map = new Dictionary<string, dbParamSetDelegate>() {
                { "string",(DbCommand cmd,SqlParam param,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = ":" + param.Name;
                    p.DbType = DbType.String;
                    p.Size = Convert.ToInt32(field["length"]);
                    p.Value = param.m_valStr;
                    cmd.Parameters.Add(p);
                } }
                ,{ "int",(DbCommand cmd,SqlParam param,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = ":" + param.Name;
                    p.DbType = DbType.Int32;
                    p.Value = param.m_valInt;
                    cmd.Parameters.Add(p);
                } }
                ,{ "datetime",(DbCommand cmd,SqlParam param,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = ":" + param.Name;
                    p.DbType = DbType.DateTime;
                    p.Value = param.m_valTm;
                    cmd.Parameters.Add(p);
                } }
                ,{ "long",(DbCommand cmd,SqlParam param,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = ":" + param.Name;
                    p.DbType = DbType.Int64;
                    p.Value = param.m_valLong;
                    cmd.Parameters.Add(p);
                } }
                ,{ "smallint",(DbCommand cmd,SqlParam param,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = ":" + param.Name;
                    p.DbType = DbType.Int16;
                    p.Value = param.m_valInt;
                    cmd.Parameters.Add(p);
                } }
                ,{ "tinyint",(DbCommand cmd,SqlParam param,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = ":" + param.Name;
                    p.DbType = DbType.Byte;
                    p.Value = param.m_valInt;
                    cmd.Parameters.Add(p);
                } }
                ,{ "bool",(DbCommand cmd,SqlParam param,JToken field)=>{
                    var p = cmd.CreateParameter();
                    p.Direction = ParameterDirection.Input;
                    p.ParameterName = ":" + param.Name;
                    p.DbType = DbType.Boolean;
                    p.Value = param.m_valBool;
                    cmd.Parameters.Add(p);
                } }
            };
        }
    }
}