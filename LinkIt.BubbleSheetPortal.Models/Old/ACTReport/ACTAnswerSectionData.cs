namespace LinkIt.BubbleSheetPortal.Models.ACTReport
{
    public class ACTAnswerSectionData
    {
        public int AnswerID { get; set; }
        public int PointsEarned { get; set; }
        public int PointsPossible { get; set; }
        public bool WasAnswered { get; set; }
        public string AnswerLetter { get; set; }
        public int QuestionOrder { get; set; }
        public int SectionID { get; set; }
        public string SectionName { get; set; }
        public int SectionOrder { get; set; }
        public int TagID { get; set; }
        public string TagName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CorrectAnswer { get; set; }
        public int QTISchemaID { get; set; }
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
        public int VirtualQuestionID { get; set; }
        public string PassageName { get; set; }
        public int? PassageID { get; set; }
    }
}