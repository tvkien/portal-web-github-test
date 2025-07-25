using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ExtractLocalCustom
    {
        public int DistrictId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int GradeId { get; set; }
        public int SubjectId { get; set; }
        public int BankdId { get; set; }
        public int TestId { get; set; }
        public int SchoolId { get; set; }
        public int TeacherId { get; set; }
        public int ClassId { get; set; }

        public int StudentId { get; set; }
        public string StrStartDate { get; set; }
        public string StrEndDate { get; set; }

        public string ListTestIDs { get; set; }

        public int UserId { get; set; }
        public int UserRoleId { get; set; }
        public string SubjectName { get; set; }
    }
}
