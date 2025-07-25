using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.AssignSurvey
{
    public class SurveyGenerationInputViewModel
    {
        public int SchoolId { get; set; }
        public int SurveyId { get; set; }
        public string Code { get; set; }
        public int? DistrictTermId { get; set; }
        public int ClassId { get; set; } = 0;
        public int? DistrictId { get; set; }
    }
}
