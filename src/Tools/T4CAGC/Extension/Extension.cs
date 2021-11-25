using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using T4CAGC.Model;

namespace T4CAGC.Extension
{
    /// <summary>
    /// 拓展方法
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// 如果集合中不包含这个值则添加
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="value">值</param>
        public static void AddRangeWhenNotContains<T>(this List<T> list, List<T> value)
        {
            value.ForEach(o => list.AddWhenNotContains(o));
        }

        /// <summary>
        /// 如果集合中不包含这个值则添加
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="value">值</param>
        public static void AddWhenNotContains<T>(this List<T> list, T value)
        {
            if (!list.Contains(value))
                list.Add(value);
        }

        /// <summary>
        /// 获取字典集合中的值，如果字典集合中不包含这个值则添加
        /// </summary>
        /// <typeparam name="K">键类型</typeparam>
        /// <typeparam name="V">值类型</typeparam>
        /// <param name="dictionary">字典集合</param>
        /// <param name="defaultValue">默认值</param>
        public static V GetAndAddWhenNotContains<K, V>(this Dictionary<K, V> dictionary, K key, V defaultValue = default)
        {
            if (!dictionary.ContainsKey(key))
                dictionary.Add(key, defaultValue);

            return dictionary[key];
        }

        /// <summary>
        /// 获取字典集合中的值，如果字典集合中不包含这个值则添加
        /// </summary>
        /// <typeparam name="K">键类型</typeparam>
        /// <typeparam name="V">值类型</typeparam>
        /// <param name="dictionary">字典集合</param>
        /// <param name="defaultValue">默认值</param>
        public static V GetAndAddWhenNotContains_ReferenceType<K, V>(this Dictionary<K, V> dictionary, K key, V defaultValue = null) where V : class, new()
        {
            if (!dictionary.ContainsKey(key))
                dictionary.Add(key, defaultValue ?? new V());

            return dictionary[key];
        }

        /// <summary>
        /// 获取缩写
        /// </summary>
        /// <remarks>如果数据长度没有超过指定的长度限制, 则返回原始数据</remarks>
        /// <param name="value">数据</param>
        /// <param name="maxLength">最大长度</param>
        /// <param name="maxBlockLength">最大的块长度</param>
        /// <returns></returns>
        public static string GetAbbreviation(this string value, int maxLength, int maxBlockLength = 3)
        {
            if (value.Length <= maxLength)
                return value;

            for (int i = maxBlockLength; i > 0; i--)
            {
                var result = Abbreviation(i);
                if (result.Length <= maxLength)
                    return result;
            }

            throw new ApplicationException($"数据: {value}, 获取到的缩写超过了最大长度的限制, 请缩短该数据的长度.");

            string Abbreviation(int blockLength)
            {
                var result = string.Empty;

                for (int i = 0, j = blockLength; i < value.Length; i++)
                {
                    if (j == 0)
                        j = blockLength;
                    else if (i == 0 || j < blockLength - 1)
                    {
                        result += value[i];
                        blockLength--;
                    }
                    else if (char.IsUpper(value[i]))
                    {
                        result += value[i];
                        if (value.Length >= i + 2 && $"{value[i]}{value[i + 1]}".ToLower() == "id")
                        {
                            result += value[i + 1];
                            blockLength = 1;
                        }
                        else
                            blockLength = 2;
                    }
                    else if (value[i] == '_')
                    {
                        result += $"_{value[i + 1]}";
                        blockLength = 2;
                        i++;
                    }
                    else
                        continue;
                }

                return result;
            }
        }

        /// <summary>
        /// 分析名称
        /// </summary>
        /// <param name="table">数据表信息</param>
        public static void AnalysisName(this TableInfo table)
        {
            var index = table.Name.IndexOf('_');
            table.ModuleName = table.Name.Substring(0, index);
            table.ReducedName = table.Name[(index + 1)..];
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="value"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static bool Exist(this string value, string keyword)
        {
            return value == $"${keyword}" || value.Contains($"${keyword}[");
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="value"></param>
        /// <param name="keyword"></param>
        /// <param name="value1"></param>
        /// <returns></returns>
        public static bool Exist(this string value, string keyword, string value1)
        {
            return value.Contains($"${keyword}[{value1}]");
        }

        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="value"></param>
        /// <param name="keyword"></param>
        /// <param name="value1"></param>
        /// <returns></returns>
        public static bool TryMatch(this string value, string keyword, out string value1)
        {
            value1 = null;
            var match = Regex.Match(value, @$"[$]{keyword}[[](.*?)[]]");
            if (!match.Success)
                return false;

            value1 = match.Groups[1].Value;

            return true;
        }

        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="value"></param>
        /// <param name="keyword"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool TryMatch(this string value, string keyword, out string value1, out string value2)
        {
            value1 = value2 = null;
            var match = Regex.Match(value, @$"[$]{keyword}[[](.*?)[]]{{(.*?)}}");
            if (!match.Success)
                return false;

            value1 = match.Groups[1].Value;
            if (match.Groups.Count >= 3)
                value2 = match.Groups[2].Value;

            return true;
        }

        /// <summary>
        /// 设置异常
        /// </summary>
        /// <param name="keyword"></param>
        public static void SettingException(this string keyword)
        {
            throw new ApplicationException($"{keyword}设置有误, 请检查格式是否与此一致: ${keyword}[].");
        }

        /// <summary>
        /// 设置值异常
        /// </summary>
        /// <param name="keyword"></param>
        public static void SettingValueException(this string keyword, string value)
        {
            throw new ApplicationException($"无效的{keyword}值{value}.");
        }

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="throw">错误时抛出异常</param>
        public static Type GetCsType(this string typeName, bool @throw = true)
        {
            var type = typeName.ToLower() switch
            {
                "bool" => typeof(bool),
                "byte" => typeof(byte),
                "sbyte" => typeof(sbyte),
                "char" => typeof(char),
                "decimal" => typeof(decimal),
                "double" => typeof(double),
                "float" => typeof(float),
                "int" => typeof(int),
                "uint" => typeof(uint),
                "long" => typeof(long),
                "ulong" => typeof(ulong),
                "object" => typeof(object),
                "short" => typeof(short),
                "ushort" => typeof(ushort),
                "string" => typeof(string),
                "date" => typeof(DateTime),
                "datetime" => typeof(DateTime),
                "time" => typeof(TimeSpan),
                "guid" => typeof(Guid),
                _ => Type.GetType(typeName, false, true)
            };

            if (type == null && @throw)
                throw new ApplicationException($"无效的类型名称{typeName}.");

            return type;
        }

        /// <summary>
        /// 获取类型关键字名称
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="throw">错误时抛出异常</param>
        public static string GetCsTypeKeyword(this string typeName, bool @throw = true)
        {
            var type_lower = typeName.ToLower();
            var type = type_lower switch
            {
                "bool" or
                "byte" or
                "sbyte" or
                "char" or
                "decimal" or
                "double" or
                "float" or
                "int" or
                "uint" or
                "long" or
                "ulong" or
                "object" or
                 "short" or
                "ushort" or
               "string" => type_lower,
                "date" or
                "datetime" => "DateTime",
                "time" => "TimeSpan",
                "guid" => "Guid",
                _ => type_lower
            };

            if (type == null && @throw)
                throw new ApplicationException($"无效的类型名称{typeName}.");

            return type;
        }
    }
}
