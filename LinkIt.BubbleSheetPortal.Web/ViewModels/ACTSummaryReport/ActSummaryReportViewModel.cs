using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTSummaryReport
{
    public class ActSummaryReportViewModel
    {
        public List<ActSingleTestReportViewModel> ListTestReport { get; set; }
        public ActSingleTestReportViewModel BaselineReport { get; set; }
        public ActSingleTestReportViewModel ImprovementReport { get; set; }
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
        public ActSummaryReportViewModel()
        {
            JS = new List<string>();
            Css = new List<string>();
        }
    }
}
