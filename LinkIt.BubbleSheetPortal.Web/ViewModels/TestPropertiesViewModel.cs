using LinkIt.BubbleSheetPortal.Web.Models.RestrictionDTO;
using System;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class TestPropertiesViewModel
    {
        public string TestName { get; set; }
        public int TestId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public string CreatedBy { get; set; }

        public int TotalQuestion { get; set; }
        public int TotalTestResult { get; set; }
        public DateTime EarliestResultDate { get; set; }
        public DateTime MostRecentResultDate { get; set; }

        public bool IsShowTemplate { get; set; }
        public int? TemplateId { get; set; }
        public string TemplateName { get; set; }
        public bool IsAuthor { get; set; }
        public int? TotalPointsPossible { get; set; }
        public string Instruction { get; set; }
        public RestrictionAccessModel RestrictionAccessList { get; set; }
        public int? DataSetCategoryID { get; set; }
        public bool HasRetakeRequest { get; set; }

        public TestPropertiesViewModel()
        {
            RestrictionAccessList = new RestrictionAccessModel();
        }
    }
}
