namespace LinkIt.BubbleSheetPortal.Models
{
    public class FormBankCriteria
    {
        public int? GradeId { get; set; }
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public int UserRole { get; set; }
        public string SubjectName { get; set; }
        public int SubjectId { get; set; }
        public bool? IsFromMultiDate { get; set; }
        public bool UsingMultiDate { get; set; }
    }
}
