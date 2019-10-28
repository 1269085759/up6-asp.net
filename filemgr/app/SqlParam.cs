using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    public class SqlParam
    {
        public byte m_valByte = 0;
        public bool m_valBool = false;
        public int m_valInt =0;
        public long m_valLong=0;
        public string m_valStr=string.Empty;
        public DateTime m_valTm=DateTime.Now;
        protected string m_type;
        protected DbType m_typeDb = DbType.Int32;
        protected string m_name;

        public string Type { get { return this.m_type; } }
        public DbType DbType { get { return this.m_typeDb; } }
        public string Name { get { return this.m_name; } }
        public SqlParam(string name,string v)
        {
            this.m_name = name;
            this.m_valStr = v;
            this.m_typeDb = DbType.String;
            this.m_type = "string";
        }
        public SqlParam(string name, byte v)
        {
            this.m_name = name;
            this.m_valByte = v;
            this.m_typeDb = DbType.Byte;
            this.m_type = "byte";
        }
        public SqlParam(string name, bool v)
        {
            this.m_name = name;
            this.m_valBool = v;
            this.m_typeDb = DbType.Boolean;
            this.m_type = "bool";
        }
        public SqlParam(string name, int v)
        {
            this.m_name = name;
            this.m_valInt = v;
            this.m_typeDb = DbType.Int32;
            this.m_type = "int";
        }
        public SqlParam(string name, long v)
        {
            this.m_name = name;
            this.m_valLong = v;
            this.m_typeDb = DbType.Int64;
            this.m_type = "long";
        }
        public SqlParam(string name, DateTime v)
        {
            this.m_name = name;
            this.m_valTm = v;
            this.m_typeDb = DbType.DateTime;
            this.m_type = "time";
        }
    }
}