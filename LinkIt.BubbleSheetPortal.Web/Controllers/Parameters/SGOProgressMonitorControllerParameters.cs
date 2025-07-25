using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class SGOProgressMonitorControllerParameters
    {
        public DistrictDecodeService DistrictDecodeService { get; set; }
        public SGOObjectService SgoObjectService { get; set; }
        public SGOGroupService SgoGroupService { get; set; }
        public SGOAttainmentGroupService SgoAttainmentGroupService { get; set; }
        public SGODataPointService SgoDataPointService { get; set; }
        public VirtualQuestionService VirtualQuestionService { get; set; }
        public SGOStudentService SgoStudentService { get; set; }
        public SGOAttainmentGoalService SGOAttainmentGoalService { get; set; }

        public SGOSelectDataPointService SgoSelectDataPointService { get; set; }

        public SGOMilestoneService SGOMilestoneService { get; set; }
        public UserService UserService { get; set; }
        public SGOObjectService SGOObjectService { get; set; }
        public SGOAuditTrailService SGOAuditTrailService { get; set; }

        public VirtualTestCustomScoreService VirtualTestCustomScoreService { get; set; }
        public SGODataPointClusterScoreService SgoDataPointClusterScoreService { get; set; }
        public DataLockerService DataLockerService { get; set; }

    }
}
