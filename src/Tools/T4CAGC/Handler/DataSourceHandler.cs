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
using T4CAGC.Extension;
using T4CAGC.Log;
using T4CAGC.Model;

namespace T4CAGC.Handler
{
    /// <summary>
    /// 数据源处理类
    /// </summary>
    public class DataSourceHandler : IHandler
    {
        public DataSourceHandler(
            Config config)
        {
            Config = config;

        }

        /// <summary>
        /// 配置
        /// </summary>
        readonly Config Config;

        /// <summary>
        /// 处理
        /// </summary>
        /// <returns>数据表信息</returns>
        public List<TableInfo> Handler()
        {
            var tables = Config.DataSourceType switch
            {
                DataSourceType.CSV => GetCSVData(),
                DataSourceType.CSV_Simple => GetCSVData_Simple(),
                _ => GetDataBaseData()
            };

            if (!tables.Any_Ex())
                throw new ApplicationException($"未获取到任何数据表.");

            tables.ForEach(o => Logger.Log(NLog.LogLevel.Info, LogType.系统信息, $"已获取数据表: {o.Name}{(o.FreeSql ? " [FreeSql]" : "")}{(o.Elasticsearch ? " [Elasticsearch]" : "")}{(o.Tree ? " [Tree]" : "")}."));

            return tables;
        }

