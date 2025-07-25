using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.TLDS
{
    public class AssociateToProfileDTO
    {
        public int TLDSGroupID { get; set; }

        public List<int> TLDSProfileIDs { get; set; }
    }
}
