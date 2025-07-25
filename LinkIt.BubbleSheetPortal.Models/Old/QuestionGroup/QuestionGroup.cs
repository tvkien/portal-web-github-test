namespace LinkIt.BubbleSheetPortal.Models.QuestionGroup
{
    public class QuestionGroup
    {
        public int QuestionGroupID { get; set; }
        public int? VirtualSectionID { get; set; }
        public string XmlContent { get; set; }
        public int Order { get; set; }
        public int VirtualTestId { get; set; }

        public int DisplayPosition { get; set; }

        public string Title { get; set; }
        public int OldQuestionGroupID { get; set; }
    }
}
