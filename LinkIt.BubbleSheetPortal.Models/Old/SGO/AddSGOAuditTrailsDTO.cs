namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class AddSGOAuditTrailsDTO
    {
        public int? SGOID { get; set; }
        public int? SGOActionTypeID { get; set; }
        public string ActionDetail { get; set; }

        public int? ChangedByUserID { get; set; }
    }
}
