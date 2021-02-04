using System;
using System.Data.Odbc;
using up6.down2.biz;
using up6.filemgr.app;

namespace up6.filemgr
{
    public partial class test : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //OdbcConnection con = new OdbcConnection("Driver={KingbaseES 8.2 ODBC Driver ANSI};Server=192.168.0.117:54321/up6;Uid=SYSTEM;Pwd=123456");
            //con.Open();
            //con.Close();
            //var id = Request.QueryString["id"];
            //if (string.IsNullOrEmpty(id)) return;
            //DnFolder df = new DnFolder();
            //var data = df.files(id);
            //Response.Write(data);

            //FolderBuilder fb = new FolderBuilder();
            //this.toContent(fb.build(id));
            //this.toContent(this.BytesToString(long.Parse(id)));
        }

        string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
    }
}