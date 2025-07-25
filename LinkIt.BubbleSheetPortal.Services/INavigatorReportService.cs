using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.ManageAccess;
using LinkIt.BubbleSheetPortal.Models.DTOs.Survey;
using LinkIt.BubbleSheetPortal.Models.Old.Configugration;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public interface INavigatorReportService
    {
        string Folder { get; set; }
        string Bucket { get; set; }

        BaseResponseModel<List<int>> ProcessUploadFiles(NavigatorReportUploadFileFormDataDTO formData, List<NavigatorReportFileDTO> files, int userId, DistributeSetting distributeSetting);
        BaseResponseModel<IEnumerable<NavigatorRecordExistResultDto>> GetRecordsExist(IEnumerable<NavigatorReportUploadFileFormDataDTO> forms);
        IQueryable<NavigatorReportUploadFileResponseDto> GetUploadedReportsInfo(int currUserId, int[] reportIds);
        BaseResponseModel<bool> Publish(NavigatorReportPublishRequestDto navigatorReportPublishRequest, int userId, int roleId, int districtId, EmailCredentialSetting emailCredentialSetting);
        BaseResponseModel<bool> UnPublish(NavigatorReportUnpublishRequestDto navigatorReportUnPublishRequest, int userId, int roleId, int districtId);
        BaseResponseModel<NavigatorReportFileInfoResponseDto> GetFile(string selectedIdentifier, int currUserId, int districtId, int roleId, int classId = 0);
        NavigatorReportFullDto GetNavigatorReportById(int navigatorReportId);
        List<StudentGrade> GetGradesStudent(string userIds);
        List<StudentProgram> GetProgramsStudent(string userIds);
        BaseResponseModel<List<NavigatorUserDto>> GetAssociateUser(string navigatorReportIds, int currUserId, bool isPublished, bool isLoadStudent, bool isLoadTeacher, bool isLoadSchool, bool isLoadDistrictAdmin, string programIds, string gradeIds, int districtId, int roleId, bool selectUserOnly = false);
        IEnumerable<ListItem> GetNavigatorCategory();
        IEnumerable<ListItem> GetKeywords();
        IEnumerable<ListItem> GetReportingPeriod();
        IEnumerable<ListItem> GetReportTypes(int? navigatorCategoryID);
        NavigatorConfigurationDTO GetConfigurationById(int? navigatorConfigurationId);
        NavigatorConfigurationDTO GetMaxConfigurationByNavigatorReportIdsAndCurrentRole(IEnumerable<int> navigatorReportIds, int roleId);
        IEnumerable<PublishByRoleRoleDefinitionDTO> GetRolesToPublishByNodePaths(string nodePath, int userId, int roleId, int? districtId);
        IEnumerable<ListItemStr> GetSchoolYears(int? districtId);
        IEnumerable<ViewableNavigatorReportAttributesDTO> GetNavigatorCheckboxesDataByStateIdAndDistrictId(int userId, int roleId, int? stateId, int? districtId);
        NavigatorReportDetailPanelDTO NavigatorGetReportDetail(string nodePath, int userId, int roleId, int districtId);
        IEnumerable<int> GetNavigatorReportIdsByNodePaths(IEnumerable<string> nodePaths, int userId, int roleId, int? districtId);
        BaseResponseModel<NavigatorReportPublishByRoleResultDTO> PublishByRoleAndNodePaths(NavigatorPublishByRoleDTO navigatorPublishByRole, EmailCredentialSetting emailCredentialSetting);
        NavigatorReportDownloadFilterDto GetFilterDownload(string nodePath, int userId, int roleId, int districtId);
        NavigatorConfigurationDTO GetNavigatorConfiguration(string nodePath, int currUserId, int roleId, int districtId);
        NavigatorManagePublishConfigurationDto GetManagePublishingConfiguration(string nodePath, int userId, int roleId, int districtId);
        bool DontHaveRightToPublish(int roleId);
        IEnumerable<NavigatorUserEmailDto> GetAssociateEmailsWhichNotPublished(string nodePath, string selectedRoleIds, int userId, int roleId, int? districtId);
        IEnumerable<ManageAccessPublishDetailDto> GetManageAccessPublishDetail(string nodePath, string checkedUserIds, int id, int roleId, int districtId);
        IEnumerable<NavigatorReportFillTableResultDto> OnFillTable(IEnumerable<NavigatorReportFillTableDto> request);
        IEnumerable<NavigatorReportDto> GetNavigatorReports(string nodePath, int userId, int roleId, int? districtId);

    }
}
