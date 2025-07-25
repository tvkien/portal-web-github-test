using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.ManageTest;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Repositories.Helper;
using System.Data;
using LinkIt.BubbleSheetPortal.Models.DTOs.Survey;
using LinkIt.BubbleSheetPortal.Models.Old.ManageTest;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ManageTestRepository : IManageTestRepository
    {
        private readonly TestDataContext _dbContext;
        private readonly DataLockerContextDataContext _dataLockerContext;
        private readonly UserDataContext _userContext;
        private readonly SurveyContextDataContext _surveyDbContext;
        private readonly IConnectionString _connectionString;

        public ManageTestRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _dbContext = TestDataContext.Get(connectionString);
            _dataLockerContext = new DataLockerContextDataContext(connectionString);
            _userContext = UserDataContext.Get(connectionString);
            _surveyDbContext = new SurveyContextDataContext(connectionString);
            _connectionString = conn;
        }

        public IList<BankData> GetBanksByUserID(GetBanksByUserIDFilter filter)
        {
            var result = _dbContext.CreateTestGetBanksByUserID_HideTeacherBank(
                filter.UserID,
                filter.RoleID,
                filter.DistrictID,
                filter.BankID,
                filter.ShowArchived,
                filter.HideTeacherBanks,
                filter.HideOtherPeopleBanks,
                filter.HideBankOnlyForm,
                filter.IsSurvey,
                filter.PageIndex,
                filter.PageSize,
                filter.GeneralSearch,
                filter.SortColumn,
                filter.SortDirection)
                .Select(x => new BankData
                {
                    BankID = x.BankID.GetValueOrDefault(),
                    BankName = x.BankName,
                    CreatedByUserID = x.CreatedByUserID ?? 0,
                    GradeName = x.Grade,
                    SubjectName = x.Subject,
                    GradeOrder = x.GradeOrder.GetValueOrDefault(),
                    Archived = x.Archived ?? false,
                    TotalRecords = x.TotalRecords ?? 0
                }).ToList();

            return result;
        }

        public IList<BankData> GetFormBanksByUserID(int userID, int roleID, int schoolID, int districtID, bool showArchived, bool? hideBankOnlyTest, bool filterByDistrict = true)
        {
            var result = _dataLockerContext.GetFormBanksByUserID(userID, roleID, schoolID, districtID, showArchived, hideBankOnlyTest)
                .Select(x => new BankData
                {
                    BankID = x.BankID,
                    BankName = x.BankName,
                    CreatedByUserID = x.CreatedByUserID ?? 0,
                    GradeName = x.Grade,
                    SubjectName = x.Subject,
                    GradeOrder = x.GradeOrder,
                    Archived = x.Archived ?? false
                }).AsEnumerable();

            var userBankAccess = this.GetUserBankAccess(new UserBankAccessCriteriaDTO
            {
                UserId = userID,
                SchoolId = schoolID,
                DistrictId = districtID,
                ShowArchived = showArchived,
                FilterByDistrict = filterByDistrict
            });
            var includeBanks = _dbContext.BankEntities.Where(m => userBankAccess.BankIncludeIds.Contains(m.BankID))
                .Select(x => new BankData
                {
                    BankID = x.BankID,
                    BankName = x.Name,
                    CreatedByUserID = x.CreatedByUserID ?? 0,
                    GradeName = x.SubjectEntity != null && x.SubjectEntity.GradeEntity != null ? x.SubjectEntity.GradeEntity.Name : string.Empty,
                    SubjectName = x.SubjectEntity != null ? x.SubjectEntity.Name : string.Empty,
                    GradeOrder = x.SubjectEntity != null && x.SubjectEntity.GradeEntity != null ? x.SubjectEntity.GradeEntity.Order : 0,
                    Archived = x.Archived ?? false
                }).AsEnumerable();

            result = result.Union(includeBanks).DistinctBy(m => m.BankID);
            result.Where(m => !userBankAccess.BankExcludeIds.Contains(m.BankID));
            return result.ToList();
        }
        public bool HasRightToEditTestBankForNetWorkAdmin(int bankId, int userId)
        {
            var result = _dbContext.HasRightToEditTestBankForNetWorkAdmin(bankId, userId).FirstOrDefault();
            if (result != null)
                return result.HasRight.Value;
            return false;
        }

        /// <summary>
        /// Get List User Bank Access
        /// Filter IncludeBanks by criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public UserBankAccessDTO GetUserBankAccess(UserBankAccessCriteriaDTO criteria)
        {
            var bankAccessQuery = _dbContext.UserBankAccessEntities.Where(m => m.UserID == criteria.UserId).AsEnumerable();

            if (bankAccessQuery.Count() == 0)
            {
                return new UserBankAccessDTO();
            }

            var userBankAccess = new UserBankAccessDTO
            {
                UserID = criteria.UserId,
                BankExcludeIds = bankAccessQuery.Where(m => m.IsIncluded == false).Select(m => m.BankID).ToList(),
            };

            bankAccessQuery = bankAccessQuery.Where(m => m.IsIncluded);

            if (criteria.ShowArchived == true)
            {
                bankAccessQuery = bankAccessQuery.Where(m => m.BankEntity.Archived == true);
            }

            if (criteria.SubjectId > 0)
            {
                bankAccessQuery = bankAccessQuery.Where(m => m.BankEntity != null && m.BankEntity.SubjectID == criteria.SubjectId);
            }

            if (criteria.SubjectIds != null && criteria.SubjectIds.Count > 0)
            {
                bankAccessQuery.Where(m => m.BankEntity != null && criteria.SubjectIds.Contains(m.BankEntity.SubjectID));
            }

            if (criteria.SubjectNames != null && criteria.SubjectNames.Any())
            {
                bankAccessQuery = bankAccessQuery.Where(m => m.BankEntity != null && m.BankEntity.SubjectEntity != null
                                && criteria.SubjectNames.Contains(m.BankEntity.SubjectEntity.Name.ToLower()));
            }

            if (criteria.BankAccessId > 0)
            {
                bankAccessQuery = bankAccessQuery.Where(m => m.BankEntity != null && m.BankEntity.BankAccessID == criteria.BankAccessId);
            }

            if (criteria.GradeIds.Any())
            {
                bankAccessQuery = bankAccessQuery.Where(m => m.BankEntity != null && m.BankEntity.SubjectEntity != null
                                && criteria.GradeIds.Contains(m.BankEntity.SubjectEntity.GradeID));
            }

            var bankAccess = bankAccessQuery.ToList();

            if (criteria.HideCreatedByOthers == true)
            {
                bankAccess = bankAccess.Where(m => m.BankEntity != null && m.BankEntity.CreatedByUserID == criteria.UserId).ToList();
            }

            if (criteria.HideCreatedByTeacher == true)
            {
                var createdIds = bankAccess.Select(m => m.BankEntity.CreatedByUserID.GetValueOrDefault()).ToList();
                var teacherIds = _userContext.UserEntities
                    .Where(m => createdIds.Contains(m.UserID) && m.RoleID == (int)RoleEnum.Teacher)
                    .Select(m => m.UserID).ToList();


                bankAccess = bankAccess.Where(m => m.BankEntity != null
                    && (!m.BankEntity.CreatedByUserID.HasValue || !teacherIds.Contains(m.BankEntity.CreatedByUserID.Value))).ToList();
            }

            userBankAccess.BankIncludeIds = bankAccess.Select(m => m.BankID).ToList();
            return userBankAccess;
        }

        public List<ListItem> GetGradeIncludes(int userId)
        {
            var grades = _dbContext.UserBankAccessEntities.Where(m => m.UserID == userId && m.IsIncluded && m.BankEntity != null
                                                                        && m.BankEntity.SubjectEntity != null
                                                                        && m.BankEntity.SubjectEntity.GradeEntity != null)
                                        .Select(m => new ListItem
                                        {
                                            Id = m.BankEntity.SubjectEntity.GradeID,
                                            Name = m.BankEntity.SubjectEntity.GradeEntity.Name
                                        }).ToList();

            return grades;
        }

        public List<Subject> GetSubjectIncludes(int userId)
        {
            var subjects = _dbContext.UserBankAccessEntities.Where(m => m.UserID == userId && m.IsIncluded && m.BankEntity != null
                                                                        && m.BankEntity.SubjectEntity != null)
                                        .Select(m => new Subject()
                                        {
                                            Id = m.BankEntity.SubjectEntity.SubjectID,
                                            Name = m.BankEntity.SubjectEntity.Name
                                        }).ToList();

            return subjects;
        }

        public IList<BankData> GetSurveyBanksByUserID(int userID, int roleID, int districtID, bool showArchived, bool filterByDistrict = true)
        {
            var result = _surveyDbContext.GetSurveyBanksByUserId(userID, roleID, districtID, showArchived)
                .Select(x => new BankData
                {
                    BankID = x.BankID.GetValueOrDefault(),
                    BankName = x.BankName,
                    CreatedByUserID = x.CreatedByUserID ?? 0,
                    GradeName = x.Grade,
                    SubjectName = x.Subject,
                    GradeOrder = x.GradeOrder.GetValueOrDefault(),
                    Archived = x.Archived ?? false
                }).AsEnumerable();

            var userBankAccess = this.GetUserBankAccess(new UserBankAccessCriteriaDTO
            {
                UserId = userID,
                DistrictId = districtID,
                ShowArchived = showArchived,
                FilterByDistrict = filterByDistrict
            });
            var includeBanks = _dbContext.BankEntities.Where(m => userBankAccess.BankIncludeIds.Contains(m.BankID))
                .Select(x => new BankData
                {
                    BankID = x.BankID,
                    BankName = x.Name,
                    CreatedByUserID = x.CreatedByUserID ?? 0,
                    GradeName = x.SubjectEntity != null && x.SubjectEntity.GradeEntity != null ? x.SubjectEntity.GradeEntity.Name : string.Empty,
                    SubjectName = x.SubjectEntity != null ? x.SubjectEntity.Name : string.Empty,
                    GradeOrder = x.SubjectEntity != null && x.SubjectEntity.GradeEntity != null ? x.SubjectEntity.GradeEntity.Order : 0,
                    Archived = x.Archived ?? false
                }).AsEnumerable();

            result = result.Union(includeBanks).DistinctBy(m => m.BankID);
            result = result.Where(m => !userBankAccess.BankExcludeIds.Contains(m.BankID));
            return result.ToList();
        }
        public void UpdateSubScoreLabelSurveyTemplate(SurveyItem item)
        {
            BulkHelper bulkHelper = new BulkHelper(_connectionString);
            string tempTableName = "#QuestionItemNeededUpdate";
            string tempTableCreateScript = $@"CREATE TABLE [{tempTableName}](VirtualQuestionId int, ItemNumber varchar(50))";
            string updateSurveyTemplateProcedureName = "UpdateSubScoreLabelSurveyTemplate";

            bulkHelper.BulkCopy(tempTableCreateScript, tempTableName, item.VirtualQuestionItemNumbers, updateSurveyTemplateProcedureName, "@VirtualTestId", item.VirtualTestId);
        }

        public IEnumerable<ReviewSurveyData> GetReviewSurveys(int userID, int roleId, int? districtId,int? termId, int? surveyAssignmentType, int? surveyBankId, int? surveyId, bool showActiveAssignment, string sort, string search, int? skip, int? take)
        {
            var results = _surveyDbContext.GetReviewSurveys(userID, roleId, districtId, termId, surveyAssignmentType, surveyBankId, surveyId, showActiveAssignment, sort, search, skip, take)
               .Select(x => new ReviewSurveyData
               {
                   VirtualTestId = x.VirtualTestID,
                   SurveyName = x.Name,
                   TermName = x.TermName,
                   SchoolName = x.SchoolName,
                   AssignmentType = x.SurveyAssignmentType,
                   Assignments = x.NumberAssignment,
                   MostRecentResponse = x.MostRecentResponsed,
                   BankId = x.BankId,
                   TermID = x.DistrictTermID,
                   TotalRecords = x.TotalRecords
               }).AsEnumerable();

            return results;
        }

        public IList<ListItem> GetAssignSurveyBanksByUserID(int? districtId, int roleId, int userId)
        {
            return _surveyDbContext.GetAuthorizedSurveyBanksByUserId(userId, roleId, districtId)
               .Select(x => new ListItem
               {
                   Id = x.BankID,
                   Name = x.BankName
               }).ToList();
        }
    }
}
