using FreeSql.DatabaseModel;
using Microservice.Library.Container;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Gen;
using Microservice.Library.OfficeDocuments;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using T4CAGC.Model;

namespace T4CAGC.Handler
{
    /// <summary>
    /// 数据源处理类
    /// </summary>
    public static class DataSourceHandler
    {
        #region 私有成员

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="value"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        static bool Exist(this string value, string keyword)
        {
            return value.Contains($"${keyword}");
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="value"></param>
        /// <param name="keyword"></param>
        /// <param name="value1"></param>
        /// <returns></returns>
        static bool Exist(this string value, string keyword, string value1)
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
        static bool TryMatch(this string value, string keyword, out string value1)
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
        static bool TryMatch(this string value, string keyword, out string value1, out string value2)
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
        static void SettingException(this string keyword)
        {
            throw new ApplicationException($"{keyword}设置有误, 请检查格式是否与此一致: ${keyword}[].");
        }

        /// <summary>
        /// 设置值异常
        /// </summary>
        /// <param name="keyword"></param>
        static void SettingValueException(this string keyword, string value)
        {
            throw new ApplicationException($"无效的{keyword}值{value}.");
        }

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="throw">错误时抛出异常</param>
        static Type GetCsType(this string typeName, bool @throw = true)
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
                "guid" => typeof(Guid),
                _ => Type.GetType(typeName, false, true)
            };

            if (type == null && @throw)
                throw new ApplicationException($"无效的类型名称{typeName}.");

            return type;
        }

        #endregion

