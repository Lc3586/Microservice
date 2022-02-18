using Model.Common.QuickInputDTO;
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
        /// 获取当前账号的匹配数据
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="keyword">关键词</param>
        /// <param name="paging">是否分页</param>
        /// <param name="rows">每页数据量</param>
        /// <param name="page">页码</param>
        /// <returns></returns>
        List<List> GetCurrentAccountMatchList(string category, string keyword, bool paging = false, int rows = 50, int page = 1);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="data">数据</param>
        void Create(Create data);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="datas">数据</param>
        void Create(List<Create> datas);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        /// <returns></returns>
        void Delete(List<string> ids);
    }
}
