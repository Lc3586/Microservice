using DataMigration.Application.Model;
using Microservice.Library.FreeSql.Gen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigration.Application.Handler
{
    /// <summary>
    /// 数据处理器
    /// </summary>
    public class DataHandler : IHandler
    {
        public DataHandler(Config config, IFreeSqlMultipleProvider<int> freeSqlMultipleProvider)
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
            FreeSqlMultipleProvider.Test();

            var orms = FreeSqlMultipleProvider.GetAllFreeSqlWithKey();

            FreeSqlMultipleProvider.Dispose();
        }
    }
}
