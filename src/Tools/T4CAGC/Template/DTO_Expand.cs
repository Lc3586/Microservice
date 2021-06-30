using Microservice.Library.Extension;
using System.Collections.Generic;
using T4CAGC.Extension;
using T4CAGC.Log;

namespace T4CAGC.Template
{
    /// <summary>
    /// 业务模型类模板
    /// </summary>
    public partial class DTO
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">选项</param>
        public DTO(DTOOptions options)
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
        readonly DTOOptions Options;

        /// <summary>
        /// 命名空间
        /// </summary>
        readonly List<string> NameSpaces = new List<string>();

        /// <summary>
        /// 模型和注释
        /// </summary>
        readonly Dictionary<string, string> ModelWithRemark = new Dictionary<string, string>();

        /// <summary>
        /// 模型和特性
        /// </summary>
        readonly Dictionary<string, List<string>> ModelWithAttributes = new Dictionary<string, List<string>>();

        /// <summary>
        /// 模型和数据映射
        /// </summary>
        readonly Dictionary<string, List<string>> ModelWithMaps = new Dictionary<string, List<string>>();

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

            NameSpaces.AddWhenNotContains($"Entity.{Options.Table.ModuleName}");
            NameSpaces.AddWhenNotContains($"Microservice.Library.DataMapping.Annotations");
            NameSpaces.AddWhenNotContains($"Microservice.Library.OpenApi.Annotations");

            //分析基础特性
            Options.Table.Fields.ForEach(o =>
            {
                if (!o.Tags.Any_Ex())
                    return;

                o.Tags.ForEach(tag =>
                {
                    if (ModelWithRemark.ContainsKey(tag))
                        return;

                    var remark = string.Empty;

                    var tag_lower = tag.ToLower();
                    if (tag_lower.Contains("list"))
                        remark = "列表";
                    else if (tag_lower.Contains("detail"))
                        remark = "详情";
                    else if (tag_lower.Contains("create"))
                        remark = "新增";
                    else if (tag_lower.Contains("edit"))
                        remark = "编辑";
                    else if (tag_lower.Contains("import"))
                        remark = "导入";
                    else if (tag_lower.Contains("export"))
                        remark = "导出";
                    else if (tag_lower.IndexOf("_") == 0)
                        return;
                    else if (tag_lower.Contains("sort"))
                        return;

                    ModelWithRemark.Add(tag, remark);

                    var attributes = new List<string>
                    {
                        $"MapFrom(typeof({Options.Table.Name}))",
                        $"OpenApiMainTag(\"{tag}\")"
                    };

                    ModelWithAttributes.Add(tag, attributes);
                });
            });
        }
    }
}
