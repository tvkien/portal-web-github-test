using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOAttainmentGroup
    {
        public int SGOAttainmentGroupId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public int? GoalValue { get; set; }
        public int SGOGroupId { get; set; }
        public int SGOAttainmentGoalId { get; set; }
        public string GoalValueCustom { get; set; }

        // Custom Property
        public string ComparisonType { get; set; }
    }
}
