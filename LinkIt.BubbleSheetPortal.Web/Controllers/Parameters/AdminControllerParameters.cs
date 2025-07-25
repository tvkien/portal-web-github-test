using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Models.Requests;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.DistrictReferenceData;
using LinkIt.BubbleSheetPortal.Web.Helpers.DistrictReferenceData;
using LinkIt.BubbleSheetPortal.Web.Helpers.ETL;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Services.TestResultRemover;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Services;
using LinkIt.BubbleSheetPortal.Models.Old.XpsQueue;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class AdminControllerParameters
    {
        public IValidator<Request> RequestValidator { get; set; }
        public IValidator<XpsQueue> XpsQueueValidator { get; set; }

        public RequestService RequestService { get; set; }

        public RosterRequestService RosterRequestService { get; set; }

        public IRosterUploadService RosterUploadService { get; set; }

        public RequestParameterService RequestParameterService { get; set; }

        public IValidator<RequestParameter> RequestParameterValidator { get; set; }

        public UserSchoolService UserSchoolService { get; set; }

        public UserService UserService { get; set; }

        public UserStudentService UserStudentService { get; set; }

        public GenderService GenderService { get; set; }

        public GradeService GradeService { get; set; }

        public StateService StateService { get; set; }

        public DistrictService DistrictService { get; set; }

        public SchoolService SchoolService { get; set; }

        public IValidator<CreateUserViewModel> CreateUserViewModelValidator { get; set; }

        public IValidator<CreateParentViewModel> CreateParentViewModelValidator { get; set; }

        public IValidator<EditUserViewModel> EditUserViewModelValidator { get; set; }

        public IValidator<EditParentViewModel> EditParentViewModelValidator { get; set; }

        public IValidator<UserSchool> UserSchoolValidator { get; set; }

        public IValidator<School> SchoolValidator { get; set; }

        public MappingInformationService MappingInformationService { get; set; }

        public StandardColumnService StandardColumnService { get; set; }

        public MappingValidatingHelper MappingValidatingHelper { get; set; }

        public MappingTransferHelper MappingTransferHelper { get; set; }

        public MappingRuleHelper MappingRuleHelper { get; set; }

        public WebUtilityHelper WebUtilityHelper { get; set; }

        public ProgramService ProgramService { get; set; }

        public RaceService RaceService { get; set; }

        public GradeDistrictService GradeDistrictService { get; set; }

        public GenderStudentService GenderStudentService { get; set; }

        public SubjectDistrictService SubjectDistrictService { get; set; }

        public ClusterDistrictService ClusterDistrictService { get; set; }

        public AchievementLevelSettingService AchievementLevelSettingService { get; set; }

        public DistrictTermService DistrictTermService { get; set; }

        public ClassService ClassService { get; set; }

        public StudentService StudentService { get; set; }

        public ClassDistrictService ClassDistrictServices { get; set; }

        public VirtualTestDistrictService VirtualTestDistrictServices { get; set; }

        public SchoolTestResultDistrictService SchoolTestResultDistrictServices { get; set; }

        public ClassTestResultDistrictService ClassTestResultDistrictServices { get; set; }

        public TeacherTestResultDistrictService TeacherTestResultDistrictServices { get; set; }

        public TeacherDistrictTermService TeacherDistrictTermService { get; set; }

        public StudentTestResultDistrictService StudentTestResultDistrictServices { get; set; }

        public VirtualTestTestResultDistrictService VirtualTestTestResultDistrictServices { get; set; }

        public DisplayTestResultDistrictService DisplayTestResultDistrictServices { get; set; }

        public IFormsAuthenticationService FormsAuthenticationService { get; set; }

        public AnswerService AnswerServices { get; set; }

        public VirtualTestWithOutTestResultService VirtualTestWithOutTestResultServices { get; set; }

        public AuthorTestWithoutTestResultService AuthorTestWithoutTestResultServices { get; set; }

        public CustomAuthorTestService CustomAuthorTestServices { get; set; }

        public DistrictStateService DistrictStateServices { get; set; }

        public ClassUserService ClassUserService { get; set; }

        public ClassAdminSchoolService ClassAdminSchoolServices { get; set; }

        public StudentParentService StudentParentService { get; set; }
        public DSPDistrictService DspDistrictService { get; set; }
        public TestResultLogService TestResultLogServices { get; set; }
        public ImpersonateLogService ImpersonateLogService { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }

        public DistrictDecodeService DistrictDecodeService { get; set; }
        public ConfigurationService ConfigurationService { get; set; }
        public DistrictRosterOptionService DistrictRosterOptionService { get; set; }
        public RosterTypeService RosterTypeService { get; set; }

        public UserMetaService UserMetaService { get; set; }

        public XLIGroupService XLIGroupService { get; set; }

        public IRubricModuleCommandService RubricModuleCommandService { get; set; }
        public AutoFocusGroupConfigService AutoFocusGroupConfigService { get; set; }

        public XpsDistrictUploadService XpsDistrictUploadService { get; set; }
        public XpsQueueService XpsQueueService { get; set; }
    }
}
