using AutoMapper;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.NavigatorReport
{
    public class NavigatorReportRepository : INavigatorReportRepository
    {
        private readonly Table<NavigatorReportEntity> table;
        private string _connectionString;
        private readonly NavigatorReportDataContext _navigatorReportDataContext;
        private readonly Table<NavigatorFolderPathwayEntity> tableNavigatorFolderPathway;

        public NavigatorReportRepository(IConnectionString conn)
        {
            this._connectionString = conn.GetLinkItConnectionString();
            _navigatorReportDataContext = new NavigatorReportDataContext(_connectionString);
            table = _navigatorReportDataContext.GetTable<NavigatorReportEntity>();
            tableNavigatorFolderPathway = _navigatorReportDataContext.GetTable<NavigatorFolderPathwayEntity>();
        }

        public IQueryable<NavigatorFolderPathwayDto> GetNavigatorFolderPathways()
        {
            return tableNavigatorFolderPathway.Select(s => new NavigatorFolderPathwayDto()
            {
                NavigatorFolderPathwayID = s.NavigatorFolderPathwayID,
                NavigatorConfigurationID = s.NavigatorConfigurationID,
                FolderPathway = s.FolderPathway,
            });
        }
        public IQueryable<NavigatorReportFullDto> Select()
        {
            return table.Select(x => new NavigatorReportFullDto
            {
                NavigatorReportID = x.NavigatorReportID,
                S3FileFullName = x.S3FileFullName,
                DistrictID = x.DistrictID,
                SchoolID = x.SchoolID,
                Year = x.Year,
                CreatedTime = x.CreatedTime,
                PublishedDate = x.PublishedDate,
                CreatedBy = x.CreatedBy,
                Status = x.Status,
                KeywordIDs = x.KeywordIDs,
                NavigatorConfigurationID = x.NavigatorConfigurationID,
                ReportingPeriodID = x.ReportingPeriodID,
                SchoolId = x.SchoolID,
                UpdatedTime = x.UpdatedTime
            });
        }

        public NavigatorReportEntity CreateOrOverwrite(NavigatorReportEntity navigatorReportEntity, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                foreach (var entity in AllNotDeleted().Where(ExpressionCheckReportExisting(navigatorReportEntity.DistrictID, navigatorReportEntity.SchoolID, navigatorReportEntity.Year, navigatorReportEntity.NavigatorConfigurationID, navigatorReportEntity.ReportingPeriodID, navigatorReportEntity.KeywordIDs, navigatorReportEntity.ReportSuffix)).ToList())
                {
                    entity.Status = Constanst.Deleted;
                }

                table.InsertOnSubmit(navigatorReportEntity);
                _navigatorReportDataContext.SubmitChanges(ConflictMode.ContinueOnConflict);
                return navigatorReportEntity;
            }
            catch (ChangeConflictException ex)
            {
                errorMessage += $"Begin ChangeConflictException_ Message: {ex.Message}\n";
                errorMessage += $"Begin ChangeConflictException_ StackTrace: {ex.StackTrace}\n";
                foreach (ObjectChangeConflict objConflict in _navigatorReportDataContext.ChangeConflicts)
                {
                    foreach (MemberChangeConflict memberConflict in objConflict.MemberConflicts)
                    {
                        errorMessage += $"memberConflict: {memberConflict}\n";
                        memberConflict.Resolve(RefreshMode.OverwriteCurrentValues);
                    }
                }
                _navigatorReportDataContext.SubmitChanges(ConflictMode.ContinueOnConflict);
                errorMessage += "End ChangeConflictException \n";
                return navigatorReportEntity;
            }
        }
        public bool IsExistsByFileName(string fileNameByConvention)
        {
            bool _isExists = AllNotDeleted().Any(c => c.S3FileFullName == fileNameByConvention);
            return _isExists;
        }
        public IQueryable<NavigatorReportEntity> AllNotDeleted()
        {
            return table.Where(c => (c.Status ?? "") != Constanst.Deleted);
        }
        public void Update(NavigatorReportEntity masterFileEntity)
        {
            if (table.FirstOrDefault(c => c.NavigatorReportID == masterFileEntity.NavigatorReportID) is NavigatorReportEntity curr)
            {
                curr.NavigatorConfigurationID = masterFileEntity.NavigatorConfigurationID;
                curr.Status = masterFileEntity.Status;
                _navigatorReportDataContext.SubmitChanges();
            }
        }
        public BaseResponseModel<NavigatorReportDTO> GetReportById(int navigatorReportId)
        {
            var entity = AllNotDeleted().FirstOrDefault(c => c.NavigatorReportID == navigatorReportId);

            if (entity == null)
            {
                return BaseResponseModel<NavigatorReportDTO>.InstanceError(TextConstants.REPORT_STATUS_NOTFOUND);
            }
            var res = Mapper.Map<NavigatorReportDTO>(entity);
            return BaseResponseModel<NavigatorReportDTO>.InstanceSuccess(res);
        }
        public BaseResponseModel<List<NavigatorReportUploadFileResponseDto>> GetUploadedReportsInfo(int currUserId, int[] reportIds)
        {
            if (reportIds == null)
                return null;
            var _list = _navigatorReportDataContext.NavigatorReportGetUploadedReportsInfo(currUserId, string.Join(";", reportIds)).ToList();
            var res = _list.Select(c => AutoMapper.Mapper.Map<NavigatorReportUploadFileResponseDto>(c)).ToList();
            return BaseResponseModel<List<NavigatorReportUploadFileResponseDto>>.InstanceSuccess(res);
        }
        public BaseResponseModel<List<NavigatorUserDto>> GetAssociateUser(string navigatorReportIds, bool isPublished, bool isLoadStudent, bool isLoadTeacher, bool isLoadSchool, bool isLoadDistrictAdmin, string programIds, string gradeIds, int districtId, int userId, int roleId, bool selectUserIdOnly)
        {
            var results = new List<NavigatorUserDto>();
            var fingertipUsers = _navigatorReportDataContext.NavigatorReportGetAssociateUserByReportIds(navigatorReportIds, isPublished, isLoadDistrictAdmin, isLoadSchool, isLoadTeacher, isLoadStudent, programIds, gradeIds, selectUserIdOnly, string.Empty, districtId, userId, roleId)
                .Select(c => Mapper.Map<NavigatorUserDto>(c)).ToList();
            return BaseResponseModel<List<NavigatorUserDto>>.InstanceSuccess(fingertipUsers);
        }

        public BaseResponseModel<List<NavigatorReportGetFileFromDBDto>> GetFiles(string navigatorReportId, int currUserId)
        {
            var navigatorReportFile = new List<NavigatorReportGetFileFromDBDto>();

            var navigatorReport = _navigatorReportDataContext.NavigatorReportGetReportFiles(currUserId, navigatorReportId).ToList();
            navigatorReportFile = navigatorReport.Select(c => Mapper.Map<NavigatorReportGetFileFromDBDto>(c)).ToList();
            return BaseResponseModel<List<NavigatorReportGetFileFromDBDto>>.InstanceSuccess(navigatorReportFile);
        }

        public BaseResponseModel<List<NavigatorReportGetFileFromDBDto>> GetFilesByClass(int navigatorReportId, int currUserId, int classId)
        {
            var navigatorReportFiles = new List<NavigatorReportGetFileFromDBDto>();

            var navigatorReport = _navigatorReportDataContext.NavigatorReportGetReportFilesByClassId(navigatorReportId, classId, currUserId).ToList();
            navigatorReportFiles = navigatorReport.Select(c => Mapper.Map<NavigatorReportGetFileFromDBDto>(c)).ToList();

            return BaseResponseModel<List<NavigatorReportGetFileFromDBDto>>.InstanceSuccess(navigatorReportFiles);
        }

        public List<StudentGrade> GetGradesStudent(string userIds)
        {
            var studentGrade = _navigatorReportDataContext.NavigatorReportGetStudentGrade(userIds).ToList();
            var result = studentGrade.Select(x => new StudentGrade() { GradeID = x.GradeID.GetValueOrDefault(), GradeName = x.Name, Order = x.Order.GetValueOrDefault() }).ToList();
            return result;

        }
        public List<StudentProgram> GetProgramsStudent(string userIds)
        {
            var programs = _navigatorReportDataContext.NavigatorReportGetStudentPrograms(userIds).ToList();
            var result = programs.Select(x => new StudentProgram() { ProgramID = x.ProgramID.GetValueOrDefault(), ProgramName = x.Name }).ToList();
            return result;
        }

        public IEnumerable<GetViewableNavigatorReportAttributesResult> GetNavigatorCheckboxesDataByStateIdAndDistrictId(int userId, int roleId, int? stateId, int? districtId, string yearsAsString)
        {
            return _navigatorReportDataContext
                .GetViewableNavigatorReportAttributes(userId, roleId, stateId, districtId, yearsAsString);
        }
        public IQueryable<NavigatorGetDirectoryListResult> NavigatorGetDirectoryList(NavigatorReportByLevelFilterDTO filter)
        {
            return _navigatorReportDataContext.NavigatorGetDirectoryList(filter.UserId
                , filter.DistrictId
                , filter.RoleId
                , filter.AcceptedYears).AsQueryable();
        }

        public ISingleResult<NavigatorGetFileDetailResult> GetNavigatorReportFileDetail(int navigatorReportId, int userId, int roleId)
        {
            return _navigatorReportDataContext.NavigatorGetFileDetail(navigatorReportId, roleId, userId);
        }

        public IQueryable<NavigatorGetSchoolFolderDetailResult> NavigatorGetSchoolFolderDetail(int userId, int districtId, int roleId, string navigatorReportId)
        {
            return _navigatorReportDataContext.NavigatorGetSchoolFolderDetail(userId, districtId, roleId, navigatorReportId).AsQueryable();
        }


        public NavigatorReportDownloadFilterDto GetFilterDownloadFile(int navigatorReportId, int userId)
        {
            var filters = _navigatorReportDataContext.NavigatorReportGetFilterDownload(navigatorReportId, userId, string.Empty).ToList();
            var filterDownload = new NavigatorReportDownloadFilterDto();
            foreach (var filter in filters)
            {
                var districtTerm = new DistrictTermDto
                {
                    Id = filter.DistrictTermID.GetValueOrDefault(),
                    Name = filter.DistrictTermName
                };
                var classFilter = new ClassDto
                {
                    Id = filter.ClassID.GetValueOrDefault(),
                    DistrictTermId = filter.DistrictTermID.GetValueOrDefault(),
                    Name = filter.ClassName
                };
                if (!filterDownload.DistrictTerms.Any(x => x.Id == districtTerm.Id))
                    filterDownload.DistrictTerms.Add(districtTerm);
                filterDownload.Classes.Add(classFilter);
            }
            return filterDownload;
        }

        public IEnumerable<NavigatorRecordExistResultDto> GetRecordsExist(IEnumerable<NavigatorReportUploadFileFormDataDTO> forms)
        {
            try
            {
                var recordsExist = forms.Where(f => table.Any(x => x.DistrictID == f.District
                                                                && x.SchoolID == f.School
                                                                && x.Year == f.SchoolYear
                                                                && x.NavigatorConfigurationID == f.ReportType
                                                                && (x.ReportSuffix ?? "") == (f.ReportSuffix ?? "")
                                                                && GetReportingPeriod(f.ReportingPeriod).Contains(x.ReportingPeriodID)
                                                                && (x.KeywordIDs == GetPrimaryKeyword(f.KeywordIds) || x.KeywordIDs.StartsWith(GetPrimaryKeyword(f.KeywordIds) + ",")))
                                );

                if (recordsExist != null && recordsExist.Any())
                    return Mapper.Map<IEnumerable<NavigatorRecordExistResultDto>>(recordsExist);
            }
            finally { }
            return null;
        }
        private static Expression<Func<NavigatorReportEntity, bool>> ExpressionCheckReportExisting(int districtId, int schoolId, string schoolYear, int reportTypeId, int reportingPeriodId, string keywordIds, string reportSuffix)
        {
            var primaryKeywordIdString = keywordIds.Split(new char[] { ',' }).FirstOrDefault();
            var reportingPeriodIds = reportingPeriodId == -1 || reportingPeriodId == 0 ? new int[] { 0, -1 } : new int[] { reportingPeriodId };

            return c => c.DistrictID == districtId
                        && c.SchoolID == schoolId
                        && c.Year == schoolYear
                        && c.NavigatorConfigurationID == reportTypeId
                        && reportingPeriodIds.Contains(c.ReportingPeriodID)
                        && (c.ReportSuffix ?? "") == (reportSuffix ?? "")
                        && (c.KeywordIDs == primaryKeywordIdString || c.KeywordIDs.StartsWith(primaryKeywordIdString + ","));
        }
        private static int[] GetReportingPeriod(int reportingPeriodId)
        {
            return reportingPeriodId == -1 || reportingPeriodId == 0 ? new int[] { 0, -1 } : new int[] { reportingPeriodId };
        }
        private static string GetPrimaryKeyword(string keywordIds)
        {
            return keywordIds.Split(new char[] { ',' }).FirstOrDefault();
        }
        public NavigatorReportGetMaxConfigurationResult GetMaxConfigurationByNavigatorReportIds(string navigatorReportIdsAsString)
        {
            return _navigatorReportDataContext.NavigatorReportGetMaxConfiguration(navigatorReportIdsAsString).FirstOrDefault();
        }

        public ISingleResult<NavigatorGetReportFolderDetailResult> NavigatorGetReportFolderDetail(string navigatorReportId, string rolesToBePublish, int? userId, int? roleId, int? districtId)
        {
            return _navigatorReportDataContext.NavigatorGetReportFolderDetail(navigatorReportId, rolesToBePublish, userId, roleId, districtId);
        }

        public IEnumerable<NavigatorGetAssociateEmailsWhichNotPublishedResult> NavigatorGetAssociateEmailsWhichNotPublished(string navigatorReportId, int userId, int roleId, int? districtId, bool showDistrictAdmin, bool showSchoolAdmin, bool showTeacher, bool showStudent)
        {
            return _navigatorReportDataContext.NavigatorGetAssociateEmailsWhichNotPublished(navigatorReportId, userId, roleId, districtId, showDistrictAdmin, showSchoolAdmin, showTeacher, showStudent);


        }

        public IMultipleResults GetManageAccessPublishDetail(string navigatorReportId, string checkedUserIds, int userId, int roleId, int districtId)
        {
            return _navigatorReportDataContext.NavigatorGetManageAccessPopupDetail_Multiple(navigatorReportId, userId, roleId, districtId, checkedUserIds);
        }
    }
}
