using Business.Utils.AuthorizePolicy;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.Authorization;
using Model.Utils.SignalR;
using System.Collections.Concurrent;
using SignalrHub = Microsoft.AspNetCore.SignalR.Hub;

namespace Business.Hub
{
    /// <summary>
    /// 日志中心
    /// </summary>
    [Authorize(nameof(ApiAuthorizeRequirement))]
    public class LogHub : SignalrHub
    {
        public LogHub()
        {

        }

        /// <summary>
        /// 客户端设置
        /// </summary>
        public static readonly ConcurrentDictionary<string, LogSetting> Settings
            = new ConcurrentDictionary<string, LogSetting>();


        #region 远程方法

        /// <summary>
        /// 开始接收
        /// </summary>
        /// <returns></returns>
        public void Start()
        {
            Settings.AddOrUpdate(
                Context.ConnectionId,
                new LogSetting { Push = true },
                (key, old) =>
                {
                    old.Push = true;
                    return old;
                });
        }

        /// <summary>
        /// 暂停接收
        /// </summary>
        /// <returns></returns>
        public void Pause()
        {
            Settings.AddOrUpdate(
                Context.ConnectionId,
                new LogSetting { Push = false },
                (key, old) =>
                {
                    old.Push = false;
                    return old;
                });
        }

        /// <summary>
        /// 设置级别
        /// </summary>
        /// <param name="levels"></param>
        /// <returns></returns>
        public void AddLevels(string[] levels)
        {
            if (!levels.Any_Ex())
                return;

            if (!Settings.ContainsKey(Context.ConnectionId))
                return;

            Settings.AddOrUpdate(
               Context.ConnectionId,
               new LogSetting(),
               (key, old) =>
               {
                   foreach (var level in levels)
                   {
                       if (!level.IsNullOrWhiteSpace()
                            && !old.Level.Contains(level))
                           old.Level.Add(level);
                   }
                   return old;
               });
        }

        /// <summary>
        /// 移除级别
        /// </summary>
        /// <remarks>参数为空时清除全部</remarks>
        /// <param name="levels"></param>
        /// <returns></returns>
        public void RemoveLevels(string[] levels)
        {
            if (!Settings.ContainsKey(Context.ConnectionId))
                return;

            Settings.AddOrUpdate(
               Context.ConnectionId,
               new LogSetting(),
               (key, old) =>
               {
                   if (!levels.Any_Ex())
                       old.Level.Clear();
                   else
                   {
                       foreach (var level in levels)
                       {
                           if (!level.IsNullOrWhiteSpace()
                                && old.Level.Contains(level))
                               old.Level.Remove(level);
                       }
                   }
                   return old;
               });
        }

        /// <summary>
        /// 设置类型
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public void AddTypes(string[] types)
        {
            if (!types.Any_Ex())
                return;

            if (!Settings.ContainsKey(Context.ConnectionId))
                return;

            Settings.AddOrUpdate(
               Context.ConnectionId,
               new LogSetting(),
               (key, old) =>
               {
                   foreach (var type in types)
                   {
                       if (!type.IsNullOrWhiteSpace()
                            && !old.Type.Contains(type))
                           old.Type.Add(type);
                   }
                   return old;
               });
        }

        /// <summary>
        /// 移除类型
        /// </summary>
        /// <remarks>参数为空时清除全部</remarks>
        /// <param name="types"></param>
        /// <returns></returns>
        public void RemoveTypes(string[] types)
        {
            if (!Settings.ContainsKey(Context.ConnectionId))
                return;

            Settings.AddOrUpdate(
               Context.ConnectionId,
               new LogSetting(),
               (key, old) =>
               {
                   if (!types.Any_Ex())
                       old.Type.Clear();
                   else
                   {
                       foreach (var type in types)
                       {
                           if (!type.IsNullOrWhiteSpace()
                                && old.Type.Contains(type))
                               old.Type.Remove(type);
                       }
                   }
                   return old;
               });
        }

        /// <summary>
        /// 设置过滤
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public void AddFilters(string[] filters)
        {
            if (!filters.Any_Ex())
                return;

            if (!Settings.ContainsKey(Context.ConnectionId))
                return;

            Settings.AddOrUpdate(
               Context.ConnectionId,
               new LogSetting(),
               (key, old) =>
               {
                   foreach (var filter in filters)
                   {
                       if (!filter.IsNullOrWhiteSpace()
                            && !old.Filter.Contains(filter))
                           old.Filter.Add(filter);
                   }
                   return old;
               });
        }

        /// <summary>
        /// 移除过滤
        /// </summary>
        /// <remarks>参数为空时清除全部</remarks>
        /// <param name="filters"></param>
        /// <returns></returns>
        public void RemoveFilters(string[] filters)
        {
            if (!Settings.ContainsKey(Context.ConnectionId))
                return;

            Settings.AddOrUpdate(
               Context.ConnectionId,
               new LogSetting(),
               (key, old) =>
               {
                   if (!filters.Any_Ex())
                       old.Filter.Clear();
                   else
                   {
                       foreach (var filter in filters)
                       {
                           if (!filter.IsNullOrWhiteSpace()
                                && old.Filter.Contains(filter))
                               old.Filter.Remove(filter);
                       }
                   }
                   return old;
               });
        }

        /// <summary>
        /// 设置关键词
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public void AddKeywords(string[] keywords)
        {
            if (!keywords.Any_Ex())
                return;

            if (!Settings.ContainsKey(Context.ConnectionId))
                return;

            Settings.AddOrUpdate(
               Context.ConnectionId,
               new LogSetting(),
               (key, old) =>
               {
                   foreach (var keyword in keywords)
                   {
                       if (!keyword.IsNullOrWhiteSpace()
                            && !old.Keyword.Contains(keyword))
                           old.Keyword.Add(keyword);
                   }
                   return old;
               });
        }

        /// <summary>
        /// 移除关键词
        /// </summary>
        /// <remarks>参数为空时清除全部</remarks>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public void RemoveKeywords(string[] keywords)
        {
            if (!Settings.ContainsKey(Context.ConnectionId))
                return;

            Settings.AddOrUpdate(
               Context.ConnectionId,
               new LogSetting(),
               ((key, old) =>
               {
                   if (!keywords.Any_Ex())
                       old.Keyword.Clear();
                   else
                   {
                       foreach (var keyword in keywords)
                       {
                           if (!keyword.IsNullOrWhiteSpace()
                                && old.Keyword.Contains(keyword))
                               old.Keyword.Remove(keyword);
                       }
                   }
                   return old;
               }));
        }

        #endregion
    }
}
