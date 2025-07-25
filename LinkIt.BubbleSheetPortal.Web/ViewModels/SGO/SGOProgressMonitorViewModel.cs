using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class SGOProgressMonitorViewModel
    {
        public int SgoId { get; set; }
        public int PermissionAccess { get; set; }
        public bool IsHavePosstAssessment { get; set; }
        public string GeneratedDate { get; set; }
        public string DescriptiveLabel { get; set; }
        public decimal TotalTeacherSGOScore { get; set; }
        public List<SGOGroup> SgoGroups { get; set; }        
        public List<SGOCalculateScoreResult> SgoCalculateScoreResults { get; set; }

        public bool HavePostAssessmentLinkit { get; set; }
        public bool IsPostAssessmentLinkitHasTestResult { get; set; }
        public int SgoType { get; set; }
        public string AttachUnstructuredUrl { get; set; }
        public string AttachUnstructuredDownloadUrl { get; set; }
        public string TotalTeacherSGOScoreCustom { get; set; }

        public bool IsPreOrPostAssessmentHasScoreNull { get; set; }
    }
}
