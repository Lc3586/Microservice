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

        /// <summary>
        /// 获取当前账号匹配的选项数据
        /// </summary>
        /// <param name="category"></param>
        /// <param name="keyword"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        List<OptionList> CurrentAccountOptionList(string category, string keyword, PaginationDTO pagination = null)
        {
            if (category.IsNullOrWhiteSpace() || keyword.IsNullOrWhiteSpace())
                return new List<OptionList>();

            var iSelect = Orm.Select<Common_QuickInput>()
                                .Where(o => (o.Public == true || o.CreatorId == Operator.AuthenticationInfo.Id) && o.Category == category && o.Keyword.Contains(keyword) && o.Keyword != keyword);

            if (pagination != null)
                iSelect.GetPagination(pagination);

            return iSelect.ToList(o => new OptionList
            {
                Content = o.Content
            });
        }

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

        public List<OptionList> GetCurrentAccountOptionList(string category, string keyword)
        {
            var result = CurrentAccountOptionList(category, keyword);

            return result;
        }

        public List<OptionList> GetCurrentAccountOptionList(string category, string keyword, PaginationDTO pagination)
        {
            var result = CurrentAccountOptionList(category, keyword, pagination);

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

        public void BatchCreate(BatchCreate data)
        {
            data.Datas?.ForEach(o => Create(o));
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
