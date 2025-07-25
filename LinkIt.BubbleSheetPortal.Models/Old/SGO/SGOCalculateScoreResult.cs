using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOCalculateScoreResult
    {
        public int SGOGroupID { get; set; }
        public string Name { get; set; }
        public decimal? PercentStudentAtTargetScore { get; set; }
        public decimal? TeacherScore { get; set; }
        public decimal? Weight { get; set; }
        public decimal? WeightedScore { get; set; }

        public string AttainmentGoal { get; set; }
    }
}
