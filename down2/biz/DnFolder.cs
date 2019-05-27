using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data.Common;
using up6.db.database;
using up6.down2.model;
using up6.filemgr.app;

namespace up6.down2.biz
{
    public class DnFolder
    {
        /// <summary>
        /// 清空数据库
        /// </summary>
        public static void Clear()
        {
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand("delete from down_folders;");
            db.ExecuteNonQuery(ref cmd);
        }

        /// <summary>
        /// 获取指定目录的所有子文件
        /// </summary>
        /// <param name="id">文件夹id</param>
        /// <returns></returns>
        public string files(string id)
        {
            var se = new SqlExec();
            var fd = se.read("up6_folders", "*", new SqlParam[] { new SqlParam("f_id", id) });
            if (fd == null) return this.filesRoot(id);
            else return this.filesChild(id);
        }

        /// <summary>
        /// 取子目录所有文件
        /// </summary>
        /// <returns></returns>
        public string filesChild(string id)
        {
            var se = new SqlExec();
            var fd = se.read("up6_folders", "f_pidRoot", new SqlParam[] { new SqlParam("f_id", id) });
            var pidRoot = fd["f_pidRoot"].ToString();
            DbFolder df = new DbFolder();

            //构建子目录路径
            PathRelBuilder prb = new PathRelBuilder();
            var fs = prb.build(id,pidRoot);
            

            return JsonConvert.SerializeObject(fs);
        }

        public JToken allFiles(string pidRoot)
        {
            var se = new SqlExec();
            var files = se.select("up6_files"
                , "f_id,f_pid,f_nameLoc,f_pathSvr,f_pathRel,f_lenSvr,f_sizeLoc"
                , new SqlParam[] { new SqlParam("f_pidRoot", pidRoot) }
            );
            return files;
        }

        /// <summary>
        /// 取根节点所有文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string filesRoot(string id)
        {
            List<DnFileInf> files = new List<DnFileInf>();
            string sql = @"select 
                             f_id
                            ,f_nameLoc
                            ,f_pathSvr
                            ,f_pathRel
                            ,f_lenSvr
                            ,f_sizeLoc
                             from up6_files
                             where f_pidRoot=@pidRoot
                            ";
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sql);
            db.AddString(ref cmd, "@pidRoot", id, 32);
            var r = db.ExecuteReader(ref cmd);
            while (r.Read())
            {
                DnFileInf f = new DnFileInf();
                f.f_id = r.GetString(0);
                f.nameLoc = r.GetString(1);
                f.pathSvr = r.GetString(2);
                f.pathRel = r.GetString(3);
                f.lenSvr = r.GetInt64(4);
                f.sizeSvr = r.GetString(5);
                files.Add(f);
            }
            r.Close();

            if (files.Count > 0)
            {
                return JsonConvert.SerializeObject(files);
            }
            return string.Empty;
        }

         public static string all_file(string id)
        {
            List<DnFileInf> files = new List<DnFileInf>();
            string sql = @"select 
                             f_id
                            ,f_nameLoc
                            ,f_pathSvr
                            ,f_pathRel
                            ,f_lenSvr
                            ,f_sizeLoc
                             from up6_files
                             where f_pidRoot=@pidRoot
                            ";
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommand(sql);
            db.AddString(ref cmd, "@pidRoot", id, 32);
            var r = db.ExecuteReader(ref cmd);
            while (r.Read())
            {
                DnFileInf f = new DnFileInf();
                f.f_id = r.GetString(0);
                f.nameLoc = r.GetString(1);
                f.pathSvr = r.GetString(2);
                f.pathRel = r.GetString(3);
                f.lenSvr = r.GetInt64(4);
                f.sizeSvr = r.GetString(5);
                files.Add(f);
            }
            r.Close();

            if(files.Count>0)
            {
                return JsonConvert.SerializeObject(files);
            }
            return string.Empty;
        }
    }
}