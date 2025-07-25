using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs
{
    public class SessionTimeOutDTO
    {
        public int WarningTimeoutMinues { get; set; }
        public int DefaultCookieTimeOutMinutes { get; set; }
        public int KeepAliveDistanceSecond { get; set; }
        public bool ShowTimeOutWarning { get; set; }
    }
}
