using System;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ReviewSurvey
{
    public class ReviewSurveyViewModel
    {
        public int VirtualTestId { get; set; }

        public string SurveyName { get; set; }
        public string TermName { get; set; }
        public string SchoolName { get; set; }
        public string AssignmentType { get; set; }
        public int? Assignments { get; set; }
        public DateTime? MostRecentResponse { get; set; }

        public string UrlAssignSurvey
        {
            get
            {
                return $"?surveyBankId={BankId}&SurveyId={VirtualTestId}&TermId={TermID}&surveyAssignmentType={AssignmentTypeRawValue}";
            }
        }

        public int? BankId { get; set; }
        public int? TermID { get; set; }
        public int? AssignmentTypeRawValue { get; set; }
    }
}
