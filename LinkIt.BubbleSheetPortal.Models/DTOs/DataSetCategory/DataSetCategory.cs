using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs
{
    public class DataSetCategory
    {
        public int DataSetCategoryID { get; set; }
        public string DataSetCategoryName { get; set; }
        public int? DistrictID { get; set; }
        //public int? StateID { get; set; }
    }
    public class DataSetCategoryDistrictDisplayNameDto
    {
        public int DataSetCategoryID { get; set; }
        public string CategoryDisplayName { get; set; }
        public int? DistrictId { get; set; }
    }
}
