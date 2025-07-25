using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class StudentGradeHistory
    {
        public int StudentGradeHistoryID { get; set; }
        public int StudentID { get; set; }
        public int GradeID { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
    }
}
