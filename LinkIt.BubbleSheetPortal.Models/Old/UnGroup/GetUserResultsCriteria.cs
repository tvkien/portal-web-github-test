using LinkIt.BubbleSheetPortal.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.Old.UnGroup
{
    public class GetUserResultsCriteria
    {
        public int? DistrictId { get; set; }
        public int? DistrictTermId { get; set; }
        public int? SurveyId { get; set; }
        public int? BankId { get; set; }
        public SurveyAssignmentTypeEnum Type { get; set; } = SurveyAssignmentTypeEnum.PublicAnonymous;
        public int? TermId { get; set; }
        public int? SchoolId { get; set; }
        public int? TeacherId { get; set; }
        public int? ClassId { get; set; }
        public string ProgramIds { get; set; }
        public string GradeIds { get; set; }
        public string Roles { get; set; }
        public int? PageSize { get; set; }
        public int? StartIndex { get; set; }
        public List<string> SearchInBox { get; set; }
        public string SearchableColumns { get; set; }
        public string SortableColumns { get; set; }
    }
}
