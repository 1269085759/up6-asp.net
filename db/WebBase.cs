using System;

namespace up6.db
{
    public class WebBase : System.Web.UI.Page
    {
        public string reqString(string name)
        {
            if (!Request.QueryString.HasKeys()) return string.Empty;
            if (string.IsNullOrEmpty(Request.QueryString[name])) return string.Empty;
            return Request.QueryString[name].Trim();
        }

        public string reqStringDecode(string name) {
            var v = this.reqString(name);
            return Server.UrlDecode(v);
        }

        public int reqInt(string name)
        {
            var v = this.reqString(name);
            if (string.IsNullOrEmpty(v)) return 0;
            return Convert.ToInt32(v);
        }

        public long reqLong(string name)
        {
            var v = this.reqString(name);
            if (string.IsNullOrEmpty(v)) return 0;
            return Convert.ToInt64(v);
        }

        public string headString(string name)
        {
            if (!Request.Headers.HasKeys()) return string.Empty;
            if (string.IsNullOrEmpty(Request.Headers[name])) return string.Empty;
            return Request.Headers[name].Trim();
        }
    }
}