using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class SGOReportControllerParameters
    {
        public SGOObjectService SGOObjectService { get; set; }
        public DistrictService DistrictService { get; set; }
        public SGODataPointService SgoDataPointService { get; set; }
        public SGODataPointClusterScoreService SgoDataPointClusterScoreService { get; set; }
        public SGOAuditTrailService SgoAuditTrailService { get; set; }
        public SGOGroupService SgoGroupService { get; set; }
        public SGOStudentService SgoStudentService { get; set; }
        public SGOAttainmentGoalService SgoAttainmentGoalService { get; set; }
        public SGOAttainmentGroupService SgoAttainmentGroupService { get; set; }
        public DistrictDecodeService DistrictDecodeService { get; set; }
        public SGOSelectDataPointService SgoSelectDataPointService { get; set; }
    }
}
