﻿using AutoMapper;
using Business.Interface.Common;
using Business.Interface.System;
using Business.Utils;
using Business.Utils.Filter;
using Business.Utils.Pagination;
using Entity.Common;
using Entity.System;
using FreeSql;
using Microservice.Library.DataMapping.Gen;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using Microservice.Library.OpenApi.Extention;
using Model.System;
using Model.System.RoleDTO;
using Model.Utils.Pagination;
using Model.Utils.Sort;
using Model.Utils.Sort.SortParamsDTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Implementation.System
{
    /// <summary>
    /// 角色业务类
    /// </summary>
    public class RoleBusiness : BaseBusiness, IRoleBusiness
    {
        #region DI

        public RoleBusiness(
            IFreeSqlProvider freeSqlProvider,
            IAutoMapperProvider autoMapperProvider,
            IOperationRecordBusiness operationRecordBusiness,
            IAuthoritiesBusiness authoritiesBusiness)
        {
            Orm = freeSqlProvider.GetFreeSql();
            Repository = Orm.GetRepository<System_Role, string>();
            Repository_RoleMenu = Orm.GetRepository<System_RoleMenu, string>();
            Repository_RoleResources = Orm.GetRepository<System_RoleResources, string>();
            Mapper = autoMapperProvider.GetMapper();
            OperationRecordBusiness = operationRecordBusiness;
            AuthoritiesBusiness = authoritiesBusiness;
        }

        #endregion

        #region 私有成员

        IFreeSql Orm { get; set; }

        IBaseRepository<System_Role, string> Repository { get; set; }

        IBaseRepository<System_RoleMenu, string> Repository_RoleMenu { get; set; }

        IBaseRepository<System_RoleResources, string> Repository_RoleResources { get; set; }

        IMapper Mapper { get; set; }

        IOperationRecordBusiness OperationRecordBusiness { get; set; }

        IAuthoritiesBusiness AuthoritiesBusiness { get; set; }

        #endregion

        #region 外部接口

        #region 基础功能

        public List<List> GetList(PaginationDTO pagination)
        {
            var entityList = Repository.Select
                                    .GetPagination(pagination)
                                    .ToList<System_Role, List>(typeof(List).GetNamesWithTagAndOther(true, "_List"));

            var result = Mapper.Map<List<List>>(entityList);

            return result;
        }

        public List<TreeList> GetTreeList(TreePaginationDTO pagination)
        {
            return GetData(pagination, false);

            List<TreeList> GetData(TreePaginationDTO pagination, bool deep)
            {
                if (pagination.ParentId.IsNullOrWhiteSpace())
                    pagination.ParentId = null;

                var entityList = Repository.Select
                                        .Where(o => o.ParentId == pagination.ParentId)
                                        .GetPagination(pagination.Pagination)
                                        .ToList<System_Role, TreeList>(typeof(TreeList).GetNamesWithTagAndOther(true, "_List"));

                var result = Mapper.Map<List<TreeList>>(entityList);

                if (result.Any())
                    result.ForEach(o =>
                    {
                        var rank = pagination.Rank;
                        if (!rank.HasValue || rank > 0)
                        {
                            pagination.ParentId = o.Id;
                            pagination.Rank--;
                            o.Children = GetData(pagination, true);
                            o.HasChildren = o.Children.Any_Ex();
                            o.ChildrenCount = o.Children?.Count ?? 0;
                        }
                        else
                        {
                            o.HasChildren = Repository.Select.Where(p => p.ParentId == o.Id).Any();
                            o.ChildrenCount = (int)Repository.Where(p => p.ParentId == o.Id).Count();
                        }
                    });
                else if (deep)
                    result = null;

                return result;
            }
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
            var newData = Mapper.Map<System_Role>(data).InitEntity();

            if (newData.Type == RoleType.会员 && newData.AutoAuthorizeRoleForUser)
                throw new MessageException($"类型为{RoleType.会员}的角色禁止配置[自动授权角色给系统用户]设置.");
            else if (newData.Type != RoleType.会员 && newData.AutoAuthorizeRoleForMember)
                throw new MessageException($"类型不为{RoleType.会员}的角色禁止配置[自动授权角色给会员]设置.");

            if (newData.Type == RoleType.超级管理员 && Repository.Where(o => o.Type == $"{RoleType.超级管理员}").Any())
                throw new MessageException($"系统中已存在{RoleType.超级管理员}角色.");

            if (Repository.Where(o => o.ParentId == newData.ParentId && o.Code == newData.Code).Any())
                throw new MessageException($"当前层级已存在编码为{newData.Code}的角色.");

            if (Repository.Where(o => o.ParentId == newData.ParentId && o.Type == newData.Type && o.Name == newData.Name).Any())
                throw new MessageException($"当前层级已存在类型为{newData.Type},且名称为{newData.Name}的角色.");

            void handler()
            {
                if (newData.ParentId.IsNullOrWhiteSpace())
                {
                    newData.ParentId = null;
                    newData.Level = 0;
                    newData.RootId = null;
                }
                else
                {
                    var parent = Repository.GetAndCheckNull(newData.ParentId);
                    newData.Level = parent.Level + 1;
                    newData.RootId = parent.RootId == null ? parent.Id : parent.RootId;
                }

                newData.Sort = Repository.Where(o => o.ParentId == newData.ParentId).Max(o => o.Sort) + 1;

                Repository.Insert(newData);

                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_Role),
                    DataId = newData.Id,
                    Explain = $"创建角色[名称 {newData.Name}, 类型 {newData.Type}]."
                });

                if (newData.AutoAuthorizeRoleForUser)
                    AuthoritiesBusiness.AutoAuthorizeRoleForUser(new Model.System.AuthorizeDTO.RoleForUser
                    {
                        RoleIds = new List<string> { newData.Id }
                    }, false);
                if (newData.AutoAuthorizeRoleForMember)
                    AuthoritiesBusiness.AutoAuthorizeRoleForMember(new Model.System.AuthorizeDTO.RoleForMember
                    {
                        RoleIds = new List<string> { newData.Id }
                    }, false);
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("创建角色失败", ex);
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
        public void Edit(Edit data)
        {
            var editData = Mapper.Map<System_Role>(data).ModifyEntity();

            if (editData.Type == RoleType.会员 && editData.AutoAuthorizeRoleForUser)
                throw new MessageException($"类型为{RoleType.会员}的角色禁止配置[自动授权角色给系统用户]设置.");
            else if (editData.Type != RoleType.会员 && editData.AutoAuthorizeRoleForMember)
                throw new MessageException($"类型不为{RoleType.会员}的角色禁止配置[自动授权角色给会员]设置.");

            if (Repository.Where(o => o.ParentId == editData.ParentId && o.Code == editData.Code && o.Id != editData.Id).Any())
                throw new MessageException($"当前层级已存在编码为{editData.Code}的角色.");

            if (Repository.Where(o => o.ParentId == editData.ParentId && o.Type == editData.Type && o.Name == editData.Name && o.Id != editData.Id).Any())
                throw new MessageException($"当前层级已存在类型为{editData.Type},且名称为{editData.Name}的角色.");

            var entity = Repository.GetAndCheckNull(editData.Id);

            if (editData.Type == RoleType.超级管理员)
            {
                if (editData.ParentId != entity.ParentId)
                    throw new MessageException($"禁止编辑此{RoleType.超级管理员}角色的层级结构.");
                else if (editData.Code != entity.Code)
                    throw new MessageException($"禁止编辑此{RoleType.超级管理员}角色的编码信息.");
            }

            var changed_ = string.Join(",",
                                       entity.GetPropertyValueChangeds<System_Role, Edit>(editData)
                                            .Select(p => $"\r\n\t {p.Description}：{p.FormerValue} 更改为 {p.CurrentValue}"));

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                if (editData.ParentId != entity.ParentId)
                {
                    if (editData.ParentId.IsNullOrWhiteSpace())
                    {
                        editData.ParentId = null;
                        editData.Level = 0;
                        editData.RootId = null;
                    }
                    else
                    {
                        var parent = Repository.GetAndCheckNull(editData.ParentId);
                        editData.Level = parent.Level + 1;
                        editData.RootId = parent.RootId == null ? parent.Id : parent.RootId;
                    }

                    if (Repository.UpdateDiy
                             .Where(o => o.ParentId == entity.ParentId && o.Id != entity.Id && o.Sort > editData.Sort)
                             .Set(o => o.Sort - 1)
                             .ExecuteAffrows() < 0)
                        throw new MessageException("重新排序失败.");

                    editData.Sort = Repository.Where(o => o.ParentId == editData.ParentId).Max(o => o.Sort) + 1;
                }

                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_Role),
                    DataId = entity.Id,
                    Explain = $"修改角色[名称 {entity.Name}, 类型 {entity.Type}].",
                    Remark = $"变更详情: \r\n\t{changed_}"
                });

                if (Repository.UpdateDiy
                      .SetSource(editData)
                      .UpdateColumns(typeof(Edit).GetNamesWithTagAndOther(false, "_Edit").ToArray())
                      .ExecuteAffrows() <= 0)
                    throw new MessageException("修改角色失败");

                if (!entity.AutoAuthorizeRoleForUser && editData.AutoAuthorizeRoleForUser)
                    AuthoritiesBusiness.AutoAuthorizeRoleForUser(new Model.System.AuthorizeDTO.RoleForUser
                    {
                        RoleIds = new List<string> { editData.Id }
                    }, false);
                else if (entity.AutoAuthorizeRoleForUser && !editData.AutoAuthorizeRoleForUser)
                    AuthoritiesBusiness.RevocationRoleForAllUser(new List<string>
                    {
                        editData.Id
                    }, false);

                if (!entity.AutoAuthorizeRoleForMember && editData.AutoAuthorizeRoleForMember)
                    AuthoritiesBusiness.AutoAuthorizeRoleForMember(new Model.System.AuthorizeDTO.RoleForMember
                    {
                        RoleIds = new List<string> { editData.Id }
                    }, false);
                else if (entity.AutoAuthorizeRoleForMember && !editData.AutoAuthorizeRoleForMember)
                    AuthoritiesBusiness.RevocationRoleForAllMember(new List<string>
                    {
                        editData.Id
                    }, false);
            });

            if (!success)
                throw ex;
        }

        [AdministratorOnly]
        public void Delete(List<string> ids)
        {
            if (Repository.Select.Where(c => ids.Contains(c.Id) && c.Type == $"RoleType.超级管理员").Any())
                throw new MessageException($"禁止删除{RoleType.超级管理员}角色.");

            var entityList = Repository.Select.Where(c => ids.Contains(c.Id)).ToList(c => new { c.Id, c.Name, c.Type });

            var orList = new List<Common_OperationRecord>();

            foreach (var entity in entityList)
            {
                orList.Add(new Common_OperationRecord
                {
                    DataType = nameof(System_Role),
                    DataId = entity.Id,
                    Explain = $"删除角色[名称 {entity.Name}, 类型 {entity.Type}]."
                });
            }

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                AuthoritiesBusiness.RevocationRoleForAllUser(ids, false);

                AuthoritiesBusiness.RevocationRoleForAllMember(ids, false);

                AuthoritiesBusiness.RevocationMenuForRole(ids, false);

                AuthoritiesBusiness.RevocationResourcesForRole(ids, false);

                AuthoritiesBusiness.RevocationCFUCForRole(ids, false);

                if (Repository.Delete(o => ids.Contains(o.Id)) <= 0)
                    throw new MessageException("未删除任何数据");

                var orIds = OperationRecordBusiness.Create(orList);
            });

            if (!success)
                throw new MessageException("删除角色失败", ex);
        }

        #endregion

        #region 拓展功能

        [AdministratorOnly]
        public void Enable(string id, bool enable)
        {
            var entity = Repository.GetAndCheckNull(id);

            if (entity.Type == RoleType.超级管理员)
                throw new MessageException($"禁止操作{RoleType.超级管理员}角色.");

            entity.Enable = enable;

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_Role),
                    DataId = entity.Id,
                    Explain = $"{(enable ? "启用" : "禁用")}角色[名称 {entity.Name}, 类型 {entity.Type}]."
                });

                if (Repository.Update(entity) <= 0)
                    throw new MessageException($"{(enable ? "启用" : "禁用")}角色失败");
            });

            if (!success)
                throw ex;
        }

        [AdministratorOnly]
        public void Sort(Sort data)
        {
            if (data.Span == 0 && (data.Method != SortMethod.top || data.Method != SortMethod.low))
                return;

            var current = Repository.Where(o => o.Id == data.Id)
                                    .ToOne(o => new
                                    {
                                        o.Id,
                                        o.ParentId,
                                        o.Name,
                                        o.Type,
                                        o.Sort
                                    });

            if (current == default)
                throw new MessageException("数据不存在或已被移除.");

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var target = data.Method switch
                {
                    SortMethod.top => Repository.Where(o => o.ParentId == current.ParentId)
                                                                .OrderBy(o => o.Sort)
                                                                .First(o => new
                                                                {
                                                                    o.Id,
                                                                    o.Sort
                                                                }),
                    SortMethod.up => Repository.Where(o => o.ParentId == current.ParentId && o.Sort == current.Sort - 1)
                 .First(o => new
                 {
                     o.Id,
                     o.Sort
                 }),
                    SortMethod.down => Repository.Where(o => o.ParentId == current.ParentId && o.Sort == current.Sort + 1)
                 .First(o => new
                 {
                     o.Id,
                     o.Sort
                 }),
                    SortMethod.low => Repository.Where(o => o.ParentId == current.ParentId)
                 .OrderByDescending(o => o.Sort)
                 .First(o => new
                 {
                     o.Id,
                     o.Sort
                 }),
                    _ => throw new MessageException($"不支持的排序方法 {data.Method}."),
                };

                if (target == default)
                    return;

                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_Role),
                    DataId = current.Id,
                    Explain = $"角色排序[名称 {current.Name}, 类型 {current.Type}]."
                });

                if (Repository.UpdateDiy
                         .Where(o => o.Id == target.Id)
                         .Set(o => o.Sort, current.Sort)
                         .ExecuteAffrows() < 0
                    || Repository.UpdateDiy
                         .Where(o => o.Id == current.Id)
                         .Set(o => o.Sort, target.Sort)
                         .ExecuteAffrows() < 0)
                    throw new MessageException("角色排序失败.");
            });

            if (!success)
                throw ex;
        }

        [AdministratorOnly]
        public void DragSort(TreeDragSort data)
        {
            if (data.Id == data.TargetId)
                return;

            var current = Repository.Where(o => o.Id == data.Id)
                                    .ToOne(o => new
                                    {
                                        o.Id,
                                        o.ParentId,
                                        o.Name,
                                        o.Type,
                                        o.Sort
                                    });

            if (current == default)
                throw new MessageException("数据不存在或已被移除.");

            var target = Repository.Where(o => o.Id == data.TargetId)
                                    .ToOne(o => new
                                    {
                                        o.Id,
                                        o.ParentId,
                                        o.Sort,
                                        o.Level,
                                        o.RootId
                                    });

            if (target == default)
                throw new MessageException("目标数据不存在.");

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                if (current.ParentId == target.ParentId)
                {
                    #region 同层级排序

                    dynamic target_new;

                    if (data.Append)
                    {
                        target_new = Repository.Where(o => o.ParentId == target.ParentId && o.Sort < target.Sort)
                                            .OrderByDescending(o => o.Sort)
                                             .First(o => new
                                             {
                                                 o.Id,
                                                 o.Sort
                                             });
                    }
                    else
                    {
                        target_new = Repository.Where(o => o.ParentId == target.ParentId && o.Sort > current.Sort)
                                            .OrderBy(o => o.Sort)
                                             .First(o => new
                                             {
                                                 o.Id,
                                                 o.Sort
                                             });
                    }

                    string target_newId = target_new.Id;
                    int target_newSort = target_new.Sort;

                    if (Repository.UpdateDiy
                             .Where(o => o.Id == target_newId)
                             .Set(o => o.Sort, current.Sort)
                             .ExecuteAffrows() < 0
                        || Repository.UpdateDiy
                             .Where(o => o.Id == current.Id)
                             .Set(o => o.Sort, target_newSort)
                             .ExecuteAffrows() < 0)
                        throw new MessageException("角色排序失败.");

                    #endregion
                }
                else
                {
                    #region 异层级排序

                    if (Repository.UpdateDiy
                             .Where(o => o.ParentId == current.ParentId && o.Sort > current.Sort)
                             .Set(o => o.Sort - 1)
                             .ExecuteAffrows() < 0
                        || Repository.UpdateDiy
                             .Where(o => o.ParentId == target.ParentId && o.Sort > (data.Append == true ? target.Sort : (target.Sort - 1)))
                             .Set(o => o.Sort + 1)
                             .ExecuteAffrows() < 0
                        || Repository.UpdateDiy
                                .Where(o => o.Id == current.Id)
                                .Set(o => o.Sort, data.Inside == true ? 0 : (data.Append == true ? (target.Sort + 1) : target.Sort))
                                .Set(o => o.ParentId, data.Inside == true ? target.Id : target.ParentId)
                                .Set(o => o.Level, data.Inside == true ? (target.Level + 1) : target.Level)
                                .Set(o => o.RootId, target.RootId)
                                .ExecuteAffrows() <= 0)
                        throw new MessageException("角色排序失败.");

                    #endregion
                }

                _ = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_Role),
                    DataId = current.Id,
                    Explain = $"角色排序[名称 {current.Name}, 类型 {current.Type}]."
                });
            });

            if (!success)
                throw ex;
        }

        #endregion

        #endregion
    }
}
