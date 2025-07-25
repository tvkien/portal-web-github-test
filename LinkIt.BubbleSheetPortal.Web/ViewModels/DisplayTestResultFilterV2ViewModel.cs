using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class DisplayTestResultFilterV2ViewModel
    {
        public int DistrictId { get; set; }
        public string SchoolIds { get; set; }
        public string CategoryIds { get; set; }
        public string GradeIds { get; set; }
        public string SubjectNames { get; set; }
        public int TermId { get; set; }
        public int ClassId { get; set; }
        public string TeacherName { get; set; }
        public string StudentName { get; set; }
        public string FromResultDate { get; set; }
        public string ToResultDate { get; set; }
        public string FromCreatedDate { get; set; }
        public string ToCreatedDate { get; set; }
        public string FromUpdatedDate { get; set; }
        public string ToUpdatedDate { get; set; }
        public string VirtualTestName { get; set; }
        public bool DisplayByTestResults { get; set; }
    }
}
