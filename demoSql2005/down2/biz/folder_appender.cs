using up6.demoSql2005.db;
using up6.demoSql2005.down2.model;
using System.Collections.Generic;
using System.Data.Common;

namespace up6.demoSql2005.down2.biz
{
    public class folder_appender
    {
        public folder_appender()
        {
        }

        public void add(ref DnFolderInf fd)
        {
            string sql = "fd_add_batch";
            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetCommandStored(sql);
            db.AddInt(ref cmd, "@f_count", fd.files.Count+1);//单独增加一个文件夹
            db.AddInt(ref cmd, "@uid", fd.uid);

            cmd.Connection.Open();
            var r = cmd.ExecuteReader();
            List<int> id_lst = new List<int>();
            while (r.Read())
            {
                id_lst.Add(r.GetInt32(0));
            }
            r.Close();
            cmd.Parameters.Clear();
            int[] ids = id_lst.ToArray();

            //批量更新文件
            sql = "update down_files set ";
            sql += " f_nameLoc=@f_nameLoc";
            sql += ",f_pathLoc=@f_pathLoc";
            sql += ",f_fileUrl=@f_fileUrl";
            sql += ",f_lenSvr=@f_lenSvr";
            sql += ",f_sizeSvr=@f_sizeSvr";
            sql += ",f_pidRoot=@f_pidRoot";
            sql += ",f_fdTask=@f_fdTask";
            sql += " where f_id=@f_id";
            cmd.CommandText = sql;
            cmd.CommandType = System.Data.CommandType.Text;
            db.AddString(ref cmd, "@f_nameLoc", "", 255);
            db.AddString(ref cmd, "@f_pathLoc", "", 255);
            db.AddString(ref cmd, "@f_fileUrl", "", 255);
            db.AddInt64(ref cmd, "@f_lenSvr", 0);
            db.AddString(ref cmd, "@f_sizeSvr", "", 10);
            db.AddInt(ref cmd, "@f_pidRoot", 0);
            db.AddBool(ref cmd, "@f_fdTask",false);
            db.AddInt(ref cmd, "@f_id", 0);
            cmd.Prepare();

            System.Diagnostics.Debug.Write("ids总数:"+ids.Length+"\n");
            System.Diagnostics.Debug.Write("files总数:"+fd.files.Count+"\n");
                        
            //更新文件夹
            fd.idSvr = ids[0];
            fd.fdTask = true;
            this.update_file(ref cmd, fd);

            //更新文件
            for(int i = 1,f_index=0 , l = ids.Length;i< l;++i,++f_index)
            {
                fd.files[f_index].idSvr = ids[i];
                fd.files[f_index].pidRoot = fd.idSvr;

                this.update_file(ref cmd, fd.files[f_index]);
            }
            cmd.Connection.Close();
        }

        void update_file(ref DbCommand cmd,DnFileInf f)
        {
            cmd.Parameters["@f_nameLoc"].Value = f.nameLoc;
            cmd.Parameters["@f_pathLoc"].Value = f.pathLoc;
            cmd.Parameters["@f_fileUrl"].Value = f.fileUrl;
            cmd.Parameters["@f_lenSvr"].Value = f.lenSvr;
            cmd.Parameters["@f_sizeSvr"].Value = f.sizeSvr;
            cmd.Parameters["@f_pidRoot"].Value = f.pidRoot;
            cmd.Parameters["@f_fdTask"].Value = f.fdTask;
            cmd.Parameters["@f_id"].Value = f.idSvr;
            cmd.ExecuteNonQuery();
        }
    }
}