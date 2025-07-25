using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.Base;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Services.ManageParent
{
    public interface IManageParentService
    {
        DatatablesViewModel<ParentGridViewModel> GetParentList(FilterParentRequestModel filterParentRequestModel);
        IEnumerable<ParentListForDistributingDto> GetParentListForDistributing(FilterParentRequestModel filterParentRequestModel);
        GenericDataTableResponseDTO<ChildrenListViewModel> GetChildrenList(GetChildrenListRequestModel criteria, int userId, int roleId, int? districtId);
        IEnumerable<int> GetChildrenStudentIdList(int parentId);
        AddOrEditParentViewModelDto GetParentUserInfo(int userId);
        void GenerateRegistrationCode(IEnumerable<int> userIds, int currentUserId, int maxCodeLength);
        bool IsParentUser(int parentUserId);
        void UpdateParentUserInfo(UpdateParentRequestModel parentModel);
        BaseResponseModel<bool> UnassignStudent(int parentUserId, int studentId);
        BaseResponseModel<bool> AddStudentsToParent(string studentIds, int parentUserId);
        BaseResponseModel<bool> AddStudentsToParent(string studentIds, int parentUserId, string relationship, bool studentDataAccess);
        bool ExistsRegistrationCode(string registrationCode);
        void TrackingLastSendDistributeEmail(int userId);
        GetParentsInformationForDistributeRegistrationCodeResult GetParentsInformationForDistributeRegistrationCode(int userId);
        bool CanEditParent(int userId, int roleId, int? districtId, int parentUserId);
        void UpdateStudentParents(int parentId, List<StudentParentCustom> studentParents);
    }
}
