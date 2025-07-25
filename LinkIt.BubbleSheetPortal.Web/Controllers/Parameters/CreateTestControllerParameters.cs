using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.AuthorizeItemLibServices;
using LinkIt.BubbleSheetPortal.Services.TestMaker;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class CreateTestControllerParameters
    {
        public DistrictService DistrictService { get; set; }
        public BankService BankService { get; set; }
        public VirtualTestService VirtualTestService { get; set; }
        public SubjectService SubjectService { get; set; }
        public AuthorizeItemLibService AuthorizeItemLibService { get; set; }
        public VirtualQuestionService VirtualQuestionService { get; set; }
        public VirtualSectionService VirtualSectionService { get; set; }
        public VirtualSectionQuestionService VirtualSectionQuestionService { get; set; }
        public QTIITemService QTIITemService { get; set; }
    }
}