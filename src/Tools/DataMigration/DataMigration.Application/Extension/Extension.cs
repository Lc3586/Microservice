using FreeSql.DatabaseModel;
using Microservice.Library.FreeSql.Gen;
using System;
using System.Collections.Generic;

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
        static List<Type> Entitys;

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
        public static void SetEntitys(List<Type> types)
        {
            Entitys = types;
        }

        /// <summary>
        /// 获取数据库实体集合
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetEntitys()
        {
            return Entitys;
        }
    }
}
