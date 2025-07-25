using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport
{
    public class DistinctSingleTagReportViewModelComparer : IEqualityComparer<SingleTagReportViewModel>
    {
        public bool Equals(SingleTagReportViewModel x, SingleTagReportViewModel y)
        {
            return x.TagName == y.TagName
                   && x.TotalAnswer == y.TotalAnswer
                   && x.CorrectAnswer == y.CorrectAnswer
                   && x.BlankAnswer == y.BlankAnswer
                   && x.Percent == y.Percent
                   && x.HistoricalAverage == y.HistoricalAverage;
        }

        public int GetHashCode(SingleTagReportViewModel obj)
        {
            string tagName = string.IsNullOrEmpty(obj.TagName) ? string.Empty : obj.TagName;
            return tagName.GetHashCode() ^
                   obj.TotalAnswer.GetHashCode() ^
                   obj.CorrectAnswer.GetHashCode() ^
                   obj.BlankAnswer.GetHashCode() ^
                   obj.Percent.GetHashCode() ^
                   obj.HistoricalAverage.GetHashCode();
        }
    }
}