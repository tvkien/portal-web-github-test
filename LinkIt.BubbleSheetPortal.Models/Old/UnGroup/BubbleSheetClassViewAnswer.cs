namespace LinkIt.BubbleSheetPortal.Models
{
    public class BubbleSheetClassViewAnswer
    {
        public int BubbleSheetId { get; set; }
        public int StudentId { get; set; }
        public int VirtualQuestionId { get; set; }
        public int QuestionOrder { get; set; }
        public int PointsPossible { get; set; }
        public int QTISchemaId { get; set; }
        public string AnswerIdentifiers { get; set; }
        public string XmlContent { get; set; }
        public string CorrectAnswer { get; set; }
        public string Status { get; set; }
        public string AnswerLetter { get; set; }
        public bool WasAnswered { get; set; }
        public int MaxChoice { get; set; }
    }

    public class QuestionClassView
    {
        public int VirtualQuestionId { get; set; }
        public int QTISchemaId { get; set; }
        public string XmlContent { get; set; }
        public string CorrectAnswer { get; set; }
        public int MaxChoice { get; set; }
    }


}