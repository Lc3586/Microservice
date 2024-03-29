﻿using AutoMapper;
using Business.Interface.Common;
using Business.Interface.System;
using Business.Utils;
using Business.Utils.Filter;
using Business.Utils.Pagination;
using Entity.Common;
using Entity.Public;
using FreeSql;
using Microservice.Library.DataMapping.Gen;
using Microservice.Library.Extension;
using Microservice.Library.FreeSql.Extention;
using Microservice.Library.FreeSql.Gen;
using Microservice.Library.OpenApi.Extention;
using Microservice.Library.SelectOption;
using Model.Common;
using Model.Public.MemberDTO;
using Model.System;
using Model.Utils.Pagination;
using Model.Utils.SampleAuthentication.SampleAuthenticationDTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Implementation.Public
{
    /// <summary>
    /// 会员业务类
    /// </summary>
    public class MemberBusiness : BaseBusiness, IMemberBusiness
    {
        #region DI

        public MemberBusiness(
            IFreeSqlProvider freeSqlProvider,
            IAutoMapperProvider autoMapperProvider,
            IOperationRecordBusiness operationRecordBusiness,
            IAuthoritiesBusiness authoritiesBusiness,
            IEntryLogBusiness entryLogBusiness)
        {
            Orm = freeSqlProvider.GetFreeSql();
            Repository = Orm.GetRepository<Public_Member, string>();
            Repository_MemberRole = Orm.GetRepository<Public_MemberRole, string>();
            Mapper = autoMapperProvider.GetMapper();
            OperationRecordBusiness = operationRecordBusiness;
            AuthoritiesBusiness = authoritiesBusiness;
            EntryLogBusiness = entryLogBusiness;
        }

        #endregion

        #region 私有成员

        readonly IFreeSql Orm;

        readonly IBaseRepository<Public_Member, string> Repository;

        readonly IBaseRepository<Public_MemberRole, string> Repository_MemberRole;

        readonly IMapper Mapper;

        readonly IOperationRecordBusiness OperationRecordBusiness;

        readonly IAuthoritiesBusiness AuthoritiesBusiness;

        readonly IEntryLogBusiness EntryLogBusiness;

        #endregion

        #region 外部接口

        #region 基础功能

        public List<List> GetList(PaginationDTO pagination)
        {
            var entityList = Repository.Select
                                    .GetPagination(pagination)
                                    .ToList<Public_Member, List>(typeof(List).GetNamesWithTagAndOther(true, "_List"));

            var result = Mapper.Map<List<List>>(entityList);

            return result;
        }

        public List<SelectOption> DropdownList(string condition, PaginationDTO pagination)
        {
            var fields = new[] {
                nameof(Public_Member.Account),
                nameof(Public_Member.Name)
            };

            var where = new List<string>();
            if (!condition.IsNullOrWhiteSpace())
            {
                var index = 0;
                var add = false;
                foreach (var item in condition)
                {
                    if (item != ' ')
                    {
                        if (where.Count < index + 1)
                            where.Add("");
                        where[index] += item;
                        add = false;
                    }
                    else
                    {
                        if (add)
                            continue;
                        index++;
                        add = true;
                    }
                }
            }

            var where_sql = where.Any() ? string.Join(" or ", where.Select(o => string.Join(" or ", fields.Select(p => $"a.\"{p}\" like '%{o}%'")))) : null;

            var type = typeof(Public_Member);

            var select = SelectExtension.Select<Public_Member, SelectOption>(a => new SelectOption
            {
                text = a.Account,
                value = a.Id,
                search = $"{a.Account} {a.Name}",
                options = new List<OptionInfo>
                {
                    new OptionInfo
                    {
                        display = type.GetDescription(nameof(a.Account)),
                        value = a.Account
                    },
                    new OptionInfo
                    {
                        display =type.GetDescription(nameof(a.Nickname)),
                        value = a.Nickname
                    },
                    new OptionInfo
                    {
                        display =type.GetDescription(nameof(a.Face)),
                        value = a.Face,
                        displayType = OptionDisplayType.image
                    },
                    new OptionInfo
                    {
                        display =type.GetDescription(nameof(a.Sex)),
                        value = a.Sex
                    },
                    new OptionInfo
                    {
                        display =type.GetDescription(nameof(a.Tel)),
                        value = a.Tel
                    },
                    new OptionInfo
                    {
                        display =type.GetDescription(nameof(a.Name)),
                        value = a.Name
                    },
                    new OptionInfo
                    {
                        display = type.GetDescription(nameof(a.CreateTime)),
                        value = a.CreateTime
                    }
                }
            });

            var list = from a in Orm.Select<Public_Member>()
                            .Where(where_sql)
                            .GetPagination(pagination)
                            .ToList<Public_Member, List>(typeof(List).GetNamesWithTagAndOther(true, "_List"))
                       select @select.Invoke(a);

            return list.ToList();
        }

        public Detail GetDetail(string id)
        {
            var entity = Repository.GetAndCheckNull(id);

            var result = Mapper.Map<Detail>(entity);

            return result;
        }

        public string Create(Create data, bool runTransaction = true)
        {
            var newData = Mapper.Map<Public_Member>(data).InitEntity();

            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(Public_Member),
                    DataId = newData.Id,
                    Explain = $"创建会员[账号 {newData.Account}, 昵称 {newData.Nickname}, 姓名 {newData.Name}]."
                });

                Repository.Insert(newData);

                AuthoritiesBusiness.AutoAuthorizeRoleForMember(new Model.System.AuthorizeDTO.RoleForMember
                {
                    MemberIds = new List<string> { newData.Id }
                }, false);
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("创建会员失败.", ex);
            }
            else
                handler();

            return newData.Id;
        }

        public Edit GetEdit(string id)
        {
            var entity = Repository.GetAndCheckNull(id);

            var result = Mapper.Map<Edit>(entity);

            return result;
        }

        public void Edit(Edit data, bool runTransaction = true)
        {
            var editData = Mapper.Map<Public_Member>(data).ModifyEntity();

            var entity = Repository.GetAndCheckNull(editData.Id);

            var changed_ = string.Join(",",
                                       entity.GetPropertyValueChangeds<Public_Member, Edit>(editData)
                                            .Select(p => $"\r\n\t {p.Description}：{p.FormerValue} 更改为 {p.CurrentValue}"));

            void handler()
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(Public_Member),
                    DataId = entity.Id,
                    Explain = $"修改会员[账号 {entity.Account}, 昵称 {entity.Nickname}, 姓名 {entity.Name}].",
                    Remark = $"变更详情: \r\n\t{changed_}"
                });

                if (Repository.UpdateDiy
                      .SetSource(editData.ModifyEntity())
                      .UpdateColumns(typeof(Edit).GetNamesWithTagAndOther(false, "_Edit").ToArray())
                      .ExecuteAffrows() <= 0)
                    throw new MessageException("修改会员失败.");
            }

            if (runTransaction)
            {
                (bool success, Exception ex) = Orm.RunTransaction(handler);

                if (!success)
                    throw new MessageException("修改会员失败.", ex);
            }
            else
                handler();
        }

        public void Delete(List<string> ids)
        {
            var entityList = Repository.Select.Where(c => ids.Contains(c.Id)).ToList(c => new { c.Id, c.Account, c.Nickname, c.Name });

            var orList = new List<Common_OperationRecord>();

            foreach (var entity in entityList)
            {
                orList.Add(new Common_OperationRecord
                {
                    DataType = nameof(Public_Member),
                    DataId = entity.Id,
                    Explain = $"删除会员[账号 {entity.Account}, 昵称 {entity.Nickname}, 姓名 {entity.Name}]."
                });
            }

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                AuthoritiesBusiness.RevocationRoleForMember(ids, false);

                var orIds = OperationRecordBusiness.Create(orList);

                if (Repository.Delete(o => ids.Contains(o.Id)) <= 0)
                    throw new MessageException("未删除任何数据.");
            });

            if (!success)
                throw new MessageException("删除会员失败.", ex);
        }

        #endregion

        #region 拓展功能

        [AdministratorOnly]
        public void Enable(string id, bool enable)
        {
            var entity = Repository.GetAndCheckNull(id);

            entity.Enable = enable;

            (bool success, Exception ex) = Orm.RunTransaction(() =>
            {
                var orId = OperationRecordBusiness.Create(new Common_OperationRecord
                {
                    DataType = nameof(Public_Member),
                    DataId = entity.Id,
                    Explain = $"{(enable ? "启用" : "禁用")}会员[账号 {entity.Account}, 昵称 {entity.Nickname}, 姓名 {entity.Name}]."
                });

                if (Repository.Update(entity) <= 0)
                    throw new MessageException($"{(enable ? "启用" : "禁用")}会员失败");
            });

            if (!success)
                throw ex;
        }

        public AuthenticationInfo WeChatLogin(string appId, string openId)
        {
            var member = Repository.Where(o => o.WeChatUserInfos.AsSelect().Where(p => p.AppId == appId && p.OpenId == openId).Any())
                .ToOne(o => new { o.Id, o.Account, o.Name, o.Nickname, o.Sex, o.Face, o.Enable });

            if (member == default)
                throw new MessageException("账号还未绑定微信.");

            if (!member.Enable)
                throw new MessageException("账号已被禁用.");

            EntryLogBusiness.Create(new Common_EntryLog
            {
                UserType = UserType.会员,
                Account = member.Account,
                Name = member.Name,
                Nickname = member.Nickname,
                Face = member.Face,
                IsAdmin = false,
                Remark = "使用微信信息登录系统.",
                CreatorId = member.Id
            });

            return new AuthenticationInfo
            {
                Id = member.Id,
                UserType = UserType.会员,
                RoleTypes = AuthoritiesBusiness.GetMemberRoleTypes(member.Id),
                RoleNames = AuthoritiesBusiness.GetMemberRoleNames(member.Id),
                Account = member.Account,
                Nickname = member.Nickname,
                Sex = member.Sex,
                Face = member.Face
            };
        }

        public AuthenticationInfo Login(string memberId)
        {
            var member = Repository.Where(o => o.Id == memberId)
                            .ToOne(o => new { o.Id, o.Account, o.Name, o.Nickname, o.Sex, o.Face, o.Enable });

            if (member == default)
                throw new MessageException("账号还未绑定微信.");

            if (!member.Enable)
                throw new MessageException("账号已被禁用.");

            EntryLogBusiness.Create(new Common_EntryLog
            {
                UserType = UserType.会员,
                Account = member.Account,
                Name = member.Name,
                Nickname = member.Nickname,
                Face = member.Face,
                IsAdmin = false,
                Remark = "使用开发功能登录系统.",
                CreatorId = member.Id
            });

            return new AuthenticationInfo
            {
                Id = member.Id,
                UserType = UserType.会员,
                RoleTypes = AuthoritiesBusiness.GetMemberRoleTypes(member.Id),
                RoleNames = AuthoritiesBusiness.GetMemberRoleNames(member.Id),
                Account = member.Account,
                Nickname = member.Nickname,
                Sex = member.Sex,
                Face = member.Face
            };
        }

        public OperatorUserInfo GetOperatorDetail(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            return Repository.Where(o => o.Id == id)
                .ToOne(o => new OperatorUserInfo
                {
                    Account = o.Account,
                    Name = o.Nickname,
                    Tel = o.Tel
                });
        }

        #endregion

        #endregion
    }
}
