﻿using AutoMapper;
using Business.Utils.Pagination;
using Entity.System;
using FreeSql;
using Microservice.Library.Container;
using Microservice.Library.DataMapping.Gen;
using Microservice.Library.Elasticsearch;
using Microservice.Library.Elasticsearch.Gen;
using Microservice.Library.File;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using Microservice.Library.OpenApi.Extention;
using Microsoft.AspNetCore.Http;
using Model.System.LogDTO;
using Model.Utils.Config;
using Model.Utils.Log;
using Model.Utils.Pagination;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FileInfo = Model.Utils.Log.LogDTO.FileInfo;

namespace Business.Utils.Log
{
    /// <summary>
    /// 日志业务类
    /// </summary>
    public class LogBusiness : BaseBusiness, ILogBusiness
    {
        #region DI

        public LogBusiness(
            IFreeSqlProvider freeSqlProvider,
            IAutoMapperProvider autoMapperProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            Orm = freeSqlProvider.GetFreeSql();
            Repository = Orm.GetRepository<System_Log, string>();
            Mapper = autoMapperProvider.GetMapper();
            HttpContextAccessor = httpContextAccessor;

            FileDir = Path.Combine(Config.AbsoluteStorageDirectory, NLoggerConfig.FileDic);
            FileNameFormat = NLoggerConfig.FileNameFormat;
            FileSuffix = NLoggerConfig.FileNameFormat[NLoggerConfig.FileNameFormat.LastIndexOf('.')..];
        }

        #endregion

        #region 私有成员

        readonly IFreeSql Orm;

        readonly IBaseRepository<System_Log, string> Repository;

        ElasticsearchClient ESClient;

        readonly IMapper Mapper;

        readonly IHttpContextAccessor HttpContextAccessor;

        readonly string FileDir;

        readonly string FileNameFormat;

        readonly string FileSuffix;

        ElasticsearchClient GetElasticsearchClient()
        {
            if (!Config.EnableElasticsearch)
                throw new MessageException("未启用Elasticsearch.");

            if (ESClient == null)
                ESClient = AutofacHelper.GetScopeService<IElasticsearchProvider>()
                                        .GetElasticsearch<System_Log>();

            return ESClient;
        }

        #endregion

        #region 公共

        public string GetDefaultType()
        {
            return Config.DefaultLoggerType.ToString();
        }

        public List<string> GetLogLevels()
        {
            var result = LogLevel.AllLoggingLevels.Select(o => o.Name).ToList();
            result.Add("None");
            return result;
        }

        public List<string> GetLogTypes()
        {
            var result = LogType.AllTypes().Select(o => o.Value).ToList();
            result.Add("None");
            return result;
        }

        public List<FileInfo> GetFileList(DateTime start, DateTime end)
        {
            var result = new List<FileInfo>();

            if (!Directory.Exists(FileDir))
                throw new MessageException("根目录不存在或已被移除.");

            start = start.AddDays(-1).Date;

            if (end > DateTime.Now)
                end = DateTime.Now;

            end = end.Date;

            while (start <= end)
            {
                start = start.AddDays(1);

                var path = Path.Combine(FileDir, string.Format(FileNameFormat, start));
                if (!File.Exists(path))
                    continue;

                var file = new System.IO.FileInfo(path);
                result.Add(new FileInfo
                {
                    Name = file.Name.Substring(0, file.Name.LastIndexOf('.')),
                    Suffix = FileSuffix,
                    Bytes = file.Length.ToString(),
                    Size = FileHelper.GetFileSize(file.Length),
                    CreateTime = file.CreationTime,
                    LastWriteTime = file.LastWriteTime
                });
            }

            return result;
        }

        public async Task GetFileContent(string name)
        {
            var filename = $"{name}{FileSuffix}";

            var path = Path.Combine(FileDir, filename);

            if (!File.Exists(path))
                throw new MessageException("文件不存在或已被移除.");

            var response = HttpContextAccessor.HttpContext.Response;
            response.ContentType = "text/plain";

            await response.SendFileAsync(path);
        }

        public async Task DownloadFile(string name)
        {
            var filename = $"{name}{FileSuffix}";

            var path = Path.Combine(FileDir, filename);

            if (!File.Exists(path))
                throw new MessageException("文件不存在或已被移除.");

            var response = HttpContextAccessor.HttpContext.Response;
            response.ContentType = "text/plain";
            response.Headers.Add("Content-Disposition", $"attachment; filename=\"{UrlEncoder.Default.Encode(filename)}\"");

            await response.SendFileAsync(path);
        }

        public List<List> GetESList(PaginationDTO pagination)
        {
            var sql = string.Empty;

            if (!pagination.FilterToSql(ref sql, out Dictionary<string, object> @params))
                throw new MessageException("搜索条件不支持");

            if (!pagination.OrderByToSql(ref sql, null, '"', true))
                throw new MessageException("排序条件不支持");

            var list = GetElasticsearchClient().SearchWithSql<System_Log>(
                out long recordCount,
                Array.Empty<string>(),
                sql.ToLower(),
                pagination.PageRows,
                60,
                true);

            pagination.RecordCount = recordCount;

            var result = Mapper.Map<List<List>>(list);

            return result;
        }

        public Detail GetESDetail(string id)
        {
            var data = GetElasticsearchClient().Get<System_Log>(id);

            var result = Mapper.Map<Detail>(data);

            return result;
        }

        public List<List> GetDBList(PaginationDTO pagination)
        {
            var list = Orm.Select<System_Log>()
                                .GetPagination(pagination)
                                .ToList<System_Log, List>(typeof(List).GetNamesWithTagAndOther(true, "_List"));

            var result = Mapper.Map<List<List>>(list);

            return result;
        }

        public Detail GetDBDetail(string id)
        {
            var data = Repository.GetAndCheckNull(id);

            var result = Mapper.Map<Detail>(data);

            return result;
        }

        #endregion
    }
}
