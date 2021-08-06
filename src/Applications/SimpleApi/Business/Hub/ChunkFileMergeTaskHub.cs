using Business.Utils.Authorization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalrHub = Microsoft.AspNetCore.SignalR.Hub;

namespace Business.Hub
{
    /// <summary>
    /// 分片文件合并任务中心
    /// </summary>
    [SampleAuthorize(nameof(ApiAuthorizeRequirement))]
    public class ChunkFileMergeTaskHub : SignalrHub
    {
        public ChunkFileMergeTaskHub()
        {

        }

        #region 远程方法



        #endregion
    }
}
