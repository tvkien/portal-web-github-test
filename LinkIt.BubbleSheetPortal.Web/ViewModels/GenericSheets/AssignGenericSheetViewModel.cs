using System.Collections.Generic;
using System.Web.Mvc;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.GenericSheets
{
    public class AssignGenericSheetViewModel : ValidatableEntity<AssignGenericSheetViewModel>
    {
        public int BubbleSheetFileId { get; set; }
        public string FileName { get; set; }
        public string ImageUrl { get; set; }
        public int SelectedRemainingStudentsId { get; set; }
        public int SelectedAllStudentsId { get; set; }
        public bool IsAllStudentsChecked { get; set; }

        public int BubbleSheetId { get; set; }
        public int? ClassId { get; set; }
        public bool IsStudentDetected { get; set; }

        public int VirtualTestSubTypeId { get; set; }

        public bool OnlyOnePage { get; set; }
        
        public IEnumerable<SelectListItem> RemainingStudents { get; set; }
        public IEnumerable<SelectListItem> AllStudents { get; set; }

        public IEnumerable<SelectListItem> Classes { get; set; }

        public string Ticket { get; set; }

        public AssignGenericSheetViewModel()
        {
            RemainingStudents = new List<SelectListItem>();
            AllStudents = new List<SelectListItem>();
        }
    }
}