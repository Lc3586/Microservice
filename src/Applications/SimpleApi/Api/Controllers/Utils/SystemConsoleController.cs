using Api.Controllers.Utils;
using Business.Handler;
using Business.Utils.Authorization;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.Mvc;
using Model.Utils.Result;
using Model.Utils.SystemConsole;
using Model.Utils.SystemConsole.InfoDTO;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// 系统中控台
    /// </summary>
    [Route("/system-console")]
    [SampleAuthorize(nameof(ApiAuthorizeRequirement))]
    [SwaggerTag("系统中控台")]
    public class SystemConsoleController : BaseApiController
    {
        #region DI

        public SystemConsoleController()
        {
            LogForward = AutofacHelper.GetService<LogForwardHandler>();
            ChunkFileMerge = AutofacHelper.GetService<ChunkFileMergeHandler>();
        }

        /// <summary>
        /// 日志转发
        /// </summary>
        readonly LogForwardHandler LogForward;

        /// <summary>
        /// 合并分片文件
        /// </summary>
        readonly ChunkFileMergeHandler ChunkFileMerge;

        #endregion

        #region 基础接口

        /// <summary>
        /// 关停
        /// </summary>
        /// <param name="name">名称，为null时全部关停</param>
        /// <returns></returns>
        [HttpPost("shutdown")]
        public async Task<object> Shutdown(string name = null)
        {
            if (name == null || name == LogForwardHandler.Name)
            {
                if (LogForward?.State() != false)
                    LogForward?.Shutdown();
            }

            if (name == null || name == ChunkFileMergeHandler.Name)
            {
                if (ChunkFileMerge?.State() != false)
                    ChunkFileMerge?.Shutdown();
            }

            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 重启
        /// </summary>
        /// <param name="name">名称，为null时全部重启</param>
        /// <returns></returns>
        [HttpPost("reboot")]
        public async Task<object> Reboot(string name = null)
        {
            if (name == null || name == LogForwardHandler.Name)
            {
                if (LogForward?.State() != false)
                    LogForward?.Shutdown();
                if (LogForward?.State() != true)
                    LogForward?.Start();
            }

            if (name == null || name == ChunkFileMergeHandler.Name)
            {
                if (ChunkFileMerge?.State() != false)
                    ChunkFileMerge?.Shutdown();
                if (ChunkFileMerge?.State() != true)
                    ChunkFileMerge?.Start();
            }

            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="name">名称，为null时获取全部</param>
        /// <returns></returns>
        [HttpPost("state")]
        [SwaggerResponse((int)HttpStatusCode.OK, "模块信息", typeof(ModularInfo))]
        public async Task<object> GetState(string name = null)
        {
            var result = new List<ModularInfo>();

            if (name == null || name == LogForwardHandler.Name)
            {
                var modular = new ModularInfo
                {
                    Name = LogForwardHandler.Name,
                    Data = new
                    {
                        启动时间 = LogForward.StartTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                        数据总量 = LogForward.DataCount
                    }
                };

                if (LogForward == null)
                    modular.State = State.未启用;
                else
                {
                    var lfstate = LogForward?.State();
                    modular.State = lfstate == null ? State.空闲 : lfstate == true ? State.运行中 : State.已停止;
                }

                result.Add(modular);
            }

            if (name == null || name == ChunkFileMergeHandler.Name)
            {
                var modular = new ModularInfo
                {
                    Name = ChunkFileMergeHandler.Name,
                    Data = new
                    {
                        启动时间 = ChunkFileMerge.StartTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                        数据总量 = ChunkFileMerge.DataCount
                    }
                };

                if (ChunkFileMerge == null)
                    modular.State = State.未启用;
                else
                {
                    var lfstate = ChunkFileMerge?.State();
                    modular.State = lfstate == null ? State.空闲 : lfstate == true ? State.运行中 : State.已停止;
                }

                result.Add(modular);
            }

            if (result.Count == 1)
                return await Task.FromResult(Success(result.First()));
            else
                return await Task.FromResult(Success(result.OrderBy(o => o.Name)));
        }

        #endregion     
    }
}
