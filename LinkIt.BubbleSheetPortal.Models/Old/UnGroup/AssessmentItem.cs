using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AssessmentItem
    {
        public string DistrictCode { get; set; }
        public int Year { get; set; }
        public string TestName { get; set; }
        public string SubjectName { get; set; }
        public string BankName { get; set; }
        public string GradeName { get; set; }
        public string DistrictTermName { get; set; }
        public string SubtestHighestGradeLevel { get; set; }
        public string SubtestLowestGradeLevel { get; set; }
    }
}
