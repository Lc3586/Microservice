
/*\__________________________________________________________________________________________________
|*												提示											 __≣|
|* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~| 
|*      此代码由T4模板自动生成																	|
|*		版本:v0.0.0.1                                                                           |__
|*		日期:2021-08-23 15:57:37                                                                   ≣|
|*																				by  LCTR		   ≣|
|* ________________________________________________________________________________________________≣|
\*/



using AutoMapper;
using Business.Interface.Common;
using Business.Interface.System;
using Business.Utils;
using Business.Utils.Filter;
using Business.Utils.Log;
using Business.Utils.Pagination;
using Entity.Common;
using FreeSql;
using FreeSql.DataAnnotations;
using Microservice.Library.DataMapping.Gen;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using Microservice.Library.Http;
using Microservice.Library.OfficeDocuments;
using Microservice.Library.OpenApi.Extention;
using Microsoft.AspNetCore.Http;
using Model.Common.FileUploadConfigDTO;
using Model.Utils.Log;
using Model.Utils.OfficeDocuments;
using Model.Utils.OfficeDocuments.ExcelDTO;
using Model.Utils.Pagination;
using Model.Utils.Sort;
using Model.Utils.Sort.SortParamsDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Business.Implementation.Common
{
    /// <summary>
    /// 文件上传配置业务类
    /// </summary>
    public class FileUploadConfigBusiness : BaseBusiness, IFileUploadConfigBusiness
    {
        #region DI

        public FileUploadConfigBusiness(
            IFreeSqlProvider freeSqlProvider,
            IHttpContextAccessor httpContextAccessor,
            IAutoMapperProvider autoMapperProvider,
            IOperationRecordBusiness operationRecordBusiness,
            IAuthoritiesBusiness authoritiesBusiness)
        {
            Orm = freeSqlProvider.GetFreeSql();
            Repository = Orm.GetRepository<Common_FileUploadConfig, string>();
            HttpContextAccessor = httpContextAccessor;
            Mapper = autoMapperProvider.GetMapper();
            OperationRecordBusiness = operationRecordBusiness;
            AuthoritiesBusiness = authoritiesBusiness;
        }

        #endregion

        #region 私有成员

        readonly IFreeSql Orm;

        readonly IBaseRepository<Common_FileUploadConfig, string> Repository;

        readonly IHttpContextAccessor HttpContextAccessor;

        readonly IMapper Mapper;

        readonly IOperationRecordBusiness OperationRecordBusiness;

        readonly IAuthoritiesBusiness AuthoritiesBusiness;

        /// <summary>
        /// 检测循环引用
        /// </summary>
        /// <param name="id">配置Id</param>
        /// <param name="referenceId">引用Id</param>
        /// <returns>True: 存在循环引用</returns>
        bool CheckCircularReference(string id, string referenceId)
        {
            if (id == referenceId)
                return true;

            if (Orm.Ado.QuerySingle<int>(
@"
WITH ""as_tree_cte""
as
(
SELECT 1 AS ""cte_level"", ""a"".""Id"", ""a"".""ReferenceId"", ""a"".""ReferenceTree""
FROM ""dbo"".""Common_FileUploadConfig"" ""a""
WHERE(""a"".""Id"" = @Id)

UNION ALL

SELECT ""wct1"".""cte_level"" + 1 AS ""cte_level"", ""wct2"".""Id"", ""wct2"".""ReferenceId"", ""wct2"".""ReferenceTree""
FROM ""as_tree_cte"" ""wct1""
INNER JOIN ""dbo"".""Common_FileUploadConfig"" ""wct2"" ON ""wct2"".""Id"" = ""wct1"".""ReferenceId""
WHERE ""wct1"".""ReferenceTree"" = 1 AND ""wct1"".""cte_level"" < @MaxLevel
)
SELECT COUNT(1)  FROM
(
SELECT ""a"".""Id"", COUNT(1) ""SUM""
FROM ""as_tree_cte"" ""a""
GROUP BY ""a"".""Id""
) ""b""
WHERE ""b"".""SUM"" > 1",
new
{
    Id = referenceId,
    MaxLevel = 100
}) > 0)
                return true;

            return false;
        }

        #endregion

        #region 基础功能

        [AdministratorOnly]
        public List<List> GetList(PaginationDTO pagination)
        {
            var entityList = Repository.Select
                                    .GetPagination(pagination)
                                    .ToList<Common_FileUploadConfig, List>(typeof(List).GetNamesWithTagAndOther(true, "_List"));

            var result = Mapper.Map<List<List>>(entityList);

            return result;
        }

        [AdministratorOnly]
        public List<List> GetTreeList(TreePaginationDTO pagination)
        {
            return GetData(pagination, false);

            List<List> GetData(TreePaginationDTO pagination, bool deep)
            {
                if (pagination.ParentId.IsNullOrWhiteSpace())
                    pagination.ParentId = null;

                var entityList = Repository.Select
                                    .Where(o => o.ParentId == pagination.ParentId)
                                    .GetPagination(pagination.Pagination)
                                    .ToList<Common_FileUploadConfig, List>(typeof(List).GetNamesWithTagAndOther(true, "_List"));

                var result = Mapper.Map<List<List>>(entityList);

                if (result.Any())
                    result.ForEach(o =>
                    {
                        var rank = pagination.Rank;
                        if (!rank.HasValue || rank > 0)
                        {
                            pagination.ParentId = o.Id;
                            pagination.Rank--;
                            o.Children = GetData(pagination, true);
                            o.HasChildren = o.Children.Any_Ex();
                            o.ChildrenCount = o.Children?.Count ?? 0;
                        }
                        else
                        {
                            o.HasChildren = Repository.Select.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Any();
                            o.ChildrenCount = (int)Repository.Where(p => p.ParentId == o.Id).Filter(pagination.Pagination, "a").Count();
                        }
                    });
                else if (deep)
                    result = null;

                return result;
            }
        }

        public Detail GetDetail(string id)
        {
            if (!Operator.IsSuperAdmin && !AuthoritiesBusiness.CurrentAccountHasCFUC(id))
                throw new MessageException("无权限.");

            var entity = Repository.GetAndCheckNull(id);

            var result = Mapper.Map<Detail>(entity);

            if (!result.ReferenceId.IsNullOrWhiteSpace())
            {
                var referenceCode = Repository.Where(p => p.Id == result.ReferenceId).ToOne(p => p.Code);
                if (referenceCode != default)
                    result.ReferenceCode = referenceCode;
            }

            if (!result.AllowedTypes.IsNullOrWhiteSpace())
                result.AllowedTypeList = result.AllowedTypes.Split(',').ToList();
            else
                result.AllowedTypeList = new List<string>();

            if (!result.ProhibitedTypes.IsNullOrWhiteSpace())
                result.ProhibitedTypeList = result.ProhibitedTypes.Split(',').ToList();
            else
                result.ProhibitedTypeList = new List<string>();

            return result;
        }

        [AdministratorOnly]
        public void Create(Create data)
        {
            var newData = Mapper.Map<Common_FileUploadConfig>(data).InitEntity();

            //@数据验证#待完善@
            if (Repository.Where(o => o.ParentId == newData.ParentId && o.Name == newData.Name).Any())
                throw new MessageException($"同层级下已存在名称为[{newData.Name}]的文件上传配置.");

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                if (!newData.ParentId.IsNullOrWhiteSpace())
                {
                    var parent = Repository
                        .Where(o => o.Id == newData.ParentId)
                        .ToOne(o => new
                        {
                            o.Id,
                            o.RootId,
                            o.Level
                        });

                    if (parent == default)
                        throw new MessageException("指定的父级不存在或已被移除.");

                    newData.RootId = parent.RootId;
                    newData.Level = parent.Level + 1;
                }
                else
                {
                    newData.ParentId = null;
                    newData.RootId = null;
                    newData.Level = 1;
                }

                newData.Sort = (int)Repository.Where(o => o.ParentId == newData.ParentId).Count() + 1;

                Repository.Insert(newData);

                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(Common_FileUploadConfig),
                    DataId = newData.Id,
                    Explain = $"创建文件上传配置[@字段说明#待完善@]."
                });
            });

            if (!success)
                throw new MessageException("创建文件上传配置失败", ex);
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
            var editData = Mapper.Map<Common_FileUploadConfig>(data).ModifyEntity();

            //@数据验证#待完善@
            if (Repository.Where(o => o.ParentId == data.ParentId && o.Name == editData.Name && o.Id != editData.Id).Any())
                throw new MessageException($"同层级下已存在名称为[{editData.Name}]的文件上传配置.");

            if (!editData.ReferenceId.IsNullOrWhiteSpace() && CheckCircularReference(editData.Id, editData.ReferenceId))
                throw new MessageException("检测到循环引用.");

            var entity = Repository.GetAndCheckNull(editData.Id);

            var changed_ = string.Join(",",
                                       entity.GetPropertyValueChangeds<Common_FileUploadConfig, Edit>(editData)
                                            .Select(p => $"\r\n\t {p.Description}：{p.FormerValue} 更改为 {p.CurrentValue}"));

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {

                if (Repository.UpdateDiy
                      .SetSource(editData)
                      .UpdateColumns(typeof(Edit).GetNamesWithTagAndOther(false, "_Edit").ToArray())
                      .ExecuteAffrows() <= 0)
                    throw new MessageException("修改文件上传配置失败");

                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(Common_FileUploadConfig),
                    DataId = editData.Id,
                    Explain = $"修改文件上传配置[@字段说明#待完善@].",
                    Remark = $"变更详情: \r\n\t{changed_}"
                });
            });

            if (!success)
                throw ex;
        }

        [AdministratorOnly]
        public void Delete(List<string> keys)
        {
            var entityList = Repository.Select.Where(c => keys.Contains(c.Id)).ToList(c => new { c.Id });

            var orList = new List<Common_OperationRecord>();

            foreach (var entity in entityList)
            {
                //@检查数据#待完善@

                orList.Add(new Common_OperationRecord
                {
                    DataType = nameof(Common_FileUploadConfig),
                    DataId = entity.Id,
                    Explain = $"删除文件上传配置[@字段说明#待完善@]."
                });
            }

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                AuthoritiesBusiness.RevocationMenuForAll(keys, false);

                if (Repository.Where(o => keys.Contains(o.Id))
                         .AsTreeCte()
                         .ToDelete()
                         .ExecuteAffrows() <= 0)
                    throw new MessageException("未删除任何数据");

                var orIds = OperationRecordBusiness.Create(orList);
            });

            if (!success)
                throw new MessageException("删除文件上传配置失败", ex);
        }

        #endregion

        #region 拓展功能

        public void Sort(Sort data)
        {
            if (data.Span == 0 && (data.Method != SortMethod.top || data.Method != SortMethod.low))
                return;

            var current = Repository.Where(o => o.Id == data.Id)
                                    .ToOne(o => new
                                    {
                                        o.Id,
                                        o.ParentId,
                                        o.Sort
                                    });

            if (current == default)
                throw new MessageException("数据不存在或已被移除.");

            (bool success, Exception ex) = Orm.RunTransaction(() =>
                {
                    var target = data.Method switch
                    {
                        SortMethod.top => Repository.Where(o => o.ParentId == current.ParentId)
                                                    .OrderBy(o => o.Sort)
                                                    .First(o => new
                                                    {
                                                        o.Id,
                                                        o.Sort
                                                    }),
                        SortMethod.up => Repository.Select.Where(o => o.ParentId == current.ParentId && o.Sort < current.Sort)
                                                .OrderByDescending(o => o.Sort)
                                                               .First(o => new
                                                               {
                                                                   o.Id,
                                                                   o.Sort
                                                               }),
                        SortMethod.down => Repository.Select.Where(o => o.ParentId == current.ParentId && o.Sort > current.Sort)
                                                .OrderBy(o => o.Sort)
                                                                 .First(o => new
                                                                 {
                                                                     o.Id,
                                                                     o.Sort
                                                                 }),
                        SortMethod.low => Repository.Select.Where(o => o.ParentId == current.ParentId)
                                                .OrderByDescending(o => o.Sort)
                                                                .First(o => new
                                                                {
                                                                    o.Id,
                                                                    o.Sort
                                                                }),
                        _ => throw new MessageException($"不支持的排序方法 {data.Method}.")
                    };

                    if (target == default)
                        return;

                    if (Repository.UpdateDiy
                         .Where(o => o.Id == target.Id)
                         .Set(o => o.Sort, current.Sort)
                         .ExecuteAffrows() < 0
                    || Repository.UpdateDiy
                         .Where(o => o.Id == current.Id)
                         .Set(o => o.Sort, target.Sort)
                         .ExecuteAffrows() < 0)
                        throw new MessageException("文件上传配置排序失败.");

                    var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                    {
                        DataType = nameof(Common_FileUploadConfig),
                        DataId = target.Id,
                        Explain = $"文件上传配置排序[排序方法: {data.Method}@字段说明#待完善@]."

                    });
                });

            if (!success)
                throw ex;
        }

        public void DragSort(TreeDragSort data)
        {
            if (data.Id == data.TargetId)
                return;

            var current = Repository.Where(o => o.Id == data.Id)
                                    .ToOne(o => new
                                    {
                                        o.Id,
                                        o.ParentId,
                                        o.Sort
                                    });

            if (current == default)
                throw new MessageException("数据不存在或已被移除.");

            var target = Repository.Where(o => o.Id == data.TargetId)
                                    .ToOne(o => new
                                    {
                                        o.Id,
                                        o.ParentId,
                                        o.RootId,
                                        o.Level,
                                        o.Sort
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
                        throw new MessageException("<#=Options.Table.Remark#>排序失败.");

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
                        throw new MessageException("文件上传配置排序失败.");

                    #endregion
                }

                _ = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(Common_FileUploadConfig),
                    DataId = target.Id,
                    Explain = $"文件上传配置拖动排序[@字段说明#待完善@]."
                });
            });

            if (!success)
                throw ex;
        }

        public async Task DownloadTemplate(string version = ExcelVersion.xlsx, bool autogenerateTemplate = true)
        {
            var response = HttpContextAccessor.HttpContext.Response;

            response.Headers.Add("Content-Disposition", $"attachment; filename=\"{UrlEncoder.Default.Encode("文件上传配置导入模板")}.{version}\"");

            if (!autogenerateTemplate)
            {
                #region 直接发送预制模板

                var filePath = PathHelper.GetAbsolutePath($"~/template/文件上传配置导入模板.{version}");
                if (!File.Exists(filePath))
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    throw new MessageException("模板文件不存在或已被移除.", new { FilePath = filePath });
                }
                await response.SendFileAsync(filePath);

                #endregion
            }
            else
            {
                #region 自动生成模板

                var table = new DataTable("文件上传配置导入模板");

                var type = typeof(Import);

                var tag = type.GetMainTag();

                type.GetProperties()
                    .ForEach(o =>
                    {
                        if (o.DeclaringType?.Name != type.Name && !o.HasTag(tag))
                            return;

                        var attr = o.GetCustomAttribute<DescriptionAttribute>();
                        if (attr == null)
                            return;

                        table.Columns.Add(attr.Description, o.PropertyType);
                    });

                var bytes = table.DataTableToExcelBytes(true, version == ExcelVersion.xlsx);
                response.StatusCode = StatusCodes.Status200OK;
                response.ContentType = "application/octet-stream";
                response.ContentLength = bytes.Length;
                response.Body.Write(bytes);

                #endregion
            }
        }

        public ImportResult Import(IFormFile file, bool autogenerateTemplate = true)
        {
            DataTable table;

            try
            {
                table = file.OpenReadStream()
                                .ReadExcel(true, file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", autogenerateTemplate ? 0 : 1);
            }
            catch (Exception _ex)
            {
                throw new MessageException("导入失败, 文件有误（请检查格式）.", _ex);
            }

            var type = typeof(Import);

            var tag = type.GetMainTag();

            var dataList = new List<Import>();

            var errors = new List<ErrorInfo>();

            var rowOffset = autogenerateTemplate ? 1 : 3;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                var data = new Import();

                var _errors = new List<ErrorInfo>();

                type.GetProperties()
                    .ForEach(o =>
                    {
                        if (o.DeclaringType?.Name != type.Name && !o.HasTag(tag))
                            return;

                        var attr = o.GetCustomAttribute<DescriptionAttribute>();
                        if (attr == null)
                            return;

                        if (!table.Columns.Contains(attr.Description))
                        {
                            _errors.Add(new ErrorInfo(i + rowOffset, attr.Description, "未找到该列。"));
                            return;
                        }

                        try
                        {
                            if (o.PropertyType == typeof(bool))
                                o.SetValue(data, row[attr.Description].ToString() == "是");
                            else if (o.PropertyType == typeof(string))
                            {
                                var value = row[attr.Description].ToString();

                                var attrColumn = o.GetCustomAttribute<ColumnAttribute>();
                                if (attrColumn != null && attrColumn.StringLength > 0 && Encoding.ASCII.GetBytes(value).Sum(c => c == 63 ? 2 : 1) > attrColumn.StringLength)
                                {
                                    _errors.Add(new ErrorInfo(i + rowOffset, attr.Description, $"数据长度过长, 不可超过{attrColumn.StringLength}个字符（其中每个中文占两个字符）。"));
                                    return;
                                }

                                o.SetValue(data, value);
                            }
                            else
                                o.SetValue(data, row[attr.Description].ChangeType(o.PropertyType));
                        }
                        catch (Exception ex)
                        {
                            _errors.Add(new ErrorInfo(i + rowOffset, attr.Description, "数据格式有误。"));

                            Logger.Log(
                                NLog.LogLevel.Error,
                                LogType.警告信息,
                                $"{file.FileName}文件第{i + rowOffset}行第{attr.Description}列数据格式有误.",
                                null,
                                ex);
                        }
                    });

                //@数据验证#待完善@
                if (_errors.Any())
                {
                    errors.AddRange(_errors);
                    continue;
                }
                else if (data.Code.IsNullOrWhiteSpace())
                {
                    errors.Add(new ErrorInfo(i + rowOffset, "编码", "编码不能为空。"));
                    continue;
                }
                else if (data.Name.IsNullOrWhiteSpace())
                {
                    errors.Add(new ErrorInfo(i + rowOffset, "名称", "内容不能为空。"));
                    continue;
                }
                else if (data.DisplayName.IsNullOrWhiteSpace())
                {
                    errors.Add(new ErrorInfo(i + rowOffset, "显示名称", "内容不能为空。"));
                    continue;
                }
                else if (data.LowerLimit < 0)
                {
                    errors.Add(new ErrorInfo(i + rowOffset, "文件数量下限", "文件数量下限必须大于或等于0。"));
                    continue;
                }
                else if (data.UpperLimit < 1)
                {
                    errors.Add(new ErrorInfo(i + rowOffset, "文件数量上限", "文件数量上限必须大于或等于1。"));
                    continue;
                }

                if (dataList.Any(o => o.Code == data.Code))
                {
                    errors.Add(new ErrorInfo(i + rowOffset, "编码", "编码不能重复。"));
                    continue;
                }

                dataList.Add(data);
            }

            if (errors.Any())
                return new ImportResult(errors);

            //检查数据
            for (int i = 0; i < dataList.Count; i++)
            {
                var data = dataList[i];

                var entity = Repository.Where(o => o.Code == data.Code).ToOne(o => new { o.Id, o.ParentId, o.RootId, o.Level, o.Sort });
                if (entity != default)
                {
                    data.Id = entity.Id;
                    data.ParentId = entity.ParentId;
                    data.RootId = entity.RootId;
                    data.Level = entity.Level;
                    data.Sort = entity.Sort;
                    data.New = false;
                    data.ModifyEntity();
                }
                else
                {
                    data.Level = 1;
                    data.New = true;
                    data.InitEntity();
                }

                if (!data.ParentCode.IsNullOrWhiteSpace())
                {
                    var parent = Repository.Where(o => o.Code == data.ParentCode).ToOne(o => new { o.Id, o.RootId, o.Level });
                    if (parent != default)
                    {
                        data.ParentId = parent.Id;
                        data.RootId = parent.RootId;
                        data.Level = parent.Level + 1;
                    }
                    else if (!dataList.Any(o => o.Code == data.ParentCode))
                    {
                        errors.Add(new ErrorInfo(i + rowOffset, "父级编码", "指定的父级不存在或已被移除。"));
                        continue;
                    }
                }

                if (!data.ReferenceCode.IsNullOrWhiteSpace())
                {
                    var referenceId = Repository.Where(o => o.Code == data.ReferenceCode).ToOne(o => o.Id);
                    if (referenceId != default)
                        data.ReferenceId = referenceId;
                    else if (!dataList.Any(o => o.Code == data.ReferenceCode))
                    {
                        errors.Add(new ErrorInfo(i + rowOffset, "引用编码", "指定的引用不存在或已被移除。"));
                        continue;
                    }
                }
            }

            if (errors.Any())
                return new ImportResult(errors);

            //分析数据
            dataList.ForEach(data =>
            {
                if (data.ParentId == null && !data.ParentCode.IsNullOrWhiteSpace())
                    SetParent(data);

                if (data.ReferenceId == null && !data.ReferenceCode.IsNullOrWhiteSpace())
                    data.ReferenceId = dataList.First(o => o.Code == data.ReferenceCode).Id;
            });

            //设置父级信息
            void SetParent(Import data)
            {
                var parent = dataList.First(o => o.Code == data.ParentCode);

                if (parent.ParentId == null && !parent.ParentCode.IsNullOrWhiteSpace())
                    SetParent(parent);

                data.ParentId = parent.Id;
                data.RootId = parent.ParentId == null ? parent.Id : parent.RootId;
                data.Level = parent.Level + 1;
            }

            //分析数据
            for (int i = 0; i < dataList.Count; i++)
            {
                var data = dataList[i];

                if (data.ReferenceId != null && CheckCircularReference(data.Id, data.ReferenceId, null))
                    errors.Add(new ErrorInfo(i + rowOffset, "引用编码", "检测到循环引用。"));
            }

            //检测循环引用
            bool CheckCircularReference(string id, string referenceId, List<string> referenceList)
            {
                if (id == referenceId)
                    return true;

                if (referenceList == null)
                    referenceList = new List<string> { referenceId };

                var reference = dataList.FirstOrDefault(o => o.Id == referenceId);

                if (reference == default)
                {
                    var nextReference = Repository.Where(o => o.Id == referenceId).ToOne(o => o.ReferenceId);
                    if (nextReference == default)
                        return false;

                    if (referenceList.Any(o => o == nextReference))
                        return true;

                    return CheckCircularReference(referenceId, nextReference, referenceList);
                }
                else
                {
                    if (reference.ReferenceId == default)
                        return false;

                    if (referenceList.Any(o => o == reference.ReferenceId))
                        return true;

                    return CheckCircularReference(referenceId, reference.ReferenceId, referenceList);
                }
            }

            if (errors.Any())
                return new ImportResult(errors);

            var result = new ImportResult(dataList.Count(o => o.New), 0);

            result.UpdateTotal = dataList.Count - result.AddTotal;

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var orList = new List<Common_OperationRecord>();

                var updateField = type.GetNamesWithTagAndOther(false, "_Import").ToArray();

                dataList.ForEach(data =>
                {
                    //重新计算排序值
                    data.Sort = (int)Repository.Where(o => o.ParentId == data.ParentId && o.Id != data.Id).Count() + 1;

                    if (data.New)
                    {
                        Repository.Insert(data);

                        orList.Add(new Common_OperationRecord
                        {
                            DataType = nameof(Common_FileUploadConfig),
                            DataId = data.Id,
                            Explain = $"导入数据（新增）文件上传配置[@字段说明#待完善@]."
                        });
                    }
                    else
                    {
                        if (Repository.UpdateDiy
                             .SetSource(data)
                             .UpdateColumns(updateField)
                             .ExecuteAffrows() <= 0)
                            throw new ApplicationException("文件上传配置更改失败.");

                        orList.Add(new Common_OperationRecord
                        {
                            DataType = nameof(Common_FileUploadConfig),
                            DataId = data.Id,
                            Explain = $"导入数据（更新）文件上传配置[@字段说明#待完善@]."
                        });
                    }
                });

                var orIds = OperationRecordBusiness.Create(orList);
            });

            if (!success)
                throw new ApplicationException("文件上传配置导入失败.", ex);

            return result;
        }

        public void Export(string version = ExcelVersion.xlsx, string paginationJson = null)
        {
            PaginationDTO pagination;
            if (!paginationJson.IsNullOrWhiteSpace())
                pagination = paginationJson.ToObject<PaginationDTO>();
            else
                pagination = new PaginationDTO { PageIndex = -1 };

            var type = typeof(Export);

            var entityList = Repository.Select
                                    .GetPagination(pagination)
                                    .ToList<Common_FileUploadConfig, Export>(type.GetNamesWithTagAndOther(true, "_Export"));

            var list = Mapper.Map<List<Export>>(entityList);

            //动态生成模板
            var table = new DataTable("文件上传配置");

            var propDic = new Dictionary<string, PropertyInfo>();

            var tag = type.GetMainTag();

            //生成列
            type.GetProperties()
                .ForEach(o =>
                {
                    if (o.DeclaringType?.Name != type.Name && !o.HasTag(tag))
                        return;

                    var attr = o.GetCustomAttribute<DescriptionAttribute>();
                    if (attr == null)
                        return;

                    propDic.Add(attr.Description, o);
                    table.Columns.Add(attr.Description, typeof(string));
                });

            //生成数据
            list?.ForEach(o =>
            {
                if (!o.ParentId.IsNullOrWhiteSpace())
                {
                    var parentCode = Repository.Where(p => p.Id == o.ParentId).ToOne(p => p.Code);
                    if (parentCode != default)
                        o.ParentCode = parentCode;
                }

                if (!o.ReferenceId.IsNullOrWhiteSpace())
                {
                    var referenceCode = Repository.Where(p => p.Id == o.ReferenceId).ToOne(p => p.Code);
                    if (referenceCode != default)
                        o.ReferenceCode = referenceCode;
                }

                var row = table.NewRow();

                propDic.ForEach(d =>
                {
                    var value = d.Value.GetValue(o);
                    if (value == null)
                        return;

                    if (d.Value.PropertyType == typeof(bool))
                        row[d.Key] = (bool)value == true ? "是" : "否";
                    else if (d.Key.Contains("附件"))
                        row[d.Key] = string.Join(" \r\n",
                                                value.ToString()
                                                .Split(',')
                                                .Select(e => $"{Config.WebRootUrl}/file/download/{e}"));
                    else if (d.Value.PropertyType == typeof(DateTime) || d.Value.PropertyType == typeof(DateTime?))
                    {
                        var attr = d.Value.GetCustomAttribute<JsonConverterAttribute>();
                        if (attr == null)
                            row[d.Key] = ((DateTime)value).ToString("yyyy-MM-dd");
                        else
                            row[d.Key] = ((DateTime)value).ToString((string)attr.ConverterParameters[0]);
                    }
                    else
                        row[d.Key] = value.ToJson().TrimStart('"').TrimEnd('"');
                });

                table.Rows.Add(row);
            });

            var bytes = table.DataTableToExcelBytes(true, version == ExcelVersion.xlsx);

            var response = HttpContextAccessor.HttpContext.Response;
            response.Headers.Add("Content-Disposition", $"attachment; filename=\"{UrlEncoder.Default.Encode("文件上传配置")}.{version}\"");
            response.StatusCode = StatusCodes.Status200OK;
            response.ContentType = "application/octet-stream";
            response.ContentLength = bytes.Length;
            response.Body.Write(bytes);
        }

        public Config GetConfig(string id)
        {
            if (!Operator.IsSuperAdmin && !AuthoritiesBusiness.CurrentAccountHasCFUC(id))
                throw new MessageException("无权限.");

            var entity = Repository
                .Where(o => o.Id == id)
                .ToOne(o => new
                {
                    o.Id,
                    o.Enable,
                    o.LowerLimit,
                    o.UpperLimit,
                    o.AllowedTypes,
                    o.ProhibitedTypes,
                    o.Explain,
                    o.ReferenceId,
                    o.ReferenceTree
                });

            if (entity == default)
                throw new MessageException("文件上传配置不存在或已被移除");

            if (!entity.Enable)
                throw new MessageException("文件上传配置不可用");

            var config = new Config
            {
                Id = entity.Id,
                LowerLimit = entity.LowerLimit,
                UpperLimit = entity.UpperLimit,
                AllowedTypeList = entity.AllowedTypes?.Split(',').ToList() ?? new List<string>(),
                ProhibitedTypeList = entity.ProhibitedTypes?.Split(',').ToList() ?? new List<string>(),
                Explain = entity.Explain
            };

            if (!entity.ReferenceId.IsNullOrWhiteSpace())
                GetReferenceConfig(config, entity.ReferenceId, entity.ReferenceTree);

            return config;

            void GetReferenceConfig(Config config, string referenceId, bool referenceTree)
            {
                if (referenceTree)
                {
                    var referenceConfigs = Orm.Ado.Query<Types>(
@"
WITH ""as_tree_cte""
as
(
SELECT 1 AS ""cte_level"", ""a"".""Id"", ""a"".""AllowedTypes"", ""a"".""ProhibitedTypes"", ""a"".""ReferenceId"", ""a"".""ReferenceTree""
FROM ""dbo"".""Common_FileUploadConfig"" ""a""
WHERE(""a"".""Id"" = @Id AND ""a"".""Enable"" = 1)

UNION ALL

SELECT ""wct1"".""cte_level"" + 1 AS ""cte_level"", ""wct2"".""Id"", ""wct2"".""AllowedTypes"", ""wct2"".""ProhibitedTypes"", ""wct2"".""ReferenceId"", ""wct2"".""ReferenceTree""
FROM ""as_tree_cte"" ""wct1""
INNER JOIN ""dbo"".""Common_FileUploadConfig"" ""wct2"" ON ""wct2"".""Id"" = ""wct1"".""ReferenceId""
WHERE ""wct1"".""ReferenceTree"" = 1 AND ""wct1"".""cte_level"" < @MaxLevel
)
SELECT DISTINCT ""a"".""Id"", ""a"".""AllowedTypes"", ""a"".""ProhibitedTypes""
FROM ""as_tree_cte"" ""a""",
new
{
    Id = referenceId,
    MaxLevel = 100
});

                    referenceConfigs.ForEach(o => Union(config, o.AllowedTypes?.Split(',').ToList(), o.ProhibitedTypes?.Split(',').ToList()));
                }
                else
                {
                    var referenceConfig = Repository.Where(o => o.Id == referenceId && o.Enable == true).ToOne(o => new
                    {
                        o.AllowedTypes,
                        o.ProhibitedTypes
                    });

                    if (referenceConfig == default)
                        return;

                    Union(config, referenceConfig.AllowedTypes?.Split(',').ToList(), referenceConfig.ProhibitedTypes?.Split(',').ToList());
                }
            }

            void Union(Config config, List<string> allowedTypeList, List<string> prohibitedTypeList)
            {
                if (allowedTypeList.Any_Ex())
                    config.AllowedTypeList = config.AllowedTypeList.Union(allowedTypeList).ToList();

                if (prohibitedTypeList.Any_Ex())
                    config.ProhibitedTypeList = config.ProhibitedTypeList.Union(prohibitedTypeList).ToList();
            }
        }

        public List<Config> GetConfigs(List<string> ids)
        {
            return ids.Select(o => GetConfig(o)).ToList();
        }

        #endregion
    }
}

