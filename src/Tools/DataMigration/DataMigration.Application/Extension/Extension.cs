using FreeSql.DatabaseModel;
using Microservice.Library.FreeSql.Gen;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DataMigration.Application.Extension
{
    /// <summary>
    /// 拓展方法
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// 数据库表集合
        /// </summary>
        static readonly Dictionary<int, List<DbTableInfo>> Tables = new Dictionary<int, List<DbTableInfo>>();

        /// <summary>
        /// 数据库实体集合
        /// </summary>
        static List<Type> EntityTypes;

        /// <summary>
        /// 获取Orm
        /// </summary>
        /// <param name="freeSqlMultipleProvider">构造器</param>
        /// <param name="key">数据库标识</param>
        /// <returns></returns>
        public static IFreeSql GetOrm(this IFreeSqlMultipleProvider<int> freeSqlMultipleProvider, int key)
        {
            return freeSqlMultipleProvider.GetFreeSql(key);
        }

        /// <summary>
        /// 获取数据库表集合
        /// </summary>
        /// <param name="freeSqlMultipleProvider">构造器</param>
        /// <param name="key">数据库标识</param>
        /// <returns></returns>
        public static List<DbTableInfo> GetTablesByDatabase(this IFreeSqlMultipleProvider<int> freeSqlMultipleProvider, int key)
        {
            if (Tables.ContainsKey(key))
                return Tables[key];

            var orm = freeSqlMultipleProvider.GetOrm(key);
            var tables = orm.DbFirst.GetTablesByDatabase();
            Tables.Add(key, tables);
            return tables;
        }

        /// <summary>
        /// 设置数据库实体集合
        /// </summary>
        public static void SetEntityTypes(List<Type> types)
        {
            EntityTypes = types;
        }

        /// <summary>
        /// 获取数据库实体集合
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetEntityTypes()
        {
            return EntityTypes;
        }

        /// <summary>
        /// 获取正则表达式在字符串中匹配到的值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="pattern">正则表达式</param>
        /// <returns>值集合, 未匹配成功时返回null</returns>
        public static List<string> Match(this string input, string pattern)
        {
            var match = Regex.Match(input, pattern);
            if (!match.Success)
                return null;

            var values = new List<string>();

            for (int i = 1; i < match.Groups.Count; i++)
            {
                values.Add(match.Groups[i].Value);
            }

            return values;
        }
    }
}
