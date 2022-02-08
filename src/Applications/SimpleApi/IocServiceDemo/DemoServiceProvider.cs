using System;
using System.Collections.Generic;
using System.Text;

namespace IocServiceDemo
{
    /// <summary>
    /// 服务提供者实现类
    /// </summary>
    public class DemoServiceProvider : IDemoServiceProvider
    {
        public DemoServiceProvider(DemoServiceOptions options)
        {
            DemoServiceManagement.SetOption(options);
        }

        public IDemoService GetService()
        {
            return Activator.CreateInstance(GetDefaultType().Use()) as IDemoService;
        }

        public IDemoService GetService<T>() where T : class
        {
            return Activator.CreateInstance(typeof(T).Use()) as IDemoService;
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
#pragma warning disable IDE0060 // 删除未使用的参数
        public static object GetService(IServiceProvider serviceProvider)
#pragma warning restore IDE0060 // 删除未使用的参数
        {
            return Activator.CreateInstance(GetDefaultType().Use());
        }

        /// <summary>
        /// 获取默认服务类型
        /// </summary>
        /// <returns></returns>
        private static Type GetDefaultType()
        {
            return DemoServiceManagement.GetAvailableType();
        }
    }
}
