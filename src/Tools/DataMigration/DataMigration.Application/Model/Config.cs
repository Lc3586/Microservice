using DataMigration.Application.Log;
using FreeSql;
using FreeSql.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public OSPlatform CurrentOS
        {
            get
            {
                if (_CurrentOS != null)
                    return _CurrentOS.Value;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    _CurrentOS = OSPlatform.Windows;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    _CurrentOS = OSPlatform.Linux;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    _CurrentOS = OSPlatform.OSX;
                else
                    throw new ApplicationException("无法获取当前的操作系统平台.");

                return _CurrentOS.Value;
            }
        }

        /// <summary>
        /// 当前操作系统平台
        /// </summary>
        private OSPlatform? _CurrentOS { get; set; }

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
        public LoggerType LoggerType { get; set; }

        /// <summary>
        /// 日志等级
        /// </summary>
        public int MinLogLevel { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public OperationType OperationType { get; set; }

        /// <summary>
        /// 数据检查
        /// </summary>
        public bool DataCheck { get; set; }

        /// <summary>
        /// 数据分页大小
        /// </summary>
        public int DataPageSize { get; set; }

        /// <summary>
        /// 使用批量插入功能（如果数据库支持的话）
        /// </summary>
        public bool UseBulkCopy { get; set; }

        /// <summary>
        /// 实体类名 -> 数据库表名&列名，命名转换规则（类名、属性名都生效）
        /// </summary>
        public NameConvertType? SyncStructureNameConvert { get; set; }

        /// <summary>
        /// 实体类dll文件
        /// </summary>
        public List<string> EntityAssemblyFiles
        {
            get
            {
                return _EntityAssemblyFiles;
            }
            set
            {
                _EntityAssemblyFiles = value;
                EntityAssemblys = value.Select(o => o.Replace(".dll", "")).ToList();
            }
        }

        List<string> _EntityAssemblyFiles { get; set; }

        /// <summary>
        /// 实体类命名空间
        /// </summary>
        public List<string> EntityAssemblys { get; set; }

        /// <summary>
        /// 生成实体类
        /// </summary>
        public bool GenerateEntitys { get; set; } = false;

        /// <summary>
        /// 实体类Razor模板文件
        /// </summary>
        public string EntityRazorTemplateFile { get; set; }
    }
}
