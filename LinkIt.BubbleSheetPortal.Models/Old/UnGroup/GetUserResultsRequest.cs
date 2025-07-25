using LinkIt.BubbleSheetPortal.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class GetUserResultsRequest
    {
        public int? DistrictId { get; set; }
        public SurveyAssignmentTypeEnum Type { get; set; } = SurveyAssignmentTypeEnum.PublicAnonymous;
        public int? SchoolId { get; set; }
        public int? ClassId { get; set; }
        public int? TermId { get; set; }
        public int? TeacherId { get; set; }
        public string GradeIds { get; set; }
        public string ProgramIds { get; set; }
        public string Roles { get; set; }
        public int? PageSize { get; set; }
        public int? StartIndex { get; set; }
        public string SearchText { get; set; }
        public string SortBy { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}
