namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGODataPointBand
    {
        public int SGODataPointBandID { get; set; }
        public int SGODataPointID { get; set; }
        public string Name { get; set; }
        public double LowValue { get; set; }
        public double HighValue { get; set; }
        public int AchievementLevel { get; set; }
    }
}