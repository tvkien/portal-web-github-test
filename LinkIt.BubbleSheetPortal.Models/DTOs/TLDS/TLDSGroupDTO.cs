using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.TLDS
{
    public class TLDSGroupDTO
    {
        public int TLDSGroupID { get; set; }

        public int TLDSUserMetaID { get; set; }

        public string GroupName { get; set; }

        public int NumberOfProfile { get; set; }

        public bool Status { get; set; }
    }
}
