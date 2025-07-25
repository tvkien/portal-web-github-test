using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class SchoolStudentData
    {
        public int SchoolStudentID { get; set; }
        public int SchoolID { get; set; }
        public int StudentID { get; set; }
        public bool Active { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }          
    }
}