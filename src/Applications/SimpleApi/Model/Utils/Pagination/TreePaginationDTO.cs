using Microservice.Library.OpenApi.Annotations;

namespace Model.Utils.Pagination
{
    /// <summary>
    /// 树状列表数据分页
    /// </summary>
    public class TreePaginationDTO
    {
        /// <summary>
        /// 父级Id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 层级数
        /// <para>为空则表示获取所有层级数据</para>
        /// </summary>
        public int? Rank { get; set; }

        /// <summary>
        /// 数据分页
        /// </summary>
        [OpenApiSchema(OpenApiSchemaType.model)]
        public PaginationDTO Pagination { get; set; } = new PaginationDTO();
    }
}
