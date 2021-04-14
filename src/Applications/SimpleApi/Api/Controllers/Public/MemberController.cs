using Api.Controllers.Utils;
using Business.Interface.System;
using Business.Utils;
using Business.Utils.Authorization;
using Microservice.Library.Extension;
using Microservice.Library.SelectOption;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Public.MemberDTO;
using Model.Utils.Pagination;
using Model.Utils.Result;
using Model.Utils.SampleAuthentication.SampleAuthenticationDTO;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// 会员接口
    /// </summary>
    [Route("/member")]
    [SwaggerTag("会员接口")]
    [SampleAuthorize(nameof(ApiAuthorizeRequirement))]
    public class MemberController : BaseApiController
    {
        #region DI

        public MemberController(
            IHttpContextAccessor accessor,
            IMemberBusiness memberBusiness)
        {
            Context = accessor.HttpContext;
            MemberBusiness = memberBusiness;
        }

        readonly IMemberBusiness MemberBusiness;

        readonly HttpContext Context;

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
            return await Task.FromResult(OpenApiJsonContent(MemberBusiness.GetList(pagination), pagination));
        }

        /// <summary>
        /// 获取下拉列表数据
        /// </summary>
        /// <param name="condition">关键词(多个用空格分隔)</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        [HttpPost("dropdown-list")]
        [SwaggerResponse((int)HttpStatusCode.OK, "下拉列表选项数据", typeof(SelectOption))]
        public async Task<object> DropdownList(string condition, PaginationDTO pagination)
        {
            return await Task.FromResult(OpenApiJsonContent(MemberBusiness.DropdownList(condition, pagination), pagination));
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
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(MemberBusiness.GetDetail(id))));
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
            return await Task.FromResult(OpenApiJsonContent(ResponseDataFactory.Success(MemberBusiness.GetEdit(id))));
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        [HttpPost("edit")]
        public async Task<object> Edit([FromBody] Edit data)
        {
            MemberBusiness.Edit(data);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        /// <returns></returns>
        [HttpPost("delete")]
        public async Task<object> Delete(IEnumerable<string> ids)
        {
            MemberBusiness.Delete(ids?.ToList());
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
            MemberBusiness.Enable(id, enable);
            return await Task.FromResult(Success());
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <remarks>仅限开发使用</remarks>
        /// <param name="memberId">会员Id</param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<object> Login(string memberId)
        {
            if (Config.RunMode == Model.Utils.Config.RunMode.Publish)
                throw new MessageException("仅限开发使用.");

            var authenticationInfo = MemberBusiness.Login(memberId);

            var claims = new List<Claim>
            {
                new Claim(nameof(AuthenticationInfo.Id), authenticationInfo.Id),

                new Claim(ClaimTypes.Name, authenticationInfo.Account),

                new Claim(nameof(AuthenticationInfo.UserType), authenticationInfo.UserType),

                new Claim(ClaimTypes.GivenName, authenticationInfo.Nickname ?? string.Empty),
                new Claim(ClaimTypes.Gender, authenticationInfo.Sex ?? string.Empty),

                new Claim(nameof(AuthenticationInfo.Face), authenticationInfo.Face ?? string.Empty),

                new Claim(ClaimTypes.AuthenticationMethod, "DevLogin")
            };

            claims.AddRange(authenticationInfo.RoleTypes.Select(o => new Claim(ClaimTypes.Role, o)));

            await Context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));

            return Success("登录成功");
        }

        #endregion        
    }
}
