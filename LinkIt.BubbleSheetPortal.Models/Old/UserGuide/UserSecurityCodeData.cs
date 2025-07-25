using System;

namespace LinkIt.BubbleSheetPortal.Models.UserGuide
{
    public class UserSecurityCodeData
    {
        public int UserSecurityCodeID { get; set; }
        public int UserID { get; set; }
        public DateTime IssueDate { get; set; }
        public bool Expired { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
    }
}
