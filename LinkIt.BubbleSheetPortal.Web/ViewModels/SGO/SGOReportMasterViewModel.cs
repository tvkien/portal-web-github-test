using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class SGOReportMasterViewModel
    {
        public int SgoId { get; set; }

        public SGOObject SgoObject { get; set; }
        public SGOCustomReport SgoCustomInformation { get; set; }        

        public string DistrictName { get; set; }

        public List<SGOReportDataPoint> PreAssessmentDataPoints { get; set; }
        public List<SGOReportDataPoint> PostAssessmentDataPoints { get; set; }
        public List<SGOReportDataPointFilter> SgoReportDataPointFilters { get; set; }

        public List<SGOReportDataPointViewModel> ListDataPoints { get; set; }

        public List<SGOAuditTrailSearchItem> ListAuditTrail { get; set; }

        public int TargetScoreType { get; set; }
        public List<SGOGroup> SgoGroups { get; set; }
        public List<SGOAttainmentGroup> SgoAttainmentGroups { get; set; }
        public List<SGOAttainmentGoal> SgoAttainmentGoals { get; set; }


        public List<SGOCalculateScoreResult> SgoCalculateScoreResults { get; set; }
        public string DescriptiveLabel { get; set; }
        public decimal TotalTeacherSGOScore { get; set; }
        public string TotalTeacherSGOScoreCustom { get; set; }


        public List<SGOScoringDetail> SgoScoringDetails { get; set; }
        public string ScoringDetailPostAssessmentTestName { get; set; }
        public string ScoringDetailPreAssessmentTestName { get; set; }
        public int SgoType { get; set; }

        public string GeneratedTime
        {
            get
            {
                var strCurrentDateTime = string.Format(String.Format("{0:g}", DateTime.UtcNow));
                return string.Format("Generated: {0}", strCurrentDateTime);
            }
        }
    }
}