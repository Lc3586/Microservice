using AutoMapper;
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
using Model.System.MenuDTO;
using Model.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Implementation.System
{
    /// <summary>
    /// 菜单业务类
    /// </summary>
    public class MenuBusiness : BaseBusiness, IMenuBusiness
    {
        #region DI

        public MenuBusiness(
            IFreeSqlProvider freeSqlProvider,
            IAutoMapperProvider autoMapperProvider,
            IOperationRecordBusiness operationRecordBusiness,
            IAuthoritiesBusiness authoritiesBusiness)
        {
            Orm = freeSqlProvider.GetFreeSql();
            Repository = Orm.GetRepository<System_Menu, string>();
            Repository_Resources = Orm.GetRepository<System_Resources, string>();
            Repository_MenuResources = Orm.GetRepository<System_MenuResources, string>();
            Mapper = autoMapperProvider.GetMapper();
            OperationRecordBusiness = operationRecordBusiness;
            AuthoritiesBusiness = authoritiesBusiness;
        }

        #endregion

        #region 私有成员

        readonly IFreeSql Orm;

        readonly IBaseRepository<System_Menu, string> Repository;

        readonly IBaseRepository<System_Resources, string> Repository_Resources;

        readonly IBaseRepository<System_MenuResources, string> Repository_MenuResources;

        readonly IMapper Mapper;

        readonly IOperationRecordBusiness OperationRecordBusiness;

        readonly IAuthoritiesBusiness AuthoritiesBusiness;

        #endregion

        #region 外部接口

        #region 基础功能

        public List<List> GetList(PaginationDTO pagination)
        {
            var entityList = Repository.Select
                                    .GetPagination(pagination)
                                    .ToList<System_Menu, List>(typeof(List).GetNamesWithTagAndOther(true, "_List"));

            var result = Mapper.Map<List<List>>(entityList);

            return result;
        }

        public List<TreeList> GetTreeList(TreeListParamter paramter, bool deep = false)
        {
            if (paramter.ParentId.IsNullOrWhiteSpace())
                paramter.ParentId = null;

            var entityList = Repository.Select
                                    .Where(o => o.ParentId == paramter.ParentId)
                                    .OrderBy(o => o.Sort)
                                    .ToList<System_Menu, TreeList>(typeof(List).GetNamesWithTagAndOther(true, "_List"));

            var result = Mapper.Map<List<TreeList>>(entityList);

            if (result.Any())
                result.ForEach(o =>
                {
                    var rank = paramter.Rank;
                    if (!rank.HasValue || rank > 0)
                    {
                        paramter.ParentId = o.Id;
                        paramter.Rank--;
                        o.Childs_ = GetTreeList(paramter, true);
                    }
                });
            else if (deep)
                result = null;

            return result;
        }

        public Detail GetDetail(string id)
        {
            var entity = Repository.GetAndCheckNull(id);

            var result = Mapper.Map<Detail>(entity);

            return result;
        }

        [AdministratorOnly]
        public void Create(Create data)
        {
            var newData = Mapper.Map<System_Menu>(data).InitEntity();

            if (Repository.Where(o => o.ParentId == newData.ParentId && o.Code == newData.Code).Any())
                throw new MessageException($"当前层级已存在编码为{newData.Code}的菜单.");

            if (Repository.Where(o => o.ParentId == newData.ParentId && o.Type == newData.Type && o.Name == newData.Name).Any())
                throw new MessageException($"当前层级已存在类型为{newData.Type},且名称为{newData.Name}的菜单.");

            newData.Uri = newData.Uri?.ToLower();

            (bool success, Exception ex) = Orm.RunTransaction(() =>
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
                    DataType = nameof(System_Menu),
                    DataId = newData.Id,
                    Explain = $"创建菜单[名称 {newData.Name}, 类型 {newData.Type}]."
                });
            });

            if (!success)
                throw new MessageException("创建菜单失败", ex);
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
            var editData = Mapper.Map<System_Menu>(data).ModifyEntity();

            if (Repository.Where(o => o.ParentId == editData.ParentId && o.Code == editData.Code && o.Id != editData.Id).Any())
                throw new MessageException($"当前层级已存在编码为{editData.Code}的菜单.");

            if (Repository.Where(o => o.ParentId == editData.ParentId && o.Type == editData.Type && o.Name == editData.Name && o.Id != editData.Id).Any())
                throw new MessageException($"当前层级已存在类型为{editData.Type},且名称为{editData.Name}的菜单.");

            editData.Uri = editData.Uri?.ToLower();

            var entity = Repository.GetAndCheckNull(editData.Id);

            var changed_ = string.Join(",",
                                       entity.GetPropertyValueChangeds<System_Menu, Edit>(editData)
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
                }

                editData.Sort = Repository.Where(o => o.ParentId == editData.ParentId).Max(o => o.Sort) + 1;

                if (Repository.UpdateDiy
                         .Where(o => o.ParentId == entity.ParentId && o.Id != entity.Id && o.Sort > editData.Sort)
                         .Set(o => o.Sort - 1)
                         .ExecuteAffrows() < 0)
                    throw new MessageException("重新排序失败.");

                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_Menu),
                    DataId = entity.Id,
                    Explain = $"修改菜单[名称 {entity.Name}, 类型 {entity.Type}].",
                    Remark = $"变更详情: \r\n\t{changed_}"
                });

                if (Repository.UpdateDiy
                      .SetSource(editData.ModifyEntity())
                      .UpdateColumns(typeof(Edit).GetNamesWithTagAndOther(false, "_Edit").ToArray())
                      .ExecuteAffrows() <= 0)
                    throw new MessageException("修改菜单失败");
            });

            if (!success)
                throw ex;
        }

        [AdministratorOnly]
        public void Delete(List<string> ids)
        {
            var entityList = Repository.Select.Where(c => ids.Contains(c.Id)).ToList(c => new { c.Id, c.Name, c.Type });

            var orList = new List<Common_OperationRecord>();

            foreach (var entity in entityList)
            {
                orList.Add(new Common_OperationRecord
                {
                    DataType = nameof(System_Menu),
                    DataId = entity.Id,
                    Explain = $"删除菜单[名称 {entity.Name}, 类型 {entity.Type}]."
                });
            }

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                AuthoritiesBusiness.RevocationMenuForAll(ids, false);

                if (Repository.Delete(o => ids.Contains(o.Id)) <= 0)
                    throw new MessageException("未删除任何数据");

                var orIds = OperationRecordBusiness.Create(orList);
            });

            if (!success)
                throw new MessageException("删除菜单失败", ex);
        }

        #endregion

        #region 拓展功能

        [AdministratorOnly]
        public void Enable(string id, bool enable)
        {
            var entity = Repository.GetAndCheckNull(id);

            entity.Enable = enable;

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_Menu),
                    DataId = entity.Id,
                    Explain = $"{(enable ? "启用" : "禁用")}菜单[名称 {entity.Name}, 类型 {entity.Type}]."
                });

                if (Repository.Update(entity) <= 0)
                    throw new MessageException($"{(enable ? "启用" : "禁用")}菜单失败");
            });

            if (!success)
                throw ex;
        }

        [AdministratorOnly]
        public void Sort(Sort data)
        {
            if (data.Span == 0 && (data.Type != Model.System.SortType.top || data.Type != Model.System.SortType.low))
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
                dynamic target;

                switch (data.Type)
                {
                    case Model.System.SortType.top:
                        target = Repository.Where(o => o.ParentId == current.ParentId)
                                             .OrderBy(o => o.Sort)
                                             .First(o => new
                                             {
                                                 o.Id,
                                                 o.Sort
                                             });
                        break;
                    case Model.System.SortType.up:
                        target = Repository.Where(o => o.ParentId == current.ParentId && o.Sort == current.Sort - 1)
                                             .First(o => new
                                             {
                                                 o.Id,
                                                 o.Sort
                                             });
                        break;
                    case Model.System.SortType.down:
                        target = Repository.Where(o => o.ParentId == current.ParentId && o.Sort == current.Sort + 1)
                                             .First(o => new
                                             {
                                                 o.Id,
                                                 o.Sort
                                             });
                        break;
                    case Model.System.SortType.low:
                        target = Repository.Where(o => o.ParentId == current.ParentId)
                                             .OrderByDescending(o => o.Sort)
                                             .First(o => new
                                             {
                                                 o.Id,
                                                 o.Sort
                                             });
                        break;
                    default:
                        throw new MessageException($"不支持的排序类型 {data.Type}.");
                }

                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_Menu),
                    DataId = current.Id,
                    Explain = $"菜单排序[名称 {current.Name}, 类型 {current.Type}]."
                });

                string targetId = target.Id;
                int targetSort = target.Sort;

                if (Repository.UpdateDiy
                         .Where(o => o.Id == targetId)
                         .Set(o => o.Sort, current.Sort)
                         .ExecuteAffrows() < 0
                    || Repository.UpdateDiy
                         .Where(o => o.Id == current.Id)
                         .Set(o => o.Sort, targetSort)
                         .ExecuteAffrows() < 0)
                    throw new MessageException("菜单排序失败.");
            });

            if (!success)
                throw ex;
        }

        [AdministratorOnly]
        public void DragSort(DragSort data)
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
                        target_new = Repository.Where(o => o.ParentId == target.ParentId && o.Sort == target.Sort + 1)
                                             .First(o => new
                                             {
                                                 o.Id,
                                                 o.Sort
                                             });
                    }
                    else
                    {
                        target_new = Repository.Where(o => o.ParentId == target.ParentId && o.Sort == current.Sort - 1)
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
                        throw new MessageException("菜单排序失败.");

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
                        throw new MessageException("菜单排序失败.");

                    #endregion
                }

                _ = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_Menu),
                    DataId = current.Id,
                    Explain = $"菜单排序[名称 {current.Name}, 类型 {current.Type}]."
                });
            });

            if (!success)
                throw ex;
        }

        public void AssociateResources(string id, List<string> resourcesIds, bool runTransaction = true)
        {
            if (!resourcesIds.Any_Ex())
                return;

            var menu = Repository.GetAndCheckNull(id);

            void handler()
            {
                var newData = new List<System_MenuResources>();

                var resources = Repository_Resources
                            .Where(o => resourcesIds.Contains(o.Id)
                                && o.Enable == true
                                && !o.Menus.AsSelect().Where(p => p.Id == id).Any())
                            .ToList(o => new
                            {
                                o.Id,
                                o.Type,
                                o.Name
                            });

                if (!resources.Any())
                    return;

                newData.AddRange(resources.Select(o => new System_MenuResources
                {
                    ResourcesId = o.Id,
                    MenuId = id
                }));

                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_MenuResources),
                    DataId = null,
                    Explain = $"菜单关联资源.",
                    Remark = $"菜单: \r\n\t[名称 {menu.Name}, 类型 {menu.Type}]\r\n" +
                            $"关联的资源: \r\n\t{string.Join(",", resources.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                });

                if (newData.Any())
                    Repository_MenuResources.Insert(newData);
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("关联失败.", ex);
            }
            else
                handler();
        }

        public void DisassociateResources(string id, List<string> resourcesIds, bool runTransaction = true)
        {
            if (!resourcesIds.Any_Ex())
                return;

            var menu = Repository.GetAndCheckNull(id);

            void handler()
            {
                var newData = new List<System_MenuResources>();

                var resources = Repository_Resources
                            .Where(o => resourcesIds.Contains(o.Id)
                                && o.Menus.AsSelect().Where(p => p.Id == id).Any())
                            .ToList(o => new
                            {
                                o.Id,
                                o.Type,
                                o.Name
                            });

                if (!resources.Any())
                    return;

                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_MenuResources),
                    DataId = null,
                    Explain = $"菜单解除关联资源.",
                    Remark = $"菜单: \r\n\t[名称 {menu.Name}, 类型 {menu.Type}]\r\n" +
                            $"解除关联的资源: \r\n\t{string.Join(",", resources.Select(o => $"[名称 {o.Name}, 类型 {o.Type}]"))}"
                });

                var deleteIds = resources.Select(o => o.Id).ToList();

                if (Repository_MenuResources.Delete(o => o.MenuId == id && deleteIds.Contains(o.ResourcesId)) < 0)
                    throw new MessageException("解除关联失败.");
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("操作失败.", ex);
            }
            else
                handler();
        }

        #endregion

        #endregion
    }
}
