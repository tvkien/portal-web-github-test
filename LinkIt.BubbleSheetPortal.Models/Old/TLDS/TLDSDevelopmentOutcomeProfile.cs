namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSDevelopmentOutcomeProfile
    {
        public int DevelopmentOutcomeProfileID { get; set; }
        public int ProfileID { get; set; }
        public int DevelopmentOutcomeTypeID { get; set; }
        public string DevelopmentOutcomeContent { get; set; }
        public string StrategyContent { get; set; }

        public string OriginalFileName { get; set; }
        public string S3FileName { get; set; }
    }
}
