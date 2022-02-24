using Business.Interface.System;
using Business.Utils.Authorization;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Model.Utils.SignalR;
using System.Collections.Concurrent;
using SignalrHub = Microsoft.AspNetCore.SignalR.Hub;

namespace Business.Hub
{
    /// <summary>
    /// 代码自动生成信息推送
    /// </summary>
    [SampleAuthorize(nameof(ApiPermissionRequirement))]
    public class CAGCHub : SignalrHub
    {
        public CAGCHub()
        {

        }

        /// <summary>
        /// 用户集合
        /// </summary>
        /// <remarks>{UserId, ConnectionId}</remarks>
        public static readonly ConcurrentDictionary<string, string> Users = new ConcurrentDictionary<string, string>();


        #region 远程方法

        /// <summary>
        /// 开始接收
        /// </summary>
        /// <returns></returns>
        public void Open()
        {
            var userId = AutofacHelper.GetScopeService<IOperator>().AuthenticationInfo.Id;
            Users.AddOrUpdate(
                userId,
                Context.ConnectionId,
                (key, old) =>
                {
                    return Context.ConnectionId;
                });
        }

        /// <summary>
        /// 停止接收
        /// </summary>
        /// <returns></returns>
        public void Close()
        {
            var userId = AutofacHelper.GetScopeService<IOperator>().AuthenticationInfo.Id;
            Users.TryRemove(userId, out string _);
        }

        #endregion
    }
}
