using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web;
using up6.db.database;
using up6.down2.model;
using up6.filemgr.app;

namespace up6.down2.biz
{
    public class DnFileOdbc: DnFile
    {
        public override void Add(ref model.DnFileInf inf)
        {
            DBConfig cfg = new DBConfig();
            var se = cfg.se();
            se.insert("down_files",
                new SqlParam[] {
                    new SqlParam("f_id",inf.id),
                    new SqlParam("f_uid",inf.uid),
                    new SqlParam("f_nameLoc",inf.nameLoc),
                    new SqlParam("f_pathLoc",inf.pathLoc),
                    new SqlParam("f_fileUrl",inf.fileUrl),
                    new SqlParam("f_lenSvr",inf.lenSvr),
                    new SqlParam("f_sizeSvr",inf.sizeSvr),
                    new SqlParam("f_fdTask",inf.fdTask),
                });
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fid"></param>
        public override void Delete(string fid, int uid)
        {
            string sql = "delete from down_files where f_id=? and f_uid=?";
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sql);
            db.AddString(ref cmd, "@f_id", fid, 32);
            db.AddInt(ref cmd, "@f_uid", uid);
            db.ExecuteNonQuery(ref cmd);
        }

        public override void process(string fid, int uid, string lenLoc, string perLoc)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update down_files set ");
            sb.Append(" f_lenLoc =?");
            sb.Append(",f_perLoc=?");
            sb.Append(" where");
            sb.Append(" f_id =? and f_uid=?");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddString(ref cmd, "@lenLoc", lenLoc, 19);
            db.AddString(ref cmd, "@f_perLoc", perLoc, 6);
            db.AddString(ref cmd, "@f_id", fid, 32);
            db.AddInt(ref cmd, "@f_uid", uid);
            db.ExecuteNonQuery(ref cmd);
        }

        public override void Clear()
        {
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand("TRUNCATE TABLE down_files ");
            db.ExecuteNonQuery(ref cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public override string all_uncmp(int uid)
        {
            List<DnFileInf> files = new List<DnFileInf>();
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            sb.Append(" f_id");//0
            sb.Append(",f_nameLoc");//1
            sb.Append(",f_pathLoc");//2
            sb.Append(",f_perLoc");//3
            sb.Append(",f_sizeSvr");//7
            sb.Append(",f_fdTask");//10
            //
            sb.Append(" from down_files");
            //
            sb.Append(" where f_uid=? and f_complete=False");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddInt(ref cmd, "@f_uid", uid);
            DbDataReader r = db.ExecuteReader(cmd);

            while (r.Read())
            {
                DnFileInf f = new DnFileInf();
                f.id = r.GetString(0);
                f.nameLoc = r.GetString(1);
                f.pathLoc = r.GetString(2);
                f.perLoc = r.GetString(3);
                f.sizeSvr = r.GetString(4);
                int ftk = r.GetInt32(5);
                if (ftk == 1) f.fdTask = true;
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
        public override string all_complete(int uid)
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
            sb.Append(" where f_uid=? and f_deleted=False and f_complete=TRUE and f_fdChild=FALSE and f_scan=TRUE");

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sb.ToString());
            db.AddInt(ref cmd, "@f_uid", uid);
            DbDataReader r = db.ExecuteReader(cmd);

            while (r.Read())
            {
                DnFileInf f = new DnFileInf();
                f.id = Guid.NewGuid().ToString("N");
                f.f_id = r.GetString(0);
                var ftk = r.GetValue(1).ToString().ToLower();
                if (ftk == "true"||ftk=="1") f.fdTask = true;
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