using System.Collections.Generic;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ACTReportData : ValidatableEntity<ACTReportData>
    {
        public int TestId { get; set; }
        public int? DistrictId { get; set; }
        private List<string> studentIdList = new List<string>();
        public int TeacherId { get; set; }
        public List<string> StudentIdList
        {
            get { return studentIdList; }
            set { studentIdList = value ?? new List<string>(); }
        }

        public int TimezoneOffset { get; set; }

        public string ActReportFileName { get; set; }
        public int ClassId { get; set; }

        // 1: Scores only; 2: Scores and Essays; 3: Essays only
        public int ReportContentOption { get; set; }

        public int? StateInformationId { get; set; } // Include state information into Knowsys SAT report
        public bool UseNewACTStudentReport { get; set; }
    }
}