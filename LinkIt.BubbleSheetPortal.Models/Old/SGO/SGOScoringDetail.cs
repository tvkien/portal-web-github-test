using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOScoringDetail
    {
        public int? StudentId { get; set; }
        public int? SgoStudentId { get; set; }
        public int? GroupOrder { get; set; }
        public string GroupName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PostScore { get; set; }
        public string BasedScore { get; set; }
        public string AchievedScore { get; set; }
        public int? AchievedTarget { get; set; }
    }
}
