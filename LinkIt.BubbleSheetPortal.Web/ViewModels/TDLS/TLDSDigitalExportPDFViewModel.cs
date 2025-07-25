using LinkIt.BubbleSheetPortal.Models.TLDS;
using LinkIt.BubbleSheetPortal.Web.ViewModels.TLDSDigital;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TLDSDigitalExportPDFViewModel
    {
        public Guid TLDSProfileLinkID { get; set; }
        public TldsFormSection2ViewModel TLDSFormSection2 { get; set; }
        public TldsFormSection3ViewModel TLDSFormSection3 { get; set; }
    }
}
