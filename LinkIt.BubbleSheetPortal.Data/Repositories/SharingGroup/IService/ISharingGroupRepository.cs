using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Old.SharingGroup;
using LinkIt.BubbleSheetPortal.Models.StudentPreferenceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.SharingGroup.IService
{
    public interface ISharingGroupRepository
    {
        AddOrEditSharingGroupResponseDto AddUserToSharingGroup(int sharingGroupId, List<int> userIds);
        UserAvailableAddSharingGroupResponseDto GetUserUserAvailableAddSharingGroup(GetUserAvailableAddSharingGroupPaginationRequest criteria);
        bool RemoveUserFromSharingGroup(int sharingGroupId, int userId);
        UserInSharingGroupResponseDto GetUserInSharingGroup(GetUserInSharingGroupPaginationRequest criteria);
        AddOrEditSharingGroupResponseDto AddSharingGroup(int userId, SharingGroupDto request);
        AddOrEditSharingGroupResponseDto EditSharingGroup(int userId, SharingGroupDto request);
        bool ChangeStatusSharingGroup(int sharingGroupId);
        bool DeleteSharingGroup(int sharingGroupId);
        bool PublishOrUnpublishSharingGroup(int userId, int sharingGroupId, bool isPublished);
        SharingGroupResponseDto GetSharingGroups(GetSharingGroupPaginationRequest criteria);
        SharingGroupDto GetDetailSharingGroup(int sharingGroupId);
        AddOrEditSharingGroupResponseDto CloneSharingGroup(SharingGroupDto request, int userId);

    }
}
