using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.ManageParent
{
    public interface IManageParentRepository
    {
        IEnumerable<GetParentListResult> GetParentList(FilterParentRequestModel filterParentRequestModel);
        IEnumerable<ManageParentGetChildrenListResult> GetChildrenList(GetChildrenListRequestModel criteria, int userId, int roleId, int? districtId);
        GetParentByRegistrationCodeResult GetParentByRegistrationCode(int districtId, string registrationCode);
        IEnumerable<int> GetChildrenStudentIdList(int parentId);
        void DisableRegistrationCode(int userId);
        BaseResponseModel<bool> UnassignStudent(int parentUserId, int studentId);
        void AddStudentsToParent(int[] studentIdsSplitted, int parentId, string relationship, bool studentDataAccess);
        bool CheckRegistrationCodeActive(int userId);

        bool ExistsRegistrationCode(string registrationCode);
        string GetStudentFirstNameByParentId(int parentId);
        List<GetStudentByNavigatorReportAndParentResult> GetStudentByNavigatorReportAndParent(int navigatorReportId, int parentId);
        void TrackingLastSendDistributeEmail(int userId);
        GetParentsInformationForDistributeRegistrationCodeResult GetParentsInformationForDistributeRegistrationCode(int parentUserId);
        IEnumerable<int> GetAccessibleUserIds(int userId, int roleId, int? districtId);
        void UpdateStudentParents(List<StudentParentCustom> studentParentCustoms);
    }
}
