using System;

namespace up6.demoSql2005.db
{
    public partial class f_process : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string md5      = Request.QueryString["md5"];
            string guid     = Request.QueryString["guid"];
            string uid      = Request.QueryString["uid"];
            string lenLoc   = Request.QueryString["lenLoc"];
            string lenSvr   = Request.QueryString["lenSvr"];
            string perSvr   = Request.QueryString["perSvr"];
            string offset   = Request.QueryString["RangePos"];
            string sizeLoc  = Request.QueryString["sizeLoc"];
            string callback = Request.QueryString["callback"];//jsonp参数

            if( !string.IsNullOrEmpty(guid))
            {
                DBFile db = new DBFile();
                db.f_process(int.Parse(uid), guid, long.Parse(offset), long.Parse(lenSvr), perSvr, false);
            }
        }
    }
}