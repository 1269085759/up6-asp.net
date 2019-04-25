using Microsoft.Experimental.IO;
using System.IO;
using System.Threading;
using System.Web;
using up6.filemgr.app;

namespace up6.db.utils
{
    /// <summary>
    /// 文件块处理器
    /// 优化文件创建逻辑，按文件实际大小创建
    /// </summary>
    public class FileBlockWriter
	{
		public long m_lenLoc;		//文件总大小。

		//文件读写锁，防止多个用户同时上传相同文件时，出现创建文件的错误
		static ReaderWriterLock m_writeLock = new ReaderWriterLock();

		public FileBlockWriter()
		{
		}

		/// <summary>
		/// 根据文件大小创建文件。
		/// 注意：多个用户同时上传相同文件时，可能会同时创建相同文件。
		/// </summary>
		public void make(string filePath,long len)
		{
			//文件不存在则创建
            if (string.IsNullOrEmpty(filePath)) return;
			if (!LongPathFile.Exists(filePath))
			{
                var pos = filePath.LastIndexOf('\\');
                if (-1 == pos) pos = filePath.LastIndexOf('/');
                var dir = filePath.Substring(0, pos);
                //自动创建目录
                if (!LongPathDirectory.Exists(dir))
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("路径不存在：{0}", dir));
                    PathTool.createDirectory(dir);
                }
                else {
                    System.Diagnostics.Debug.WriteLine(string.Format("路径存在：{0}", dir));
                }

                var fs = LongPathFile.Open(filePath, FileMode.Create, FileAccess.Write);
                fs.SetLength(len);
                fs.Close();
			}
		}

		/// <summary>
		/// 续传文件
		/// </summary>
		/// <param name="fileRange">文件块</param>
		/// <param name="path">远程文件完整路径。d:\www\web\upload\201204\10\md5.exe</param>
		public void write(string path, long offset, ref HttpPostedFile fileRange)
		{
			//文件已存在，写入数据
			FileStream fs = LongPathFile.Open(path, FileMode.Open, FileAccess.Write, FileShare.Write);
			fs.Seek(offset, SeekOrigin.Begin);                
			byte[] ByteArray = new byte[fileRange.InputStream.Length];
            fileRange.InputStream.Seek(0, SeekOrigin.Begin);
            fileRange.InputStream.Read(ByteArray, 0, (int)fileRange.InputStream.Length);
			fs.Write(ByteArray, 0, (int)fileRange.InputStream.Length);
			fs.Flush();
			fs.Close();
		}
	}
}
