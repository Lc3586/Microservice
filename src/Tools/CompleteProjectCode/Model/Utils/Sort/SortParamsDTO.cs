using Microservice.Library.OpenApi.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/* 
 * 排序参数业务模型
 */
namespace Model.Utils.Sort.SortParamsDTO
{
    /// <summary>
    /// 普通排序
    /// </summary>
    public class Sort
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 排序方法
        /// <para>默认值 up</para>
        /// </summary>
        [OpenApiSchema(OpenApiSchemaType.@enum, OpenApiSchemaFormat.enum_description, SortMethod.up)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SortMethod Method { get; set; }

        /// <summary>
        /// 跨度
        /// <para>移动几位</para>
        /// <para>默认值 1</para>
        /// </summary>
        public int Span { get; set; } = 1;
    }

    /// <summary>
    /// 拖动排序
    /// </summary>
    public class DragSort
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 目标Id
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// 是否排在目标之后
        /// </summary>
        /// <remarks>默认true</remarks>
        public bool Append { get; set; } = true;
    }

    /// <summary>
    /// 树状列表拖动排序
    /// </summary>
    public class TreeDragSort : DragSort
    {
        /// <summary>
        /// 是否移至目标内部
        /// </summary>
        /// <remarks>默认false</remarks>
        public bool Inside { get; set; } = false;
    }
}