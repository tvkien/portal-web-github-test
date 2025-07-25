using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.ACTReport;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport
{
    public class SingleTagReportViewModel
    {
        public string TagName { get; set; }
        public string TagNameForOrder { get; set; }
        public int TotalAnswer { get; set; }
        public int CorrectAnswer { get; set; }
        public int IncorrectAnswer { get; set; }
        public int BlankAnswer { get; set; }
        public int Percent { get; set; }
        public int HistoricalAverage { get; set; }
        public List<ACTAnswerSectionData> ListAnswerInTag { get; set; }
        public int? Order { get; set; }
    }
}