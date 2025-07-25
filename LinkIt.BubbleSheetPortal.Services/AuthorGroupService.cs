using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class AuthorGroupService
    {
        private readonly IReadOnlyRepository<AuthorGroupList> repository;
        private readonly IRepository<AuthorGroupUser> authorGroupUserRepository;
        private readonly IRepository<AuthorGroup> authorGroupRepository;
        private readonly IRepository<AuthorGroupSchool> authorGroupSchoolRepository;
        private readonly IRepository<AuthorGroupDistrict> authorGroupDistrictRepository;
        private readonly IAuthorGroupRepository extendAuthorGroupRepository;
        private readonly IRepository<AuthorGroupBank> authorGroupBankRepository;
        private readonly IRepository<User> userRepository;
        private readonly IRepository<UserSchool> userSchoolRepository;
        private readonly DSPDistrictRepository dSPDistrictRepository;

        private const int _networkAdminRole = 27;

        public AuthorGroupService(IReadOnlyRepository<AuthorGroupList> repository,
            IRepository<AuthorGroupUser> authorGroupUserRepository, IRepository<AuthorGroup> authorGroupRepository,
            IRepository<AuthorGroupSchool> authorGroupSchoolRepository,
            IRepository<AuthorGroupDistrict> authorGroupDistrictRepository,
            IAuthorGroupRepository extendAuthorGroupRepository,
            IRepository<AuthorGroupBank> authorGroupBankRepository,
            IRepository<User> userRepository,
            IRepository<UserSchool> userSchoolRepository,
            DSPDistrictRepository dSPDistrictRepository)
        {
            this.repository = repository;
            this.authorGroupUserRepository = authorGroupUserRepository;
            this.authorGroupRepository = authorGroupRepository;
            this.authorGroupSchoolRepository = authorGroupSchoolRepository;
            this.authorGroupDistrictRepository = authorGroupDistrictRepository;
            this.extendAuthorGroupRepository = extendAuthorGroupRepository;
            this.authorGroupBankRepository = authorGroupBankRepository;
            this.userRepository = userRepository;
            this.userSchoolRepository = userSchoolRepository;
            this.dSPDistrictRepository = dSPDistrictRepository;
        }

        public IQueryable<AuthorGroupList> GetAuthorGroupListHasAccessTo(int userId, int stateId, int districtId, int schoolId)
        {
            var query = extendAuthorGroupRepository
                .GetAuthorGroupListHasAccessTo(userId, stateId, districtId, schoolId);
            return query.Select(en => new AuthorGroupList
            {
                AuthorGroupId = en.AuthorGroupID,
                Name = en.Name,
                Level = en.Level,
                SchoolId = en.SchoolID,
                DistrictId = en.DistrictID,
                StateId = en.StateID,
                UserId = en.UserID,
                DistrictName = en.Districts,
                SchoolName = en.Schools,
                UserNameList = en.Users
            }).ToList().AsQueryable();
        }

        public IQueryable<AuthorGroupList> GetAuthorGroupList(int stateId, int districtId, int schoolId)
        {
            return extendAuthorGroupRepository
                .GetAuthorGroupList(stateId, districtId, schoolId);
        }

        public void DeleteAuthorGroupUser(int authorGroupId, int userId)
        {
            var item =
                authorGroupUserRepository.Select()
                    .FirstOrDefault(a => a.AuthorGroupId.Equals(authorGroupId) && a.UserId.Equals(userId));
            if (item != null)
            {
                authorGroupUserRepository.Delete(item);
            }
        }

        public void DeleteAuthorGroupDistrict(int authorGroupId, int districtId)
        {
            var item =
                authorGroupDistrictRepository.Select()
                    .FirstOrDefault(a => a.AuthorGroupId.Equals(authorGroupId) && a.DistrictId.Equals(districtId));
            if (item != null)
            {
                authorGroupDistrictRepository.Delete(item);
            }
        }

        public void DeleteAuthorGroupSchool(int authorGroupId, int schoolId)
        {
            var item =
                authorGroupSchoolRepository.Select()
                    .FirstOrDefault(a => a.AuthorGroupId.Equals(authorGroupId) && a.SchoolId.Equals(schoolId));
            if (item != null)
            {
                authorGroupSchoolRepository.Delete(item);
            }
        }

        public void AddAuthorGroupUser(int authorGroupId, int userId)
        {
            var isExisted =
                authorGroupUserRepository.Select()
                    .Any(a => a.AuthorGroupId.Equals(authorGroupId) && a.UserId.Equals(userId));
            if (!isExisted)
            {
                authorGroupUserRepository.Save(new AuthorGroupUser
                {
                    AuthorGroupId = authorGroupId,
                    UserId = userId
                });
            }
        }

        public void AddAuthorGroupDistrict(int authorGroupId, int districtId)
        {
            var isExisted =
                authorGroupDistrictRepository.Select()
                    .Any(a => a.AuthorGroupId.Equals(authorGroupId) && a.DistrictId.Equals(districtId));
            if (!isExisted)
            {
                authorGroupDistrictRepository.Save(new AuthorGroupDistrict
                {
                    AuthorGroupId = authorGroupId,
                    DistrictId = districtId
                });
            }
        }

        public void AddAuthorGroupSchool(int authorGroupId, int schoolId)
        {
            var isExisted =
                authorGroupSchoolRepository.Select()
                    .Any(a => a.AuthorGroupId.Equals(authorGroupId) && a.SchoolId.Equals(schoolId));
            if (!isExisted)
            {
                authorGroupSchoolRepository.Save(new AuthorGroupSchool
                {
                    AuthorGroupId = authorGroupId,
                    SchoolId = schoolId
                });
            }
        }

        public void AddAuthorGroup(AuthorGroup authorGroup)
        {
            authorGroupRepository.Save(authorGroup);
        }

        public void Save(AuthorGroup authorGroup)
        {
            authorGroupRepository.Save(authorGroup);
        }

        public void DeleteAuthorGroup(int authorGroupId)
        {
            extendAuthorGroupRepository.DeleteAuthorGroup(authorGroupId);
        }

        public AuthorGroup GetAuthorGroupById(int authorGroupId)
        {
            return authorGroupRepository.Select().FirstOrDefault(x => x.Id == authorGroupId);
        }

        public bool IsUserInAuthorGroup(int userId, int authorGroupId)
        {
            return authorGroupUserRepository.Select().Any(x => x.UserId == userId && x.AuthorGroupId == authorGroupId);
        }

        public IQueryable<User> GetUsersUserHasAccessTo(int userId, int stateId, int districtId, int schoolId)
        {
            return extendAuthorGroupRepository.GetUsersUserHasAccessTo(userId, stateId, districtId, schoolId);
        }

        public IQueryable<AuthorGroupList> GetAuthorGroupBanks(int bankID, int userId)
        {
            var query = extendAuthorGroupRepository.GetAuthorGroupBanks(bankID, userId);
            return query.Select(en => new AuthorGroupList
            {
                AuthorGroupId = en.AuthorGroupID,
                Name = en.Name,
                Level = en.Level,
                SchoolId = en.SchoolID,
                DistrictId = en.DistrictID,
                StateId = en.StateID,
                UserId = en.UserID,
                DistrictName = en.Districts,
                SchoolName = en.Schools,
                UserNameList = en.Users
            }).ToList().AsQueryable();
        }

        public IQueryable<int> GetBanksOfUsers(int userId)
        {
            var authorGroupIds = authorGroupUserRepository.Select().Where(m => m.UserId == userId).Select(m => m.AuthorGroupId).ToList();

            // get schools of User
            var schoolIds = userSchoolRepository.Select().Where(m => m.UserId == userId).Select(m => m.SchoolId).ToList();

            if(schoolIds.Count > 0)
            {
                // get author group of schools
                var schoolAuthorGroupIds = authorGroupSchoolRepository.Select().Where(m => schoolIds.Contains(m.SchoolId)).Select(m => m.AuthorGroupId).ToList();

                authorGroupIds.AddRange(schoolAuthorGroupIds);
            }

            var user = userRepository.Select().FirstOrDefault(m => m.Id == userId);

            if (user != null)
            {
                var districtIdsOfUser = new List<int>();

                if (user.RoleId == _networkAdminRole) // Network Admin
                {
                    //get district of user
                    districtIdsOfUser = dSPDistrictRepository.GetDistricIdbyNetWorkAdmin(userId);
                }
                else
                {
                    districtIdsOfUser.Add(user.DistrictId.GetValueOrDefault());
                }

                // get author group of districts
                if (districtIdsOfUser != null)
                {
                    var districtAuthorGroupIds = authorGroupDistrictRepository.Select()
                        .Where(m => districtIdsOfUser.Contains(m.DistrictId)).Select(m => m.AuthorGroupId).ToList();
                    authorGroupIds.AddRange(districtAuthorGroupIds);
                }
            }
            authorGroupIds = authorGroupIds.Distinct().ToList();
            var bankIds =  authorGroupBankRepository.Select().Where(m => authorGroupIds.Contains(m.AuthorGroupID)).Select(m => m.BankID);
            return bankIds;
        }

        public IQueryable<AuthorGroupList> GetAuthorGroupNotInBank(int userId, int stateId, int districtId, int schoolId, int bankID)
        {
            var query = extendAuthorGroupRepository.GetAuthorGroupNotInBank(userId, stateId, districtId, schoolId, bankID);
            return query.Select(en => new AuthorGroupList
            {
                AuthorGroupId = en.AuthorGroupID,
                Name = en.Name,
                Level = en.Level,
                SchoolId = en.SchoolID,
                DistrictId = en.DistrictID,
                StateId = en.StateID,
                UserId = en.UserID,
                DistrictName = en.Districts,
                SchoolName = en.Schools,
                UserNameList = en.Users
            }).ToList().AsQueryable();
        }

        public void AddAuthorGroupBank(int authorGroupID, int bankID)
        {
            var isExisted =
                authorGroupBankRepository.Select()
                    .Any(a => a.AuthorGroupID == authorGroupID && a.BankID == bankID);
            if (!isExisted)
            {
                authorGroupBankRepository.Save(new AuthorGroupBank
                {
                    AuthorGroupID = authorGroupID,
                    BankID = bankID
                });
            }
        }

        public void RemoveAuthorGroupBank(int authorGroupID, int bankID)
        {
            var items =
                authorGroupBankRepository.Select()
                    .Where(a => a.AuthorGroupID == authorGroupID && a.BankID == bankID);
            foreach (var item in items)
            {
                authorGroupBankRepository.Delete(item);
            }
        }
    }
}