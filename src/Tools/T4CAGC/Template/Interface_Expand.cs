using Microservice.Library.Extension;
using System.Collections.Generic;
using System.Linq;
using T4CAGC.Extension;
using T4CAGC.Log;
using T4CAGC.Model;

namespace T4CAGC.Template
{
    /// <summary>
    /// 业务接口类模板
    /// </summary>
    public partial class Interface
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">选项</param>
        public Interface(InterfaceOptions options)
        {
            Options = options;
            AnalysisTable();
        }

        /// <summary>
        /// 跳过此次生成
        /// </summary>
        public bool Ignore = false;

        /// <summary>
        /// 选项
        /// </summary>
        readonly InterfaceOptions Options;

        /// <summary>
        /// 命名空间
        /// </summary>
        readonly List<string> NameSpaces = new List<string>();

        /// <summary>
        /// 方法集合
        /// </summary>
        /// <remarks>{方法,业务模型}</remarks>
        readonly Dictionary<Function, string> Functions = new Dictionary<Function, string>();

        /// <summary>
        /// 主键集合
        /// </summary>
        readonly List<FieldInfo> PrimaryKeys = new List<FieldInfo>();

        /// <summary>
        /// 分析表数据
        /// </summary>
        void AnalysisTable()
        {
            if (Options.Table.RelationshipTable)
            {
                Logger.Log(NLog.LogLevel.Info, LogType.系统跟踪, $"表信息中存在联合主键, 且没有其他字段, 可能为关系表, 已跳过.", Options.Table.Name);
                Ignore = true;
                return;
            }
            else if (!Options.Table.Fields.Any_Ex(o => o.Primary))
            {
                Logger.Log(NLog.LogLevel.Info, LogType.系统跟踪, $"表信息中未找到主键, 已跳过.", Options.Table.Name);
                Ignore = true;
                return;
            }

            if (Options.Table.Fields.Any(o => o.Consts.Any_Ex() || o.Enums.Any_Ex()))
                NameSpaces.AddWhenNotContains($"Model.{Options.Table.ModuleName}");

            NameSpaces.AddWhenNotContains($"Model.{Options.Table.ModuleName}.{Options.Table.ReducedName}DTO");

            NameSpaces.AddWhenNotContains("System.Collections.Generic");
            Functions.GetAndAddWhenNotContains(Function.Delete);

            Options.Table.Fields.ForEach(o =>
            {
                if (o.Primary)
                    PrimaryKeys.AddWhenNotContains(o);

                o.Tags?.ForEach(tag =>
                {
                    var tag_lower = tag.ToLower();

                    if (tag_lower.Contains("list"))
                    {
                        NameSpaces.AddWhenNotContains("Model.Utils.Pagination");
                        NameSpaces.AddWhenNotContains("System.Collections.Generic");

                        Functions.GetAndAddWhenNotContains(Function.List, tag);
                    }
                    else if (tag_lower.Contains("detail"))
                    {
                        if (Options.Table.Fields.Count(field => field.Primary) > 1)
                            NameSpaces.AddWhenNotContains("System.Collections.Generic");

                        Functions.GetAndAddWhenNotContains(Function.Detail, tag);
                    }
                    else if (tag_lower.Contains("create"))
                        Functions.GetAndAddWhenNotContains(Function.Create, tag);
                    else if (tag_lower.Contains("edit"))
                        Functions.GetAndAddWhenNotContains(Function.Edit, tag);
                    else if (tag_lower.Contains("enable"))
                    {
                        if (Options.Table.Fields.Count(field => field.Primary) > 1)
                            NameSpaces.AddWhenNotContains("System.Collections.Generic");

                        Functions.GetAndAddWhenNotContains(Function.Enable, tag);
                    }
                    else if (tag_lower.Contains("sort"))
                    {
                        NameSpaces.AddWhenNotContains("Model.Utils.Sort.SortParamsDTO");

                        Functions.GetAndAddWhenNotContains(Function.Sort, tag);
                    }
                    else if (tag_lower.Contains("import"))
                    {
                        NameSpaces.AddWhenNotContains("Microsoft.AspNetCore.Http");
                        NameSpaces.AddWhenNotContains("Model.Utils.OfficeDocuments");
                        NameSpaces.AddWhenNotContains("Model.Utils.OfficeDocuments.ExcelDTO");
                        NameSpaces.AddWhenNotContains("System.Threading.Tasks");

                        Functions.GetAndAddWhenNotContains(Function.Import, tag);
                    }
                    else if (tag_lower.Contains("export"))
                    {
                        NameSpaces.AddWhenNotContains("Model.Utils.OfficeDocuments");

                        Functions.GetAndAddWhenNotContains(Function.Export, tag);
                    }
                });
            });
        }
    }
}
