namespace LinkIt.BubbleSheetPortal.Models
{
    public class VirtualTestConversion
    {
        public int VirtualTestConversionID { get; set; }
        public string Name { get; set; }
        public int ConversionSetID { get; set; }
        public int VirtualTestID { get; set; }
        public int TargetType { get; set; }
        public string TargetIDs { get; set; }
    }
}