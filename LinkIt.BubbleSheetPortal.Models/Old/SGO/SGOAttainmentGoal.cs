using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOAttainmentGoal
    {
        public int SGOAttainmentGoalId { get; set; }
        public int SGOId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public decimal DefaultGoal { get; set; }
        public string ComparisonType { get; set; }
    }
}
