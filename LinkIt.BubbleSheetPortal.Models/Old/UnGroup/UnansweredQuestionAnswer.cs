namespace LinkIt.BubbleSheetPortal.Models
{
    public class UnansweredQuestionAnswer
    {
        public int QuestionId { get; set; }
        public int QuestionOrder { get; set; }
        public string SelectedAnswer { get; set; }

        //\
        public int SectionIndex { get; set; }
        public int SectionQuestionIndex { get; set; }

        public bool IsTextEntryQuestion { get; set; }
    }
}