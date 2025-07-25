using System;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOExportData
    {
        public string Name { get; set; }
        public decimal? TargetScore { get; set; }
        public DateTime? FinalSignoffDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SISID { get; set; }
        public string LocalCode { get; set; }
        public string StateId { get; set; }
        public string SchoolName { get; set; }
        public int UserID { get; set; }
        public string ScoreCustom { get; set; }
        public int TargetScoreType { get; set; }
        public int Type { get; set; }
    }
}