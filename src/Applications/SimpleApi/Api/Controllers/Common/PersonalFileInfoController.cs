using Api.Controllers.Utils;
using Business.Interface.Common;
using Business.Utils.Authorization;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.Mvc;
using Model.Common.FileDTO;
using Model.Utils.Pagination;
using Model.Utils.Result;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// 个人文件信息接口
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "文件管理")]
    [Route("/personal-file-info")]
    [SampleAuthorize(nameof(ApiAuthorizeRequirement))]
    [SwaggerTag("个人文件信息接口")]
    public class PersonalFileInfoController : BaseController
    {
        #region DI

        public PersonalFileInfoController(IPersonalFileInfoBusiness personalFileInfoBusiness)
        {
            PersonalFileInfoBusiness = personalFileInfoBusiness;
        }

        readonly IPersonalFileInfoBusiness PersonalFileInfoBusiness;

        #endregion

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        [HttpPost("list")]
        [Consumes("application/json", "application/x-www-form-urlencoded")]
        [Produces("application/json")]
        [SwaggerResponse((int)HttpStatusCode.OK, "列表数据", typeof(FileInfo))]
        public async Task<object> GetList([FromBody] PaginationDTO pagination)
        {
            return await Task.FromResult(OpenApiJsonContent(PersonalFileInfoBusiness.GetList(pagination), pagination));
        }

        /// <summary>
        /// 获取详情数据
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        [HttpPost("detail-data/{id}")]
        [Consumes("application/json", "application/x-www-form-urlencoded")]
        [Produces("application/json")]
        [SwaggerResponse((int)HttpStatusCode.OK, "详情数据", typeof(FileInfo))]
        public async Task<object> GetDetail(string id)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(PersonalFileInfoBusiness.GetDetail(id))));
        }

        /// <summary>
        /// 获取详情数据集合
        /// </summary>
        /// <param name="ids">id逗号拼接</param>
        /// <returns></returns>
        [HttpGet("detail-list/{ids}")]
        [Consumes("application/json", "application/x-www-form-urlencoded")]
        [Produces("application/json")]
        [SwaggerResponse((int)HttpStatusCode.OK, "详情数据", typeof(FileInfo))]
        public async Task<object> GetDetails(string ids)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(PersonalFileInfoBusiness.GetDetails(ids))));
        }

        /// <summary>
        /// 获取详情数据集合
        /// </summary>
        /// <param name="ids">id集合</param>
        /// <returns></returns>
        [HttpPost("detail-list")]
        [Consumes("application/json", "application/x-www-form-urlencoded")]
        [Produces("application/json")]
        [SwaggerResponse((int)HttpStatusCode.OK, "详情数据", typeof(FileInfo))]
        public async Task<object> GetDetails(List<string> ids)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(PersonalFileInfoBusiness.GetDetails(ids))));
        }

        /// <summary>
        /// 文件重命名
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        [HttpGet("rename/{id}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<object> Rename(string id, string filename)
        {
            PersonalFileInfoBusiness.Rename(id, filename);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        /// <returns></returns>
        [HttpPost("delete")]
        [Consumes("application/json", "application/x-www-form-urlencoded")]
        [Produces("application/json")]
        public async Task<object> Delete(IEnumerable<string> ids)
        {
            PersonalFileInfoBusiness.Delete(ids?.ToList());
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 预览
        /// </summary>
        /// <remarks>用于查看文件缩略图或视频截图</remarks>
        /// <param name="id">Id</param>
        /// <param name="width">指定宽度</param>
        /// <param name="height">指定高度</param>
        /// <param name="time">视频的时间轴位置（默认值: 00:00:00.001）</param>
        /// <returns></returns>
        [HttpGet("preview/{id}")]
        public async Task Preview(string id, int width = 500, int height = 500, TimeSpan? time = null)
        {
            await PersonalFileInfoBusiness.Preview(id, width, height, time);
        }

        /// <summary>
        /// 浏览
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        [HttpGet("browse/{id}")]
        public async Task Browse(string id)
        {
            await PersonalFileInfoBusiness.Browse(id);
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        [HttpGet("download/{id}")]
        public async Task Download(string id)
        {
            await PersonalFileInfoBusiness.Download(id);
        }
    }
}
