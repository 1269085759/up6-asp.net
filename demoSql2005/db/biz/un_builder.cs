using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace up6.demoSql2005.db.biz
{
    public class un_builder
    {
        /// <summary>
        /// 加载未上传完的文件和文件夹列表
        /// </summary>
        private List<un_file> files = new List<un_file>();
        private Dictionary<int, int/*对应到files的索引*/> folders = new Dictionary<int, int>();
        public un_builder()
        {
        }

        public string read(string uid)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            sb.Append(" f_id");//0
            sb.Append(",f_pid");//1
            sb.Append(",f_pidRoot");//2
            sb.Append(",f_fdTask");//3
            sb.Append(",f_fdID");//4
            sb.Append(",f_fdChild");//5
            sb.Append(",f_nameLoc");//6
            sb.Append(",f_nameSvr");//7
            sb.Append(",f_pathLoc");//8
            sb.Append(",f_pathSvr");//9
            sb.Append(",f_pathRel");//10
            sb.Append(",f_md5");//11
            sb.Append(",f_lenLoc");//12
            sb.Append(",f_sizeLoc");//13
            sb.Append(",f_pos");//15
            sb.Append(",f_lenSvr");//16
            sb.Append(",f_perSvr");//17
            sb.Append(",f_complete");//18
            //
            sb.Append(" from up6_files");
            //
            sb.Append(" where f_uid=@f_uid and f_complete=0 and f_deleted=0");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddInt(ref cmd, "@f_uid", int.Parse(uid));
            DbDataReader r = db.ExecuteReader(cmd);

            while (r.Read())
            {
                var pidRoot = r.GetInt32(2);

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
            f.uid = uid;
            f.read(0, ref r);

            //是文件夹
            if (f.fdTask)
            {
                int fd_index = 0;//有文件夹
                if (this.folders.TryGetValue(f.fdID, out fd_index))
                {
                    this.files[fd_index].copy(ref f);
                }//没有文件夹，先创建一个空文件夹
                else
                {
                    f.files = new List<folder.fd_file>();
                    this.folders.Add(f.pidRoot, files.Count);
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
                //fd.idSvr = pidRoot;
                fd.fdID = pidRoot;
                fd.files = new List<folder.fd_file>();
                fd.files.Add(f);

                this.folders.Add(pidRoot, this.files.Count);
                this.files.Add(fd);
            }//存在文件夹
            else
            {
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