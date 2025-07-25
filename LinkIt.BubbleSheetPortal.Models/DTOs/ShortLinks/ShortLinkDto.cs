namespace LinkIt.BubbleSheetPortal.Models.DTOs
{
    public class ShortLinkDto
    {
        public int ShortLinkID { get; set; }
        public string Code { get; set; }
        public string FullLink { get; set; }
        public int? QTITestClassAssignmentId { get; set; }
    }
}
