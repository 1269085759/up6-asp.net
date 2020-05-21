using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    public class OracleCmdReader : SqlCmdReader
    {
        public OracleCmdReader()
        {
            //初始化数据读取器
            this.m_map = new Dictionary<string, readerDelegate>() {
                { "string",(DbDataReader r,int index)=>{
                    return r.IsDBNull(index) ? string.Empty:r.GetString(index);
                } }
                ,{ "int",(DbDataReader r,int index)=>{
                    return r.IsDBNull(index) ? 0:r.GetInt32(index);
                } }
                ,{ "datetime",(DbDataReader r,int index)=>{
                    return r.IsDBNull(index) ? DateTime.Now:r.GetDateTime(index);
                } }
                ,{ "long",(DbDataReader r,int index)=>{
                    return r.IsDBNull(index) ? 0:r.GetInt64(index);
                } }
                ,{ "smallint",(DbDataReader r,int index)=>{
                    return r.IsDBNull(index) ? 0:r.GetInt16(index);
                } }
                ,{ "tinyint",(DbDataReader r,int index)=>{
                    return r.IsDBNull(index) ? 0:r.GetByte(index);
                } }
                ,{ "short",(DbDataReader r,int index)=>{
                    return r.IsDBNull(index) ? 0:r.GetInt16(index);
                } }
                ,{ "byte",(DbDataReader r,int index)=>{
                    return r.IsDBNull(index) ? 0:r.GetByte(index);
                } }
                ,{ "bool",(DbDataReader r,int index)=>{
                    return r.IsDBNull(index) ? 0:r.GetInt32(index);
                } }
            };
        }
    }
}