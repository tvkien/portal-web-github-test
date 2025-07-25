using System;

namespace LinkIt.BubbleSheetPortal.Models.Isolating
{
    public class OnlineTestSessionAnswerDTO
    {
        public bool? Answered { get; set; }
        public int? QuestionOrder { get; set; }
        public int? AnswerOrder { get; set; }
        public int VirtualQuestionID { get; set; }
        public bool? ManualReview { get; set; }
        public int QTIOnlineTestSessionID { get; set; }
        public DateTime? TimeStamp { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public int? StatusID { get; set; }
        public int? TimesVisited { get; set; }
        public int? TimeSpent { get; set; }
    }
}
