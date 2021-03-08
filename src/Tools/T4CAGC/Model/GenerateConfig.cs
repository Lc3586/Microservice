using FreeSql;
using FreeSql.DatabaseModel;
using NLog;
using System.Collections.Generic;
using T4CAGC.Log;

namespace T4CAGC.Model
{
    /// <summary>
    /// 生成配置
    /// </summary>
    public class GenerateConfig
    {
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 数据源
        /// </summary>
        /// <remarks>
        /// <para>CSV文件路径</para>
        /// <para>数据库连接字符串</para>
        /// </remarks>
        public string DataSource { get; set; }

        /// <summary>
        /// 数据源类型
        /// </summary>
        /// <remarks>默认 <see cref="DataSourceType.CSV"/></remarks>
        public DataSourceType DataSourceType { get; set; } = DataSourceType.CSV;

        /// <summary>
        /// 数据库类型
        /// </summary>
        /// <remarks>默认 <see cref="DataType.SqlServer"/></remarks>
        public DataType DataBaseType { get; set; } = DataType.SqlServer;

        /// <summary>
        /// 数据表类型
        /// </summary>
        /// <remarks>默认 <see cref="DbTableType.TABLE"/> + <see cref="DbTableType.VIEW"/></remarks>
        public List<DbTableType> TableType { get; set; } = new List<DbTableType> { DbTableType.TABLE, DbTableType.VIEW };

        /// <summary>
        /// 日志组件类型
        /// </summary>
        /// <remarks>默认 <see cref="LoggerType.File"/></remarks>
        public LoggerType LoggerType { get; } = LoggerType.File;

        /// <summary>
        /// 需要记录的日志的最低等级
        /// </summary>
        /// <remarks>默认 <see cref="LogLevel.Trace.Ordinal"/></remarks>
        public int MinLogLevel { get; } = LogLevel.Trace.Ordinal;

        /// <summary>
        /// 输出路径
        /// </summary>
        public string OutputPath { get; set; }

        /// <summary>
        /// 生成类型
        /// </summary>
        /// <remarks>默认 <see cref="Config.GenType.All"/></remarks>
        public GenType GenType { get; } = GenType.All;

        /// <summary>
        /// 指定表
        /// </summary>
        /// <remarks>多张表请使用英文逗号进行分隔[,]. 默认指定所有的表.</remarks>
        public string SpecifyTable { get; }

        /// <summary>
        /// 忽略表
        /// </summary>
        /// <remarks>多张表请使用英文逗号进行分隔[,].</remarks>
        public string IgnoreTable { get; }

        /// <summary>
        /// 表设置
        /// </summary>
        /// <remarks>此配置将会作为默认规则的补充</remarks>
        public List<TableSetting> TableSettings { get; set; }
    }
}
