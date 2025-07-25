using LinkIt.BubbleSheetPortal.Models.DTOs;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.TestResultRemover
{
    public class CategoriesGradesAndSubjectsModel
    {
        public List<SelectListItemDTO> Categories { get; set; }
        public List<SelectListItemDTO> Grades { get; set; }
        public List<SelectListItemDTO> Subjects { get; set; }
    }
}
