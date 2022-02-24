using Microservice.Library.File.Model;
using Model.Common.FileDTO;
using Model.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interface.Common
{
    /// <summary>
    /// 文件信息业务接口类
    /// </summary>
    public interface IFileBusiness
    {
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

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        /// <returns></returns>
        void Delete(List<string> ids);

        /// <summary>
        /// 预览
        /// </summary>
        /// <remarks>用于查看文件缩略图或视频截图</remarks>
        /// <param name="id">文件Id</param>
        /// <param name="width">指定宽度</param>
        /// <param name="height">指定高度</param>
        /// <param name="time">视频的时间轴位置</param>
        /// <returns></returns>
        Task Preview(string id, int? width = null, int? height = null, TimeSpan? time = null);

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
        /// <param name="rename">文件重命名</param>
        /// <returns></returns>
        Task Download(string id, string rename = null);

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="id">文件Id</param>
        /// <param name="dirPath">文件存放路径（文件夹）</param>
        /// <param name="rename">文件重命名</param>
        /// <returns></returns>
        Task Download(string id, string dirPath, string rename = null);

        #region 拓展功能

        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <param name="extension">文件拓展名</param>
        /// <returns></returns>
        string GetFileTypeByExtension(string extension);

        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <param name="mime">MIME类型</param>
        /// <returns></returns>
        string GetFileTypeByMIME(string mime);

        /// <summary>
        /// 获取文件类型预览图链接地址
        /// </summary>
        /// <param name="extension">文件拓展名</param>
        /// <returns></returns>
        Task GetFileTypeImageUrl(string extension);

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="length">文件字节数</param>
        /// <returns></returns>
        string GetFileSize(string length);

        /// <summary>
        /// 获取视频信息
        /// </summary>
        /// <param name="id">文件Id</param>
        /// <param name="format">获取有关输入多媒体流的容器格式的信息</param>
        /// <param name="streams">获取有关输入多媒体流中包含的每个媒体流的信息</param>
        /// <param name="chapters">获取有关以该格式存储的章节的信息</param>
        /// <param name="programs">获取有关程序及其输入多媒体流中包含的流的信息</param>
        /// <param name="version">获取与程序版本有关的信息、获取与库版本有关的信息、获取与程序和库版本有关的信息</param>
        /// <returns></returns>
        VideoInfo GetVideoInfo(string id, bool format = true, bool streams = false, bool chapters = false, bool programs = false, bool version = false);

        /// <summary>
        /// 获取文件库信息
        /// </summary>
        /// <returns></returns>
        List<LibraryInfo> GetLibraryInfo();

        /// <summary>
        /// 获取所有文件类型
        /// </summary>
        /// <returns></returns>
        List<string> GetFileTypes();

        /// <summary>
        /// 获取所有文件存储类型
        /// </summary>
        /// <returns></returns>
        List<string> GetStorageTypes();

        /// <summary>
        /// 获取所有文件状态
        /// </summary>
        /// <returns></returns>
        List<string> GetFileStates();

        #endregion
    }
}
