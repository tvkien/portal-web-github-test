using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BubbleSheetStudentResults
    {
        public string StudentName { get; set; }
        public string Status { get; set; }
        public string Ticket { get; set; }
        public int StudentId { get; set; }
        public int? AnsweredCount { get; set; }
        public int? TotalCount { get; set; }
        public int PointsEarned { get; set; }
        public int PointsPossible { get; set; }
        public int? ClassId { get; set; }
        public int? ProcessedPage { get; set; }
        public int BubbleSheetId { get; set; }
        public int RosterPosition { get; set; }
        public BubbleSheetFinalStatus BubbleSheetFinalStatus { get; set; }
    }
}