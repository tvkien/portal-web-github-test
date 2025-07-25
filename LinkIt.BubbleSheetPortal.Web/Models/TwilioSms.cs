using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Models
{
    public class TwilioSms
    {
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public int SendResult { get; set; }
    }
}