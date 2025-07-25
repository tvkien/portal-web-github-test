using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.DistrictDecode
{
    public class DistrictDataParmJsonDto
    {
        public List<string> GnsAttendanceFileNameKeyWords { get; set; }
        public List<string> GnsTermFileNameKeyWords { get; set; }
        public List<string> IncludedTerms { get; set; }

        public int VirtualTestCustomScoreID { get; set; }
        public int DistrictPerformanceBandVirtualTestScoreSettingID { get; set; }
        public List<SchoolPerformanceBandVirtualTestScoreSettingIdDTO> SchoolPerformanceBandVirtualTestScoreSetting { get; set; }
        public DistrictDataParmJsonDto()
        {
            SchoolPerformanceBandVirtualTestScoreSetting = new List<SchoolPerformanceBandVirtualTestScoreSettingIdDTO>();
        }
    }

    public class SchoolPerformanceBandVirtualTestScoreSettingIdDTO
    {
        public int SchoolId { get; set; }
        public int PerformanceBandVirtualTestScoreSettingID { get; set; }
    }
}
