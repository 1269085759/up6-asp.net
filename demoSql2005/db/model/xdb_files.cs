using System;

namespace up6.demoSql2005.db
{
    public class xdb_files
    {
        /// <summary>
        /// 6.3版本新增字段
        /// </summary>
        public string id = string.Empty;
        /// <summary>
        /// 文件夹ID
        /// </summary>
        public string pid = string.Empty;
        /// <summary>
        /// 根级文件夹ID
        /// </summary>
        public string pidRoot = string.Empty;
        /// <summary>
        /// 表示当前项是否是一个文件夹项。
        /// </summary>
        public bool fdTask = false;
        /// <summary>
        /// 与xdb_folders.fd_id对应
        /// </summary>
        /// <summary>
        /// 是否是文件夹中的子文件
        /// </summary>
        public bool fdChild = false;
        /// <summary>
        /// 用户ID。与第三方系统整合使用。
        /// </summary>
        public int uid = 0;
        /// <summary>
        /// 文件在本地电脑中的名称。
        /// </summary>
        public string nameLoc = string.Empty;
        /// <summary>
        /// 文件在服务器中的名称。
        /// </summary>
        public string nameSvr = string.Empty;
        /// <summary>
        /// 文件在本地电脑中的完整路径。示例：D:\Soft\QQ2012.exe
        /// </summary>
        public string pathLoc = string.Empty;
        /// <summary>
        /// 文件在服务器中的完整路径。示例：F:\ftp\uer\md5.exe
        /// </summary>
        public string pathSvr = string.Empty;
        /// <summary>
        /// 文件在服务器中的相对路径。示例：/www/web/upload/md5.exe
        /// </summary>
        public string pathRel = string.Empty;
        /// <summary>
        /// 文件MD5
        /// </summary>
        public string md5 = string.Empty;
        /// <summary>
        /// 数字化的文件长度。以字节为单位，示例：120125
        /// 文件大小可能超过2G，所以使用long
        /// </summary>
        public long lenLoc = 0;
        /// <summary>
        /// 格式化的文件尺寸。示例：10.03MB
        /// </summary>
        public string sizeLoc = string.Empty;
        /// <summary>
        /// 文件续传位置。
        /// 文件大小可能超过2G，所以使用long
        /// </summary>
        public long offset = 0;
        /// <summary>
        /// 已上传大小。以字节为单位
        /// 文件大小可能超过2G，所以使用long
        /// </summary>
        public long lenSvr = 0;
        /// <summary>
        /// 已上传百分比。示例：10%
        /// </summary>
        public string perSvr = "0%";
        public bool complete = false;
        public DateTime time = DateTime.Now;
        public bool deleted = false;
    }
}
