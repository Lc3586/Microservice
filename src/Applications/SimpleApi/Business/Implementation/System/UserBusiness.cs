using AutoMapper;
using Business.Interface.Common;
using Business.Interface.System;
using Business.Utils;
using Business.Utils.Filter;
using Business.Utils.Pagination;
using Entity.Common;
using Entity.System;
using FreeSql;
using Microservice.Library.Cache.Gen;
using Microservice.Library.Cache.Model;
using Microservice.Library.Cache.Services;
using Microservice.Library.DataMapping.Gen;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using Microservice.Library.OpenApi.Extention;
using Microservice.Library.SelectOption;
using Model.Common;
using Model.System;
using Model.System.UserDTO;
using Model.Utils.Pagination;
using Model.Utils.SampleAuthentication.SampleAuthenticationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Business.Implementation.System
{
    /// <summary>
    /// 系统用户业务类
    /// </summary>
    public class UserBusiness : BaseBusiness, IUserBusiness
    {
        #region DI

        public UserBusiness(
            IFreeSqlProvider freeSqlProvider,
            IAutoMapperProvider autoMapperProvider,
            IOperationRecordBusiness operationRecordBusiness,
            IAuthoritiesBusiness authoritiesBusiness,
            IEntryLogBusiness entryLogBusiness,
            IRoleBusiness roleBusiness,
            ICacheProvider cacheProvider)
        {
            Orm = freeSqlProvider.GetFreeSql();
            Repository = Orm.GetRepository<System_User, string>();
            Repository_Role = Orm.GetRepository<System_Role, string>();
            Repository_UserRole = Orm.GetRepository<System_UserRole, string>();
            Repository_UserMenu = Orm.GetRepository<System_UserMenu, string>();
            Repository_UserResources = Orm.GetRepository<System_UserResources, string>();
            Mapper = autoMapperProvider.GetMapper();
            OperationRecordBusiness = operationRecordBusiness;
            AuthoritiesBusiness = authoritiesBusiness;
            EntryLogBusiness = entryLogBusiness;
            RoleBusiness = roleBusiness;
            Cache = cacheProvider.GetCache();
        }

        #endregion

        #region 私有成员

        readonly IFreeSql Orm;

        readonly IBaseRepository<System_User, string> Repository;

        readonly IBaseRepository<System_Role, string> Repository_Role;

        readonly IBaseRepository<System_UserRole, string> Repository_UserRole;

        readonly IBaseRepository<System_UserMenu, string> Repository_UserMenu;

        readonly IBaseRepository<System_UserResources, string> Repository_UserResources;

        readonly IMapper Mapper;

        readonly IOperationRecordBusiness OperationRecordBusiness;

        readonly IAuthoritiesBusiness AuthoritiesBusiness;

        readonly IEntryLogBusiness EntryLogBusiness;

        readonly IRoleBusiness RoleBusiness;

        readonly ICache Cache;

        /// <summary>
        /// 初始化超级管理员账号
        /// </summary>
        void InitAdmin()
        {
            try
            {
                (bool success, Exception ex) = Orm.RunTransaction(() =>
                {
                    //创建账号
                    Create(new Create
                    {
                        Account = Config.AdminAccount,
                        Password = Config.AdminInitPassword,
                        Enable = true,
                        Nickname = "超级管理员",
                        Remark = "系统自动创建超级管理员账号."
                    }, false);

                    //创建角色
                    RoleBusiness.Create(new Model.System.RoleDTO.Create
                    {
                        Name = "超级管理员",
                        Type = RoleType.超级管理员,
                        Enable = true,
                        Remark = "系统自动创建超级管理员角色.",
                        Code = "000000"
                    }, false);

                    //角色授权
                    AuthoritiesBusiness.AuthorizeRoleForUser(new Model.System.AuthorizeDTO.RoleForUser
                    {
                        UserIds = new List<string> { Repository.Where(o => o.Account == Config.AdminAccount).ToOne(o => o.Id) },
                        RoleIds = Repository_Role.Where(o => o.Type == $"{RoleType.超级管理员}").ToList(o => o.Id)
                    }, false);
                });

                if (!success)
                    throw ex;
            }
            catch (ApplicationException ex)
            {
                throw new MessageException("初始化超级管理员账号失败.", ex);
            }
        }

        #endregion

        #region 外部接口

        #region 基础功能

        public List<List> GetList(PaginationDTO pagination)
        {
            var entityList = Repository.Select
                                    .GetPagination(pagination)
                                    .ToList<System_User, List>(typeof(List).GetNamesWithTagAndOther(true, "_List"));

            var result = Mapper.Map<List<List>>(entityList);

            return result;
        }

        public List<SelectOption> DropdownList(string condition, PaginationDTO pagination)
        {
            var fields = new[] {
                nameof(System_User.Account),
                nameof(System_User.Name)
            };

            var where = new List<string>();
            if (!condition.IsNullOrWhiteSpace())
            {
                var index = 0;
                var add = false;
                foreach (var item in condition)
                {
                    if (item != ' ')
                    {
                        if (where.Count < index + 1)
                            where.Add("");
                        where[index] += item;
                        add = false;
                    }
                    else
                    {
                        if (add)
                            continue;
                        index++;
                        add = true;
                    }
                }
            }

            var where_sql = where.Any() ? string.Join(" or ", where.Select(o => string.Join(" or ", fields.Select(p => $"a.\"{p}\" like '%{o}%'")))) : null;

            var type = typeof(System_User);

            var select = SelectExtension.Select<System_User, SelectOption>(a => new SelectOption
            {
                text = a.Account,
                value = a.Id,
                search = $"{a.Account} {a.Name}",
                options = new List<OptionInfo>
                {
                    new OptionInfo
                    {
                        display = type.GetDescription(nameof(a.Account)),
                        value = a.Account
                    },
                    new OptionInfo
                    {
                        display =type.GetDescription(nameof(a.Face)),
                        value = a.Face,
                        displayType = OptionDisplayType.image
                    },
                    new OptionInfo
                    {
                        display =type.GetDescription(nameof(a.Name)),
                        value = a.Name
                    },
                    new OptionInfo
                    {
                        display =type.GetDescription(nameof(a.Tel)),
                        value = a.Tel
                    },
                    new OptionInfo
                    {
                        display = type.GetDescription(nameof(a.CreateTime)),
                        value = a.CreateTime
                    }
                }
            });

            var list = from a in Orm.Select<System_User>()
                            .Where(where_sql)
                            .GetPagination(pagination)
                            .ToList<System_User, List>(typeof(List).GetNamesWithTagAndOther(true, "_List"))
                       select @select.Invoke(a);

            return list.ToList();
        }

        public Detail GetDetail(string id)
        {
            var entity = Repository.GetAndCheckNull(id);

            var result = Mapper.Map<Detail>(entity);

            return result;
        }

        [AdministratorOnly]
        public void Create(Create data, bool runTransaction = true)
        {
            var newData = Mapper.Map<System_User>(data).InitEntity();

            if (Repository.Select.Where(c => c.Account == newData.Account).Any())
                throw new MessageException($"已存在账号为{newData.Account}的用户.");

            newData.Password = $"{newData.Account}{newData.Password}".ToMD5String();

            if (newData.Face.IsNullOrWhiteSpace())
                newData.Face = null;

            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_User),
                    DataId = newData.Id,
                    Explain = $"创建用户[账号 {newData.Account}, 姓名 {newData.Name}]."
                });

                Repository.Insert(newData);

                AuthoritiesBusiness.AutoAuthorizeRoleForUser(new Model.System.AuthorizeDTO.RoleForUser
                {
                    UserIds = new List<string> { newData.Id }
                }, false);
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("创建用户失败.", ex);
            }
            else
                handler();
        }

        [AdministratorOnly]
        public Edit GetEdit(string id)
        {
            var entity = Repository.GetAndCheckNull(id);

            var result = Mapper.Map<Edit>(entity);

            return result;
        }

        [AdministratorOnly]
        public void Edit(Edit data, bool runTransaction = true)
        {
            var editData = Mapper.Map<System_User>(data).ModifyEntity();

            var entity = Repository.GetAndCheckNull(editData.Id);

            var changed_ = string.Join(",",
                                       entity.GetPropertyValueChangeds<System_User, Edit>(editData)
                                            .Select(p => $"\r\n\t {p.Description}：{p.FormerValue} 更改为 {p.CurrentValue}"));
            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_User),
                    DataId = entity.Id,
                    Explain = $"修改用户[账号 {entity.Account}, 姓名 {entity.Name}].",
                    Remark = $"变更详情: \r\n\t{changed_}"
                });

                if (Repository.UpdateDiy
                      .SetSource(editData.ModifyEntity())
                      .UpdateColumns(typeof(Edit).GetNamesWithTagAndOther(false, "_Edit").ToArray())
                      .ExecuteAffrows() <= 0)
                    throw new MessageException("未修改任何数据.");
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("修改用户失败.", ex);
            }
            else
                handler();
        }

        [AdministratorOnly]
        public void Delete(List<string> ids)
        {
            if (Repository.Select.Where(c => ids.Contains(c.Id) && c.Account == Config.AdminAccount).Any())
                throw new MessageException($"禁止删除账号为{Config.AdminAccount}的用户.");

            var entityList = Repository.Select.Where(c => ids.Contains(c.Id)).ToList(c => new { c.Id, c.Account, c.Name });

            var orList = new List<Common_OperationRecord>();

            foreach (var entity in entityList)
            {
                orList.Add(new Common_OperationRecord
                {
                    DataType = nameof(System_User),
                    DataId = entity.Id,
                    Explain = $"删除用户[账号 {entity.Account}, 姓名 {entity.Name}]."
                });
            }

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                AuthoritiesBusiness.RevocationRoleForUser(ids, false);

                AuthoritiesBusiness.RevocationMenuForUser(ids, false);

                AuthoritiesBusiness.RevocationResourcesForUser(ids, false);

                AuthoritiesBusiness.RevocationCFUCForUser(ids, false);

                var orIds = OperationRecordBusiness.Create(orList);

                if (Repository.Delete(o => ids.Contains(o.Id)) <= 0)
                    throw new MessageException("未删除任何数据.");
            });

            if (!success)
                throw new MessageException("删除用户失败.", ex);
        }

        #endregion

        #region 拓展功能

        [AdministratorOnly]
        public void Enable(string id, bool enable)
        {
            var entity = Repository.GetAndCheckNull(id);

            if (entity.Account == Config.AdminAccount)
                throw new MessageException($"禁止操作账号为{Config.AdminAccount}的用户.");

            entity.Enable = enable;

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_User),
                    DataId = entity.Id,
                    Explain = $"{(enable ? "启用" : "禁用")}用户[账号 {entity.Account}, 姓名 {entity.Name}]."
                });

                if (Repository.Update(entity) <= 0)
                    throw new MessageException($"{(enable ? "启用" : "禁用")}用户失败");
            });

            if (!success)
                throw ex;
        }

        public void UpdatePassword(UpdatePassword data)
        {
            var editData = Mapper.Map<System_User>(data).ModifyEntity();

            var entity = Repository.GetAndCheckNull(editData.Id);

            editData.Password = $"{entity.Account}{data.OldPassword}".ToMD5String();

            if (!Operator.IsSuperAdmin && entity.Id != Operator.AuthenticationInfo.Id)
                throw new MessageException("您只能修改自己的密码.");

            if (!entity.Password.Equals(editData.Password))
                throw new MessageException("原密码有误.");

            editData.Password = $"{entity.Account}{data.NewPassword}".ToMD5String();

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_User),
                    DataId = entity.Id,
                    Explain = $"修改密码[账号 {entity.Account}, 姓名 {entity.Name}]."
                });

                if (Repository.UpdateDiy
                      .SetSource(editData)
                      .UpdateColumns(typeof(UpdatePassword).GetNamesWithTagAndOther(false, "_Edit").ToArray())
                      .ExecuteAffrows() <= 0)
                    throw new MessageException("修改密码失败");
            });

            if (!success)
                throw ex;
        }

        public void ResetPassword(string id, string newPassword)
        {
            var entity = Repository.GetAndCheckNull(id).ModifyEntity();

            entity.Password = $"{entity.Account}{newPassword}".ToMD5String();

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_User),
                    DataId = entity.Id,
                    Explain = $"重置密码[账号 {entity.Account}, 姓名 {entity.Name}]."
                });

                if (Repository.Update(entity) <= 0)
                    throw new MessageException("重置密码失败");
            });

            if (!success)
                throw ex;
        }

        public List<Claim> CreateClaims(AuthenticationInfo authenticationInfo, string authenticationMethod)
        {
            var claims = new List<Claim>
            {
                new Claim(nameof(AuthenticationInfo.Id), authenticationInfo.Id),

                new Claim(ClaimTypes.Name, authenticationInfo.Account),

                new Claim(nameof(AuthenticationInfo.UserType), authenticationInfo.UserType),

                new Claim(ClaimTypes.GivenName, authenticationInfo.Nickname ?? string.Empty),
                new Claim(ClaimTypes.Gender, authenticationInfo.Sex ?? string.Empty),

                new Claim(nameof(AuthenticationInfo.Face), authenticationInfo.Face ?? string.Empty),

                new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod)
            };

            claims.AddRange(authenticationInfo.RoleTypes.Select(o => new Claim(ClaimTypes.Role, o)));

            return claims;
        }

        public AuthenticationInfo AnalysisClaims(List<Claim> claims)
        {
            return new AuthenticationInfo()
            {
                Id = claims?.FirstOrDefault(o => o.Type == nameof(AuthenticationInfo.Id))?.Value,

                Account = claims?.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,

                UserType = claims?.FirstOrDefault(o => o.Type == nameof(AuthenticationInfo.UserType))?.Value,

                RoleTypes = claims?.Where(o => o.Type == ClaimTypes.Role).Select(o => o.Value).ToList(),

                Nickname = claims?.FirstOrDefault(o => o.Type == ClaimTypes.GivenName)?.Value,
                Sex = claims?.FirstOrDefault(o => o.Type == ClaimTypes.Gender)?.Value,

                Face = claims?.FirstOrDefault(o => o.Type == nameof(AuthenticationInfo.Face))?.Value,

                AuthenticationMethod = claims?.FirstOrDefault(o => o.Type == ClaimTypes.AuthenticationMethod)?.Value
            };
        }

        public AuthenticationInfo Login(string account, string password)
        {
            bool? again = null;
        again:
            if (again == false)
                throw new MessageException("登录失败.");

            var user = Repository.Where(o => o.Account == account)
                .ToOne(o => new { o.Id, o.Account, o.Name, o.Nickname, o.Sex, o.Face, o.Enable, o.Password });

            if (user == default)
            {
                if (again != null)
                {
                    again = false;
                    goto again;
                }

                if (account.Equals(Config.AdminAccount) && password.Equals(Config.AdminInitPassword) && Repository.Select.Count() == 0)
                {
                    InitAdmin();

                    //throw new MessageException("账号已初始化, 请重新登录.");
                    again = true;
                    goto again;
                }

                throw new MessageException("账号不存在或已被移除.");
            }

            if (!user.Enable)
                throw new MessageException("账号已被禁用.");

            if (!$"{account}{password}".ToMD5String().Equals(user.Password))
                throw new MessageException("密码错误.");

            EntryLogBusiness.Create(new Common_EntryLog
            {
                UserType = UserType.系统用户,
                Account = user.Account,
                Name = user.Name,
                Nickname = user.Nickname,
                Face = user.Face,
                IsAdmin = AuthoritiesBusiness.IsAdminUser(user.Id),
                Remark = "使用账号密码信息登录系统.",
                CreatorId = user.Id
            });

            return new AuthenticationInfo
            {
                Id = user.Id,
                UserType = UserType.系统用户,
                RoleTypes = AuthoritiesBusiness.GetUserRoleTypes(user.Id),
                Account = user.Account,
                Nickname = user.Nickname,
                Sex = user.Sex,
                Face = user.Face
            };
        }

        public AuthenticationInfo LoginWithLimitTimes(string account, string password)
        {
            var cacheKey = $"User-LoginWithLimitTimes-{account}";

            try
            {
                var result = Login(account, password);
                if (Cache.ContainsKey(cacheKey))
                    Cache.RemoveCache(cacheKey);
                return result;
            }
            catch (MessageException ex)
            {
                int times = 1;
                if (Cache.ContainsKey(cacheKey))
                {
                    times = (int)Cache.GetCache(cacheKey);
                    if (times >= Config.LoginFailedTimesLimit)
                    {
                        if (Repository.UpdateDiy.Where(o => o.Account == account).Set(o => o.Enable, false).ExecuteAffrows() < 0)
                            throw new MessageException("系统繁忙.", ex);

                        throw new MessageException("登录失败, 账号已禁用, 请联系管理员.", ex);
                    }
                    times++;
                }

                Cache.SetCache(cacheKey, times, TimeSpan.FromMinutes(30), ExpireType.Relative);

                throw new MessageException($"登录失败, 您还有{Config.LoginFailedTimesLimit - times}次尝试的机会, 全部失败后账号将被禁用.", ex);
            }
        }

        public AuthenticationInfo WeChatLogin(string appId, string openId)
        {
            var user = Repository.Where(o => o.WeChatUserInfos.AsSelect().Where(p => p.AppId == appId && p.OpenId == openId).Any())
                .ToOne(o => new { o.Id, o.Account, o.Name, o.Nickname, o.Sex, o.Face, o.Enable, o.Password });

            if (user == default)
                throw new MessageException("账号还未绑定微信.");

            if (!user.Enable)
                throw new MessageException("账号已被禁用.");

            EntryLogBusiness.Create(new Common_EntryLog
            {
                UserType = UserType.系统用户,
                Account = user.Account,
                Name = user.Name,
                Nickname = user.Nickname,
                Face = user.Face,
                IsAdmin = AuthoritiesBusiness.IsAdminUser(user.Id),
                Remark = "使用微信信息登录系统.",
                CreatorId = user.Id
            });

            return new AuthenticationInfo
            {
                Id = user.Id,
                UserType = UserType.系统用户,
                RoleTypes = AuthoritiesBusiness.GetUserRoleTypes(user.Id),
                Account = user.Account,
                Nickname = user.Name,
                Sex = user.Sex,
                Face = user.Face
            };
        }

        public OperatorUserInfo GetOperatorDetail(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            return Repository.Where(o => o.Id == id)
                .ToOne(o => new OperatorUserInfo
                {
                    Account = o.Account,
                    Name = o.Name,
                    Tel = o.Tel
                });
        }

        #endregion

        #endregion
    }
}
