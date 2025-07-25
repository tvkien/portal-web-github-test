using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOCustomReport
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Teacher { get; set; }
        public string School { get; set; }
        public string Grade { get; set; }
        public string Course { get; set; }

        public string DistrictTerms { get; set; }
        public int? TotalStudent { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string EffectiveStatus { get; set; }
        public DateTime? EffectiveStatusDate { get; set; }
        public int Version { get; set; }
        public bool IsArchived { get; set; }
        public int OwnerUserID { get; set; }
        public int ApproverUserID { get; set; }

        public string ApproverName { get; set; }

        public string Feedback { get; set; }
        public string EducatorComment { get; set; }
        public string AdminComment { get; set; }
        public string TeacherComment { get; set; }
    }
}
