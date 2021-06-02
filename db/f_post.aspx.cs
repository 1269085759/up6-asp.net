using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Web;
using up6.db.biz;
using up6.db.model;
using up6.db.utils;
using up6.filemgr.app;

namespace up6.db
{
    public partial class f_post : WebBase
    {
        bool safe_check(params string[] ps)
        {
            foreach (var v in ps)
            {
                System.Diagnostics.Debug.Write("参数值：");
                System.Diagnostics.Debug.WriteLine(v);
                if (string.IsNullOrEmpty(v)) return false;
            }
            foreach (string key in Request.Headers.Keys)
            {
                var vs = Request.Headers.GetValues(key);
                //XDebug.Output(key + " "+String.Join(",", vs));
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
            string uid          = this.headString("uid");
            string f_id         = this.headString("id");
            string lenSvr       = this.headString("lenSvr");//已传大小
            string lenLoc       = this.headString("lenLoc");//本地文件大小
            string blockOffset  = this.headString("blockOffset");
            string blockSize    = this.headString("blockSize");//当前块大小
            string blockIndex   = this.headString("blockIndex");//当前块索引，基于1
            string blockMd5     = this.headString("blockMd5");//块MD5
            string complete     = this.headString("complete");//true/false
            string pathSvr      = Request.Form["pathSvr"];//
            string pathLoc = string.Empty;
            string token      = Request.Form["token"];//
            pathSvr = Server.UrlDecode(pathSvr);

            if( !this.safe_check(lenLoc,uid,f_id,blockOffset,pathSvr)) return;


            //有文件块数据
            if (Request.Files.Count > 0)
            {
                bool verify = false;
                string msg = string.Empty;
                string md5Svr = string.Empty;
                HttpPostedFile file = Request.Files.Get(0);//文件块
                var stm = file.InputStream;
                var stmLen = int.Parse(blockSize);
                pathLoc = file.FileName;

                //加密
                ConfigReader cr = new ConfigReader();
                var sec = cr.module("path");
                var encrypt = (bool)sec.SelectToken("$.security.encrypt");
                if (encrypt)
                {
                    CryptoTool ct = new CryptoTool();
                    pathLoc = ct.decode(file.FileName);
                    stm = ct.decode(file.InputStream,int.Parse(blockSize));
                }

                //token验证
                WebSafe ws = new WebSafe();
                FileInf fileSvr = new FileInf();
                fileSvr.id = f_id;
                fileSvr.pathLoc = file.FileName;
                FileInfo fi = new FileInfo(pathLoc);
                fileSvr.nameLoc = fi.Name;
                var ret = ws.validToken(token, fileSvr);
                //token难失败
                verify = ret;
                if(!verify)
                {
                    msg = string.Format("token error loc:{0}",token);
                }

                //计算文件块MD5
                if (!string.IsNullOrEmpty(blockMd5))
                {
                    md5Svr = Md5Tool.calc(stm);
                }

                //文件块大小验证
                if(verify) verify = int.Parse(blockSize) == stm.Length;
                if (!verify)
                {
                    msg = "block size error sizeSvr:"+ stm.Length + " sizeLoc:"+blockSize;
                }

                //块MD5验证
                if ( verify && !string.IsNullOrEmpty(blockMd5) )
                {
                    verify = md5Svr == blockMd5;
                    if(!verify) msg = "block md5 error";
                }

                if (verify)
                {
                    PathBuilder pb = new PathBuilder();
                    pathSvr = pb.relToAbs(pathSvr);
                    //2.0保存文件块数据
                    FileBlockWriter res = new FileBlockWriter();
                    res.make(pathSvr, Convert.ToInt64(lenLoc));
                    res.write(pathSvr, Convert.ToInt64(blockOffset), stm);
                    up6_biz_event.file_post_block(f_id,Convert.ToInt32(blockIndex));

                    //生成信息
                    JObject o = new JObject();
                    o["msg"] = "ok";
                    o["md5"] = md5Svr;//文件块MD5
                    o["offset"] = blockOffset;//偏移
                    msg = JsonConvert.SerializeObject(o);
                }
                this.toContentJson(msg);
            }
        }
    }
}