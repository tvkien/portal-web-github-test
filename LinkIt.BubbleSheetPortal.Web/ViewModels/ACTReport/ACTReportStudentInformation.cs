using System;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport
{
    public class ACTReportStudentInformation
    {
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string TestResultClassName { get; set; }
        public string DistrictTermName { get; set; }
        public string TestResultTeacherName { get; set; }

        public string ReportClassName { get; set; }
        public string ReportTeacherName { get; set; }

        public string TestName { get; set; }
        public DateTime TestDate { get; set; }
    }
}