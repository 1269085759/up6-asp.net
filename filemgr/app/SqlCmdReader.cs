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
        Dictionary<string, readerDelegate> m_map;

        public readerDelegate this[string index]
        {
            get { return this.m_map[index]; }
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