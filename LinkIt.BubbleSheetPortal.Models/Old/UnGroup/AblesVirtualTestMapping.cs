using System;
using System.Collections.Generic;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AblesVirtualTestMapping
    {
        public int VirtualTestID { get; set; }

        public int ValueMapping { get; set; }

        public string AblesTestName { get; set; }

        public string Round { get; set; }
        public int DistrictID { get; set; }
        public bool IsASD { get; set; }
    }
}