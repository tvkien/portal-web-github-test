using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.BusinessObjects;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class AnswerViewerControllerParameters
    {
        public QTIITemService QTIITemServices { get; set; }
        public VirtualQuestionService VirtualQuestionServices { get; set; }
        public VulnerabilityService VulnerabilityServices { get; set; }
        public VirtualQuestionService VirtualQuestionService { get; set; }
        public VirtualTestService VirtualTestService { get; set; }
        public RestrictionBO RestrictionBO { get; set; }
        public QuestionGroupService QuestionGroupService { get; set; }
    }
}