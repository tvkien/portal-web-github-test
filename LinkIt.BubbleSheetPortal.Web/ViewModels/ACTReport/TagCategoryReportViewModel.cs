using LinkIt.BubbleSheetPortal.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport
{
    public class TagCategoryReportViewModel
    {
        public int TagCategoryId { get; set; }
        public string TagCategoryName { get; set; }
        public string TagCategoryDescription { get; set; }
        public List<SingleTagReportViewModel> SingleTagReportViewModels { get; set; }
        public bool IsTechniqueCategory { get { return TagCategoryDescription.Trim().Equals("technique", StringComparison.CurrentCultureIgnoreCase); } }

        public bool IsShowTechnicalCategory
        {
            get
            {
                if (IsTechniqueCategory)
                {
                    return SingleTagReportViewModels.Any(x => x.IncorrectAnswer > 0 || x.BlankAnswer > 0);
                }
                return true;
            }
        }

        public ACTPresentationType? PresentationType { get; set; }
        public int? Order { get; set; }
    }
}