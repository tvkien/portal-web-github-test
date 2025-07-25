using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSUserMeta
    {
        public TLDSUserMeta()
        {
            
        }
        public int TLDSUserMetaID { get; set; }
        public int UserID { get; set; }
        public string MetaValue { get; set; }
    }
}
