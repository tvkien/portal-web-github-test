using System.Collections.Generic;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ReplacePrimaryTeacherViewModel
    {
        public ReplacePrimaryTeacherViewModel()
        {
            Teachers = new List<SelectListItem>();
        }

        public int ClassId { get; set; }
        public int SelectedTeacher { get; set; }

        public IEnumerable<SelectListItem> Teachers { get; set; }
        
    }
}