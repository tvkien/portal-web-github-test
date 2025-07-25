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
    public class MailBoxControllerParameters
    {
        public ParentConnectService ParentConnectService { get; set; }
        public DistrictService DistrictService { get; set; }
        public UserSchoolService UserSchoolService { get; set; }
        public SchoolService SchoolService { get; set; }
        public ClassUserService ClassUserService { get; set; }
        public ClassService ClassService { get; set; }
        public StudentsInClassService StudentsInClassService { get; set; }
        public ClassListService ClassListService { get; set; }
        public RoleService RoleService { get; set; }
        public UserService UserService { get; set; }
        public DistrictConfigurationService DistrictConfigurationService { get; set; }
        public ClassStudentService ClassStudentService { get; set; }

        public IValidator<EditParentViewModel> EditParentViewModelValidator { get; set; }
    }
}