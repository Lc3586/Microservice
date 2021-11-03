using DataMigration.Application.Log;
using FreeSql;
using FreeSql.Internal;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

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
        /// 当前操作系统平台
        /// </summary>
        public string CurrentOS
        {
            get
            {
                if (_CurrentOS != null)
                    return _CurrentOS;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    _CurrentOS = OSPlatform.Windows.ToString();
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    _CurrentOS = OSPlatform.Linux.ToString();
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    _CurrentOS = OSPlatform.OSX.ToString();
                else
                    throw new ApplicationException("无法获取当前的操作系统平台.");

                return _CurrentOS;
            }
        }

        /// <summary>
        /// 当前操作系统平台
        /// </summary>
        private string _CurrentOS { get; set; }

        /// <summary>
        /// 文件清单
        /// <para>[{文件名 : [{平台 : 文件路径}]}]</para>
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> FileList { get; set; }

        private Dictionary<string, Dictionary<string, string>> FileAbsolutePathList { get; set; } = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// 获取文件清单中文件的绝对路径
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        public string GetFileAbsolutePath(string filename)
        {
            string result;
            if (FileAbsolutePathList.ContainsKey(filename) && FileAbsolutePathList[filename].ContainsKey(CurrentOS))
                result = FileAbsolutePathList[filename][CurrentOS];
            else
            {
                if (!FileList.ContainsKey(filename))
                    throw new ApplicationException("未在config.json中配置{filename}文件");

                if (!FileList[filename].ContainsKey(CurrentOS))
                    throw new ApplicationException($"未找到符合当前系统【{CurrentOS}】版本的{filename}文件");

                result = Path.GetFullPath(FileList[filename][CurrentOS], AppContext.BaseDirectory);

                if (!FileAbsolutePathList.ContainsKey(filename))
                    FileAbsolutePathList.Add(filename, new Dictionary<string, string>());

                FileAbsolutePathList[filename].Add(CurrentOS, result);
            }

            if (!File.Exists(result))
                throw new ApplicationException($"指定目录下未找到{filename}文件: {result}.");

            return result;
        }

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

        /// <summary>
        /// 生成实体类
        /// </summary>
        public bool GenerateEntitys { get; set; } = false;
    }
}
