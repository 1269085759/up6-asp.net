using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    /// <summary>
    /// DbReader读
    /// </summary>
    public class SqlCmdReader
    {
        public delegate JToken readerDelegate(DbDataReader r, int index);
        protected Dictionary<string, readerDelegate> m_map;

        public readerDelegate this[string index]
        {
            get { return this.m_map[index]; }
        }

        public JObject read(DbDataReader r,JToken fields)
        {
            int i = 0;
            JObject o = new JObject();
            foreach(var f in fields)
            {
                var name = f["name"].ToString();
                var type = f["type"].ToString().ToLower();
                o[name] = this.m_map[type](r, i++);
            }
            return o;
        }

        public SqlCmdReader()
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
                ,{ "double",(DbDataReader r,int index)=>{
                    return r.IsDBNull(index) ? 0:r.GetFloat(index);
                } }
                ,{ "decimal",(DbDataReader r,int index)=>{
                    return r.IsDBNull(index) ? 0:r.GetDecimal(index);
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
                    return r.IsDBNull(index) ? false:r.GetBoolean(index);
                } }
            };
        }
    }
}