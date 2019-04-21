using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    /// <summary>
    /// where条件对象
    /// </summary>
    public class SqlWhereCondition
    {
        /// <summary>
        /// 数据表字段名称
        /// </summary>
        public string name = string.Empty;
        /// <summary>
        /// 数据表字段值
        /// </summary>
        public string value = string.Empty;
        /// <summary>
        /// 谓词，=,int,like（默认）
        /// </summary>
        public string predicate = "like";

        public SqlWhereCondition() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="p">条件谓词，=,like，如果是int型则填int</param>
        /// <param name="v"></param>
        public SqlWhereCondition(string n, string v="", string p = "like")
        {
            this.name = n;
            this.value = v;
            this.predicate = p;
        }

        public SqlWhereCondition(string n,int v)
        {
            this.name = n;
            this.value = v.ToString();
            this.predicate = "int";
        }

        public SqlWhereCondition(string n, bool v)
        {
            this.name = n;
            this.value = v ? "1" : "0";
            this.predicate = "int";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p">0:列名，1:谓词，2:值</param>
        public SqlWhereCondition(string[] p)
        {
            this.name = p[0];
            this.predicate = p[1];
            this.value = p[2];
        }

        public string toSql() {
            string sql = string.Format("{0} like '%{1}%'", this.name, this.value);
            if (string.Equals(this.predicate, "="))
            {
                sql = string.Format("{0} ='{1}'", this.name, this.value);
            }
            else if (string.Equals(this.predicate, "int"))
            {
                sql = string.Format("{0} ={1}", this.name, this.value);
            }
            return sql;
        }
    }
}