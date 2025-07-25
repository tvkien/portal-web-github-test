using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class SGOObjectViewModel
    {
        public string StrName { get; set; }
        public string StrStartDate { get; set; }
        public string StrEndDate { get; set; }

        public int SGOId { get; set; }

        public List<ListItemsViewModel> ListFullGrades { get; set; }

        public List<int> ListGradeSelected { get; set; }
        public string ListGradeIds { get; set; }

        public int DefaultWeek { get; set; }

        public int PermissionAccess { get; set; }

        public string Feedback { get; set; }

        public string AdminReviewDirections { get; set; }

        public bool IsReviewer { get; set; }

        public bool IsUnstructuredSgo { get; set; }
        public int? AssosiatedSchoolId { get; set; }

        public SGOObjectViewModel()
        {
            ListFullGrades = new List<ListItemsViewModel>();
            ListGradeSelected = new List<int>();
        }
    }
}