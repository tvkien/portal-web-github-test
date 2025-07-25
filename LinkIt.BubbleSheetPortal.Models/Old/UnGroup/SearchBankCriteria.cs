using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class SearchBankCriteria
    {
        public int? GradeId { get; set; }
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public int UserRole { get; set; }
        public string SubjectName { get; set; }
        public string ModuleCode { get; set; }
        public int SchoolId { get; set; }
        public string Level { get; set; }
        public string ClassIds { get; set; }
    }
}
