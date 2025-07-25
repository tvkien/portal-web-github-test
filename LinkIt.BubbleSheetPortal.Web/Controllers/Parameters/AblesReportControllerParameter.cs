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

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class AblesReportControllerParameter
    {
        public AblesReportService AblesReportService { get; set; }
        public TestService TestService { get; set; }
        public DistrictDecodeService DistrictDecodeService{ get; set; }
        public TeacherDistrictTermService TeacherDistrictTermService{ get; set; }
        public UserSchoolService UserSchoolService{ get; set; }
        public TestResultService TestResultService { get; set; }
        public ClassStudentService ClassStudentService { get; set; }
        public ClassService ClassService { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }
        public DistrictTermClassService DistrictTermClassService { get; set; }
        public DistrictTermService DistrictTermService { get; set; }
        public SchoolService SchoolService { get; set; }
    }
}