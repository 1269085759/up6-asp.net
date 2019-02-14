using System.IO;
using up6.db.model;
using up6.db.utils;

namespace up6.db.biz.folder
{
    /// <summary>
    /// 以uuid模式存储，文件夹层级结构与客户端完全一致。
    /// </summary>
    public class fd_uuid_appender : fd_appender
    {
        public fd_uuid_appender()
        {
            this.pb = new PathBuilderUuid();
        }

        public override void save()
        {
            this.db.connection.Open();
            this.m_root.pathSvr = this.pb.genFolder(this.m_root.uid, this.m_root.nameLoc);
            this.m_root.pathSvr = this.m_root.pathSvr.Replace("\\", "/");
            this.m_root.pidRoot = string.Empty;
            if (!Directory.Exists(this.m_root.pathSvr)) Directory.CreateDirectory(this.m_root.pathSvr);

            this.save_file(this.m_root);
            this.save_folder(this.m_root);

            this.db.connection.Close();

        }
        protected override void get_md5s(){}
        protected override void get_md5_files() { }
    }
}