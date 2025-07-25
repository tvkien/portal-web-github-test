using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    [Serializable]
    public class DateFormatModel
    {
        public int DistrictId { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public string JQueryDateFormat { get; set; }
        public string HandsonTableDateFormat { get; set; }
    }
}
