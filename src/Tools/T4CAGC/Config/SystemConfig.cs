using Microservice.Library.Configuration.Annotations;
using T4CAGC.Log;

namespace T4CAGC.Config
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class SystemConfig
    {
        /// <summary>
        /// 程序状态
        /// </summary>
        public ProgramState ProgramState { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 版本说明
        /// </summary>
        public string VersionDescription { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; } = "default";

        #region 日志

        /// <summary>
        /// 默认日志组件名称
        /// </summary>
        public string LoggerName { get; set; } = "SystemLog";

        /// <summary>
        /// 默认日志组件类型
        /// </summary>
        public LoggerType LoggerType { get; set; } = LoggerType.Console;

        /// <summary>
        /// 默认日志组件布局
        /// </summary>
        public string LoggerLayout { get; set; }

        /// <summary>
        /// 需要记录的日志的最低等级
        /// </summary>
        public int MinLogLevel { get; set; }

        #endregion

        /// <summary>
        /// 命令行配置
        /// </summary>
        [JsonConfig("jsonconfig/swagger.json", "Commands")]
        public CommandSetting Command { get; set; }
    }
}
