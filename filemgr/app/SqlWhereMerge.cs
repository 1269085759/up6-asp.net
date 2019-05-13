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
        /// <summary>
        /// 字段名称，表达式
        /// </summary>
        Dictionary<string, string> m_cds;

        public SqlWhereMerge()
        {
            this.m_cds = new Dictionary<string, string>();
        }

        /// <summary>
        /// 相等条件，a=b
        /// </summary>
        public void equal(string n, string v)
        {
            this.m_cds[n] = string.Format("{0}='{1}'", n, v);
        }

        /// <summary>
        /// 相等条件，a=b
        /// </summary>
        /// <param name="n"></param>
        /// <param name="v"></param>
        public void equal(string n, int v)
        {
            this.m_cds[n] = string.Format("{0}={1}", n, v);
        }

        /// <summary>
        /// 相等条件，a=b，从查询字符串中获取变量（Request.QueryString)
        /// </summary>
        /// <param name="n"></param>
        /// <param name="v"></param>
        /// <param name="ignore">是否自动忽略空值</param>
        public void req_equal(string n, string requestName, bool ignore = true)
        {
            string v = HttpContext.Current.Request.QueryString[requestName];
            if (string.IsNullOrEmpty(v))
            {
                if (ignore) return;
                v = string.Empty;
            }
            this.m_cds.Add(n, string.Format("{0} = '{1}'", n, v));
        }

        public void req_like(string n, string requestName, bool ignore = true)
        {
            string v = HttpContext.Current.Request.QueryString[requestName];
            if (string.IsNullOrEmpty(v))
            {
                if (ignore) return;
                v = string.Empty;
            }
            this.m_cds.Add(n, string.Format("{0} like '%{1}%'", n, v));
        }

        public void like(string n, string v)
        {
            this.m_cds.Add(n, string.Format("{0} like '%{1}%'", n, v));
        }

        public void add(SqlWhereCondition c)
        {
            this.m_cds.Add(c.name, c.toSql());
        }
        public void add(string[] c)
        {
            if (c[1].Equals("like"))
            {
                this.m_cds.Add(c[0], string.Format("{0} like '%{1}%'", c[0], c[2].Trim()));
            }
            else if (c[1].Equals("="))
            {
                this.m_cds.Add(c[0], string.Format("{0} = '{1}'", c[0], c[2].Trim()));
            }
        }

        /// <summary>
        /// 添加条件语句
        /// </summary>
        /// <param name="n">列名</param>
        /// <param name="p">条件谓词:=,like</param>
        /// <param name="v">值</param>
        public void add(string n, string p, string v)
        {
            if (string.IsNullOrEmpty(n) || string.IsNullOrEmpty(v)) return;
            this.add(new string[] { n, p, v });
        }

        /// <summary>
        /// 自动添加请求变量
        /// </summary>
        /// <param name="n">字段名称</param>
        /// <param name="p">谓词，=,</param>
        /// <param name="v">Request 参数名称</param>
        public void addReq(string n, string p, string v)
        {
            v = HttpContext.Current.Request.QueryString[v];
            if (string.IsNullOrEmpty(v)) return;
            this.add(new string[] { n, p, v });
        }

        public void addInt(string n, string v)
        {
            if (string.IsNullOrEmpty(v)) return;
            this.add(new string[] { n, "int", v });
        }

        public void clear() { this.m_cds.Clear(); }
        public void del(string n) { this.m_cds.Remove(n); }
        public string to_sql()
        {
            //return string.Join(" and ", this.m_cds.ToArray());
            return string.Join(" and  ", this.m_cds.Select((n) => n.Value).ToArray());
        }

        /// <summary>
        /// 拼接搜索条件，自动删除空白
        /// </summary>
        /// <param name="cds"></param>
        /// <returns>a='a' and b like '%b%'</returns>
        public static string merge(SqlWhereCondition[] cds)
        {
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
    }
}