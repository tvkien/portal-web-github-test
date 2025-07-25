namespace LinkIt.BubbleSheetPortal.Web.Models.DataTable
{
    public class GetModuleAccessRequest : GenericDataTableRequest
    {
        public int? XLIGroupID { get; set; }
        public int? DistrictID { get; set; }
        public int? XLIAreaID { get; set; }
        public int? XLIModuleID { get; set; }
    }

    public class GetSchoolAccessRequest : GenericDataTableRequest
    {
        public int XLIModuleID { get; set; }

        public int DistrictID { get; set; }
    }
}
