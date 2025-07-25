using LinkIt.BubbleSheetPortal.InteractiveRubric.Services;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.Survey;
using LinkIt.BubbleSheetPortal.Services.TestMaker;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class TestMakerParameters
    {
        public QTIITemService QtiItemService { get; set; }
        public QTIItemConvert QTIItemConvert { get; set; }
        public QtiGroupService QtiGroupService { get; set; }
        public VirtualQuestionService VirtualQuestionService { get; set; }
        public VirtualSectionQuestionService VirtualSectionQuestionService { get; set; }
        public VirtualSectionService VirtualSectionService { get; set; }
        public VirtualTestService VirtualTestService { get; set; }
        public QtiBankService QtiBankService { get; set; }
        public DistrictDecodeService DistrictDecodeService { get; set; }
        public ManageTestService ManageTestService { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }
        public QTIItemPreviewRequestService QTIItemPreviewRequestService { get; set; }
        public ConfigurationService ConfigurationService { get; set; }
        public APIAccountService APIAccountService { get; set; }
        public VirtualQuestionPassageNoShuffleService VirtualQuestionPassageNoShuffleService { get; set; }
        public AlgorithmicScoreService AlgorithmicScoreService { get; set; }
        public QuestionGroupService QuestionGroupServices { get; set; }
        public MultiPartExpressionService MultiPartExpressionService { get; set; }
        public IRubricModuleQueryService RubricModuleQueryService { get; set; }
        public IRubricModuleCommandService RubricModuleCommandService { get; set; }

        public QTIRefObjectService PassageService { get; set; }
        public Qti3pPassageService qti3pPassageService { get; set; }
        public ManageSurveyService ManageSurveyService { get; set; }
        public QTIOnlineTestSessionService QTIOnlineTestSessionService { get; set; }
        public DataFileUploadPassageService dataFileUploadPassageService { get; set; }
    }
}
