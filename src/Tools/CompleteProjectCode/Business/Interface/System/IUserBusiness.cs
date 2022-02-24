using Microservice.Library.SelectOption;
using Model.Common;
using Model.System.UserDTO;
using Model.Utils.Pagination;
using Model.Utils.SampleAuthentication.SampleAuthenticationDTO;
using System.Collections.Generic;
using System.Security.Claims;

namespace Business.Interface.System
{
    /// <summary>
    /// 系统用户业务接口类
    /// </summary>
    public interface IUserBusiness
    {
        #region 基础功能

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<List> GetList(PaginationDTO pagination);

        /// <summary>
        /// 获取下拉框数据
        /// </summary>
        /// <param name="condition">关键词(多个用空格分隔)</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<SelectOption> DropdownList(string condition, PaginationDTO pagination);

        /// <summary>
        /// 获取详情数据
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        Detail GetDetail(string id);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        /// <returns></returns>
        void Create(Create data, bool runTransaction = true);

        /// <summary>
        /// 获取编辑数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Edit GetEdit(string id);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        /// <returns></returns>
        void Edit(Edit data, bool runTransaction = true);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        /// <returns></returns>
        void Delete(List<string> ids);

        #endregion

        #region 拓展功能

        /// <summary>
        /// 启用/禁用
        /// </summary>
        /// <param name="id">数据</param>
        /// <param name="enable">设置状态</param>
        /// <returns></returns>
        void Enable(string id, bool enable);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="data">数据</param>
        void UpdatePassword(UpdatePassword data);

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="newPassword">新密码</param>
        void ResetPassword(string id, string newPassword);

        /// <summary>
        /// 获取声明信息
        /// </summary>
        /// <param name="authenticationInfo">验证信息</param>
        /// <param name="authenticationMethod">验证方法</param>
        /// <returns>声明信息</returns>
        List<Claim> CreateClaims(AuthenticationInfo authenticationInfo, string authenticationMethod);

        /// <summary>
        /// 解析声明信息
        /// </summary>
        /// <param name="claims">声明信息</param>
        /// <returns>验证信息</returns>
        AuthenticationInfo AnalysisClaims(List<Claim> claims);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        AuthenticationInfo Login(string account, string password);

        /// <summary>
        /// 登录（失败多次将会锁定账号）
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        AuthenticationInfo LoginWithLimitTimes(string account, string password);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="openId"></param>
        AuthenticationInfo WeChatLogin(string appId, string openId);

        /// <summary>
        /// 获取操作者详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        OperatorUserInfo GetOperatorDetail(string id);

        #endregion
    }
}
