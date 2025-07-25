using System.Collections.Generic;
using System.Web.Mvc;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.GenericSheets
{
    public class AssignGenericSheetActSatViewModel : ValidatableEntity<AssignGenericSheetActSatViewModel>
    {
        public int BubbleSheetFileId { get; set; }
        public string FileName { get; set; }
        public string ImageUrl { get; set; }

        public bool IsAllStudentsChecked { get; set; }
        
        public int BubbleSheetId { get; set; }
        public int? ClassId { get; set; }
        public int SelectClass { get; set; }
        public int SelectStudent { get; set; }

        public int VirtualTestSubTypeId { get; set; }
        
        public IEnumerable<SelectListItem> Classes { get; set; }

        public AssignGenericSheetActSatViewModel()
        {
        }
    }
}