using Microsoft.AspNetCore.Http;
using Model.Common.FileDTO;
using Model.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interface.Common
{
    /// <summary>
    /// 文件处理业务接口类
    /// </summary>
    public interface IFileBusiness
    {
        #region 数据

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<FileInfo> GetList(PaginationDTO pagination);

        /// <summary>
        /// 获取详情数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        FileInfo GetDetail(string id);

        /// <summary>
        /// 获取详情数据集合
        /// </summary>
        /// <param name="ids">id逗号拼接</param>
        /// <returns></returns>
        List<FileInfo> GetDetails(string ids);

        /// <summary>
        /// 获取详情数据集合
        /// </summary>
        /// <param name="ids">id集合</param>
        /// <returns></returns>
        List<FileInfo> GetDetails(List<string> ids);

        #endregion

        #region 文件操作

        /// <summary>
        /// 文件MD5值校验
        /// </summary>
        /// <param name="md5">文件MD5值</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        ValidationMD5Response ValidationFileMD5(string md5, string filename = null);

        /// <summary>
        /// 预备上传分片文件
        /// </summary>
        /// <param name="file_md5">文件MD5值</param>
        /// <param name="md5">分片文件MD5值</param>
        /// <param name="index">分片文件索引</param>
        /// <param name="specs">分片文件规格</param>
        /// <returns><see cref="Model.Common.FileState"/>文件状态</returns>
        PreUploadChunkFileResponse PreUploadChunkFile(string file_md5, string md5, int index, int specs);

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
        /// 分片文件全部上传完毕
        /// </summary>
        /// <param name="file_md5">文件MD5值</param>
        /// <param name="specs">分片文件规格</param>
        /// <param name="total">分片文件总数</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        FileInfo UploadChunkFileFinished(string file_md5, int specs, int total, string filename = null);

        /// <summary>
        /// 通过外链上传单个图片
        /// </summary>
        /// <param name="url">外链地址</param>
        /// <param name="download">是否下载资源</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        Task<FileInfo> SingleImageFromUrl(string url, bool download = false, string filename = null);

        /// <summary>
        /// 通过外链上传单个文件
        /// </summary>
        /// <param name="url">外链地址</param>
        /// <param name="download">是否下载资源</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        Task<FileInfo> SingleFileFromUrl(string url, bool download = false, string filename = null);

        /// <summary>
        /// 通过Base64字符串上传单个图片
        /// </summary>
        /// <param name="base64">Base64字符串</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        FileInfo SingleImageFromBase64(string base64, string filename = null);

        /// <summary>
        /// 上传单个文件
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="filename">文件重命名</param>
        /// <returns></returns>
        Task<FileInfo> SingleFile(IFormFile file, string filename = null);

        ///// <summary>
        ///// 单图上传
        ///// </summary>
        ///// <remarks>单个上传</remarks>
        ///// <param name="option">图片上传参数</param>
        ///// <returns></returns>
        //FileInfo SingleImage(ImageUploadParams option);

        ///// <summary>
        ///// 文件上传
        ///// </summary>
        ///// <remarks>单个上传</remarks>
        ///// <param name="option">文件上传参数</param>
        ///// <returns></returns>
        //Task<FileInfo> SingleFile(FileUploadParams option);

        /// <summary>
        /// 预览
        /// </summary>
        /// <remarks>用于查看文件缩略图或视频截图</remarks>
        /// <param name="id">文件Id</param>
        /// <param name="width">指定宽度</param>
        /// <param name="height">指定高度</param>
        /// <param name="time">视频的时间轴位置</param>
        /// <returns></returns>
        Task Preview(string id, int width, int height, TimeSpan? time = null);

        /// <summary>
        /// 浏览
        /// </summary>
        /// <param name="id">文件Id</param>
        /// <returns></returns>
        Task Browse(string id);

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="id">文件Id</param>
        /// <returns></returns>
        Task Download(string id);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        /// <returns></returns>
        void Delete(List<string> ids);

        #endregion
    }
}
