using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class SecurityPreferenceViewModel
    {
        public bool IsAdmin { get { return CurrentRoleId == (int)Permissions.DistrictAdmin || CurrentRoleId == (int)Permissions.NetworkAdmin; } }
        public bool IsSchoolAdminOrTeacher { get { return CurrentRoleId == (int)Permissions.SchoolAdmin || CurrentRoleId == (int)Permissions.Teacher; } }
        public bool IsPublisher { get { return CurrentRoleId == (int)Permissions.Publisher; } }
        public int CurrentDistrictId { get; set; }
        public bool IsNetworkAdmin { get; set; }
        public List<int> ListDistricIds { get; set; }
        public int CurrentRoleId { get; set; }
        public bool IsSchoolAdmin { get { return CurrentRoleId == (int)Permissions.SchoolAdmin; } }
        public bool IsTeacher { get { return CurrentRoleId == (int)Permissions.Teacher; } }
    }
}
