using AutoMapper;
using Business.Interface.Common;
using Business.Interface.System;
using Business.Utils;
using Business.Utils.Pagination;
using Entity.Common;
using Entity.Public;
using Entity.System;
using FreeSql;
using Microservice.Library.Cache.Gen;
using Microservice.Library.Cache.Model;
using Microservice.Library.Cache.Services;
using Microservice.Library.Container;
using Microservice.Library.DataMapping.Gen;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using Microservice.Library.OpenApi.Extention;
using Microservice.Library.WeChat.Extension;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Model.Common;
using Model.Common.WeChatUserInfoDTO;
using Model.Utils.Pagination;
using Model.Utils.SampleAuthentication.SampleAuthenticationDTO;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Business.Implementation.Common
{
    /// <summary>
    /// 微信用户信息业务类
    /// </summary>
    public class WeChatOAuthHandler : BaseBusiness, IWeChatUserInfoBusiness, IWeChatOAuthHandler, IDependency
    {
        #region DI

        public WeChatOAuthHandler(
            IFreeSqlProvider freeSqlProvider,
            IAutoMapperProvider autoMapperProvider,
            ICacheProvider cacheProvider,
            IOperationRecordBusiness operationRecordBusiness,
            IMemberBusiness memberBusiness,
            IUserBusiness userBusiness,
            IFileBusiness fileBusiness)
        {
            Orm = freeSqlProvider.GetFreeSql();
            Repository = Orm.GetRepository<Common_WeChatUserInfo, string>();
            Repository_User = Orm.GetRepository<System_User, string>();
            Repository_UserWeChatUserInfo = Orm.GetRepository<System_UserWeChatUserInfo, string>();
            Repository_Member = Orm.GetRepository<Public_Member, string>();
            Repository_MemberWeChatUserInfo = Orm.GetRepository<Public_MemberWeChatUserInfo, string>();
            Mapper = autoMapperProvider.GetMapper();
            Cache = cacheProvider.GetCache();
            OperationRecordBusiness = operationRecordBusiness;
            MemberBusiness = memberBusiness;
            UserBusiness = userBusiness;
            FileBusiness = fileBusiness;
        }

        #endregion

        #region 私有成员

        readonly IFreeSql Orm;

        readonly IBaseRepository<Common_WeChatUserInfo, string> Repository;

        readonly IBaseRepository<System_User, string> Repository_User;

        readonly IBaseRepository<System_UserWeChatUserInfo, string> Repository_UserWeChatUserInfo;

        readonly IBaseRepository<Public_Member, string> Repository_Member;

        readonly IBaseRepository<Public_MemberWeChatUserInfo, string> Repository_MemberWeChatUserInfo;

        readonly IMapper Mapper;

        readonly ICache Cache;

        readonly IOperationRecordBusiness OperationRecordBusiness;

        readonly IMemberBusiness MemberBusiness;

        readonly IUserBusiness UserBusiness;

        readonly IFileBusiness FileBusiness;

        /// <summary>
        /// 创建微信用户信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="openId"></param>
        /// <param name="scope"></param>
        void CreateWeChatUserInfo(string appId, string openId, string scope)
        {
            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var entity = new Common_WeChatUserInfo
                {
                    AppId = appId,
                    OpenId = openId,
                    Scope = scope,
                    Enable = false
                }.InitEntityWithoutOP();

                Repository.Insert(entity);

                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(Common_WeChatUserInfo),
                    DataId = entity.Id,
                    Explain = $"创建微信用户信息[AppId {entity.AppId}, OpenId {entity.OpenId}]."
                });
            });

            if (!success)
                throw new ApplicationException("创建微信用户信息失败.", ex);
        }

        /// <summary>
        /// 更新微信用户信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userinfo"></param>
        void UpdateWeChatUserInfo(string appId, OAuthUserInfo userinfo)
        {
            if (Repository.UpdateDiy
                 .Where(o => o.AppId == appId && o.OpenId == userinfo.openid)
                 .Set(o => o.Nickname, userinfo.nickname)
                 .Set(o => o.HeadimgUrl, userinfo.headimgurl)
                 .Set(o => o.Sex, userinfo.sex)
                 .Set(o => o.Country, userinfo.country)
                 .Set(o => o.Province, userinfo.province)
                 .Set(o => o.City, userinfo.city)
                 .Set(o => o.Enable, true)
                 .Set(o => o.ModifyTime, DateTime.Now)
                 .ExecuteAffrows() <= 0)
                throw new ApplicationException($"更新微信用户信息失败, \r\n\tAppId: {appId}, \r\n\tOpenId: {userinfo.openid}.");
        }

        /// <summary>
        /// 获取微信信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        dynamic GetWeChatUserInfo(string appId, string openId)
        {
            var info = Repository.Where(o => o.AppId == appId && o.OpenId == openId).ToOne(o => new
            {
                o.Nickname,
                o.Sex,
                o.HeadimgUrl
            });

            if (info == null)
                throw new ApplicationException("微信信息不存在或已被移除.");

            return info;
        }

        /// <summary>
        /// 获取会员信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        dynamic GetMember(string appId, string openId)
        {
            var info = Repository_Member.Where(o => o.WeChatUserInfos.AsSelect().Where(p => p.AppId == appId && p.OpenId == openId).Any()).ToOne(o => new
            {
                o.Id,
                o.Account,
                o.Nickname,
                o.Name
            });

            return info;
        }

        /// <summary>
        /// 获取会员信息
        /// </summary>
        /// <param name="memberId">会员Id</param>
        /// <returns></returns>
        dynamic GetMember(string memberId)
        {
            var info = Repository_Member.Where(o => o.Id == memberId).ToOne(o => new
            {
                o.Id,
                o.Account,
                o.Nickname,
                o.Name
            });

            return info;
        }

        /// <summary>
        /// 保存外链文件
        /// </summary>
        /// <param name="uri">资源连接</param>
        /// <param name="fileId">文件Id</param>
        void SaveFile(string uri, out string fileId)
        {
            var file = FileBusiness.SingleImage(new Model.Common.FileDTO.ImageUploadParams
            {
                UrlOrBase64 = $"{uri.Substring(0, uri.LastIndexOf('/'))}/0",
                Download = true,
                IsCompress = true
            });

            fileId = file.Id;
        }

        /// <summary>
        /// 系统用户绑定微信
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId"></param>
        /// <param name="openId"></param>
        void BindUser(string userId, string appId, string openId)
        {
            var user = Repository_User.Where(o => o.Id == userId).ToOne(o => new
            {
                o.Id,
                o.Account,
                o.Name
            });

            if (user == null)
                throw new ApplicationException("绑定微信失败, 用户不存在或已被移除.");

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var entity = new System_UserWeChatUserInfo
                {
                    UserId = user.Id,
                    WeChatUserInfoId = Repository.Where(o => o.AppId == appId && o.OpenId == openId).ToOne(o => o.Id)
                };

                Repository_UserWeChatUserInfo.Insert(entity);

                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_UserWeChatUserInfo),
                    DataId = $"{entity.UserId}+{entity.WeChatUserInfoId}",
                    Explain = $"用户绑定微信[账号 {user.Account}, 姓名 {user.Name}], AppId {appId}, OpenId {openId}]."
                });
            });

            if (!success)
                throw new ApplicationException("绑定微信失败.", ex);
        }

        /// <summary>
        /// 更新系统用户信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId"></param>
        /// <param name="openId"></param>
        void UpdateUser(string userId, string appId, string openId)
        {
            var info = GetWeChatUserInfo(appId, openId);
            SaveFile(info.HeadimgUrl, out string imgId);
            UserBusiness.Edit(new Model.System.UserDTO.Edit
            {
                Id = userId,
                Nickname = info.Nickname,
                Face = imgId,
                Sex = info.Sex == 1 ? "男" : info.Sex == 2 ? "女" : null
            }, true);
        }

        /// <summary>
        /// 会员绑定微信
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="openId"></param>
        /// <param name="autoCreate">会员不存在时自动创建会员</param>
        void BindMember(string appId, string openId, bool autoCreate)
        {
            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var member = GetMember(appId, openId);

                if (member == null)
                {
                    if (!autoCreate)
                        throw new ApplicationException("绑定微信失败, 会员不存在或已被移除.");

                    var memberId = CreateMember(appId, openId, false);
                    member = GetMember(memberId);
                }

                var entity = new Public_MemberWeChatUserInfo
                {
                    MemberId = member.Id,
                    WeChatUserInfoId = Repository.Where(o => o.AppId == appId && o.OpenId == openId).ToOne(o => o.Id)
                };

                Repository_MemberWeChatUserInfo.Insert(entity);

                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(Public_MemberWeChatUserInfo),
                    DataId = $"{entity.MemberId}+{entity.WeChatUserInfoId}",
                    Explain = $"会员绑定微信[账号 {member.Account}, 昵称 {member.Nickname}, 姓名 {member.Name}], AppId {appId}, OpenId {openId}]."
                });
            });

            if (!success)
                throw new ApplicationException($"绑定微信失败, AppId[{appId}], OpenId[{openId}].", ex);
        }

        /// <summary>
        /// 创建会员
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="openId"></param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        /// <returns>返回会员Id</returns>
        string CreateMember(string appId, string openId, bool runTransaction = true)
        {
            var info = GetWeChatUserInfo(appId, openId);
            SaveFile(info.HeadimgUrl, out string imgId);
            return MemberBusiness.Create(new Model.Public.MemberDTO.Create
            {
                Account = $"member_{Repository_User.Select.Count():000000000}",
                Nickname = info.Nickname,
                Face = imgId,
                Sex = info.Sex == 1 ? "男" : info.Sex == 2 ? "女" : null,
                Enable = true,
                Remark = "通过微信自动创建会员账号."
            }, runTransaction);
        }

        /// <summary>
        /// 更新会员信息
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="appId"></param>
        /// <param name="openId"></param>
        void UpdateMember(string memberId, string appId, string openId)
        {
            var info = GetWeChatUserInfo(appId, openId);
            SaveFile(info.HeadimgUrl, out string imgId);
            MemberBusiness.Edit(new Model.Public.MemberDTO.Edit
            {
                Id = memberId,
                Nickname = info.Nickname,
                Face = imgId,
                Sex = info.Sex == 1 ? "男" : info.Sex == 2 ? "女" : null
            }, true);
        }

        /// <summary>
        /// 会员登录
        /// </summary>
        /// <param name="context"></param>
        /// <param name="authenticationInfo"></param>
        /// <returns></returns>
        async Task MemberLogin(HttpContext context, AuthenticationInfo authenticationInfo)
        {
            var claims = new List<Claim>
            {
                new Claim(nameof(AuthenticationInfo.Id), authenticationInfo.Id),
                new Claim(nameof(AuthenticationInfo.Account), authenticationInfo.Account),
                new Claim(nameof(AuthenticationInfo.Nickname), authenticationInfo.Nickname),
                new Claim(nameof(AuthenticationInfo.Face), authenticationInfo.Face)
            };

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
        }

        #endregion

        #region 外部接口

        public List<List> GetList(PaginationDTO pagination)
        {
            var entityList = Orm.Select<Common_WeChatUserInfo>()
                                .GetPagination(pagination)
                                .ToList<Common_WeChatUserInfo, List>(typeof(List).GetNamesWithTagAndOther(true, "_List"));

            var result = Mapper.Map<List<List>>(entityList);

            return result;
        }

        public Detail GetDetail(string id)
        {
            var entity = Repository.GetAndCheckNull(id);

            var result = Mapper.Map<Detail>(entity);

            result._Users = Repository_User.Where(o => o.WeChatUserInfos.AsSelect().Where(p => p.OpenId == result.OpenId).Any())
                                .ToDtoList<System_User, Model.System.UserDTO.Detail>(typeof(Model.System.UserDTO.Detail).GetNamesWithTagAndOther(true, "_List"));

            result._Members = Repository_Member.Where(o => o.WeChatUserInfos.AsSelect().Where(p => p.OpenId == result.OpenId).Any())
                                .ToDtoList<Public_Member, Model.Public.MemberDTO.Detail>(typeof(Model.Public.MemberDTO.Detail).GetNamesWithTagAndOther(true, "_List"));

            return result;
        }

        public string GetState(StateInfo data)
        {
            var state = IdHelper.NextIdString().Replace("-", "");
            Cache.SetCache(state, data, TimeSpan.FromMinutes(20), ExpireType.Absolute);
            return state;
        }

        public async Task Handler(HttpContext context, string appId, string openId, string scope, string state = null)
        {
            if (string.IsNullOrWhiteSpace(state))
                throw new ApplicationException("state参数不可为空.");

            if (!Cache.ContainsKey(state))
                throw new ApplicationException("state参数已过期.");

            var stateInfo = Cache.GetCache<StateInfo>(state);

            if (!Repository.Where(o => o.AppId == appId && o.OpenId == openId).Any())
            {
                CreateWeChatUserInfo(appId, openId, scope);
            }
            else if (!Repository.Where(o => o.AppId == appId && o.OpenId == openId && o.Scope == scope).Any())
            {
                //作用域变更，去更新微信用户信息

                //    if (Repository.UpdateDiy
                //         .Set(o => o.Scope, scope)
                //         .Where(o => o.AppId == appId && o.OpenId == openId)
                //         .ExecuteAffrows() <= 0)
                //        throw new ApplicationException($"更新微信用户作用域失败, \r\n\topenId: {openId}, \r\n\tscope: {scope}.");
            }
            else
            {
                //检查是否存在绑定关系
                var checkUserOrMember = stateInfo.Type == WeChatStateType.系统用户登录 || stateInfo.Type == WeChatStateType.系统用户绑定微信;

                if (Repository.Where(o => o.AppId == appId && o.OpenId == openId
                                        && ((checkUserOrMember == true && o.Users.AsSelect().Any()) || (!checkUserOrMember == true && o.Members.AsSelect().Any())))
                            .Any())
                {
                    switch (stateInfo.Type)
                    {
                        case WeChatStateType.系统用户登录:
                            break;
                        case WeChatStateType.会员登录:
                            await MemberLogin(context, MemberBusiness.Login(openId));
                            break;
                        case WeChatStateType.系统用户绑定微信:
                        case WeChatStateType.微信信息同步至系统用户信息:
                        case WeChatStateType.微信信息同步至会员信息:
                            goto next;
                        default:
                            break;
                    }

                    context.Response.Redirect(stateInfo.RedirectUrl);

                    return;
                }
                else if (stateInfo.Type == WeChatStateType.系统用户登录)
                {
                    //Console.WriteLine("输出未绑定提示.");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "text/plain;charset=UTF-8";
                    await context.Response.WriteAsync("请先绑定微信.");
                    return;
                }
            }

            next:
            context.Response.Redirect($"{Config.WeChatService.OAuthUserInfoUrl}?state={state}");
            await Task.FromResult(true);
        }

        public async Task Handler(HttpContext context, string appId, OAuthUserInfo userinfo, string state = null)
        {
            if (!Repository.Where(o => o.AppId == appId && o.OpenId == userinfo.openid).Any())
                throw new ApplicationException("微信信息不存在或已被移除.");

            if (string.IsNullOrWhiteSpace(state))
                throw new ApplicationException("state参数不可为空.");

            if (!Cache.ContainsKey(state))
                throw new ApplicationException("state参数已过期.");

            UpdateWeChatUserInfo(appId, userinfo);

            var stateInfo = Cache.GetCache<StateInfo>(state);

            switch (stateInfo.Type)
            {
                case WeChatStateType.系统用户绑定微信:
                    BindUser(stateInfo.Data["UserId"].ToString(), appId, userinfo.openid);
                    if (stateInfo.Data["AsyncUserInfo"]?.ToString() == true.ToString())
                        UpdateUser(stateInfo.Data["UserId"].ToString(), appId, userinfo.openid);
                    break;
                case WeChatStateType.微信信息同步至系统用户信息:
                    UpdateUser(stateInfo.Data["UserId"].ToString(), appId, userinfo.openid);
                    break;
                case WeChatStateType.会员登录:
                    BindMember(appId, userinfo.openid, (bool)stateInfo.Data["AutoCreate"]);
                    await MemberLogin(context, MemberBusiness.Login(userinfo.openid));
                    break;
                case WeChatStateType.微信信息同步至会员信息:
                    UpdateMember(stateInfo.Data["MemberId"].ToString(), appId, userinfo.openid);
                    break;
                case WeChatStateType.系统用户登录:
                default:
                    break;
            }

            context.Response.Redirect(stateInfo.RedirectUrl);
            await Task.FromResult(true);
        }

        #endregion
    }
}
