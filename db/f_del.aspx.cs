﻿using System;
using up6.db.biz;
using up6.db.database;
using up6.filemgr.app;

namespace up6.db
{
    public partial class f_del : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string fid = this.reqString("id");
            string uid = this.reqString("uid");
            string callback = this.reqStringSafe("callback");
            int ret = 0;

            if (string.IsNullOrEmpty(fid) || 
                string.IsNullOrEmpty(uid)
                )
            {
            }//参数不为空
            else
            {
                DBConfig cfg = new DBConfig();
                DBFile db = cfg.db();
                db.Delete(Convert.ToInt32(uid), fid);
                up6_biz_event.file_del(fid,Convert.ToInt32(uid));
                ret = 1;
            }
            this.toContentJson(callback + "(" + ret + ")");//返回jsonp格式数据
        }
    }
}