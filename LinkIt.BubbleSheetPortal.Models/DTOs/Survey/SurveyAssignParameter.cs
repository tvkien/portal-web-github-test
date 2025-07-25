using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.Survey
{
    public class SurveyAssignParameter
    {
        public List<AssignCodeFullLink> AssignCodeFullLinks { get; set; }
        public int SurveyId { get; set; }
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }
        public int SurveyAssignmentType { get; set; }
        public string Preferences { get; set; }
    }

    public class AssignCodeFullLink
    {
        public string Code { get; set; }
        public string ShortLinkKey { get; set; }
        public string FullLink { get; set; }
        public string Email { get; set; }
        public int UserId { get; set; }
    }

    public class AssignResult
    {
        public string Email { get; set; }
        public int RoleId { get; set; }
        public int QTITestClassAssignmentId { get; set; }
    }
}
