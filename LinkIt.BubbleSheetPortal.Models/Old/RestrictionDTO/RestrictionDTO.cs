using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Models.RestrictionDTO
{
    public class RestrictionDTO
    {
        public int Id { get; set; }
        public PublishLevelTypeEnum PublishLevelType { get; set; }
        public int PublishLevelId { get; set; } // DistrictId or SchoolId
        public int TestRestrictionModuleId { get; set; }
        public string TestRestrictionModuleCode { get; set; }
        public RestrictionObjectType RestrictionObjectType { get; set; }
        public int RestrictionObjectId { get; set; }  // BankId or VirtualTestId
        public int RoleId { get; set; }
        public int UserId { get; set; }
    }
}