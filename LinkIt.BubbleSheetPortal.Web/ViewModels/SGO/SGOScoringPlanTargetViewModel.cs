using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class SGOScoringPlanTargetViewModel
    {
        public int SgoId { get; set; }
        public int TargetScoreType { get; set; }
        public int SgoType { get; set; }

        public List<SGOGroup> SgoGroups { get; set; }        
        public List<SGOAttainmentGroup> SgoAttainmentGroups { get; set; }
        public List<SGOAttainmentGoal> SgoAttainmentGoals { get; set; }
        public List<SelectListItem> PreAssessmentSelectListItems { get; set; }
        public List<SelectListItem> PreAssessmentCustomSelectListItems { get; set; }

        public bool HavePostAssessmentToBeCreated { get; set; }
        public int ToBeCreatedTotalPointPossible { get; set; }
        public string PostAssessmentLinkitTotalPointPossible { get; set; }

        public bool HavePostAssessment { get; set; }
        public bool HavePostAssessmentCustom { get; set; }
        public bool IsCustomTextScoreTypePostAssessment { get; set; }

        public string RationaleUnstructured { get; set; }
        public string AttachUnstructuredUrl { get; set; }
        public string AttachUnstructuredDownloadUrl { get; set; }
    }
}