﻿using Model.Common.QuickInputDTO;
using Model.Utils.Pagination;
using System.Collections.Generic;

namespace Business.Interface.Common
{
    /// <summary>
    /// 快捷输入业务接口类
    /// </summary>
    public interface IQuickInputBusiness
    {
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<List> GetList(PaginationDTO pagination);

        /// <summary>
        /// 获取当前账号的选项数据
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        List<OptionList> GetCurrentAccountOptionList(string category, string keyword);

        /// <summary>
        /// 获取当前账号的选项数据（分页）
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="keyword">关键词</param>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<OptionList> GetCurrentAccountOptionList(string category, string keyword, PaginationDTO pagination);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="data">数据</param>
        void Create(Create data);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="data">数据</param>
        void BatchCreate(BatchCreate data);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        /// <returns></returns>
        void Delete(List<string> ids);
    }
}
