using Api.Controllers.Utils;
using Business.Handler;
using Business.Utils.Authorization;
using Microservice.Library.Container;
using Microsoft.AspNetCore.Mvc;
using Model.Utils.Result;
using Model.Utils.SystemConsole;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
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
            if (name == null || name == "日志转发")
            {
                if (LogForward?.State() != false)
                    LogForward?.Shutdown();
            }

            if (name == null || name == "合并分片文件")
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
            if (name == null || name == "日志转发")
            {
                if (LogForward?.State() != false)
                    LogForward?.Shutdown();
                if (LogForward?.State() != true)
                    LogForward?.Start();
            }

            if (name == null || name == "合并分片文件")
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
        [SwaggerResponse((int)HttpStatusCode.OK, "状态", typeof(Dictionary<string, string>))]
        public async Task<object> GetState(string name = null)
        {
            var result = new Dictionary<string, string>();

            if (name == null || name == "日志转发")
            {
                if (LogForward == null)
                    result.Add("日志转发", State.未启用);
                else
                {
                    var lfstate = LogForward?.State();
                    result.Add("日志转发", lfstate == null ? State.空闲 : lfstate == true ? State.运行中 : State.已停止);
                }
            }

            if (name == null || name == "合并分片文件")
            {
                if (ChunkFileMerge == null)
                    result.Add("合并分片文件", State.未启用);
                else
                {
                    var lfstate = ChunkFileMerge?.State();
                    result.Add("合并分片文件", lfstate == null ? State.空闲 : lfstate == true ? State.运行中 : State.已停止);
                }
            }

            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(result)));
        }

        #endregion     
    }
}
