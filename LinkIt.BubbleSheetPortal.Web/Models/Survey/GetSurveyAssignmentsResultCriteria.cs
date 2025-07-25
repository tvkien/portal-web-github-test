using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Web.Models.DataTable;

namespace LinkIt.BubbleSheetPortal.Web.Models.Survey
{
    public class GetSurveyAssignmentsResultCriteria
    {
        public int? DistrictId { get; set; }
        public bool ShowInActive { get; set; }
        public string Code { get; set; }
        public int? DistrictTermId { get; set; }
        public int? SurveyId { get; set; }
        public int? BankId { get; set; }
        public SurveyAssignmentTypeEnum Type { get; set; } = SurveyAssignmentTypeEnum.PublicAnonymous;
        public int? iDisplayLength { get; set; }
    }
}
