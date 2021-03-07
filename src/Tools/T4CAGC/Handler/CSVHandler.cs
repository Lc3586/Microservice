using Microservice.Library.OfficeDocuments;
using System.Collections.Generic;
using System.Text;
using T4CAGC.Model;

namespace T4CAGC.Handler
{
    /// <summary>
    /// CSV文件处理类
    /// </summary>
    public class CSVHandler
    {
        /// <summary>
        /// 分析
        /// </summary>
        /// <param name="filename">csv文件</param>
        /// <returns></returns>
        public static List<TableInfo> Analysis(string filename)
        {
            var dt = filename.ReadCSV(true, Encoding.UTF8);

            return null;
        }
    }
}
