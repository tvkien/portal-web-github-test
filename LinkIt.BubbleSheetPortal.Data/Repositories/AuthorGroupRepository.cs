using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AuthorGroupRepository : IRepository<AuthorGroup>, IAuthorGroupRepository
    {
        private readonly Table<AuthorGroupEntity> table;
        private readonly Table<AuthorGroupListView> tableAuthorGroupListView;
        private readonly Table<UserByUserSchoolView> tableUserByUserSchool; 
        private readonly AssessmentDataContext dbContext;

        public AuthorGroupRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<AuthorGroupEntity>();
            tableAuthorGroupListView = AssessmentDataContext.Get(connectionString).GetTable<AuthorGroupListView>();
            tableUserByUserSchool = AssessmentDataContext.Get(connectionString).GetTable<UserByUserSchoolView>();
            dbContext = AssessmentDataContext.Get(connectionString);
            Mapper.CreateMap<AuthorGroup, AuthorGroupEntity>()
                .ForMember(dest => dest.AuthorGroupID, opt => opt.MapFrom(src => src.Id));
        }

        public IQueryable<AuthorGroup> Select()
        {
            return table.Select(x => new AuthorGroup
                                     {
                                         DistrictId = x.DistrictID,
                                         Id = x.AuthorGroupID,
                                         Level = x.Level,
                                         Name = x.Name,
                                         SchoolId = x.SchoolID,
                                         StateId = x.StateID,
                                         UserId = x.UserID
                                     });
        }

        public IQueryable<GetAuthorGroupListResult> GetAuthorGroupListHasAccessTo(int userId, int stateId, int districtId, int schoolId)
        {
            return dbContext.GetAuthorGroupList(userId, stateId, districtId, schoolId).AsQueryable();                
        }

        public IQueryable<AuthorGroupList> GetAuthorGroupList(int stateId, int districtId, int schoolId)
        {
            var query = tableAuthorGroupListView.Select(en=>new AuthorGroupList
                                                                {
                                                                    AuthorGroupId = en.AuthorGroupID,
                                                                    DistrictId = en.DistrictID,
                                                                    DistrictName = en.Districts,
                                                                    Level = en.Level,
                                                                    Name = en.Name,
                                                                    SchoolId = en.SchoolID,
                                                                    SchoolName = en.Schools,
                                                                    StateId = en.StateID,
                                                                    UserId = en.UserID,
                                                                    UserNameList = en.Users                                                                    
                                                                });

            if (stateId > 0)
                query = query.Where(en => en.StateId == stateId);
            if(districtId > 0)
                query = query.Where(en => en.DistrictId == districtId);
            if (schoolId > 0)
                query = query.Where(en => en.SchoolId == schoolId);

            return query;
        }

        public void Save(AuthorGroup item)
        {
            var entity = table.FirstOrDefault(x => x.AuthorGroupID.Equals(item.Id));

            if (entity.IsNull())
            {
                entity = new AuthorGroupEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.Id = entity.AuthorGroupID;
        }

        public void Delete(AuthorGroup item)
        {
            var entity = table.FirstOrDefault(x => x.AuthorGroupID.Equals(item.Id));
            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }


        public void DeleteAuthorGroup(int authorGroupId)
        {
            dbContext.DeleteAuthorGroup(authorGroupId);
        }

        public IQueryable<User> GetUsersUserHasAccessTo(int userId, int stateId, int districtId, int schoolId)
        {
            var query = tableUserByUserSchool.Where(en => en.UserSchoolUserID == userId);
            if (stateId > 0)
                query = query.Where(en => en.StateID == stateId);
            if (districtId > 0)
                query = query.Where(en => en.DistrictID == districtId);
            if (schoolId > 0)
                query = query.Where(en => en.UserSchoolSchoolID == schoolId);
            return query
                .Select(en => new User
                                  {
                                      Id = en.UserID,
                                      LastName = en.NameLast,
                                      FirstName = en.NameFirst,
                                      UserName = en.UserName,
                                      UserStatusId = en.UserStatusID
                                  }).Distinct();
        }

        public List<GetAuthorGroupBanksResult> GetAuthorGroupBanks(int bankID, int userId)
        {
            var result = dbContext.GetAuthorGroupBanks(bankID, userId).ToList();
            return result;
        }

        public IQueryable<GetAuthorGroupNotInBankResult> GetAuthorGroupNotInBank(int userId, int stateId, int districtId, int schoolId, int bankID)
        {
            return dbContext.GetAuthorGroupNotInBank(userId, stateId, districtId, schoolId, bankID).AsQueryable();
        }
    }
}