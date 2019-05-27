using System;
using up6.down2.biz;
using up6.filemgr.app;

namespace up6.filemgr
{
    public partial class test : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            DnFolder df = new DnFolder();
            var data = df.files(id);
            Response.Write(data);
        }
    }
}