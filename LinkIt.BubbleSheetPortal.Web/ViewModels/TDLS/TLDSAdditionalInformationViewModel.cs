using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TLDSAdditionalInformationViewModel
    {
        public int? AdditionalInformationId { get; set; }
        public int? ProfileId { get; set; }
        public string AreasOfNote { get; set; }
        public string StrategiesForEnhancedSupport { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}