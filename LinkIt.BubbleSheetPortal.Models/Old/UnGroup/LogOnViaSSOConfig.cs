using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class LogOnViaSSOConfig
    {
        public LogOnViaSSOConfig()
        {
            DistrictID = 0;
            LogOnViaSSO = false;
        }
        public int DistrictID { get; set; }
        public bool LogOnViaSSO { get; set; }
        public string SSOLogonURL { get; set; }
    }
}
