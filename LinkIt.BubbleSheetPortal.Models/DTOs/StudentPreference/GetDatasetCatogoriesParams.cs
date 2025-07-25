using LinkIt.BubbleSheetPortal.Models.DTOs.Base;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.StudentPreference
{
    public class GetDatasetCatogoriesParams: GenericDataTableRequestBase
    {
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int SchoolId { get; set; }
    }
}
