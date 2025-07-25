namespace LinkIt.BubbleSheetPortal.Models.DTOs.DistrictDecode
{
    public class DistrictDataParmDTO
    {
        public int DistrictDataParmID { get; set; }
        public int DistrictID { get; set; }
        public int DataSetOriginID { get; set; }
        public int DataSetCategoryID { get; set; }
        public string JSONDataConfig { get; set; }
        public string ImportType { get; set; }
    }
}
