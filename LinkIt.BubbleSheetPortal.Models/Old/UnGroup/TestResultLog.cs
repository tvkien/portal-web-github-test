using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestResultLog
    {        
        public int TestResultLogID { get; set; }
        public int TestResultID { get; set; }
        public int VirtualTestID { get; set; }
        public int StudentID { get; set; }
        public int? TeacherID { get; set; }
        public int? SchoolID { get; set; }
        public DateTime ResultDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? TermID { get; set; }
        public int? ClassID { get; set; }
        public int GradedByID { get; set; }
        public int ScoreType { get; set; }
        public decimal? ScoreValue { get; set; }
        public int? SubmitType { get; set; }
        public int? DistrictTermID { get; set; }
        public int? UserID { get; set; }
        public int? OriginalUserID { get; set; }
        public int? LegacyBatchID { get; set; }
        public int? BubbleSheetID { get; set; }
        public int? QTIOnlineTestSessionID { get; set; }
        public int DistrictID { get; set; }

        public string TRData { get; set; }
        public string UIN { get; set; }
        public string DistrictTermName { get; set; }
        public string DistrictName { get; set; }
        public string SchoolName { get; set; }
        public string UserName { get; set; }
        public string ClassName { get; set; }
        public string StudentFirst { get; set; }
        public string StudentLast { get; set; }
        public string TestName { get; set; }
    }
}
