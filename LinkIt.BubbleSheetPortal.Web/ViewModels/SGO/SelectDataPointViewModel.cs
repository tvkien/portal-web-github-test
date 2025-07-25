using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class SelectDataPointViewModel
    {
        public int SgoId { get; set; }
        public string SgoDataPointIds { get; set; }
        public int SGOMAXPreAssessment { get; set; }
        public int SGOMAXPostAssessment { get; set; }
        public int PermissionAccess { get; set; }

        public int SgoStatusId { get; set; }
        public int PostAssessmentDataPointId { get; set; }

        public string DirectionConfigurationValue { get; set; }
    }
}