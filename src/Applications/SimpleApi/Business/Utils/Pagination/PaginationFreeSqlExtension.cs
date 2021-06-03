using FreeSql;
using FreeSql.Internal.Model;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using Model.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Utils.Pagination
{
    /// <summary>
    /// 分页设置FreeSql拓展方法
    /// </summary>
    public static class PaginationFreeSqlExtension
    {
        static IFreeSql Orm
        {
            get
            {
                if (_Orm == null)
                    _Orm = AutofacHelper.GetService<IFreeSqlProvider>().GetFreeSql();
                return _Orm;
            }
        }

        static IFreeSql _Orm;

        /// <summary>
        /// 获取表字段信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csName">属性名称</param>
        /// <returns></returns>
        static ColumnInfo GetColumnInfo<T>(string csName)
        {
            if (_Tables == null)
                _Tables = new Dictionary<Type, Dictionary<string, ColumnInfo>>();

            var type = typeof(T);

            if (!_Tables.ContainsKey(type))
                _Tables.Add(type, Orm.GetTableInfo<T>().ColumnsByCs);

            if (!_Tables[type].ContainsKey(csName))
                throw new MessageException($"{csName} 字段不存在.");

            return _Tables[type][csName];
        }

        static Dictionary<Type, Dictionary<string, ColumnInfo>> _Tables;

        private static DynamicFilterInfo ToDynamicFilterInfo(this PaginationDynamicFilterInfo paginationDynamicFilterInfo)
        {
            return new DynamicFilterInfo
            {
                Logic = ToDynamicFilterLogic(paginationDynamicFilterInfo.Relation),
                Field = paginationDynamicFilterInfo.Field,
                Operator = ToDynamicFilterOperator(paginationDynamicFilterInfo.Compare),
                Value = paginationDynamicFilterInfo.Value,
                Filters = paginationDynamicFilterInfo.DynamicFilterInfo?.Select(o => ToDynamicFilterInfo(o)).ToList()
            };
        }

        private static DynamicFilterInfo ToDynamicFilterInfo(this List<PaginationDynamicFilterInfo> paginationDynamicFilterInfo)
        {
            DynamicFilterInfo dynamicFilterInfo;
            var dynamicFilterInfos = paginationDynamicFilterInfo.Select(o => ToDynamicFilterInfo(o));

            if (paginationDynamicFilterInfo.Count == 1)
                dynamicFilterInfo = dynamicFilterInfos.First();
            else
                dynamicFilterInfo = new DynamicFilterInfo
                {
                    Logic = DynamicFilterLogic.And,
                    Filters = dynamicFilterInfos.ToList()
                };

            return dynamicFilterInfo;
        }

        private static DynamicFilterLogic ToDynamicFilterLogic(this FilterGroupRelation filterGroupRelation)
        {
            return filterGroupRelation switch
            {
                FilterGroupRelation.or => DynamicFilterLogic.Or,
                _ => DynamicFilterLogic.And,
            };
        }

        private static DynamicFilterOperator ToDynamicFilterOperator(this FilterCompare filterCompare)
        {
            return filterCompare switch
            {
                FilterCompare.@in => DynamicFilterOperator.Contains,
                FilterCompare.inStart => DynamicFilterOperator.StartsWith,
                FilterCompare.inEnd => DynamicFilterOperator.EndsWith,
                FilterCompare.notIn => DynamicFilterOperator.NotContains,
                FilterCompare.notInStart => DynamicFilterOperator.NotStartsWith,
                FilterCompare.notInEnd => DynamicFilterOperator.NotEndsWith,
                FilterCompare.eq => DynamicFilterOperator.Eq,
                FilterCompare.notEq => DynamicFilterOperator.NotEqual,
                FilterCompare.le => DynamicFilterOperator.LessThanOrEqual,
                FilterCompare.lt => DynamicFilterOperator.LessThan,
                FilterCompare.ge => DynamicFilterOperator.GreaterThanOrEqual,
                FilterCompare.gt => DynamicFilterOperator.GreaterThan,
                FilterCompare.inSet => DynamicFilterOperator.Any,
                FilterCompare.notInSet => DynamicFilterOperator.NotAny,
                FilterCompare.range => DynamicFilterOperator.Range,
                FilterCompare.dateRange => DynamicFilterOperator.DateRange,
                _ => throw new MessageException($"不支持的过滤条件: {filterCompare}"),
            };
        }

        /// <summary>
        /// 过滤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagination">分页参数</param>
        /// <param name="alias">指定别名</param>
        /// <returns></returns>
        public static ISelect<T> Filter<T>(this ISelect<T> source, PaginationDTO pagination, string alias = "a")
        {
            if (pagination.DynamicFilterInfo != null && pagination.DynamicFilterInfo.Any())
                source.WhereDynamicFilter(pagination.DynamicFilterInfo.ToDynamicFilterInfo());

            string where = string.Empty;

            Check(pagination.Filter);

            if (!pagination.FilterToSql(ref where, alias))
                throw new MessageException("搜索条件不支持");

            if (!string.IsNullOrWhiteSpace(where))
                source.Where(where);

            return source;

            void Check(List<PaginationFilter> filters)
            {
                if (!filters.Any_Ex())
                    return;

                filters.ForEach(o =>
                {
                    o.Field = GetColumnInfo<T>(o.Field).Attribute.Name;
                    Check(o.DynamicFilterInfo);
                });
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagination">分页参数</param>
        /// <param name="alias">指定别名</param>
        /// <returns></returns>
        public static ISelect<T> OrderBy<T>(this ISelect<T> source, PaginationDTO pagination, string alias = "a")
        {
            Check();

            string orderby = string.Empty;
            if (!pagination.OrderByToSql(ref orderby, alias))
                throw new MessageException("排序条件不支持");

            if (!string.IsNullOrWhiteSpace(orderby))
                source.OrderBy(orderby);

            return source;

            void Check()
            {
                if (!string.IsNullOrWhiteSpace(pagination.SortField))
                    pagination.SortField = GetColumnInfo<T>(pagination.SortField).Attribute.Name;

                if (!pagination.AdvancedSort.Any_Ex())
                    return;

                pagination.AdvancedSort.ForEach(o => o.Field = GetColumnInfo<T>(o.Field).Attribute.Name);
            }
        }

        /// <summary>
        /// 过滤
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagination">分页参数</param>
        /// <param name="alias">指定别名</param>
        /// <returns></returns>
        public static ISelectGrouping<TKey, TReturn> Filter<TKey, TReturn>(this ISelectGrouping<TKey, TReturn> source, PaginationDTO pagination, string alias = "a")
        {
            //if (DynamicFilterInfo != null && DynamicFilterInfo.Any())
            //    throw new MessageException("分组查询时不支持DynamicFilterInfo条件.");

            //string where = string.Empty;
            //if (!FilterToSql(ref where, "a"))
            //    throw new MessageException("搜索条件不支持");

            //if (!string.IsNullOrWhiteSpace(where))
            //    throw new MessageException("分组查询时不支持Filter条件.");

            return source;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagination">分页参数</param>
        /// <param name="alias">指定别名</param>
        /// <returns></returns>
        public static ISelectGrouping<TKey, TReturn> OrderBy<TKey, TReturn>(this ISelectGrouping<TKey, TReturn> source, PaginationDTO pagination, string alias = "a")
        {
            //string orderby = string.Empty;
            //if (!OrderByToSql(ref orderby, "a"))
            //    throw new MessageException("排序条件不支持");

            //if (!string.IsNullOrWhiteSpace(orderby))
            //    throw new MessageException("分组查询时不支持AdvancedSort条件.");

            return source;
        }

        /// <summary>
        /// 获取分页后的数据
        /// </summary>
        /// <typeparam name="TReturn">实体类型</typeparam>
        /// <param name="source"></param>
        /// <param name="pagination">分页参数</param>
        /// <param name="alias">指定别名</param>
        /// <returns></returns>
        public static ISelect<TReturn> GetPagination<TReturn>(this ISelect<TReturn> source, PaginationDTO pagination, string alias = "a") where TReturn : class
        {
            source.Filter(pagination, alias);
            pagination.RecordCount = source.Count();
            source.OrderBy(pagination, alias);

            if (pagination.PageIndex == -1)
                return source;
            else
                return source.Page(pagination.PageIndex, pagination.PageRows);
        }

        /// <summary>
        /// 获取分页后的数据
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TReturn">实体类型</typeparam>
        /// <param name="source"></param>
        /// <param name="pagination">分页参数</param>
        /// <param name="alias">指定别名</param>
        /// <returns></returns>
        public static ISelectGrouping<TKey, TReturn> GetPagination<TKey, TReturn>(this ISelectGrouping<TKey, TReturn> source, PaginationDTO pagination, string alias = "a") where TReturn : class
        {
            source.Filter(pagination, alias);
            pagination.RecordCount = source.Count();
            source.OrderBy(pagination, alias);

            if (pagination.PageIndex == -1)
                return source;
            else
                return source.Page(pagination.PageIndex, pagination.PageRows);
        }
    }
}
