using Amazon.IdentityManagement.Model;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Extensions;
using LinkIt.BubbleSheetPortal.Data.Repositories.SharingGroup.IService;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Old.SharingGroup;
using LinkIt.BubbleSheetPortal.Models.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.SharingGroup.Service
{
    public class SharingGroupRepository : ISharingGroupRepository
    {
        private readonly Table<SharingGroupEntity> _table;
        private readonly Table<SharingGroupUserEntity> _tableSharingGroupUser;
        private readonly UserDataContext _dbContext;
        public SharingGroupRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _dbContext = UserDataContext.Get(connectionString);
            _table = _dbContext.GetTable<SharingGroupEntity>();
            _tableSharingGroupUser = _dbContext.GetTable<SharingGroupUserEntity>();
        }
        public AddOrEditSharingGroupResponseDto AddUserToSharingGroup(int sharingGroupId, List<int> userIds)
        {
            try
            {
                _dbContext.SP_SharingGroup_AssignUsersIntoSharingGroup(sharingGroupId, string.Join(",", userIds));
            }
            catch (System.Exception ex)
            {
                return new AddOrEditSharingGroupResponseDto() { Success = false, Message = ex.Message };
            }
            return new AddOrEditSharingGroupResponseDto();
        }

        public UserAvailableAddSharingGroupResponseDto GetUserUserAvailableAddSharingGroup(GetUserAvailableAddSharingGroupPaginationRequest request)
        {
            var result = new UserAvailableAddSharingGroupResponseDto();
            int? totalRecord = 0;
            var users = _dbContext.SP_SharingGroup_GetUserAvailableAddSharingGroup(request.RoleID, request.UserID,
                request.DistrictID, request.SharingGroupID, request.RoleIdList, request.SchoolIdList, request.ShowInactiveUser, request.GeneralSearch, request.SortColumn,
                request.SortDirection, request.StartRow,
                request.PageSize,
                ref totalRecord).ToList();
            if (users != null)
            {
                result.Data = users.Select(s => new UserInSharingGroupDto()
                {
                    SchoolList = s.SchoolList ?? string.Empty,
                    UserId = s.UserID,
                    UserStatusId = s.UserStatusID,
                    FirstName = s.NameFirst,
                    LastName = s.NameLast,
                    RoleName = s.RoleName,
                    UserName = s.UserName,
                    DistrictId = s.DistrictID,
                    RoleId = s.RoleID,
                    SharingGroupName = s.SharingGroupName,
                    UserStatusName = s.UserStatusName
                }).ToList();
                result.TotalRecord = totalRecord.HasValue ? totalRecord.Value : 0;
                return result;
            }
            return new UserAvailableAddSharingGroupResponseDto();
        }
        public bool RemoveUserFromSharingGroup(int sharingGroupId, int userId)
        {
            try
            {
                var entity = _tableSharingGroupUser.Where(w => w.SharingGroupID == sharingGroupId && w.UserID == userId).FirstOrDefault();
                if (entity != null)
                {
                    _tableSharingGroupUser.DeleteOnSubmit(entity);
                    _tableSharingGroupUser.Context.SubmitChanges();
                }
            }
            catch (System.Exception)
            {
                return false;
            }
            return true;
        }
        public AddOrEditSharingGroupResponseDto AddSharingGroup(int userId, SharingGroupDto request)
        {
            try
            {
                var exitsName = _table.Where(w => w.Name == request.Name.Trim()
                    && w.DistrictID == request.DistrictID).FirstOrDefault();
                if (exitsName != null)
                {
                    return new AddOrEditSharingGroupResponseDto() { Success = false, Message = "Name already exists" };
                }
                var entity  = new SharingGroupEntity();
                entity.Name = request.Name;
                entity.Active = true;
                entity.DistrictID = request.DistrictID.HasValue ? request.DistrictID.Value : 0;
                entity.CreatedBy = userId;
                entity.CreatedDate = DateTime.UtcNow; 
                _table.InsertOnSubmit(entity);
                _table.Context.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                return new AddOrEditSharingGroupResponseDto() { Success = false, Message = ex.Message };
            }
            return new AddOrEditSharingGroupResponseDto();
        }
        public AddOrEditSharingGroupResponseDto CloneSharingGroup(SharingGroupDto request, int userId)
        {
            try
            {
                var exitsName = _table.Where(w => w.Name == request.Name.Trim() && w.DistrictID == request.DistrictID).FirstOrDefault();
                if (exitsName != null)
                {
                    return new AddOrEditSharingGroupResponseDto() { Success = false, Message = "Name already exists" };
                }

                _dbContext.SP_SharingGroup_CloneSharingGroup(userId, request.DistrictID, request.SharingGroupID, request.Name);
            }
            catch(System.Exception ex)
            {
                return new AddOrEditSharingGroupResponseDto() { Success = false, Message = ex.Message };
            }
            return new AddOrEditSharingGroupResponseDto();
        }
        public AddOrEditSharingGroupResponseDto EditSharingGroup(int userId, SharingGroupDto request)
        {
            try
            {
                var exitsName = _table.Where(w => w.SharingGroupID != request.SharingGroupID
                    && w.Name == request.Name.Trim()
                    && w.DistrictID == request.DistrictID).FirstOrDefault();
                if (exitsName != null)
                {
                    return new AddOrEditSharingGroupResponseDto() { Success = false, Message = "Name already exists" };
                }
                var entity = _table.Where(w => w.SharingGroupID == request.SharingGroupID).FirstOrDefault();
                if (entity != null)
                {
                    entity.Name = request.Name;
                    entity.UpdatedBy = userId;
                    entity.UpdatedDate = DateTime.UtcNow;
                    _table.Context.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                return new AddOrEditSharingGroupResponseDto() { Success = false, Message = ex.Message };
            }
            return new AddOrEditSharingGroupResponseDto();
        }
        public SharingGroupDto GetDetailSharingGroup(int sharingGroupId)
        {
            try
            {
                var entity = _table.Where(w => w.SharingGroupID == sharingGroupId).FirstOrDefault();
                if (entity != null)
                {
                    var sharingGroup = new SharingGroupDto();
                    sharingGroup.SharingGroupID = entity.SharingGroupID;
                    sharingGroup.Name = entity.Name;
                    sharingGroup.Active = entity.Active;
                    sharingGroup.DistrictID = entity.DistrictID;
                    sharingGroup.CreatedBy = entity.CreatedBy;
                    sharingGroup.IsPublished = entity.IsPublished;
                    return sharingGroup;
                }
            }
            catch (System.Exception)
            {
                return null;
            }
            return null;
        }
        public bool PublishOrUnpublishSharingGroup(int userId, int sharingGroupId, bool isPublished)
        {
            try
            {
                _dbContext.SP_SharingGroup_PublishSharingGroup(userId, sharingGroupId, isPublished);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool ChangeStatusSharingGroup(int sharingGroupId)
        {
            try
            {
                var entity = _table.Where(w => w.SharingGroupID == sharingGroupId).FirstOrDefault();
                if (entity != null)
                {
                    entity.Active = !entity.Active;
                    _table.Context.SubmitChanges();
                }
            }
            catch (System.Exception)
            {
                return false;
            }
            return true;
        }
        public bool DeleteSharingGroup(int sharingGroupId)
        {
            try
            {
                _dbContext.SP_SharingGroup_DeleteSharingGroup(sharingGroupId);
            }
            catch (System.Exception)
            {
                return false;
            }
            return true;
        }
        public SharingGroupResponseDto GetSharingGroups(GetSharingGroupPaginationRequest request)
        {
            var result = new SharingGroupResponseDto();
            int? totalRecord = 0;
            var data = _dbContext.SP_SharingGroup_GetSharingGroups(request.RoleID, request.UserID,
                request.DistrictID, request.GeneralSearch, request.ShowInactiveSharingGroup,
                request.ShowCreatedByOtherSharingGroup, request.SortColumn,
                request.SortDirection, request.StartRow,
                request.PageSize,
                ref totalRecord).ToList();
            if (data != null)
            {
                result.Data = data.Select(s => new SharingGroupDto()
                {
                    SharingGroupID = s.SharingGroupID,
                    Name = s.Name,
                    Active = s.Active,
                    DistrictID = s.DistrictID,
                    StrUserGroupName = s.StrUserGroupName,
                    OwnerName = s.OwnerName,
                    IsPublished = s.IsPublished.HasValue ? s.IsPublished.Value : false,
                    CreatedBy = s.CreatedBy ?? 0
                }).ToList();
                result.TotalRecord = totalRecord.HasValue ? totalRecord.Value : 0;
                return result;
            }
            return new SharingGroupResponseDto();
        }
        public UserInSharingGroupResponseDto GetUserInSharingGroup(GetUserInSharingGroupPaginationRequest request)
        {
            var result = new UserInSharingGroupResponseDto();
            int? totalRecord = 0;
            var users = _dbContext.SP_SharingGroup_GetUserInSharingGroup(request.RoleID, request.UserID,
                request.DistrictID, request.SharingGroupID, request.IsShowInactiveUser, request.GeneralSearch, request.SortColumn,
                request.SortDirection, request.StartRow,
                request.PageSize,
                ref totalRecord).ToList();
            if (users != null)
            {
                result.Data = users.Select(s => new UserInSharingGroupDto()
                {
                    SchoolList = s.SchoolList ?? string.Empty,
                    UserId = s.UserID,
                    UserStatusId = s.UserStatusID,
                    FirstName = s.NameFirst,
                    LastName = s.NameLast,
                    RoleName = s.RoleName,
                    UserName = s.UserName,
                    DistrictId = s.DistrictID,
                    RoleId = s.RoleID,
                    SharingGroupName = s.SharingGroupName,
                    UserStatusName = s.UserStatusName
                }).ToList();
                result.TotalRecord = totalRecord.HasValue ? totalRecord.Value : 0;
                return result;
            }
            return new UserInSharingGroupResponseDto();
        }
        public SharingGroupResponseDto GetSharingGroups_V2(GetSharingGroupPaginationRequest request)
        {
            var result = new SharingGroupResponseDto();
            var parameters = new List<(string, string, SqlDbType, object, ParameterDirection)>();
            parameters.Add(("UserID", "", SqlDbType.Int, request.UserID, ParameterDirection.Input));
            parameters.Add(("RoleID", "", SqlDbType.Int, request.RoleID, ParameterDirection.Input));
            parameters.Add(("DistrictID", "", SqlDbType.Int, request.DistrictID, ParameterDirection.Input));
            parameters.Add(("GeneralSearch", "", SqlDbType.VarChar, request.GeneralSearch.ToString(), ParameterDirection.Input));
            parameters.Add(("SortColumn", "", SqlDbType.VarChar, request.SortColumn.ToString(), ParameterDirection.Input));
            parameters.Add(("SortDirection", "", SqlDbType.VarChar, request.SortDirection.ToString(), ParameterDirection.Input));
            parameters.Add(("StartRow", "", SqlDbType.Int, request.StartRow, ParameterDirection.Input));
            parameters.Add(("PageSize", "", SqlDbType.Int, request.PageSize, ParameterDirection.Input));
            parameters.Add(("TotalRecord", "", SqlDbType.Int, request.TotalRecord, ParameterDirection.Input));
            var res = _dbContext.Query<SharingGroupDto>(new SqlParameterRequest()
            {
                StoredName = "PT_GetSharingGroups",
                Parameters = parameters

            }, out _);
            result.Data = res;
            return result;
        }
    }
}
