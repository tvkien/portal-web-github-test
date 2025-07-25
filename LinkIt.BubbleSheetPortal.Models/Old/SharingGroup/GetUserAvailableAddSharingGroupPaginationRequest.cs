using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.Old.SharingGroup
{
    public class GetUserAvailableAddSharingGroupPaginationRequest : PaggingInfo
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public int DistrictID { get; set; }
        public int SharingGroupID { get; set; }
        public string RoleIdList { get; set; }
        public string SchoolIdList { get; set; }
        public bool ShowInactiveUser { get; set; }
        public string GeneralSearch { get; set; }
        public int TotalRecord { get; set; }
    }
    public class GetUserAvailableAddSharingGroupRequest : GenericDataTableRequest
    {
        public int DistrictID { get; set; }
        public int SharingGroupID { get; set; }
        public bool ShowInactiveUser { get; set; }
        public string RoleIdList { get; set; }
        public string SchoolIdList { get; set; }
    }
}
