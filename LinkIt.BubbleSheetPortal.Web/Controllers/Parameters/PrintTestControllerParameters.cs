using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.Isolating;
using LinkIt.BubbleSheetPortal.Web.BusinessObjects;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class PrintTestControllerParameters
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
        public VirtualTestFileService VirtualTestFileService { get; set; }
        public TestFeedbackService TestFeedbackService { get; set; }
        public VirtualTestService VirtualTestService { get; set; }
        public BankService BankService { get; set; }
        public DistrictDecodeService DistrictDecodeService { get; set; }
        public ManageTestService ManageTestService { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }
        public QTIOnlineTestSessionService QTIOnlineTestSessionService { get; set; }
        public IsolatingTestTakerService IsolatingTestTakerService { get; set; }
        public DownloadPdfService DownloadPdfService { get; set; }
        public BatchPrintingQueueService BatchPrintingQueueService { get; set; }
        public RestrictionBO RestrictionBO { get; set; }
        public S3Service S3Service { get; set; }
    }
}
