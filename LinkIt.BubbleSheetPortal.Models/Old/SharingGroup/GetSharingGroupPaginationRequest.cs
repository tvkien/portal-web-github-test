using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.Old.SharingGroup
{
    public class GetSharingGroupPaginationRequest : PaggingInfo
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public int DistrictID { get; set; }
        public string GeneralSearch { get; set; }
        public int TotalRecord { get; set; }
        public bool ShowInactiveSharingGroup { get; set; }
        public bool ShowCreatedByOtherSharingGroup { get; set; }
    }
    public class GetSharingGroupRequest : GenericDataTableRequest
    {
        public int? DistrictID { get; set; }
        public bool ShowInactiveSharingGroup { get; set; }
        public bool ShowCreatedByOtherSharingGroup { get; set; }
    }
}
