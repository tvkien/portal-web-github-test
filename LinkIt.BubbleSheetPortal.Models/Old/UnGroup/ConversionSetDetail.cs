namespace LinkIt.BubbleSheetPortal.Models
{
    public class ConversionSetDetail
    {
        public int ConversionSetDetailID { get; set; }
        public int ConversionSetID { get; set; }
        public int Input1 { get; set; }
        public int Input2 { get; set; }
        public decimal ConvertedScore { get; set; }
        public string ConvertedScore_A { get; set; }
    }
}
