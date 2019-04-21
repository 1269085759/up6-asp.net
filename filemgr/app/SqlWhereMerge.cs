using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    /*
     * 用法：
        string where = WhereMerge.merge(new WhereCondition[] {
         new WhereCondition{ name="cid",value=Request.QueryString["cid"],predicate="="}
        ,new WhereCondition{ name="title",value=Request.QueryString["key"]
        }
        });

        用法2：
        SqlWhereMerge sw = new SqlWhereMerge();
        sw.add("a=1 and b=1");
        var where = sw.merge();
     */
    /// <summary>
    /// where条件合并器
    /// </summary>
    public class SqlWhereMerge
    {
        List<string> m_cds;

        public SqlWhereMerge() {
            this.m_cds = new List<string>();
        }

        /// <summary>
        /// 添加条件语句
        /// </summary>
        /// <param name="c">a=b</param>
        public void add(string c) { this.m_cds.Add(c); }
        public void add(SqlWhereCondition c)
        {
            this.add(c.toSql());
        }
        public void add(string[] c)
        {
            if (c[1].Equals("like"))
            {
                this.add(string.Format("{0} like '%{1}%'", c[0], c[2].Trim()));
            }
            else if (c[1].Equals("="))
            {
                this.add(string.Format("{0} = '{1}'", c[0], c[2].Trim()));
            }
        }

        /// <summary>
        /// 添加条件语句
        /// </summary>
        /// <param name="n">列名</param>
        /// <param name="p">条件谓词:=,like</param>
        /// <param name="v">值</param>
        public void add(string n,string p,string v)
        {
            if (string.IsNullOrEmpty(n) || string.IsNullOrEmpty(v)) return;
            this.add(new string[] { n, p, v });
        }

        /// <summary>
        /// 自动添加请求变量
        /// </summary>
        public void addReq(string n,string p,string v) {
            v = HttpContext.Current.Request.QueryString[v];
            if (string.IsNullOrEmpty(v)) return;
            this.add(new string[] { n,p,v});
        }

        public void addInt(string n,string v)
        {
            if (string.IsNullOrEmpty(v)) return;
            this.add(new string[] { n, "int", v });
        }

        public string merge() { return string.Join(" and ", this.m_cds.ToArray()); }

        /// <summary>
        /// 拼接搜索条件，自动删除空白
        /// </summary>
        /// <param name="cds"></param>
        /// <returns>a='a' and b like '%b%'</returns>
        public static string merge(SqlWhereCondition[] cds) {
            List<string> arr = new List<string>();
            foreach (var c in cds)
            {
                if (!string.IsNullOrEmpty(c.name)
                    && !string.IsNullOrEmpty(c.value)
                    && !string.IsNullOrEmpty(c.predicate))
                {
                    if (c.predicate.Equals("like"))
                    {
                        arr.Add(string.Format("{0} like '%{1}%'", c.name, c.value.Trim()));
                    }
                    else if (c.predicate.Equals("="))
                    {
                        arr.Add(string.Format("{0} = '{1}'", c.name, c.value.Trim()));
                    }
                    else if (c.predicate.Equals("int"))
                    {
                        c.value.Trim();
                        if (c.value == "true") c.value = "1";
                        else if (c.value == "false") c.value = "0";
                        arr.Add(string.Format("{0} = {1}", c.name, c.value.Trim()));
                    }
                }
            }
            if (arr.Count > 0) return string.Join(",", arr.ToArray());
            return string.Empty;
        }

        public static string merge(string[] cds) {
            return string.Join(",", cds);
        }
    }
}