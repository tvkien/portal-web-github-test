using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class APIFunction
    {
        public int APIFunctionId { get; set; }
        public string URI { get; set; }
        public int Status { get; set; }
        public string Version { get; set; }
        public string HTTPAction { get; set; }
    }
}
