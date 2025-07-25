using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestPropertyExportData
    {
        public int VirtualTestID { get; set; }
        public string TestName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public int QuestionCount { get; set; }
        public int TotalPointsPossible { get; set; }
        public int TestResultCount { get; set; }
        public DateTime? EarliestResultDate { get; set; }
        public DateTime? MostRecentResultDate { get; set; }
        public string TestCategory { get; set; }
        public bool? InterviewStyleAssessment { get; set; }
        public string BankName { get; set; }
        public string BankGrade { get; set; }
        public string BankSubject { get; set; }
        public int? QuestionNumber { get; set; }
        public string PassageNumber { get; set; }
        public int? QTIItemID { get; set; }
        public string QTIItemTitle { get; set; }
        public string VirtualQuestionTags { get; set; }
        public string QTISchemaName { get; set; }
        public int? PointsPossible { get; set; }
        public string StandardNumbers { get; set; }
        public string XmlContent { get; set; }
        public string ResponseProcessing { get; set; }
        public int QTISchemaID { get; set; }
        public bool? IsRubricBasedQuestion { get; set; }
        public string CorrectAnswer { get; set; }
        public string AnswerKey { get; set; }
        public string AlgorithmicExpression { get; set;}
        public int VirtualQuestionID { get; set; }

    }
}
