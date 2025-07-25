using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.SGO
{
    public static class SGOHelper
    {
        public static int GetDataPointScoreType(SGODataPoint sgoDataPoint)
        {
            var scoreType = (int)SGOScoreTypeEnum.ScoreRaw; //default scoreType is scoreRaw
            if ( (sgoDataPoint.Type == (int)SGODataPointTypeEnum.PreAssessmentHistorical ||
                 sgoDataPoint.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom ||
                 sgoDataPoint.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom )
                && sgoDataPoint.ScoreType.HasValue)
            {
                scoreType = sgoDataPoint.ScoreType.Value;
            }
            return scoreType;
        }
    }
}