using LinkIt.BubbleSheetPortal.Web.Models.DataTable;

namespace LinkIt.BubbleSheetPortal.Web.Models.PerformanceBandAutomations
{
    public class PerformanceBandAutomationFilter : GenericDataTableRequest
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