        /// <summary>
        /// 获取CSV文件数据
        /// </summary>
        /// <param name="filename">csv文件</param>
        /// <param name="specifyTable">指定表</param>
        /// <param name="ignoreTable">忽略表</param>
        /// <returns></returns>
        public static List<TableInfo> GetCSVData(
             this string filename,
             List<string> specifyTable = null,
             List<string> ignoreTable = null)
        {
            var tables = filename.ReadCSV(true, Encoding.UTF8);
            int rowsCount = tables.Rows.Count,
                columnsCount = tables.Columns.Count;

            var tableInfos = new List<TableInfo>();

            int rowIndex = 0,
                columnIndex = 0;

            try
            {
                while (NextTable(out TableInfo tableInfo))
                {
                    SetTableInfo(tableInfo);
                    tableInfos.Add(tableInfo);
                }
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException($"第{rowIndex + 2}行第{columnIndex + 1}列[{tables.Columns[columnIndex].ColumnName}]附近存在错误.", ex);
            }

            return tableInfos;

            bool NextTable(out TableInfo tableInfo)
            {
                for (; rowIndex < rowsCount; rowIndex++)
                {
                    for (; columnIndex < columnsCount; columnIndex++)
                    {
                        var value = tables.Rows[rowIndex][columnIndex].ToString();
                        if (value.Exist(SettingKeyword.表))
                        {
                            tableInfo = new TableInfo();

                            if (!value.TryMatch(SettingKeyword.表, out string tableName, out string remark))
                            {
                                if (!value.TryMatch(SettingKeyword.树状结构表, out tableName, out remark))
                                    SettingKeyword.表.SettingException();

                                tableInfo.Tree = true;
                            }

                            if (specifyTable?.Contains(tableName) == false
                                || ignoreTable?.Contains(tableName) == true
                                || tableInfos.Any(o => o.Name == tableName))
                                //未指定的表 或者 忽略的表 或者 冗余数据
                                continue;

                            tableInfo.Name = tableName;
                            tableInfo.Remark = remark;

                            return true;
                        }
                    }

                    if (columnIndex == columnsCount)
                        columnIndex = 0;
                }

                tableInfo = null;
                return false;
            }

            void SetTableInfo(TableInfo tableInfo)
            {
                while (NextField(tableInfo, out FieldInfo fieldInfo))
                {
                    SetFieldInfo(tableInfo, fieldInfo);
                    tableInfo.Fields.Add(fieldInfo);
                }
            }

            bool NextField(TableInfo tableInfo, out FieldInfo fieldInfo)
            {
                fieldInfo = null;

                for (; rowIndex < rowsCount; rowIndex++)
                {
                    for (; columnIndex < columnsCount; columnIndex++)
                    {
                        var value = tables.Rows[rowIndex][columnIndex].ToString();

                        if (value.IsNullOrWhiteSpace())
                            //无效内容
                            continue;

                        if (value.Exist(SettingKeyword.表))
                        {
                            if (value.Exist(SettingKeyword.表, tableInfo.Name))
                                //冗余数据
                                continue;
                            else
                                //下一张表
                                return false;
                        }

                        string fieldName = null;
                        string remark = null;
                        bool @virtual = false;

                        if (value.Exist(SettingKeyword.字段))
                        {
                            if (!value.TryMatch(SettingKeyword.字段, out fieldName, out remark))
                                SettingKeyword.字段.SettingException();
                        }
                        else if (value.Exist(SettingKeyword.关联数据字段))
                        {
                            if (!value.TryMatch(SettingKeyword.关联数据字段, out fieldName, out remark))
                                SettingKeyword.关联数据字段.SettingException();

                            @virtual = true;
                        }
                        else
                            //无效内容
                            continue;

                        if (tableInfo.Fields.Any(o => o.Name == fieldName))
                            //冗余数据
                            continue;

                        fieldInfo = new FieldInfo
                        {
                            Name = fieldName,
                            Remark = remark,
                            Virtual = @virtual
                        };

                        return true;
                    }

                    if (columnIndex == columnsCount)
                        columnIndex = 0;
                }

                return false;
            }

            void SetFieldInfo(TableInfo tableInfo, FieldInfo fieldInfo)
            {
                for (; rowIndex < rowsCount; rowIndex++)
                {
                    for (; columnIndex < columnsCount; columnIndex++)
                    {
                        var value = tables.Rows[rowIndex][columnIndex].ToString();

                        if (value.IsNullOrWhiteSpace())
                            //无效内容
                            continue;

                        if (value.Exist(SettingKeyword.表))
                        {
                            if (value.Exist(SettingKeyword.表, tableInfo.Name))
                                //冗余数据
                                continue;
                            else
                                //下一张表
                                return;
                        }

                        if (value.Exist(SettingKeyword.字段))
                        {
                            if (value.Exist(SettingKeyword.字段, fieldInfo.Name))
                                //冗余数据
                                continue;
                            else
                                //下一个字段
                                return;
                        }

                        if (value.Exist(SettingKeyword.关联数据字段))
                        {
                            if (value.Exist(SettingKeyword.关联数据字段, fieldInfo.Name))
                                //冗余数据
                                continue;
                            else
                                //下一个字段
                                return;
                        }

                        if (!fieldInfo.Virtual)
                        {
                            #region 基础

                            if (value.Exist(SettingKeyword.主键))
                                fieldInfo.Primary = true;
                            else if (value.Exist(SettingKeyword.可为空))
                                fieldInfo.Nullable = true;
                            else if (value.Exist(SettingKeyword.索引))
                            {
                                if (!value.TryMatch(SettingKeyword.索引, out string indexType))
                                    SettingKeyword.索引.SettingException();

                                if (!Enum.TryParse(indexType, out IndexType _indexType))
                                    SettingKeyword.索引.SettingValueException(indexType);

                                fieldInfo.Index = _indexType;
                            }
                            else if (value.Exist(SettingKeyword.数据库类型))
                            {
                                if (!value.TryMatch(SettingKeyword.数据库类型, out string dbType))
                                    SettingKeyword.数据库类型.SettingException();

                                fieldInfo.DbType = dbType;
                            }
                            else if (value.Exist(SettingKeyword.类型))
                            {
                                if (!value.TryMatch(SettingKeyword.类型, out string dataType))
                                    SettingKeyword.索引.SettingException();

                                fieldInfo.DataType = dataType.GetCsType();
                            }
                            else if (value.Exist(SettingKeyword.长度))
                            {
                                if (!value.TryMatch(SettingKeyword.长度, out string length))
                                    SettingKeyword.长度.SettingException();

                                fieldInfo.Length = Convert.ToInt32(length);
                            }
                            else if (value.Exist(SettingKeyword.小数位))
                            {
                                if (!value.TryMatch(SettingKeyword.小数位, out string scale))
                                    SettingKeyword.小数位.SettingException();

                                fieldInfo.Scale = Convert.ToInt32(scale);
                            }

                            #endregion

                            #region 拓展功能

                            else if (value.Exist(SettingKeyword.非空验证))
                                fieldInfo.Required = true;
                            else if (value.Exist(SettingKeyword.说明))
                            {
                                if (!value.TryMatch(SettingKeyword.说明, out string description))
                                    SettingKeyword.说明.SettingException();

                                fieldInfo.Description = description;
                            }
                            else if (value.Exist(SettingKeyword.标签))
                            {
                                if (!value.TryMatch(SettingKeyword.标签, out string tags))
                                    SettingKeyword.标签.SettingException();

                                fieldInfo.Tag = tags.Split(',').ToList();
                            }
                            else if (value.Exist(SettingKeyword.接口框架数据格式化))
                            {
                                if (!value.TryMatch(SettingKeyword.接口框架数据格式化, out string oasf))
                                    SettingKeyword.接口框架数据格式化.SettingException();

                                fieldInfo.OASF = oasf;
                            }
                            else if (value.Exist(SettingKeyword.映射))
                            {
                                if (!value.TryMatch(SettingKeyword.映射, out string mapType, out string mapField))
                                    SettingKeyword.映射.SettingException();

                                fieldInfo.Map.Add(mapType.GetCsType(), mapField);
                            }
                            else if (value.Exist(SettingKeyword.常量))
                            {
                                if (!value.TryMatch(SettingKeyword.常量, out string @const, out string constValue))
                                {
                                    if (!value.TryMatch(SettingKeyword.常量, out @const))
                                        SettingKeyword.常量.SettingException();

                                    @const.Split(',').ForEach(o => fieldInfo.Const.Add(o, o));
                                }
                                else
                                    fieldInfo.Const.Add(@const, constValue);
                            }
                            else if (value.Exist(SettingKeyword.枚举))
                            {
                                if (!value.TryMatch(SettingKeyword.枚举, out string @enum, out string enumValue))
                                {
                                    if (!value.TryMatch(SettingKeyword.枚举, out @enum))
                                        SettingKeyword.枚举.SettingException();

                                    @enum.Split(',').ForEach(o => fieldInfo.Enum.Add(o, null));
                                }
                                else
                                    fieldInfo.Enum.Add(@enum, enumValue.ToInt());
                            }

                            #endregion
                        }
                        else
                        {
                            #region 数据关联

                            if (value.Exist(SettingKeyword.一对一关联标记))
                            {
                                if (!value.TryMatch(SettingKeyword.一对一关联标记, out string kValue))
                                    SettingKeyword.一对一关联标记.SettingException();

                                fieldInfo.FK = true;
                                fieldInfo.KValue = kValue;
                            }
                            else if (value.Exist(SettingKeyword.一对多关联标记))
                            {
                                if (!value.TryMatch(SettingKeyword.一对多关联标记, out string kValue))
                                    SettingKeyword.一对多关联标记.SettingException();

                                fieldInfo.FRK = true;
                                fieldInfo.KValue = kValue;
                            }
                            else if (value.Exist(SettingKeyword.多对多关联标记))
                            {
                                if (!value.TryMatch(SettingKeyword.多对多关联标记, out string kValue))
                                    SettingKeyword.多对多关联标记.SettingException();

                                fieldInfo.RK = true;
                                fieldInfo.KValue = kValue;
                            }
                            else if (value.Exist(SettingKeyword.关联数据))
                            {
                                if (!value.TryMatch(SettingKeyword.关联数据, out string bind))
                                    SettingKeyword.关联数据.SettingException();

                                fieldInfo.Bind = bind;
                            }

                            #endregion
                        }
                    }

                    if (columnIndex == columnsCount)
                        columnIndex = 0;
                }
            }
        }

