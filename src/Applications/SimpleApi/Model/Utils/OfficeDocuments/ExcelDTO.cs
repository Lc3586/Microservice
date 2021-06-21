using System.Collections.Generic;

namespace Model.Utils.OfficeDocuments.ExcelDTO
{
    /// <summary>
    /// 导入结果
    /// </summary>
    public class ImportResult
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addTotal">新增数据量</param>
        /// <param name="updateTotal">更新数据量</param>
        /// <param name="error">错误信息</param>
        public ImportResult(int addTotal, int updateTotal, List<ErrorInfo> error = null)
        {
            AddTotal = addTotal;
            UpdateTotal = updateTotal;
            Error = error;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error">错误信息</param>
        public ImportResult(List<ErrorInfo> error)
        {
            AddTotal = 0;
            UpdateTotal = 0;
            Error = error;
        }

        /// <summary>
        /// 新增数据量
        /// </summary>
        public int AddTotal { get; set; }

        /// <summary>
        /// 更新数据量
        /// </summary>
        public int UpdateTotal { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public List<ErrorInfo> Error { get; set; }
    }

    /// <summary>
    /// 错误信息
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIndex">行序号</param>
        /// <param name="columnName">列名</param>
        /// <param name="info">信息</param>
        public ErrorInfo(int rowIndex, string columnName, string info)
        {
            RowIndex = rowIndex;
            ColumnName = columnName;
            Info = info;
        }

        /// <summary>
        /// 行序号
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public string Info { get; set; }
    }
}
