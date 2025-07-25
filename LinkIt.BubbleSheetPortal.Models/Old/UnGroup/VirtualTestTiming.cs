using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualTestTiming
    {
        public int VirtualTestTimingId { get; set; }
        public int VirtualTestId { get; set; }
        public int TimingOptionId { get; set; }
        public int Value { get; set; }
        public int DistrictId { get; set; }
    }
}
