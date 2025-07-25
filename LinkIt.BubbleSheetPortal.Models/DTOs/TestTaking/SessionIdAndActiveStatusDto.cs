using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.TestTaking
{
    public class SessionIdAndActiveStatusDto
    {
        public int QTIOnlineTestSessionId { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? Timestamp { get; set; }
        public int StatusId { get; set; }
    }
}
