using System.Collections.Generic;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class VirtualTestPropertiesViewModel
    {
        public int VirtualTestId { get; set; }

        public string Name { get; set; }

        public string Instruction { get; set; }

        public int? TestScoreMethodID { get; set; } = -1;

        public int? VirtualTestSubTypeID { get; set; }

        public bool IsBranchingTest { get; set; }

        public bool IsTeacherLed { get; set; }

        public List<SelectListItem> AvailableScoringMethods { get; set; } = new List<SelectListItem>();

        public bool IsSectionBranchingTest { get; set; } = false;

        public int? TotalPointsPossible { get; set; }

        public string BasicSciencePaletteSymbol { get; set; }

        public bool IsCustomItemNaming { get; set; }

        public bool IsNumberQuestions { get; set; }
        public List<SelectListItem> AvailableNavigationMethods { get; set; } = new List<SelectListItem>();
        public int? NavigationMethodID { get; set; }

        public int? CurrentNavigationMethodID { get; set; }

        public bool IsOverwriteTestResults { get; set; }

        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        public int? DatasetCategoryID { get; set; } = -1;
        public bool IsSurvey { get; set; }
        public bool HasRetakeRequest { get; set; }
    }
}
