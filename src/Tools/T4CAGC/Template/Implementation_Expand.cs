using Microservice.Library.Extension;
using System.Collections.Generic;
using System.Linq;
using T4CAGC.Extension;
using T4CAGC.Log;
using T4CAGC.Model;

namespace T4CAGC.Template
{
    /// <summary>
    /// 业务实现类模板
    /// </summary>
    public partial class Implementation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">选项</param>
        public Implementation(ImplementationOptions options)
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
        readonly ImplementationOptions Options;

        /// <summary>
        /// 命名空间
        /// </summary>
        readonly List<string> NameSpaces = new List<string>();

        /// <summary>
        /// 依赖注入服务
        /// </summary>
        readonly Dictionary<string, string> DIServices = new Dictionary<string, string>();

        /// <summary>
        /// 方法集合
        /// </summary>
        /// <remarks>{方法,业务模型}</remarks>
        readonly Dictionary<Function, string> Functions = new Dictionary<Function, string>();

        /// <summary>
        /// 方法集合
        /// </summary>
        /// <remarks>{方法,字段}</remarks>
        readonly Dictionary<Function, FieldInfo> FunctionsWithField = new Dictionary<Function, FieldInfo>();

        /// <summary>
        /// 方法集合
        /// </summary>
        /// <remarks>{方法,字段集合}</remarks>
        readonly Dictionary<Function, List<FieldInfo>> FunctionsWithFields = new Dictionary<Function, List<FieldInfo>>();

        /// <summary>
        /// 主键字段集合
        /// </summary>
        readonly List<FieldInfo> PrimaryKeys = new List<FieldInfo>();

        /// <summary>
        /// 非空字段集合
        /// </summary>
        readonly List<FieldInfo> RequiredKeys = new List<FieldInfo>();

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

            NameSpaces.AddWhenNotContains($"Entity.{Options.Table.ModuleName}");
            NameSpaces.AddWhenNotContains($"Model.{Options.Table.ModuleName}.{Options.Table.ReducedName}DTO");
            NameSpaces.AddWhenNotContains("Model.Utils.Log");
            NameSpaces.AddWhenNotContains($"Business.Interface.{Options.Table.ModuleName}");
            NameSpaces.AddWhenNotContains("Business.Interface.Common");
            NameSpaces.AddWhenNotContains("Entity.Common");
            NameSpaces.AddWhenNotContains("System");
            NameSpaces.AddWhenNotContains("System.Collections.Generic");
            NameSpaces.AddWhenNotContains("System.Linq");
            NameSpaces.AddWhenNotContains("AutoMapper");
            NameSpaces.AddWhenNotContains("Microservice.Library.Extension");
            NameSpaces.AddWhenNotContains("Microservice.Library.DataMapping.Gen");
            NameSpaces.AddWhenNotContains("Microservice.Library.OpenApi.Extention");
            NameSpaces.AddWhenNotContains("Business.Utils");

            DIServices.GetAndAddWhenNotContains("IOperationRecordBusiness", "OperationRecordBusiness");

            Functions.GetAndAddWhenNotContains(Function.Delete);

            if (Options.Table.FreeSql)
            {
                NameSpaces.AddWhenNotContains("FreeSql");
                NameSpaces.AddWhenNotContains("Microservice.Library.FreeSql.Extention");
                NameSpaces.AddWhenNotContains("Microservice.Library.FreeSql.Gen");
            }

            if (Options.Table.Elasticsearch)
            {
                NameSpaces.AddWhenNotContains("Microservice.Library.Elasticsearch");
                NameSpaces.AddWhenNotContains("Microservice.Library.Elasticsearch.Gen");
            }

