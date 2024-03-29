﻿using Api.Controllers.Utils;
using Business.Interface.System;
using Business.Utils.Authorization;
using Microservice.Library.Extension;
using Microsoft.AspNetCore.Mvc;
using Model.System.MenuDTO;
using Model.Utils.Pagination;
using Model.Utils.Result;
using Model.Utils.Sort.SortParamsDTO;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// 菜单接口
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "基础模块,权限模块")]
    [Route("/menu")]
    [SampleAuthorize(nameof(ApiPermissionRequirement))]
    [SwaggerTag("菜单接口")]
    public class MenuController : BaseApiController
    {
        #region DI

        public MenuController(IMenuBusiness menuBusiness)
        {
            MenuBusiness = menuBusiness;
        }

        readonly IMenuBusiness MenuBusiness;

        #endregion

        #region 基础接口

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        [HttpPost("list")]
        [SwaggerResponse((int)HttpStatusCode.OK, "列表数据", typeof(List))]
        public async Task<object> GetList([FromBody] PaginationDTO pagination)
        {
            return await Task.FromResult(OpenApiJsonContent(MenuBusiness.GetList(pagination), pagination));
        }

        /// <summary>
        /// 获取树状列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        [HttpPost("tree-list")]
        [SwaggerResponse((int)HttpStatusCode.OK, "树状列表数据", typeof(TreeList))]
        public async Task<object> GetTreeList([FromBody] TreePaginationDTO pagination)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(MenuBusiness.GetTreeList(pagination))));
        }

        /// <summary>
        /// 获取详情数据
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        [HttpPost("detail-data/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "详情数据", typeof(Detail))]
        public async Task<object> GetDetail(string id)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(MenuBusiness.GetDetail(id))));
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<object> Create([FromBody] Create data)
        {
            MenuBusiness.Create(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 获取编辑数据
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        [HttpPost("edit-data/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "编辑数据", typeof(Edit))]
        public async Task<object> GetEdit(string id)
        {
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(MenuBusiness.GetEdit(id))));
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("edit")]
        public async Task<object> Edit([FromBody] Edit data)
        {
            MenuBusiness.Edit(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        /// <returns></returns>
        [HttpPost("delete")]
        [NoJsonParamter]
        public async Task<object> Delete(IEnumerable<string> ids)
        {
            MenuBusiness.Delete(ids?.ToList());
            return await Task.FromResult(Success());
        }

        #endregion

        #region 拓展接口

        /// <summary>
        /// 启用/禁用
        /// </summary>
        /// <param name="id">数据</param>
        /// <param name="enable">设置状态</param>
        /// <returns></returns>
        [HttpPost("enable/{id}/{enable}")]
        public async Task<object> Enable(string id, bool enable)
        {
            MenuBusiness.Enable(id, enable);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("sort")]
        public async Task<object> Sort([FromBody] Sort data)
        {
            MenuBusiness.Sort(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 拖动排序
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("dragsort")]
        public async Task<object> DragSort([FromBody] TreeDragSort data)
        {
            MenuBusiness.DragSort(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 添加关联资源
        /// </summary>
        /// <param name="id">数据</param>
        /// <param name="resourcesIds">资源Id集合</param>
        /// <returns></returns>
        [HttpPost("associate-resources/{id}")]
        [NoJsonParamter]
        public async Task<object> AssociateResources(string id, IEnumerable<string> resourcesIds)
        {
            MenuBusiness.AssociateResources(id, resourcesIds?.ToList());
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 移除关联资源
        /// </summary>
        /// <param name="id">数据</param>
        /// <param name="resourcesIds">资源Id集合</param>
        /// <returns></returns>
        [HttpPost("disassociate-resources/{id}")]
        [NoJsonParamter]
        public async Task<object> DisassociateResources(string id, IEnumerable<string> resourcesIds)
        {
            MenuBusiness.DisassociateResources(id, resourcesIds?.ToList());
            return await Task.FromResult(Success());
        }

        #endregion        
    }
}
