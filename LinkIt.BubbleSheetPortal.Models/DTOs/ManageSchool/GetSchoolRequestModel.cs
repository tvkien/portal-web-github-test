namespace LinkIt.BubbleSheetPortal.Models
{
    public class GetSchoolRequestModel
    {
        public int DistrictId { get; set; }
        public int SchoolId { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public string SortColumns { get; set; }
        public string GeneralSearch { get; set; }
    }
}
