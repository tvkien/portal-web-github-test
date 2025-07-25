namespace LinkIt.BubbleSheetPortal.Models.DTOs.VirtualSection
{
    public class VirtualSectionDto
    {
        public int VirtualSectionId { get; set; }
        public int VirtualTestId { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public string SectionName { get { return string.IsNullOrWhiteSpace(Title) ? "Section " + Order.ToString() : Title; } }
    }
}
