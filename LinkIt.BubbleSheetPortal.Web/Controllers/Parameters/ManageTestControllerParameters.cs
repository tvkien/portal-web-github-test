using LinkIt.BubbleSheetPortal.InteractiveRubric.Services;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.BusinessObjects;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class ManageTestControllerParameters
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

        public QuestionGroupService VirtualQuestionGroupService { get; set; }

        public UserService UserService { get; set; }
        public AuthorGroupService AuthorGroupService { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }

        public TestRestrictionModuleService TestRestrictionModuleService { get; set; }

        public RestrictionBO RestrictionBO { get; set; }

        public IRubricModuleCommandService RubricModuleCommandService { get; set; }

        public ConfigurationService ConfigurationService { get; set; }

        public QTIOnlineTestSessionService QTIOnlineTestSessionService { get; set; }

        public VirtualTestCustomScoreService VirtualTestCustomScoreService { get; set; }

        public VirtualTestVirtualTestCustomScoreService VirtualTestVirtualTestCustomScoreService { get; set; }
    }
}
