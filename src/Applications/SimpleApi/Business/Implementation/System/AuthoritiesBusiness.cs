﻿using AutoMapper;
using Business.Interface.Common;
using Business.Interface.System;
using Business.Utils;
using Business.Utils.Pagination;
using Entity.Common;
using Entity.Public;
using Entity.System;
using FreeSql;
using Microservice.Library.DataMapping.Gen;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using Model.System;
using Model.System.AuthorizeDTO;
using Model.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Implementation.System
{
    /// <summary>
    /// 权限业务类
    /// </summary>
    public class AuthoritiesBusiness : BaseBusiness, IAuthoritiesBusiness
    {
        #region DI

        public AuthoritiesBusiness(
            IFreeSqlProvider freeSqlProvider,
            IAutoMapperProvider autoMapperProvider,
            IOperationRecordBusiness operationRecordBusiness)
        {
            Orm = freeSqlProvider.GetFreeSql();
            Repository_User = Orm.GetRepository<System_User, string>();
            Repository_UserRole = Orm.GetRepository<System_UserRole, string>();
            Repository_UserMenu = Orm.GetRepository<System_UserMenu, string>();
            Repository_UserResources = Orm.GetRepository<System_UserResources, string>();
            Repository_Member = Orm.GetRepository<Public_Member, string>();
            Repository_MemberRole = Orm.GetRepository<Public_MemberRole, string>();
            Repository_Role = Orm.GetRepository<System_Role, string>();
            Repository_RoleMenu = Orm.GetRepository<System_RoleMenu, string>();
            Repository_RoleResources = Orm.GetRepository<System_RoleResources, string>();
            Repository_Menu = Orm.GetRepository<System_Menu, string>();
            Repository_Resources = Orm.GetRepository<System_Resources, string>();
            Repository_FileUploadConfig = Orm.GetRepository<Common_FileUploadConfig, string>();
            Repository_UserCFUC = Orm.GetRepository<System_UserCFUC, string>();
            Repository_RoleCFUC = Orm.GetRepository<System_RoleCFUC, string>();
            Mapper = autoMapperProvider.GetMapper();
            OperationRecordBusiness = operationRecordBusiness;
        }

        #endregion

        #region 私有成员

        readonly IFreeSql Orm;

        readonly IBaseRepository<System_User, string> Repository_User;

        readonly IBaseRepository<System_UserRole, string> Repository_UserRole;

        readonly IBaseRepository<System_UserMenu, string> Repository_UserMenu;

        readonly IBaseRepository<System_UserResources, string> Repository_UserResources;

        readonly IBaseRepository<Public_Member, string> Repository_Member;

        readonly IBaseRepository<Public_MemberRole, string> Repository_MemberRole;

        readonly IBaseRepository<System_Role, string> Repository_Role;

        readonly IBaseRepository<System_RoleMenu, string> Repository_RoleMenu;

        readonly IBaseRepository<System_RoleResources, string> Repository_RoleResources;

        readonly IBaseRepository<System_Menu, string> Repository_Menu;

        readonly IBaseRepository<System_Resources, string> Repository_Resources;

        readonly IBaseRepository<Common_FileUploadConfig, string> Repository_FileUploadConfig;

        readonly IBaseRepository<System_UserCFUC, string> Repository_UserCFUC;

        readonly IBaseRepository<System_RoleCFUC, string> Repository_RoleCFUC;

        readonly IMapper Mapper;

        readonly IOperationRecordBusiness OperationRecordBusiness;

        private dynamic GetUserWithCheck(string userId)
        {
            var user = Repository_User.Where(o => o.Id == userId).ToOne(o => new
            {
                o.Id,
                o.Enable,
                o.Account,
                o.Name
            });

            if (user == default)
                throw new MessageException("用户不存在或已被移除.");

            if (!user.Enable)
                throw new MessageException("用户账号已禁用.");

            if (user.Account == Config.AdminAccount && Repository_UserRole.Where(o => o.UserId == userId && o.Role.Type == $"{RoleType.超级管理员}").Any())//Repository_UserRole.Where(o => o.UserId == userId && o.Role.Type == $"{RoleType.超级管理员}").Any()
                throw new MessageException($"{RoleType.超级管理员}无需进行相关授权操作.");

            return user;
        }

        private dynamic GetMemberWithCheck(string memberId)
        {
            var user = Repository_Member.Where(o => o.Id == memberId).ToOne(o => new
            {
                o.Id,
                o.Enable,
                o.Account,
                o.Nickname,
                o.Name
            });

            if (user == default)
                throw new MessageException("会员不存在或已被移除.");

            if (!user.Enable)
                throw new MessageException("会员账号已禁用.");

            return user;
        }

        private dynamic GetRoleWithCheck(string roleId)
        {
            var role = Repository_Role.Where(o => o.Id == roleId).ToOne(o => new
            {
                o.Id,
                o.Enable,
                o.Type,
                o.Name
            });

            if (role == default)
                throw new MessageException("角色不存在或已被移除.");

            if (role.Type == RoleType.超级管理员)
                throw new MessageException($"{RoleType.超级管理员}角色无需进行相关授权操作.");

            if (!role.Enable)
                throw new MessageException("角色账号已禁用.");

            return role;
        }

        #endregion

        #region 外部接口

        #region 授权

        public void AutoAuthorizeRoleForUser(RoleForUser data, bool runTransaction = true)
        {
            if (data.UserIds.Any_Ex())
            {
                var roleIds = Repository_Role.Where(o => o.AutoAuthorizeRoleForUser == true
                                                        && o.Enable == true
                                                        && o.Type != $"{RoleType.超级管理员}"
                                                        && o.Type != $"{RoleType.会员}")
                                            .ToList(o => o.Id);

                if (roleIds.Any())
                    AuthorizeRoleForUser(new RoleForUser
                    {
                        UserIds = data.UserIds,
                        RoleIds = roleIds
                    }, runTransaction);
            }
            else if (data.RoleIds.Any_Ex())
            {
                var userIds = Repository_User.Select.Where(o => !o.Roles.AsSelect()
                                                            .Where(p => data.RoleIds.Contains(p.Id)
                                                                        || p.Type == $"{RoleType.超级管理员}")
                                                            .Any())
                                                    .ToList(o => o.Id);
                if (userIds.Any())
                    AuthorizeRoleForUser(new RoleForUser
                    {
                        UserIds = userIds,
                        RoleIds = data.RoleIds
                    }, runTransaction);
            }
            else
                throw new MessageException("参数不可为空.");
        }

        public void AutoAuthorizeRoleForMember(RoleForMember data, bool runTransaction = true)
        {
            if (data.MemberIds.Any_Ex())
            {
                var roleIds = Repository_Role.Where(o => o.AutoAuthorizeRoleForUser == true
                                                        && o.Enable == true
                                                        && o.Type == $"{RoleType.会员}")
                                            .ToList(o => o.Id);
                if (roleIds.Any())
                    AuthorizeRoleForMember(new RoleForMember
                    {
                        MemberIds = data.MemberIds,
                        RoleIds = roleIds
                    }, runTransaction);
            }
            else if (data.RoleIds.Any_Ex())
            {
                var memberIds = Repository_Member.Select.Where(o => !o.Roles.AsSelect()
                                                            .Where(p => data.RoleIds.Contains(p.Id))
                                                            .Any())
                                                        .ToList(o => o.Id);
                if (memberIds.Any())
                    AuthorizeRoleForMember(new RoleForMember
                    {
                        MemberIds = memberIds,
                        RoleIds = data.RoleIds
                    }, runTransaction);
            }
            else
                throw new MessageException("参数不可为空.");
        }

        public void AuthorizeRoleForUser(RoleForUser data, bool runTransaction = true)
        {
            if (!data.UserIds.Any_Ex())
                return;

            var users = data.UserIds.Select(o => GetUserWithCheck(o));

            if (!users.Any())
                return;

            void handler()
            {
                var newData = new List<System_UserRole>();

                foreach (var user in users)
                {
                    string userId = user.Id;
                    var roles = Repository_Role
                                .Where(o => (data.All == true || data.RoleIds.Contains(o.Id))
                                    && o.Enable == true
                                    && !o.Users.AsSelect().Where(p => p.Id == userId).Any())
                                .ToList(o => new
                                {
                                    o.Id,
                                    o.Type,
                                    o.Name
                                });

                    if (!roles.Any())
                        continue;

                    newData.AddRange(roles.Select(o => new System_UserRole
                    {
                        RoleId = o.Id,
                        UserId = userId
                    }));

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(System_UserRole),
                        DataId = $"{userId}+{string.Join(",", roles.Select(o => o.Id))}",
                        Explain = $"授权角色给用户.",
                        Remark = $"被授权的用户: \r\n\t[账号 {user.Account}, 姓名 {user.Name}]\r\n" +
                                 $"授权的角色: \r\n\t{string.Join(",", roles.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                    });
                }

                if (newData.Any())
                    Repository_UserRole.Insert(newData);

                if (data.RevocationOtherRoles)
                    data.UserIds.ForEach(o =>
                    {
                        RevocationRoleForUser(new RoleForUser
                        {
                            UserIds = new List<string> { o },
                            RoleIds = Repository_UserRole.Where(p => p.UserId == o && !data.RoleIds.Contains(p.RoleId)).ToList(p => p.RoleId)
                        }, false);
                    });
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("授权失败.", ex);
            }
            else
                handler();
        }

        public void AuthorizeRoleForUser(string roleId, string userId, bool allChilds = false, bool runTransaction = true)
        {
            List<string> roleIds = new List<string>();
            var q_Up = Repository_Role.Select.Where($"a.[Id] = '{roleId}' AND a.[Enable] = 1")
                                        .AsTreeCte(null, true)
                                        .Where($"a.[Enable] = 1");
            roleIds.AddRange(q_Up.ToList(o => o.Id));

            roleIds.Remove(roleId);

            var q_Down = Repository_Role.Select.Where($"a.[Id] = '{roleId}' AND a.[Enable] = 1");

            if (allChilds)
                q_Down = q_Down.AsTreeCte()
                             .Where($"a.[Enable] = 1");

            roleIds.AddRange(q_Down.Where($"a.[Enable] = 1").ToList(o => o.Id));

            if (roleIds.Any())
                AuthorizeRoleForUser(new RoleForUser
                {
                    UserIds = new List<string> { userId },
                    RoleIds = roleIds
                }, runTransaction);
        }

        public void AuthorizeRoleForMember(RoleForMember data, bool runTransaction = true)
        {
            if (!data.MemberIds.Any_Ex() || !data.RoleIds.Any_Ex())
                return;

            if (Repository_Role.Where(o => data.RoleIds.Contains(o.Id) && o.Type != $"{RoleType.会员}").Any())
                throw new MessageException($"只允许将类型为 {RoleType.会员}的角色授权给会员.");

            var members = data.MemberIds.Select(o => GetMemberWithCheck(o));

            if (!members.Any())
                return;

            void handler()
            {
                var newData = new List<Public_MemberRole>();

                foreach (var member in members)
                {
                    string memberId = member.Id;
                    var roles = Repository_Role
                                .Where(o => data.RoleIds.Contains(o.Id)
                                    && o.Enable == true
                                    && !o.Members.AsSelect().Where(p => p.Id == memberId).Any())
                                .ToList(o => new
                                {
                                    o.Id,
                                    o.Type,
                                    o.Name
                                });

                    if (!roles.Any())
                        continue;

                    newData.AddRange(roles.Select(o => new Public_MemberRole
                    {
                        RoleId = o.Id,
                        MemberId = memberId
                    }));

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(Public_MemberRole),
                        DataId = $"{memberId}+{string.Join(",", roles.Select(o => o.Id))}",
                        Explain = $"授权角色给会员.",
                        Remark = $"被授权的会员: \r\n\t[账号 {member.Account}, 昵称 {member.Nickname}]\r\n" +
                                $"授权的角色: \r\n\t{string.Join(",", roles.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                    });
                }

                if (newData.Any())
                    Repository_MemberRole.Insert(newData);

                if (data.RevocationOtherRoles)
                    data.MemberIds.ForEach(o =>
                    {
                        RevocationRoleForMember(new RoleForMember
                        {
                            MemberIds = new List<string> { o },
                            RoleIds = Repository_MemberRole.Where(p => p.MemberId == o && !data.RoleIds.Contains(p.RoleId)).ToList(p => p.RoleId)
                        }, false);
                    });
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("授权失败.", ex);
            }
            else
                handler();
        }

        public void AuthorizeRoleForMember(string roleId, string memberId, bool allChilds = false, bool runTransaction = true)
        {
            List<string> roleIds = new List<string>();
            var q_Up = Repository_Role.Select.Where($"a.[Id] = '{roleId}' AND a.[Enable] = 1")
                                        .AsTreeCte(null, true)
                                        .Where($"a.[Enable] = 1");
            roleIds.AddRange(q_Up.ToList(o => o.Id));

            roleIds.Remove(roleId);

            var q = Repository_Role.Select.Where($"a.[Id] = '{roleId}' AND a.[Enable] = 1");

            if (allChilds)
                q = q.AsTreeCte();

            roleIds = q.Where($"a.[Enable] = 1").ToList(o => o.Id);

            if (roleIds.Any())
                AuthorizeRoleForMember(new RoleForMember
                {
                    MemberIds = new List<string> { memberId },
                    RoleIds = roleIds
                }, runTransaction);
        }

        public void AuthorizeMenuForUser(MenuForUser data, bool runTransaction = true)
        {
            if (!data.UserIds.Any_Ex())
                return;

            var users = data.UserIds.Select(o => GetUserWithCheck(o));

            if (!users.Any())
                return;

            void handler()
            {
                var newData = new List<System_UserMenu>();

                foreach (var user in users)
                {
                    string userId = user.Id;
                    var menus = Repository_Menu
                                .Where(o => (data.All == true || data.MenuIds.Contains(o.Id))
                                    && o.Enable == true
                                    && !o.Users.AsSelect().Where(p => p.Id == userId).Any())
                                .ToList(o => new
                                {
                                    o.Id,
                                    o.Type,
                                    o.Name
                                });

                    if (!menus.Any())
                        continue;

                    newData.AddRange(menus.Select(o => new System_UserMenu
                    {
                        MenuId = o.Id,
                        UserId = userId
                    }));

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(System_UserMenu),
                        DataId = $"{userId}+{string.Join(",", menus.Select(o => o.Id))}",
                        Explain = $"授权菜单给用户.",
                        Remark = $"被授权的用户: \r\n\t[账号 {user.Account}, 姓名 {user.Name}]\r\n" +
                                $"授权的菜单: \r\n\t{string.Join(",", menus.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                    });
                }

                if (newData.Any())
                    Repository_UserMenu.Insert(newData);
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("授权失败.", ex);
            }
            else
                handler();
        }

        public void AuthorizeMenuForUser(string menuId, string userId, bool allChilds = false, bool runTransaction = true)
        {
            List<string> menuIds = new List<string>();
            var q_Up = Repository_Menu.Select.Where($"a.[Id] = '{menuId}' AND a.[Enable] = 1")
                                        .AsTreeCte(null, true)
                                        .Where($"a.[Enable] = 1");
            menuIds.AddRange(q_Up.ToList(o => o.Id));

            menuIds.Remove(menuId);

            var q = Repository_Menu.Select.Where($"a.[Id] = '{menuId}' AND a.[Enable] = 1");

            if (allChilds)
                q = q.AsTreeCte();

            menuIds = q.Where($"a.[Enable] = 1").ToList(o => o.Id);

            if (menuIds.Any())
                AuthorizeMenuForUser(new MenuForUser
                {
                    UserIds = new List<string> { userId },
                    MenuIds = menuIds
                }, runTransaction);
        }

        public void AuthorizeResourcesForUser(ResourcesForUser data)
        {
            if (!data.UserIds.Any_Ex())
                return;

            var users = data.UserIds.Select(o => GetUserWithCheck(o));

            if (!users.Any())
                return;

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var newData = new List<System_UserResources>();

                foreach (var user in users)
                {
                    string userId = user.Id;
                    var resources = Repository_Resources
                                .Where(o => (data.All == true || data.ResourcesIds.Contains(o.Id))
                                    && o.Enable == true
                                    && !o.Users.AsSelect().Where(p => p.Id == userId).Any())
                                .ToList(o => new
                                {
                                    o.Id,
                                    o.Type,
                                    o.Name
                                });

                    if (!resources.Any())
                        continue;

                    newData.AddRange(resources.Select(o => new System_UserResources
                    {
                        ResourcesId = o.Id,
                        UserId = userId
                    }));

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(System_UserResources),
                        DataId = $"{userId}+{string.Join(",", resources.Select(o => o.Id))}",
                        Explain = $"授权资源给用户.",
                        Remark = $"被授权的用户: \r\n\t[账号 {user.Account}, 姓名 {user.Name}]\r\n" +
                                $"授权的资源: \r\n\t{string.Join(",", resources.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                    });
                }

                if (newData.Any())
                    Repository_UserResources.Insert(newData);
            });

            if (!success)
                throw new MessageException("授权失败.", ex);
        }

        public void AuthorizeMenuForRole(MenuForRole data, bool runTransaction = true)
        {
            if (!data.RoleIds.Any_Ex())
                return;

            var roles = data.RoleIds.Select(o => GetRoleWithCheck(o));

            if (!roles.Any())
                return;

            void handler()
            {
                var newData = new List<System_RoleMenu>();

                foreach (var role in roles)
                {
                    string roleId = role.Id;
                    var menus = Repository_Menu
                                .Where(o => (data.All == true || data.MenuIds.Contains(o.Id))
                                    && o.Enable == true
                                    && !o.Roles.AsSelect().Where(p => p.Id == roleId).Any())
                                .ToList(o => new
                                {
                                    o.Id,
                                    o.Type,
                                    o.Name
                                });

                    if (!menus.Any())
                        continue;

                    newData.AddRange(menus.Select(o => new System_RoleMenu
                    {
                        MenuId = o.Id,
                        RoleId = roleId
                    }));

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(System_RoleMenu),
                        DataId = $"{roleId}+{string.Join(",", menus.Select(o => o.Id))}",
                        Explain = $"授权菜单给角色.",
                        Remark = $"被授权的角色: \r\n\t[名称 {role.Name}, 类型 {role.Type}]\r\n" +
                                $"授权的菜单: \r\n\t{string.Join(",", menus.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                    });
                }

                if (newData.Any())
                    Repository_RoleMenu.Insert(newData);
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("授权失败.", ex);
            }
            else
                handler();
        }

        public void AuthorizeMenuForRole(string menuId, string roleId, bool allChilds = false, bool runTransaction = true)
        {
            List<string> menuIds = new List<string>();
            var q_Up = Repository_Menu.Select.Where($"a.[Id] = '{menuId}' AND a.[Enable] = 1")
                                        .AsTreeCte(null, true)
                                        .Where($"a.[Enable] = 1");
            menuIds.AddRange(q_Up.ToList(o => o.Id));

            menuIds.Remove(menuId);

            var q = Repository_Menu.Select.Where($"a.[Id] = '{menuId}' AND a.[Enable] = 1");

            if (allChilds)
                q = q.AsTreeCte();

            menuIds = q.Where($"a.[Enable] = 1").ToList(o => o.Id);

            if (menuIds.Any())
                AuthorizeMenuForRole(new MenuForRole
                {
                    RoleIds = new List<string> { roleId },
                    MenuIds = menuIds
                }, runTransaction);
        }

        public void AuthorizeResourcesForRole(ResourcesForRole data)
        {
            if (!data.RoleIds.Any_Ex())
                return;

            var roles = data.RoleIds.Select(o => GetRoleWithCheck(o));

            if (!roles.Any())
                return;

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var newData = new List<System_RoleResources>();

                foreach (var role in roles)
                {
                    string roleId = role.Id;
                    var menus = Repository_Resources
                                .Where(o => (data.All == true || data.ResourcesIds.Contains(o.Id))
                                    && o.Enable == true
                                    && !o.Roles.AsSelect().Where(p => p.Id == roleId).Any())
                                .ToList(o => new
                                {
                                    o.Id,
                                    o.Type,
                                    o.Name
                                });

                    if (!menus.Any())
                        continue;

                    newData.AddRange(menus.Select(o => new System_RoleResources
                    {
                        ResourcesId = o.Id,
                        RoleId = roleId
                    }));

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(System_RoleResources),
                        DataId = $"{roleId}+{string.Join(",", menus.Select(o => o.Id))}",
                        Explain = $"授权资源给角色.",
                        Remark = $"被授权的角色: \r\n\t[名称 {role.Name}, 类型 {role.Type}]\r\n" +
                                $"授权的资源: \r\n\t{string.Join(",", menus.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                    });
                }

                if (newData.Any())
                    Repository_RoleResources.Insert(newData);
            });

            if (!success)
                throw new MessageException("授权失败.", ex);
        }

        public void AuthorizeCFUCForUser(CFUCForUser data, bool runTransaction = true)
        {
            if (!data.UserIds.Any_Ex())
                return;

            var users = data.UserIds.Select(o => GetUserWithCheck(o));

            if (!users.Any())
                return;

            void handler()
            {
                var newData = new List<System_UserCFUC>();

                foreach (var user in users)
                {
                    string userId = user.Id;
                    var configs = Repository_FileUploadConfig
                                .Where(o => (data.All == true || data.ConfigIds.Contains(o.Id))
                                    && o.Enable == true
                                    && !o.System_Users.AsSelect().Where(p => p.Id == userId).Any())
                                .ToList(o => new
                                {
                                    o.Id,
                                    o.Code,
                                    o.Name
                                });

                    if (!configs.Any())
                        continue;

                    newData.AddRange(configs.Select(o => new System_UserCFUC
                    {
                        ConfigId = o.Id,
                        UserId = userId
                    }));

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(System_UserCFUC),
                        DataId = $"{userId}+{string.Join(",", configs.Select(o => o.Id))}",
                        Explain = $"授权文件上传配置给用户.",
                        Remark = $"被授权的用户: \r\n\t[账号 {user.Account}, 姓名 {user.Name}]\r\n" +
                                $"授权的文件上传配置: \r\n\t{string.Join(",", configs.Select(o => $"[名称 {o.Name}, 编码 {o.Code}]"))}"
                    });
                }

                if (newData.Any())
                    Repository_UserCFUC.Insert(newData);
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("授权失败.", ex);
            }
            else
                handler();
        }

        public void AuthorizeCFUCForUser(string configId, string userId, bool allChilds = false, bool runTransaction = true)
        {
            List<string> configIds = new List<string>();
            var q_Up = Repository_FileUploadConfig.Select.Where($"a.[Id] = '{configId}' AND a.[Enable] = 1")
                                        .AsTreeCte(null, true)
                                        .Where($"a.[Enable] = 1");
            configIds.AddRange(q_Up.ToList(o => o.Id));

            configIds.Remove(configId);

            var q = Repository_FileUploadConfig.Select.Where($"a.[Id] = '{configId}' AND a.[Enable] = 1");

            if (allChilds)
                q = q.AsTreeCte();

            configIds = q.Where($"a.[Enable] = 1").ToList(o => o.Id);

            if (configIds.Any())
                AuthorizeCFUCForUser(new CFUCForUser
                {
                    UserIds = new List<string> { userId },
                    ConfigIds = configIds
                }, runTransaction);
        }

        public void AuthorizeCFUCForRole(CFUCForRole data, bool runTransaction = true)
        {
            if (!data.RoleIds.Any_Ex())
                return;

            var roles = data.RoleIds.Select(o => GetRoleWithCheck(o));

            if (!roles.Any())
                return;

            void handler()
            {
                var newData = new List<System_RoleCFUC>();

                foreach (var role in roles)
                {
                    string roleId = role.Id;
                    var configs = Repository_FileUploadConfig
                                .Where(o => (data.All == true || data.ConfigIds.Contains(o.Id))
                                    && o.Enable == true
                                    && !o.System_Roles.AsSelect().Where(p => p.Id == roleId).Any())
                                .ToList(o => new
                                {
                                    o.Id,
                                    o.Code,
                                    o.Name
                                });

                    if (!configs.Any())
                        continue;

                    newData.AddRange(configs.Select(o => new System_RoleCFUC
                    {
                        ConfigId = o.Id,
                        RoleId = roleId
                    }));

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(System_RoleCFUC),
                        DataId = $"{roleId}+{string.Join(",", configs.Select(o => o.Id))}",
                        Explain = $"授权文件上传配置给角色.",
                        Remark = $"被授权的角色: \r\n\t[名称 {role.Name}, 类型 {role.Type}]\r\n" +
                                $"授权的文件上传配置: \r\n\t{string.Join(",", configs.Select(o => $"[名称 {o.Name}, 编码 {o.Code}]"))}"
                    });
                }

                if (newData.Any())
                    Repository_RoleCFUC.Insert(newData);
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("授权失败.", ex);
            }
            else
                handler();
        }

        public void AuthorizeCFUCForRole(string configId, string roleId, bool allChilds = false, bool runTransaction = true)
        {
            List<string> configIds = new List<string>();
            var q_Up = Repository_FileUploadConfig.Select.Where($"a.[Id] = '{configId}' AND a.[Enable] = 1")
                                        .AsTreeCte(null, true)
                                        .Where($"a.[Enable] = 1");
            configIds.AddRange(q_Up.ToList(o => o.Id));

            configIds.Remove(configId);

            var q = Repository_FileUploadConfig.Select.Where($"a.[Id] = '{configId}' AND a.[Enable] = 1");

            if (allChilds)
                q = q.AsTreeCte();

            configIds = q.Where($"a.[Enable] = 1").ToList(o => o.Id);

            if (configIds.Any())
                AuthorizeCFUCForRole(new CFUCForRole
                {
                    RoleIds = new List<string> { roleId },
                    ConfigIds = configIds
                }, runTransaction);
        }

        #endregion

        #region 撤销授权

        public void RevocationRoleForAllUser(List<string> roleIds, bool runTransaction = true)
        {
            var userIds = Repository_User.Where(o => o.Roles.AsSelect().Where(p => roleIds.Contains(p.Id)).Any()).ToList(o => o.Id);

            RevocationRoleForUser(new RoleForUser
            {
                UserIds = userIds,
                RoleIds = roleIds
            }, runTransaction);
        }

        public void RevocationRoleForAllMember(List<string> roleIds, bool runTransaction = true)
        {
            var memberIds = Repository_Member.Where(o => o.Roles.AsSelect().Where(p => roleIds.Contains(p.Id)).Any()).ToList(o => o.Id);

            RevocationRoleForMember(new RoleForMember
            {
                MemberIds = memberIds,
                RoleIds = roleIds
            }, runTransaction);
        }

        public void RevocationRoleForUser(List<string> userIds, bool runTransaction = true)
        {
            RevocationRoleForUser(new RoleForUser
            {
                UserIds = userIds,
                All = true
            }, runTransaction);
        }

        public void RevocationRoleForMember(List<string> memberIds, bool runTransaction = true)
        {
            RevocationRoleForMember(new RoleForMember
            {
                MemberIds = memberIds,
                All = true
            }, runTransaction);
        }

        public void RevocationMenuForUser(List<string> userIds, bool runTransaction = true)
        {
            RevocationMenuForUser(new MenuForUser
            {
                UserIds = userIds,
                All = true
            }, runTransaction);
        }

        public void RevocationResourcesForUser(List<string> userIds, bool runTransaction = true)
        {
            RevocationResourcesForUser(new ResourcesForUser
            {
                UserIds = userIds,
                All = true
            }, runTransaction);
        }

        public void RevocationCFUCForUser(List<string> userIds, bool runTransaction = true)
        {
            RevocationCFUCForUser(new CFUCForUser
            {
                UserIds = userIds,
                All = true
            }, runTransaction);
        }

        public void RevocationMenuForRole(List<string> roleIds, bool runTransaction = true)
        {
            RevocationMenuForRole(new MenuForRole
            {
                RoleIds = roleIds,
                All = true
            }, runTransaction);
        }

        public void RevocationResourcesForRole(List<string> roleIds, bool runTransaction = true)
        {
            RevocationResourcesForRole(new ResourcesForRole
            {
                RoleIds = roleIds,
                All = true
            }, runTransaction);
        }

        public void RevocationCFUCForRole(List<string> roleIds, bool runTransaction = true)
        {
            RevocationCFUCForRole(new CFUCForRole
            {
                RoleIds = roleIds,
                All = true
            }, runTransaction);
        }

        public void RevocationRoleForUser(RoleForUser data, bool runTransaction = true)
        {
            if (!data.UserIds.Any_Ex())
                return;

            var users = data.UserIds.Select(o => GetUserWithCheck(o));

            if (!users.Any())
                return;

            void handler()
            {
                foreach (var user in users)
                {
                    string userId = user.Id;
                    var roles = Repository_Role
                                .Where(o => (data.All == true || data.RoleIds.Contains(o.Id))
                                    && o.Enable == true
                                    && o.Users.AsSelect().Where(p => p.Id == userId).Any())
                                .ToList(o => new
                                {
                                    o.Id,
                                    o.Type,
                                    o.Name
                                });

                    if (!roles.Any())
                        continue;

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(System_UserRole),
                        DataId = $"{userId}+{string.Join(",", roles.Select(o => o.Id))}",
                        Explain = $"撤销用户的角色授权.",
                        Remark = $"被撤销授权的用户: \r\n\t[账号 {user.Account}, 姓名 {user.Name}]\r\n" +
                                 $"撤销授权的角色: \r\n\t{string.Join(",", roles.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                    });

                    var deleteIds = roles.Select(o => o.Id).ToList();

                    if (Repository_UserRole.Delete(o => o.UserId == userId && deleteIds.Contains(o.RoleId)) < 0)
                        throw new MessageException("撤销授权失败.");
                }
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("撤销授权失败.", ex);
            }
            else
                handler();
        }

        public void RevocationRoleForUser(string roleId, string userId, bool allChilds = false, bool runTransaction = true)
        {
            List<string> roleIds;
            var q = Repository_Role.Select.Where($"a.[Id] = '{roleId}'");

            if (allChilds)
                q = q.AsTreeCte();

            roleIds = q.ToList(o => o.Id);

            if (roleIds.Any())
                RevocationRoleForUser(new RoleForUser
                {
                    UserIds = new List<string> { userId },
                    RoleIds = roleIds
                }, runTransaction);
        }

        public void RevocationRoleForMember(RoleForMember data, bool runTransaction = true)
        {
            if (!data.MemberIds.Any_Ex())
                return;

            var members = data.MemberIds.Select(o => GetMemberWithCheck(o));

            if (!members.Any())
                return;

            void handler()
            {
                foreach (var member in members)
                {
                    string memberId = member.Id;
                    var roles = Repository_Role
                                .Where(o => (data.All == true || data.RoleIds.Contains(o.Id))
                                    && o.Enable == true
                                    && o.Members.AsSelect().Where(p => p.Id == memberId).Any())
                                .ToList(o => new
                                {
                                    o.Id,
                                    o.Type,
                                    o.Name
                                });

                    if (!roles.Any())
                        continue;

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(Public_MemberRole),
                        DataId = $"{memberId}+{string.Join(",", roles.Select(o => o.Id))}",
                        Explain = $"撤销会员的角色授权.",
                        Remark = $"被撤销授权的会员: \r\n\t[账号 {member.Account}, 昵称 {member.Nickname}]\r\n" +
                                 $"撤销授权的角色: \r\n\t{string.Join(",", roles.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                    });

                    var deleteIds = roles.Select(o => o.Id).ToList();

                    if (Repository_MemberRole.Delete(o => o.MemberId == memberId && deleteIds.Contains(o.RoleId)) < 0)
                        throw new MessageException("撤销授权失败.");
                }
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("撤销授权失败.", ex);
            }
            else
                handler();
        }

        public void RevocationRoleForMember(string roleId, string memberId, bool allChilds = false, bool runTransaction = true)
        {
            List<string> roleIds;
            var q = Repository_Role.Select.Where($"a.[Id] = '{roleId}'");

            if (allChilds)
                q = q.AsTreeCte();

            roleIds = q.ToList(o => o.Id);

            if (roleIds.Any())
                RevocationRoleForMember(new RoleForMember
                {
                    MemberIds = new List<string> { memberId },
                    RoleIds = roleIds
                }, runTransaction);
        }

        public void RevocationMenuForUser(MenuForUser data, bool runTransaction = true)
        {
            if (!data.UserIds.Any_Ex())
                return;

            var users = data.UserIds.Select(o => GetUserWithCheck(o));

            if (!users.Any())
                return;

            void handler()
            {
                foreach (var user in users)
                {
                    string userId = user.Id;
                    var menus = Repository_Menu
                                .Where(o => (data.All == true || data.MenuIds.Contains(o.Id))
                                    && o.Users.AsSelect().Where(p => p.Id == userId).Any())
                                .ToList(o => new
                                {
                                    o.Id,
                                    o.Type,
                                    o.Name
                                });

                    if (!menus.Any())
                        continue;

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(System_UserMenu),
                        DataId = $"{userId}+{string.Join(",", menus.Select(o => o.Id))}",
                        Explain = $"撤销用户的菜单授权.",
                        Remark = $"被撤销授权的用户: \r\n\t[账号 {user.Account}, 姓名 {user.Name}]\r\n" +
                                 $"撤销授权的菜单: \r\n\t{string.Join(",", menus.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                    });

                    var deleteIds = menus.Select(o => o.Id).ToList();

                    if (Repository_UserMenu.Delete(o => o.UserId == userId && deleteIds.Contains(o.MenuId)) < 0)
                        throw new MessageException("撤销授权失败.");
                }
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("撤销授权失败.", ex);
            }
            else
                handler();
        }

        public void RevocationMenuForUser(string menuId, string userId, bool allChilds = false, bool runTransaction = true)
        {
            List<string> menuIds;
            var q = Repository_Menu.Select.Where($"a.[Id] = '{menuId}'");

            if (allChilds)
                q = q.AsTreeCte();

            menuIds = q.ToList(o => o.Id);

            if (menuIds.Any())
                RevocationMenuForUser(new MenuForUser
                {
                    UserIds = new List<string> { userId },
                    MenuIds = menuIds
                }, runTransaction);
        }

        public void RevocationResourcesForUser(ResourcesForUser data, bool runTransaction = true)
        {
            if (!data.UserIds.Any_Ex())
                return;

            var users = data.UserIds.Select(o => GetUserWithCheck(o));

            if (!users.Any())
                return;

            void handler()
            {
                foreach (var user in users)
                {
                    string userId = user.Id;
                    var resourcess = Repository_Resources
                                .Where(o => (data.All == true || data.ResourcesIds.Contains(o.Id))
                                    && o.Users.AsSelect().Where(p => p.Id == userId).Any())
                                .ToList(o => new
                                {
                                    o.Id,
                                    o.Type,
                                    o.Name
                                });

                    if (!resourcess.Any())
                        continue;

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(System_UserResources),
                        DataId = $"{userId}+{string.Join(",", resourcess.Select(o => o.Id))}",
                        Explain = $"撤销用户的资源授权.",
                        Remark = $"被撤销授权的用户: \r\n\t[账号 {user.Account}, 姓名 {user.Name}]\r\n" +
                                 $"撤销授权的资源: \r\n\t{string.Join(",", resourcess.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                    });

                    var deleteIds = resourcess.Select(o => o.Id).ToList();

                    if (Repository_UserResources.Delete(o => o.UserId == userId && deleteIds.Contains(o.ResourcesId)) < 0)
                        throw new MessageException("撤销授权失败.");
                }
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("撤销授权失败.", ex);
            }
            else
                handler();
        }

        public void RevocationCFUCForUser(CFUCForUser data, bool runTransaction = true)
        {
            if (!data.UserIds.Any_Ex())
                return;

            var users = data.UserIds.Select(o => GetUserWithCheck(o));

            if (!users.Any())
                return;

            void handler()
            {
                foreach (var user in users)
                {
                    string userId = user.Id;
                    var config = Repository_FileUploadConfig
                                .Where(o => (data.All == true || data.ConfigIds.Contains(o.Id))
                                    && o.System_Users.AsSelect().Where(p => p.Id == userId).Any())
                                .ToList(o => new
                                {
                                    o.Id,
                                    o.Code,
                                    o.Name
                                });

                    if (!config.Any())
                        continue;

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(System_UserCFUC),
                        DataId = $"{userId}+{string.Join(",", config.Select(o => o.Id))}",
                        Explain = $"撤销用户的文件上传配置授权.",
                        Remark = $"被撤销授权的用户: \r\n\t[账号 {user.Account}, 姓名 {user.Name}]\r\n" +
                                 $"撤销授权的文件上传配置: \r\n\t{string.Join(",", config.Select(o => $"[名称 {o.Name}, 编码 {o.Code}]"))}"
                    });

                    var deleteIds = config.Select(o => o.Id).ToList();

                    if (Repository_UserCFUC.Delete(o => o.UserId == userId && deleteIds.Contains(o.ConfigId)) < 0)
                        throw new MessageException("撤销授权失败.");
                }
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("撤销授权失败.", ex);
            }
            else
                handler();
        }

        public void RevocationCFUCForUser(string configId, string userId, bool allChilds = false, bool runTransaction = true)
        {
            List<string> configIds;
            var q = Repository_FileUploadConfig.Select.Where($"a.[Id] = '{configId}'");

            if (allChilds)
                q = q.AsTreeCte();

            configIds = q.ToList(o => o.Id);

            if (configIds.Any())
                RevocationCFUCForUser(new CFUCForUser
                {
                    UserIds = new List<string> { userId },
                    ConfigIds = configIds
                }, runTransaction);
        }

        public void RevocationMenuForRole(MenuForRole data, bool runTransaction = true)
        {
            if (!data.RoleIds.Any_Ex())
                return;

            var roles = data.RoleIds.Select(o => GetRoleWithCheck(o));

            if (!roles.Any())
                return;

            void handler()
            {
                foreach (var role in roles)
                {
                    string roleId = role.Id;
                    var menus = Repository_Menu
                                .Where(o => (data.All == true || data.MenuIds.Contains(o.Id))
                                    && o.Roles.AsSelect().Where(p => p.Id == roleId).Any())
                                .ToList(o => new
                                {
                                    o.Id,
                                    o.Type,
                                    o.Name
                                });

                    if (!menus.Any())
                        continue;

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(System_RoleMenu),
                        DataId = $"{roleId}+{string.Join(",", menus.Select(o => o.Id))}",
                        Explain = $"撤销角色的菜单授权.",
                        Remark = $"被撤销授权的角色: \r\n\t[名称 {role.Name}, 类型 {role.Type}]\r\n" +
                                 $"撤销授权的菜单: \r\n\t{string.Join(",", menus.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                    });

                    var deleteIds = menus.Select(o => o.Id).ToList();

                    if (Repository_RoleMenu.Delete(o => o.RoleId == roleId && deleteIds.Contains(o.MenuId)) < 0)
                        throw new MessageException("撤销授权失败.");
                }
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("撤销授权失败.", ex);
            }
            else
                handler();
        }

        public void RevocationMenuForRole(string menuId, string roleId, bool allChilds = false, bool runTransaction = true)
        {
            List<string> menuIds;
            var q = Repository_Menu.Select.Where($"a.[Id] = '{menuId}'");

            if (allChilds)
                q = q.AsTreeCte();

            menuIds = q.ToList(o => o.Id);

            if (menuIds.Any())
                RevocationMenuForRole(new MenuForRole
                {
                    RoleIds = new List<string> { roleId },
                    MenuIds = menuIds
                }, runTransaction);
        }

        public void RevocationResourcesForRole(ResourcesForRole data, bool runTransaction = true)
        {
            if (!data.RoleIds.Any_Ex())
                return;

            var roles = data.RoleIds.Select(o => GetRoleWithCheck(o));

            if (!roles.Any())
                return;

            void handler()
            {
                foreach (var role in roles)
                {
                    string roleId = role.Id;
                    var resourcess = Repository_Resources
                                .Where(o => (data.All == true || data.ResourcesIds.Contains(o.Id))
                                    && o.Roles.AsSelect().Where(p => p.Id == roleId).Any())
                                .ToList(o => new
                                {
                                    o.Id,
                                    o.Type,
                                    o.Name
                                });

                    if (!resourcess.Any())
                        continue;

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(System_RoleResources),
                        DataId = $"{roleId}+{string.Join(",", resourcess.Select(o => o.Id))}",
                        Explain = $"撤销角色的资源授权.",
                        Remark = $"被撤销授权的角色: \r\n\t[名称 {role.Name}, 类型 {role.Type}]\r\n" +
                                 $"撤销授权的资源: \r\n\t{string.Join(",", resourcess.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                    });

                    var deleteIds = resourcess.Select(o => o.Id).ToList();

                    if (Repository_RoleResources.Delete(o => o.RoleId == roleId && deleteIds.Contains(o.ResourcesId)) < 0)
                        throw new MessageException("撤销授权失败.");
                }
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("撤销授权失败.", ex);
            }
            else
                handler();
        }

        public void RevocationCFUCForRole(CFUCForRole data, bool runTransaction = true)
        {
            if (!data.RoleIds.Any_Ex())
                return;

            var roles = data.RoleIds.Select(o => GetRoleWithCheck(o));

            if (!roles.Any())
                return;

            void handler()
            {
                foreach (var role in roles)
                {
                    string roleId = role.Id;
                    var configs = Repository_FileUploadConfig
                                .Where(o => (data.All == true || data.ConfigIds.Contains(o.Id))
                                    && o.System_Roles.AsSelect().Where(p => p.Id == roleId).Any())
                                .ToList(o => new
                                {
                                    o.Id,
                                    o.Code,
                                    o.Name
                                });

                    if (!configs.Any())
                        continue;

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(System_RoleCFUC),
                        DataId = $"{roleId}+{string.Join(",", configs.Select(o => o.Id))}",
                        Explain = $"撤销角色的文件上传配置授权.",
                        Remark = $"被撤销授权的角色: \r\n\t[名称 {role.Name}, 类型 {role.Type}]\r\n" +
                                 $"撤销授权的文件上传配置: \r\n\t{string.Join(",", configs.Select(o => $"[名称 {o.Name}, 编码 {o.Code}]"))}"
                    });

                    var deleteIds = configs.Select(o => o.Id).ToList();

                    if (Repository_RoleCFUC.Delete(o => o.RoleId == roleId && deleteIds.Contains(o.ConfigId)) < 0)
                        throw new MessageException("撤销授权失败.");
                }
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("撤销授权失败.", ex);
            }
            else
                handler();
        }

        public void RevocationCFUCForRole(string configId, string roleId, bool allChilds = false, bool runTransaction = true)
        {
            List<string> configIds;
            var q = Repository_FileUploadConfig.Select.Where($"a.[Id] = '{configId}'");

            if (allChilds)
                q = q.AsTreeCte();

            configIds = q.ToList(o => o.Id);

            if (configIds.Any())
                RevocationCFUCForRole(new CFUCForRole
                {
                    RoleIds = new List<string> { roleId },
                    ConfigIds = configIds
                }, runTransaction);
        }

        public void RevocationMenuForAll(List<string> menuIds, bool runTransaction = true)
        {
            var menus = Repository_Menu.Where(o => menuIds.Contains(o.Id)).ToList(o => new
            {
                o.Id,
                o.Type,
                o.Name
            });

            if (!menus.Any())
                return;

            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_UserMenu) + "+" + nameof(System_RoleMenu),
                    Explain = $"撤销所有用户和角色的菜单授权.",
                    Remark = $"撤销授权的菜单: \r\n\t{string.Join(",", menus.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                });

                if (Repository_UserMenu.Delete(o => (menuIds.Contains(o.MenuId))) < 0)
                    throw new MessageException("撤销用户的菜单授权失败.");

                if (Repository_RoleMenu.Delete(o => (menuIds.Contains(o.MenuId))) < 0)
                    throw new MessageException("撤销角色的菜单授权失败.");
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("撤销授权失败.", ex);
            }
            else
                handler();
        }

        public void RevocationResourcesForAll(List<string> resourcesIds, bool runTransaction = true)
        {
            var resources = Repository_Resources.Where(o => resourcesIds.Contains(o.Id)).ToList(o => new
            {
                o.Id,
                o.Type,
                o.Name
            });

            if (!resources.Any())
                return;

            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_UserResources) + "+" + nameof(System_RoleResources),
                    Explain = $"撤销所有用户和角色的资源授权.",
                    Remark = $"撤销授权的资源: \r\n\t{string.Join(",", resources.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                });

                if (Repository_UserResources.Delete(o => (resourcesIds.Contains(o.ResourcesId))) < 0)
                    throw new MessageException("撤销用户的资源授权失败.");

                if (Repository_RoleResources.Delete(o => (resourcesIds.Contains(o.ResourcesId))) < 0)
                    throw new MessageException("撤销角色的资源授权失败.");
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("撤销授权失败.", ex);
            }
            else
                handler();
        }

        public void RevocationCFUCForAll(List<string> configIds, bool runTransaction = true)
        {
            var configs = Repository_FileUploadConfig.Where(o => configIds.Contains(o.Id)).ToList(o => new
            {
                o.Id,
                o.Code,
                o.Name
            });

            if (!configs.Any())
                return;

            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_UserCFUC) + "+" + nameof(System_RoleCFUC),
                    Explain = $"撤销所有用户和角色的文件上传配置授权.",
                    Remark = $"撤销授权的文件上传配置: \r\n\t{string.Join(",", configs.Select(o => $"[名称 {o.Name}, 编码 {o.Code}]"))}"
                });

                if (Repository_UserCFUC.Delete(o => (configIds.Contains(o.ConfigId))) < 0)
                    throw new MessageException("撤销用户的文件上传配置授权失败.");

                if (Repository_RoleCFUC.Delete(o => (configIds.Contains(o.ConfigId))) < 0)
                    throw new MessageException("撤销角色的文件上传配置授权失败.");
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("撤销授权失败.", ex);
            }
            else
                handler();
        }

        #endregion

        #region 获取授权

        public Model.System.UserDTO.Authorities GetUser(string userId, bool includeRole, bool includeMenu, bool includeResources, bool includeCFUC, bool mergeRoleMenu = true, bool mergeRoleResources = true, bool mergeRoleCFUC = true)
        {
            var user = Repository_User.Where(o => o.Id == userId)
                                    .ToOne(o => new Model.System.UserDTO.Authorities
                                    {
                                        Id = o.Id,
                                        Account = o.Account,
                                        Enable = o.Enable
                                    });

            if (user == default)
                throw new MessageException("用户不存在或已被移除.");

            if (!user.Enable)
                throw new MessageException("用户账号已禁用.");

            if (includeRole)
                user._Roles = GetUserRole(userId, includeMenu && !mergeRoleMenu, includeResources && !mergeRoleResources, includeCFUC && !mergeRoleCFUC);

            if (includeMenu)
                user._Menus = GetUserMenu(userId, mergeRoleMenu);

            if (includeResources)
                user._Resources = GetUserResources(userId, mergeRoleResources);

            if (includeCFUC)
                user._FileUploadConfigs = GetUserCFUC(userId, mergeRoleCFUC);

            return user;
        }

        public Model.Public.MemberDTO.Authorities GetMember(string memberId, bool includeRole, bool includeMenu, bool includeResources, bool includeCFUC)
        {
            var member = Repository_Member.Where(o => o.Id == memberId)
                                    .ToOne(o => new Model.Public.MemberDTO.Authorities
                                    {
                                        Id = o.Id,
                                        Account = o.Account,
                                        Enable = o.Enable
                                    });

            if (member == default)
                throw new MessageException("会员用户不存在或已被移除.");

            if (!member.Enable)
                throw new MessageException("会员账号已禁用.");

            if (includeRole)
                member._Roles = GetMemberRole(memberId, includeMenu, includeResources, includeCFUC);

            return member;
        }

        public List<Model.System.RoleDTO.Authorities> GetUserRole(string userId, bool includeMenu, bool includeResources, bool includeCFUC)
        {
            var roles = Repository_Role.Where(o => o.Users.AsSelect().Where(p => p.Id == userId).Any() && o.Enable == true)
                                         .ToList(o => new Model.System.RoleDTO.Authorities
                                         {
                                             Id = o.Id,
                                             Name = o.Name,
                                             Type = o.Type,
                                             Code = o.Code
                                         });

            if (includeMenu || includeResources || includeCFUC)
                roles.ForEach(o =>
                {
                    if (includeMenu)
                        o._Menus = GetRoleMenu(o.Id);

                    if (includeResources)
                        o._Resources = GetRoleResources(o.Id, new PaginationDTO { PageIndex = -1 });

                    if (includeCFUC)
                        o._FileUploadConfigs = GetRoleCFUC(o.Id);
                });

            return roles;
        }

        public List<Model.System.RoleDTO.Authorities> GetMemberRole(string memberId, bool includeMenu, bool includeResources, bool includeCFUC)
        {
            var roles = Repository_Role.Where(o => o.Members.AsSelect().Where(p => p.Id == memberId).Any() && o.Enable == true)
                                         .ToList(o => new Model.System.RoleDTO.Authorities
                                         {
                                             Id = o.Id,
                                             Name = o.Name,
                                             Type = o.Type,
                                             Code = o.Code
                                         });

            if (includeMenu || includeResources || includeCFUC)
                roles.ForEach(o =>
                {
                    if (includeMenu)
                        o._Menus = GetRoleMenu(o.Id);

                    if (includeResources)
                        o._Resources = GetRoleResources(o.Id, new PaginationDTO { PageIndex = -1 });

                    if (includeCFUC)
                        o._FileUploadConfigs = GetRoleCFUC(o.Id);
                });

            return roles;
        }

        public List<string> GetUserRoleTypes(string userId)
        {
            var roleTypes = Repository_Role.Where(o => o.Users.AsSelect().Where(p => p.Id == userId).Any() && o.Enable == true)
                                        .GroupBy(o => o.Type)
                                        .ToList(o => o.Key);
            return roleTypes;
        }

        public List<string> GetUserRoleNames(string userId)
        {
            var roleNames = Repository_Role.Where(o => o.Users.AsSelect().Where(p => p.Id == userId).Any() && o.Enable == true)
                                        .GroupBy(o => o.Name)
                                        .ToList(o => o.Key);
            return roleNames;
        }

        public List<string> GetMemberRoleTypes(string memberId)
        {
            var roleTypes = Repository_Role.Where(o => o.Members.AsSelect().Where(p => p.Id == memberId).Any() && o.Enable == true)
                                        .GroupBy(o => o.Type)
                                        .ToList(o => o.Key);
            return roleTypes;
        }

        public List<string> GetMemberRoleNames(string memberId)
        {
            var roleNames = Repository_Role.Where(o => o.Members.AsSelect().Where(p => p.Id == memberId).Any() && o.Enable == true)
                                        .GroupBy(o => o.Name)
                                        .ToList(o => o.Key);
            return roleNames;
        }

        public List<Model.System.MenuDTO.Authorities> GetUserMenu(string userId, bool mergeRoleMenu)
        {
            var menus = Repository_Menu.Where(o => (o.Users.AsSelect()
                                                            .Where(p => p.Id == userId)
                                                            .Any()
                                                    || (mergeRoleMenu == true
                                                        && o.Roles.AsSelect()
                                                                .Where(p => p.Users.AsSelect()
                                                                                .Where(q => q.Id == userId)
                                                                                .Any() && p.Enable == true)
                                                                .Any())) && o.Enable == true)
                                    .ToList(o => new Model.System.MenuDTO.Authorities
                                    {
                                        Id = o.Id,
                                        Name = o.Name,
                                        Type = o.Type,
                                        Code = o.Code,
                                        Uri = o.Uri,
                                        Method = o.Method
                                    });

            return menus;
        }

        public List<Model.System.MenuDTO.Authorities> GetMemberMenu(string memberId)
        {
            var menus = Repository_Menu.Where(o => o.Roles.AsSelect()
                                                        .Where(p => p.Members.AsSelect()
                                                                            .Where(q => q.Id == memberId)
                                                                            .Any() && p.Enable == true)
                                                        .Any() && o.Enable == true)
                                    .ToList(o => new Model.System.MenuDTO.Authorities
                                    {
                                        Id = o.Id,
                                        Name = o.Name,
                                        Type = o.Type,
                                        Code = o.Code,
                                        Uri = o.Uri,
                                        Method = o.Method
                                    });

            return menus;
        }

        public List<Model.System.ResourcesDTO.Authorities> GetUserResources(string userId, bool mergeRoleResources, PaginationDTO pagination = null)
        {
            var select = Repository_Resources.Where(o => (o.Users.AsSelect()
                                                            .Where(p => p.Id == userId)
                                                            .Any()
                                                    || (mergeRoleResources == true
                                                        && o.Roles.AsSelect()
                                                                .Where(p => p.Users.AsSelect()
                                                                                .Where(q => q.Id == userId)
                                                                                .Any() && p.Enable == true)
                                                                .Any())) && o.Enable == true);
            if (pagination != null)
                select = select.GetPagination(pagination);

            var resources = select.ToList(o => new Model.System.ResourcesDTO.Authorities
            {
                Id = o.Id,
                Name = o.Name,
                Type = o.Type,
                Code = o.Code,
                Uri = o.Uri
            });

            return resources;
        }

        public List<Model.System.ResourcesDTO.Authorities> GetMemberResources(string memberId, PaginationDTO pagination = null)
        {
            var select =
               Repository_Resources.Where(o => o.Roles.AsSelect()
                                                        .Where(p => p.Members.AsSelect()
                                                                            .Where(q => q.Id == memberId)
                                                                            .Any() && p.Enable == true)
                                                        .Any() && o.Enable == true);
            if (pagination != null)
                select = select.GetPagination(pagination);

            var resources = select.ToList(o => new Model.System.ResourcesDTO.Authorities
            {
                Id = o.Id,
                Name = o.Name,
                Type = o.Type,
                Code = o.Code,
                Uri = o.Uri
            });

            return resources;
        }

        public List<Model.Common.FileUploadConfigDTO.Authorities> GetUserCFUC(string userId, bool mergeRoleCFUC)
        {
            var configs = Repository_FileUploadConfig.Where(o => (o.System_Users.AsSelect()
                                                            .Where(p => p.Id == userId)
                                                            .Any()
                                                    || (mergeRoleCFUC == true
                                                        && o.System_Roles.AsSelect()
                                                                .Where(p => p.Users.AsSelect()
                                                                                .Where(q => q.Id == userId)
                                                                                .Any() && p.Enable == true)
                                                                .Any())) && o.Enable == true)
                                    .ToList(o => new Model.Common.FileUploadConfigDTO.Authorities
                                    {
                                        Id = o.Id,
                                        Code = o.Code,
                                        Name = o.Name,
                                        DisplayName = o.DisplayName,
                                        Public = o.Public
                                    });

            return configs;
        }

        public List<Model.Common.FileUploadConfigDTO.Authorities> GetMemberCFUC(string memberId)
        {
            var configs = Repository_FileUploadConfig.Where(o => o.System_Roles.AsSelect()
                                                        .Where(p => p.Members.AsSelect()
                                                                            .Where(q => q.Id == memberId)
                                                                            .Any() && p.Enable == true)
                                                        .Any() && o.Enable == true)
                                    .ToList(o => new Model.Common.FileUploadConfigDTO.Authorities
                                    {
                                        Id = o.Id,
                                        Code = o.Code,
                                        Name = o.Name,
                                        DisplayName = o.DisplayName,
                                        Public = o.Public
                                    });

            return configs;
        }

        public Model.System.RoleDTO.Authorities GetRole(string roleId, bool includeMenu, bool includeResources, bool includeCFUC)
        {
            var role = Repository_Role.Where(o => o.Id == roleId)
                                    .ToOne(o => new Model.System.RoleDTO.Authorities
                                    {
                                        Id = o.Id,
                                        Name = o.Name,
                                        Type = o.Type,
                                        Code = o.Code
                                    });

            if (role == default)
                throw new MessageException("角色不存在或已被移除.");

            if (!role.Enable)
                throw new MessageException("角色已禁用.");

            if (includeMenu)
                role._Menus = GetRoleMenu(role.Id);

            if (includeResources)
                role._Resources = GetRoleResources(role.Id, new PaginationDTO { PageIndex = -1 });

            if (includeCFUC)
                role._FileUploadConfigs = GetRoleCFUC(role.Id);

            return role;
        }

        public List<Model.System.MenuDTO.Authorities> GetRoleMenu(string roleId)
        {
            return Repository_Menu.Where(o => o.Roles.AsSelect()
                                            .Where(p => p.Id == roleId)
                                            .Any() && o.Enable == true)
                                .ToList(o => new Model.System.MenuDTO.Authorities
                                {
                                    Id = o.Id,
                                    Name = o.Name,
                                    Type = o.Type,
                                    Code = o.Code,
                                    Uri = o.Uri,
                                    Method = o.Method
                                });
        }

        public List<Model.System.ResourcesDTO.Authorities> GetRoleResources(string roleId, PaginationDTO pagination)
        {
            return Repository_Resources.Where(o => o.Roles.AsSelect()
                                            .Where(p => p.Id == roleId)
                                            .Any() && o.Enable == true)
                                .GetPagination(pagination)
                                .ToList(o => new Model.System.ResourcesDTO.Authorities
                                {
                                    Id = o.Id,
                                    Name = o.Name,
                                    Type = o.Type,
                                    Code = o.Code,
                                    Uri = o.Uri
                                });
        }

        public List<Model.Common.FileUploadConfigDTO.Authorities> GetRoleCFUC(string roleId)
        {
            return Repository_FileUploadConfig.Where(o => o.System_Roles.AsSelect()
                                            .Where(p => p.Id == roleId)
                                            .Any() && o.Enable == true)
                                .ToList(o => new Model.Common.FileUploadConfigDTO.Authorities
                                {
                                    Id = o.Id,
                                    Code = o.Code,
                                    Name = o.Name,
                                    DisplayName = o.DisplayName,
                                    Public = o.Public
                                });
        }

        #endregion

        #region 验证授权

        public bool IsSuperAdminUser(string userId, bool checkEnable = true)
        {
            return Repository_UserRole.Where(o => o.UserId == userId && o.Role.Type == $"{RoleType.超级管理员}" && (checkEnable == false || o.Role.Enable == true)).Any();
        }

        public bool IsSuperAdminRole(string roleId, bool checkEnable = true)
        {
            return Repository_Role.Where(o => o.Id == roleId && o.Type == $"{RoleType.超级管理员}" && (checkEnable == false || o.Enable == true)).Any();
        }

        public bool IsAdminUser(string userId, bool checkEnable = true)
        {
            return Repository_UserRole.Where(o => o.UserId == userId && (o.Role.Type == $"{RoleType.超级管理员}" || o.Role.Type == $"{RoleType.管理员}") && (checkEnable == false || o.Role.Enable == true)).Any();
        }

        public bool IsAdminRole(string roleId, bool checkEnable = true)
        {
            return Repository_Role.Where(o => o.Id == roleId && (o.Type == $"{RoleType.超级管理员}" || o.Type == $"{RoleType.管理员}") && (checkEnable == false || o.Enable == true)).Any();
        }

        public bool UserHasRole(string userId, string roleId, bool checkEnable = true)
        {
            return Repository_UserRole.Where(o => o.UserId == userId && o.RoleId == roleId && (checkEnable == false || o.Role.Enable == true)).Any();
        }

        public bool UserHasRoleType(string userId, string roleType, bool checkEnable = true)
        {
            return Repository_UserRole.Where(o => o.UserId == userId && o.Role.Type == roleType && (checkEnable == false || o.Role.Enable == true)).Any();
        }

        public bool MemberHasRole(string memberId, string roleId, bool checkEnable = true)
        {
            return Repository_MemberRole.Where(o => o.MemberId == memberId && o.RoleId == roleId && (checkEnable == false || o.Role.Enable == true)).Any();
        }

        public bool MemberHasRoleType(string memberId, string roleType, bool checkEnable = true)
        {
            return Repository_MemberRole.Where(o => o.MemberId == memberId && o.Role.Type == roleType && (checkEnable == false || o.Role.Enable == true)).Any();
        }

        public bool UserHasMenu(string userId, string menuId, bool checkEnable = true)
        {
            return Repository_Menu.Where(o => o.Id == menuId && (checkEnable == false || o.Enable == true)
                                            && (o.Users.AsSelect().Where(p => p.Id == userId).Any()
                                                || o.Roles.AsSelect().Where(p => p.Users.AsSelect().Where(q => q.Id == userId).Any()).Any()))
                                .Any();
        }

        public bool UserHasMenuUri(string userId, string menuUri, bool checkEnable = true)
        {
            return Repository_Menu.Where(o => menuUri.Contains(o.Uri) && (checkEnable == false || o.Enable == true)
                                            && (o.Users.AsSelect().Where(p => p.Id == userId).Any()
                                                || o.Roles.AsSelect().Where(p => p.Users.AsSelect().Where(q => q.Id == userId).Any()).Any()))
                                .Any();
        }

        public bool MemberHasMenu(string memberId, string menuId, bool checkEnable = true)
        {
            return Repository_Menu.Where(o => o.Id == menuId && (checkEnable == false || o.Enable == true)
                                            && o.Roles.AsSelect().Where(p => p.Members.AsSelect().Where(q => q.Id == memberId).Any()).Any())
                                .Any();
        }

        public bool MemberHasMenuUri(string memberId, string menuUri, bool checkEnable = true)
        {
            return Repository_Menu.Where(o => menuUri.Contains(o.Uri) && (checkEnable == false || o.Enable == true)
                                            && o.Roles.AsSelect().Where(p => p.Members.AsSelect().Where(q => q.Id == memberId).Any()).Any())
                                .Any();
        }

        public bool UserHasResources(string userId, string resourcesId, bool checkEnable = true)
        {
            return Repository_Resources.Where(o => o.Id == resourcesId && (checkEnable == false || o.Enable == true)
                                            && (o.Users.AsSelect().Where(p => p.Id == userId).Any()
                                                || o.Roles.AsSelect().Where(p => p.Users.AsSelect().Where(q => q.Id == userId).Any()).Any()))
                                .Any();
        }

        public bool UserHasResourcesUri(string userId, string resourcesUri, bool checkEnable = true)
        {
            return Repository_Resources.Where(o => resourcesUri.Contains(o.Uri) && (checkEnable == false || o.Enable == true)
                                            && (o.Users.AsSelect().Where(p => p.Id == userId).Any()
                                                || o.Roles.AsSelect().Where(p => p.Users.AsSelect().Where(q => q.Id == userId).Any()).Any()))
                                .Any();
        }

        public bool MemberHasResources(string memberId, string resourcesId, bool checkEnable = true)
        {
            return Repository_Resources.Where(o => o.Id == resourcesId && (checkEnable == false || o.Enable == true)
                                            && o.Roles.AsSelect().Where(p => p.Members.AsSelect().Where(q => q.Id == memberId).Any()).Any())
                                .Any();
        }

        public bool MemberHasResourcesUri(string memberId, string resourcesUri, bool checkEnable = true)
        {
            return Repository_Resources.Where(o => resourcesUri.Contains(o.Uri) && (checkEnable == false || o.Enable == true)
                                            && o.Roles.AsSelect().Where(p => p.Members.AsSelect().Where(q => q.Id == memberId).Any()).Any())
                                .Any();
        }

        public bool UserHasCFUC(string userId, string configId, bool checkEnable = true)
        {
            return Repository_FileUploadConfig.Where(o => o.Id == configId && (checkEnable == false || o.Enable == true)
                                            && (o.System_Users.AsSelect().Where(p => p.Id == userId).Any()
                                                || o.System_Roles.AsSelect().Where(p => p.Users.AsSelect().Where(q => q.Id == userId).Any()).Any()))
                                .Any();
        }

        public bool MemberHasCFUC(string memberId, string configId, bool checkEnable = true)
        {
            return Repository_FileUploadConfig.Where(o => o.Id == configId && (checkEnable == false || o.Enable == true)
                                            && o.System_Roles.AsSelect().Where(p => p.Members.AsSelect().Where(q => q.Id == memberId).Any()).Any())
                                .Any();
        }

        public bool CurrentAccountHasCFUC(string configId, bool checkEnable = true)
        {
            return Operator.AuthenticationInfo.UserType switch
            {
                UserType.系统用户 => UserHasCFUC(Operator.AuthenticationInfo.Id, configId),
                UserType.会员 => MemberHasCFUC(Operator.AuthenticationInfo.Id, configId),
                _ => false,
            };
        }

        #endregion

        #region 拓展方法

        public List<Model.System.MenuDTO.AuthoritiesTree> GetRoleMenuTree(string roleId, TreePaginationDTO pagination)
        {
            return GetRoleMenuTree(pagination, false);

            List<Model.System.MenuDTO.AuthoritiesTree> GetRoleMenuTree(TreePaginationDTO pagination, bool deep)
            {
                if (pagination.ParentId.IsNullOrWhiteSpace())
                    pagination.ParentId = null;

                var result = Repository_Menu.Select
                                        .Where(o => o.ParentId == pagination.ParentId
                                            && (Operator.IsSuperAdmin == true
                                                || o.Users.AsSelect().Any(p => p.Id == Operator.AuthenticationInfo.Id)
                                                || o.Roles.AsSelect().Any(p => p.Users.AsSelect().Any(q => q.Id == Operator.AuthenticationInfo.Id))))
                                        .GetPagination(pagination.Pagination)
                                        .ToList(o => new Model.System.MenuDTO.AuthoritiesTree
                                        {
                                            Id = o.Id,
                                            Name = o.Name,
                                            Type = o.Type,
                                            Code = o.Code,
                                            Uri = o.Uri,
                                            Method = o.Method,
                                            Authorized = o.Roles.AsSelect().Any(p => p.Id == roleId) ? true : false
                                        });

                if (result.Any())
                    result.ForEach(o =>
                    {
                        var rank = pagination.Rank;
                        if (!rank.HasValue || rank > 0)
                        {
                            pagination.ParentId = o.Id;
                            pagination.Rank--;
                            o.Children = GetRoleMenuTree(pagination, true);
                            o.HasChildren = o.Children.Any_Ex();
                            o.ChildrenCount = o.Children?.Count ?? 0;
                        }
                        else
                        {
                            o.HasChildren = Repository_Menu.Select.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Any();
                            o.ChildrenCount = (int)Repository_Menu.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Count();
                        }
                    });
                else if (deep)
                    result = null;

                return result;
            }
        }

        public List<Model.System.RoleDTO.AuthoritiesTree> GetUserRoleTree(string userId, TreePaginationDTO pagination)
        {
            return GetUserRoleTree(pagination, false);

            List<Model.System.RoleDTO.AuthoritiesTree> GetUserRoleTree(TreePaginationDTO pagination, bool deep)
            {
                if (pagination.ParentId.IsNullOrWhiteSpace())
                    pagination.ParentId = null;

                var result = Repository_Role.Select
                                        .Where(o => o.ParentId == pagination.ParentId
                                            && o.Type != $"{RoleType.会员}"
                                            && (Operator.IsSuperAdmin == true
                                                || o.Users.AsSelect().Any(p => p.Id == Operator.AuthenticationInfo.Id)))
                                        .GetPagination(pagination.Pagination)
                                        .ToList(o => new Model.System.RoleDTO.AuthoritiesTree
                                        {
                                            Id = o.Id,
                                            Name = o.Name,
                                            Type = o.Type,
                                            Code = o.Code,
                                            Authorized = o.Users.AsSelect().Any(p => p.Id == userId) ? true : false
                                        });

                if (result.Any())
                    result.ForEach(o =>
                    {
                        var rank = pagination.Rank;
                        if (!rank.HasValue || rank > 0)
                        {
                            pagination.ParentId = o.Id;
                            pagination.Rank--;
                            o.Children = GetUserRoleTree(pagination, true);
                            o.HasChildren = o.Children.Any_Ex();
                            o.ChildrenCount = o.Children?.Count ?? 0;
                        }
                        else
                        {
                            o.HasChildren = Repository_Role.Select.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Any();
                            o.ChildrenCount = (int)Repository_Role.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Count();
                        }
                    });
                else if (deep)
                    result = null;

                return result;
            }
        }

        public List<Model.System.RoleDTO.AuthoritiesTree> GetMemberRoleTree(string memberId, TreePaginationDTO pagination)
        {
            return GetMemberRoleTree(pagination, false);

            List<Model.System.RoleDTO.AuthoritiesTree> GetMemberRoleTree(TreePaginationDTO pagination, bool deep)
            {
                if (pagination.ParentId.IsNullOrWhiteSpace())
                    pagination.ParentId = null;

                var result = Repository_Role.Select
                                        .Where(o => o.ParentId == pagination.ParentId
                                            && o.Type == $"{RoleType.会员}"
                                            && (Operator.IsSuperAdmin == true
                                                || o.Users.AsSelect().Any(p => p.Id == Operator.AuthenticationInfo.Id)))
                                        .GetPagination(pagination.Pagination)
                                        .ToList(o => new Model.System.RoleDTO.AuthoritiesTree
                                        {
                                            Id = o.Id,
                                            Name = o.Name,
                                            Type = o.Type,
                                            Code = o.Code,
                                            Authorized = o.Members.AsSelect().Any(p => p.Id == memberId) ? true : false
                                        });

                if (result.Any())
                    result.ForEach(o =>
                    {
                        var rank = pagination.Rank;
                        if (!rank.HasValue || rank > 0)
                        {
                            pagination.ParentId = o.Id;
                            pagination.Rank--;
                            o.Children = GetMemberRoleTree(pagination, true);
                            o.HasChildren = o.Children.Any_Ex();
                            o.ChildrenCount = o.Children?.Count ?? 0;
                        }
                        else
                        {
                            o.HasChildren = Repository_Role.Select.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Any();
                            o.ChildrenCount = (int)Repository_Role.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Count();
                        }
                    });
                else if (deep)
                    result = null;

                return result;
            }
        }

        public List<Model.System.RoleDTO.AuthoritiesTree> GetCurrentAccountRoleTree(TreePaginationDTO pagination)
        {
            return Operator.AuthenticationInfo.UserType switch
            {
                UserType.系统用户 => GetUserRoleTree(Operator.AuthenticationInfo.Id, pagination),
                UserType.会员 => GetMemberRoleTree(Operator.AuthenticationInfo.Id, pagination),
                _ => null
            };
        }

        public List<Model.System.MenuDTO.AuthoritiesTree> GetUserMenuTree(string userId, TreePaginationDTO pagination)
        {
            return GetUserMenuTree(pagination, false);

            List<Model.System.MenuDTO.AuthoritiesTree> GetUserMenuTree(TreePaginationDTO pagination, bool deep)
            {
                if (pagination.ParentId.IsNullOrWhiteSpace())
                    pagination.ParentId = null;

                var result = Repository_Menu.Select
                                        .Where(o => o.ParentId == pagination.ParentId
                                            && (Operator.IsSuperAdmin == true
                                                || o.Users.AsSelect().Any(p => p.Id == Operator.AuthenticationInfo.Id)
                                                || o.Roles.AsSelect().Any(p => p.Users.AsSelect().Any(q => q.Id == Operator.AuthenticationInfo.Id))))
                                        .GetPagination(pagination.Pagination)
                                        .ToList(o => new Model.System.MenuDTO.AuthoritiesTree
                                        {
                                            Id = o.Id,
                                            Name = o.Name,
                                            Type = o.Type,
                                            Code = o.Code,
                                            Uri = o.Uri,
                                            Method = o.Method,
                                            Authorized = (o.Users.AsSelect().Any(p => p.Id == userId)
                                                || o.Roles.AsSelect().Any(p => p.Users.AsSelect().Any(q => q.Id == userId))) ? true : false
                                        });

                if (result.Any())
                    result.ForEach(o =>
                    {
                        var rank = pagination.Rank;
                        if (!rank.HasValue || rank > 0)
                        {
                            pagination.ParentId = o.Id;
                            pagination.Rank--;
                            o.Children = GetUserMenuTree(pagination, true);
                            o.HasChildren = o.Children.Any_Ex();
                            o.ChildrenCount = o.Children?.Count ?? 0;
                        }
                        else
                        {
                            o.HasChildren = Repository_Menu.Select.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Any();
                            o.ChildrenCount = (int)Repository_Menu.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Count();
                        }
                    });
                else if (deep)
                    result = null;

                return result;
            }
        }

        public List<Model.System.MenuDTO.AuthoritiesTree> GetMemberMenuTree(string memberId, TreePaginationDTO pagination)
        {
            return GetMemberMenuTree(pagination, false);

            List<Model.System.MenuDTO.AuthoritiesTree> GetMemberMenuTree(TreePaginationDTO pagination, bool deep)
            {
                if (pagination.ParentId.IsNullOrWhiteSpace())
                    pagination.ParentId = null;

                var result = Repository_Menu.Select
                                        .Where(o => o.ParentId == pagination.ParentId
                                            && (o.Roles.AsSelect().Any(p => p.Members.AsSelect().Any(q => q.Id == Operator.AuthenticationInfo.Id))))
                                        .GetPagination(pagination.Pagination)
                                        .ToList(o => new Model.System.MenuDTO.AuthoritiesTree
                                        {
                                            Id = o.Id,
                                            Name = o.Name,
                                            Type = o.Type,
                                            Code = o.Code,
                                            Uri = o.Uri,
                                            Method = o.Method,
                                            Authorized = (o.Roles.AsSelect().Any(p => p.Members.AsSelect().Any(q => q.Id == memberId))) ? true : false
                                        });

                if (result.Any())
                    result.ForEach(o =>
                    {
                        var rank = pagination.Rank;
                        if (!rank.HasValue || rank > 0)
                        {
                            pagination.ParentId = o.Id;
                            pagination.Rank--;
                            o.Children = GetMemberMenuTree(pagination, true);
                            o.HasChildren = o.Children.Any_Ex();
                            o.ChildrenCount = o.Children?.Count ?? 0;
                        }
                        else
                        {
                            o.HasChildren = Repository_Menu.Select.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Any();
                            o.ChildrenCount = (int)Repository_Menu.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Count();
                        }
                    });
                else if (deep)
                    result = null;

                return result;
            }
        }

        public List<Model.System.MenuDTO.AuthoritiesTree> GetCurrentAccountMenuTree(TreePaginationDTO pagination)
        {
            return Operator.AuthenticationInfo.UserType switch
            {
                UserType.系统用户 => GetUserMenuTree(Operator.AuthenticationInfo.Id, pagination),
                UserType.会员 => GetMemberMenuTree(Operator.AuthenticationInfo.Id, pagination),
                _ => null
            };
        }

        public List<Model.System.ResourcesDTO.Authorities> GetRoleResourcesList(string roleId, PaginationDTO pagination)
        {
            return Repository_Resources.Where(o => !o.Roles.AsSelect()
                                .Where(p => p.Id == roleId)
                                .Any() && o.Enable == true)
                    .GetPagination(pagination)
                    .ToList(o => new Model.System.ResourcesDTO.Authorities
                    {
                        Id = o.Id,
                        Name = o.Name,
                        Type = o.Type,
                        Code = o.Code,
                        Uri = o.Uri
                    });
        }

        public List<Model.System.ResourcesDTO.Authorities> GetUserResourcesList(string userId, PaginationDTO pagination)
        {
            return Repository_Resources.Where(o => !o.Users.AsSelect()
                                                    .Where(p => p.Id == userId)
                                                    .Any()
                                                && !o.Roles.AsSelect()
                                                    .Where(p => p.Users.AsSelect().Where(q => q.Id == userId).Any())
                                                    .Any()
                                                && o.Enable == true)
                    .GetPagination(pagination)
                    .ToList(o => new Model.System.ResourcesDTO.Authorities
                    {
                        Id = o.Id,
                        Name = o.Name,
                        Type = o.Type,
                        Code = o.Code,
                        Uri = o.Uri
                    });
        }

        public List<Model.System.ResourcesDTO.Authorities> GetCurrentAccountResourcesList(PaginationDTO pagination)
        {
            return Operator.AuthenticationInfo.UserType switch
            {
                UserType.系统用户 => GetUserResources(Operator.AuthenticationInfo.Id, true, pagination),
                UserType.会员 => GetMemberResources(Operator.AuthenticationInfo.Id, pagination),
                _ => null
            };
        }

        public List<Model.Common.FileUploadConfigDTO.AuthoritiesTree> GetRoleCFUCTree(string roleId, TreePaginationDTO pagination)
        {
            return GetRoleCFUCTree(pagination, false);

            List<Model.Common.FileUploadConfigDTO.AuthoritiesTree> GetRoleCFUCTree(TreePaginationDTO pagination, bool deep)
            {
                if (pagination.ParentId.IsNullOrWhiteSpace())
                    pagination.ParentId = null;

                var result = Repository_FileUploadConfig.Select
                                        .Where(o => o.ParentId == pagination.ParentId
                                            && (Operator.IsSuperAdmin == true
                                                || o.System_Users.AsSelect().Any(p => p.Id == Operator.AuthenticationInfo.Id)
                                                || o.System_Roles.AsSelect().Any(p => p.Users.AsSelect().Any(q => q.Id == Operator.AuthenticationInfo.Id))))
                                        .GetPagination(pagination.Pagination)
                                        .ToList(o => new Model.Common.FileUploadConfigDTO.AuthoritiesTree
                                        {
                                            Id = o.Id,
                                            Code = o.Code,
                                            Name = o.Name,
                                            DisplayName = o.DisplayName,
                                            Public = o.Public,
                                            Authorized = o.System_Roles.AsSelect().Any(p => p.Id == roleId) ? true : false
                                        });

                if (result.Any())
                    result.ForEach(o =>
                    {
                        var rank = pagination.Rank;
                        if (!rank.HasValue || rank > 0)
                        {
                            pagination.ParentId = o.Id;
                            pagination.Rank--;
                            o.Children = GetRoleCFUCTree(pagination, true);
                            o.HasChildren = o.Children.Any_Ex();
                            o.ChildrenCount = o.Children?.Count ?? 0;
                        }
                        else
                        {
                            o.HasChildren = Repository_FileUploadConfig.Select.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Any();
                            o.ChildrenCount = (int)Repository_FileUploadConfig.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Count();
                        }
                    });
                else if (deep)
                    result = null;

                return result;
            }
        }

        public List<Model.Common.FileUploadConfigDTO.AuthoritiesTree> GetUserCFUCTree(string userId, TreePaginationDTO pagination)
        {
            return GetUserCFUCTree(pagination, false);

            List<Model.Common.FileUploadConfigDTO.AuthoritiesTree> GetUserCFUCTree(TreePaginationDTO pagination, bool deep)
            {
                if (pagination.ParentId.IsNullOrWhiteSpace())
                    pagination.ParentId = null;

                var result = Repository_FileUploadConfig.Select
                                        .Where(o => o.ParentId == pagination.ParentId
                                            && (Operator.IsSuperAdmin == true
                                                || o.System_Users.AsSelect().Any(p => p.Id == Operator.AuthenticationInfo.Id)
                                                || o.System_Roles.AsSelect().Any(p => p.Users.AsSelect().Any(q => q.Id == Operator.AuthenticationInfo.Id))))
                                        .GetPagination(pagination.Pagination)
                                        .ToList(o => new Model.Common.FileUploadConfigDTO.AuthoritiesTree
                                        {
                                            Id = o.Id,
                                            Code = o.Code,
                                            Name = o.Name,
                                            DisplayName = o.DisplayName,
                                            Public = o.Public,
                                            Authorized = (o.System_Users.AsSelect().Any(p => p.Id == userId)
                                                || o.System_Roles.AsSelect().Any(p => p.Users.AsSelect().Any(q => q.Id == userId))) ? true : false
                                        });

                if (result.Any())
                    result.ForEach(o =>
                    {
                        var rank = pagination.Rank;
                        if (!rank.HasValue || rank > 0)
                        {
                            pagination.ParentId = o.Id;
                            pagination.Rank--;
                            o.Children = GetUserCFUCTree(pagination, true);
                            o.HasChildren = o.Children.Any_Ex();
                            o.ChildrenCount = o.Children?.Count ?? 0;
                        }
                        else
                        {
                            o.HasChildren = Repository_FileUploadConfig.Select.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Any();
                            o.ChildrenCount = (int)Repository_FileUploadConfig.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Count();
                        }
                    });
                else if (deep)
                    result = null;

                return result;
            }
        }

        public List<Model.Common.FileUploadConfigDTO.AuthoritiesTree> GetMemberCFUCTree(string memberId, TreePaginationDTO pagination)
        {
            return GetMemberCFUCTree(pagination, false);

            List<Model.Common.FileUploadConfigDTO.AuthoritiesTree> GetMemberCFUCTree(TreePaginationDTO pagination, bool deep)
            {
                if (pagination.ParentId.IsNullOrWhiteSpace())
                    pagination.ParentId = null;

                var result = Repository_FileUploadConfig.Select
                                        .Where(o => o.ParentId == pagination.ParentId
                                            && (o.System_Roles.AsSelect().Any(p => p.Members.AsSelect().Any(q => q.Id == Operator.AuthenticationInfo.Id))))
                                        .GetPagination(pagination.Pagination)
                                        .ToList(o => new Model.Common.FileUploadConfigDTO.AuthoritiesTree
                                        {
                                            Id = o.Id,
                                            Code = o.Code,
                                            Name = o.Name,
                                            DisplayName = o.DisplayName,
                                            Public = o.Public,
                                            Authorized = (o.System_Roles.AsSelect().Any(p => p.Members.AsSelect().Any(q => q.Id == memberId))) ? true : false
                                        });

                if (result.Any())
                    result.ForEach(o =>
                    {
                        var rank = pagination.Rank;
                        if (!rank.HasValue || rank > 0)
                        {
                            pagination.ParentId = o.Id;
                            pagination.Rank--;
                            o.Children = GetMemberCFUCTree(pagination, true);
                            o.HasChildren = o.Children.Any_Ex();
                            o.ChildrenCount = o.Children?.Count ?? 0;
                        }
                        else
                        {
                            o.HasChildren = Repository_FileUploadConfig.Select.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Any();
                            o.ChildrenCount = (int)Repository_FileUploadConfig.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Count();
                        }
                    });
                else if (deep)
                    result = null;

                return result;
            }
        }

        public List<Model.Common.FileUploadConfigDTO.AuthoritiesTree> GetCurrentAccountCFUCTree(TreePaginationDTO pagination)
        {
            return Operator.AuthenticationInfo.UserType switch
            {
                UserType.系统用户 => GetUserCFUCTree(Operator.AuthenticationInfo.Id, pagination),
                UserType.会员 => GetMemberCFUCTree(Operator.AuthenticationInfo.Id, pagination),
                _ => null
            };
        }

        #endregion

        #endregion
    }
}
