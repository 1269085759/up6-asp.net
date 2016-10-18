using up6.demoSql2005.db;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace up6.demoSql2005.down2.biz
{
    public class un_builder
    {
        /// <summary>
        /// 加载未上传完的文件和文件夹列表
        /// </summary>
        public List<un_file> files = new List<un_file>();
        private Dictionary<int, int/*对应到files的索引*/> folders = new Dictionary<int, int>();

        public un_builder()
        {
        }

        public string read(string uid)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            sb.Append(" f_id");//0
            sb.Append(",f_nameLoc");//1
            sb.Append(",f_pathLoc");//2
            sb.Append(",f_perLoc");//3
            sb.Append(",f_lenLoc");//4
            sb.Append(",f_fileUrl");//5
            sb.Append(",f_lenSvr");//6
            sb.Append(",f_sizeSvr");//7
            sb.Append(",f_pathLoc");//8
            sb.Append(",f_pidRoot");//9
            sb.Append(",f_fdTask");//10
            //
            sb.Append(" from down_files");
            //
            sb.Append(" where f_uid=@f_uid and f_complete=0");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddInt(ref cmd, "@f_uid", int.Parse(uid));
            DbDataReader r = db.ExecuteReader(cmd);

            biz.cmp_builder ub = new biz.cmp_builder();
            while (r.Read())
            {
                var pidRoot = r.GetInt32(9);

                //是一个子文件
                if (pidRoot != 0)
                {
                    this.add_child(ref r, pidRoot);
                }//是一个文件项
                else
                {
                    this.add_file(ref r, pidRoot);
                }
            }
            r.Close();

            return this.to_json();//
        }

        /// <summary>
        /// 添加一个文件项
        /// </summary>
        void add_file(ref DbDataReader r, int uid)
        {
            un_file f = new un_file();
            f.read(0, ref r);

            //是文件夹
            if (f.fdTask)
            {
                int fd_index = 0;
                if (this.folders.TryGetValue(f.idSvr, out fd_index))
                {
                    this.files[fd_index].nameLoc = f.nameLoc;
                    this.files[fd_index].pathLoc = f.pathLoc;
                    this.files[fd_index].fileUrl = f.fileUrl;
                    this.files[fd_index].lenLoc = f.lenLoc;
                    this.files[fd_index].lenSvr = f.lenSvr;
                    this.files[fd_index].sizeSvr = f.sizeSvr;
                    this.files[fd_index].perLoc = f.perLoc;
                    this.files[fd_index].fdTask = true;
                }
                else
                {
                    f.files = new List<model.DnFileInf>();
                    this.folders.Add(f.idSvr, files.Count);
                    this.files.Add(f);
                }
            }
            else
            {
                files.Add(f);
            }
        }

        /// <summary>
        /// 查找父级文件夹并添加到其文件列表中
        /// </summary>
        void add_child(ref DbDataReader r, int pidRoot)
        {
            un_file f = new un_file();
            f.read(pidRoot, ref r);//

            int fd_index = 0;
            //不存在文件夹
            if (!this.folders.TryGetValue(pidRoot, out fd_index))
            {
                un_file fd = new un_file();
                fd.idSvr = pidRoot;
                fd.files = new List<model.DnFileInf>();
                fd.files.Add(f);

                this.folders.Add(pidRoot, this.files.Count);
                this.files.Add(fd);
            }//存在文件夹
            else {
                this.files[fd_index].files.Add(f);
            }
        }

        string to_json()
        {
            if (this.files.Count > 0)
            {
                return JsonConvert.SerializeObject(this.files);
            }
            return null;
        }
    }
}