using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class TestResultTransferControllerParameters
    {
        public TestResultService TestResultServices { get; set; }
        public ClassService ClassServices { get; set; }
        public UserService UserServices { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }
    }
}