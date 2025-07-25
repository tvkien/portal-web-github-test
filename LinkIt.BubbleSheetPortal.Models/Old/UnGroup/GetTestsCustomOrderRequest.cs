using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class GetTestsCustomOrderRequest
    {
        public int BankID { get; set; }
        public int? DistrictId { get; set; }
        public string ModuleCode { get; set; }
        public bool IsIncludeRetake { get; set; } = false;
    }
}
