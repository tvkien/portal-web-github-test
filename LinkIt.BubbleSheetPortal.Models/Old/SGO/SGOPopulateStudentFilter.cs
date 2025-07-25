using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOPopulateStudentFilter
    {
        public int SGOId { get; set; }
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public int UserRoleId { get; set; }
        public string GenderIds { get; set; }
        public string RaceIds { get; set; }
        public string ProgramIds { get; set; }
        public string TermIds { get; set; }
        public string ClassIds { get; set; }
        public bool? IncludeAddedStudents { get; set; }
    }
}
