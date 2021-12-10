using Microservice.Library.Extension;
using Microservice.Library.OpenApi.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using T4CAGC.Extension;

namespace T4CAGC.Template
{
    /// <summary>
    /// 实体类模板
    /// </summary>
    public partial class Entity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">选项</param>
        public Entity(EntityOptions options)
        {
            Options = options;
            AnalysisTable();
        }

        /// <summary>
        /// 选项
        /// </summary>
        readonly EntityOptions Options;

        /// <summary>
        /// 命名空间
        /// </summary>
        readonly List<string> NameSpaces = new List<string>();

        /// <summary>
        /// 特性
        /// </summary>
        readonly List<string> Attributes = new List<string>();

        /// <summary>
        /// 字段和特性
        /// </summary>
        readonly Dictionary<string, List<string>> FieldWithAttributes = new Dictionary<string, List<string>>();

        /// <summary>
        /// 分析表数据
        /// </summary>
        void AnalysisTable()
        {
            NameSpaces.AddWhenNotContains("System");

            //分析基础特性
            Options.Table.Fields.ForEach(o =>
            {
                var attributes = new List<string>();

                #region OpenApiSchema特性

                if (o.Tags.Any_Ex())
                {
                    NameSpaces.AddWhenNotContains("Microservice.Library.OpenApi.Annotations");

                    attributes.AddWhenNotContains($"OpenApiSubTag(\"{string.Join("\", \"", o.Tags)}\")");
                }

                if (o.OAS.Any_Ex())
                {
                    NameSpaces.AddWhenNotContains("Microservice.Library.OpenApi.Annotations");

                    var attribute = string.Empty;

                    var oastName = typeof(OpenApiSchemaType).GetFields().FirstOrDefault(f => (string)f.GetValue(null) == o.OAS[0])?.Name;
                    if (oastName == null)
                        throw new ApplicationException($"无效的接口架构类型 {o.OAS[0]}");
                    attribute += $"OpenApiSchemaType.{oastName}";

                    if (o.OAS.Length > 1)
                    {
                        var oasfName = typeof(OpenApiSchemaFormat).GetFields().FirstOrDefault(f => (string)f.GetValue(null) == o.OAS[1])?.Name;
                        if (oasfName == null)
                            throw new ApplicationException($"无效的接口架构格式 {o.OAS[1]}");
                        attribute += $", OpenApiSchemaFormat.{oasfName}";
                    }

                    if (o.OAS.Length > 2)
                        attribute += $", \"{o.OAS[2]}\"";

                    attributes.AddWhenNotContains($"OpenApiSchema({attribute})");
                }
                else if (o.Enums.Any_Ex())
                {
                    attributes.AddWhenNotContains($"OpenApiSchema(OpenApiSchemaType.@enum, OpenApiSchemaFormat.enum_description)");
                    attributes.AddWhenNotContains($"JsonConverter(typeof(StringEnumConverter))");
                }

                if (o.OASDTF.Any_Ex())
                {
                    NameSpaces.AddWhenNotContains("Microservice.Library.OpenApi.JsonExtension");

                    attributes.AddWhenNotContains($"JsonConverter(typeof(DateTimeConverter), {o.OASDTF})");
                }

                #endregion

                #region 数据验证

                if (o.Required)
                {
                    NameSpaces.AddWhenNotContains("System.ComponentModel.DataAnnotations");

                    attributes.AddWhenNotContains($"Required(ErrorMessage = \"{o.Description ?? o.Remark}不可为空\")");
                }

                #endregion

                #region 其他

                if (!o.Description.IsNullOrWhiteSpace())
                {
                    NameSpaces.AddWhenNotContains("System.ComponentModel");

                    attributes.AddWhenNotContains($"Description(\"{o.Description}\")");
                }

                #endregion

                #region 关联数据

                if (o.Virtual)
                {
                    NameSpaces.AddWhenNotContains("Microservice.Library.OpenApi.Annotations");
                    attributes.AddWhenNotContains("OpenApiIgnore");
                    NameSpaces.AddWhenNotContains("Newtonsoft.Json");
                    attributes.AddWhenNotContains("JsonIgnore");
                    NameSpaces.AddWhenNotContains("System.Xml.Serialization");
                    attributes.AddWhenNotContains("XmlIgnore");

                    NameSpaces.AddWhenNotContains($"Entity.{o.Bind.Split('_')[0]}");

                    if (!o.FK)
                    {
                        if (o.Bind != Options.Table.Name)
                            NameSpaces.AddWhenNotContains($"Entity.{o.KValue.Split('.')[0].Split('_')[0]}");
                        NameSpaces.AddWhenNotContains("System.Collections.Generic");
                    }

                    if (o.RK)
                        attributes.AddWhenNotContains($"Navigate(ManyToMany = typeof({o.KValue}))");
                    else
                        attributes.AddWhenNotContains($"Navigate(nameof({o.KValue}))");
                }

                #endregion

                FieldWithAttributes.GetAndAddWhenNotContains_ReferenceType(o.Name).AddRangeWhenNotContains(attributes);
            });

            //FreeSql特性
            if (Options.Table.FreeSql)
            {
                NameSpaces.AddWhenNotContains("FreeSql.DataAnnotations");

                Attributes.AddWhenNotContains("Table");

                var pks = new List<string>();
                var idxs = new Dictionary<string, List<string>>();
                var defaultIndexName = $"{Options.Table.Name}_idx_01".GetAbbreviation(30);

                Options.Table.Fields.ForEach(o =>
                {
                    if (o.Primary)
                        pks.Add($"pk_{Options.Table.Name}_{o.Name}".GetAbbreviation(30));

                    if (o.Index != Model.IndexType.None)
                        idxs.GetAndAddWhenNotContains(o.IndexName.IsNullOrWhiteSpace() ? defaultIndexName : o.IndexName, new List<string>()).Add($"nameof({o.Name}) + \" {o.Index}");

                    var attributes = new List<string>();

                    #region Column特性

                    var columnAttributes = new List<string>();
                    if (!o.DbType.IsNullOrWhiteSpace())
                        columnAttributes.Add($"DbType = \"{o.DbType}\"");

                    if (o.Length != 0)
                    {
                        if (o.CsType == typeof(string))
                            columnAttributes.Add($"StringLength = {o.Length}");
                        else
                            columnAttributes.Add($"Precision = {o.Length}");
                    }

                    if (o.Scale != 0)
                        columnAttributes.Add($"Scale = {o.Scale}");

                    if (o.Nullable)
                        columnAttributes.Add($"IsNullable = true");

                    if (columnAttributes.Any())
                        attributes.AddWhenNotContains($"Column({string.Join(", ", columnAttributes)})");

                    #endregion

                    FieldWithAttributes.GetAndAddWhenNotContains_ReferenceType(o.Name).AddRangeWhenNotContains(attributes);
                });

                if (pks.Any())
                    Attributes.AddWhenNotContains($"OraclePrimaryKeyName(\"{string.Join(",", pks)}\")");

                if (idxs.Any())
                    idxs.ForEach(o => Attributes.AddWhenNotContains($"Index(\"{o.Key}\", {string.Join(",\" + ", o.Value)}\")"));
            }

            //ES特性
            if (Options.Table.Elasticsearch)
            {
                NameSpaces.AddWhenNotContains("Nest");
                NameSpaces.AddWhenNotContains("Microservice.Library.Elasticsearch.Annotations");

                Attributes.AddWhenNotContains("ElasticsearchIndiceExtension");

                Options.Table.Fields.ForEach(o =>
                {
                    if (o.Primary)
                        Attributes.AddWhenNotContains($"ElasticsearchType(RelationName = nameof({Options.Table.Name}), IdProperty = nameof({o.Name}))");

                    string attributes;

                    if (o.NEST.IsNullOrWhiteSpace())
                    {
                        if (o.CsType == typeof(int) || o.CsType == typeof(decimal) || o.CsType == typeof(float) || o.CsType == typeof(long))
                            attributes = "Number";
                        else if (o.CsType == typeof(bool))
                            attributes = "Boolean";
                        else if (o.CsType == typeof(DateTime) || o.CsType == typeof(TimeSpan))
                        {
                            attributes = "Date";

                            if (o.OASDTF.Any_Ex())
                            {
                                NameSpaces.AddWhenNotContains("Microservice.Library.OpenApi.JsonExtension");

                                attributes += $"(Format = \"{o.OASDTF}\")";
                            }
                        }
                        else if (o.CsType == typeof(string) && o.Length > 100)
                            attributes = "Text";
                        else
                            attributes = "Keyword";
                    }
                    else
                        attributes = o.NEST;

                    FieldWithAttributes.GetAndAddWhenNotContains_ReferenceType(o.Name).AddWhenNotContains(attributes);
                });
            }

            //清理冗余命名空间
            NameSpaces.Remove($"Entity.{Options.Table.ModuleName}");
        }
    }
}
