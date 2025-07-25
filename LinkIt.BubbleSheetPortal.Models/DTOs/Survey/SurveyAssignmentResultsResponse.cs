using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.Survey
{
    public class SurveyAssignmentResultsResponse
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public string Respondent { get; set; }
        public string Email { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string ShortLink { get; set; }
        public string Code { get; set; }
        public int? StudentId { get; set; }
        public int? TotalRecords { get; set; }
    }
}
