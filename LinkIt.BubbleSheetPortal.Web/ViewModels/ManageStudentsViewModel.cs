using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ManageStudentsViewModel
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public string School { get; set; }
        public int? GradeOrder { get; set; }
        public string FirstClassName { get; set; }
        public string FirstParentName { get; set; }

        public string ClassesName { get; set; }
        public string ParentsName { get; set; }
        public string GradeName { get; set; }
    }
}