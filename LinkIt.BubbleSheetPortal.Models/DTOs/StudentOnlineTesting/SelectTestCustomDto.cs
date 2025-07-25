namespace LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting
{
    public class SelectTestCustomDto
    {
        public int CurrentVirtualTestID { get; set; }
        public int VirtualTestID { get; set; }        
        public string GradeName { get; set; }
        public string SubjectName { get; set; }
        public string BankName { get; set; }
        public string TestName { get; set; }
        public int DistrictID { get; set; }
    }
}
