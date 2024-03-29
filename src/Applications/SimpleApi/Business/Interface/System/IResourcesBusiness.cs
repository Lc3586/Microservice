﻿using Microservice.Library.SelectOption;
using Model.System.ResourcesDTO;
using Model.Utils.Pagination;
using System.Collections.Generic;

namespace Business.Interface.System
{
    /// <summary>
    /// 资源业务接口类
    /// </summary>
    public interface IResourcesBusiness
    {
        #region 基础功能

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<List> GetList(PaginationDTO pagination);

        /// <summary>
        /// 获取下拉框数据
        /// </summary>
        /// <param name="condition">关键词(多个用空格分隔)</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<SelectOption> DropdownList(string condition, PaginationDTO pagination);

        /// <summary>
        /// 获取详情数据
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        Detail GetDetail(string id);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        void Create(Create data);

        /// <summary>
        /// 获取编辑数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Edit GetEdit(string id);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        void Edit(Edit data);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        /// <returns></returns>
        void Delete(List<string> ids);

        #endregion

        #region 拓展功能

        /// <summary>
        /// 启用/禁用
        /// </summary>
        /// <param name="id">数据</param>
        /// <param name="enable">设置状态</param>
        /// <returns></returns>
        void Enable(string id, bool enable);

        /// <summary>
        /// 关联菜单
        /// </summary>
        /// <param name="id">数据</param>
        /// <param name="menuIds">菜单Id集合</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        /// <returns></returns>
        void AssociateMenus(string id, List<string> menuIds, bool runTransaction = true);

        /// <summary>
        /// 解除关联菜单
        /// </summary>
        /// <param name="id">数据</param>
        /// <param name="menuIds">菜单Id集合</param>
        /// <param name="runTransaction">运行事务（默认运行）</param>
        /// <returns></returns>
        void DisassociateMenus(string id, List<string> menuIds, bool runTransaction = true);

        #endregion
    }
}
