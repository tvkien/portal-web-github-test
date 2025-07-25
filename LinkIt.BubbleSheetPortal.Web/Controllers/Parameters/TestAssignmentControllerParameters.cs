using LinkIt.BubbleSheetPortal.DynamoIsolating.Services;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Services;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.CodeGen;
using LinkIt.BubbleSheetPortal.Services.Isolating;
using LinkIt.BubbleSheetPortal.Services.TeacherReviewer;
using LinkIt.BubbleSheetPortal.Services.TestResultRemover;
using LinkIt.BubbleSheetPortal.Web.BusinessObjects;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class TestAssignmentControllerParameters
    {
        public UserService UserServices { get; set; }
        public IncorrectQuestionOrderService IncorrectQuestionOrderServices { get; set; }
        public QTITestClassAssignmentService QTITestClassAssignmentServices { get; set; }
        public DistrictSettingsService DistrictSettingsServices { get; set; }
        public QTITestStudentAssignmentService QTITestStudentAssignmentServices { get; set; }
        public ConfigurationService ConfigurationServices { get; set; }
        public ClassPrintingGroupService ClassPrintingGroupServices { get; set; }
        public ProgramService ProgramServices { get; set; }
        public TestService TestServices { get; set; }
        public ClassStudentCustomService classStudentCustomServices { get; set; }
        public ClassService ClassServices { get; set; }
        public StudentService StudentServices { get; set; }
        public ClassCustomService ClassCustomServices { get; set; }
        public PreferencesService PreferencesServices { get; set; }
        public ListBankService ListBankServices { get; set; }

        public BankService BankServices { get; set; }
        public DistrictDecodeService DistrictDecodeServices { get; set; }
        public TestFeedbackService TestFeedbackService { get; set; }
        public ItemFeedbackService ItemFeedbackService { get; set; }
        public QTIOnlineTestSessionService QTIOnlineTestSessionService { get; set; }
        public TestResultService TestResultService { get; set; }

        public APIAccountService APIAccountServices { get; set; }
        public TestResultScoreService TestResultScoreService { get; set; }
        public BankService BankService { get; set; }
        public DistrictDecodeService DistrictDecodeService { get; set; }
        public VirtualTestService VirtualTestService { get; set; }
        public ManageTestService ManageTestService { get; set; }
        public IsolatingTestTakerService IsolatingTestTakerService { get; set; }
        public BankDistrictService BankDistrictService { get; set; }
        public TestResultRemoverService TestResultRemoverService { get; set; }
        public StudentPreferencesService StudentPreferencesService { get; set; }
        public VirtualTestDistrictService VirtualTestDistrictService { get; set; }
        public VirtualQuestionService VirtualQuestionService { get; set; }
        public TestCodeGenerator TestCodeGenerator { get; set; }
        public AnswerService AnswerService { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }
        public ITeacherReviewerService TeacherReviewerService { get; set; }
        public QuestionGroupService QuestionGroupService { get; set; }

        public DistrictService DistrictServices { get; set; }
        public RestrictionBO RestrictionBO { get; set; }
        public XLIMenuPermissionService XLIMenuPermissionService { get; set; }
        public XLIAreaDistrictModuleService XLIAreaDistrictModuleService { get; set; }

        public IRubricModuleQueryService RubricQuestionCategoryService { get; set; }

        public IRubricModuleCommandService RubricModuleCommandService { get; set; }
        public SchoolService SchoolService { get; set; }
        public GetOnlineTestSessionStatusIsolatingService GetOnlineTestSessionDynamoService { get; set; }
        public StateService StateService { get; set; }

        public XmlContentSerializer XmlContentSerializer { get; set; }

        public ClassStudentService ClassStudentService { get; set; }

        public VirtualTestVirtualTestCustomScoreService VirtualTestVirtualTestCustomScoreService { get; set; }

        public VirtualTestFileService VirtualTestFileServices { get; set; }
        public VirtualSectionService VirtualSectionServices { get; set; }

        public VirtualSectionQuestionService VirtualSectionQuestionServices { get; set; }

        public VirtualQuestionService VirtualQuestionServices { get; set; }

        public QtiBankService QtiBankServices { get; set; }

        public QtiGroupService QtiGroupServices { get; set; }

        public QTIITemService QTIITemServices { get; set; }
        public QuestionGroupService VirtualQuestionGroupService { get; set; }

        public VirtualQuestionTopicService VirtualQuestionTopicServices { get; set; }

        public VirtualQuestionItemTagService VirtualQuestionItemTagServices { get; set; }

        public VirtualQuestionLessonOneService VirtualQuestionLessonOneServices { get; set; }

        public VirtualQuestionLessonTwoService VirtualQuestionLessonTwoServices { get; set; }
        public MasterStandardService MasterStandardServices { get; set; }
        public PerformanceBandAutomationService PerformanceBandAutomationService { get; set; }
        public ParentConnectService ParentConnectService { get; set; }

        public AuthenticationCodeGenerator AuthenticationCodeGenerator { get; set; }
    }
}
