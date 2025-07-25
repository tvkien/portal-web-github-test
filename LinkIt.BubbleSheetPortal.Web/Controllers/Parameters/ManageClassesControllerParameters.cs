using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.ManageParent;
using LinkIt.BubbleSheetPortal.Validators;
using LinkIt.BubbleSheetPortal.Web.ViewModels.Validators;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class ManageClassesControllerParameters
    {
        public DistrictService DistrictService { get; set; }

        public UserService UserService { get; set; }

        public ClassService ClassService { get; set; }

        public GradeService GradeService { get; set; }

        public ClassTypeService ClassTypeService { get; set; }

        public UserSchoolService UserSchoolService { get; set; }

        public SchoolService SchoolService { get; set; }

        public CEESchoolService CEESchoolService { get; set; }

        public StudentService StudentService { get; set; }

        public GenderService GenderService { get; set; }

        public RaceService RaceService { get; set; }

        public StudentProgramService StudentProgramService { get; set; }

        public ProgramService ProgramService { get; set; }

        public UserStudentService UserStudentService { get; set; }

        public ClassStudentDataService ClassStudentDataService { get; set; }

        public SchoolStudentDataService SchoolStudentDataService { get; set; }

        public DistrictTermService DistrictTermService { get; set; }

        public ClassListService ClassListService { get; set; }

        public SchoolTeacherService SchoolTeacherService { get; set; }

        public SchoolTeacherListService SchoolTeacherListService { get; set; }

        public ClassDistrictTermService ClassDistrictTermService { get; set; }

        public ClassTestResultListService ClassTestResultListService { get; set; }

        public BubbleSheetService BubbleSheetService { get; set; }

        public StudentsInClassService StudentsInClassService { get; set; }

        public ClassStudentService ClassStudentService { get; set; }

        public DistrictTermClassService DistrictTermClassService { get; set; }

        public IValidator<School> SchoolValidator { get; set; }

        public SchoolRosterValidator SchoolRosterValidator { get; set; }

        public CEESchoolValidator CEESchoolValidator { get; set; }

        public IValidator<Class> ClassValidator { get; set; }

        public AddClassViewModelValidator AddClassViewModelValidator { get; set; }

        public AddClassRosterViewModelValidator AddClassRosterViewModelValidator { get; set; }

        public AddStudentViewModelValidator AddStudentViewModelValidator { get; set; }

        public AddEditStudentViewModelValidator AddEditStudentViewModelValidator { get; set; }

        public AddOrEditTermViewModelValidator AddOrEditTermViewModelValidator { get; set; }

        public ClassUserService ClassUserService { get; set; }

        public StudentTransferService StudentTransferService { get; set; }

        public IValidator<ClassUser> ClassUserValidator { get; set; }

        public DistrictRosterUploadService DistrictRosterUploadService { get; set; }

        public TestAssignmentService TestAssignmentService { get; set; }

        public QTITestClassAssignmentService QTITestClassAssignmentService { get; set; }

        public StateService StateService { get; set; }

        public TestResultService TestResultService { get; set; }

        public DistrictDecodeService DistrictDecodeService { get; set; }

        public TeacherDistrictTermService TeacherDistrictTermService { get; set; }

        public StudentMetaService StudentMetaService { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }
        public ClassStudentCustomService ClassStudentCustomService { get; set; }
        public ConfigurationService ConfigurationService { get; set; }
        public SGOObjectService SGOObjectService { get; set; }

        public StudentGradeHistoryService StudentGradeHistoryService { get; set; }
        public ManageParentService ManageParentService { get; set; }
        public SchoolMetaService SchoolMetaService { get; set; }
    }
}
