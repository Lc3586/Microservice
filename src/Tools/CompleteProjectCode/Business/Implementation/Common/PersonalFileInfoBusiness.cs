using AutoMapper;
using Business.Interface.Common;
using Business.Utils;
using Business.Utils.Pagination;
using Entity.Common;
using FreeSql;
using Microservice.Library.Container;
using Microservice.Library.DataMapping.Gen;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using Microservice.Library.OpenApi.Extention;
using Microsoft.AspNetCore.Http;
using Model.Common.PersonalFileInfoDTO;
using Model.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Implementation.Common
{
    /// <summary>
    /// 个人文件信息业务类
    /// </summary>
    public class PersonalFileInfoBusiness : BaseBusiness, IPersonalFileInfoBusiness, IDependency
    {
        #region DI

        public PersonalFileInfoBusiness(
            IFreeSqlProvider freeSqlProvider,
            IAutoMapperProvider autoMapperProvider,
            IHttpContextAccessor httpContextAccessor,
            IFileBusiness fileBusiness)
        {
            Orm = freeSqlProvider.GetFreeSql();
            Mapper = autoMapperProvider.GetMapper();
            Repository = Orm.GetRepository<Common_PersonalFileInfo, string>();
            HttpContextAccessor = httpContextAccessor;
            FileBusiness = fileBusiness;
            HttpResponse = HttpContextAccessor?.HttpContext?.Response;
        }

        #endregion

        #region 私有成员

        readonly IMapper Mapper;

        readonly IFreeSql Orm;

        readonly IBaseRepository<Common_PersonalFileInfo, string> Repository;

        readonly IHttpContextAccessor HttpContextAccessor;

        readonly IFileBusiness FileBusiness;

        readonly HttpResponse HttpResponse;

        #endregion

        #region 外部接口

        public List<PersonalFileInfo> GetList(PaginationDTO pagination)
        {
            var entityList = Repository.Where(o => Operator.IsAdmin == true || o.CreatorId == Operator.AuthenticationInfo.Id)
                        .GetPagination(pagination)
                        .ToList<Common_PersonalFileInfo, PersonalFileInfo>(typeof(PersonalFileInfo).GetNamesWithTagAndOther(true, "_List"));

            var result = Mapper.Map<List<PersonalFileInfo>>(entityList);

            return result;
        }

        public PersonalFileInfo GetDetail(string id)
        {
            var entity = Repository.GetAndCheckNull(id);

            var result = Mapper.Map<PersonalFileInfo>(entity);

            return result;
        }

        public List<PersonalFileInfo> GetDetails(string ids)
        {
            var list = GetList(new PaginationDTO { PageIndex = -1, DynamicFilterInfo = new List<PaginationDynamicFilterInfo> { new PaginationDynamicFilterInfo { Field = "Id", Compare = FilterCompare.inSet, Value = ids } } });
            return list;
        }

        public List<PersonalFileInfo> GetDetails(List<string> ids)
        {
            var list = GetList(new PaginationDTO { PageIndex = -1, DynamicFilterInfo = new List<PaginationDynamicFilterInfo> { new PaginationDynamicFilterInfo { Field = "Id", Compare = FilterCompare.inSet, Value = ids } } });
            return list;
        }

        public void Rename(string id, string filename)
        {
            var file_extension = Repository.GetAndCheckNull(id, "文件不存在或已被删除.");
            file_extension.Name = filename;
            file_extension.ModifyEntity();
            Repository.Update(file_extension);
        }

        public Edit GetEdit(string id)
        {
            var entity = Repository.GetAndCheckNull(id);

            var result = Mapper.Map<Edit>(entity);

            return result;
        }

        public void Edit(Edit data)
        {
            var editData = Mapper.Map<Common_PersonalFileInfo>(data).ModifyEntity();

            //@数据验证#待完善@
            //if (Repository.Where(o => o.Name == editData.Name && o.Id != editData.Id).Any())
            //    throw new MessageException($"同层级下已存在名称为[{editData.Name}]的文件上传配置.");

            if (Repository.UpdateDiy
                  .SetSource(editData)
                  .UpdateColumns(typeof(Edit).GetNamesWithTagAndOther(false, "_Edit").ToArray())
                  .ExecuteAffrows() <= 0)
                throw new MessageException("修改个人文件信息失败");
        }


        public async Task Preview(string id, int width, int height, TimeSpan? time = null)
        {
            var file_extension = Repository.Get(id);
            if (file_extension == null)
            {
                Common.FileBusiness.Response(HttpResponse, StatusCodes.Status404NotFound, "文件不存在或已被删除.");
                return;
            }

            await FileBusiness.Preview(file_extension.FileId, width, height, time);
        }

        public async Task Browse(string id)
        {
            var file_extension = Repository.Get(id);
            if (file_extension == null)
            {
                Common.FileBusiness.Response(HttpResponse, StatusCodes.Status404NotFound, "文件不存在或已被删除.");
                return;
            }

            await FileBusiness.Browse(file_extension.FileId);
        }

        public async Task Download(string id, string rename = null)
        {
            var file_extension = Repository.Get(id);
            if (file_extension == null)
            {
                Common.FileBusiness.Response(HttpResponse, StatusCodes.Status404NotFound, "文件不存在或已被删除.");
                return;
            }

            await FileBusiness.Download(file_extension.FileId, rename.IsNullOrWhiteSpace() ? file_extension.Name : rename);
        }

        public async Task Download(string id, string dirPath, string rename = null)
        {
            var file_extension = Repository.Get(id);
            if (file_extension == null)
            {
                Common.FileBusiness.Response(HttpResponse, StatusCodes.Status404NotFound, "文件不存在或已被删除.");
                return;
            }

            await FileBusiness.Download(file_extension.FileId, dirPath, rename.IsNullOrWhiteSpace() ? file_extension.Name : rename);
        }

        public void Delete(List<string> ids)
        {
            if (Repository.Delete(o => ids.Contains(o.Id)) <= 0)
                throw new MessageException("未删除任何数据.", ids);
        }

        #endregion
    }
}
