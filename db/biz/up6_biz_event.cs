using System;
using System.Collections.Generic;
using System.Web;
using up6.db.biz.folder;
using up6.db.model;

namespace up6.db.biz
{
    public class up6_biz_event
    {
        public static void file_create_same(FileInf f) { }
        public static void file_create(FileInf f) { }
        public static void file_post_complete(string id) { }
        public static void file_post_block(string id,int blockIndex) { }
        public static void file_post_process(string id) { }
        public static void folder_create(fd_root fd) { }
        public static void folder_post_complete(string id) { }
        /// <summary>
        /// 文件和文件夹都触发
        /// </summary>
        /// <param name="id"></param>
        public static void file_del(string id,int uid) { }
    }
}