

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
        public static string GetFileTypeByExtension(string extension)
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
                case ".pdf":
                case ".doc":
                case ".docx":
                    return 电子文档;
                case ".txt":
                case ".js":
                case ".css":
                case ".cs":
                case ".html":
                case ".vue":
                case ".ts":
                case ".xml":
                case ".json":
                    return 文本文件;
                case ".zip":
                case ".rar":
                case ".7z":
                    return 压缩包;
                default:
                    return 未知;
            }
        }

        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <param name="mimetype">MIME类型</param>
        /// <returns></returns>
        public static string GetFileTypeByMIME(string mimetype)
        {
            if (mimetype.StartsWith("image/"))
                return 图片;
            else if (mimetype.StartsWith("audio/"))
                return 音频;
            else if (mimetype.StartsWith("video/"))
                return 视频;
            else if (mimetype.StartsWith("text/"))
                return 文本文件;
            else
            {
                switch (mimetype)
                {
                    case "application/ogg":
                        return 音频;
                    case "application/mp4":
                        return 视频;
                    case "application/vnd.ms-excel":
                    case "vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                        return 电子表格;
                    case "application/pdf":
                    case "application/msword":
                    case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                        return 电子文档;
                    case "application/json":
                    case "application/javascript":
                        return 文本文件;
                    case "application/x-tar":
                    case "application/zip":
                    case "application/x-compressed":
                    case "application/x-zip-compressed":
                        return 压缩包;
                    default:
                        return 未知;
                }
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
