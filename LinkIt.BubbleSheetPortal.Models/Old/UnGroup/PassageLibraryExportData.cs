namespace LinkIt.BubbleSheetPortal.Models
{
    public class PassageLibraryExportData
    {
        public string PassageNumber { get; set; }
        public string PassageName { get; set; }
        public string Grade { get; set; }
        public string Subject { get; set; }
        public string TextType { get; set; }
        public string TextSubType { get; set; }
        public string FleschKinkaid { get; set; }
        public string AssociatedQTIItems { get; set; }
        public int QTICount { get; set; }
        public string MediaResources { get; set; }
        public string Fullpath { get; set; }
    }
}
