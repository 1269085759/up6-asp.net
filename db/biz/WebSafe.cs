using up6.db.model;
using up6.db.utils;
using up6.filemgr.app;

namespace up6.db.biz
{
    /// <summary>
    /// Web安全模板
    /// </summary>
    public class WebSafe
    {
        /// <summary>
        /// 难证token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="f"></param>
        /// <param name="action">动作：init,block</param>
        /// <returns></returns>
        public bool validToken(string token, FileInf f,string action="init")
        {
            ConfigReader cr = new ConfigReader();
            var sec = cr.module("path");
            var encrypt = (bool)sec.SelectToken("$.security.token");
            if (encrypt)
            {
                if (string.IsNullOrEmpty(token.Trim())) return false;
                CryptoTool ct = new CryptoTool();
                return ct.token(f,action) == token;
            }
            return true;
        }
    }
}