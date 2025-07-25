using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.Reporting;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class DataLockerControllerParameters
    {
        public ManageTestService ManageTestService { get; set; }

        public VirtualTestService VirtualTestService { get; set; }

        public DistrictService DistrictServices { get; set; }

        public BankService BankServices { get; set; }

        public SubjectService SubjectServices { get; set; }
        public BankDistrictService BankDistrictService { get; set; }
        public BankSchoolService BankSchoolService { get; set; }
        public SchoolService SchoolService { get; set; }
        public UserSchoolService UserSchoolService { get; set; }
        public DistrictDecodeService DistrictDecodeService { get; set; }

        public VirtualSectionService VirtualSectionServices { get; set; }

        public VirtualSectionQuestionService VirtualSectionQuestionServices { get; set; }

        public VirtualQuestionService VirtualQuestionServices { get; set; }

        public QtiBankService QtiBankServices { get; set; }

        public QtiGroupService QtiGroupServices { get; set; }

        public QTIITemService QTIITemServices { get; set; }

        public VirtualTestFileService VirtualTestFileServices { get; set; }

        public VirtualQuestionTopicService VirtualQuestionTopicServices { get; set; }

        public VirtualQuestionItemTagService VirtualQuestionItemTagServices { get; set; }

        public VirtualQuestionLessonOneService VirtualQuestionLessonOneServices { get; set; }

        public VirtualQuestionLessonTwoService VirtualQuestionLessonTwoServices { get; set; }

        public MasterStandardService MasterStandardServices { get; set; }

        public VirtualQuestionAnswerScoreService VirtualQuestionAnswerScoreServices { get; set; }

        public VirtualQuestionSubService VirtualQuestionSubServices { get; set; }

        public UserService UserService { get; set; }
        public AuthorGroupService AuthorGroupService { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }
        public StudentService StudentService { get; set; }

        public DataLockerTemplateService DataLockerTemplateService { get; set; }
        public VirtualTestCustomScoreService VirtualTestCustomScoreService { get; set; }
        public VirtualTestVirtualTestCustomScoreService VirtualTestVirtualTestCustomScoreService { get; set; }

        public UserBankService UserBankService { get; set; }
        public VirtualTestCustomSubScoreService VirtualTestCustomSubScoreService { get; set; }
        public DataLockerService DataLockerService { get; set; }
        public ClassService ClassService { get; set; }
        public TestResultService TestResultService { get; set; }
        public TestResultScoreService TestResultScoreService { get; set; }
        public DownloadPdfService DownloadPdfService { get; set; }
        public ClassUserService ClassUserService { get; set; }
        public ConfigurationService ConfigurationService { get; set; }
        public VirtualTestCustomMetaDataService VirtualTestCustomMetaDataService { get; set; }
        public ClassStudentService ClassStudentService { get; set; }
        public PreferencesService PreferencesService { get; set; }
        public DataLockerForStudentService DataLockerForStudentService { get; set; }
        public QTITestClassAssignmentService QTITestClassAssignmentService { get; set; }
        public IReportingHttpClient ReportingHttpClient { get; set; }
    }
}
