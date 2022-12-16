using Microservice.Library.Extension;
using System.Collections.Generic;
using System.Linq;
using T4CAGC.Extension;
using T4CAGC.Log;
using T4CAGC.Model;

namespace T4CAGC.Template
{
    /// <summary>
    /// 控制器类模板
    /// </summary>
    public partial class Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">选项</param>
        public Controller(ControllerOptions options)
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
        readonly ControllerOptions Options;

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

            NameSpaces.AddWhenNotContains("Api.Controllers.Utils");
            NameSpaces.AddWhenNotContains($"Model.{Options.Table.ModuleName}.{Options.Table.ReducedName}DTO");
            NameSpaces.AddWhenNotContains($"Business.Interface.{Options.Table.ModuleName}");
            NameSpaces.AddWhenNotContains("Business.Utils.Authorization");
            NameSpaces.AddWhenNotContains("Microsoft.AspNetCore.Mvc");
            NameSpaces.AddWhenNotContains("Model.Utils.Result");
            NameSpaces.AddWhenNotContains("Swashbuckle.AspNetCore.Annotations");
            NameSpaces.AddWhenNotContains("System.Net");
            NameSpaces.AddWhenNotContains("System.Threading.Tasks");

            NameSpaces.AddWhenNotContains("System.Collections.Generic");
            NameSpaces.AddWhenNotContains("System.Linq");
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

                        Functions.GetAndAddWhenNotContains(Function.List, tag);
                    }
                    else if (tag_lower.Contains("detail"))
                    {
                        Functions.GetAndAddWhenNotContains(Function.Detail, tag);
                    }
                    else if (tag_lower.Contains("create"))
                        Functions.GetAndAddWhenNotContains(Function.Create, tag);
                    else if (tag_lower.Contains("edit"))
                        Functions.GetAndAddWhenNotContains(Function.Edit, tag);
                    else if (tag_lower.Contains("enable"))
                    {
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

                        Functions.GetAndAddWhenNotContains(Function.Import, tag);
                    }
                    else if (tag_lower.Contains("export"))
                    {
                        NameSpaces.AddWhenNotContains("Model.Utils.OfficeDocuments");

                        Functions.GetAndAddWhenNotContains(Function.Export, tag);
                    }
                    else if (tag_lower.Contains("dropdownlist"))
                    {
                        NameSpaces.AddWhenNotContains("Model.Utils.Pagination");
                        NameSpaces.AddWhenNotContains("Microservice.Library.SelectOption");

                        Functions.GetAndAddWhenNotContains(Function.DropdownList, tag);
                    }
                });
            });
        }
    }
}
