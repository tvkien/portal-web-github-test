using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSProfileMeta
    {
        public int TLDSProfileMetaID { get; set; }
        public int TLDSProfileID { get; set; }
        public string MetaName { get; set; }
        public string MetaValue { get; set; }
    }
}
