using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.CodeGen;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class StudentTestLaunchControllerParameters
    {
        public UserService UserServices { get; set; }        
        public ListBankService ListBankServices { get; set; }
        public TimingOptionService TimingOptionService { get; set; }
        public VirtualTestTimingService VirtualTestTimingService { get; set; }
        public ClassCustomService ClassCustomServices { get; set; }
        public QTITestClassAssignmentService QTITestClassAssignmentServices { get; set; }
        public ConfigurationService ConfigurationServices { get; set; }
        public QTITestStudentAssignmentService QTITestStudentAssignmentServices { get; set; }
        public PreferencesService PreferencesServices { get; set; }
        public DistrictDecodeService DistrictDecodeService { get; set; }
        public StudentService StudentServices { get; set; }
        public StudentMetaService StudentMetaService { get; set; }
        public APIAccountService APIAccountServices { get; set; }
        public ClassStudentService ClassStudentService { get; set; }
        public TestService TestService { get; set; }
        public TestResultService TestResultService { get; set; }
        public QTIOnlineTestSessionService QTIOnlineTestSessionService { get; set; }
        public TestCodeGenerator TestCodeGenerator { get; set; }
        public AuthenticationCodeGenerator AuthenticationCodeGenerator { get; set; }
    }
}