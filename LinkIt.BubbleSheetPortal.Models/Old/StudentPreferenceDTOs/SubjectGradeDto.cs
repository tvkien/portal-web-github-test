using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.StudentPreferenceDTOs
{
    public class SubjectGradeDto
    {
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
        public int GradeID { get; set; }
        public string GradeName { get; set; }
        public int GradeOrder { get; set; }
    }
}
