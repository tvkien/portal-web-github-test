using LinkIt.BubbleSheetPortal.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Models.Survey
{
    public class GetUserResultsCriteria
    {
        public int? DistrictId { get; set; }
        public SurveyAssignmentTypeEnum Type { get; set; } = SurveyAssignmentTypeEnum.PublicAnonymous;
        public int? iDisplayLength { get; set; }
        public int? SchoolId { get; set; }
        public int? ClassId { get; set; }
        public int? TermId { get; set; }
        public int? TeacherId { get; set; }
        public string GradeIds { get; set; }
        public string ProgramIds { get; set; }
        public string Roles { get; set; }
    }
}
