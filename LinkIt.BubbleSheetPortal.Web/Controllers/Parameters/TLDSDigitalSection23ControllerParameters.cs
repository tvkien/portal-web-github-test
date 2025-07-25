using LinkIt.BubbleSheetPortal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class TLDSDigitalSection23ControllerParameters
    {
        public TLDSDigitalSection23Service TLDSDigitalSection23Service { get; set; }

	public TLDSService TLDSService { get; set; }

        public DistrictDecodeService DistrictDecodeService { get; set; }

        public GenderService GenderService { get; set; }
    }
}
