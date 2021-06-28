using AutoMapper;
using Business.Interface.Common;
using Business.Interface.Example;
using Business.Utils;
using Business.Utils.Filter;
using Business.Utils.Pagination;
using Entity.Common;
using Entity.Example;
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
using Model.Example.DBDTO;
using Model.Utils.OfficeDocuments;
using Model.Utils.OfficeDocuments.ExcelDTO;
using Model.Utils.Pagination;
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

namespace Business.Implementation.Example
{
    /// <summary>
    /// 操作记录业务类
    /// </summary>
    public class SampleBusiness : BaseBusiness, ISampleBusiness
    {
        #region DI

        public SampleBusiness(
            IFreeSqlProvider freeSqlProvider,
            IAutoMapperProvider autoMapperProvider,
            IOperationRecordBusiness operationRecordBusiness,
            IHttpContextAccessor httpContextAccessor)
        {
            Mapper = autoMapperProvider.GetMapper();
            Orm = freeSqlProvider.GetFreeSql();
            Repository = Orm.GetRepository<Sample_DB, string>();
            OperationRecordBusiness = operationRecordBusiness;
            HttpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region 私有成员

        readonly IMapper Mapper;

        readonly IFreeSql Orm;

        readonly IBaseRepository<Sample_DB, string> Repository;

        readonly IOperationRecordBusiness OperationRecordBusiness;

        readonly IHttpContextAccessor HttpContextAccessor;

        #endregion

        #region 外部接口

        #region 基础功能

        public List<List> GetList(PaginationDTO pagination)
        {
            var entityList = Repository.Select
                                    .GetPagination(pagination)
                                    .ToList<Sample_DB, List>(typeof(List).GetNamesWithTagAndOther(true, "_List"));

            var result = Mapper.Map<List<List>>(entityList);

            return result;
        }

        public Detail GetDetail(string id)
        {
            var entity = Repository.GetAndCheckNull(id);

            var result = Mapper.Map<Detail>(entity);

            return result;
        }

        public void Create(Create data)
        {
            var newData = Mapper.Map<Sample_DB>(data).InitEntity();

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                Repository.Insert(newData);

                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(Sample_DB),
                    DataId = newData.Id,
                    Explain = $"创建示例数据[名称 {newData.Name}]."
                });
            });

            if (!success)
                throw new MessageException("创建示例数据失败", ex);
        }

        public Edit GetEdit(string id)
        {
            var entity = Repository.GetAndCheckNull(id);

            var result = Mapper.Map<Edit>(entity);

            return result;
        }

        public void Edit(Edit data)
        {
            var editData = Mapper.Map<Sample_DB>(data).ModifyEntity();

            var entity = Repository.GetAndCheckNull(editData.Id);

            var changed_ = string.Join(",",
                                       entity.GetPropertyValueChangeds<Sample_DB, Edit>(editData)
                                            .Select(p => $"\r\n\t {p.Description}：{p.FormerValue} 更改为 {p.CurrentValue}"));

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(Sample_DB),
                    DataId = entity.Id,
                    Explain = $"修改示例数据[名称 {entity.Name}].",
                    Remark = $"变更详情: \r\n\t{changed_}"
                });

                if (Repository.UpdateDiy
                      .SetSource(editData.ModifyEntity())
                      .UpdateColumns(typeof(Edit).GetNamesWithTagAndOther(false, "_Edit").ToArray())
                      .ExecuteAffrows() <= 0)
                    throw new MessageException("修改示例数据失败");
            });

            if (!success)
                throw ex;
        }

        [AdministratorOnly]
        public void Delete(List<string> ids)
        {
            var entityList = Repository.Select.Where(c => ids.Contains(c.Id)).ToList(c => new { c.Id, c.Name });

            var orList = new List<Common_OperationRecord>();

            foreach (var entity in entityList)
            {
                orList.Add(new Common_OperationRecord
                {
                    DataType = nameof(Sample_DB),
                    DataId = entity.Id,
                    Explain = $"删除示例数据[名称 {entity.Name}]."
                });
            }

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                if (Repository.Delete(o => ids.Contains(o.Id)) <= 0)
                    throw new MessageException("未删除任何数据");

                var orIds = OperationRecordBusiness.Create(orList);
            });

            if (!success)
                throw new MessageException("删除示例数据失败", ex);
        }

        #endregion

        #region 拓展功能

        public void Enable(string id, bool enable)
        {
            var entity = Repository.GetAndCheckNull(id);

            entity.Enable = enable;

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(Sample_DB),
                    DataId = entity.Id,
                    Explain = $"{(enable ? "启用" : "禁用")}示例数据[名称 {entity.Name}]."
                });

                if (Repository.Update(entity) <= 0)
                    throw new MessageException($"{(enable ? "启用" : "禁用")}示例数据失败");
            });

            if (!success)
                throw ex;
        }

        public async Task DownloadTemplate(string version = "xlsx", bool autogenerateTemplate = false)
        {
            var response = HttpContextAccessor.HttpContext.Response;

            if (autogenerateTemplate)
            {
                #region 方式一: 直接发送预制模板

                response.Headers.Add("Content-Disposition", $"attachment; filename=\"{UrlEncoder.Default.Encode("示例导入模板")}.{version}\"");
                var filePath = PathHelper.GetAbsolutePath($"~/template/示例导入模板.{version}");
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
                #region 方式二: 自动生成模板

                var table = new DataTable("示例导入模板");

                typeof(Sample_DB)
                    .GetProperties()
                    .ForEach(o =>
                    {
                        if (!o.HasTag(typeof(Create).GetMainTag()))
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

            var entityList = new List<Sample_DB>();

            var errors = new List<ErrorInfo>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                var entity = new Sample_DB();

                var _errors = new List<ErrorInfo>();

                entity.GetType().GetProperties().ForEach(o =>
                {
                    if (!o.HasTag(typeof(Create).GetMainTag()))
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
                    catch
                    {
                        _errors.Add(new ErrorInfo(i + 3, attr.Description, "数据格式有误。"));
                    }
                });

                if (_errors.Any())
                {
                    errors.AddRange(_errors);
                    continue;
                }

                if (entity.Name.IsNullOrEmpty())
                {
                    errors.Add(new ErrorInfo(i + 3, "名称", "内容不能为空。"));
                    continue;
                }
                else if (entity.Content.IsNullOrEmpty())
                {
                    errors.Add(new ErrorInfo(i + 3, "内容", "内容不能为空。"));
                    continue;
                }

                entityList.Add(entity);
            }

            if (errors.Any())
                return new ImportResult(errors);

            var newDataList = new List<Sample_DB>();
            var editDataList = new List<Sample_DB>();

            entityList.ForEach(entity =>
            {
                var idSelect = Repository.Where(o => o.Name == entity.Name);
                if (idSelect.Any())
                {
                    entity.Id = idSelect.ToOne(o => o.Id);
                    editDataList.Add(entity.ModifyEntity());
                }
                else
                {
                    newDataList.Add(entity.InitEntity());
                }
            });

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                if (editDataList.Any())
                    if (Repository.UpdateDiy
                         .SetSource(editDataList)
                         .UpdateColumns(typeof(Edit).GetNamesWithTagAndOther(false, "_Edit").ToArray())
                         .ExecuteAffrows() <= 0)
                        throw new ApplicationException("示例更改失败.");

                if (newDataList.Any())
                    Repository.Insert(newDataList);
            });

            if (!success)
                throw new ApplicationException("示例导入失败.", ex);

            return new ImportResult(newDataList.Count, editDataList.Count, errors);
        }

        public void Export(string version = "xlsx", string paginationJson = null)
        {
            PaginationDTO pagination;
            if (!paginationJson.IsNullOrWhiteSpace())
                pagination = paginationJson.ToObject<PaginationDTO>();
            else
                pagination = new PaginationDTO { PageIndex = -1 };

            var list = GetList(pagination);

            //动态生成模板
            var table = new DataTable("示例");

            var propDic = new Dictionary<string, PropertyInfo>();

            //生成列
            typeof(Sample_DB)
                .GetProperties()
                .ForEach(o =>
                {
                    if (!o.HasTag(typeof(List).GetMainTag()))
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
            response.Headers.Add("Content-Disposition", $"attachment; filename=\"{UrlEncoder.Default.Encode("示例")}.{version}\"");
            response.StatusCode = StatusCodes.Status200OK;
            response.ContentType = "application/octet-stream";
            response.ContentLength = bytes.Length;
            response.Body.Write(bytes);
        }

        #endregion

        #endregion
    }
}
