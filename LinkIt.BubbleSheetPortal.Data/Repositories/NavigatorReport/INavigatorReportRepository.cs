using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.PublishByRole;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.NavigatorReport
{
    public interface INavigatorReportRepository
    {
        IQueryable<NavigatorFolderPathwayDto> GetNavigatorFolderPathways();
        NavigatorReportEntity CreateOrOverwrite(NavigatorReportEntity navigatorReportEntity, out string errorMessage);
        bool IsExistsByFileName(string fileNameByConvention);
        IEnumerable<NavigatorRecordExistResultDto> GetRecordsExist(IEnumerable<NavigatorReportUploadFileFormDataDTO> forms);
        IQueryable<NavigatorReportEntity> AllNotDeleted();
        void Update(NavigatorReportEntity masterFileEntity);
        BaseResponseModel<NavigatorReportDTO> GetReportById(int navigatorReportId);
        BaseResponseModel<List<NavigatorReportUploadFileResponseDto>> GetUploadedReportsInfo(int currUserId, int[] reportIds);
        BaseResponseModel<List<NavigatorReportGetFileFromDBDto>> GetFiles(string navigatorReportId, int currUserId);

        IQueryable<NavigatorReportFullDto> Select();
        List<StudentGrade> GetGradesStudent(string userIds);
        List<StudentProgram> GetProgramsStudent(string userIds);
        IEnumerable<GetViewableNavigatorReportAttributesResult> GetNavigatorCheckboxesDataByStateIdAndDistrictId(int userId, int roleId, int? stateId, int? districtId, string yearsAsString);
        BaseResponseModel<List<NavigatorUserDto>> GetAssociateUser(string navigatorReportIds, bool isPublished, bool isLoadStudent, bool isLoadTeacher, bool isLoadSchool, bool isLoadDistrictAdmin, string programIds, string gradeIds, int districtId, int userId, int roleId, bool selectUserIdOnly);
        IQueryable<NavigatorGetDirectoryListResult> NavigatorGetDirectoryList(NavigatorReportByLevelFilterDTO filter);
        ISingleResult<NavigatorGetFileDetailResult> GetNavigatorReportFileDetail(int navigatorReportId, int userId, int roleId);
        IQueryable<NavigatorGetSchoolFolderDetailResult> NavigatorGetSchoolFolderDetail(int userId, int districtId, int roleId, string navigatorReportId);
    
        NavigatorReportDownloadFilterDto GetFilterDownloadFile(int navigatorReportId, int userId);
        BaseResponseModel<List<NavigatorReportGetFileFromDBDto>> GetFilesByClass(int navigatorReportId, int currUserId, int classId);
        NavigatorReportGetMaxConfigurationResult GetMaxConfigurationByNavigatorReportIds(string navigatorReportIdsAsString);
        ISingleResult<NavigatorGetReportFolderDetailResult> NavigatorGetReportFolderDetail(string navigatorReportId, string rolesToBePublish, int? userId, int? roleId, int? districtId);
        IEnumerable<NavigatorGetAssociateEmailsWhichNotPublishedResult> NavigatorGetAssociateEmailsWhichNotPublished(string navigatorReportId, int userId, int roleId, int? districtId, bool showDistrictAdmin, bool showSchoolAdmin, bool showTeacher, bool showStudent);
        IMultipleResults GetManageAccessPublishDetail(string navigatorReportId, string checkedUserIds, int userId, int roleId, int districtId);
    }
}
