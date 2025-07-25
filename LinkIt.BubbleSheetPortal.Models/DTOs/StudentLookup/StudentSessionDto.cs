using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.StudentLookup
{
    public class StudentSessionDto
    {
        public DateTime TimeStampDate { get; set; }
        public string TimeStamp { get; set; }
        public string PointOfEntryDisplay { get; set; }
        public string BrowserNameDisplay { get; set; }
        public string StudentWANIP { get; set; }
        public string PointOfEntry { get; set; }
        public string TestCode { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public int Type { get; set; }
    }
}
