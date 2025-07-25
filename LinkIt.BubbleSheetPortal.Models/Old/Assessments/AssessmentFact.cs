using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AssessmentFact
    {
        public string DistrictCode { get; set; }
        public DateTime ResultDate { get; set; }
        public string TestName { get; set; }
        public string StudentCode { get; set; }
        public int TotalPointsEarned { get; set; }
        public int TotalPointsPossible { get; set; }
        public int Year { get; set; }
    }
}
