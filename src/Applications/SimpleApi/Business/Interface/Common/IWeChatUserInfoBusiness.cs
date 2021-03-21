using Model.Common.WeChatUserInfoDTO;
using Model.Utils.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interface.Common
{
    /// <summary>
    /// 微信用户信息业务接口类
    /// </summary>
    public interface IWeChatUserInfoBusiness
    {
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<List> GetList(PaginationDTO pagination);

        /// <summary>
        /// 获取详情数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Detail GetDetail(string id);

        /// <summary>
        /// 获取State参数
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        string GetState(StateInfo data);

        /// <summary>
        /// 系统用户登录
        /// </summary>
        /// <param name="state"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task UserLogin(string state, string token);

        /// <summary>
        /// 获取操作说明
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        Task<string> GetExplain(string state);

        /// <summary>
        /// 微信确认操作
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        Task Confirm(string state);

        /// <summary>
        /// 微信取消操作
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        Task Cancel(string state);
    }
}
