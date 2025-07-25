using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.StudentLookup
{
    public class StudentSessionInfoDto
    {
        public DateTime TimeStamp { get; set; }
        public string StudentWANIP { get; set; }
        public string PointOfEntry { get; set; }
        public string TestCode { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public int Type { get; set; }
    }
}
