using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4CAGC.Model
{
    /// <summary>
    /// 方法功能
    /// </summary>
    public enum Function
    {
        #region 基础功能

        /// <summary>
        /// 列表
        /// </summary>
        List,
        /// <summary>
        /// 详情
        /// </summary>
        Detail,
        /// <summary>
        /// 新增
        /// </summary>
        Create,
        /// <summary>
        /// 编辑
        /// </summary>
        Edit,
        /// <summary>
        /// 删除
        /// </summary>
        Delete,

        #endregion

        #region 拓展功能

        /// <summary>
        /// 启用/禁用
        /// </summary>
        Enable,
        /// <summary>
        /// 排序
        /// </summary>
        Sort,
        /// <summary>
        /// 导入
        /// </summary>
        Import,
        /// <summary>
        /// 导出
        /// </summary>
        Export,
        /// <summary>
        /// 下拉列表
        /// </summary>
        DropdownList

        #endregion
    }
}
