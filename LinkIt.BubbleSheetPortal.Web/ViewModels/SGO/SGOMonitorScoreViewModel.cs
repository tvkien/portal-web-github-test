using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class SGOMonitorScoreViewModel
    {
        public int PermissionAccess { get; set; }
        public int SgoId { get; set; }
        public int ApproverUserID { get; set; }

        public bool HavePostAssessmentLinkit { get; set; }
        public bool IsPostAssessmentLinkitHasTestResult { get; set; }

        public string EducatorComments { get; set; }

        public bool IsReviewer { get; set; }

        public string GeneratedDate { get; set; }

        public bool IsSaveResultScore { get; set; }
        public string DirectionConfigurationValue { get; set; }
        public int SgoType { get; set; }

        public string AttachUnstructuredUrl { get; set; }
        public string AttachUnstructuredDownloadUrl { get; set; }
    }
}