        /// <summary>
        /// 获取CSV文件数据
        /// </summary>
        /// <param name="filename">csv文件</param>
        /// <param name="specifyTable">指定表</param>
        /// <param name="ignoreTable">忽略表</param>
        /// <returns></returns>
        public List<TableInfo> GetCSVData()
        {
            var tables = Config.DataSource.ReadCSV(true, Encoding.UTF8);
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

                    if (!tableInfo.Fields.Any_Ex(o => !o.Primary && !o.Virtual))
                        tableInfo.RelationshipTable = true;

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

                        if (value.Exist(SettingKeyword.普通表))
                            tableInfo = new TableInfo();
                        else if (value.Exist(SettingKeyword.FreeSql表))
                            tableInfo = new TableInfo { FreeSql = true };
                        else if (value.Exist(SettingKeyword.Elasticsearch字段属性))
                            tableInfo = new TableInfo { Elasticsearch = true };
                        else
                            continue;

                        if (value.Exist(SettingKeyword.树状结构))
                            tableInfo.Tree = true;

                        if (!value.TryMatch(SettingKeyword.普通表, out string tableName, out string remark))
                        {
                            if (value.TryMatch(SettingKeyword.FreeSql表, out tableName, out remark))
                                tableInfo.FreeSql = true;
                            else
                            {
                                if (value.TryMatch(SettingKeyword.Elasticsearch表, out tableName, out remark))
                                    tableInfo.Elasticsearch = true;
                                else
                                    SettingKeyword.普通表.SettingException();
                            }
                        }

                        if (Config.SpecifyTables?.Contains(tableName) == false
                            || Config.IgnoreTables?.Contains(tableName) == true
                            || tableInfos.Any(o => o.Name == tableName))
                            //未指定的表 或者 忽略的表 或者 冗余数据
                            continue;

                        tableInfo.Name = tableName;
                        tableInfo.AnalysisName();
                        tableInfo.Remark = remark;

                        return true;
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

                        if (value.Exist(SettingKeyword.普通表)
                            || value.Exist(SettingKeyword.FreeSql表)
                            || value.Exist(SettingKeyword.Elasticsearch字段属性))
                        {
                            if (value.Exist(SettingKeyword.普通表, tableInfo.Name)
                                || value.Exist(SettingKeyword.FreeSql表, tableInfo.Name)
                                || value.Exist(SettingKeyword.Elasticsearch字段属性, tableInfo.Name))
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

                        if (value.Exist(SettingKeyword.普通表)
                            || value.Exist(SettingKeyword.FreeSql表)
                            || value.Exist(SettingKeyword.Elasticsearch字段属性))
                        {
                            if (value.Exist(SettingKeyword.普通表, tableInfo.Name)
                                || value.Exist(SettingKeyword.FreeSql表, tableInfo.Name)
                                || value.Exist(SettingKeyword.Elasticsearch字段属性, tableInfo.Name))
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

                                fieldInfo.Type = dataType;
                                fieldInfo.CsType = dataType.GetCsType();
                                fieldInfo.CsTypeKeyword = dataType.GetCsTypeKeyword();
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

                                fieldInfo.Tags = tags.Split(',').ToList();
                            }
                            else if (value.Exist(SettingKeyword.接口架构属性))
                            {
                                if (!value.TryMatch(SettingKeyword.接口架构属性, out string oas))
                                    SettingKeyword.接口架构属性.SettingException();

                                fieldInfo.OAS = oas.Split(',');
                            }
                            else if (value.Exist(SettingKeyword.接口架构时间格式化))
                            {
                                if (!value.TryMatch(SettingKeyword.接口架构时间格式化, out string oasdtf))
                                    SettingKeyword.接口架构时间格式化.SettingException();

                                fieldInfo.OASDTF = oasdtf;
                            }
                            else if (value.Exist(SettingKeyword.Elasticsearch字段属性))
                            {
                                if (!value.TryMatch(SettingKeyword.Elasticsearch字段属性, out string nest))
                                    SettingKeyword.Elasticsearch字段属性.SettingException();

                                fieldInfo.NEST = nest;
                            }
                            else if (value.Exist(SettingKeyword.映射))
                            {
                                if (!value.TryMatch(SettingKeyword.映射, out string mapType, out string mapField))
                                    SettingKeyword.映射.SettingException();

                                fieldInfo.Maps.Add(mapType.GetCsType(), mapField);
                            }
                            else if (value.Exist(SettingKeyword.常量))
                            {
                                if (!value.TryMatch(SettingKeyword.常量, out string @const, out string constValue))
                                {
                                    if (!value.TryMatch(SettingKeyword.常量, out @const))
                                        SettingKeyword.常量.SettingException();

                                    @const.Split(',').ForEach(o => fieldInfo.Consts.Add(o, o));
                                }
                                else
                                    fieldInfo.Consts.Add(@const, constValue);
                            }
                            else if (value.Exist(SettingKeyword.枚举))
                            {
                                if (!value.TryMatch(SettingKeyword.枚举, out string @enum, out string enumValue))
                                {
                                    if (!value.TryMatch(SettingKeyword.枚举, out @enum))
                                        SettingKeyword.枚举.SettingException();

                                    @enum.Split(',').ForEach(o => fieldInfo.Enums.Add(o, null));
                                }
                                else
                                    fieldInfo.Enums.Add(@enum, enumValue.ToInt());
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
        public List<TableInfo> GetCSVData_Simple()
        {
            throw new NotImplementedException("暂不支持.");
        }

        /// <summary>
        /// 获取数据库数据
        /// </summary>
        /// <param name="tableType">表类型</param>
        /// <param name="specifyTable">指定表</param>
        /// <param name="ignoreTable">忽略表</param>
        /// <returns></returns>
        public List<TableInfo> GetDataBaseData()
        {
            var orm = AutofacHelper.GetService<IFreeSqlProvider>()
                  .GetFreeSql();

            var tables = new List<DbTableInfo>();

            if (Config.SpecifyTables.Any_Ex())
                Config.SpecifyTables.ForEach(o => tables.Add(orm.DbFirst.GetTableByName(o, true)));
            else
                tables = orm.DbFirst.GetTablesByDatabase();

            if (Config.TableType.Any_Ex() || Config.IgnoreTables.Any_Ex())
                tables.RemoveAll(o =>
                (Config.TableType.Any_Ex() && !Config.TableType.Contains(o.Type))
                || (Config.IgnoreTables.Any_Ex() && Config.IgnoreTables.Any(p => string.Equals(o.Name, p, StringComparison.OrdinalIgnoreCase))));

            return tables.Select(dbTable =>
            {
                var tableInfo = new TableInfo
                {
                    Name = dbTable.Name,
                    Remark = dbTable.Comment,
                    Fields = dbTable.Columns
                        .OrderBy(column => column.Position)
                        .Select(column =>
                        {
                            var fieldInfo = new FieldInfo
                            {
                                Name = column.Name,
                                Remark = column.Coment,
                                DbType = column.DbTypeText,
                                Type = column.CsType.Name,
                                CsType = column.CsType,
                                Primary = column.IsPrimary,
                                Nullable = column.IsNullable,
                                Length = column.MaxLength
                            };

                            return fieldInfo;
                        })
                        .ToList()
                };

                tableInfo.AnalysisName();

                return tableInfo;
            }).ToList();
        }
    }
}
