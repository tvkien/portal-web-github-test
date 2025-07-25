using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AssessmentResponse
    {
        public string DistrictCode { get; set; }
        public DateTime ResultDate { get; set; }
        public string TestName { get; set; }
        public string StudentCode { get; set; }
        public int QuestionOrder { get; set; }
        public string AnswerText { get; set; }
        public int PointsEarned { get; set; }
        public int Year { get; set; }
    }
}