        /// <summary>
        /// 获取CSV文件数据
        /// </summary>
        /// <remarks>简单模式</remarks>
        /// <param name="filename"></param>
        /// <param name="specifyTable"></param>
        /// <param name="ignoreTable"></param>
        /// <returns></returns>
        public static List<TableInfo> GetCSVData_Simple(
             this string filename,
             List<string> specifyTable = null,
             List<string> ignoreTable = null)
        {
            throw new NotImplementedException("暂不支持.");

            var tables = filename.ReadCSV(true, Encoding.UTF8);
            int rowsCount = tables.Rows.Count,
                columnsCount = tables.Columns.Count;

            var tableInfos = new List<TableInfo>();

            int rowIndex = 0,
                columnIndex = 0;

            try
            {
                while (NextTable(out TableInfo tableInfo))
                {
                    SetTableInfo(tableInfo);
                    tableInfos.Add(tableInfo);
                }
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException($"第{rowIndex + 2}行第{columnIndex + 1}列[{tables.Columns[columnIndex].ColumnName}]附近存在错误.", ex);
            }

            return tableInfos;

            bool NextTable(out TableInfo tableInfo)
            {
                for (; rowIndex < rowsCount; rowIndex++)
                {
                    for (; columnIndex < columnsCount; columnIndex++)
                    {
                        var value = tables.Rows[rowIndex][columnIndex].ToString();
                        if (value.Exist(SettingKeyword.表))
                        {
                            if (!value.TryMatch(SettingKeyword.表, out string tableName, out string remark))
                                SettingKeyword.表.SettingException();

                            if (specifyTable?.Contains(tableName) == false
                                || ignoreTable?.Contains(tableName) == true
                                || tableInfos.Any(o => o.Name == tableName))
                                //未指定的表 或者 忽略的表 或者 冗余数据
                                continue;

                            tableInfo = new TableInfo
                            {
                                Name = tableName,
                                Remark = remark,
                                Fields = new List<FieldInfo>()
                            };

                            return true;
                        }
                    }

                    if (columnIndex == columnsCount)
                        columnIndex = 0;
                }

                tableInfo = null;
                return false;
            }

            void SetTableInfo(TableInfo tableInfo)
            {
                while (NextField(tableInfo, out FieldInfo fieldInfo))
                {
                    SetFieldInfo(tableInfo, fieldInfo);
                    tableInfo.Fields.Add(fieldInfo);
                }
            }

            bool NextField(TableInfo tableInfo, out FieldInfo fieldInfo)
            {
                fieldInfo = null;

                for (; rowIndex < rowsCount; rowIndex++)
                {
                    for (; columnIndex < columnsCount; columnIndex++)
                    {
                        var value = tables.Rows[rowIndex][columnIndex].ToString();

                        if (value.IsNullOrWhiteSpace())
                            //无效内容
                            continue;

                        if (value.Exist(SettingKeyword.表))
                        {
                            if (value.Exist(SettingKeyword.表, tableInfo.Name))
                                //冗余数据
                                continue;
                            else
                                //下一张表
                                return false;
                        }

                        string fieldName = null;
                        string remark = null;
                        bool @virtual = false;

                        if (value.Exist(SettingKeyword.字段))
                        {
                            if (!value.TryMatch(SettingKeyword.字段, out fieldName, out remark))
                                SettingKeyword.字段.SettingException();
                        }
                        else if (value.Exist(SettingKeyword.关联数据字段))
                        {
                            if (!value.TryMatch(SettingKeyword.关联数据字段, out fieldName, out remark))
                                SettingKeyword.关联数据字段.SettingException();

                            @virtual = true;
                        }
                        else
                            //无效内容
                            continue;

                        if (tableInfo.Fields.Any(o => o.Name == fieldName))
                            //冗余数据
                            continue;

                        fieldInfo = new FieldInfo
                        {
                            Name = fieldName,
                            Remark = remark,
                            Virtual = @virtual
                        };

                        return true;
                    }

                    if (columnIndex == columnsCount)
                        columnIndex = 0;
                }

                return false;
            }

            void SetFieldInfo(TableInfo tableInfo, FieldInfo fieldInfo)
            {
                for (; rowIndex < rowsCount; rowIndex++)
                {
                    for (; columnIndex < columnsCount; columnIndex++)
                    {
                        var value = tables.Rows[rowIndex][columnIndex].ToString();

                        if (value.IsNullOrWhiteSpace())
                            //无效内容
                            continue;

                        if (value.Exist(SettingKeyword.表))
                        {
                            if (value.Exist(SettingKeyword.表, tableInfo.Name))
                                //冗余数据
                                continue;
                            else
                                //下一张表
                                return;
                        }

                        if (value.Exist(SettingKeyword.字段))
                        {
                            if (value.Exist(SettingKeyword.字段, fieldInfo.Name))
                                //冗余数据
                                continue;
                            else
                                //下一个字段
                                return;
                        }

                        if (value.Exist(SettingKeyword.关联数据字段))
                        {
                            if (value.Exist(SettingKeyword.关联数据字段, fieldInfo.Name))
                                //冗余数据
                                continue;
                            else
                                //下一个字段
                                return;
                        }

                        if (!fieldInfo.Virtual)
                        {
                            #region 基础

                            if (value.Exist(SettingKeyword.主键))
                                fieldInfo.Primary = true;
                            else if (value.Exist(SettingKeyword.可为空))
                                fieldInfo.Nullable = true;
                            else if (value.Exist(SettingKeyword.索引))
                            {
                                if (!value.TryMatch(SettingKeyword.索引, out string indexType))
                                    SettingKeyword.索引.SettingException();

                                if (!Enum.TryParse(indexType, out IndexType _indexType))
                                    SettingKeyword.索引.SettingValueException(indexType);

                                fieldInfo.Index = _indexType;
                            }
                            else if (value.Exist(SettingKeyword.数据库类型))
                            {
                                if (!value.TryMatch(SettingKeyword.数据库类型, out string dbType))
                                    SettingKeyword.数据库类型.SettingException();

                                fieldInfo.DbType = dbType;
                            }
                            else if (value.Exist(SettingKeyword.类型))
                            {
                                if (!value.TryMatch(SettingKeyword.类型, out string dataType))
                                    SettingKeyword.索引.SettingException();

                                fieldInfo.DataType = dataType.GetCsType();
                            }
                            else if (value.Exist(SettingKeyword.长度))
                            {
                                if (!value.TryMatch(SettingKeyword.长度, out string length))
                                    SettingKeyword.长度.SettingException();

                                fieldInfo.Length = Convert.ToInt32(length);
                            }
                            else if (value.Exist(SettingKeyword.小数位))
                            {
                                if (!value.TryMatch(SettingKeyword.小数位, out string scale))
                                    SettingKeyword.小数位.SettingException();

                                fieldInfo.Scale = Convert.ToInt32(scale);
                            }

                            #endregion

                            #region 拓展功能

                            else if (value.Exist(SettingKeyword.非空验证))
                                fieldInfo.Required = true;
                            else if (value.Exist(SettingKeyword.说明))
                            {
                                if (!value.TryMatch(SettingKeyword.说明, out string description))
                                    SettingKeyword.说明.SettingException();

                                fieldInfo.Description = description;
                            }
                            else if (value.Exist(SettingKeyword.标签))
                            {
                                if (!value.TryMatch(SettingKeyword.标签, out string tags))
                                    SettingKeyword.标签.SettingException();

                                fieldInfo.Tag = tags.Split(',').ToList();
                            }
                            else if (value.Exist(SettingKeyword.接口框架数据格式化))
                            {
                                if (!value.TryMatch(SettingKeyword.接口框架数据格式化, out string oasf))
                                    SettingKeyword.接口框架数据格式化.SettingException();

                                fieldInfo.OASF = oasf;
                            }

                            #endregion
                        }
                        else
                        {
                            #region 数据关联

                            if (value.Exist(SettingKeyword.一对一关联标记))
                            {
                                if (!value.TryMatch(SettingKeyword.一对一关联标记, out string kValue))
                                    SettingKeyword.一对一关联标记.SettingException();

                                fieldInfo.FK = true;
                                fieldInfo.KValue = kValue;
                            }
                            else if (value.Exist(SettingKeyword.多对多关联标记))
                            {
                                if (!value.TryMatch(SettingKeyword.多对多关联标记, out string kValue))
                                    SettingKeyword.多对多关联标记.SettingException();

                                fieldInfo.RK = true;
                                fieldInfo.KValue = kValue;
                            }
                            else if (value.Exist(SettingKeyword.关联数据))
                            {
                                if (!value.TryMatch(SettingKeyword.关联数据, out string bind))
                                    SettingKeyword.关联数据.SettingException();

                                fieldInfo.Bind = bind;
                            }

                            #endregion
                        }
                    }

                    if (columnIndex == columnsCount)
                        columnIndex = 0;
                }
            }
        }

