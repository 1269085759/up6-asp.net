using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Newtonsoft.Json;
using up6.db.model;
using up6.db.database;

namespace up6.db.biz
{
    public class un_builder
    {
        /// <summary>
        /// 加载未上传完的文件和文件夹列表
        /// </summary>
        private List<FileInf> files = new List<FileInf>();
        public un_builder()
        {
        }

        public string read(string uid)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            sb.Append(" f_id");//0
            sb.Append(",f_fdTask");//1
            sb.Append(",f_nameLoc");//2
            sb.Append(",f_nameSvr");//3
            sb.Append(",f_pathLoc");//4
            sb.Append(",f_pathSvr");//5
            sb.Append(",f_pathRel");//6
            sb.Append(",f_md5");//7
            sb.Append(",f_lenLoc");//8
            sb.Append(",f_sizeLoc");//9
            sb.Append(",f_pos");//10
            sb.Append(",f_lenSvr");//11
            sb.Append(",f_perSvr");//12
            //
            sb.Append(" from up6_files");
            //
            sb.Append(" where f_uid=@f_uid and f_complete=0 and f_deleted=0 and f_fdChild=0 and f_scan=0");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddInt(ref cmd, "@f_uid", int.Parse(uid));
            DbDataReader r = db.ExecuteReader(cmd);

            while (r.Read())
            {
                var f = new FileInf();
                f.id = r.GetString(0);
                f.fdTask = r.GetBoolean(1);
                f.nameLoc = r.GetString(2);
                f.nameSvr = r.GetString(3);
                f.pathLoc = r.GetString(4);
                f.pathSvr = r.GetString(5);
                f.pathRel = r.GetString(6);
                f.md5 = r.GetString(7);
                f.lenLoc = r.GetInt64(8);
                f.sizeLoc = r.GetString(9);
                f.offset = r.GetInt64(10);
                f.lenSvr = r.GetInt64(11);
                f.perSvr = r.GetString(12);
                this.files.Add(f);
            }
            r.Close();

            return this.to_json();//
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