            Options.Table.Fields.ForEach(o =>
            {
                if (o.Primary)
                    PrimaryKeys.AddWhenNotContains(o);

                if (o.Required)
                    RequiredKeys.AddWhenNotContains(o);

                o.Tags?.ForEach(tag =>
                {
                    var tag_lower = tag.ToLower();

                    if (tag_lower.Contains("list"))
                    {
                        NameSpaces.AddWhenNotContains("Business.Utils.Pagination");
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
                        if (o.CsType == typeof(bool))
                            FunctionsWithField.GetAndAddWhenNotContains(Function.Enable, o);

                        Functions.GetAndAddWhenNotContains(Function.Enable, tag);
                    }
                    else if (tag_lower.Contains("sort"))
                    {
                        NameSpaces.AddWhenNotContains("Model.Utils.Sort");
                        NameSpaces.AddWhenNotContains("Model.Utils.Sort.SortParamsDTO");

                        if (o.CsType == typeof(int))
                            FunctionsWithField.GetAndAddWhenNotContains(Function.Sort, o);

                        Functions.GetAndAddWhenNotContains(Function.Sort, tag);
                    }
                    else if (tag_lower.Contains("import"))
                    {
                        NameSpaces.AddWhenNotContains("Business.Utils.Log");
                        NameSpaces.AddWhenNotContains("FreeSql.DataAnnotations");
                        NameSpaces.AddWhenNotContains("System.Data");
                        NameSpaces.AddWhenNotContains("System.Text.Encodings.Web");
                        NameSpaces.AddWhenNotContains("Microsoft.AspNetCore.Http");
                        NameSpaces.AddWhenNotContains("Microservice.Library.Http");
                        NameSpaces.AddWhenNotContains("Microservice.Library.OfficeDocuments");
                        NameSpaces.AddWhenNotContains("Model.Utils.OfficeDocuments");
                        NameSpaces.AddWhenNotContains("Model.Utils.OfficeDocuments.ExcelDTO");
                        NameSpaces.AddWhenNotContains("System.Threading.Tasks");
                        NameSpaces.AddWhenNotContains("System.ComponentModel");
                        NameSpaces.AddWhenNotContains("System.Text");
                        NameSpaces.AddWhenNotContains("System.Reflection");
                        NameSpaces.AddWhenNotContains("System.IO");

                        DIServices.GetAndAddWhenNotContains("IHttpContextAccessor", "HttpContextAccessor");

                        Functions.GetAndAddWhenNotContains(Function.Import, tag);
                    }
                    else if (tag_lower.Contains("export"))
                    {
                        NameSpaces.AddWhenNotContains("System.Data");
                        NameSpaces.AddWhenNotContains("Microsoft.AspNetCore.Http");
                        NameSpaces.AddWhenNotContains("Microservice.Library.OfficeDocuments");
                        NameSpaces.AddWhenNotContains("Model.Utils.Pagination");
                        NameSpaces.AddWhenNotContains("Model.Utils.OfficeDocuments");
                        NameSpaces.AddWhenNotContains("Newtonsoft.Json");
                        NameSpaces.AddWhenNotContains("System.ComponentModel");
                        NameSpaces.AddWhenNotContains("System.Reflection");
                        NameSpaces.AddWhenNotContains("System.Text.Encodings.Web");
                        NameSpaces.AddWhenNotContains("System.IO");

                        DIServices.GetAndAddWhenNotContains("IHttpContextAccessor", "HttpContextAccessor");

                        Functions.GetAndAddWhenNotContains(Function.Export, tag);
                    }
                    else if (tag_lower.Contains("dropdownlist"))
                    {
                        NameSpaces.AddWhenNotContains("Model.Utils.Pagination");
                        NameSpaces.AddWhenNotContains("System.Collections.Generic");
                        NameSpaces.AddWhenNotContains("Microservice.Library.SelectOption");

                        Functions.GetAndAddWhenNotContains(Function.DropdownList, tag);

                        FunctionsWithFields.GetAndAddWhenNotContains_ReferenceType(Function.DropdownList).Add(o);
                    }
                });
            });
        }
    }
}
