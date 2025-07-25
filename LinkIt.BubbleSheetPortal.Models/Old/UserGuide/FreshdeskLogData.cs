using System;

namespace LinkIt.BubbleSheetPortal.Models.UserGuide
{
    public class FreshdeskLogData
    {
        public int FreshdeskLogID { get; set; }
        public int UserID { get; set; }
        public string Email { get; set; }
        public DateTime LastLoginDate { get; set; }
    }
}
