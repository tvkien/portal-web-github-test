using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTSummaryReport
{
    public class ActSummaryReportData
    {
        public int TestId { get; set; }
        public int? DistrictId { get; set; }
        public int? SchoolId { get; set; }        
        public int? TeacherId { get; set; }
        public int? ClassId { get; set; }
        public int? DistrictTermId { get; set; }

        public string StrStudentIdList { get; set; }
        public string StrTestIdList { get; set; }

        private List<string> studentIdList = new List<string>();
        public List<string> StudentIdList
        {
            get { return studentIdList; }
            set { studentIdList = value ?? new List<string>(); }
        }

        public string ImprovementOption { get; set; }

        //private List<int> testIdList = new List<int>();
        //public List<int> TestIdList
        //{
        //    get { return testIdList; }
        //    set { testIdList = value ?? new List<int>(); }
        //}
        
        public int TimezoneOffset { get; set; }
        public string ReportFileName { get; set; }

        public int VirtualTestSubTypeId { get; set; }
        public int CurrentUserDistrict { get; set; }
    }
}
