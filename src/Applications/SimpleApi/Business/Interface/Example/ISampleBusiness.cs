using Microsoft.AspNetCore.Http;
using Model.Example.DBDTO;
using Model.Utils.OfficeDocuments;
using Model.Utils.OfficeDocuments.ExcelDTO;
using Model.Utils.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interface.Example
{
    /// <summary>
    /// 示例业务接口类
    /// </summary>
    public interface ISampleBusiness
    {
        #region 基础功能

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<List> GetList(PaginationDTO pagination);

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
        /// 下载导入模板
        /// </summary>
        /// <param name="version">
        /// <para>指定Excel文件版本</para>
        /// <para><see cref="ExcelVersion.xls"/>: 2003版本</para>
        /// <para>(默认)<see cref="ExcelVersion.xlsx"/>: 2007及以上版本</para>
        /// </param>
        /// <param name="autogenerateTemplate">
        /// <para>指明要使用的模板类型</para>
        /// <para>true: 自动生成模板</para>
        /// <para>(默认)false: 使用预制模板</para>
        /// </param>
        /// <returns></returns>
        Task DownloadTemplate(string version = ExcelVersion.xlsx, bool autogenerateTemplate = false);

        /// <summary>
        /// 数据导入
        /// </summary>
        /// <param name="file">Execl文件</param>
        /// <param name="autogenerateTemplate">
        /// <para>指明所使用的模板类型</para>
        /// <para>true: 自动生成的模板</para>
        /// <para>(默认)false: 预制模板</para>
        /// </param>
        /// <returns></returns>
        ImportResult Import(IFormFile file, bool autogenerateTemplate = false);

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="version">
        /// Excel文件版本,
        /// <see cref="ExcelVersion.xls"/>2003,(默认)
        /// <seealso cref="ExcelVersion.xlsx"/>2007
        /// </param>
        /// <param name="paginationJson">分页参数Json字符串</param>
        /// <returns></returns>
        void Export(string version = ExcelVersion.xlsx, string paginationJson = null);

        #endregion
    }
}
