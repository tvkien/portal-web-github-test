using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker.EnterResult
{
    public class ResultEntrySaveResultModel
    {
        public bool IsSaveForPublish { get; set; }

        public IList<ResultEntrySaveScoreModel> Scores { get; set; }
    }
}
