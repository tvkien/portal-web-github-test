using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SATSummaryReport
{
    public class SATSummaryReportViewModel
    {
        public List<SATSingleTestReportViewModel> ListTestReport { get; set; }
        public SATSingleTestReportViewModel BaselineReport { get; set; }
        public SATSingleTestReportViewModel ImprovementReport { get; set; }
        public string DistrictName { get; set; }
        public string TestNames { get; set; }
        public string SchoolName { get; set; }
        public string ClassName { get; set; }
        public string TeacherName { get; set; }
        public string DistrictTermName { get; set; }
        public string DistrictLogoUrl { get; set; }
        public int VirtualTestSubTypeId { get; set; }

        public List<string> JS { get; set; }
        public List<string> Css { get; set; }
        public SATSummaryReportViewModel()
        {
            JS = new List<string>();
            Css = new List<string>();
        }
    }
}
