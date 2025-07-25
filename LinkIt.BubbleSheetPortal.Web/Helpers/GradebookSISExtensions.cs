using LinkIt.BubbleSheetPortal.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public static class GradebookSISExtensions
    {
        static int[] _exportTypesThatShowExportTypeOptions = new int[] { (int)GradebookSIS.Canvas, (int)GradebookSIS.Skyward };
        public static bool DoesShowExportScoreTypeOption(this IEnumerable<int> gradebookSISs)
        {
            return gradebookSISs.Any(c => _exportTypesThatShowExportTypeOptions.Contains(c));
        }
    }
}
