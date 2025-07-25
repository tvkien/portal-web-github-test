using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ExtractTestResultCustom
    {
        public int? districtId { get; set; }
        public string GradeName { get; set; }
        public string SubjectName { get; set; }
        public string BankName { get; set; }
        public string TestName { get; set; }
        public string SchoolName { get; set; }
        public string TeacherName { get; set; }
        public string ClassName { get; set; }
        public string StudentName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int RoleId { get; set; }
        public List<int> ListClasses { get; set; }
        public int CurrentUserId { get; set; }
    }
}
