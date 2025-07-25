namespace LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker
{
    public class ScoreTypeListItem
    {
        public int Id { get; set; }
        public string ScoreTypeName { get; set; }
        public string ScoreName { get; set; }
        public string Description { get;set; }
        public string Overview { get; set; }
        public string ShortOverview { get; set; }
        public string ScoreTypeCode { get; set; }
        public int? VirtualTestCustomSubScoreID { get; set; }//subscoreid
        public bool? IsPredefinedList { get; set; }
        public string ShortScoreType { get; set; }
        public bool IsAutoCalculation { get; set; }
        public decimal MaxScore { get; set; }
        public int? VirtualTestCustomMetaDataID { get; set; }
    }
}
