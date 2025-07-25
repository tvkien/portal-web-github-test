using LinkIt.BubbleSheetPortal.Common.Enum;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.AssignSurvey
{
    public class SurveyAssignmentViewModel
    {
        public bool IsAdmin { get; set; }
        public bool CanSelectTeachers { get; set; }
        public bool IsSchoolAdmin { get; set; }
        public bool IsPublisher { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        

        public int? SurveyBankId { get; set; }
        public int? SurveyId { get; set; }
        public int? TermId { get; set; }
        public int? SurveyAssignmentType { get; set; }

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
