using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class PopulateReportingControllerParameters
    {
        public TeacherDistrictTermService TeacherDistrictTermService { get; set; }
        public UserSchoolService UserSchoolService { get; set; }
        public GradeService GradeService { get; set; }
        public SubjectService SubjectService { get; set; }
        public UserBankService UserBankService { get; set; }
        public PopulateReportingService PopulateReportingService { get; set; }
    }
}