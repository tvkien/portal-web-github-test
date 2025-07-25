using LinkIt.BubbleSheetPortal.Models.Old.UserGroup;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class GetModuleAccessResponse
    {
        public int TotalRecord { get; set; }

        public IEnumerable<GetModuleAccessDataDto> Data { get; set; }
    }

    public class GetModuleAccessDataDto : XLIModuleAccessDto
    {
        public int? GroupId { get; set; }
        public string GroupName { get; set; }

        public GetModuleAccessDataDto(XLIModuleAccessDto dto, int? groupId, string groupName)
        {
            AreaID = dto.AreaID;
            AreaName = dto.AreaName;
            CurrentAccess = dto.CurrentAccess;
            DistrictAccess = dto.DistrictAccess;
            ModuleCode = dto.ModuleCode;
            ModuleName = dto.ModuleName;
            ModuleID = dto.ModuleID;
            SchoolAccess = dto.SchoolAccess;
            UserGroupAccess = dto.UserGroupAccess;
            GroupId = groupId;
            GroupName = groupName;
        }
    }
}
