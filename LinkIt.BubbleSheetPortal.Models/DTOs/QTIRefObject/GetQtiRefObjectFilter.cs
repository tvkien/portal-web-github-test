namespace LinkIt.BubbleSheetPortal.Models.DTOs.QTIRefObject
{
    public class GetQtiRefObjectFilter : PaggingInfo
    {
        public int UserId { get; set; }
        public int? GradeId { get; set; }
        public string Subject { get; set; }
        public int? TextTypeId { get; set; }
        public int? TextSubTypeId { get; set; }
        public int? FleschKincaidId { get; set; }
        public string Name { get; set; }
        public int? PassageNumber { get; set; }
        public int? DistrictId { get; set; }
        public string QTIRefObjectIDs { get; set; }
        public string ExcludeQTIRefObjectIDs { get; set; }
        public string GeneralSearch { get; set; }
    }
}
