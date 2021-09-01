using Model.Common.PersonalFileInfoDTO;
using Model.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interface.Common
{
    /// <summary>
    /// 个人文件信息业务接口类
    /// </summary>
    public interface IPersonalFileInfoBusiness
    {
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<PersonalFileInfo> GetList(PaginationDTO pagination);

        /// <summary>
        /// 获取详情数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PersonalFileInfo GetDetail(string id);

        /// <summary>
        /// 获取详情数据集合
        /// </summary>
        /// <param name="ids">id逗号拼接</param>
        /// <returns></returns>
        List<PersonalFileInfo> GetDetails(string ids);

        /// <summary>
        /// 获取详情数据集合
        /// </summary>
        /// <param name="ids">id集合</param>
        /// <returns></returns>
        List<PersonalFileInfo> GetDetails(List<string> ids);

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        void Rename(string id, string filename);

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
        /// <param name="id">Id</param>
        /// <param name="width">指定宽度</param>
        /// <param name="height">指定高度</param>
        /// <param name="time">视频的时间轴位置</param>
        /// <returns></returns>
        Task Preview(string id, int width, int height, TimeSpan? time = null);

        /// <summary>
        /// 浏览
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        Task Browse(string id);

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        Task Download(string id);
    }
}
