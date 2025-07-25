namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class QuestionGroupPropertiesViewModel
    {
        public QuestionGroupPropertiesViewModel()
        {
            VirtualTestId = 0;
            VirtualSectionId = 0;
            QuestionGroupId = 0;            
            Instruction = string.Empty;
            DisplayPosition = 0;
            QuestionGroupTitle = string.Empty;
            FirstQuestionInGroup = 0;
        }
        public int VirtualTestId { get; set; }
        public int VirtualSectionId { get; set; } 
        public string Instruction { get; set; }
        public int QuestionGroupId { get; set; }
        public int DisplayPosition { get; set; }
        public string QuestionGroupTitle { get; set; }
        public bool IsShowNormalBranchingButton { get; set; }

        //Extra Property
        public int FirstQuestionInGroup { get; set; }
    }
}
