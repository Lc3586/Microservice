

namespace Model.Common
{
    /// <summary>
    /// 文件类型
    /// </summary>
    public static class FileType
    {
        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <param name="extension">文件扩展名(.jpg)</param>
        /// <returns></returns>
        public static string GetFileType(string extension)
        {
            switch (extension)
            {
                case ".webp":
                case ".jpg":
                case ".png":
                case ".ioc":
                case ".bmp":
                case ".gif":
                case ".tif":
                case ".tga":
                    return 图片;
                case ".mp2":
                case ".ac3":
                case ".mp3":
                case ".m4a":
                case ".m4r":
                case ".mmf":
                case ".ogg":
                case ".amr":
                case ".aac":
                case ".vqf":
                case ".wma":
                case ".ape":
                case ".wav":
                case ".flac":
                case ".cda":
                case ".dts":
                    return 音频;
                case ".swf":
                case ".3gp":
                case ".3g2":
                case ".mp4":
                case ".mpeg":
                case ".mpg":
                case ".dat":
                case ".mov":
                case ".vob":
                case ".qt":
                case ".rm":
                case ".asf":
                case ".avi":
                case ".navi":
                case ".divx":
                case ".flv":
                case ".f4v":
                case ".qsv":
                case ".wmv":
                case ".mkv":
                case ".rmvb":
                case ".webm":
                    return 视频;
                case ".xls":
                case ".xlsx":
                case ".csv":
                    return 电子表格;
                case ".doc":
                case ".docx":
                case ".pdf":
                    return 电子文档;
                case ".txt":
                    return 文本文件;
                case ".zip":
                case ".rar":
                case ".7z":
                    return 压缩包;
                default:
                    return 未知;
            }
        }

        public const string 电子文档 = "电子文档";

        public const string 电子表格 = "电子表格";

        public const string 文本文件 = "文本文件";

        public const string 图片 = "图片";

        public const string 音频 = "音频";

        public const string 视频 = "视频";

        public const string 压缩包 = "压缩包";

        public const string 未知 = "未知";

        public const string 外链资源 = "外链资源";
    }
}
