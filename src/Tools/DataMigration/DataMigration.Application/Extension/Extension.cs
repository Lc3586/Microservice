using FreeSql.DatabaseModel;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Gen;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataMigration.Application.Extension
{
    /// <summary>
    /// 拓展方法
    /// </summary>
    public static class Extension
    {
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
        /// <param name="tableMatchs">表名正则表达式</param>
        /// <param name="tables">指定数据库表名</param>
        /// <param name="exclusionTables">排除数据库表名</param>
        /// <returns></returns>
        public static List<DbTableInfo> GetTablesByDatabase(this IFreeSqlMultipleProvider<int> freeSqlMultipleProvider, int key, List<string> tableMatchs = null, List<string> tables = null, List<string> exclusionTables = null)
        {
            var orm = freeSqlMultipleProvider.GetOrm(key);
            var dbTables = new List<DbTableInfo>();

            if (tableMatchs.Any_Ex())
                dbTables = orm.DbFirst.GetTablesByDatabase();
            else if (tables.Any_Ex())
                tables.ForEach(o => dbTables.Add(orm.DbFirst.GetTableByName(o, true)));
            else
                dbTables = orm.DbFirst.GetTablesByDatabase();

            if (tableMatchs.Any_Ex() || tables.Any_Ex() || exclusionTables.Any_Ex())
                dbTables.RemoveAll(o =>
                (!tableMatchs.Any_Ex(p => Regex.IsMatch(o.Name, p, RegexOptions.IgnoreCase) == true)
                    && (!tables.Any_Ex() || !tables.Any(p => string.Equals(o.Name, p, StringComparison.OrdinalIgnoreCase))))
                || (exclusionTables.Any_Ex() && exclusionTables.Any(p => string.Equals(o.Name, p, StringComparison.OrdinalIgnoreCase))));

            return dbTables;
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
        /// 获取数据库表集合
        /// </summary>
        /// <param name="freeSqlMultipleProvider">构造器</param>
        /// <param name="key">数据库标识</param>
        /// <param name="tableMatchs">表名正则表达式</param>
        /// <param name="tables">指定数据库表名</param>
        /// <param name="exclusionTables">排除数据库表名</param>
        /// <returns></returns>
        public static List<Type> GetEntityTypes(this IFreeSqlMultipleProvider<int> freeSqlMultipleProvider, int key, List<string> tableMatchs = null, List<string> tables = null, List<string> exclusionTables = null)
        {
            if (tableMatchs.Any_Ex() || tables.Any_Ex() || exclusionTables.Any_Ex())
            {
                var orm = freeSqlMultipleProvider.GetOrm(key);
                var result = new List<Type>();

                EntityTypes.ForEach(o =>
                {
                    var dbTable = orm.CodeFirst.GetTableByEntity(o);

                    if (tableMatchs.Any_Ex() || tables.Any_Ex() || exclusionTables.Any_Ex())
                        if ((!tableMatchs.Any_Ex(p => Regex.IsMatch(dbTable.DbName, p, RegexOptions.IgnoreCase) == true)
                            && (!tables.Any_Ex() || !tables.Any(p => string.Equals(dbTable.DbName, p, StringComparison.OrdinalIgnoreCase))))
                        || (exclusionTables.Any_Ex() && exclusionTables.Any(p => string.Equals(dbTable.DbName, p, StringComparison.OrdinalIgnoreCase))))
                        return;

                    result.Add(o);
                });

                return result;
            }
            else
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

        /// <summary>
        /// 新增或更新
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (!dic.ContainsKey(key))
                dic.Add(key, value);
            else
                dic[key] = value;
        }

        /// <summary>
        /// 新增或追加
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrAppend<TKey, TValue>(this Dictionary<TKey, List<TValue>> dic, TKey key, TValue value)
        {
            if (!dic.ContainsKey(key))
                dic.Add(key, new List<TValue> { value });
            else
                dic[key].Add(value);
        }
    }
}
