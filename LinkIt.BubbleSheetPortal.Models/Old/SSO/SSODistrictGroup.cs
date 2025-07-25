using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.SSO
{
    public class SSODistrictGroup
    {
        public int SSODistrictGroupID { get; set; }
        public int SSOInformationID { get; set; }
        public int DistrictID { get; set; }
        public virtual SSOInformation SSOInformation { get; set; }
        public virtual District District { get; set; }
    }
}
