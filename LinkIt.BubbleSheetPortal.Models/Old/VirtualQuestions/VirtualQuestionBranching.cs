namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualQuestionBranching
    {
        public int VirtualQuestionBranchingID { get; set; }

        public int VirtualQuestionID { get; set; }

        public string AnswerChoice { get; set; }

        public int TargetVirtualQuestionID { get; set; }

        public int VirtualTestID { get; set; }

        public string Comment { get; set; }
    }
}
