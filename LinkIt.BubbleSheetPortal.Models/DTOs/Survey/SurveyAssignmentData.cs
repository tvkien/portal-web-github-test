using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.Survey
{
    public class SurveyAssignmentData
    {
        public int TestId { get; set; }
        public int DistrictId { get; set; }
        public int DistrictTermId { get; set; }
        public int UserId { get; set; }
        public string TestName { get; set; }
        public int NumberOfCode { get; set; }
        public List<string> Emails { get; set; }
        public List<int> AssignUserIds { get; set; }
        public int SurveyAssignmentType { get; set; }
    }
    public class DistributeSetting
    {
        public string TestCodePrefix { get; set; }
        public string DistributedBy { get; set; }
        public string HTTPProtocol { get; set; }
    }
}
