using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using up6.filemgr.app;

namespace up6.filemgr
{
    public partial class index : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string op = Request.QueryString["op"];

            if (op == "data") this.load_data();
        }

        void load_data() {
            SqlExec se = new SqlExec();
            var data = DbBase.page_to_layer_table("up6_files"
                , "f_id"
                ,"f_id,f_nameLoc,f_sizeLoc,f_time"
                ,"f_fdChild=0 and f_complete=1"
                ,"f_time desc");

            PageTool.to_content(data);
        }
    }
}