using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class LookupStudentViewModel
    {
        public int StudentId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }        
        public string Code { get; set; }
        public string StateCode { get; set; }
        public string SchoolName { get; set; }
        public string GradeName { get; set; }
        public string RaceName { get; set; }
        public string GenderCode { get; set; }
        public int? Status { get; set; }
        public bool CanAccess { get; set; }
        public string RegistrationCode { get; set; }
        public string UserName { get; set; }
        public bool HasPassword { get; set; }
    }
}
