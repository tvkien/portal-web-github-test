namespace LinkIt.BubbleSheetPortal.Models
{
    public class GetModuleAccessPaginationRequest : PaggingInfo
    {
        public int? XLIGroupID { get; set; }
        public int? DistrictID { get; set; }
        public int? XLIAreaID { get; set; }
        public int? XLIModuleID { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }

        public string SearchString { get; set; }
    }
}
