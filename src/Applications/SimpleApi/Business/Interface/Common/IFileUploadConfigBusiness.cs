
/*\__________________________________________________________________________________________________
|*												提示											 __≣|
|* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~| 
|*      此代码由T4模板自动生成																	|
|*		版本:v0.0.0.1                                                                           |__
|*		日期:2021-08-23 15:57:37                                                                   ≣|
|*																				by  LCTR		   ≣|
|* ________________________________________________________________________________________________≣|
\*/



using Microsoft.AspNetCore.Http;
using Model.Common.FileUploadConfigDTO;
using Model.Utils.OfficeDocuments;
using Model.Utils.OfficeDocuments.ExcelDTO;
using Model.Utils.Pagination;
using Model.Utils.Sort.SortParamsDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interface.Common
{
    /// <summary>
    /// 文件上传配置接口类
    /// </summary>
    public interface IFileUploadConfigBusiness
    {
        #region 基础功能

        /// <summary>
        /// 获取树状列表数据
        /// </summary>
        /// <param name="pagination">分页设置</param>
        /// <returns></returns>
        List<List> GetTreeList(TreePaginationDTO pagination);

        /// <summary>
        /// 获取详情数据
        /// </summary>
        /// <param name="id">主键</param>
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
        /// <param name="id">主键</param>
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
        /// <param name="keys">[主键]</param>
        /// <returns></returns>
        void Delete(List<string> keys);

        #endregion

        #region 拓展功能

        /// <summary>
        /// 普通排序
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        void Sort(Sort data);

        /// <summary>
        /// 拖动排序
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        void DragSort(DragSort data);

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
        /// <see cref="ExcelVersion.xls"/>2003,
        /// (默认)<see cref="ExcelVersion.xlsx"/>2007
        /// </param>
        /// <param name="paginationJson">分页参数Json字符串</param>
        /// <returns></returns>
        void Export(string version = ExcelVersion.xlsx, string paginationJson = null);

        #endregion
    }
}

