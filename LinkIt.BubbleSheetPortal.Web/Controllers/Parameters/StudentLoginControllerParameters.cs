using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class StudentLoginControllerParameters
    {
        public SchoolStudentRegistrationService SchoolStudentRegistrationServices { get; set; }
        public ProgramService ProgramServices { get; set; }
        public DistrictService DistrictServices { get; set; }
        public StateService StateServices { get; set; }
        public StudentService StudentServices { get; set; }
        public UserService UserServices { get; set; }
        public RoleService RoleServices { get; set; }
        public StudentMetaService StudentMetaServices { get; set; }
        public DistrictDecodeService DistrictDecodeServices { get; set; }
        public ClassStudentDataService ClassStudentDataServices { get; set; }
        public StudentProgramService StudentProgramServices { get; set; }
        public StudentParentService StudentParentServices { get; set; }
        public ClassListService ClassListServices { get; set; }
        public SchoolService SchoolServices { get; set; }
        public GradeService GradeServices { get; set; }
        public EmailService EmailServices { get; set; }
        public IFormsAuthenticationService FormsAuthenticationService { get; set; }
        public ChytenReportService ChytenReportServices { get; set; }
        public ConfigurationService ConfigurationService { get; set; }
        public UserLogonService UserLogonService { get; set; }
    }
}