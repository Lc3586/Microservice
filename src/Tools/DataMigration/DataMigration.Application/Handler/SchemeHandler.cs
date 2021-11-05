﻿using DataMigration.Application.Model;
using Microservice.Library.FreeSql.Gen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigration.Application.Handler
{
    /// <summary>
    /// 模式处理器
    /// </summary>
    public class SchemeHandler : IHandler
    {
        public SchemeHandler(Config config, IFreeSqlMultipleProvider<int> freeSqlMultipleProvider)
        {
            Config = config;
            FreeSqlMultipleProvider = freeSqlMultipleProvider;
        }

        /// <summary>
        /// 配置
        /// </summary>
        readonly Config Config;

        /// <summary>
        /// 
        /// </summary>
        readonly IFreeSqlMultipleProvider<int> FreeSqlMultipleProvider;

        /// <summary>
        /// 处理
        /// </summary>
        public void Handler()
        {
            try
            {
                FreeSqlMultipleProvider.Test();

                //FreeSqlMultipleProvider.SyncStructure(1);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("目标数据库同步结构失败.", ex);
            }
        }
    }
}
