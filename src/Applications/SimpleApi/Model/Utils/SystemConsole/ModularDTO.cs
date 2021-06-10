using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Utils.SystemConsole.InfoDTO
{
    /// <summary>
    /// 模块信息
    /// </summary>
    public class ModularInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public object Data { get; set; }
    }
}
