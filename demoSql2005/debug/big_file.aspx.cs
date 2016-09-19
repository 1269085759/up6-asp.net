using System;
using System.IO;

namespace up6.demoSql2005.debug
{
    public partial class big_file : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.file_create();
        }

        void create4()
        {
            File.Delete("D:\\test.rar");
            BinaryWriter bw = new BinaryWriter(new FileStream("D:\\test.rar", FileMode.Create));
            Random ran = new Random();

            long mb = 1048576;
            long gb = 1073741824;

            for (int i = 0; i < (mb * 500); ++i)
            {
                int RandKey = ran.Next(524288000);
                bw.Write((byte)RandKey);
            }
            bw.Close();
        }
        void create2()
        {
        }

        void file_create()
        {
            long mb = 1048576;
            long gb = 1073741824;
            db.FileBlockWriter fw = new db.FileBlockWriter();
            fw.make("d:\\test.db", gb * 10);
        }
    }
}