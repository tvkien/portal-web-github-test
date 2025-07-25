using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class StudentPreferencesSearchModel
    {
        public int? DistrictID { get; set; }
        public int? SchoolID { get; set; }
        public int? UserID { get; set; }
        public int? VirtualTestID { get; set; }
    }
}