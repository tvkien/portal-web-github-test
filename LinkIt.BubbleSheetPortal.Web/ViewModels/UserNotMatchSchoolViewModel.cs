namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class UserNotMatchSchoolViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public int SchoolId { get; set; }
    }

    public class UserNotMatchSchoolDataViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int SchoolId { get; set; }
    }
}
