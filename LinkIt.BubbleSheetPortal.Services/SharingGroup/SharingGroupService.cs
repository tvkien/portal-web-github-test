using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Data.Repositories.SharingGroup.IService;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Old.DataLockerForStudent;
using LinkIt.BubbleSheetPortal.Models.Old.SharingGroup;
using LinkIt.BubbleSheetPortal.Services.EDM;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Services.SharingGroup
{
    public class SharingGroupService
    {
        private readonly ISharingGroupRepository _sharingGroupRepository;
        public SharingGroupService(ISharingGroupRepository sharingGroupRepository)
        {
            this._sharingGroupRepository = sharingGroupRepository;
        }
        public AddOrEditSharingGroupResponseDto AddUserToSharingGroup(int sharingGroupId, List<int> userIds)
        {
            return _sharingGroupRepository.AddUserToSharingGroup(sharingGroupId, userIds);
        }
        public UserAvailableAddSharingGroupResponseDto GetUserUserAvailableAddSharingGroup(GetUserAvailableAddSharingGroupPaginationRequest request)
        {
            return _sharingGroupRepository.GetUserUserAvailableAddSharingGroup(request);
        }
        public AddOrEditSharingGroupResponseDto AddSharingGroup(int userId, SharingGroupDto request)
        {
            return _sharingGroupRepository.AddSharingGroup(userId, request);
        }

        public AddOrEditSharingGroupResponseDto CloneSharingGroup(SharingGroupDto request, int userId)
        {
            return _sharingGroupRepository.CloneSharingGroup(request, userId);
        }

        public AddOrEditSharingGroupResponseDto EditSharingGroup(int userId, SharingGroupDto request)
        {
            return _sharingGroupRepository.EditSharingGroup(userId, request);
        }
        public SharingGroupDto GetDetailSharingGroup(int sharingGroupId)
        {
            return _sharingGroupRepository.GetDetailSharingGroup(sharingGroupId);
        }
        public bool DeleteSharingGroup(int sharingGroupId)
        {
            return _sharingGroupRepository.DeleteSharingGroup(sharingGroupId);
        }
        public bool ChangeStatusSharingGroup(int sharingGroupId)
        {
            return _sharingGroupRepository.ChangeStatusSharingGroup(sharingGroupId);
        }
        public bool PublishOrUnpublishSharingGroup(int userId, int sharingGroupId, bool isPublished)
        {
            return _sharingGroupRepository.PublishOrUnpublishSharingGroup(userId, sharingGroupId, isPublished);
        }
        public SharingGroupResponseDto GetSharingGroups(GetSharingGroupPaginationRequest request)
        {
            return _sharingGroupRepository.GetSharingGroups(request);
        }
        public UserInSharingGroupResponseDto GetUserInSharingGroup(GetUserInSharingGroupPaginationRequest request)
        {
            return _sharingGroupRepository.GetUserInSharingGroup(request);
        }
        public bool RemoveUserFromSharingGroup(int sharingGroupId, int userId)
        {
            return _sharingGroupRepository.RemoveUserFromSharingGroup(sharingGroupId, userId);
        }
    }
}
