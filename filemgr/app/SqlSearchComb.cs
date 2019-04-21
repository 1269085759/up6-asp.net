using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    /// <summary>
    /// SQL搜索组合器，与sql.search.js配合使用
    /// var ssc = new SqlSearchComb();
    /// ssc.parse();
    /// ssc.to_sql()
    /// 添加到页面
    /// this.param.Add("where",Value)
    /// </summary>
    public class SqlSearchComb
    {
        List<SqlSearchCombParam> m_pars = new List<SqlSearchCombParam>();

        public SqlSearchComb() {
        }

        public void add(SqlSearchCombParam p )
        {
            this.m_pars.Add(p);
        }

        /// <summary>
        /// 解析请求变量。
        /// <para>请求格式：url?where=urlencode(v)</para>
        /// </summary>
        /// <param name="pname">变量名称，默认为where</param>
        public void parse(string pname="where")
        {
            var kv = HttpContext.Current.Request.QueryString[pname];
            if (!string.IsNullOrEmpty(kv))
            {
                kv = HttpContext.Current.Server.UrlDecode(kv);
                this.m_pars= JsonConvert.DeserializeObject<List<SqlSearchCombParam>>(kv);
            }
        }

        /// <summary>
        /// 转换为SQL语句
        /// <para>age=10 and qq=1</para>
        /// </summary>
        public string to_sql() {
            var ss = from s in this.m_pars
                     select s.expression;
            return string.Join(" and  ", ss);
        }

        /// <summary>
        /// 获取JSON值，提供给sql.search.js使用。注册成变量(page.where)
        /// </summary>
        public JToken Value { get {
                return JToken.FromObject(this.m_pars);
            } }
    }
}