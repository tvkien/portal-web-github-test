using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TLDSDigital
{
    public class TldsWelcomeViewModel
    {
        public TldsFormStatusViewModel FormSection2Status { get; set; }

        public TldsFormStatusViewModel FormSection3Status { get; set; }
    }

    public class TldsFormStatusViewModel
    {
        public string ButtonText { get; set; }

        public bool IsSubmitted { get; set; }
    }
}
