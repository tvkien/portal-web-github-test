using System;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGODataPoint
    {
        public int SGODataPointID { get; set; }
        public string Name { get; set; }
        public string SubjectName { get; set; }
        public int GradeID { get; set; }
        public int? VirtualTestID { get; set; }
        public string AttachScoreUrl { get; set; }
        public string AttachScoreDownload { get; set; }
        public int SGOID { get; set; }
        public double Weight { get; set; }
        public decimal TotalPoints { get; set; }
        public int Type { get; set; }
        public int? AchievementLevelSettingID { get; set; }
        public DateTime? ResultDate { get; set; }
        public string RationaleGuidance { get; set; }
        public int? ScoreType { get; set; }
        public int ImprovementBasedDataPoint { get; set; }
        public int DataSetCategoryID { get; set; }

        //Custom property
        public bool IsCustomCutScore { get; set; }
        public int UserID { get; set; }
        public int? VirtualTestCustomScoreId { get; set; }
        public bool IsTemporary { get; set; }
    }
}
