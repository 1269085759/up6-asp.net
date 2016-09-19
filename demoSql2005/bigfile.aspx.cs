using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HttpUploaderWeb.demoSql2005
{
	public partial class bigfile : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			FileStream fs = File.OpenWrite("d:\\test.db");
			BinaryWriter w = new BinaryWriter(fs);
			for (long i = 0; i < 6442450944; ++i)
			{
				w.Write((byte)0);
			}
			w.Close();
			fs.Close();
		}
	}
}