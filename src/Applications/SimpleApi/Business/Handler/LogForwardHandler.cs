using Business.Hub;
using Business.Utils.Log;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.SignalR;
using Model.Utils.Log;
using Model.Utils.SignalR;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Handler
{
    /// <summary>
    /// SignalR日志转发
    /// </summary>
    public class LogForwardHandler
    {
        #region 私有成员

        /// <summary>
        /// 数据队列
        /// </summary>
        /// <remarks>
        /// </remarks>
        readonly ConcurrentQueue<LogInfo> LogQueue = new ConcurrentQueue<LogInfo>();

        IHubContext<LogHub> LogHub
        {
            get
            {
                if (_LogHub == null)
                    _LogHub = AutofacHelper.GetService<IHubContext<LogHub>>();
                return _LogHub;
            }
        }

        IHubContext<LogHub> _LogHub;

        TaskCompletionSource<bool> TCS;

        #endregion

        #region 公共部分

        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "日志转发模块";

        /// <summary>
        /// 启动时间
        /// </summary>
        public DateTime? StartTime;

        /// <summary>
        /// 数据总量
        /// </summary>
        public int DataCount => LogQueue.Count;

        /// <summary>
        /// 当前状态
        /// </summary>
        /// <returns></returns>
        public bool? State()
        {
            return TCS?.Task.Status == TaskStatus.RanToCompletion ? TCS?.Task.Result : null;
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            TCS = new TaskCompletionSource<bool>();
            StartTime = DateTime.Now;
            Run();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Shutdown()
        {
            if (TCS == null)
                TCS = new TaskCompletionSource<bool>();
            StartTime = null;
            TCS?.SetResult(false);
        }

        /// <summary>
        /// 新增转发数据
        /// </summary>
        /// <param name="log">日志</param>
        public void Add(LogInfo log)
        {
            LogQueue.Enqueue(log);

            //开始推送
            TCS?.SetResult(true);
        }

        /// <summary>
        /// 运行
        /// </summary>
        private async void Run()
        {
            while (true)
            {
                if (TCS != null)
                {
                    if (!await TCS.Task)
                        return;
                    TCS = null;
                }

                if (LogQueue.IsEmpty)
                {
                    TCS = new TaskCompletionSource<bool>();
                    continue;
                }

                try
                {
                    await Processing();
                }
                catch (Exception ex)
                {
                    Logger.Log(
                        NLog.LogLevel.Error,
                        LogType.系统异常,
                        "处理SignalR日志转发时异常.",
                        null,
                        ex);
                }
            }
        }

        /// <summary>
        /// 处理转发
        /// </summary>
        private async Task Processing()
        {
            var Queue = LogQueue;
            var Count = Queue.Count;

            for (int i = 0; i < Count; i++)
            {
                Queue.TryDequeue(out LogInfo log);

                try
                {
                    foreach (var setting in Hub.LogHub.Settings)
                    {
                        if (!setting.Value.Push
                            || (log.Level.IsNullOrWhiteSpace() ? setting.Value.Level.Contains("None") : !setting.Value.Level.Contains(log.Level))
                            || (log.LogType.IsNullOrWhiteSpace() ? setting.Value.Type.Contains("None") : !setting.Value.Type.Contains(log.LogType)))
                            continue;

                        if (setting.Value.Filter.Any(o => log.Data?.Contains(o) == true))
                            continue;

                        if (setting.Value.Keyword.Any(o => log.Data?.Contains(o) != true))
                            continue;

                        await LogHub.Clients.Client(setting.Key).SendCoreAsync(
                            LogHubMethod.Log,
                            new object[]
                            {
                                log
                            });
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(
                        NLog.LogLevel.Error,
                        LogType.系统异常,
                        $"处理SignalR日志转发时异常.",
                        null,
                        ex);
                }
            }
        }

        #endregion
    }
}
