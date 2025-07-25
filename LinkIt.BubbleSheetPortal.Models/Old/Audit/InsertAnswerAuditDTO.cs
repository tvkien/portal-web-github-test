namespace LinkIt.BubbleSheetPortal.Models.Audit
{
    public class InsertAnswerAuditDTO
    {
        public int? AnswerID { get; set; }
        public int? AnswerSubID { get; set; }
        public QTIOnlineTestSession QTIOnlineTestSession { get; set; }
        public int UserID { get; set; }

        public int NewPointsEarned { get; set; }
        public int? PreviousPointsEarned { get; set; }
        public int VirtualQuestionID { get; set; }
        public int VirtualQuestionSubID { get; set; }
        public int QTIOnlineTestSessionID { get; set; }
    }
}
