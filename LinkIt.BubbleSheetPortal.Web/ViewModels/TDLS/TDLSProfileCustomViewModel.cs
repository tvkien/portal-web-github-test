using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TDLSProfileCustomViewModel
    {
        public int ProfileId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string ECSCompletingFormEducatorName { get; set; }

        public int? Status { get; set; }
        public DateTime? LastStatusDate { get; set; }
        public string School { get; set; }
        public bool Viewable { get; set; }
        public bool Updateable { get; set; }
        public string StatusName { get; set; }
        public bool OnlyView { get; set; }
        public int? EnrolmentYear { get; set; }
    }
}
