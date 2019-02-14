using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace up6.db.utils
{
    public class Md5Tool
    {
        public static string calc(byte[] data)
        {
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(data);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
            }
            return strbul.ToString();
        }

        public static string calc(Stream s)
        {
            byte[] data = new byte[s.Length];
            s.Read(data, 0, (int)s.Length);

            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(data);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
            }
            return strbul.ToString();
        }
    }
}