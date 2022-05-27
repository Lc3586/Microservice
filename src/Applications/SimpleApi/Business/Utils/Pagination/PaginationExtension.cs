using Model.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Business.Utils.Pagination
{
    /// <summary>
    /// 分页设置拓展方法
    /// </summary>
    public static class PaginationExtension
    {
        /// <summary>
        /// 筛选条件转sql语句
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <param name="sql">sql语句</param>
        /// <param name="params">参数</param>
        /// <param name="alias">别名</param>
        /// <param name="character">名称标识符</param>
        /// <returns>筛选条件是否有误</returns>
        public static bool FilterToSql(this PaginationDTO pagination, ref string sql, out Dictionary<string, object> @params, string alias = null, char character = '"')
        {
            @params = new Dictionary<string, object>();
#pragma warning disable CS0618 // 类型或成员已过时
            if (pagination.Filter == null || !pagination.Filter.Any())
#pragma warning restore CS0618 // 类型或成员已过时
                return true;
            try
            {
                string predicate = string.Empty;

#pragma warning disable CS0618 // 类型或成员已过时
                for (int i = 0, j = i; i < pagination.Filter.Count; i++)
#pragma warning restore CS0618 // 类型或成员已过时
                {
#pragma warning disable CS0618 // 类型或成员已过时
                    var filter = pagination.Filter[i];
#pragma warning restore CS0618 // 类型或成员已过时
                    if (filter == null)
                        continue;

                    if (filter.Group?.Flag == FilterGroupFlag.start)
                        predicate += "(";

                    string field = filter.Field.Replace("'", "''").Replace("-", "");

                    string value;
                    if (filter.ValueIsField)
                    {
                        value = filter.Value?.ToString();
                        if (alias != null)
                            value = $"{character}{alias}{character}.{character}{value}{character}";
                        else
                            value = $"{character}{value}{character}";
                    }
                    else
                    {
                        value = $"@{field}";

                        @params.Add(field, filter.Value);
                    }

                    if (alias != null)
                        field = $"{character}{alias}{character}.{character}{field}{character}";
                    else
                        field = $"{character}{field}{character}";

                    bool skip = false;
                    switch (filter.Compare)
                    {
                        case FilterCompare.@in:
                            if (string.IsNullOrEmpty(value))
                            {
                                skip = true;
                                break;
                            }
                            predicate += $"{field} LIKE {value}";
                            break;
#pragma warning disable CS0618 // 类型或成员已过时
                        case FilterCompare.includedIn:
#pragma warning restore CS0618 // 类型或成员已过时
                            if (string.IsNullOrEmpty(value))
                            {
                                skip = true;
                                break;
                            }
                            predicate += $"{value} LIKE CONCAT('%',{field},'%')";
                            break;
                        case FilterCompare.eq:
                            if (value == null)
                                predicate += $"{field} is null";
                            else
                            {
                                predicate += $"{field} = {value}";
                            }
                            break;
                        case FilterCompare.notEq:
                            if (value == null)
                                predicate += $"{field} is not null";
                            else
                            {
                                predicate += $"{field} != {value}";
                            }
                            break;
                        case FilterCompare.le:
                            predicate += $"{field} <= {value}";
                            break;
                        case FilterCompare.lt:
                            predicate += $"{field} < {value}";
                            break;
                        case FilterCompare.ge:
                            predicate += $"{field} >= {value}";
                            break;
                        case FilterCompare.gt:
                            predicate += $"{field} > {value}";
                            break;
                        case FilterCompare.inSet:
                            predicate += $"{field} IN ({value})";
                            break;
                        case FilterCompare.notInSet:
                            predicate += $"{field} NOT IN ({value})";
                            break;
                        default:
                            skip = true;
                            break;
                    }

                    if (filter.Group?.Flag == FilterGroupFlag.end)
                        predicate += ")";

#pragma warning disable CS0618 // 类型或成员已过时
                    if (!skip && i != pagination.Filter.Count - 1)
#pragma warning restore CS0618 // 类型或成员已过时
                    {
                        string relation = filter.Group?.Relation.ToString();
                        predicate += relation switch
                        {
                            "or" => $" OR ",
                            _ => $" AND ",
                        };
                    }

                    if (!skip)
                        j++;
                }

                if (!string.IsNullOrWhiteSpace(predicate))
                    sql += ' ' + predicate + ' ';

                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
            {
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// 排序条件转sql语句
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <param name="sql">sql语句</param>
        /// <param name="alias">别名</param>
        /// <param name="character">名称标识符</param>
        /// <param name="withOrderByKeyword">附加orderby关键词</param>
        /// <returns>排序条件是否有误</returns>
        public static bool OrderByToSql(this PaginationDTO pagination, ref string sql, string alias = null, char character = '"', bool withOrderByKeyword = false)
        {
            try
            {
                string predicate = string.Empty;

                if (pagination.AdvancedSort != null && pagination.AdvancedSort.Any())
                {
                    foreach (var item in pagination.AdvancedSort)
                    {
                        if (item == null)
                            continue;

                        string field = item.Field.Replace("'", "''").Replace("-", "");
                        if (alias != null)
                            field = $" {character}{alias}{character}.{character}{field}{character} ";
                        else
                            field = $" {character}{field}{character} ";
                        string type = item.Type.ToString();


                        if (string.IsNullOrEmpty(type))
                            type = "asc";

                        if (!string.IsNullOrEmpty(predicate))
                            predicate += ',';

                        predicate += $" {field} {type} ";
                    }
                }
                else if (!string.IsNullOrEmpty(pagination.SortField))
                {
                    predicate += $" {(alias != null ? $" {character}{alias}{character}.{character}{pagination.SortField.Replace("'", "''").Replace("-", "")}{character} " : $" {character}{pagination.SortField.Replace("'", "''").Replace("-", "")}{character} ")} {pagination.SortType} ";
                }

                if (!string.IsNullOrWhiteSpace(predicate))
                {
                    if (withOrderByKeyword)
                        sql += " order by ";

                    sql += ' ' + predicate + ' ';
                }

                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
            {
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// 筛选条件转动态linq
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <param name="LinqDynamic">动态linq</param>
        /// <returns>筛选条件是否有误</returns>
        public static bool FilterToLinqDynamic<TSource>(this PaginationDTO pagination, ref IQueryable<TSource> LinqDynamic)
        {
            try
            {
#pragma warning disable CS0618 // 类型或成员已过时
                if (pagination.Filter == null || !pagination.Filter.Any())
#pragma warning restore CS0618 // 类型或成员已过时
                    return true;

                string predicate = string.Empty;
                List<object> args = new List<object>();

#pragma warning disable CS0618 // 类型或成员已过时
                for (int i = 0, j = i; i < pagination.Filter.Count; i++)
#pragma warning restore CS0618 // 类型或成员已过时
                {
#pragma warning disable CS0618 // 类型或成员已过时
                    var filter = pagination.Filter[i];
#pragma warning restore CS0618 // 类型或成员已过时
                    if (filter == null)
                        continue;

                    if (filter.Group?.Flag == FilterGroupFlag.start)
                        predicate += "(";

                    string field = filter.Field;
                    object value = filter.Value;

                    bool skip = false;
                    switch (filter.Compare)
                    {
                        case FilterCompare.@in:
                            if (string.IsNullOrEmpty(value.ToString()))
                            {
                                skip = true;
                                break;
                            }
                            predicate += $"{field}.Contains({(filter.ValueIsField ? "" : "@")}{j})";
                            args.Add(value);
                            break;
#pragma warning disable CS0618 // 类型或成员已过时
                        case FilterCompare.includedIn:
#pragma warning restore CS0618 // 类型或成员已过时
                            if (string.IsNullOrEmpty(value.ToString()))
                            {
                                skip = true;
                                break;
                            }
                            predicate += $"{(filter.ValueIsField ? "" : "@")}{j}.Contains({field})";
                            args.Add(value);
                            break;
                        case FilterCompare.eq:
                            predicate += $"{field} == {(filter.ValueIsField ? "" : "@")}{j}";
                            args.Add(value);
                            break;
                        case FilterCompare.notEq:
                            predicate += $"!{field}.Equals({(filter.ValueIsField ? "" : "@")}{j})";
                            args.Add(value);
                            break;
                        case FilterCompare.le:
                            predicate += $"{field} <= {(filter.ValueIsField ? "" : "@")}{j}";
                            args.Add(value);
                            break;
                        case FilterCompare.lt:
                            predicate += $"{field} < {(filter.ValueIsField ? "" : "@")}{j}";
                            args.Add(value);
                            break;
                        case FilterCompare.ge:
                            predicate += $"{field} >= {(filter.ValueIsField ? "" : "@")}{j}";
                            args.Add(value);
                            break;
                        case FilterCompare.gt:
                            predicate += $"{field} > {(filter.ValueIsField ? "" : "@")}{j}";
                            args.Add(value);
                            break;
                        case FilterCompare.inSet:
                        case FilterCompare.notInSet:
                        default:
                            skip = true;
                            break;
                    }

                    if (filter.Group?.Flag == FilterGroupFlag.end)
                        predicate += ")";

#pragma warning disable CS0618 // 类型或成员已过时
                    if (!skip && i != pagination.Filter.Count - 1)
#pragma warning restore CS0618 // 类型或成员已过时
                    {
                        string relation = filter.Group.Relation.ToString();
                        switch (filter.Group.Relation)
                        {
                            case FilterGroupRelation.and:
                                predicate += $" && ";
                                break;
                            case FilterGroupRelation.or:
                                predicate += $" || ";
                                break;
                            default:
                                break;
                        }
                    }

                    if (!skip)
                        j++;
                }

                if (!string.IsNullOrWhiteSpace(predicate))
                    LinqDynamic = (IQueryable<TSource>)typeof(DynamicQueryableExtensions)
                                                .GetMethods()
                                                .FirstOrDefault(o =>
                                                                    o.Name == "Where"
                                                                    && o.IsGenericMethod
                                                                    && o.GetParameters().Count(p =>
                                                                                                    !new[] { typeof(IQueryable<>), typeof(string), typeof(object[]) }
                                                                                                    .Contains(p.ParameterType.IsGenericType ? p.ParameterType.GetGenericTypeDefinition() : p.ParameterType)
                                                                                              ) == 0
                                                                )
                                               .MakeGenericMethod(typeof(TSource))
                                               .Invoke(null, new object[] { LinqDynamic, predicate, args.ToArray() });

                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
            {
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// 排序条件转动态linq
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="LinqDynamic">动态linq</param>
        /// <returns>排序条件是否有误</returns>
        public static bool OrderByToLinqDynamic<TSource>(this PaginationDTO pagination, ref IQueryable<TSource> LinqDynamic)
        {
            try
            {
                if (pagination.AdvancedSort != null && pagination.AdvancedSort.Any())
                {
                    IOrderedQueryable<TSource> orderByLinq = null;

                    foreach (var item in pagination.AdvancedSort)
                    {
                        if (item == null)
                            continue;

                        string field = item.Field;
                        string type = item.Type.ToString();

                        if (orderByLinq == null)
                            orderByLinq = LinqDynamic.OrderBy($"{field} {type}");
                        else
                            orderByLinq = orderByLinq.ThenBy($"{field} {type}");
                    }

                    LinqDynamic = orderByLinq;
                }
                else if (!string.IsNullOrEmpty(pagination.SortField))
                {
                    LinqDynamic = LinqDynamic.OrderBy($"{pagination.SortField} {pagination.SortType}");
                }
                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
            {
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}
