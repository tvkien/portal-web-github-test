using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ResetPasswordLog
    {
        public int ResetPasswordID { get; set; }
        public DateTime RequestDate { get; set; }
        public int? DistrictCode { get; set; }
        public string UserName { get; set; }
        public string IPAddress { get; set; }
    }
}
