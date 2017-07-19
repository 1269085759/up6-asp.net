using System;
using System.IO;
using System.Text;
using System.Web;
using up6.db;
using up6.db.database;
using up6.db.model;

namespace up6.down2.db
{
    public partial class f_down : System.Web.UI.Page
    {
        bool check_params(params string[] vs)
        {
            foreach(string v in vs)
            {
                if (String.IsNullOrEmpty(v)) return false;
            }
            return true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string id           = Request.Headers["id"];//文件id
            string blockIndex   = Request.Headers["blockIndex"];//基于1
            string blockOffset  = Request.Headers["blockOffset"];//块偏移，相对于整个文件
            string blockSize    = Request.Headers["blockSizze"];//块大小（当前需要下载的）
            string pathSvr      = Request.Headers["pathSvr"];//文件在服务器的位置
            pathSvr             = HttpUtility.UrlDecode(pathSvr);

            if (this.check_params(id,blockIndex,blockOffset,pathSvr))
            {
                Response.StatusCode = 500;
                return;
            }

            Stream iStream = null;
            try
            {
                // Open the file.
                iStream = new FileStream(pathSvr, FileMode.Open, FileAccess.Read, FileShare.Read);
                iStream.Seek(long.Parse(blockOffset),SeekOrigin.Begin);//定位

                // Total bytes to read:
                long dataToRead = long.Parse(blockSize);

                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Length", blockSize );

                byte[] buffer = new Byte[ int.Parse( blockSize)];
                int length;
                // Verify that the client is connected.
                if (Response.IsClientConnected)
                {
                    // Read the data in buffer.
                    length = iStream.Read(buffer, 0, int.Parse(blockSize));

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
    }
}