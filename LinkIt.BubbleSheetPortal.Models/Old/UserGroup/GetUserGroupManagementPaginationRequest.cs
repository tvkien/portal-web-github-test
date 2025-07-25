namespace LinkIt.BubbleSheetPortal.Models
{
    public class GetUserGroupManagementPaginationRequest : PaggingInfo
    {
        public int? DistrictID { get; set; }

        public int? UserID { get; set; }

        public int? RoleID { get; set; }

        public string GeneralSearch { get; set; }
    }
}
