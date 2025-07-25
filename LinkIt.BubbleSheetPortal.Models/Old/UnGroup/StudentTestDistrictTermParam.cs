using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.Old.UnGroup
{
    public class StudentTestDistrictTermParam
    {
        public int? DistrictId { get; set; }

        public int? SchoolId { get; set; }

        public int? TeacherId { get; set; }

        public int? DistrictTermId { get; set; }

        public int? ClassId { get; set; }

        public List<int> VirtualTestIds { get; set; }

        public List<int> VirtualTestSubTypeIds { get; set; }

        public DateTime? ResultDateFrom { get; set; }

        public DateTime? ResultDateTo { get; set; }
    }
}
