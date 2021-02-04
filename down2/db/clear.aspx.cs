using System;
using up6.db.database;
using up6.down2.biz;

namespace up6.down2.db
{
    public partial class clear : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DBConfig cfg = new DBConfig();
            cfg.downF().Clear();
        }
    }
}