using System.Collections.Generic;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class AddTeacherToClassViewModel
    {
        public AddTeacherToClassViewModel()
        {
            Teachers = new List<SelectListItem>();
            LOETypes = new List<SelectListItem>();
        }

        public int ClassId { get; set; }

        public int SelectedTeacher { get; set; }
        public int SelectedLOE { get; set; }

        public IEnumerable<SelectListItem> Teachers { get; set; }
        public IEnumerable<SelectListItem> LOETypes { get; set; }
    }
}