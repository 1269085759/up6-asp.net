using Newtonsoft.Json;
using System;
using System.IO;
using System.Web;
using up6.down2.biz;

namespace up6.filemgr.app
{
    public partial class down2_svr : WebBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string op = Request.QueryString["op"];
            if (op == "init") this.file_init();
            else if (op == "del") this.file_del();
            else if (op == "down") this.file_down();
            else if (op == "proc") this.file_proc();
        }

        void file_init()
        {
            string id = Request.QueryString["id"];
            string uid = Request.QueryString["uid"];
            string fdTask = Request.QueryString["fdTask"];
            string nameLoc = Request.QueryString["nameLoc"];//客户端使用的是encodeURIComponent编码，
            string pathLoc = Request.QueryString["pathLoc"];//客户端使用的是encodeURIComponent编码，
            string lenSvr = Request.QueryString["lenSvr"];
            string sizeSvr = Request.QueryString["sizeSvr"];
            string cbk = Request.QueryString["callback"];//应用于jsonp数据
            pathLoc = HttpUtility.UrlDecode(pathLoc);//utf-8解码
            nameLoc = HttpUtility.UrlDecode(nameLoc);
            sizeSvr = HttpUtility.UrlDecode(sizeSvr);

            if (string.IsNullOrEmpty(uid)
                || string.IsNullOrEmpty(pathLoc)
                || string.IsNullOrEmpty(lenSvr))
            {
                Response.Write(cbk + "({\"value\":null})");
                Response.End();
                return;
            }

            down2.model.DnFileInf inf = new down2.model.DnFileInf();
            inf.id = id;
            inf.uid = int.Parse(uid);
            inf.nameLoc = nameLoc;
            inf.pathLoc = pathLoc;//记录本地存储位置
            inf.lenSvr = long.Parse(lenSvr);
            inf.sizeSvr = sizeSvr;
            inf.fdTask = fdTask == "1";
            DnFile db = new DnFile();
            db.Add(ref inf);

            string json = JsonConvert.SerializeObject(inf);
            json = HttpUtility.UrlEncode(json);
            json = json.Replace("+", "%20");
            json = cbk + "({\"value\":\"" + json + "\"})";//返回jsonp格式数据。
            PageTool.to_content(json);
        }
        void file_del()
        {
            string fid = Request.QueryString["id"];
            string uid = Request.QueryString["uid"];
            string cbk = Request.QueryString["callback"];

            if (string.IsNullOrEmpty(uid)
                || string.IsNullOrEmpty(fid))
            {
                Response.Write(cbk + "({\"value\":null})");
                return;
            }

            DnFile db = new DnFile();
            db.Delete(fid, int.Parse(uid));

            PageTool.to_content(cbk + "({\"value\":1})");
        }
        void file_down()
        {
            string id = Request.Headers["id"];//文件id
            string blockIndex = Request.Headers["blockIndex"];//基于1
            string blockOffset = Request.Headers["blockOffset"];//块偏移，相对于整个文件
            string blockSize = Request.Headers["blockSize"];//块大小（当前需要下载的）
            string pathSvr = Request.Headers["pathSvr"];//文件在服务器的位置
            pathSvr = HttpUtility.UrlDecode(pathSvr);

            if ( this.head_val_null_empty("id, blockIndex, blockOffset, pathSvr"))
            {
                Response.StatusCode = 500;
                var o = this.head_to_json();
                PageTool.to_content(o);
                return;
            }

            Response.Clear();
            Stream iStream = null;
            try
            {
                // Open the file.
                iStream = new FileStream(pathSvr, FileMode.Open, FileAccess.Read, FileShare.Read);
                iStream.Seek(long.Parse(blockOffset), SeekOrigin.Begin);//定位

                // Total bytes to read:
                long dataToRead = long.Parse(blockSize);

                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Length", blockSize);

                int buf_size = Math.Min(1048576, int.Parse(blockSize));
                byte[] buffer = new Byte[buf_size];
                int length;
                while (dataToRead > 0)
                {
                    // Verify that the client is connected.
                    if (Response.IsClientConnected)
                    {
                        // Read the data in buffer.
                        length = iStream.Read(buffer, 0, buf_size);
                        dataToRead -= length;

                        // Write the data to the current output stream.
                        Response.OutputStream.Write(buffer, 0, length);

                        // Flush the data to the HTML output.
                        Response.Flush();
                    }
                    else
                    {
                        //prevent infinite loop if user disconnects
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                // Trap the error, if any.
                Response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    //Close the file.
                    iStream.Close();
                }
            }
        }
        void file_proc()
        {
            string fid = Request.QueryString["id"];
            string uid = Request.QueryString["uid"];
            string lenLoc = Request.QueryString["lenLoc"];
            string per = Request.QueryString["perLoc"];
            string cbk = Request.QueryString["callback"];
            //

            if (string.IsNullOrEmpty(uid)
                || string.IsNullOrEmpty(fid)
                || string.IsNullOrEmpty(cbk)
                || string.IsNullOrEmpty(lenLoc))
            {
                Response.Write(cbk + "({\"value\":0})");
                Response.End();
                return;
            }

            DnFile db = new DnFile();
            db.process(fid, int.Parse(uid), lenLoc, per);

            PageTool.to_content(cbk + "({\"value\":1})");
        }
    }
}