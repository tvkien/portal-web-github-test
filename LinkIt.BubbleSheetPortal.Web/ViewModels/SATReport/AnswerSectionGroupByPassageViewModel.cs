using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SATReport
{
    public class AnswerSectionGroupByPassageViewModel
    {
        public string PassageName { get; set; }
        public List<AnswerSectionViewModel> AnswerSectionViewModels { get; set; }
    }
}