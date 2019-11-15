using Microsoft.Experimental.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace up6.filemgr.app
{
    public class PathTool
    {

        /// <summary>
        /// 自动创建多层级路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="separator">路径分隔符，默认：/</param>
        public static void createDirectory(string path, char separator = '/')
        {
            var dirs = path.Split(separator);
            var folder = "";
            foreach (var dir in dirs)
            {
                if (folder != "")
                {
                    folder = folder + "/" + dir;
                }
                else
                {
                    folder = dir;
                }
                if (!LongPathDirectory.Exists(folder))
                {
                    LongPathDirectory.Create(folder);
                }
                System.Diagnostics.Debug.WriteLine(folder);
            }
        }

        /// <summary>
        /// 合并路径，自动判断结尾符
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string combin(string a,string b)
        {
            if (a.EndsWith("/") ) return a + b;
            return a + "/" + b;
        }
    }
}