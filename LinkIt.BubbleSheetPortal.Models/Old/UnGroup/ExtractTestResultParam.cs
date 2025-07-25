using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ExtractTestResultParam
    {
        public int ExtractTestResultParamID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? GradeID { get; set; }
        public int? SubjectID { get; set; }
        public int? BankID { get; set; }
        public int? SchoolID { get; set; }
        public int? TeacherID { get; set; }
        public int? ClassID { get; set; }
        public int? StudentID { get; set; }
        public string ListTestIDs { get; set; }    
        public string ListIdsUncheck { get; set; }
        public int UserID { get; set; }
        public int UserRoleID { get; set; }
        public string User_ListSchoolId { get; set; }
        public string SubjectName { get; set; }
        public string GeneralSearch { get; set; }
    }
}

