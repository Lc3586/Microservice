using AutoMapper;
using Business.Interface.Common;
using Business.Utils;
using Business.Utils.Pagination;
using Entity.Common;
using FreeSql;
using Microservice.Library.DataMapping.Gen;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using Microservice.Library.OpenApi.Extention;
using Model.Common.QuickInputDTO;
using Model.Utils.Pagination;
using System.Collections.Generic;

namespace Business.Implementation.Common
{
    /// <summary>
    /// 快捷输入业务类
    /// </summary>
    public class QuickInputBusiness : BaseBusiness, IQuickInputBusiness
    {
        #region DI

        public QuickInputBusiness(
            IFreeSqlProvider freeSqlProvider,
            IAutoMapperProvider autoMapperProvider)
        {
            Mapper = autoMapperProvider.GetMapper();
            Orm = freeSqlProvider.GetFreeSql();
            Repository = Orm.GetRepository<Common_QuickInput, string>();
        }

        #endregion

        #region 私有成员

        readonly IMapper Mapper;

        readonly IFreeSql Orm;

        readonly IBaseRepository<Common_QuickInput, string> Repository;

        #endregion

        #region 外部接口

        public List<List> GetList(PaginationDTO pagination)
        {
            var entityList = Repository.Select
                                    .GetPagination(pagination)
                                    .ToList<Common_QuickInput, List>(typeof(List).GetNamesWithTagAndOther(true, "_List"));

            var result = Mapper.Map<List<List>>(entityList);

            return result;
        }

        public List<List> GetCurrentAccountMatchList(string category, string keyword, bool paging = false, int rows = 50, int page = 1)
        {
            if (category.IsNullOrWhiteSpace() || keyword.IsNullOrWhiteSpace())
                return new List<List>();

            var iSelect = Orm.Select<Common_QuickInput>()
                                .Where(o => (o.Public == true || o.CreatorId == Operator.AuthenticationInfo.Id) && o.Category == category && o.Keyword.Contains(keyword));

            if (paging)
                iSelect.Page(page, rows);

            var entityList = iSelect.ToList<Common_QuickInput, List>(typeof(List).GetNamesWithTagAndOther(true, "_List"));

            var result = Mapper.Map<List<List>>(entityList);

            return result;
        }

        public void Create(Create data)
        {
            var history = Repository.Where(o =>
                                            ((data.Public == true && o.Public == true)
                                            || (data.Public == false && o.CreatorId == Operator.AuthenticationInfo.Id))
                                            && o.Category == data.Category
                                            && o.Keyword == data.Keyword)
                                    .ToOne(o => new
                                    {
                                        o.Id,
                                        o.Content
                                    });

            //已存在则不新增
            if (history != default && history.Content == data.Content)
                return;

            var newData = Mapper.Map<Common_QuickInput>(data).InitEntity();
            newData.InitEntity();
            Repository.Insert(newData);
        }

        public void BatchCreate(List<Create> datas)
        {
            datas.ForEach(o => Create(o));
        }

        public void Delete(List<string> ids)
        {
            var entityList = Repository.Select.Where(c => ids.Contains(c.Id) && (Operator.IsAdmin == true || c.CreatorId == Operator.AuthenticationInfo.Id)).ToList(c => c.Id);

            if (entityList.Any_Ex() && Repository.Delete(o => ids.Contains(o.Id)) < 0)
                throw new MessageException("删除失败");
        }

        #endregion
    }
}
