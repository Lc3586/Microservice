using DataMigration.Application.Log;
using FreeSql;
using FreeSql.Internal;
using NLog;
using System.Collections.Generic;

namespace DataMigration.Application.Model
{
    /// <summary>
    /// 配置
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 源数据库连接字符串
        /// </summary>
        public string SourceConnectingString { get; set; }

        /// <summary>
        /// 源数据库类型
        /// </summary>
        public DataType SourceDataType { get; set; }

        /// <summary>
        /// 目标数据库连接字符串
        /// </summary>
        public string TargetConnectingString { get; set; }

        /// <summary>
        /// 目标数据库类型
        /// </summary>
        public DataType TargetDataType { get; set; }

        /// <summary>
        /// 日志组件类型
        /// </summary>
        /// <remarks>默认 <see cref="LoggerType.File"/></remarks>
        public LoggerType LoggerType { get; set; } = LoggerType.File;

        /// <summary>
        /// 需要记录的日志的最低等级
        /// </summary>
        /// <remarks>默认 <see cref="LogLevel.Trace.Ordinal"/></remarks>
        public int MinLogLevel { get; set; } = LogLevel.Trace.Ordinal;

        /// <summary>
        /// 操作类型
        /// </summary>
        /// <remarks>默认 <see cref="DataMigration.Application.Model.OperationType.All"/> 全部</remarks>
        public OperationType OperationType { get; set; } = OperationType.All;

        /// <summary>
        /// 实体类名 -> 数据库表名&列名，命名转换规则（类名、属性名都生效）
        /// </summary>
        public NameConvertType? SyncStructureNameConvert { get; set; }

        /// <summary>
        /// 实体类命名空间
        /// </summary>
        public List<string> EntityAssemblys { get; set; }
    }
}
