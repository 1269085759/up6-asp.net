using System;
using up6.db.database;

namespace up6.db
{
    public partial class clear : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DBFile.Clear();
            DBFolder.Clear();
        }
    }
}