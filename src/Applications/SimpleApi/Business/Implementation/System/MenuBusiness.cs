using AutoMapper;
using Business.Filter;
using Business.Interface.Common;
using Business.Interface.System;
using Business.Utils;
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
            Mapper = autoMapperProvider.GetMapper();
            OperationRecordBusiness = operationRecordBusiness;
            AuthoritiesBusiness = authoritiesBusiness;
        }

        #endregion

        #region 私有成员

        readonly IFreeSql Orm;

        readonly IBaseRepository<System_Menu, string> Repository;

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
                throw new ApplicationException($"当前层级已存在编码为{newData.Code}的菜单.");

            if (Repository.Where(o => o.ParentId == newData.ParentId && o.Type == newData.Type && o.Name == newData.Name).Any())
                throw new ApplicationException($"当前层级已存在类型为{newData.Type},且名称为{newData.Name}的菜单.");

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
                throw new ApplicationException("创建菜单失败", ex);
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
                throw new ApplicationException($"当前层级已存在编码为{editData.Code}的菜单.");

            if (Repository.Where(o => o.ParentId == editData.ParentId && o.Type == editData.Type && o.Name == editData.Name && o.Id != editData.Id).Any())
                throw new ApplicationException($"当前层级已存在类型为{editData.Type},且名称为{editData.Name}的菜单.");

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
                         .Where(o => o.ParentId == entity.ParentId && o.Id != entity.Id && o.Sort > entity.Sort)
                         .Set(o => o.Sort - 1)
                         .ExecuteAffrows() < 0)
                    throw new ApplicationException("重新排序失败.");

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
                    throw new ApplicationException("修改菜单失败");
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
                    throw new ApplicationException("未删除任何数据");

                var orIds = OperationRecordBusiness.Create(orList);
            });

            if (!success)
                throw new ApplicationException("删除菜单失败", ex);
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
                    throw new ApplicationException($"{(enable ? "启用" : "禁用")}菜单失败");
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

            if (current.Id == null)
                throw new ApplicationException("数据不存在或已被移除.");

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var newSort = current.Sort;

                switch (data.Type)
                {
                    case Model.System.SortType.top:
                        if (Repository.UpdateDiy
                                 .Where(o => o.ParentId == current.ParentId && o.Id != current.Id)
                                 .Set(o => o.Sort + 1)
                                 .ExecuteAffrows() < 0)
                            throw new ApplicationException("菜单排序失败.");

                        newSort = 0;
                        break;
                    case Model.System.SortType.up:
                        if (Repository.UpdateDiy
                                 .Where(o => o.ParentId == current.ParentId && o.Id != current.Id && o.Sort < current.Sort && o.Sort >= current.Sort - data.Span)
                                 .Set(o => o.Sort + 1)
                                 .ExecuteAffrows() < 0)
                            throw new ApplicationException("菜单排序失败.");

                        newSort -= data.Span;
                        break;
                    case Model.System.SortType.down:
                        if (Repository.UpdateDiy
                                 .Where(o => o.ParentId == current.ParentId && o.Id != current.Id && o.Sort > current.Sort && o.Sort <= current.Sort + data.Span)
                                 .Set(o => o.Sort - 1)
                                 .ExecuteAffrows() < 0)
                            throw new ApplicationException("菜单排序失败.");

                        newSort += data.Span;
                        break;
                    case Model.System.SortType.low:
                        newSort = Repository.Select.Max(o => o.Sort);

                        if (Repository.UpdateDiy
                                 .Where(o => o.ParentId == current.ParentId && o.Id != current.Id)
                                 .Set(o => o.Sort - 1)
                                 .ExecuteAffrows() < 0)
                            throw new ApplicationException("菜单排序失败.");
                        break;
                    default:
                        throw new ApplicationException($"不支持的排序类型 {data.Type}.");
                }

                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(System_Role),
                    DataId = current.Id,
                    Explain = $"菜单排序[名称 {current.Name}, 类型 {current.Type}]."
                });

                if (Repository.UpdateDiy
                        .Where(o => o.Id == current.Id)
                        .Set(o => o.Sort, newSort)
                        .ExecuteAffrows() <= 0)
                    throw new ApplicationException("菜单排序失败.");
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

            if (current.Id == null)
                throw new ApplicationException("数据不存在或已被移除.");

            var target = Repository.Where(o => o.Id == data.TargetId)
                                    .ToOne(o => new
                                    {
                                        o.Id,
                                        o.ParentId,
                                        o.Sort,
                                        o.Level,
                                        o.RootId
                                    });

            if (target.Id == null)
                throw new ApplicationException("目标数据不存在.");

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                if (current.ParentId != target.ParentId)
                {
                    if (Repository.UpdateDiy
                             .Where(o => o.ParentId == current.ParentId && o.Id != current.Id && o.Sort > current.Sort)
                             .Set(o => o.Sort - 1)
                             .ExecuteAffrows() < 0)
                        throw new ApplicationException("菜单排序失败.");

                    if (Repository.UpdateDiy
                             .Where(o => o.ParentId == target.ParentId && o.Sort >= target.Sort)
                             .Set(o => o.Sort + 1)
                             .ExecuteAffrows() < 0)
                        throw new ApplicationException("菜单排序失败.");

                    if (current.ParentId.IsNullOrWhiteSpace())
                    {
                        if (Repository.UpdateDiy
                                .Where(o => o.Id == current.Id)
                                .Set(o => o.Sort, target.Sort)
                                .Set(o => o.ParentId, null)
                                .Set(o => o.Level, 0)
                                .Set(o => o.RootId, null)
                                .ExecuteAffrows() <= 0)
                            throw new ApplicationException("菜单排序失败.");
                    }
                    else
                    {
                        if (Repository.UpdateDiy
                                .Where(o => o.Id == current.Id)
                                .Set(o => o.Sort, target.Sort)
                                .Set(o => o.ParentId, target.ParentId)
                                .Set(o => o.Level, target.Level)
                                .Set(o => o.RootId, target.RootId)
                                .ExecuteAffrows() <= 0)
                            throw new ApplicationException("菜单排序失败.");
                    }
                }
                else
                {
                    if (Repository.UpdateDiy
                             .Where(o => o.ParentId == current.ParentId && o.Id != current.Id && (current.Sort > target.Sort ? (o.Sort < current.Sort && o.Sort >= target.Sort) : (o.Sort > current.Sort && o.Sort <= target.Sort)))
                             .Set(o => current.Sort > target.Sort ? o.Sort + 1 : o.Sort - 1)
                             .ExecuteAffrows() < 0)
                        throw new ApplicationException("菜单排序失败.");

                    if (Repository.UpdateDiy
                            .Where(o => o.Id == current.Id)
                            .Set(o => o.Sort, target.Sort)
                            .ExecuteAffrows() <= 0)
                        throw new ApplicationException("菜单排序失败.");
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

        #endregion

        #endregion
    }
}