        /// <summary>
        /// 获取数据库数据
        /// </summary>
        /// <param name="tableType">表类型</param>
        /// <param name="specifyTable">指定表</param>
        /// <param name="ignoreTable">忽略表</param>
        /// <returns></returns>
        public static List<TableInfo> GetDataBaseData(
             this List<DbTableType> tableType,
             List<string> specifyTable = null,
             List<string> ignoreTable = null)
        {
            var orm = AutofacHelper.GetService<IFreeSqlProvider>()
                  .GetFreeSql();

            var tables = orm.DbFirst.GetTablesByDatabase();

            var tableInfoList = tables
                .Where(table =>
                {
                    if (!tableType.Contains(table.Type))
                        return false;

                    if (specifyTable?.Contains(table.Name) == false)
                        return false;

                    if (ignoreTable?.Contains(table.Name) == true)
                        return false;

                    return true;
                })
                .Select(table =>
                {
                    var tableInfo = new TableInfo
                    {
                        Name = table.Name,
                        Remark = table.Comment,
                        Fields = table.Columns
                            .OrderBy(column => column.Position)
                            .Select(column =>
                            {
                                var fieldInfo = new FieldInfo
                                {
                                    Name = column.Name,
                                    Remark = column.Coment,
                                    DbType = column.DbTypeText,
                                    DataType = column.CsType,
                                    Primary = column.IsPrimary,
                                    Nullable = column.IsNullable,
                                    Length = column.MaxLength
                                };

                                return fieldInfo;
                            })
                            .ToList()
                    };

                    return tableInfo;
                })
                .ToList();

            return tableInfoList;
        }
    }
}
