using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting
{
    public class StudentOnlineTestingViewDto
    {
        public DateTime? AssignmentDate { get; set; }
        public string TestName { get; set; }
        public string TeacherName { get; set; }
        public string ClassName { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public string RedirectUrl { get; set; }
        public int? AssignmentModifiedUserID { get; set; }
        public string AssignmentFirstName { get; set; }
        public string AssignmentLastName { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMsg { get; set; }
        public bool IsTutorialMode { get; set; }
        public string AssignmentGUID { get; set; }
        public int QTITestClassAssignmentID { get; set; }
        public DateTime? StartDate { get; set; }
        public int? QTIOnlineTestSessionID { get; set; }
        public DateTime? DistrictTermDateStart { get; set; }
        public DateTime? DistrictTermDateEnd { get; set; }
    }
}
