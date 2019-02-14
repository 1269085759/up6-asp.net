using System.IO;
using System.Web;
//using System.IO.MemoryMappedFiles;

namespace up6.db.utils
{
    /// <summary>
    /// 基于内存映射文件的文件块保存类
    /// 1.可提高大文件保存的效率
    /// 2.减少系统IO
    /// </summary>
    public class MapBlockWriter
    {
        /// <summary>
        /// 创建一个文件
        /// </summary>
        /// <param name="path"></param>
        public void makeFile(string path,long len)
        {
            long mb = 1048576;//1mb
            long gb = 1073741824;//1GB

            //自动创建目录
            if (!Directory.Exists(path)) Directory.CreateDirectory(Path.GetDirectoryName(path));

            //using (var mf = MemoryMappedFile.CreateFromFile(path, FileMode.Create, "up6-mem-file", len))
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">本地文件路径</param>
        /// <param name="offset">块索引</param>
        /// <param name="length">块大小</param>
        public void write(string path,long offset, ref HttpPostedFile block)
        {
            long length = block.InputStream.Length;
            //using (var mmf = MemoryMappedFile.CreateFromFile(path, FileMode.Open))
            //{
            //    using (var view = mmf.CreateViewAccessor(offset, length))
            //    {
            //        byte[] data = new byte[block.InputStream.Length];
            //        block.InputStream.Read(data, 0, (int)block.InputStream.Length);
            //        //view.Write(0,block.InputStream.ReadByte())
            //        view.WriteArray<byte>(0, data, 0, (int)block.InputStream.Length);
            //    }
            //}
        }
    }
}