using System;

namespace up6.down2.db
{
    public partial class clear : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DnFile.Clear();
            DnFolder.Clear();
        }
    }
}