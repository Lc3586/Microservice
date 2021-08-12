using Entity.Common;
using Microservice.Library.DataMapping.Annotations;
using Microservice.Library.DataMapping.Application;
using Microservice.Library.OpenApi.Annotations;
using System;
using System.Collections.Generic;

/* 
 * 文件信息业务模型
 */
namespace Model.Common.ChunkFileMergeTaskDTO
{
    /// <summary>
    /// 列表
    /// </summary>
    [MapFrom(typeof(Common_ChunkFileMergeTask))]
    [MapTo(typeof(Common_ChunkFileMergeTask))]
    [OpenApiMainTag("List")]
    public class List : Common_ChunkFileMergeTask
    {
        /// <summary>
        /// 完整名称
        /// </summary>
        public string FullName => $"{Name}{Extension}";

        /// <summary>
        /// 来源成员映射选项
        /// </summary>
        [OpenApiIgnore]
        public static MemberMapOptions<Common_ChunkFileMergeTask, List> FromMemberMapOptions =
            new MemberMapOptions<Common_ChunkFileMergeTask, List>();

        /// <summary>
        /// 当前成员映射选项
        /// </summary>
        [OpenApiIgnore]
        public static MemberMapOptions<List, Common_ChunkFileMergeTask> ToMemberMapOptions =
            new MemberMapOptions<List, Common_ChunkFileMergeTask>();
    }

    /// <summary>
    /// 分片来源信息
    /// </summary>
    public class ChunksSourceInfo
    {
        /// <summary>
        /// 分片规格
        /// </summary>
        public int Specs { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 热度信息
        /// </summary>
        public List<ActivityInfo> Activitys { get; set; }
    }

    /// <summary>
    /// 热度信息
    /// </summary>
    public class ActivityInfo
    {
        /// <summary>
        /// 热度
        /// </summary>
        public int Activity { get; set; }

        /// <summary>
        /// 百分比
        /// </summary>
        public decimal Percentage { get; set; }
    }
}
