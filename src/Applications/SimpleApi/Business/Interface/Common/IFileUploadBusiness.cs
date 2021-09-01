using Microsoft.AspNetCore.Http;
using Model.Common.FileUploadDTO;
using Model.Common.PersonalFileInfoDTO;
using System.Threading.Tasks;

namespace Business.Interface.Common
{
    /// <summary>
    /// 文件上传业务接口类
    /// </summary>
    public interface IFileUploadBusiness
    {
        #region 文件操作

        /// <summary>
        /// 预备上传文件
        /// </summary>
        /// <param name="md5">文件MD5值</param>
        /// <param name="filename">文件重命名</param>
        /// <param name="section">是否分片处理</param>
        /// <param name="type">文件类型</param>
        /// <param name="extension">文件拓展名</param>
        /// <param name="specs">分片文件规格</param>
        /// <param name="total">分片文件总数</param>
        /// <returns></returns>
        PreUploadFileResponse PreUploadFile(string md5, string filename, bool section = false, string type = null, string extension = null, int? specs = null, int? total = null);

        /// <summary>
        /// 预备上传分片文件
        /// </summary>
        /// <param name="file_md5">文件MD5值</param>
        /// <param name="md5">分片文件MD5值</param>
        /// <param name="index">分片文件索引</param>
        /// <param name="specs">分片文件规格</param>
        /// <param name="forced">强制上传</param>
        /// <returns><see cref="Model.Common.FileState"/>文件状态</returns>
        PreUploadChunkFileResponse PreUploadChunkFile(string file_md5, string md5, int index, int specs, bool forced = false);

        /// <summary>
        /// 单分片文件上传
        /// </summary>
        /// <remarks>单个上传</remarks>
        /// <param name="key">上传标识</param>
        /// <param name="md5">分片文件MD5值</param>
        /// <param name="file">分片文件</param>
        /// <returns></returns>
        Task SingleChunkFile(string key, string md5, IFormFile file);

        /// <summary>
        /// 单分片文件上传
        /// </summary>
        /// <remarks>单个上传</remarks>
        /// <param name="key">上传标识</param>
        /// <param name="md5">分片文件MD5值</param>
        /// <returns></returns>
        Task SingleChunkFileByArrayBuffer(string key, string md5);

        /// <summary>
        /// 分片文件全部上传完毕
        /// </summary>
        /// <param name="file_md5">文件MD5值</param>
        /// <param name="specs">分片文件规格</param>
        /// <param name="total">分片文件总数</param>
        /// <param name="type">文件类型</param>
        /// <param name="extension">文件拓展名</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        PersonalFileInfo UploadChunkFileFinished(string file_md5, int specs, int total, string type, string extension, string filename);

        /// <summary>
        /// 通过Base64字符串上传单个图片
        /// </summary>
        /// <param name="base64">Base64字符串</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        Task<PersonalFileInfo> SingleImageFromBase64(string base64, string filename = null);

        /// <summary>
        /// 通过外链上传单个文件
        /// </summary>
        /// <param name="url">外链地址</param>
        /// <param name="filename">文件重命名</param>
        /// <param name="download">是否下载资源</param>
        /// <returns></returns>
        Task<PersonalFileInfo> SingleFileFromUrl(string url, string filename, bool download = false);

        /// <summary>
        /// 上传单个文件
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        Task<PersonalFileInfo> SingleFile(IFormFile file, string filename = null);

        /// <summary>
        /// 上传单个文件
        /// </summary>
        /// <param name="type">上传标识</param>
        /// <param name="extension">文件拓展名</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        Task<PersonalFileInfo> SingleFileByArrayBuffer(string type, string extension, string filename);

        #endregion
    }
}
