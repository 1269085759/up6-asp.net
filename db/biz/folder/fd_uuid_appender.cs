using System.IO;
using System.Text;
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
            this.m_root.pathSvr = this.pb.genFolder(this.m_root.uid, this.m_root.nameLoc);
            if (!Directory.Exists(this.m_root.pathSvr)) Directory.CreateDirectory(this.m_root.pathSvr);

            base.save();
        }
        protected override void get_md5s()
        {
        }
        protected override void get_md5_files() { }
        protected override void saveFiles()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into up6_files(");
            sb.Append(" f_id");
            sb.Append(",f_pid");
            sb.Append(",f_pidRoot");
            sb.Append(",f_fdTask");
            sb.Append(",f_fdChild");
            sb.Append(",f_sizeLoc");
            sb.Append(",f_uid");
            sb.Append(",f_nameLoc");
            sb.Append(",f_nameSvr");
            sb.Append(",f_pathLoc");
            sb.Append(",f_pathSvr");
            sb.Append(",f_pathRel");
            sb.Append(",f_md5");
            sb.Append(",f_lenLoc");
            sb.Append(",f_perSvr");
            sb.Append(",f_complete");

            sb.Append(") values (");

            sb.Append(" @f_id");
            sb.Append(",@f_pid");
            sb.Append(",@f_pidRoot");
            sb.Append(",@f_fdTask");
            sb.Append(",@f_fdChild");
            sb.Append(",@f_sizeLoc");
            sb.Append(",@f_uid");
            sb.Append(",@f_nameLoc");
            sb.Append(",@f_nameSvr");
            sb.Append(",@f_pathLoc");
            sb.Append(",@f_pathSvr");
            sb.Append(",@f_pathRel");
            sb.Append(",@f_md5");
            sb.Append(",@f_lenLoc");
            sb.Append(",@f_perSvr");
            sb.Append(",@f_complete");
            sb.Append(") ;");

            var cmd = this.db.GetCommand(sb.ToString());
            this.db.AddString(ref cmd, "@f_id", string.Empty, 32);
            this.db.AddString(ref cmd, "@f_pid", string.Empty, 32);
            this.db.AddString(ref cmd, "@f_pidRoot", string.Empty, 32);
            this.db.AddBool(ref cmd, "@f_fdTask", false);
            this.db.AddString(ref cmd, "@f_sizeLoc", string.Empty, 32);
            this.db.AddBool(ref cmd, "@f_fdChild", true);
            this.db.AddInt(ref cmd, "@f_uid", 0);
            this.db.AddString(ref cmd, "@f_nameLoc", string.Empty, 255);
            this.db.AddString(ref cmd, "@f_nameSvr", string.Empty, 255);
            this.db.AddString(ref cmd, "@f_pathLoc", string.Empty, 255);
            this.db.AddString(ref cmd, "@f_pathSvr", string.Empty, 255);
            this.db.AddString(ref cmd, "@f_pathRel", string.Empty, 255);
            this.db.AddString(ref cmd, "@f_md5", string.Empty, 40);
            this.db.AddInt64(ref cmd, "@f_lenLoc", 0);
            this.db.AddString(ref cmd, "@f_perSvr", string.Empty, 6);
            this.db.AddBool(ref cmd, "@f_complete", false);
            cmd.Prepare();
            foreach (var f in this.m_root.files)
            {
                if (string.IsNullOrEmpty(f.pathSvr))
                    f.pathSvr = Path.Combine(this.m_root.pathSvr,f.pathRel);
                if (string.IsNullOrEmpty(f.nameSvr))
                    f.nameSvr = f.nameLoc;
                cmd.Parameters["@f_id"].Value = f.id;
                cmd.Parameters["@f_pid"].Value = f.pid;
                cmd.Parameters["@f_pidRoot"].Value = this.m_root.id;
                cmd.Parameters["@f_sizeLoc"].Value = f.sizeLoc;
                cmd.Parameters["@f_uid"].Value = f.uid;
                cmd.Parameters["@f_nameLoc"].Value = f.nameLoc;
                cmd.Parameters["@f_nameSvr"].Value = f.nameSvr;
                cmd.Parameters["@f_pathLoc"].Value = f.pathLoc;
                cmd.Parameters["@f_pathSvr"].Value = f.pathSvr;
                cmd.Parameters["@f_pathRel"].Value = f.pathRel;
                cmd.Parameters["@f_md5"].Value = f.md5;
                cmd.Parameters["@f_lenLoc"].Value = f.lenLoc;
                cmd.Parameters["@f_perSvr"].Value = f.lenLoc > 0 ? f.perSvr : "0%";
                cmd.Parameters["@f_complete"].Value = f.lenLoc > 0 ? f.complete : true;
                cmd.ExecuteNonQuery();

                //创建文件
                if (!f.complete && f.lenSvr < 1)
                {
                    FileBlockWriter fr = new FileBlockWriter();
                    fr.make(f.pathSvr, f.lenLoc);
                }
            }
            var rt = this.m_root;
            //添加根目录
            cmd.Parameters["@f_id"].Value = rt.id;
            cmd.Parameters["@f_pid"].Value = string.Empty;
            cmd.Parameters["@f_pidRoot"].Value = string.Empty;
            cmd.Parameters["@f_fdTask"].Value = true;
            cmd.Parameters["@f_fdChild"].Value = false;
            cmd.Parameters["@f_sizeLoc"].Value = rt.sizeLoc;
            cmd.Parameters["@f_uid"].Value = rt.uid;
            cmd.Parameters["@f_nameLoc"].Value = rt.nameLoc;
            cmd.Parameters["@f_nameSvr"].Value = string.Empty;
            cmd.Parameters["@f_pathLoc"].Value = rt.pathLoc;
            cmd.Parameters["@f_pathSvr"].Value = rt.pathSvr;
            cmd.Parameters["@f_pathRel"].Value = string.Empty;
            cmd.Parameters["@f_md5"].Value = rt.md5;
            cmd.Parameters["@f_lenLoc"].Value = rt.lenLoc;
            cmd.Parameters["@f_perSvr"].Value = rt.lenLoc > 0 ? rt.perSvr : "0%";
            cmd.Parameters["@f_complete"].Value = rt.lenLoc > 0 ? rt.complete : true;
            cmd.ExecuteNonQuery();
        }
    }
}