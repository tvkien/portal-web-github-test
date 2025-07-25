using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSAdditionalInformation
    {
        public int AdditionalInformationID { get; set; }
        public int ProfileID { get; set; }
        public string AreasOfNote { get; set; }
        public string StrategiesForEnhancedSupport { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
