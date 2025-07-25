namespace LinkIt.BubbleSheetPortal.Models.DTOs.PerformanceBandAutomations
{
    public class TestForPBSFilter: PaggingInfo
    {
        public int DistrictID { get; set; }
        public string VirtualTestTypeIds { get; set; }
        public string GradeIDs { get; set; }
        public string SubjectNames { get; set; }
        public string GeneralSearch { get; set; }
        public string PBSInEffect { get; set; }
        public string PBSGroup { get; set; }
    }
}
