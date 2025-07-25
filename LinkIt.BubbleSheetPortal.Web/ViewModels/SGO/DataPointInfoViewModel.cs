using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class DataPointInfoViewModel
    {
        public int SGODataPointID { get; set; }
        public string Name { get; set; }
        public int SGOID { get; set; }
        public double Weight { get; set; }
        public double WeightPercent { get; set; }

        public bool IsCustomCutScore { get; set; }
        public int TotalStudentHasScore { get; set; }
        public int TotalQuestions { get; set; }
        public string Tooltip { get; set; }
        public int Type { get; set; }
        public int ScoreType { get; set; }
        public int VirtualTestSubScoreId { get; set; }

        public bool EnableDefaultCutScore
        {
            get
            {
                if( (Type == (int) SGODataPointTypeEnum.PreAssessmentCustom)
                   && (ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA1 || ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA2))
                {
                    return false;
                }
                return true;
            }
        }

        public List<ListItemStr> ScoreTypesList { get; set; }
    }
}