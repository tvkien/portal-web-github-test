using Envoc.Core.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.Old.SharingGroup
{
    public class SharingGroupDto
    {
        public int? SharingGroupID { get; set; }
        public string Name { get; set; }
        public bool? Active { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? DistrictID { get; set; }
        public string StrUserGroupName { get; set; }
        public bool ShowInactiveSharingGroup { get; set; }
        public bool ShowCreatedByOtherSharingGroup { get; set; }
        public string OwnerName { get; set; }
        public bool? IsPublished { get; set; }
    }
}
