using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.CodeGen;
using LinkIt.BubbleSheetPortal.Services.CommonServices;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class PassThroughControllerParameters
    {
        public UserService UserServices { get; set; }
        public IFormsAuthenticationService AuthenticationServices { get; set; }
        public DistrictService DistrictServices { get; set; }
        public APIAccountService APIAccountServices { get; set; }
        public APIPermissionService APIPermissionServices { get; set; }
        public APIFunctionService APIFunctionServices { get; set; }
        public UserSchoolService UserSchoolServices { get; set; }
        public UserLogonService UserLogonService { get; set; }
        public StudentService StudentService { get; set; }
        public StudentMetaService StudentMetaService { get; set; }
        public UserDistrictSectorService UserDistrictSectorService { get; set; }
        public ConfigurationService ConfigurationService { get; set; }
        public SchoolService SchoolService { get; set; }
        public DistrictTermService DistrictTermService { get; set; }
        public ClassService ClassService { get; set; }
        public QTITestClassAssignmentService QTITestClassAssignmentService { get; set; }
        public QTITestStudentAssignmentService QTITestStudentAssignmentService { get; set; }
        public ClassStudentService ClassStudentService { get; set; }
        public VirtualTestService VirtualTestService { get; set; }
        public TestCodeGenerator TestCodeGenerator { get; set; }
        public PreferencesService PreferencesService { get; set; }

        public IShortLinkService ShortLinkService { get; set; }
        public DistrictDecodeService DistrictDecodeService { get; set; }

    }
}
