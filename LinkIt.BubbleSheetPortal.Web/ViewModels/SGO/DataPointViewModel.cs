using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class DataPointViewModel
    {
        public int SGODataPointId { get; set; }
        public int DataPointIndex { get; set; }
        public string Name { get; set; }
        public int? SubjectID { get; set; }
        public string SubjectName { get; set; }
        public int GradeId { get; set; }
        public int? VirtualTestId { get; set; }
        public string AttactScoreUrl { get; set; }
        public string AttactScoreDownloadLink { get; set; }
        public int SGOId { get; set; }
        public double Weight { get; set; }
        public decimal TotalPoints { get; set; }
        public string TestType { get; set; }
        public string DataPointGroupType { get; set; }
        public int? DataSetCategoryID { get; set; }
        public DateTime? ResultDate { get; set; }
        public string RationaleGuidance { get; set; }
        public int ScoreType { get; set; }       
        public int? VirtualTestCustomSubScoreId { get; set; }

        public string StateStandardFilters { get; set; }
        public string TopicFilters { get; set; }
        public string SkillFilters { get; set; }
        public string OtherFilters { get; set; }
        public string ClusterScoreFilters { get; set; }
        public string StudentDataPointData { get; set; }

        public int? BypassDataPointNumberRestriction { get; set; }
        public string DirectionConfigurationValue { get; set; }
        public bool? CreateTemporaryExternalVirtualTest { get; set; }

        public int HasReview { get; set; }
    }
}
