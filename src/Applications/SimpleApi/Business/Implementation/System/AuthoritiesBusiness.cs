using AutoMapper;
using Business.Interface.Common;
using Business.Interface.System;
using Business.Utils;
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

            if (user == null)
                throw new ApplicationException("用户不存在或已被移除.");

            if (!user.Enable)
                throw new ApplicationException("用户账号已禁用.");

            if (Repository_UserRole.Where(o => o.UserId == userId && o.Role.Type == $"{RoleType.超级管理员}").Any())
                throw new ApplicationException($"拥有{RoleType.超级管理员}角色的用户无需进行相关授权操作.");

            return user;
        }

        private dynamic GetMemberWithCheck(string memberId)
        {
            var user = Repository_Member.Where(o => o.Id == memberId).ToOne(o => new
            {
                o.Id,
                o.Enable,
                o.Account,
                o.Name
            });

            if (user == null)
                throw new ApplicationException("会员不存在或已被移除.");

            if (!user.Enable)
                throw new ApplicationException("会员账号已禁用.");

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

            if (role == null)
                throw new ApplicationException("角色不存在或已被移除.");

            if (role.Type == RoleType.超级管理员)
                throw new ApplicationException($"{RoleType.超级管理员}角色无需进行相关授权操作.");

            if (!role.Enable)
                throw new ApplicationException("角色账号已禁用.");

            return role;
        }

        #endregion

        #region 外部接口

        #region 授权

        public void AutoAuthorizeRoleForUser(RoleForUser data, bool runTransaction = true)
        {
            if (data.UserIds.Any_Ex())
            {
                var roleIds = Repository_Role.Where(o => o.AutoAuthorizeRoleForUser == true && o.Enable == true).ToList(o => o.Id);
                if (roleIds.Any())
                    AuthorizeRoleForUser(new RoleForUser
                    {
                        UserIds = data.UserIds,
                        RoleIds = roleIds
                    }, runTransaction);
            }
            else if (data.RoleIds.Any_Ex())
            {
                var userIds = Repository_User.Select.ToList(o => o.Id);
                if (userIds.Any())
                    AuthorizeRoleForUser(new RoleForUser
                    {
                        UserIds = userIds,
                        RoleIds = data.RoleIds
                    }, runTransaction);
            }
            else
                throw new ApplicationException("参数不可为空.");
        }

        public void AutoAuthorizeRoleForMember(RoleForMember data, bool runTransaction = true)
        {
            if (data.MemberIds.Any_Ex())
            {
                var roleIds = Repository_Role.Where(o => o.AutoAuthorizeRoleForUser == true && o.Enable == true).ToList(o => o.Id);
                if (roleIds.Any())
                    AuthorizeRoleForMember(new RoleForMember
                    {
                        MemberIds = data.MemberIds,
                        RoleIds = roleIds
                    }, runTransaction);
            }
            else if (data.RoleIds.Any_Ex())
            {
                var memberIds = Repository_Member.Select.ToList(o => o.Id);
                if (memberIds.Any())
                    AuthorizeRoleForMember(new RoleForMember
                    {
                        MemberIds = memberIds,
                        RoleIds = data.RoleIds
                    }, runTransaction);
            }
            else
                throw new ApplicationException("参数不可为空.");
        }

        public void AuthorizeRoleForUser(RoleForUser data, bool runTransaction = true)
        {
            var users = data.UserIds.Select(o => GetUserWithCheck(o));

            var roles = Repository_Role.Where(o => (data.All == true || data.RoleIds.Contains(o.Id)) && o.Enable == true).ToList(o => new
            {
                o.Id,
                o.Type,
                o.Name
            });

            if (!roles.Any())
                throw new ApplicationException("没有可供授权的角色.");

            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_UserRole),
                    DataId = null,
                    Explain = $"授权角色给用户.",
                    Remark = $"被授权的用户: \r\n\t{string.Join(",", users.Select(o => $"[账号 {o.Account}, 姓名 {o.Name}]"))}\r\n" +
                             $"授权的角色: \r\n\t{string.Join(",", roles.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                });

                Repository_UserRole.Insert(roles.SelectMany(o => users.Select(p => new System_UserRole
                {
                    UserId = p.Id,
                    RoleId = o.Id
                })));

                if (data.RevocationOtherRoles)
                    data.UserIds.ForEach(o =>
                    {
                        var roleIds = roles.Select(p => p.Id);
                        RevocationRoleForUser(new RoleForUser
                        {
                            UserIds = new List<string> { o },
                            RoleIds = Repository_UserRole.Where(p => p.UserId == o && !roleIds.Contains(p.RoleId)).ToList(p => p.RoleId)
                        }, false);
                    });
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new ApplicationException("授权失败.", ex);
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
            var members = data.MemberIds.Select(o => GetMemberWithCheck(o));

            if (Repository_Role.Where(o => data.RoleIds.Contains(o.Id) && o.Type != $"{RoleType.会员}").Any())
                throw new ApplicationException($"只允许将类型为 {RoleType.会员}的角色授权给会员.");

            var roles = Repository_Role.Where(o => data.RoleIds.Contains(o.Id) && o.Enable == true).ToList(o => new
            {
                o.Id,
                o.Type,
                o.Name
            });

            if (!roles.Any())
                throw new ApplicationException("没有可供授权的角色.");

            if (roles.Any(o => o.Type != RoleType.会员))
                throw new ApplicationException($"只能将角色类型为{RoleType.会员}的角色授权给会员.");

            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(Public_MemberRole),
                    DataId = null,
                    Explain = $"授权角色给会员.",
                    Remark = $"被授权的会员: \r\n\t{string.Join(",", members.Select(o => $"[账号 {o.Account}, 姓名 {o.Name}]"))}\r\n" +
                            $"授权的角色: \r\n\t{string.Join(",", roles.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                });

                Repository_MemberRole.Insert(roles.SelectMany(o => members.Select(p => new Public_MemberRole
                {
                    MemberId = p.Id,
                    RoleId = o.Id
                })));

                if (data.RevocationOtherRoles)
                    data.MemberIds.ForEach(o =>
                    {
                        var roleIds = roles.Select(p => p.Id);
                        RevocationRoleForMember(new RoleForMember
                        {
                            MemberIds = new List<string> { o },
                            RoleIds = Repository_MemberRole.Where(p => p.MemberId == o && !roleIds.Contains(p.RoleId)).ToList(p => p.RoleId)
                        }, false);
                    });
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new ApplicationException("授权失败.", ex);
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
            var users = data.UserIds.Select(o => GetUserWithCheck(o));

            var menus = Repository_Menu.Where(o => (data.All == true || data.MenuIds.Contains(o.Id)) && o.Enable == true).ToList(o => new
            {
                o.Id,
                o.Type,
                o.Name
            });

            if (!menus.Any())
                throw new ApplicationException("没有可供授权的菜单.");

            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_UserMenu),
                    DataId = null,
                    Explain = $"授权菜单给用户.",
                    Remark = $"被授权的用户: \r\n\t{string.Join(",", users.Select(o => $"[账号 {o.Account}, 姓名 {o.Name}]"))}\r\n" +
                            $"授权的菜单: \r\n\t{string.Join(",", menus.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                });

                Repository_UserMenu.Insert(menus.SelectMany(o => users.Select(p => new System_UserMenu
                {
                    UserId = p.Id,
                    MenuId = o.Id
                })));
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new ApplicationException("授权失败.", ex);
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
            var users = data.UserIds.Select(o => GetUserWithCheck(o));

            var resources = Repository_Resources.Where(o => (data.All == true || data.ResourcesIds.Contains(o.Id)) && o.Enable == true).ToList(o => new
            {
                o.Id,
                o.Type,
                o.Name
            });

            if (!resources.Any())
                throw new ApplicationException("没有可供授权的资源.");

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_UserResources),
                    DataId = null,
                    Explain = $"授权资源给用户.",
                    Remark = $"被授权的用户: \r\n\t{string.Join(",", users.Select(o => $"[账号 {o.Account}, 姓名 {o.Name}]"))}\r\n" +
                            $"授权的资源: \r\n\t{string.Join(",", resources.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                });

                Repository_UserResources.Insert(resources.SelectMany(o => users.Select(p => new System_UserResources
                {
                    UserId = p.Id,
                    ResourcesId = o.Id
                })));
            });

            if (!success)
                throw new ApplicationException("授权失败.", ex);
        }

        public void AuthorizeMenuForRole(MenuForRole data, bool runTransaction = true)
        {
            var roles = data.RoleIds.Select(o => GetRoleWithCheck(o));

            var menus = Repository_Menu.Where(o => (data.All == true || data.MenuIds.Contains(o.Id)) && o.Enable == true).ToList(o => new
            {
                o.Id,
                o.Type,
                o.Name
            });

            if (!menus.Any())
                throw new ApplicationException("没有可供授权的菜单.");

            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_RoleMenu),
                    DataId = null,
                    Explain = $"授权菜单给角色.",
                    Remark = $"被授权的角色: \r\n\t{string.Join(",", roles.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}\r\n" +
                            $"授权的菜单: \r\n\t{string.Join(",", menus.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                });

                Repository_RoleMenu.Insert(menus.SelectMany(o => roles.Select(p => new System_RoleMenu
                {
                    RoleId = p.Id,
                    MenuId = o.Id
                })));
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new ApplicationException("授权失败.", ex);
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
            var roles = data.RoleIds.Select(o => GetRoleWithCheck(o));

            var resources = Repository_Resources.Where(o => (data.All == true || data.ResourcesIds.Contains(o.Id)) && o.Enable == true).ToList(o => new
            {
                o.Id,
                o.Type,
                o.Name
            });

            if (!resources.Any())
                throw new ApplicationException("没有可供授权的资源.");

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_RoleResources),
                    DataId = null,
                    Explain = $"授权资源给角色.",
                    Remark = $"被授权的角色: \r\n\t{string.Join(",", roles.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}\r\n" +
                            $"授权的资源: \r\n\t{string.Join(",", resources.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                });

                Repository_RoleResources.Insert(resources.SelectMany(o => roles.Select(p => new System_RoleResources
                {
                    RoleId = p.Id,
                    ResourcesId = o.Id
                })));
            });

            if (!success)
                throw new ApplicationException("授权失败.", ex);
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

        public void RevocationRoleForUser(RoleForUser data, bool runTransaction = true)
        {
            var users = data.UserIds.Select(o => GetUserWithCheck(o));

            var roles = Repository_UserRole.Where(o => data.UserIds.Contains(o.UserId) && (data.All == true || data.RoleIds.Contains(o.RoleId))).ToList(o => new
            {
                o.RoleId,
                o.Role.Type,
                o.Role.Name
            });

            if (!roles.Any())
                return;

            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_UserRole),
                    DataId = null,
                    Explain = $"撤销用户的角色授权.",
                    Remark = $"被撤销授权的用户: \r\n\t{string.Join(",", users.Select(o => $"[账号 {o.Account}, 姓名 {o.Name}]"))}\r\n" +
                            $"撤销授权的角色: \r\n\t{string.Join(",", roles.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                });

                var roleIds = roles.Select(o => o.RoleId);

                if (Repository_UserRole.Delete(o => data.UserIds.Contains(o.UserId) && (roleIds.Contains(o.RoleId))) < 0)
                    throw new ApplicationException("撤销授权失败.");
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new ApplicationException("撤销授权失败.", ex);
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
            var members = data.MemberIds.Select(o => GetMemberWithCheck(o));

            var roles = Repository_MemberRole.Where(o => data.MemberIds.Contains(o.MemberId) && (data.All == true || data.RoleIds.Contains(o.RoleId))).ToList(o => new
            {
                o.RoleId,
                o.Role.Type,
                o.Role.Name
            });

            if (!roles.Any())
                return;

            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(Public_MemberRole),
                    DataId = null,
                    Explain = $"撤销会员的角色授权.",
                    Remark = $"被撤销授权的会员: \r\n\t{string.Join(",", members.Select(o => $"[账号 {o.Account}, 姓名 {o.Name}]"))}\r\n" +
                            $"撤销授权的角色: \r\n\t{string.Join(",", roles.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                });

                var roleIds = roles.Select(o => o.RoleId);

                if (Repository_MemberRole.Delete(o => data.MemberIds.Contains(o.MemberId) && (roleIds.Contains(o.RoleId))) < 0)
                    throw new ApplicationException("撤销授权失败.");
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new ApplicationException("撤销授权失败.", ex);
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
            var users = data.UserIds.Select(o => GetUserWithCheck(o));

            var menus = Repository_UserMenu.Where(o => data.UserIds.Contains(o.UserId) && (data.All == true || data.MenuIds.Contains(o.MenuId))).ToList(o => new
            {
                o.MenuId,
                o.Menu.Type,
                o.Menu.Name
            });

            if (!menus.Any())
                return;

            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_UserMenu),
                    DataId = null,
                    Explain = $"撤销用户的菜单授权.",
                    Remark = $"被撤销授权的用户: \r\n\t{string.Join(",", users.Select(o => $"[账号 {o.Account}, 姓名 {o.Name}]"))}\r\n" +
                            $"撤销授权的菜单: \r\n\t{string.Join(",", menus.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                });

                var menuIds = menus.Select(o => o.MenuId);

                if (Repository_UserMenu.Delete(o => data.UserIds.Contains(o.UserId) && (menuIds.Contains(o.MenuId))) < 0)
                    throw new ApplicationException("撤销授权失败.");
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new ApplicationException("撤销授权失败.", ex);
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
            var users = data.UserIds.Select(o => GetUserWithCheck(o));

            var resourcess = Repository_UserResources.Where(o => data.UserIds.Contains(o.UserId) && (data.All == true || data.ResourcesIds.Contains(o.ResourcesId))).ToList(o => new
            {
                o.ResourcesId,
                o.Resources.Type,
                o.Resources.Name
            });

            if (!resourcess.Any())
                return;

            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_UserResources),
                    DataId = null,
                    Explain = $"撤销用户的资源授权.",
                    Remark = $"被撤销授权的用户: \r\n\t{string.Join(",", users.Select(o => $"[账号 {o.Account}, 姓名 {o.Name}]"))}\r\n" +
                            $"撤销授权的资源: \r\n\t{string.Join(",", resourcess.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                });

                var resourcesIds = resourcess.Select(o => o.ResourcesId);

                if (Repository_UserResources.Delete(o => data.UserIds.Contains(o.UserId) && (resourcesIds.Contains(o.ResourcesId))) < 0)
                    throw new ApplicationException("撤销授权失败.");
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new ApplicationException("撤销授权失败.", ex);
            }
            else
                handler();
        }

        public void RevocationMenuForRole(MenuForRole data, bool runTransaction = true)
        {
            var roles = data.RoleIds.Select(o => GetRoleWithCheck(o));

            var menus = Repository_RoleMenu.Where(o => data.RoleIds.Contains(o.RoleId) && (data.All == true || data.MenuIds.Contains(o.MenuId))).ToList(o => new
            {
                o.MenuId,
                o.Menu.Type,
                o.Menu.Name
            });

            if (!menus.Any())
                return;

            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_RoleMenu),
                    DataId = null,
                    Explain = $"撤销角色的菜单授权.",
                    Remark = $"被撤销授权的角色: \r\n\t{string.Join(",", roles.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}\r\n" +
                            $"撤销授权的菜单: \r\n\t{string.Join(",", menus.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                });

                var menuIds = menus.Select(o => o.MenuId);

                if (Repository_RoleMenu.Delete(o => data.RoleIds.Contains(o.RoleId) && (menuIds.Contains(o.MenuId))) < 0)
                    throw new ApplicationException("撤销授权失败.");
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new ApplicationException("撤销授权失败.", ex);
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
            var roles = data.RoleIds.Select(o => GetRoleWithCheck(o));

            var resourcess = Repository_RoleResources.Where(o => data.RoleIds.Contains(o.RoleId) && (data.All == true || data.ResourcesIds.Contains(o.ResourcesId))).ToList(o => new
            {
                o.ResourcesId,
                o.Resources.Type,
                o.Resources.Name
            });

            if (!resourcess.Any())
                return;

            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_RoleResources),
                    DataId = null,
                    Explain = $"撤销角色的资源授权.",
                    Remark = $"被撤销授权的角色: \r\n\t{string.Join(",", roles.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}\r\n" +
                            $"撤销授权的资源: \r\n\t{string.Join(",", resourcess.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                });

                var resourcesIds = resourcess.Select(o => o.ResourcesId);

                if (Repository_RoleResources.Delete(o => data.RoleIds.Contains(o.RoleId) && (resourcesIds.Contains(o.ResourcesId))) < 0)
                    throw new ApplicationException("撤销授权失败.");
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new ApplicationException("撤销授权失败.", ex);
            }
            else
                handler();
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
                    throw new ApplicationException("撤销用户的菜单授权失败.");

                if (Repository_RoleMenu.Delete(o => (menuIds.Contains(o.MenuId))) < 0)
                    throw new ApplicationException("撤销角色的菜单授权失败.");
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new ApplicationException("撤销授权失败.", ex);
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
                    throw new ApplicationException("撤销用户的资源授权失败.");

                if (Repository_RoleResources.Delete(o => (resourcesIds.Contains(o.ResourcesId))) < 0)
                    throw new ApplicationException("撤销角色的资源授权失败.");
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new ApplicationException("撤销授权失败.", ex);
            }
            else
                handler();
        }

        #endregion

        #region 获取授权

        public Model.System.UserDTO.Authorities GetUser(string userId, bool includeRole, bool includeMenu, bool includeResources, bool mergeRoleMenu = true, bool mergeRoleResources = true)
        {
            var user = Repository_User.Where(o => o.Id == userId)
                                    .ToOne(o => new Model.System.UserDTO.Authorities
                                    {
                                        Id = o.Id,
                                        Account = o.Account,
                                        Enable = o.Enable
                                    });

            if (user == null)
                throw new ApplicationException("用户不存在或已被移除.");

            if (!user.Enable)
                throw new ApplicationException("用户账号已禁用.");

            if (includeRole)
                user._Roles = GetUserRole(userId, includeMenu && !mergeRoleMenu, includeResources && !mergeRoleResources);

            if (includeMenu)
                user._Menus = GetUserMenu(userId, mergeRoleMenu);

            if (includeResources)
                user._Resources = GetUserResources(userId, mergeRoleResources);

            return user;
        }

        public Model.Public.MemberDTO.Authorities GetMember(string memberId, bool includeRole, bool includeMenu, bool includeResources)
        {
            var member = Repository_Member.Where(o => o.Id == memberId)
                                    .ToOne(o => new Model.Public.MemberDTO.Authorities
                                    {
                                        Id = o.Id,
                                        Account = o.Account,
                                        Enable = o.Enable
                                    });

            if (member == null)
                throw new ApplicationException("会员用户不存在或已被移除.");

            if (!member.Enable)
                throw new ApplicationException("会员账号已禁用.");

            if (includeRole)
                member._Roles = GetMemberRole(memberId, includeMenu, includeResources);

            return member;
        }

        public List<Model.System.RoleDTO.Authorities> GetUserRole(string userId, bool includeMenu, bool includeResources)
        {
            var roles = Repository_Role.Where(o => o.Users.AsSelect().Where(p => p.Id == userId).Any() && o.Enable == true)
                                         .ToList(o => new Model.System.RoleDTO.Authorities
                                         {
                                             Id = o.Id,
                                             Name = o.Name,
                                             Type = o.Type,
                                             Code = o.Code
                                         });

            if (includeMenu || includeResources)
                roles.ForEach(o =>
                {
                    if (includeMenu)
                        o._Menus = GetRoleMenu(o.Id);

                    if (includeResources)
                        o._Resources = GetRoleResources(o.Id);
                });

            return roles;
        }

        public List<Model.System.RoleDTO.Authorities> GetMemberRole(string memberId, bool includeMenu, bool includeResources)
        {
            var roles = Repository_Role.Where(o => o.Members.AsSelect().Where(p => p.Id == memberId).Any() && o.Enable == true)
                                         .ToList(o => new Model.System.RoleDTO.Authorities
                                         {
                                             Id = o.Id,
                                             Name = o.Name,
                                             Type = o.Type,
                                             Code = o.Code
                                         });

            if (includeMenu || includeResources)
                roles.ForEach(o =>
                {
                    if (includeMenu)
                        o._Menus = GetRoleMenu(o.Id);

                    if (includeResources)
                        o._Resources = GetRoleResources(o.Id);
                });

            return roles;
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

        public List<Model.System.ResourcesDTO.Authorities> GetUserResources(string userId, bool mergeRoleResources)
        {
            var resources = Repository_Resources.Where(o => (o.Users.AsSelect()
                                                            .Where(p => p.Id == userId)
                                                            .Any()
                                                    || (mergeRoleResources == true
                                                        && o.Roles.AsSelect()
                                                                .Where(p => p.Users.AsSelect()
                                                                                .Where(q => q.Id == userId)
                                                                                .Any() && p.Enable == true)
                                                                .Any())) && o.Enable == true)
                                    .ToList(o => new Model.System.ResourcesDTO.Authorities
                                    {
                                        Id = o.Id,
                                        Name = o.Name,
                                        Type = o.Type,
                                        Code = o.Code,
                                        Uri = o.Uri
                                    });

            return resources;
        }

        public List<Model.System.ResourcesDTO.Authorities> GetMemberResources(string memberId)
        {
            var resources = Repository_Resources.Where(o => o.Roles.AsSelect()
                                                        .Where(p => p.Members.AsSelect()
                                                                            .Where(q => q.Id == memberId)
                                                                            .Any() && p.Enable == true)
                                                        .Any() && o.Enable == true)
                                    .ToList(o => new Model.System.ResourcesDTO.Authorities
                                    {
                                        Id = o.Id,
                                        Name = o.Name,
                                        Type = o.Type,
                                        Code = o.Code,
                                        Uri = o.Uri
                                    });

            return resources;
        }

        public Model.System.RoleDTO.Authorities GetRole(string roleId, bool includeMenu, bool includeResources)
        {
            var role = Repository_Role.Where(o => o.Id == roleId)
                                    .ToOne(o => new Model.System.RoleDTO.Authorities
                                    {
                                        Id = o.Id,
                                        Name = o.Name,
                                        Type = o.Type,
                                        Code = o.Code
                                    });

            if (role == null)
                throw new ApplicationException("角色不存在或已被移除.");

            if (!role.Enable)
                throw new ApplicationException("角色已禁用.");

            if (includeMenu)
                role._Menus = GetRoleMenu(role.Id);

            if (includeResources)
                role._Resources = GetRoleResources(role.Id);

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

        public List<Model.System.ResourcesDTO.Authorities> GetRoleResources(string roleId)
        {
            return Repository_Resources.Where(o => o.Roles.AsSelect()
                                            .Where(p => p.Id == roleId)
                                            .Any() && o.Enable == true)
                                .ToList(o => new Model.System.ResourcesDTO.Authorities
                                {
                                    Id = o.Id,
                                    Name = o.Name,
                                    Type = o.Type,
                                    Code = o.Code,
                                    Uri = o.Uri
                                });
        }

        #endregion

        #region 验证授权

        public bool IsSuperAdminUser(string userId, bool checkEnable = true)
        {
            return Repository_UserRole.Where(o => o.UserId == userId && o.Role.Type == RoleType.超级管理员 && (checkEnable == false || o.Role.Enable == true)).Any();
        }

        public bool IsSuperAdminRole(string roleId, bool checkEnable = true)
        {
            return Repository_Role.Where(o => o.Id == roleId && o.Type == RoleType.超级管理员 && (checkEnable == false || o.Enable == true)).Any();
        }

        public bool IsAdminUser(string userId, bool checkEnable = true)
        {
            return Repository_UserRole.Where(o => o.UserId == userId && (o.Role.Type == RoleType.超级管理员 || o.Role.Type == RoleType.管理员) && (checkEnable == false || o.Role.Enable == true)).Any();
        }

        public bool IsAdminRole(string roleId, bool checkEnable = true)
        {
            return Repository_Role.Where(o => o.Id == roleId && (o.Type == RoleType.超级管理员 || o.Type == RoleType.管理员) && (checkEnable == false || o.Enable == true)).Any();
        }

        public bool UserHasRole(string userId, string roleId, bool checkEnable = true)
        {
            return Repository_UserRole.Where(o => o.UserId == userId && o.RoleId == roleId && (checkEnable == false || o.Role.Enable == true)).Any();
        }

        public bool MemberHasRole(string memberId, string roleId, bool checkEnable = true)
        {
            return Repository_MemberRole.Where(o => o.MemberId == memberId && o.RoleId == roleId && (checkEnable == false || o.Role.Enable == true)).Any();
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
            return Repository_Menu.Where(o => o.Uri == menuUri && (checkEnable == false || o.Enable == true)
                                            && (o.Users.AsSelect().Where(p => p.Id == userId).Any()
                                                || o.Roles.AsSelect().Where(p => p.Users.AsSelect().Where(q => q.Id == userId).Any()).Any()))
                                .Any();
        }

        public bool MemberHasMenu(string userId, string menuId, bool checkEnable = true)
        {
            return Repository_Menu.Where(o => o.Id == menuId && (checkEnable == false || o.Enable == true)
                                            && o.Roles.AsSelect().Where(p => p.Users.AsSelect().Where(q => q.Id == userId).Any()).Any())
                                .Any();
        }

        public bool MemberHasMenuUri(string userId, string menuUri, bool checkEnable = true)
        {
            return Repository_Menu.Where(o => o.Uri == menuUri && (checkEnable == false || o.Enable == true)
                                            && o.Roles.AsSelect().Where(p => p.Users.AsSelect().Where(q => q.Id == userId).Any()).Any())
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
            return Repository_Resources.Where(o => o.Uri == resourcesUri && (checkEnable == false || o.Enable == true)
                                            && (o.Users.AsSelect().Where(p => p.Id == userId).Any()
                                                || o.Roles.AsSelect().Where(p => p.Users.AsSelect().Where(q => q.Id == userId).Any()).Any()))
                                .Any();
        }

        public bool MemberHasResources(string userId, string resourcesId, bool checkEnable = true)
        {
            return Repository_Resources.Where(o => o.Id == resourcesId && (checkEnable == false || o.Enable == true)
                                            && o.Roles.AsSelect().Where(p => p.Users.AsSelect().Where(q => q.Id == userId).Any()).Any())
                                .Any();
        }

        public bool MemberHasResourcesUri(string userId, string resourcesUri, bool checkEnable = true)
        {
            return Repository_Resources.Where(o => o.Uri == resourcesUri && (checkEnable == false || o.Enable == true)
                                            && o.Roles.AsSelect().Where(p => p.Users.AsSelect().Where(q => q.Id == userId).Any()).Any())
                                .Any();
        }

        #endregion

        #region 拓展方法

        public List<Model.System.MenuDTO.AuthoritiesTree> GetRoleMenuTree(string roleId, Model.System.MenuDTO.TreeListParamter paramter)
        {
            paramter.MenuType.RemoveAll(o => o.IsNullOrWhiteSpace());
            return GetRoleMenuTree(paramter);

            List<Model.System.MenuDTO.AuthoritiesTree> GetRoleMenuTree(Model.System.MenuDTO.TreeListParamter paramter, bool deep = false)
            {
                if (paramter.ParentId.IsNullOrWhiteSpace())
                    paramter.ParentId = null;

                var result = Repository_Menu.Select
                                        .Where(o => o.ParentId == paramter.ParentId
                                            && (paramter.MenuType.Count() == 0 || paramter.MenuType.Contains(o.Type))
                                            && (Operator.IsSuperAdmin == true
                                                || o.Users.AsSelect().Any(p => p.Id == Operator.AuthenticationInfo.Id)
                                                || o.Roles.AsSelect().Any(p => p.Users.AsSelect().Any(q => q.Id == Operator.AuthenticationInfo.Id))))
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
                        var rank = paramter.Rank;
                        if (!rank.HasValue || rank > 0)
                        {
                            paramter.ParentId = o.Id;
                            paramter.Rank--;
                            o.Childs_ = GetRoleMenuTree(paramter, true);
                        }
                    });
                else if (deep)
                    result = null;

                return result;
            }
        }

        public List<Model.System.RoleDTO.AuthoritiesTree> GetUserRoleTree(string userId, Model.System.RoleDTO.TreeListParamter paramter)
        {
            paramter.RoleType.RemoveAll(o => o.IsNullOrWhiteSpace() || o == RoleType.会员);

            return GetUserRoleTree(paramter);

            List<Model.System.RoleDTO.AuthoritiesTree> GetUserRoleTree(Model.System.RoleDTO.TreeListParamter paramter, bool deep = false)
            {
                if (paramter.ParentId.IsNullOrWhiteSpace())
                    paramter.ParentId = null;

                var result = Repository_Role.Select
                                        .Where(o => o.ParentId == paramter.ParentId
                                            && (paramter.RoleType.Count() == 0 || paramter.RoleType.Contains(o.Type))
                                            && (Operator.IsSuperAdmin == true
                                                || o.Users.AsSelect().Any(p => p.Id == Operator.AuthenticationInfo.Id)))
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
                        var rank = paramter.Rank;
                        if (!rank.HasValue || rank > 0)
                        {
                            paramter.ParentId = o.Id;
                            paramter.Rank--;
                            o.Childs_ = GetUserRoleTree(paramter, true);
                        }
                    });
                else if (deep)
                    result = null;

                return result;
            }
        }

        public List<Model.System.RoleDTO.AuthoritiesTree> GetMemberRoleTree(string memberId, Model.System.RoleDTO.TreeListParamter paramter)
        {
            paramter.RoleType.Clear();
            paramter.RoleType.Add(RoleType.会员);
            return GetMemberRoleTree(paramter);

            List<Model.System.RoleDTO.AuthoritiesTree> GetMemberRoleTree(Model.System.RoleDTO.TreeListParamter paramter, bool deep = false)
            {
                if (paramter.ParentId.IsNullOrWhiteSpace())
                    paramter.ParentId = null;

                var result = Repository_Role.Select
                                        .Where(o => o.ParentId == paramter.ParentId
                                            && (paramter.RoleType.Count() == 0 || paramter.RoleType.Contains(o.Type))
                                            && (Operator.IsSuperAdmin == true
                                                || o.Users.AsSelect().Any(p => p.Id == Operator.AuthenticationInfo.Id)))
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
                        var rank = paramter.Rank;
                        if (!rank.HasValue || rank > 0)
                        {
                            paramter.ParentId = o.Id;
                            paramter.Rank--;
                            o.Childs_ = GetMemberRoleTree(paramter, true);
                        }
                    });
                else if (deep)
                    result = null;

                return result;
            }
        }

        public List<Model.System.MenuDTO.AuthoritiesTree> GetUserMenuTree(string userId, Model.System.MenuDTO.TreeListParamter paramter)
        {
            paramter.MenuType.RemoveAll(o => o.IsNullOrWhiteSpace());
            return GetUserMenuTree(paramter);

            List<Model.System.MenuDTO.AuthoritiesTree> GetUserMenuTree(Model.System.MenuDTO.TreeListParamter paramter, bool deep = false)
            {
                if (paramter.ParentId.IsNullOrWhiteSpace())
                    paramter.ParentId = null;

                var result = Repository_Menu.Select
                                        .Where(o => o.ParentId == paramter.ParentId
                                            && (paramter.MenuType.Count() == 0 || paramter.MenuType.Contains(o.Type))
                                            && (Operator.IsSuperAdmin == true
                                                || o.Users.AsSelect().Any(p => p.Id == Operator.AuthenticationInfo.Id)
                                                || o.Roles.AsSelect().Any(p => p.Users.AsSelect().Any(q => q.Id == Operator.AuthenticationInfo.Id))))
                                        .ToList(o => new Model.System.MenuDTO.AuthoritiesTree
                                        {
                                            Id = o.Id,
                                            Name = o.Name,
                                            Type = o.Type,
                                            Code = o.Code,
                                            Uri = o.Uri,
                                            Method = o.Method,
                                            Authorized = o.Users.AsSelect().Any(p => p.Id == userId)
                                                || o.Roles.AsSelect().Any(p => p.Users.AsSelect().Any(q => q.Id == userId)) ? true : false
                                        });

                if (result.Any())
                    result.ForEach(o =>
                    {
                        var rank = paramter.Rank;
                        if (!rank.HasValue || rank > 0)
                        {
                            paramter.ParentId = o.Id;
                            paramter.Rank--;
                            o.Childs_ = GetUserMenuTree(paramter, true);
                        }
                    });
                else if (deep)
                    result = null;

                return result;
            }
        }

        #endregion

        #endregion
    }
}
