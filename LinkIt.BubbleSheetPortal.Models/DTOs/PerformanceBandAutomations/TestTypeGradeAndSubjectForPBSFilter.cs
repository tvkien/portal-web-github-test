using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.PerformanceBandAutomations
{
    public class TestTypeGradeAndSubjectForPBSFilter
    {
        public int DistrictId { get; set; }
        public DataSetOriginEnum? DataSetOriginID { get; set; }
        public string SchoolIds { get; set; }
    }
}
