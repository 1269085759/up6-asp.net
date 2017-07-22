using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using up6.db;
using up6.db.database;
using up6.db.model;
using up6.down2.model;

namespace up6.down2.biz
{
    public class DnFile
    {
        public void Add(ref model.DnFileInf inf)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into down_files(");
            sql.Append(" f_id");
            sql.Append(",f_uid");
            sql.Append(",f_nameLoc");
            sql.Append(",f_pathLoc");
            sql.Append(",f_fileUrl");
            sql.Append(",f_lenSvr");
            sql.Append(",f_sizeSvr");

            sql.Append(") values(");
            sql.Append(" @f_id");
            sql.Append(",@f_uid");
            sql.Append(",@f_nameLoc");
            sql.Append(",@f_pathLoc");
            sql.Append(",@f_fileUrl");
            sql.Append(",@f_lenSvr");
            sql.Append(",@f_sizeSvr");
            sql.Append(");");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sql.ToString());
            db.AddString(ref cmd, "@f_id", inf.id, 32);
            db.AddInt(ref cmd, "@f_uid", inf.uid);
            db.AddString(ref cmd, "@f_nameLoc", inf.nameLoc, 255);
            db.AddString(ref cmd, "@f_pathLoc", inf.pathLoc, 255);
            db.AddString(ref cmd, "@f_fileUrl", inf.fileUrl, 255);
            db.AddInt64(ref cmd, "@f_lenSvr", inf.lenSvr);
            db.AddString(ref cmd, "@f_sizeSvr", inf.sizeSvr,10);
            db.ExecuteNonQuery(ref cmd);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fid"></param>
        public void Delete(string fid, int uid)
        {
            string sql = "delete from down_files where f_id=@f_id and f_uid=@f_uid";
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sql);
            db.AddString(ref cmd, "@f_id", fid,32);
            db.AddInt(ref cmd, "@f_uid", uid);
            db.ExecuteNonQuery(ref cmd);
        }

        public void process(string fid, int uid, string lenLoc, string perLoc)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update down_files set ");
            sb.Append(" f_lenLoc =@lenLoc");
            sb.Append(",f_perLoc=@f_perLoc");
            sb.Append(" where");
            sb.Append(" f_id =@f_id and f_uid=@f_uid;");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddString(ref cmd, "@lenLoc", lenLoc, 19);
            db.AddString(ref cmd, "@f_perLoc", perLoc, 6);
            db.AddString(ref cmd, "@f_id", fid,32);
            db.AddInt(ref cmd, "@f_uid", uid);
            db.ExecuteNonQuery(ref cmd);
        }

        static public void Clear()
        {
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand("delete from down_files;");
            db.ExecuteNonQuery(ref cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public string all_uncmp(int uid)
        {
            List<DnFileInf> files = new List<DnFileInf>();
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            sb.Append(" id");//0
            sb.Append(",f_id");//0
            sb.Append(",f_nameLoc");//1
            sb.Append(",f_pathLoc");//2
            sb.Append(",f_perLoc");//3
            sb.Append(",f_sizeSvr");//7
            sb.Append(",f_fdTask");//10
            //
            sb.Append(" from down_files");
            //
            sb.Append(" where f_uid=@f_uid and f_complete=0");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddInt(ref cmd, "@f_uid", uid);
            DbDataReader r = db.ExecuteReader(cmd);

            while (r.Read())
            {
                DnFileInf f = new DnFileInf();
                f.id = r.GetString(0);
                f.f_id = r.GetString(1);
                f.nameLoc = r.GetString(2);
                f.pathLoc = r.GetString(3);
                f.perLoc = r.GetString(4);
                f.sizeSvr = r.GetString(5);
                f.fdTask = r.GetBoolean(6);
                files.Add(f);
            }
            r.Close();

            if (files.Count > 0)
            {
                return JsonConvert.SerializeObject(files);
            }
            return string.Empty;
        }

        /// <summary>
        /// 从up6_files表中加载所有已经上传完毕的文件
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public string all_complete(int uid)
        {
            List<DnFileInf> fs = new List<DnFileInf>();
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            sb.Append(" f_id");//0
            sb.Append(",f_fdTask");//1
            sb.Append(",f_nameLoc");//2
            sb.Append(",f_sizeLoc");//3
            sb.Append(",f_lenSvr");//4
            sb.Append(",f_pathSvr");//5
            sb.Append(" from up6_files ");
            //
            sb.Append(" where f_uid=@f_uid and f_deleted=0 and f_complete=1 and f_fdChild=0");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddInt(ref cmd, "@f_uid", uid);
            DbDataReader r = db.ExecuteReader(cmd);

            while (r.Read())
            {
                DnFileInf f = new DnFileInf();
                f.id = Guid.NewGuid().ToString("N");
                f.f_id = r.GetString(0);
                f.fdTask = r.GetBoolean(1);
                f.nameLoc = r.GetString(2);
                f.sizeLoc = r.GetString(3);
                f.sizeSvr = r.GetString(3);
                f.lenSvr = r.GetInt64(4);
                f.pathSvr = r.GetString(5);
                fs.Add(f);
            }
            r.Close();
            if (fs.Count < 1) return string.Empty;
            return JsonConvert.SerializeObject(fs);
        }
    }
}