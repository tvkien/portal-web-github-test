using System;

namespace LinkIt.BubbleSheetPortal.Models.ACTReport
{
    public class ACTStudentInformation
    {
        public string StudentCode { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string ClassName { get; set; }
        public string DistrictTermName { get; set; }
        public string TeacherName { get; set; }
        public string TestName { get; set; }
        public DateTime TestDate { get; set; }
    }
}