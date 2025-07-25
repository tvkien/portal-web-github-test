namespace LinkIt.BubbleSheetPortal.Models
{
    public class GetGroupUserRequest : PaggingInfo
    {
        public int? DistrictID { get; set; }
        public int? UserID { get; set; }
        public int? RoleID { get; set; }
        public bool IsShowInactiveUser { get; set; }
        public int? XLIGroupID { get; set; }

        public string GeneralSearch { get; set; }
    }
}