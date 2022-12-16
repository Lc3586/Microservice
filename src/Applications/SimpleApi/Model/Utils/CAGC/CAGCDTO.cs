/* 
 * 自动生成代码相关业务模型
 */
namespace Model.Utils.CAGC.CAGCDTO
{
    /// <summary>
    /// 临时文件信息
    /// </summary>
    public class TempInfo
    {
        /// <summary>
        /// 占用空间
        /// </summary>
        public string OccupiedSpace { get; set; }

        /// <summary>
        /// 文件数量
        /// </summary>
        public int FileCount { get; set; }
    }

    /// <summary>
    /// 清理临时文件返回信息
    /// </summary>
    public class ClearnTempResult
    {
        /// <summary>
        /// 释放空间
        /// </summary>
        public string FreeSpace { get; set; }

        /// <summary>
        /// 文件数量
        /// </summary>
        public int FileCount { get; set; }
    }
}