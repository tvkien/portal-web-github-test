using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.AssignSurvey
{
    public class FilterUserResultsViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string SchoolName { get; set; }
        public int RoleId { get; set; }
    }
}
