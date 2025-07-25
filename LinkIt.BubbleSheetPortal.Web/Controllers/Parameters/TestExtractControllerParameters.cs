using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class TestExtractControllerParameters
    {
        public ExtractTestResultService ExtractTestResultServices { get; set; }
        public UserSchoolService UserSchoolServices { get; set; }
        public TestResultService TestResultServices { get; set; }
        public S3PortalLinkService S3PortalLinkServices { get; set; }
        public S3PermissionService S3PermissionServices { get; set; }
        public TestExtractTemplateService TestExtractTemplateServices { get; set; }
        public DistrictDecodeService DistrictDecodeService { get; set; }
        public ExtractLocalTestResultsQueueService ExtractQueueService { get; set; }
        public VirtualTestTimingOptionService VirtualTestTimingOptionServices { get; set; }

    }
}