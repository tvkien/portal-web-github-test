namespace LinkIt.BubbleSheetPortal.Models.DTOs.QTIRefObject
{
    public class GetQtiRefObjectFilterRequest : GenericDataTableRequest
    {
        public string NameSearch { get; set; }
        public int? GradeId { get; set; }
        public string Subject { get; set; }
        public int? TextTypeId { get; set; }
        public int? TextSubTypeId { get; set; }
        public int? FleschKincaidId { get; set; }
        public int? PassageNumber { get; set; }
        public int? DistrictId { get; set; }
        public string QtiItemIdString { get; set; }
        public string AssignedObjectIdList { get; set; }
    }
}
