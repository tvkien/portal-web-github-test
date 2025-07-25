using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class NextApplicableQuestion
    {
        public int StudentID { get; set; }
        public int VirtualQuestionID { get; set; }
        public int QTIOnlineTestSessionAnswerID { get; set; }
        public bool IsLastQuestion { get; set; }
    }
}