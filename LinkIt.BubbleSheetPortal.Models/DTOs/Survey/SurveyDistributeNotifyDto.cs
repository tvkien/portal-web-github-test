using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.Survey
{
    public class SurveyDistributeNotifyDto
    {
        public int? DistrictId { get; set; }
        public int SurveyId { get; set; }
        public string SurveyName { get; set; }
        public string PublishedBy { get; set; }
        public List<DistributeInformation> DistributeInformations { get; set; }
        public int SurveyAssignmentType { get; set; }
    }

    public class DistributeInformation
    {
        public string Email { get; set; }
        public string SurveyLink { get; set; }
        public int QTITestClassAssignmentId { get; set; }
    }
}
