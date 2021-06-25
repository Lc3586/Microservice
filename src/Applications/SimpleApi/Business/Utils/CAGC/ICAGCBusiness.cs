﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Utils.CAGC
{
    /// <summary>
    /// 自动生成代码业务接口类
    /// </summary>
    public interface ICAGCBusiness
    {
        /// <summary>
        /// 获取所有生成类型
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetGenTypes();

        /// <summary>
        /// 使用CSV文件生成代码
        /// </summary>
        /// <param name="file">CSV文件</param>
        /// <param name="genType">生成类型</param>
        /// <returns></returns>
        Task<string> GenerateByCSV(IFormFile file, string genType);

        /// <summary>
        /// 下载已生成的代码文件
        /// </summary>
        /// <param name="key">用于下载文件的key</param>
        /// <returns></returns>
        Task Download(string key);
    }
}
