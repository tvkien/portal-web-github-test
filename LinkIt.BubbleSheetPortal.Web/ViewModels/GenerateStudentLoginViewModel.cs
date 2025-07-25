using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
	public class GenerateStudentLoginViewModel
	{
        public int? DistrictId { get; set; }
        public int? SchoolId { get; set; }
        public List<int> StudentIds { get; set; }
	}
}
