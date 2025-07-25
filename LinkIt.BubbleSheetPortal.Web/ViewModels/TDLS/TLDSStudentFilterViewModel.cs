using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TLDSStudentFilterViewModel
    {
        public int StudentId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Code { get; set; }
        public string SchoolName { get; set; }
        public string GradeName { get; set; }
        public string GenderCode { get; set; }
        public int? ProfileID { get; set; }
    }
}