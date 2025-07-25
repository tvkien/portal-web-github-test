using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.PerformanceBandAutomations
{
    public class PerformanceBandConfigurationDto
    {
        public int PerformanceBandConfigurationID { get; set; }
        public int StateID { get; set; }
        public int DistrictID { get; set; }
        public string Year { get; set; }
        public string Season { get; set; }
        public int DataSetCategoryID { get; set; }
        public string Subject { get; set; }
        public string Grade { get; set; }
        public string Keyword { get; set; }
        public int ScoreTier { get; set; }
        public string SubscoreName { get; set; }
        public string ScoreName { get; set; }
        public int PerformanceBandGroupID { get; set; }
        public string Cutoffs { get; set; }
        public bool IsPrincipalLevel { get; set; }
        public string CustomYearSeason { get; set; }
    }

    public class PerformanceBandSettingDto
    {
        public int VirtualTestID { get; set; }
        public int DistrictID { get; set; }
        public int PerformanceBandGroupID { get; set; }
        public string Cutoffs { get; set; }
        public string ScoreName { get; set; }
        public bool IsPrincipalLevel { get; set; }
        public int ScoreTier { get; set; }
        public string SubScoreName { get; set; }
    }

    public class PbsVirtualTestDto
    {
        public int VirtualTestID { get; set; }
    }

    public class PerformanceBandConfigurationFilter
    {
        public int DataSetCategoryID { get; set; }
        public string TestName { get; set; }
        public string SubjectName { get; set; }
        public string GradeName { get; set; }
        public int DistrictID { get; set; }
        public int StateID { get; set; }
        public List<string> SubscoreNames { get; set; } = new List<string>();
    }
}
