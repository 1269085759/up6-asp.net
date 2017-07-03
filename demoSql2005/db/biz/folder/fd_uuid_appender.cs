using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace up6.demoSql2005.db.biz.folder
{
    /// <summary>
    /// 以uuid模式存储，文件夹层级结构与客户端完全一致。
    /// </summary>
    public class fd_uuid_appender : fd_appender
    {
        public fd_uuid_appender()
        {
            this.pb = new PathUuidBuilder();
        }

        public override void save()
        {
            this.m_root.pathRel = this.m_root.nameLoc;//
            
            this.m_root.pathSvr = this.pb.genFolder(this.m_root.uid, this.m_root.nameLoc);
            if (!Directory.Exists(this.m_root.pathSvr)) Directory.CreateDirectory(this.m_root.pathSvr);

            base.save();
        }
        protected override void get_md5s()
        {
        }
        protected override void get_md5_files() { }

        public override void update_rel()
        {

            //更新文件的层级ID
            foreach (fd_file f in this.m_root.files)
            {
                f.nameSvr = f.nameLoc;

                //构建层级路径
                string parentPath = this.m_root.pathSvr;
                string parentRel = this.m_root.pathRel;
                int parentIndex = 0;
                if ( !string.IsNullOrEmpty(f.pid)) parentIndex = this.map_fd_ids[f.pid];
                if ( !string.IsNullOrEmpty(f.pid) ) parentPath = this.m_root.folders[parentIndex].pathSvr;
                if ( !string.IsNullOrEmpty(f.pid) ) parentRel = this.m_root.folders[parentIndex].pathRel;
                f.pathSvr = Path.Combine(parentPath, f.nameLoc);
                f.pathRel = Path.Combine(parentRel, f.nameLoc);
            }
        }
    }
}