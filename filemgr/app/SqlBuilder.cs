using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    public class SqlBuilder
    {
        /// <summary>
        /// 拼接SQL语句
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fields"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public string select(string table, string fields, string where)
        {
            string sql_where = string.Empty;
            if (!string.IsNullOrEmpty(where.Trim())) sql_where = string.Format("where {0}", where);
            var sql = string.Format("select {0} from {1} {2}", fields, table, sql_where);
            return sql;
        }

        public string select(string table, string fields, SqlParam[] where)
        {
            string sql_where = string.Empty;
            if (where !=null ) sql_where = string.Format("where {0}", where);
            var sql = string.Format("select {0} from {1} {2}", fields, table, sql_where);
            return sql;
        }
    }
}