using Microsoft.AspNetCore.Http;
using Model.Common.PersonalFileInfoDTO;

/* 
 * 文件上传业务模型
 */
namespace Model.Common.FileUploadDTO
{
    /// <summary>
    /// 预备上传文件输出信息
    /// </summary>
    public class PreUploadFileResponse
    {
        /// <summary>
        /// 是否已上传过了
        /// </summary>
        /// <remarks>
        /// <para>如已上传,则返回文件信息<see cref="FileInfo"/></para>
        /// </remarks>
        public bool Uploaded { get; set; }

        /// <summary>
        /// 个人文件信息
        /// </summary>
        public PersonalFileInfo FileInfo { get; set; }
    }

    /// <summary>
    /// 预备上传分片文件输出信息
    /// </summary>
    public class PreUploadChunkFileResponse
    {
        /// <summary>
        /// 状态
        /// </summary>
        /// <remarks><see cref="Model.Common.PUCFRState"/></remarks>
        public string State { get; set; }

        /// <summary>
        /// 上传标识
        /// </summary>
        public string Key { get; set; }
    }

    /// <summary>
    /// 分片文件上传参数
    /// </summary>
    public class ChunkFileUploadParams
    {
        /// <summary>
        /// 分片文件
        /// </summary>
        public IFormFile File { get; set; }

        /// <summary>
        /// 上传标识
        /// </summary>
        public string Key { get; set; }
    }

    /// <summary>
    /// 分片文件全部上传完毕参数
    /// </summary>
    public class UploadChunkFileFinishedParams
    {
        /// <summary>
        /// 上传标识
        /// </summary>
        public string Key { get; set; }


    }

    /// <summary>
    /// 图片上传参数
    /// </summary>
    public class ImageUploadParams
    {
        /// <summary>
        /// 文件
        /// </summary>
        public IFormFile File { get; set; }

        /// <summary>
        /// 文件名
        /// 注意:不指定文件名时将使用原始名称,使用Base64时使用Guid
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 链接或Base64字符串
        /// </summary>
        public string UrlOrBase64 { get; set; }

        /// <summary>
        /// 下载外链资源链接
        /// </summary>
        public bool Download { get; set; } = false;

        ///// <summary>
        ///// 保存至数据库
        ///// </summary>
        //public bool Save2Db { get; set; } = true;

        /// <summary>
        /// 图片转Base64链接
        /// </summary>
        public bool ToBase64Url { get; set; } = false;

        /// <summary>
        /// 图片转Base64
        /// </summary>
        public bool ToBase64 { get; set; } = false;

        /// <summary>
        /// 压缩图片
        /// 默认开启
        /// </summary>
        public bool IsCompress { get; set; } = true;

        /// <summary>
        /// 图片压缩选项
        /// 默认配置:按照200像素的宽度等比压缩图片，并且保存原图
        /// </summary>
        public ImageCompressOption CompressOption { get; set; } = new ImageCompressOption();
    }

    /// <summary>
    /// 图片压缩选项
    /// </summary>
    public class ImageCompressOption
    {
        /// <summary>
        /// 保存原图
        /// </summary>
        public bool SaveOriginal { get; set; } = true;

        /// <summary>
        /// 压缩后的宽度
        /// 注意:只设置高度时将进行等比压缩
        /// </summary>
        public int Width { get; set; } = 200;

        /// <summary>
        /// 压缩后的高度
        /// </summary>
        public int Height { get; set; } = 0;
    }

    /// <summary>
    /// 文件上传参数
    /// </summary>
    public class FileUploadParams
    {
        /// <summary>
        /// 文件
        /// </summary>
        public IFormFile File { get; set; }

        /// <summary>
        /// 文件名
        /// 注意:不指定文件名时将使用原始名称,使用Base64时使用Guid
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 外链资源链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 下载外链资源链接
        /// </summary>
        public bool Download { get; set; } = false;

        ///// <summary>
        ///// 保存至数据库
        ///// </summary>
        //public bool Save2Db { get; set; } = true;

        /// <summary>
        /// 压缩文件
        /// 默认关闭
        /// </summary>
        public bool IsCompress { get; set; } = false;

        /// <summary>
        /// 文件压缩选项
        /// </summary>
        public FileCompressOption CompressOption { get; set; } = new FileCompressOption();
    }

    /// <summary>
    /// 文件压缩选项
    /// </summary>
    public class FileCompressOption
    {
        /// <summary>
        /// 压缩比例
        /// </summary>
        public double Level { get; set; } = 0.8;
    }
}
