using FreeSql;
using FreeSql.DatabaseModel;
using NLog;
using System.Collections.Generic;
using System.Text;
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
        public LoggerType LoggerType { get; set; } = LoggerType.File;

        /// <summary>
        /// 需要记录的日志的最低等级
        /// </summary>
        /// <remarks>默认 <see cref="LogLevel.Trace.Ordinal"/></remarks>
        public int MinLogLevel { get; set; } = LogLevel.Trace.Ordinal;

        /// <summary>
        /// 输出路径
        /// </summary>
        /// <remarks>生成类型为<see cref="GenType.EnrichmentProject"/>充实项目时此路径应该为项目（解决方案）根路径.</remarks>
        public string OutputPath { get; set; }

        /// <summary>
        /// 编码名称
        /// </summary>
        public string EncodingName { get; set; } = Encoding.UTF8.EncodingName;

        /// <summary>
        /// 生成类型
        /// </summary>
        /// <remarks>默认 <see cref="T4CAGC.Model.GenType.EnrichmentProject"/> 充实项目</remarks>
        public GenType GenType { get; set; } = GenType.EnrichmentProject;

        /// <summary>
        /// 覆盖文件
        /// </summary>
        public bool OverlayFile { get; set; }

        /// <summary>
        /// 指定表
        /// </summary>
        public List<string> SpecifyTable { get; set; }

        /// <summary>
        /// 忽略表
        /// </summary>
        public List<string> IgnoreTable { get; set; }

        /// <summary>
        /// 表设置
        /// </summary>
        /// <remarks>此配置将会作为默认规则的补充</remarks>
        public List<TableSetting> TableSettings { get; set; }
    }
}
