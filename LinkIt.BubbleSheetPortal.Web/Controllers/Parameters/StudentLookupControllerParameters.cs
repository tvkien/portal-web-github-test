using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class StudentLookupControllerParameters
    {
        public UserService UserServices { get; set; }
        public RaceService RaceServices { get; set; }
        public StudentService StudentServices { get; set; }
        public UserSchoolService UserSchoolServices { get; set; }
        public GradeService GradeService { get; set; }

        public VulnerabilityService VulnerabilityService { get; set; }
        public ConfigurationService ConfigurationService { get; set; }
        public EmailService EmailService { get; set; }
        public StudentMetaService StudentMetaService { get; set; }
        public ClassService ClassService { get; set; }
        public DownloadPdfService DownloadPdfService { get; set; }
    }
}
