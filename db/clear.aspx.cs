using System;
using up6.db.database;

namespace up6.db
{
    public partial class clear : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DBConfig cfg = new DBConfig();
            DBFile db = cfg.db();
            db.Clear();
        }
    }
}