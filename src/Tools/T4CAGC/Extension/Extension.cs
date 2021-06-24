using System;
using System.Collections.Generic;

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
    }
}
