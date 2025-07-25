using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ReviewSurvey
{
    public class ReviewSurveyRequestViewModel
    {
        public int? DistrictId { get; set; }
        public int? SurveyBankId { get; set; }
        public int? SurveyId { get; set; }
        public int? TermId { get; set; }
        public SurveyAssignmentTypeEnum? SurveyAssignmentType { get; set; }
        public bool ShowActiveAssignment { get; set; }
    }
}
