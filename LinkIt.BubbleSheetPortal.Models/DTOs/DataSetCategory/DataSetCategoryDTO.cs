namespace LinkIt.BubbleSheetPortal.Models.DTOs
{
    public class DataSetCategoryDTO
    {
        public int DataSetCategoryID { get; set; }
        public string DataSetCategoryName { get; set; }
        public int? DistrictID { get; set; }
        public string DistrictName { get; set; }
        public string DisplayName { get; set; }
        public string CombinedDisplayText => $"{DataSetCategoryName}{(string.IsNullOrEmpty(DisplayName) ? "" : $" ({DisplayName})")}";
    }
}
