using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class TestAssignmentViewModel
    {
        public bool IsAdmin { get; set; }
        public bool CanSelectTeachers { get; set; }
        public bool IsTeacher { get; set; }
        public bool IsSchoolAdmin { get; set; }
        public bool IsPublisher { get; set; }
        public int DistrictId { get; set; }

        public bool IsNetworkAdmin { get; set; }
        public bool IsDistrictAdmin { get; set; }
        public List<int> ListDistricIds { get; set; }
        public bool IsShowTutorialMode { get; set; }
        public bool UseRostersAtTimeOfTestTaking { get; set; }
        public bool EnableStudentLevelAssignment { get; set; }
        public string UseRostersAtTimeOfTestTakingWording { get; set; }
        public string StrIds
        {
            get
            {
                var ids = string.Empty;
                if (this.ListDistricIds == null || !this.ListDistricIds.Any())
                {
                    return ids;
                }
                ids = this.ListDistricIds.Aggregate(ids, (current, id) => current + (id + ","));
                return ids.TrimEnd(new[] { ',' });
            }
        }

        public bool IsLaunchTeacherLedTest { get; set; }
    }
}
