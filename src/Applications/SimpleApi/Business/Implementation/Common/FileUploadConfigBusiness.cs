
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
using Business.Utils;
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
            IOperationRecordBusiness operationRecordBusiness)
        {
            Orm = freeSqlProvider.GetFreeSql();
            Repository = Orm.GetRepository<Common_FileUploadConfig, string>();
            HttpContextAccessor = httpContextAccessor;
            Mapper = autoMapperProvider.GetMapper();
            OperationRecordBusiness = operationRecordBusiness;
        }

        #endregion

        #region 私有成员

        readonly IFreeSql Orm;

        readonly IBaseRepository<Common_FileUploadConfig, string> Repository;

        readonly IHttpContextAccessor HttpContextAccessor;

        readonly IMapper Mapper;

        readonly IOperationRecordBusiness OperationRecordBusiness;

        #endregion

        #region 基础功能

        public List<List> GetList(PaginationDTO pagination)
        {
            var entityList = Repository.Select
                                    .GetPagination(pagination)
                                    .ToList<Common_FileUploadConfig, List>(typeof(List).GetNamesWithTagAndOther(true, "_List"));

            var result = Mapper.Map<List<List>>(entityList);

            return result;
        }

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
                            o.Childs_ = GetData(pagination, true);
                        }
                    });
                else if (deep)
                    result = null;

                return result;
            }
        }

        public Detail GetDetail(string id)
        {
            var entity = Repository.GetAndCheckNull(id);

            var result = Mapper.Map<Detail>(entity);

            return result;
        }

        public void Create(Create data)
        {
            var newData = Mapper.Map<Common_FileUploadConfig>(data).InitEntity();

            //@数据验证#待完善@
            if (Repository.Where(o => o.ParentId == data.ParentId && o.Name == newData.Name).Any())
                throw new MessageException($"同层级下已存在名称为[{newData.Name}]的文件上传配置.");

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {


                newData.Sort = Repository.Select.Max(o => o.Sort) + 1;

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

        public Edit GetEdit(string id)
        {
            var entity = Repository.GetAndCheckNull(id);

            var result = Mapper.Map<Edit>(entity);

            return result;
        }

        public void Edit(Edit data)
        {
            var editData = Mapper.Map<Common_FileUploadConfig>(data).ModifyEntity();

            //@数据验证#待完善@
            if (Repository.Where(o => o.ParentId == data.ParentId && o.Name == editData.Name && o.Id != editData.Id).Any())
                throw new MessageException($"同层级下已存在名称为[{editData.Name}]的文件上传配置.");

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
                if (Repository.Delete(o => keys.Contains(o.Id)) <= 0)
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
                                        o.Sort
                                    });

            if (current == default)
                throw new MessageException("数据不存在或已被移除.");

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var target = data.Method switch
                {
                    SortMethod.top => Repository.Select.OrderBy(o => o.Sort)
                                                .First(o => new
                                                {
                                                    o.Id,
                                                    o.Sort
                                                }),
                    SortMethod.up => Repository.Select.OrderByDescending(o => o.Sort)
                                               .First(o => new
                                               {
                                                   o.Id,
                                                   o.Sort
                                               }),
                    SortMethod.down => Repository.Select.OrderBy(o => o.Sort)
                                                 .First(o => new
                                                 {
                                                     o.Id,
                                                     o.Sort
                                                 }),
                    SortMethod.low => Repository.Select.OrderByDescending(o => o.Sort)
                                                .First(o => new
                                                {
                                                    o.Id,
                                                    o.Sort
                                                }),
                    _ => throw new MessageException($"不支持的排序方法 {data.Method}."),
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

        public void DragSort(DragSort data)
        {
            if (data.Id == data.TargetId)
                return;

            var current = Repository.Where(o => o.Id == data.Id)
                                    .ToOne(o => new
                                    {
                                        o.Id,
                                        o.Sort
                                    });

            if (current == default)
                throw new MessageException("数据不存在或已被移除.");

            var target = Repository.Where(o => o.Id == data.TargetId)
                                    .ToOne(o => new
                                    {
                                        o.Id,
                                        o.Sort
                                    });

            if (target == default)
                throw new MessageException("目标数据不存在.");

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                dynamic target_new;

                if (data.Append)
                {
                    target_new = Repository.Where(o => o.Sort == target.Sort + 1)
                                         .First(o => new
                                         {
                                             o.Id,
                                             o.Sort
                                         });
                }
                else
                {
                    target_new = Repository.Where(o => o.Sort == current.Sort - 1)
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
                    throw new MessageException("文件上传配置排序失败.");

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

        public async Task DownloadTemplate(string version = ExcelVersion.xlsx, bool autogenerateTemplate = false)
        {
            var response = HttpContextAccessor.HttpContext.Response;

            if (autogenerateTemplate)
            {
                #region 直接发送预制模板

                response.Headers.Add("Content-Disposition", $"attachment; filename=\"{UrlEncoder.Default.Encode("文件上传配置导入模板")}.{version}\"");
                var filePath = PathHelper.GetAbsolutePath($"~/template/文件上传配置导入模板.{version}");
                if (!File.Exists(filePath))
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    throw new MessageException("模板文件不存在或已被移除.");
                }
                await response.SendFileAsync(filePath);

                #endregion
            }
            else
            {
                #region 自动生成模板

                var table = new DataTable("文件上传配置导入模板");

                typeof(Common_FileUploadConfig)
                    .GetProperties()
                    .ForEach(o =>
                    {
                        if (!o.HasTag(typeof(Import).GetMainTag()))
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

        public ImportResult Import(IFormFile file, bool autogenerateTemplate = false)
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

            var entityList = new List<Common_FileUploadConfig>();

            var errors = new List<ErrorInfo>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                var entity = new Common_FileUploadConfig();

                var _errors = new List<ErrorInfo>();

                entity.GetType().GetProperties().ForEach(o =>
                {
                    if (!o.HasTag(typeof(Import).GetMainTag()))
                        return;

                    var attr = o.GetCustomAttribute<DescriptionAttribute>();
                    if (attr == null)
                        return;

                    if (!table.Columns.Contains(attr.Description))
                    {
                        _errors.Add(new ErrorInfo(i + 3, attr.Description, "未找到该列。"));
                        return;
                    }

                    try
                    {
                        if (o.PropertyType == typeof(bool))
                            o.SetValue(entity, row[attr.Description].ToString() == "是");
                        else if (o.PropertyType == typeof(string))
                        {
                            var attrColumn = o.GetCustomAttribute<ColumnAttribute>();
                            if (attrColumn == null)
                                return;

                            var value = row[attr.Description].ToString();
                            if (Encoding.ASCII.GetBytes(value).Sum(c => c == 63 ? 2 : 1) > attrColumn.StringLength)
                            {
                                _errors.Add(new ErrorInfo(i + 3, attr.Description, $"数据长度过长, 不可超过{attrColumn.StringLength}个字符（其中每个中文占两个字符）。"));
                                return;
                            }
                            o.SetValue(entity, value);
                        }
                        else
                            o.SetValue(entity, row[attr.Description].ChangeType(o.PropertyType));
                    }
                    catch (Exception ex)
                    {
                        _errors.Add(new ErrorInfo(i + 3, attr.Description, "数据格式有误。"));

                        Logger.Log(
                            NLog.LogLevel.Error,
                            LogType.警告信息,
                            $"{file.FileName}文件第{i + 3}行第{attr.Description}列数据格式有误.",
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
                else if (entity.Name.IsNullOrWhiteSpace())
                {
                    errors.Add(new ErrorInfo(i + 3, "名称", "内容不能为空。"));
                    continue;
                }
                else if (entity.DisplayName.IsNullOrWhiteSpace())
                {
                    errors.Add(new ErrorInfo(i + 3, "显示名称", "内容不能为空。"));
                    continue;
                }
                else if (entity.LowerLimit <= 0)
                {
                    errors.Add(new ErrorInfo(i + 3, "文件数量下限", "文件数量下限必须大于或等于0。"));
                    continue;
                }
                else if (entity.UpperLimit <= 1)
                {
                    errors.Add(new ErrorInfo(i + 3, "文件数量上限", "文件数量上限必须大于或等于1。"));
                    continue;
                }

                entityList.Add(entity);
            }

            if (errors.Any())
                return new ImportResult(errors);

            var result = new ImportResult(0, 0);

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var newDataList = new List<Common_FileUploadConfig>();
                var editDataList = new List<Common_FileUploadConfig>();

                var orList = new List<Common_OperationRecord>();

                entityList.ForEach(entity =>
                {
                    //@数据匹配#待完善@
                    //var idSelect = Repository.Where(o => o.Name == entity.Name);
                    //if (idSelect.Any())
                    //{
                    //    entity.Id = idSelect.ToOne(o => o.Id);
                    //    editDataList.Add(entity.ModifyEntity());
                    //
                    //    orList.Add(new Common_OperationRecord
                    //    {
                    //        DataType = nameof(Common_FileUploadConfig),
                    //        DataId = entity.Id,
                    //        Explain = $"导入数据（更新）文件上传配置[@字段说明#待完善@]."
                    //    });
                    //}
                    //else
                    //{
                    //    newDataList.Add(entity.InitEntity());
                    //
                    //    orList.Add(new Common_OperationRecord
                    //    {
                    //        DataType = nameof(Common_FileUploadConfig),
                    //        DataId = entity.Id,
                    //        Explain = $"导入数据（新增）文件上传配置[@字段说明#待完善@]."
                    //    });
                    //}
                });

                result.AddTotal = newDataList.Count;
                result.UpdateTotal = editDataList.Count;

                if (editDataList.Any())
                    if (Repository.UpdateDiy
                         .SetSource(editDataList)
                         .UpdateColumns(typeof(Import).GetNamesWithTagAndOther(false, "_Edit").ToArray())
                         .ExecuteAffrows() <= 0)
                        throw new ApplicationException("文件上传配置更改失败.");

                if (newDataList.Any())
                    Repository.Insert(newDataList);

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

            var list = GetList(pagination);

            //动态生成模板
            var table = new DataTable("文件上传配置");

            var propDic = new Dictionary<string, PropertyInfo>();

            //生成列
            typeof(Common_FileUploadConfig)
                .GetProperties()
                .ForEach(o =>
                {
                    if (!o.HasTag(typeof(Export).GetMainTag()))
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
            var entity = Repository.GetAndCheckNull(id);

            var result = Mapper.Map<Config>(entity);

            return result;
        }

        public List<Config> GetConfigList(List<string> ids)
        {
            var entityList = Repository.Select
                                       .Where(o => ids.Contains(o.Id))
                                       .ToList<Common_FileUploadConfig, Config>(typeof(Config).GetNamesWithTagAndOther(true, "_List"));

            var result = Mapper.Map<List<Config>>(entityList);

            return result;
        }

        #endregion
    }
}

