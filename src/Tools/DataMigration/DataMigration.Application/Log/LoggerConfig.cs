﻿namespace DataMigration.Application.Log
{
    /// <summary>
    /// 日志配置
    /// </summary>
    public class LoggerConfig
    {
        public static readonly string LogName = "Default";
        public static readonly string LogType = "LogType";
        public static readonly string Data = "Data";
        public static readonly string Layout = $@"${{date:format=yyyy-MM-dd HH\:mm\:ss}}"
                                                + $"\r\n|${{level}}"
                                                + $"\r\n|日志类型:${{event-properties:item={LogType}}}"
                                                + $"\r\n|内容:${{message}}"
                                                + $"\r\n|备份数据:${{event-properties:item={Data}}}";
        public static readonly string FileDic = "log";
        public static readonly string FileName = "${date:format=yyyy-MM-dd}.txt";
        public static readonly string FileNameFormat = "{0:yyyy-MM-dd}.txt";
    }
}
