using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TLDSGuardiantContactDetailViewModel
    {
        public int? GuardiantContactDetailId { get; set; }
        public int? ProfileId { get; set; }
        public string FullName { get; set; }
        public string RelationshipToChild { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}