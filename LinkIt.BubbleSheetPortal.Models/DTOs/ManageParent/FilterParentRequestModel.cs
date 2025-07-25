using LinkIt.BubbleSheetPortal.Models.DTOs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent
{
    public class FilterParentRequestModel : GenericDataTableRequestBase
    {
        public int? DistrictId { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public string ParentName { get; set; }
        public string StudentName { get; set; }
        public int? SchoolId { get; set; }
        public int? GradeId { get; set; }
        public bool ShowInactiveParent { get; set; }
        public int TimezoneOffset { get; set; }
        public string ParentDetailPrintingFileName { get; set; }
        public string SelectedUserIds { get; set; }
        public int LoginTimeFrame { get; set; }
        public bool? HasRegistrationCode { get; set; }
        public DateTime DateTimeUTC { get; set; }
    }
}
