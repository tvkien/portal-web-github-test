using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class RemoveTestResultsControllerParameters
    {
        public TestResultService TestResultServices { get; set; }
        public ClassService ClassServices { get; set; }
        public UserService UserServices { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }

        public VirtualTestDistrictService VirtualTestDistrictServices { get; set; }
        public DistrictDecodeService DistrictDecodeServices { get; set; }
        public SubjectService SubjectService { get; set; }
        public ClassDistrictService ClassDistrictServices { get; set; }
        public TestResultLogService TestResultLogServices { get; set; }
    }
}
