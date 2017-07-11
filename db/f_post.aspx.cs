using System;
using System.Web;
using up6.db.utils;

namespace up6.db
{
    public partial class f_post : System.Web.UI.Page
    {
        bool safe_check(params string[] ps)
        {
            foreach (var v in ps)
            {
                if (string.IsNullOrEmpty(v)) return false;
            }
            foreach (string key in Request.Headers.Keys)
            {
                var vs = Request.Headers.GetValues(key);
                XDebug.Output("key:" + key + String.Join(",", vs));
            }
            return true;
        }

        /// <summary>
        /// 只负责拼接文件块。将接收的文件块数据写入到文件中。
        /// 更新记录：
        ///		2012-04-12 更新文件大小变量类型，增加对2G以上文件的支持。
        ///		2012-04-18 取消更新文件上传进度信息逻辑。
        ///		2012-10-30 增加更新文件进度功能。
        ///		2015-03-19 文件路径由客户端提供，此页面不再查询文件在服务端的路径。减少一次数据库访问操作。
        ///     2016-03-31 增加文件夹信息字段
        ///     2017-07-11 优化参数检查逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string uid          = Request.Headers["uid"];
            string f_id         = Request.Headers["id"];
            string lenSvr       = Request.Headers["lenSvr"];//已传大小
            string lenLoc       = Request.Headers["lenLoc"];//本地文件大小
            string blockOffset  = Request.Headers["blockOffset"];
            string blockSize    = Request.Headers["blockSize"];//当前块大小
            string blockIndex   = Request.Headers["blockIndex"];//当前块索引，基于1
            string complete     = Request.Headers["complete"];//true/false
            string pathSvr      = Request.Headers["pathSvr"];//add(2015-03-19):
            pathSvr             = HttpUtility.UrlDecode(pathSvr);

            if( !this.safe_check(lenLoc,uid,f_id,blockOffset,pathSvr)) return;

            //有文件块数据
            if (Request.Files.Count > 0)
            {
                long offset = Convert.ToInt64(blockOffset);

                //临时文件大小
                HttpPostedFile file = Request.Files.Get(0);


                //2.0保存文件块数据
                FileBlockWriter res = new FileBlockWriter();
                res.write(pathSvr, offset, ref file);
                
                Response.Write("ok");
            }
        }
    }
}