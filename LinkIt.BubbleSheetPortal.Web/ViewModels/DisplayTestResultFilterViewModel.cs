using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class DisplayTestResultFilterViewModel
    {
        public int DistrictId { get; set; }
        public int VirtualTestId { get; set; }
        public int ClassId { get; set; }
        public string StudentName { get; set; }
        public int SchoolId { get; set; }
        public string TeacherName { get; set; }
        public int TermrId { get; set; }
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public int TimePeriod { get; set; }
    }
}