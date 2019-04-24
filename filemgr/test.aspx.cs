using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using up6.filemgr.app;

namespace up6.filemgr
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            JArray ids = new JArray {
                new JObject{ { "f_id", "38699a00ef9845aeb98687384ec3316c" } }
                ,new JObject{ { "f_id", "b87716eaa7fb4df38c120e464cb398fb" } }
            };
            SqlExec se = new SqlExec();
            se.exec_batch("up6_folders"
                , "update up6_folders set f_deleted=1 where f_id=@f_id"
                , string.Empty
                , "f_id"
                , ids);
        }
    }
}