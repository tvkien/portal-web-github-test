using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class LookupStudentCustom
    {
        public int? DistrictId { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public string StateCode { get; set; }
        public int? SchoolId { get; set; }
        public int? GradeId { get; set; }
        public string RaceName { get; set; }
        public int? GenderId { get; set; }
        public int SGOID { get; set; }
        public bool ShowInactiveStudent { get; set; }
        public string SSearch { get; set; }
        public int? ClassId { get; set; }
        public int? TLDSProfileID { get; set; }
        public string ShowAssociatedStudent { get; set; }
        public int TimezoneOffset { get; set; }
        public string StudentDetailPrintingFileName { get; set; }

        public string SelectedUserIds { get; set; }
        public bool SingleTemplate { get; set; }
    }
}
