using System;
using System.IO;
using up6.db.model;

namespace up6.db.biz
{
    /// <summary>
    /// 根据UUID生成存储路径
    /// 所有文件按原始文件名称存储
    /// 所有文件夹中的文件按原始文件名称存储
    /// 文件存在重复
    /// </summary>
    public class PathBuilderUuid : PathBuilder
    {
        public override string genFolder(ref FileInf fd)
        {
            var uuid = fd.id; //取消生成新ID,使用原始文件夹ID
            DateTime timeCur = DateTime.Now;
            string path = Path.Combine(this.getRoot(), timeCur.ToString("yyyy"));
            path = Path.Combine(path, timeCur.ToString("MM"));
            path = Path.Combine(path, timeCur.ToString("dd"));
            path = Path.Combine(path, uuid);
            path = Path.Combine(path, fd.nameLoc);

            return path;
        }

        /// <summary>
        /// 保留原始文件名称
        /// 文件存在重复
        /// 格式：
        ///     upload/uid/年/月/日/uuid/file_name
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public override string genFile(int uid, ref FileInf f)
        {
            var uuid = f.id;//取消生成ID，使用自已的ID
            DateTime timeCur = DateTime.Now;
            string path = Path.Combine(this.getRoot(), timeCur.ToString("yyyy"));
            path = Path.Combine(path, timeCur.ToString("MM"));
            path = Path.Combine(path, timeCur.ToString("dd"));
            path = Path.Combine(path, uuid);
            path = Path.Combine(path, f.nameLoc);

            return path;
        }
    }
}