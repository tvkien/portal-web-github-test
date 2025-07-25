namespace LinkIt.BubbleSheetPortal.Models
{
    public class GetUserForAddGroupRequest : PaggingInfo
    {
        public int? DistrictID { get; set; }
        public int? UserID { get; set; }
        public int? UserRoleID { get; set; }
        public bool IsShowInactiveUser { get; set; }
        public int XLIGroupID { get; set; }

        public int? SchoolID { get; set; }
        public int? RoleID { get; set; }

        public string GeneralSearch { get; set; }
    }
}