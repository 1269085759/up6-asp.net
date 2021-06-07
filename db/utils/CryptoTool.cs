using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using up6.db.model;

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

            return Convert.ToBase64String(resultArray,Base64FormattingOptions.None);
        }

        /// <summary>
        /// 计算token，进行授权检查
        /// token = encode( md5(id+nameLoc))
        /// </summary>
        /// <param name="f"></param>
        /// <param name="action">动作：init,block,cmp</param>
        /// <returns></returns>
        public string token(FileInf f,string action="init")
        {
            string str = f.id + f.nameLoc + action;
            if (action == "block") str = f.id + f.pathSvr + action;
            str = this.md5(str);
            str = this.encode(str);
            return str;
        }

        public string md5(string s)
        {
            byte[] sor = Encoding.UTF8.GetBytes(s);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            }
            return strbul.ToString();
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