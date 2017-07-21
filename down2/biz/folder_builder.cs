using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Common;
using up6.db.database;
using up6.db.model;
using up6.down2.model;

namespace up6.down2.biz
{
    public class folder_builder
    {
        public folder_builder()
        {
        }

        public string to_json(string id)
        {
            FileInf fd = new DnFolderInf();
            DBFile dbf = new DBFile();
            dbf.read(id, ref fd);

            //初始化操作
            DnFolderInf dfi = (DnFolderInf)(fd);
            dfi.lenLoc = 0;
            dfi.sizeLoc = "0byte";
            dfi.perLoc = "0%";
            dfi.f_id = dfi.id;
            dfi.id = "";

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
            var reader = db.ExecuteReader(ref cmd);
            while (reader.Read())
            {
                DnFileInf f = new DnFileInf();
                f.f_id = reader.GetString(0);
                f.nameLoc = reader.GetString(1);
                f.pathSvr = reader.GetString(2);
                f.pathRel = reader.GetString(3);
                f.lenSvr = reader.GetInt64(4);
                f.sizeSvr = reader.GetString(5);
                dfi.files.Add(f);
            }
            reader.Close();

            return JsonConvert.SerializeObject(dfi);
        }
    }
}