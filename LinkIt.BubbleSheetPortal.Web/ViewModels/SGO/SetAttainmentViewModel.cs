using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class SetAttainmentViewModel
    {
        public int TargetScoreType { get; set; }
        public string TotalPointsPossibleOfPostAssessmentValue { get; set; }
        public SGOAttainmentGoal SgoAttainmentGoal { get; set; }
        public List<SGOGroup> SgoGroups { get; set; }
        public List<SGOAttainmentGroup> SgoAttainmentGroups { get; set; }
    }
}