using LinkIt.BubbleSheetPortal.Web.ViewModels.SATReport;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport
{
    public class SectionTagViewModel
    {
        public string SectionName { get; set; }
        public List<TagCategoryReportViewModel> TagCategoryReportViewModels { get; set; }
        public List<AnswerSectionViewModel> AnswerSectionViewModels { get; set; }
        public int DomainTagCategoryId { get; set; }
        public List<AnswerSectionGroupByPassageViewModel> AnswerSectionGroupByPassageViewModels { get; set; }
        public bool IsDisplayPassageNumber { get; set; }
    }
}