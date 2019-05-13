using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    /// <summary>
    /// SQL搜索组合器变量
    /// </summary>
    public class SqlSearchCombParam
    {
        /// <summary>
        /// 显示名称
        /// <para>示例：年龄>20</para>
        /// </summary>
        public string name = string.Empty;
        /// <summary>
        /// 字段（唯一）
        /// <para>示例：id</para>
        /// </summary>
        public string field = string.Empty;
        /// <summary>
        /// 表达式。
        /// <para>示例：age=20</para>
        /// </summary>
        public string expression = string.Empty;
    }
}