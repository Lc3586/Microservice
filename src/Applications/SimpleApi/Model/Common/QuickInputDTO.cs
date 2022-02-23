using Entity.Common;
using Microservice.Library.DataMapping.Annotations;
using Microservice.Library.OpenApi.Annotations;
using System.Collections.Generic;

/* 
 * 快捷输入业务模型
 */
namespace Model.Common.QuickInputDTO
{
    /// <summary>
    /// 列表
    /// </summary>
    [MapFrom(typeof(Common_QuickInput))]
    [OpenApiMainTag("List")]
    public class List : Common_QuickInput
    {

    }

    /// <summary>
    /// 选项列表
    /// </summary>
    [MapFrom(typeof(Common_QuickInput))]
    [OpenApiMainTag("OptionList")]
    public class OptionList : Common_QuickInput
    {

    }

    /// <summary>
    /// 新增
    /// </summary>
    [MapFrom(typeof(Common_QuickInput))]
    [OpenApiMainTag("Create")]
    public class Create : Common_QuickInput
    {

    }

    /// <summary>
    /// 新增
    /// </summary>
    public class BatchCreate
    {
        /// <summary>
        /// 数据集合
        /// </summary>
        [OpenApiSchema(OpenApiSchemaType.model)]
        public List<Create> Datas { get; set; }
    }
}
