using up6.demoSql2005.db;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace up6.demoSql2005.down2.biz
{
    /// <summary>
    /// 为所有已上传完的文件和文件夹生成JSON
    /// </summary>
    public class cmp_builder
    {
        public List<cmp_file> files = new List<cmp_file>();//文件列表
        private Dictionary<int/*pidRoot*/, int/*对应到files的索引*/> folders = new Dictionary<int, int>();

        public cmp_builder()
        {
        }

        public string read(int uid)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            sb.Append(" up6_files.f_id");//0
            sb.Append(",up6_files.f_pid");//1
            sb.Append(",up6_files.f_fdTask");//2
            sb.Append(",up6_files.f_fdID");//3
            sb.Append(",up6_files.f_fdChild");//4
            sb.Append(",up6_files.f_pidRoot");//5
            sb.Append(",up6_files.f_nameLoc");//6
            sb.Append(",up6_files.f_sizeLoc");//6
            sb.Append(",up6_files.f_pathLoc");//7
            sb.Append(",up6_files.f_lenSvr");//12
            sb.Append(" from up6_files ");
            //
            sb.Append(" where up6_files.f_uid=@f_uid and up6_files.f_deleted=0 and up6_files.f_complete=1");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddInt(ref cmd, "@f_uid", uid);
            DbDataReader r = db.ExecuteReader(cmd);

            while (r.Read())
            {
                var pidRoot = r.GetInt32(5);

                //是一个子文件
                if (pidRoot != 0)
                {
                    this.add_child(ref r, pidRoot);
                }//是一个文件项
                else
                {
                    this.add_file(ref r, uid);
                }
            }
            r.Close();

            return this.to_json();//
        }

        /// <summary>
        /// 添加一个文件项
        /// </summary>
        public virtual void add_file(ref DbDataReader r, int uid)
        {
            cmp_file f = new cmp_file();
            f.read(0, ref r);

            if (f.fdTask)
            {
                int fd_index = 0;
                if (this.folders.TryGetValue(f.fdID, out fd_index))
                {
                    this.files[fd_index].nameLoc = f.nameLoc;
                    this.files[fd_index].pathLoc = f.pathLoc;
                    this.files[fd_index].fileUrl = f.fileUrl;
                    this.files[fd_index].lenLoc = f.lenLoc;
                    this.files[fd_index].lenSvr = f.lenSvr;
                    this.files[fd_index].sizeSvr = f.sizeSvr;
                    this.files[fd_index].perLoc = f.perLoc;
                    this.files[fd_index].fdTask = true;
                    this.files[fd_index].fdID = f.fdID;
                }
                else
                {
                    f.files = new List<model.DnFileInf>();
                    this.folders.Add(f.fdID, files.Count);
                    this.files.Add(f);
                }
            }//根级文件
            else
            {
                files.Add(f);
            }
        }

        /// <summary>
        /// 查找父级文件夹并添加到其文件列表中
        /// </summary>
        public void add_child(ref DbDataReader r, int pidRoot)
        {
            cmp_file f = new cmp_file();
            f.read(pidRoot, ref r);//

            int fd_index = 0;
            //不存在文件夹
            if (!this.folders.TryGetValue(pidRoot, out fd_index))
            {
                cmp_file fd = new cmp_file();
                fd.idSvr = pidRoot;
                fd.files = new List<model.DnFileInf>();
                fd.files.Add(f);

                this.folders.Add(pidRoot, this.files.Count);
                this.files.Add(fd);
            }//存在文件夹
            else
            {
                this.files[fd_index].files.Add(f);
            }
        }

        public string to_json()
        {
            if (this.files.Count > 0)
            {
                return JsonConvert.SerializeObject(this.files);
            }
            return null;
        }
    }
}