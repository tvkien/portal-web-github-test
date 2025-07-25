using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent
{
    public class ParentGridViewModel
    {

        public int UserId { get; set; }
        public string ParentFullName { get; set; }
        public IEnumerable<ParentGridSchoolNameViewModel> SchoolNames { get; set; }
        public string RegistrationCode { get; set; }
        public bool Active { get; set; }
        public DateTime? EmailLastSent { get; set; }
        public string Email { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
    public class ParentGridSchoolNameViewModel
    {
        public string SchoolName { get; set; }
        public string[] StudentNames { get; set; }
    }
}
