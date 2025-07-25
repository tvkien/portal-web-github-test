using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting
{
    public class StudentAssignmentDto
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public int QTITestClassAssignmentID { get; set; }
        public int ClassId { get; set; }

        public string Type { get; set; }
        public string StudentIds { get; set; }
        public DateTime AssignmentDate { get; set; }
        public DateTime? ResultDate { get; set; }
        public string FullName { get; set; }
    }

    public class StudentAssginmentGroupDto
    {
        public StudentAssginmentGroupDto()
        {
            OnlineTest = new List<StudentAssignmentDto>();
            BubbleSheet = new List<StudentAssignmentDto>();
        }

        public List<StudentAssignmentDto> OnlineTest { get; set; }
        public List<StudentAssignmentDto> BubbleSheet { get; set; }
    }
}
