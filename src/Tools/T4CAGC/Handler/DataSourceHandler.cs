using FreeSql.DatabaseModel;
using Microservice.Library.Container;
using Microservice.Library.FreeSql.Gen;
using Microservice.Library.OfficeDocuments;
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
        /// <summary>
        /// 获取CSV文件数据
        /// </summary>
        /// <param name="filename">csv文件</param>
        /// <param name="specifyTable">指定表</param>
        /// <param name="ignoreTable">忽略表</param>
        /// <returns></returns>
        public static List<TableInfo> GetCSVData(
             string filename,
             List<string> specifyTable = null,
             List<string> ignoreTable = null)
        {
            var tables = filename.ReadCSV(true, Encoding.UTF8);

            var result = new List<TableInfo>();

            int currentTableIndex = 0,
                currentFieldIndex = 0;

            foreach (DataRow row in tables.Rows)
            {
                foreach (DataColumn column in tables.Columns)
                {
                    var value = row[column].ToString();
                    if (value.Contains($"${SettingKeyword.表}"))
                    {
                        var match = Regex.Match(value, @$"[$]{SettingKeyword.表}[[](.*?)[]]{{(.*?)}}");
                        if (!match.Success)
                            continue;

                        var tableName = match.Groups[1].Value;
                        if (specifyTable?.Contains(tableName) == false)
                            continue;

                        if (ignoreTable?.Contains(tableName) == true)
                            continue;

                        result[currentTableIndex].Name = tableName;

                        if (match.Groups.Count >= 3)
                            result[currentTableIndex].Remark = match.Groups[2].Value;
                    }
                    else if (value.Contains($"${SettingKeyword.字段}"))
                    {

                    }
                    else if (value.Contains($"${SettingKeyword.主键}"))
                    {

                    }
                    else if (value.Contains($"${SettingKeyword.索引}"))
                    {

                    }
                    else if (value.Contains($"${SettingKeyword.关联数据字段}"))
                    {

                    }
                    else if (value.Contains($"${SettingKeyword.一对一关联标记}"))
                    {

                    }
                    else if (value.Contains($"${SettingKeyword.多对多关联标记}"))
                    {

                    }
                    else if (value.Contains($"${SettingKeyword.关联数据}"))
                    {

                    }
                    else if (value.Contains($"${SettingKeyword.类型}"))
                    {

                    }
                    else if (value.Contains($"${SettingKeyword.长度}"))
                    {

                    }
                    else if (value.Contains($"${SettingKeyword.小数位}"))
                    {

                    }
                    else if (value.Contains($"${SettingKeyword.可为空}"))
                    {

                    }
                    else if (value.Contains($"${SettingKeyword.说明}"))
                    {

                    }
                    else if (value.Contains($"${SettingKeyword.标签}"))
                    {

                    }
                    else if (value.Contains($"${SettingKeyword.接口框架数据格式化}"))
                    {

                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取数据库数据
        /// </summary>
        /// <param name="tableType">表类型</param>
        /// <param name="specifyTable">指定表</param>
        /// <param name="ignoreTable">忽略表</param>
        /// <returns></returns>
        public static List<TableInfo> GetDataBaseData(
             List<DbTableType> tableType,
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
