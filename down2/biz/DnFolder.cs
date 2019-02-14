using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Common;
using up6.db.database;
using up6.down2.model;

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