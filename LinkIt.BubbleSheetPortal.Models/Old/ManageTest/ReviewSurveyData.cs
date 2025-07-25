using System;

namespace LinkIt.BubbleSheetPortal.Models.Old.ManageTest
{
    public class ReviewSurveyData
    {
        public int VirtualTestId { get; set; }

        public string SurveyName { get; set; }
        public string TermName { get; set; }
        public string SchoolName { get; set; }
        public int? AssignmentType { get; set; }
        public int? Assignments { get; set; }
        public DateTime? MostRecentResponse { get; set; }
        public int? BankId { get; set; }
        public int? TermID { get; set; }
        public int? TotalRecords { get; set; }
    }
}
