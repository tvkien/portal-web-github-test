using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
	public class PrintStudentLoginViewModel
	{
        public IEnumerable<LookupStudent> Students { get; set; }
    }
}
