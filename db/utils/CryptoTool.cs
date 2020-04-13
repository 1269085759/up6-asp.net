using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace up6.db.utils
{
    public class CryptoTool
    {
        string key = "2C4DD1CC9KAX4TA9";
        string iv  = "2C4DD1CC9KAX4TA9";

        /// <summary>
        /// aes-cbc-解密
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public string decode(string txt)
        {
            byte[] arrTxt = Convert.FromBase64String(txt);
            MemoryStream ms = new MemoryStream();
            ms.Write(arrTxt, 0, arrTxt.Length);

            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(iv);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.Zeros;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(ms.ToArray(), 0, (int)ms.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stm"></param>
        /// <param name="lenOri">原始块长度</param>
        /// <returns></returns>
        public System.IO.MemoryStream decode(System.IO.Stream stm,int lenOri)
        {
            stm.Seek(0, SeekOrigin.Begin);
            //改为BLOCKSIZE的倍数
            int len = (int)stm.Length % 16;
            if(len > 0)
            {
                len = (16 - len) + (int)stm.Length;
                stm.SetLength(len);
            }
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(iv);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.Zeros;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] dataIn = new byte[stm.Length];
            stm.Read(dataIn, 0, (int)stm.Length);
            byte[] res = cTransform.TransformFinalBlock(dataIn, 0, dataIn.Length);
            System.IO.MemoryStream ms = new System.IO.MemoryStream(res,0,lenOri);
            return ms;
        }

        /// <summary>
        /// 完善逻辑，补齐文本长度，必须是16的倍数
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public string encode(string v)
        {
            byte[] arrTxt = UTF8Encoding.UTF8.GetBytes(v);
            MemoryStream ms = new MemoryStream();
            ms.Write(arrTxt, 0, arrTxt.Length);

            int len = v.Length % 16;
            if (len > 0)
            {
                len = (16 - len) + v.Length;
                ms.SetLength(len);
                ms.Seek(0, SeekOrigin.Begin);
                ms.Write(arrTxt, 0, arrTxt.Length);
            }

            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(iv);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.Zeros;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(ms.ToArray(), 0, (int)ms.Length);

            return Convert.ToBase64String(resultArray);
        }

        public string cbc_encode(string key, string iv, string v)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(iv);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(v);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.Zeros;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray);
        }

        public string cbc_decode(string key, string iv, string toDecrypt)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(iv);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.Zeros;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}