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
            if (!Directory.Exists(this.m_root.pathSvr)) Directory.CreateDirectory(this.m_root.pathSvr);

            this.save_file(this.m_root);
            this.save_folder(this.m_root);

            foreach (FileInf f in this.m_root.files)
            {
                f.pathSvr = Path.Combine(this.m_root.pathSvr, f.pathRel);
                f.nameSvr = f.nameLoc;
                f.fdChild = true;

                //创建文件
                if (!f.complete && f.lenSvr < 1)
                {
                    FileBlockWriter fr = new FileBlockWriter();
                    fr.make(f.pathSvr, f.lenLoc);
                }

                this.save_file(f);
            }

            foreach (FileInf fd in this.m_root.folders)
            {
                fd.pathSvr = Path.Combine(this.m_root.pathSvr, fd.pathRel);
                if (!Directory.Exists(fd.pathSvr)) Directory.CreateDirectory(fd.pathSvr);
                fd.nameSvr = fd.nameLoc;
                this.save_folder(fd);
            }

            this.db.connection.Close();

        }
        protected override void get_md5s(){}
        protected override void get_md5_files() { }
    }
}