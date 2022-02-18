using Entity.Common;
using Microservice.Library.DataMapping.Annotations;
using Microservice.Library.OpenApi.Annotations;

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
    /// 新增
    /// </summary>
    [MapFrom(typeof(Common_QuickInput))]
    [OpenApiMainTag("Create")]
    public class Create : Common_QuickInput
    {

    }
}
