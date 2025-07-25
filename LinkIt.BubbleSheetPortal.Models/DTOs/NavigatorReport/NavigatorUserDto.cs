using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorUserDto
    {
        public int UserID { get; set; }
        
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string SchoolName { get; set; }
       
        public DateTime? PublishTime { get; set; }
        public int PublishStatus { get; set; }
    }
    public class NavigatorUserEmailDto
    {
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public int UserId { get; set; }
    }
}
