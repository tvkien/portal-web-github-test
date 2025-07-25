using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class SGOStudentScoreInDataPointViewModel
    {
        public int StudentID { get; set; }
        public decimal? Score { get; set; }
        public decimal TotalScore { get; set; }
        public string Color { get; set; }
        public int DataPointBandID { get; set; }
        public int TotalQuestions { get; set; }

        public decimal ScorePercent { get; set; }
        public int AchievementLevel { get; set; }
        public string ScoreText { get; set; }
        public bool IsCustomLabelValue { get; set; }
    }

    public class SGOStudentScoreViewModel
    {
        public int SGODataPointID { get; set; }
        public int Type { get; set; }
        public List<SGOStudentScoreInDataPointViewModel> StudentScoreInDataPoints { get; set; }
    }
}