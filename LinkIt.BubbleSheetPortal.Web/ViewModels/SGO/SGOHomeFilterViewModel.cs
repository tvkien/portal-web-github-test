using LinkIt.BubbleSheetPortal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    [Serializable]
    public class SGOHomeFilterViewModel
    {
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int? SchoolId { get; set; }
        public int? TeacherId { get; set; }
        public int? ReviewerId { get; set; }
        public int? DistrictTermId { get; set; }
        public bool IsArchivedStatusActive { get; set; }
        public bool IsArchivedStatusArchived { get; set; }
        public string SGOStatusIds { get; set; }
        public DateTime? InstructionPeriodFrom { get; set; }
        public DateTime? InstructionPeriodTo { get; set; }

        public string InstructionPeriodFromString
        {
            get { return InstructionPeriodFrom.DisplayDateWithFormat(); }
        }
        public string InstructionPeriodToString
        {
            get { return InstructionPeriodTo.DisplayDateWithFormat(); }
        }
    }
}