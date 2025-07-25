using LinkIt.BubbleService.Models.Reading;
using LinkIt.BubbleService.Shared.Data;
using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class ErrorCorrectionControllerParameters
    {
        public BubbleSheetErrorService BubbleSheetErrorService { get; set; }

        public BubbleSheetFileService BubbleSheetFileService { get; set; }

        public UnansweredQuestionService UnansweredQuestionService { get; set; }

        public BubbleSheetStudentResultsService BubbleSheetStudentResultsService { get; set; }

        public TestResubmissionService TestResubmissionService { get; set; }

        public ReadResultService ReadResultService { get; set; }

        public SchoolService SchoolService { get; set; }

        public ACTAnswerQuestionService actAnswerQuestionService { get; set; }

        
        public VirtualTestService VirtualTestService { get; set; }

    }
}