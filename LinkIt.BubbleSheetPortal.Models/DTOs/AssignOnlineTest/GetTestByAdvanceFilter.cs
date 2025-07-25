using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class GetTestByAdvanceFilter
    {
        public List<GetTestResult> Data { get; set; }
        public int TotalRecords { get; set; }
    }

    public class GetTestResult
    {
        public int VirtualTestID { get; set; }
        public string VirtualTestName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int BankID { get; set; }
        public string BankName { get; set; }
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
        public int GradeID { get; set; }
        public string GradeName { get; set; }
    }
}
